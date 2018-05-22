namespace BusinessLogic.Tests.Identity
{
    using BusinessLogic.Identity;

    using Xunit;

    public class UserIdentityTests
    {
        public class Ctor
        {
            [Fact]
            public void ValidArguments_Ctor_SetsProperties()
            {
                UserIdentity sut = new UserIdentity(55, 44);

                Assert.Equal(55, sut.AccountId);
                Assert.Equal(44, sut.EmployeeId);
                Assert.Null(sut.DelegateId);
                Assert.Equal("55,44", sut.Name);
                Assert.True(sut.IsAuthenticated);
            }

            [Fact]
            public void ZeroArguments_Ctor_SetsPropertiesAndIsAuthenticatedFalse()
            {
                UserIdentity sut = new UserIdentity(0, 0);

                Assert.Equal(0, sut.AccountId);
                Assert.Equal(0, sut.EmployeeId);
                Assert.Null(sut.DelegateId);
                Assert.Equal("0,0", sut.Name);
                Assert.False(sut.IsAuthenticated);
            }

            [Fact]
            public void ValidArguments_Ctor2_SetsProperties()
            {
                UserIdentity sut = new UserIdentity(55, 44, 33);

                Assert.Equal(55, sut.AccountId);
                Assert.Equal(44, sut.EmployeeId);
                Assert.Equal(33, sut.DelegateId);
                Assert.Equal("55,44,33", sut.Name);
                Assert.True(sut.IsAuthenticated);
            }
        }

        public class Properties
        {
            [Fact]
            public void GetDefault_SubAccountId_ReturnsZero()
            {
                UserIdentity sut = new UserIdentity(11, 12);

                Assert.Equal(0, sut.SubAccountId);
            }

            [Fact]
            public void Set_SubAccountId_ReturnsCorrectly()
            {
                UserIdentity sut = new UserIdentity(11, 12) { SubAccountId = 13 };

                Assert.Equal(13, sut.SubAccountId);
            }

            [Fact]
            public void GetDefault_AuthenticationType_ReturnsZero()
            {
                UserIdentity sut = new UserIdentity(11, 12);

                Assert.Null(sut.AuthenticationType);
            }

            [Fact]
            public void Set_AuthenticationType_ReturnsCorrectly()
            {
                UserIdentity sut = new UserIdentity(11, 12) { AuthenticationType = "Testing" };

                Assert.Equal("Testing", sut.AuthenticationType);
            }


            [Fact]
            public void GetDefault_CombinedAccessRoleId_ReturnsZero()
            {
                UserIdentity sut = new UserIdentity(11, 12);

                Assert.Null(sut.CombinedAccessRoleId);
            }

            [Fact]
            public void Set_CombinedAccessRoleId_ReturnsCorrectly()
            {
                UserIdentity sut = new UserIdentity(11, 12) { CombinedAccessRoleId = "123,234,345" };

                Assert.Equal("123,234,345", sut.CombinedAccessRoleId);
            }
        }
    }
}
