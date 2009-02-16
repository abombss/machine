// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modified by and Portions Copyright 2008 Machine Project

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.GallioAdapter.Properties;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class MachineSpecificationController : BaseTestController
  {
    public override void Dispose()
    {
      base.Dispose();
    }

    /// <inheritdoc />
    protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep,
      TestExecutionOptions options, IProgressMonitor progressMonitor)
    {
      IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();
      using (
        progressMonitor.BeginTask(Resources.MachineSpecificationController_RunningMachineSpecifications,
          testCommands.Count))
      {
        if (progressMonitor.IsCanceled)
          return TestOutcome.Canceled;

        if (options.SkipTestExecution)
        {
          SkipAll(rootTestCommand, parentTestStep);
          return TestOutcome.Skipped;
        }
        else
        {
          using (var monitor = new RunMonitor(testCommands, parentTestStep, progressMonitor))
          {
            return monitor.Run();
          }
        }
      }
    }

    #region Nested type: RunMonitor

    class RunMonitor : ISpecificationRunListener, IDisposable
    {
      readonly List<Assembly> _assemblies;
      readonly IProgressMonitor _progressMonitor;
      readonly IList<ITestCommand> _testCommands;
      readonly Stack<ITestContext> _testContexts;
      readonly Dictionary<string, ITestCommand> _testsByName;
      readonly ITestStep _topMostStep;
      Result _worstResult;

      public RunMonitor(IList<ITestCommand> testCommands, ITestStep parentTestStep, IProgressMonitor progressMonitor)
      {
        _testCommands = testCommands;
        _topMostStep = parentTestStep;
        _progressMonitor = progressMonitor;
        _assemblies = new List<Assembly>();
        _testContexts = new Stack<ITestContext>();
        _testsByName = new Dictionary<string, ITestCommand>();
      }

      #region IDisposable Members

      public void Dispose()
      {
      }

      #endregion

      #region ISpecificationRunListener Members

      public void OnAssemblyStart(AssemblyInfo assembly)
      {
        HandleStart(assembly.Name);
      }

      public void OnAssemblyEnd(AssemblyInfo assembly)
      {
        HandleFinished(assembly.Name, null);
      }

      public void OnRunStart()
      {
      }

      public void OnRunEnd()
      {
      }

      public void OnContextStart(ContextInfo context)
      {
        HandleStart(context.Name);
      }

      public void OnContextEnd(ContextInfo context)
      {
        HandleFinished(context.Name, null);
      }

      public void OnSpecificationStart(SpecificationInfo specification)
      {
        HandleStart(specification.FullName);
      }

      public void OnSpecificationEnd(SpecificationInfo specification, Result result)
      {
        HandleFinished(specification.FullName, result);
      }

      public void OnFatalError(ExceptionResult exception)
      {
      }

      #endregion

      public TestOutcome Run()
      {
        _worstResult = Result.Pass();
        Filter filter = InitializeAndCreateFilter(_testCommands);

        var runner = new DefaultRunner(this, new RunOptions(filter));
        runner.RunAssemblies(_assemblies);
        return _worstResult.Status == Status.Passing ? TestOutcome.Passed : TestOutcome.Failed;
      }

      Filter InitializeAndCreateFilter(IEnumerable<ITestCommand> testCommands)
      {
        foreach (ITestCommand testCommand in testCommands)
        {
          if (testCommand.Test is MachineSpecificationTest)
          {
            var spec = testCommand.Test as MachineSpecificationTest;
            _testsByName.Add(spec.FriendlyId, testCommand);
          }
          else if (testCommand.Test is MachineContextTest)
          {
            var context = testCommand.Test as MachineContextTest;
            _testsByName.Add(context.FriendlyId, testCommand);
          }
          else if (testCommand.Test is MachineAssemblyTest)
          {
            var assemblyTest = testCommand.Test as MachineAssemblyTest;
            _assemblies.Add(assemblyTest.Assembly);
            _testsByName.Add(assemblyTest.FriendlyId, testCommand);
          }
        }

        return new NamedExclusionFilter(_testsByName.Keys);
      }

      void HandleStart(string name)
      {
        ITestCommand testCommand;
        if (!_testsByName.TryGetValue(name, out testCommand))
          return;

        _progressMonitor.SetStatus(testCommand.Test.Name);

        ITestStep parentTestStep = _testContexts.Count != 0 ? _testContexts.Peek().TestStep : _topMostStep;
        ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
        _testContexts.Push(testContext);

        testContext.LifecyclePhase = LifecyclePhases.Execute;
      }

      static TestOutcome CreateTestoutcome(Result result)
      {
        switch (result.Status)
        {
          case Status.Failing:
            return TestOutcome.Failed;
          case Status.Passing:
            return TestOutcome.Passed;
          case Status.NotImplemented:
            return TestOutcome.Pending;
          case Status.Ignored:
            return TestOutcome.Ignored;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      static void SetParentsOutcome(ITestContext context, TestOutcome outcome)
      {
        if (context != null)
        {
          context.SetInterimOutcome(outcome);
          SetParentsOutcome(context.Parent, outcome);
        }
      }

      void HandleFinished(string name, Result result)
      {
        if (_testContexts.Count == 0)
          return;

        ITestContext testContext = _testContexts.Peek();
        _testContexts.Pop();

        _progressMonitor.Worked(1);

        if (result != null)
        {
          string logStreamName = result.Passed ? TestLogStreamNames.Warnings : TestLogStreamNames.Failures;

          TestLogWriter logWriter = testContext.LogWriter;

          if (result.Exception != null)
          {
            using (logWriter[logStreamName].BeginSection("Stack Trace"))
            using (logWriter[logStreamName].BeginMarker(Marker.StackTrace))
              logWriter[logStreamName].Write(result.Exception.StackTrace);
          }

          if (!string.IsNullOrEmpty(result.ConsoleOut))
          {
            using (logWriter[TestLogStreamNames.ConsoleOutput].BeginSection("Console Output"))
            using (logWriter[TestLogStreamNames.ConsoleOutput].BeginMarker(Marker.Monospace))
              logWriter[TestLogStreamNames.ConsoleOutput].Write(result.ConsoleOut);
          }

          if (!string.IsNullOrEmpty(result.ConsoleError))
          {
            using (logWriter[TestLogStreamNames.ConsoleError].BeginSection("Error Output"))
            using (logWriter[TestLogStreamNames.ConsoleError].BeginMarker(Marker.Monospace))
              logWriter[TestLogStreamNames.ConsoleError].Write(result.ConsoleError);
          }

          testContext.FinishStep(CreateTestoutcome(result), null);
          if (!result.Passed)
          {
            SetParentsOutcome(testContext.Parent, TestOutcome.Failed);
          }
        }
        else
        {
          testContext.FinishStep(testContext.Outcome, null);
        }
      }
    }

    #endregion
  }
}