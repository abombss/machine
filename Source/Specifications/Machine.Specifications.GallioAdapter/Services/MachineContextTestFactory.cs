using Gallio.Model;
using Machine.Specifications.GallioAdapter.Model;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Services
{
  /// <summary>Factory for creating a <see cref="MachineSpecificationTest"/>.
  /// A factory is used to move out all the gross metadata stuff that needs to get done.</summary>
  public class MachineContextTestFactory
  {
    public MachineContextTest CreateTest(ITestComponent parent, Context context)
    {
      var test = new MachineContextTest(context);
      PopulateMetadata(parent, test);
      return test;
    }

    void PopulateMetadata(ITestComponent parent, MachineContextTest test)
    {
      Context context = test.Context;
      AddCategoryMetadataFromTags(test, context);

      if (context.Subject != null && context.Subject.Type != null)
      {
        test.Metadata.Add(MetadataKeys.TestsOn, context.Subject.Type.FullName);
      }

      if (context.Subject != null)
      {
        test.Metadata.Add(MetadataKeys.Description, context.Subject.FullConcern);
      }
    }

    void AddCategoryMetadataFromTags(ITestComponent test, Context context)
    {
      foreach (Tag tag in context.Tags)
      {
        test.Metadata.Add(MetadataKeys.CategoryName, tag.Name);
      }
    }
  }
}