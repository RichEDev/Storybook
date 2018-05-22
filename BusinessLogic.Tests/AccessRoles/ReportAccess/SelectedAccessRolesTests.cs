namespace BusinessLogic.Tests.AccessRoles.ReportsAccess
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.AccessRoles.ReportsAccess;

    using Xunit;

    public class SelectedAccessRolesTests
    {
        [Fact]
        public void Ctor_WithPopulatedArguments_PopulatedCollection()
        {
            SelectedAccessRoles sut = new SelectedAccessRoles(new[] { 1, 2, 3 });

            Assert.Equal(3, sut.Count);
        }

        [Fact]
        public void Ctor_WithEmptyLinkedAccessRoleCollection_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SelectedAccessRoles(new List<int>()));
        }

        [Fact]
        public void Ctor_WithNullLinkedAccessRoleCollection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SelectedAccessRoles(null));
        }

        [Fact]
        public void Ctor_WithValidArguments_SetsLevel()
        {
            SelectedAccessRoles sut = new SelectedAccessRoles(new[] { 1, 2, 3 });

            Assert.Equal(ReportingAccess.SelectedRoles, sut.Level);
        }
    }
}
