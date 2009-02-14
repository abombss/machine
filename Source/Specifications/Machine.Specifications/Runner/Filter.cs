using System;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public abstract class Filter : MarshalByRefObject, ISpecificationFilter
  {
    public static readonly Filter Empty = new EmptyFilter();

    public abstract bool Exclude(SpecificationInfo specification);

    [Serializable]
    private class EmptyFilter : Filter
    {
      public override bool Exclude(SpecificationInfo specification)
      {
        return false;
      }
    }
  }
}