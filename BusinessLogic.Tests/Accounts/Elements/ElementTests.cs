namespace BusinessLogic.Tests.Accounts.Elements
{
    using BusinessLogic.Accounts.Elements;

    using Xunit;

    public class ElementTests
    {
        public class Ctor
        {
            [Theory]
            [InlineData(1, 1, "Test", "For unit testing", false, true, false, "Testing", true)]
            [InlineData(15, 10, "Test2", "For unit testing2", true, false, true, "Testing2", false)]
            public void Initializing_Ctor_SetsProperties(
                int id
                , int categoryId
                , string name
                , string description
                , bool accessRoleCanEdit
                , bool accessRoleCanAdd
                , bool accessRoleCanDelete
                , string friendlyName
                , bool accessRoleApplicable)
            {
                Element sut = new Element(id, categoryId, name, description, accessRoleCanEdit, accessRoleCanAdd, accessRoleCanDelete, friendlyName, accessRoleApplicable);

                Assert.Equal(id, sut.Id);
                Assert.Equal(categoryId, sut.CategoryId);
                Assert.Equal(name, sut.Name);
                Assert.Equal(description, sut.Description);
                Assert.Equal(accessRoleCanEdit, sut.AccessRolesCanEdit);
                Assert.Equal(accessRoleCanAdd, sut.AccessRolesCanAdd);
                Assert.Equal(accessRoleCanDelete, sut.AccessRolesCanDelete);
                Assert.Equal(friendlyName, sut.FriendlyName);
                Assert.Equal(accessRoleApplicable, sut.AccessRolesApplicable);
            }
        }
    }
}
