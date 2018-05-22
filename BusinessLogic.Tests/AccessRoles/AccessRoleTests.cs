namespace BusinessLogic.Tests.AccessRoles
{
    using System;

    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts.Elements;

    using Xunit;

    public class AccessRoleTests
    {
        public class Ctor
        {
            [Fact]
            public void NullApplicationScopes_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new AccessRole(0, "name", "description", null, new AllDataReportsAccess(), new AccessScopeCollection()));
            }

            [Fact]
            public void NullAccessScopes_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new AccessRole(0, "name", "description", new ApplicationScopeCollection(), new AllDataReportsAccess(), null));
            }

            [Fact]
            public void NullReportAccess_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new AccessRole(0, "name", "description", new ApplicationScopeCollection(), null, new AccessScopeCollection()));
            }

            [Fact]
            public void ValidArguments_Ctor_SetsProperties()
            {
                AccessRole sut = new AccessRole(
                    5,
                    "name",
                    "description",
                    new ApplicationScopeCollection { new ApiAccess(), new MobileAccess() },
                    new AllDataReportsAccess(),
                    new AccessScopeCollection { new AccessScope(ModuleElements.AccessRoles, true, true, true, true) });

                Assert.Equal(5, sut.Id);
                Assert.Equal("name", sut.Name);
                Assert.Equal("description", sut.Description);

                Assert.Equal(1, sut.AccessScopes.Count);
                Assert.NotNull(sut.AccessScopes[ModuleElements.AccessRoles]);


                Assert.NotNull(sut.ApplicationScopes);
                Assert.True(sut.ApplicationScopes.Contains(typeof(ApiAccess)));
                Assert.True(sut.ApplicationScopes.Contains(typeof(MobileAccess)));

                Assert.NotNull(sut.ReportsAccess);
                Assert.IsType<AllDataReportsAccess>(sut.ReportsAccess);
            }
        }
    }
}
