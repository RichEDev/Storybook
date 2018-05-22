namespace BusinessLogic.Tests.Employees.AccessRoles
{
    using System;

    using BusinessLogic.Employees.AccessRoles;

    using Xunit;

    public class AssignedAccessRoleTests
    {
        [Fact]
        public void ValidArugments_Ctor_SetsProperties()
        {
            AssignedAccessRole sut = new AssignedAccessRole(55, 44, 33);
            
            Assert.Equal(55, sut.EmployeeId);
            Assert.Equal(44, sut.AccessRoleId);
            Assert.Equal(33, sut.SubAccountId);

            Assert.Equal($"{sut.EmployeeId},{sut.AccessRoleId},{sut.SubAccountId}", sut.Id);
        }

        [Fact]
        public void SetId_Property_ThrowsNotImplemented()
        {
            AssignedAccessRole sut = new AssignedAccessRole(55, 44, 33);

            Assert.Throws<NotImplementedException>(() => sut.Id = "bob");
        }
    }
}
