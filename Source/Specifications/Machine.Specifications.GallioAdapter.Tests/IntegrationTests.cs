using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Machine.Specifications.Example;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{

  [TestFixture][Ignore]
  [RunSample(typeof(when_transferring_between_two_accounts))]
  public class RunSimpleTest : BaseTestWithSampleRunner
  {
    [Test]
    public void PassTestPassed()
    {


      //TestStepRun run = Runner.GetPrimaryTestStepRun(
      //  CodeReference.CreateFromMember(typeof(when_transferring_between_two_accounts).GetField("should_debit_the_from_account_by_the_amount_transferred")));
      //Assert.AreEqual(TestStatus.Passed, run.Result.Outcome.Status);
    }

  }
}