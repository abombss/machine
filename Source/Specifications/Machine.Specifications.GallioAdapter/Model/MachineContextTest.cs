using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>Represents a typeical TestFixture</summary>
  public class MachineContextTest : MachineGallioTest
  {
    readonly Context _context;

    public MachineContextTest(Context context)
      : base(context.Name, Reflector.Wrap(context.Type))
    {
      Kind = TestKinds.Fixture;
      _context = context;
    }

    public Context Context
    {
      get { return _context; }
    }
  }
}