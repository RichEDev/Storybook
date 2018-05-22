namespace BusinessLogic.Tests.Employees.AccessRoles
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Employees.AccessRoles;

    using Xunit;

    public class EmployeeCombinedAccessRoleTests
    {
        [Fact]
        public void NullAssignedAccessRoles_Ctor_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmployeeCombinedAccessRole(null));
        }

        [Fact]
        public void ValidArgument_Ctor_SetsProperties()
        {
            // 1st access role - with claimant style scopes
            ApplicationScopeCollection applicationScopesClaimant = new ApplicationScopeCollection { new WebsiteAccess(), new MobileAccess() };
            AccessScopeCollection accessScopeCollectionClaimant = new AccessScopeCollection
                                                              {
                                                                  new AccessScope(ModuleElements.Allowances, true, true, true, true),
                                                                  new AccessScope(ModuleElements.Claims, true, true, true, true),
                                                                  new AccessScope(ModuleElements.Advances, false, false, false, false),
                                                                  new AccessScope(ModuleElements.Colours, false, false, false, false),
                                                                  new AccessScope(ModuleElements.NewExpenses, true, true, true, true)
                                                              };
            IAccessRole accessRoleClaimant = new AccessRole(53, "Claimant", "desc", applicationScopesClaimant, new EmployeesResponsibleFor(), accessScopeCollectionClaimant);

            // 2nd access role - with admin style scopes
            ApplicationScopeCollection applicationScopesAdmin = new ApplicationScopeCollection { new WebsiteAccess(), new MobileAccess(), new ApiAccess() };
            AccessScopeCollection accessScopeCollectionAdmin = new AccessScopeCollection
                                                              {
                                                                  new AccessScope(ModuleElements.Allowances, true, true, true, true),
                                                                  new AccessScope(ModuleElements.Claims, true, true, true, true),
                                                                  new AccessScope(ModuleElements.Advances, true, true, true, true),
                                                                  new AccessScope(ModuleElements.Colours, true, true, true, false),
                                                                  new AccessScope(ModuleElements.NewExpenses, true, true, true, true),
                                                                  new AccessScope(ModuleElements.AccessRoles, true, true, true, true)
                                                              };
            IAccessRole accessRoleAdmin = new AccessRole(54, "Admin", "desc", applicationScopesAdmin, new AllDataReportsAccess(), accessScopeCollectionAdmin);

            // 3rd access role - with scopes to try and wrongly override
            ApplicationScopeCollection applicationScopesBreaker = new ApplicationScopeCollection { new ApiAccess() };
            AccessScopeCollection accessScopeCollectionBreaker = new AccessScopeCollection
                                                                   {
                                                                       new AccessScope(ModuleElements.Allowances, false, false, false, false),
                                                                       new AccessScope(ModuleElements.Claims, false, false, false, false),
                                                                       new AccessScope(ModuleElements.Advances, false, false, false, false),
                                                                       new AccessScope(ModuleElements.Colours, false, false, false, false),
                                                                       new AccessScope(ModuleElements.NewExpenses, false, false, false, false),
                                                                       new AccessScope(ModuleElements.AccessRoles, false, false, false, false)
                                                                   };
            IAccessRole accessRoleBreaker = new AccessRole(55, "Breaker", "desc", applicationScopesBreaker, new EmployeesResponsibleFor(), accessScopeCollectionBreaker);

            // 4th access role - with scopes to try and wrongly override
            ApplicationScopeCollection applicationScopesBreaker2 = new ApplicationScopeCollection();
            AccessScopeCollection accessScopeCollectionBreaker2 = new AccessScopeCollection();
            IAccessRole accessRoleBreaker2 = new AccessRole(56, "Breaker", "desc", applicationScopesBreaker2, new SelectedAccessRoles(new[] { 5 }), accessScopeCollectionBreaker2);

            IEnumerable<IAccessRole> accessRoles = new List<IAccessRole> { accessRoleClaimant, accessRoleAdmin, accessRoleBreaker, accessRoleBreaker2 };

            EmployeeCombinedAccessRole sut = new EmployeeCombinedAccessRole(accessRoles);

            // ApplicationScopes
            Assert.Equal(3, sut.ApplicationScopes.Count);
            Assert.True(sut.ApplicationScopes.Contains(typeof(WebsiteAccess)));
            Assert.True(sut.ApplicationScopes.Contains(typeof(MobileAccess)));
            Assert.True(sut.ApplicationScopes.Contains(typeof(ApiAccess)));

            // ReportAccess
            Assert.IsType<AllDataReportsAccess>(sut.ReportsAccess);

            // Scopes
            Assert.True(sut.Scopes.CanAdd(ModuleElements.AccessRoles));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.AccessRoles));
            Assert.True(sut.Scopes.CanDelete(ModuleElements.AccessRoles));
            Assert.True(sut.Scopes.CanView(ModuleElements.AccessRoles));

            Assert.True(sut.Scopes.CanAdd(ModuleElements.NewExpenses));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.NewExpenses));
            Assert.True(sut.Scopes.CanDelete(ModuleElements.NewExpenses));
            Assert.True(sut.Scopes.CanView(ModuleElements.NewExpenses));

            Assert.True(sut.Scopes.CanAdd(ModuleElements.Colours));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.Colours));
            Assert.False(sut.Scopes.CanDelete(ModuleElements.Colours));
            Assert.True(sut.Scopes.CanView(ModuleElements.Colours));

            Assert.True(sut.Scopes.CanAdd(ModuleElements.Advances));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.Advances));
            Assert.True(sut.Scopes.CanDelete(ModuleElements.Advances));
            Assert.True(sut.Scopes.CanView(ModuleElements.Advances));

            Assert.True(sut.Scopes.CanAdd(ModuleElements.Claims));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.Claims));
            Assert.True(sut.Scopes.CanDelete(ModuleElements.Claims));
            Assert.True(sut.Scopes.CanView(ModuleElements.Claims));

            Assert.True(sut.Scopes.CanAdd(ModuleElements.Allowances));
            Assert.True(sut.Scopes.CanEdit(ModuleElements.Allowances));
            Assert.True(sut.Scopes.CanDelete(ModuleElements.Allowances));
            Assert.True(sut.Scopes.CanView(ModuleElements.Allowances));

            Assert.Equal("53,54,55,56", sut.Id);
        }
    }
}
