using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Machine.Specifications.GallioAdapter.Services;

namespace Machine.Specifications.GallioAdapter
{
  public class MachineSpecificationsFramework : BaseTestFramework
  {
    private static readonly Guid ID = new Guid("{FC625EE0-9AB9-4212-98D9-CF497C3C6192}");

    public override Guid Id
    {
      get { return ID; }
    }

    public override string Name
    {
      get { return "Machine.Specifications"; }
    }

    public override ITestExplorer CreateTestExplorer(TestModel testModel)
    {
      return new MachineSpecificationsExplorer(testModel);
    }
  }
}
