using System.Collections.Generic;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.GallioAdapter.Model
{
  /// <summary>Represents a typeical TestFixture</summary>
  public class MachineContextTest : MachineGallioTest
  {
    readonly ITypeInfo _context;
    readonly IEnumerable<string> _tags;
    readonly bool _isIgnored;

    public MachineContextTest(ITypeInfo context, IEnumerable<string> tags, bool isIgnored)
      : base(context.Name, context)
    {
      _context = context;
      _tags = tags;
      _isIgnored = isIgnored;
      Kind = TestKinds.Fixture;
    }

    public IEnumerable<string> Tags
    {
      get { return _tags; }
    }

    public bool IsIgnored
    {
      get { return _isIgnored; }
    }

    public override string FriendlyId
    {
      get { return _context.Name.ReplaceUnderscores(); }
    }
  }
}