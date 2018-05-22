namespace BusinessLogic.Tests.AccessRoles.Scopes
{
    using System;

    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts.Elements;

    using Xunit;

    public class AccessScopeCollectionTests
    {
        public class GrantAccess : AccessScopeCollectionFixture
        {
            [Fact]
            public void SetElement_GrantAccess_ReturnsCorrectly()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.GrantAccess(ModuleElements.ProjectCodes, ScopeType.View));
                Assert.False(sut.GrantAccess(ModuleElements.ProjectCodes, ScopeType.Add));
                Assert.False(sut.GrantAccess(ModuleElements.ProjectCodes, ScopeType.Edit));
                Assert.False(sut.GrantAccess(ModuleElements.ProjectCodes, ScopeType.Delete));

                Assert.True(sut.GrantAccess(ModuleElements.Claims, ScopeType.View));
                Assert.False(sut.GrantAccess(ModuleElements.Claims, ScopeType.Add));
                Assert.False(sut.GrantAccess(ModuleElements.Claims, ScopeType.Edit));
                Assert.True(sut.GrantAccess(ModuleElements.Claims, ScopeType.Delete));

                Assert.True(sut.GrantAccess(ModuleElements.Colours, ScopeType.View));
                Assert.False(sut.GrantAccess(ModuleElements.Colours, ScopeType.Add));
                Assert.True(sut.GrantAccess(ModuleElements.Colours, ScopeType.Edit));
                Assert.True(sut.GrantAccess(ModuleElements.Colours, ScopeType.Delete));

                Assert.True(sut.GrantAccess(ModuleElements.CostCodes, ScopeType.View));
                Assert.True(sut.GrantAccess(ModuleElements.CostCodes, ScopeType.Add));
                Assert.True(sut.GrantAccess(ModuleElements.CostCodes, ScopeType.Edit));
                Assert.True(sut.GrantAccess(ModuleElements.CostCodes, ScopeType.Delete));

                // Force the default case in the switch by creating a non-existing ScopeType via the backdoor
                Assert.False(Enum.IsDefined(typeof(ScopeType), int.MaxValue));
                Assert.False(sut.GrantAccess(ModuleElements.CostCodes, (ScopeType)int.MaxValue));
            }

            [Fact]
            public void NotSetElement_GrantAccess_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.GrantAccess(ModuleElements.CompanyPolicy, ScopeType.View));
                Assert.False(sut.GrantAccess(ModuleElements.CompanyPolicy, ScopeType.Add));
                Assert.False(sut.GrantAccess(ModuleElements.CompanyPolicy, ScopeType.Edit));
                Assert.False(sut.GrantAccess(ModuleElements.CompanyPolicy, ScopeType.Delete));
            }
        }

        public class CanAdd : AccessScopeCollectionFixture
        {
            [Fact]
            public void NotSetElement_CanAdd_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanAdd(ModuleElements.CompanyHelpAndSupportInformation));
            }

            [Fact]
            public void ExistingFalseElement_CanAdd_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanAdd(ModuleElements.ProjectCodes));
            }

            [Fact]
            public void ExistingTrueElement_CanAdd_ReturnsTrue()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.True(sut.CanAdd(ModuleElements.Advances));
            }
        }

        public class CanDelete : AccessScopeCollectionFixture
        {
            [Fact]
            public void NotSetElement_CanDelete_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanDelete(ModuleElements.CompanyHelpAndSupportInformation));
            }

            [Fact]
            public void ExistingFalseElement_CanDelete_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanDelete(ModuleElements.ProjectCodes));
            }

            [Fact]
            public void ExistingTrueElement_CanDelete_ReturnsTrue()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.True(sut.CanDelete(ModuleElements.Claims));
            }
        }

        public class CanEdit : AccessScopeCollectionFixture
        {
            [Fact]
            public void NotSetElement_CanEdit_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanEdit(ModuleElements.CompanyHelpAndSupportInformation));
            }

            [Fact]
            public void ExistingFalseElement_CanEdit_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanEdit(ModuleElements.ProjectCodes));
            }

            [Fact]
            public void ExistingTrueElement_CanEdit_ReturnsTrue()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.True(sut.CanEdit(ModuleElements.Colours));
            }
        }

        public class CanView : AccessScopeCollectionFixture
        {
            [Fact]
            public void NotSetElement_CanView_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanView(ModuleElements.CompanyHelpAndSupportInformation));
            }

            [Fact]
            public void ExistingFalseElement_CanView_ReturnsFalse()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.False(sut.CanView(ModuleElements.ProjectCodes));
            }

            [Fact]
            public void ExistingTrueElement_CanView_ReturnsTrue()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.True(sut.CanView(ModuleElements.Claims));
            }
        }

        public class Indexer : AccessScopeCollectionFixture
        {
            [Fact]
            public void ExistingElement_Indexer_ReturnsCorrectly()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.Equal(ModuleElements.ProjectCodes, sut[ModuleElements.ProjectCodes].Element);
            }

            [Fact]
            public void NotSetElement_Indexer_ReturnsNull()
            {
                AccessScopeCollection sut = this.CreateStandard();

                Assert.Null(sut[ModuleElements.Workflows]);
            }
        }


        public class AccessScopeCollectionFixture
        {
            protected AccessScopeCollection CreateStandard()
            {
                AccessScopeCollection accessScopeCollection = new AccessScopeCollection
                                                                  {
                                                                      new AccessScope(
                                                                          ModuleElements
                                                                              .ProjectCodes,
                                                                          false,
                                                                          false,
                                                                          false,
                                                                          false),
                                                                      new AccessScope(
                                                                          ModuleElements.Claims,
                                                                          false,
                                                                          false,
                                                                          false,
                                                                          true),
                                                                      new AccessScope(
                                                                          ModuleElements.Colours,
                                                                          false,
                                                                          false,
                                                                          true,
                                                                          true),
                                                                      new AccessScope(
                                                                          ModuleElements
                                                                              .CostCodes,
                                                                          false,
                                                                          true,
                                                                          true,
                                                                          true),
                                                                      new AccessScope(
                                                                          ModuleElements.Advances,
                                                                          true,
                                                                          true,
                                                                          true,
                                                                          true)
                                                                  };

                return accessScopeCollection;
            }
        }
    }
}
