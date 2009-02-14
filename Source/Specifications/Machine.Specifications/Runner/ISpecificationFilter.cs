using System;

namespace Machine.Specifications.Runner
{
  public interface ISpecificationFilter
  {
    bool Exclude(SpecificationInfo specification);
  }
}