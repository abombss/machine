// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using System;
using System.IO;
using System.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Framework.Utilities;
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Machine.Specifications.GallioAdapter.Tests
{

  /// <summary>
  /// Used together with <see cref="BaseTestWithSampleRunner" /> to specify
  /// a sample test to run.
  /// </summary>
  [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
  public class RunSampleAttribute : Attribute
  {
    public RunSampleAttribute(Type fixtureType)
    {
      if (fixtureType == null)
        throw new ArgumentNullException("fixtureType");

      FixtureType = fixtureType;
    }

    public RunSampleAttribute(Type fixtureType, string methodName)
    {
      if (fixtureType == null)
        throw new ArgumentNullException("fixtureType");
      if (methodName == null)
        throw new ArgumentNullException("methodName");

      FixtureType = fixtureType;
      MethodName = methodName;
    }

    public Type FixtureType { get; private set; }
    public string MethodName { get; private set; }
  }

  public abstract class BaseTestWithSampleRunner
  {
    private readonly SampleRunner runner = new SampleRunner();
    private static bool isSampleRunning;
    IDisposable runtime;

    /// <summary>
    /// Gets the sample runner for the fixture.
    /// </summary>
    public SampleRunner Runner
    {
      get { return runner; }
    }

    /// <summary>
    /// Gets the report for the tests that ran.
    /// </summary>
    public Report Report
    {
      get { return Runner.Report; }
    }

    [TearDown]
    public void TearDown()
    {
      runtime.Dispose();
    }

    [SetUp]
    public void RunDeclaredSamples()
    {
      var runtimeSetup = new RuntimeSetup()
      {
        ConfigurationFilePath = Path.Combine(
          AssemblyUtils.GetAssemblyLocalPath(Assembly.GetExecutingAssembly())
          , "Gallio.plugin")
      };
      runtime = RuntimeBootstrap.Initialize(runtimeSetup, new TextLogger(new StringWriter()));

      //// Protect the sample runner from re-entrance when we are asked to run
      //// samples that are defined in nested classes of the fixture.
      if (isSampleRunning)
        return;

      try
      {
        isSampleRunning = true;
        object[] attribs = GetType().GetCustomAttributes(typeof(RunSampleAttribute), true);
        if (attribs.Length != 0)
        {
          foreach (RunSampleAttribute attrib in attribs)
          {
            if (attrib.MethodName == null)
              runner.AddFixture(attrib.FixtureType);
            else
              runner.AddMethod(attrib.FixtureType, attrib.MethodName);
          }

          runner.Run();
        }
      }
      finally
      {
        isSampleRunning = false;
      }
    }

    protected static void AssertLogContains(TestStepRun run, string expectedOutput)
    {
      AssertLogContains(run, expectedOutput, TestLogStreamNames.Default);
    }

    protected static void AssertLogContains(TestStepRun run, string expectedOutput, string streamName)
    {
      Assert.That(run.TestLog.GetStream(streamName).ToString(), Text.Contains(expectedOutput));
    }

    protected static void AssertLogDoesNotContain(TestStepRun run, string expectedOutput)
    {
      AssertLogDoesNotContain(run, expectedOutput, TestLogStreamNames.Default);
    }

    protected static void AssertLogDoesNotContain(TestStepRun run, string expectedOutput, string streamName)
    {
      Assert.That(run.TestLog.GetStream(streamName).ToString(), Text.DoesNotContain(expectedOutput));
    }
  }
}