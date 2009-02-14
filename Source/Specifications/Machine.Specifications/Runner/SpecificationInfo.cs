using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class SpecificationInfo
  {
    public string Name { get; private set; }

    public string FullName
    {
      get
      {
        return Context.FullName + "/" + Name;
      }
    }

    public ContextInfo Context { get; private set; }

    public SpecificationInfo(ContextInfo context, string name)
    {
      Context = context;
      Name = name;
    }
  }
}
