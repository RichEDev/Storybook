namespace SqlDataAccess.Tests.Employees.AccessRoles
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;

    using BusinessLogic;
    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;
    using Common.Logging.NullLogger;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SQLDataAccess;
    using SQLDataAccess.AccessRoles;
    using SQLDataAccess.Employees.AccessRoles;

    using Xunit;
    public class SqlEmployeeCombinedAccessRoleFactoryTests
    {
        public class Ctor : Fixture
        {
            [Fact]
            public void NullAssignedAccessRolesFactory_Ctor_ThrowsArgumentNull()
            {
                this.AssignedAccessRolesFactory = null;

                Assert.Throws<ArgumentNullException>(() => new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger));
            }

            [Fact]
            public void NullAccessRolesFactory_Ctor_ThrowsArgumentNull()
            {
                this.AccessRolesFactory = null;

                Assert.Throws<ArgumentNullException>(() => new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger));
            }

            [Fact]
            public void NullCacheFactory_Ctor_ThrowsArgumentNull()
            {
                this.CacheFactory = null;

                Assert.Throws<ArgumentNullException>(() => new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger));
            }

            [Fact]
            public void NullIdentityProvider_Ctor_ThrowsArgumentNull()
            {
                this.IdentityProvider = null;

                Assert.Throws<ArgumentNullException>(() => new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger));
            }

            [Fact]
            public void NullLogger_Ctor_ThrowsArgumentNull()
            {
                this.Logger = null;

                Assert.Throws<ArgumentNullException>(() => new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger));
            }
        }

        public class Get : Fixture
        {
            [Fact]
            public void EmployeeWithAssignedAccessRoles_Get_ReturnsCombinedAccessRole()
            {
                this.Logger.IsDebugEnabled.Returns(true);


                IEnumerable<IAssignedAccessRole> assignedAccessRoles = new[] { new AssignedAccessRole(55, 2, 1), new AssignedAccessRole(55, 3, 1) };

                this.AssignedAccessRolesFactory.Get(55, Arg.Any<Predicate<IAssignedAccessRole>>()).Returns(assignedAccessRoles);

                this.AccessRolesFactory[2].Returns(new AccessRole(2, "two", string.Empty, new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection()));
                this.AccessRolesFactory[3].Returns(new AccessRole(3, "three", string.Empty, new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection()));

                IEmployeeAccessScope accessScope = this.Sut.Get(55, 1);

                Assert.Equal("2,3", accessScope.Id);

                // Test it went to the database to get the assigned access roles
                this.AssignedAccessRolesFactory.Received(1).Get(55, Arg.Any<Predicate<IAssignedAccessRole>>());

                // Test each assigned access role was retrieved from the database
                var accessRoleOneChecked = this.AccessRolesFactory.Received(1)[2];
                var accessRoleTwoChecked = this.AccessRolesFactory.Received(1)[3];

                this.Logger.Received(2).Debug(Arg.Any<string>());
            }

            [Fact]
            public void EmployeeWithNoAssigneAccessRoles_Get_ReturnsNull()
            {
                this.AssignedAccessRolesFactory.Get(55, Arg.Any<Predicate<IAssignedAccessRole>>()).Returns(new List<IAssignedAccessRole>());

                IEmployeeAccessScope accessScope = this.Sut.Get(55, 1);

                Assert.Null(accessScope);
            }
        }
        // get comp

        public class Get_Composite : Fixture
        {
            [Fact]
            public void Composite_GetComposite_ReturnsCorrectAccessScope()
            {
                this.Logger.IsDebugEnabled.Returns(true);

                IEnumerable<IAssignedAccessRole> assignedAccessRoles = new[] { new AssignedAccessRole(55, 2, 1), new AssignedAccessRole(55, 3, 1) };

                this.AssignedAccessRolesFactory.Get(55, Arg.Any<Predicate<IAssignedAccessRole>>()).Returns(assignedAccessRoles);

                this.AccessRolesFactory[2].Returns(new AccessRole(2, "two", string.Empty, new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection()));
                this.AccessRolesFactory[3].Returns(new AccessRole(3, "three", string.Empty, new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection()));


                IEmployeeAccessScope accessScope = this.Sut.Get("2,3");

                Assert.Equal("2,3", accessScope.Id);

                this.IdentityProvider.Received(1).GetUserIdentity();
                this.Logger.Received(4).Debug(Arg.Any<string>());
            }
        }


        public abstract class Fixture
        {
            public IAssignedAccessRolesFactory AssignedAccessRolesFactory { get; set; }
            public IDataFactory<IAccessRole, int> AccessRolesFactory { get; set; }
            public IAccountCacheFactory<IEmployeeAccessScope, string> CacheFactory { get; set; }
            public IIdentityProvider IdentityProvider { get; set; }

            public ILog Logger { get; set; }

            public IEmployeeCombinedAccessRoles Sut { get; set; }

            protected Fixture()
            {
                this.AssignedAccessRolesFactory = Substitute.For<IAssignedAccessRolesFactory>();
                this.AccessRolesFactory = Substitute.For<IDataFactory<IAccessRole, int>>();
                this.CacheFactory = Substitute.For<IAccountCacheFactory<IEmployeeAccessScope, string>>();
                this.IdentityProvider = Substitute.For<IIdentityProvider>();
                this.IdentityProvider.GetUserIdentity().Returns(new UserIdentity(217, 55));
                this.Logger = Substitute.For<ILog>();
                this.Sut = new SqlEmployeeCombinedAccessRolesFactory(this.AssignedAccessRolesFactory, this.AccessRolesFactory, this.CacheFactory, this.IdentityProvider, this.Logger);
            }
        }
    }
}
