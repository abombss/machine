using System.Reflection;
using Gallio.Model;
using Gallio.Reflection;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>Represents a group of tests within a specific assembly.</summary>
  public class MachineAssemblyTest : MachineGallioTest
  {
    public MachineAssemblyTest(string name, IAssemblyInfo assembly)
      : base(name, assembly)
    {
      Kind = TestKinds.Assembly;
      ModelUtils.PopulateMetadataFromAssembly(assembly, Metadata);
    }

    public Assembly Assembly
    {
      get { return ReflectionUtils.GetAssembly(CodeElement).Resolve(false); }
    }
  }
}