using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class RunOptions : MarshalByRefObject
  {
    public IEnumerable<string> IncludeTags { get; set; }
    public IEnumerable<string> ExcludeTags { get; set; }
    public Filter Filter { get; set; }

    public RunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags)
      : this(Runner.Filter.Empty, includeTags, excludeTags)
    {
    }

    public RunOptions(Filter filter)
      : this(filter, new string[]{}, new string[]{})
    {
    }

    public RunOptions(Filter filter, IEnumerable<string> includeTags, IEnumerable<string> excludeTags)
    {
      Filter = filter;
      IncludeTags = includeTags;
      ExcludeTags = excludeTags;
    }

    public static RunOptions Default { get { return new RunOptions(Runner.Filter.Empty, new string[] {}, new string[] {}); } }
  }
}
