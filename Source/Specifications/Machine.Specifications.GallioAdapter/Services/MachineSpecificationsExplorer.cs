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
using System.Linq;
using System.Reflection;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.GallioAdapter.Properties;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class MachineSpecificationsExplorer : BaseTestExplorer
  {
    const string MachineSpecificationsAssemblyDisplayName = @"Machine.Specifications";
    readonly MachineContextTestFactory _contextTestFactory;

    public readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;
    public readonly Dictionary<Version, ITest> frameworkTests;
    public readonly Dictionary<ITypeInfo, ITest> typeTests;
    readonly MachineSpecificationsTestFactory _specificationTestFactory;

    public MachineSpecificationsExplorer(TestModel testModel)
      : base(testModel)
    {
      frameworkTests = new Dictionary<Version, ITest>();
      assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
      typeTests = new Dictionary<ITypeInfo, ITest>();
      _contextTestFactory = new MachineContextTestFactory(new ContextMetadataBuilder());
      _specificationTestFactory = new MachineSpecificationsTestFactory();
    }

    public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
    {
      //Debugger.Launch();
      Version frameworkVersion = GetFrameworkVersion(assembly);

      if (frameworkVersion != null)
      {
        ITest frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
        ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest, true);

        if (consumer != null)
          consumer(assemblyTest);
      }
    }

    static Version GetFrameworkVersion(IAssemblyInfo assembly)
    {
      AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MachineSpecificationsAssemblyDisplayName);
      return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
    }

    ITest GetFrameworkTest(Version frameworkVersion, ITest rootTest)
    {
      ITest frameworkTest;
      if (!frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
      {
        frameworkTest = CreateFrameworkTest(frameworkVersion);
        rootTest.AddChild(frameworkTest);
        frameworkTests.Add(frameworkVersion, frameworkTest);
      }
      return frameworkTest;
    }

    ITest GetAssemblyTest(IAssemblyInfo assembly, ITest frameworkTest, bool populateRecursively)
    {
      ITest assemblyTest;
      if (!assemblyTests.TryGetValue(assembly, out assemblyTest))
      {
        assemblyTest = CreateAssemblyTest(assembly);
        frameworkTest.AddChild(assemblyTest);

        assemblyTests.Add(assembly, assemblyTest);
      }

      if (populateRecursively)
      {
        PopulateAssemblyTest(assembly, assemblyTest);
      }

      return assemblyTest;
    }

    void PopulateAssemblyTest(IAssemblyInfo assembly, ITest assemblyTest)
    {
      assembly.GetTypes()
        .Where(type => type.IsContext())
        .ForEach(type =>
        {
          var contextTest = _contextTestFactory.CreateTest(assemblyTest, type);
          foreach (var fieldSpecificationInfo in type.GetSpecifications())
          {
            var specificationTest = _specificationTestFactory.CreateTest(contextTest, fieldSpecificationInfo);
            contextTest.AddChild(specificationTest);
          }
          assemblyTest.AddChild(contextTest);
        });

      //var explorer = new AssemblyExplorer();
      //IEnumerable<Context> contexts = explorer.FindContextsIn(assembly.Resolve(false));
      //foreach (Context context in contexts)
      //{
      //  MachineContextTest machineContextTest = _contextTestFactory.CreateTest(assemblyTest, context);
      //  assemblyTest.AddChild(machineContextTest);

      //  PopulateSpecificationTest(context, machineContextTest);
      //}
    }

    //void PopulateSpecificationTest(Context context, MachineContextTest test)
    //{
    //  foreach (Specification specification in context.Specifications)
    //  {
    //    test.AddChild(new MachineSpecificationTest(specification));
    //  }
    //}

    static ITest CreateFrameworkTest(Version frameworkVersion)
    {
      return
        new MachineFrameworkTest(
          String.Format(Resources.MachineSpecificationExplorer_FrameworkNameWithVersionFormat, frameworkVersion), null);
    }

    static ITest CreateAssemblyTest(IAssemblyInfo assembly)
    {
      var assemblyTest = new MachineAssemblyTest(assembly.Name, assembly);
      return assemblyTest;
    }
  }
}