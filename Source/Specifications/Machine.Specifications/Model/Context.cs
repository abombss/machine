using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class Context
  {
    readonly List<Specification> _specifications;
    readonly object _instance;
    readonly Subject _subject;
    readonly IEnumerable<Establish> _beforeEachs;
    readonly IEnumerable<Establish> _beforeAlls;
    readonly Because _because;
    readonly IEnumerable<Cleanup> _afterEachs;
    readonly IEnumerable<Cleanup> _afterAlls;
    public string Name { get; private set; }
    public bool IsIgnored { get; private set; }

    public object Instance
    {
      get { return _instance; }
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public Type Type { get; private set; }

    public Subject Subject
    {
      get { return _subject; }
    }

    public bool HasBecauseClause
    {
      get { return _because != null; }
    }

    public Result CriticalContextFailure
    {
      get; private set;
    }

    public Context(Type type, object instance, IEnumerable<Establish> beforeEachs,
      IEnumerable<Establish> beforeAlls, Because because, IEnumerable<Cleanup> afterEachs,
      IEnumerable<Cleanup> afterAlls, Subject subject, bool isIgnored)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
      _instance = instance;
      _afterAlls = afterAlls;
      _afterEachs = afterEachs;
      _beforeAlls = beforeAlls;
      _because = because;
      _beforeEachs = beforeEachs;
      _specifications = new List<Specification>();
      _subject = subject;
      IsIgnored = isIgnored;
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public IEnumerable<Result> VerifyAllSpecifications()
    {
      return EnumerateAndVerifyAllSpecifications().Where(x => x.Result != null).Select(x => x.Result).ToList();
    }

    public IEnumerable<SpecificationVerificationIteration> EnumerateAndVerifyAllSpecifications()
    {
      if (!Specifications.Any()) yield break;

      bool hasRunnableSpecifications = Specifications.Where(x => !x.IsIgnored && x.IsDefined).Any();

      if (hasRunnableSpecifications)
      {
        RunContextBeforeAll();
      }

      Result result = null;
      Specification current = null;
      foreach (var next in Specifications)
      {
        yield return new SpecificationVerificationIteration(current, result, next);
        current = next;
        result = VerifyOrIgnoreSpecification(current);
      }

      yield return new SpecificationVerificationIteration(current, result, null);

      if (hasRunnableSpecifications)
      {
        RunContextAfterAll();
      }

      CriticalContextFailure = null;
    }

    private Result VerifyOrIgnoreSpecification(Specification specification)
    {
      if (specification.IsIgnored)
      {
        return Result.Ignored();
      }
      else if (!specification.IsDefined)
      {
        return Result.NotImplemented();
      }
      else if (CriticalContextFailure != null)
      {
        return CriticalContextFailure;
      }
      else
      {
        return InternalVerifySpecification(specification);
      }
    }

    public Result VerifySpecification(Specification specification)
    {
      RunContextBeforeAll();

      var result = InternalVerifySpecification(specification);
      
      RunContextAfterAll();

      if (result.Passed && CriticalContextFailure != null)
      {
        result = CriticalContextFailure;
      }

      CriticalContextFailure = null;

      return result;
    }
    
    private Result InternalVerifySpecification(Specification specification)
    {
      VerificationContext context = new VerificationContext(_instance);
      try
      {
        _beforeEachs.InvokeAll();
        _because.InvokeIfNotNull();
      }
      catch (Exception err)
      {
        return Result.ContextFailure(err);
      }

      var result = specification.Verify(context);

      try
      {
        _afterEachs.InvokeAll();
      }
      catch (Exception err)
      {
        if (result.Passed)
        {
          return Result.ContextFailure(err);
        }
        return result;
      }

      return result;
    }

    public void RunContextBeforeAll()
    {
      try
      {
        _beforeAlls.InvokeAll();
      }
      catch (Exception err)
      {
        CriticalContextFailure = Result.ContextFailure(err);
      }
    }

    public void RunContextAfterAll()
    {
      try
      {
        _afterAlls.InvokeAll();
      }
      catch (Exception err)
      {
        CriticalContextFailure = Result.ContextFailure(err);
      }
    }

    // TODO: Rename to Name
    public string FullName
    {
      get
      {
        string line = "";

        if (Subject != null)
        {
          line += Subject.FullConcern + ", ";
        }

        return line + Name;
      }
    }
  }

  public class SpecificationVerificationIteration
  {
    public Specification Next { get; private set; }
    public Specification Current { get; private set; }
    public Result Result { get; private set; }

    public SpecificationVerificationIteration(Specification current, Result result, Specification next)
    {
      Next = next;
      Current = current;
      Result = result;
    }
  }
}