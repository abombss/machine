using Gallio;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Services;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineFrameworkTest : MachineGallioTest
  {
    public MachineFrameworkTest(string name, ICodeElementInfo codeElement)
      : base(name, codeElement)
    {
      Kind = TestKinds.Framework;
    }

    /// <summary>Create the test controller at the framework level because we can run
    /// multiple assemblies given a single AppDomainRunner.</summary>
    public override Func<ITestController> TestControllerFactory
    {
      get { return () => new MachineSpecificationController(); }
    }
  }
}