using Gallio.Model;
using Gallio.Reflection;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>A base class for wrapping machine specifications around gallio tests</summary>
  public abstract class MachineGallioTest : BaseTest
  {
    protected MachineGallioTest(string name, ICodeElementInfo codeElement)
      : base(name, codeElement)
    {
    }
  }
}