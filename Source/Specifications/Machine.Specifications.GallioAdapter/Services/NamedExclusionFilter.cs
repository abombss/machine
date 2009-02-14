using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner;

namespace Machine.Specifications.GallioAdapter.Services
{
  /// <summary>
  /// Filter for only running contexts and specifications provided from gallio.
  /// </summary>
  /// <remarks>Currently it only relies on names, so if there are duplicate names
  /// for assemblies, contexts, or specifications this could cause lots of trouble.</remarks>
  [Serializable]
  public class NamedExclusionFilter : Filter
  {
    readonly string[] _specsToRun;

    public NamedExclusionFilter()
    {
      _specsToRun = new List<string>().ToArray();
    }

    public NamedExclusionFilter(IEnumerable<string> specsToRun)
    {
      _specsToRun = new List<string>(specsToRun).ToArray();
    }

    public override bool Exclude(SpecificationInfo specification)
    {
      return !_specsToRun.Contains(specification.FullName);
    }
  }
}