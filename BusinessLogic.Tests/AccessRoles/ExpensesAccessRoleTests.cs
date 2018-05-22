namespace BusinessLogic.Tests.AccessRoles
{
    using System;

    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;

    using Xunit;

    public class ExpensesAccessRoleTests
    {
        public class Ctor
        {
            [Fact]
            public void ValidArguments_Ctor_SetsProperties()
            {
                IAccessRole accessRole = new AccessRole(15, "name", "desc", new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection());
                ExpensesAccessRole sut = new ExpensesAccessRole(accessRole, 10, 11, true, true, true, true);

                Assert.Equal(10, sut.ClaimMaximumAmount);
                Assert.Equal(11, sut.ClaimMinimumAmount);
                Assert.True(sut.CanEditCostCode);
                Assert.True(sut.CanEditDepartment);
                Assert.True(sut.CanEditProjectCode);
                Assert.True(sut.MustHaveBankAccount);
            }
        }
    }
}