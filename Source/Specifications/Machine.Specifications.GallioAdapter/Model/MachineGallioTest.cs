using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Services;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>A base class for wrapping machine specifications around gallio tests</summary>
  public abstract class MachineGallioTest : BaseTest
  {
    protected MachineGallioTest(string name, ICodeElementInfo codeElement)
      : base(name, codeElement)
    {
    }

    /// <summary>Id used to lookup tests from exploration during execution 
    /// in the <see cref="MachineSpecificationController.RunMonitor"/></summary>
    public abstract string FriendlyId { get; }
  }
}