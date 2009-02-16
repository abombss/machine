using System.Collections.Generic;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
  /// <summary>Factory for creating a <see cref="MachineSpecificationTest"/>.
  /// A factory is used to move out all the gross metadata stuff that needs to get done.</summary>
  public class MachineContextTestFactory
  {
    readonly IMachineContextMetadataBuilder _metadataBuilder;

    public MachineContextTestFactory(IMachineContextMetadataBuilder metadataBuilder)
    {
      _metadataBuilder = metadataBuilder;
    }

    public MachineContextTest CreateTest(ITest parent, ITypeInfo contextType)
    {
      var contextTest = new MachineContextTest(contextType, contextType.GetTags(), contextType.IsIgnored());
      _metadataBuilder.PopulateMetadata(parent, contextTest);
      return contextTest;
    }

    //void PopulateMetadata(ITestComponent parent, MachineContextTest test)
    //{
    //  AddCategoryMetadataFromTags(test, test.Tags);

    //  if (context.Subject != null && context.Subject.Type != null)
    //  {
    //    test.Metadata.Add(MetadataKeys.TestsOn, context.Subject.Type.FullName);
    //  }

    //  if (context.Subject != null)
    //  {
    //    test.Metadata.Add(MetadataKeys.Description, context.Subject.FullConcern);
    //  }
    //}


  }

  public interface IMachineContextMetadataBuilder
  {
    void PopulateMetadata(ITest parent, MachineContextTest contextTest);
  }

  public class ContextMetadataBuilder : IMachineContextMetadataBuilder
  {
    public virtual void PopulateMetadata(ITest parent, MachineContextTest contextTest)
    {
      AddCategoryMetadataFromTags(contextTest, contextTest.Tags);
    }

    private static void AddCategoryMetadataFromTags(ITestComponent test, IEnumerable<string> tags)
    {
      foreach (var tag in tags)
      {
        test.Metadata.Add(MetadataKeys.CategoryName, tag);
      }
    }
  }
}