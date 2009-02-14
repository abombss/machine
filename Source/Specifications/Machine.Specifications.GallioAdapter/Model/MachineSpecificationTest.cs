using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>An individual test case.</summary>
  public class MachineSpecificationTest : MachineGallioTest
  {
    readonly Specification _specification;

    public MachineSpecificationTest(Specification specification)
      : base(specification.Name, Reflector.Wrap(specification.FieldInfo))
    {
      Kind = TestKinds.Test;
      IsTestCase = true;
      _specification = specification;
    }

    public Specification Specification
    {
      get { return _specification; }
    }
  }
}