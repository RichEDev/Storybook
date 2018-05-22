namespace BusinessLogic.Tests.AccessRoles.ReportsAccess
{
    using BusinessLogic.AccessRoles.ReportsAccess;

    using Xunit;

    public class AllDataReportsAccessTests
    {
        [Fact]
        public void Ctor_WithValidArguments_SetsLevel()
        {
            AllDataReportsAccess sut = new AllDataReportsAccess();

            Assert.Equal(ReportingAccess.AllData, sut.Level);
        }
    }
}
