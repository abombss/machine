using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>An individual test case.</summary>
  public class MachineSpecificationTest : MachineGallioTest
  {
    readonly MachineContextTest _contextTest;
    readonly IFieldInfo _specificationFieldInfo;

    public MachineSpecificationTest(MachineContextTest contextTest, IFieldInfo specificationFieldInfo)
      : base(specificationFieldInfo.Name, specificationFieldInfo)
    {
      Kind = TestKinds.Test;
      IsTestCase = true;
      _contextTest = contextTest;
      _specificationFieldInfo = specificationFieldInfo;
    }

    public override string FriendlyId
    {
      get { return _contextTest.FriendlyId + "/" + _specificationFieldInfo.Name.ReplaceUnderscores(); }
    }
  }
}