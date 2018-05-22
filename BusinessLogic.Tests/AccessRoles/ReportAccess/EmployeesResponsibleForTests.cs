namespace BusinessLogic.Tests.AccessRoles.ReportsAccess
{
    using BusinessLogic.AccessRoles.ReportsAccess;

    using Xunit;

    public class EmployeesResponsibleForTests
    {
        [Fact]
        public void Ctor_WithValidArguments_SetsLevel()
        {
            EmployeesResponsibleFor sut = new EmployeesResponsibleFor();

            Assert.Equal(ReportingAccess.EmployeesResponsibleFor, sut.Level);
        }
    }
}
