using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
  public class MachineSpecificationsTestFactory
  {
    public MachineSpecificationTest CreateTest(MachineContextTest contextTest, IFieldInfo specification)
    {
      var specificationTest = new MachineSpecificationTest(contextTest, specification);
      return specificationTest;
    }
  }
}