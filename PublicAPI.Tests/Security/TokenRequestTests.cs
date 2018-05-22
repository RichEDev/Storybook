namespace PublicAPI.Tests.Security
{
    using PublicAPI.Security;

    using Xunit;

    public class TokenRequestTests
    {

        [Fact]
        public void SetProperties_Properties_SetCorrectly()
        {
            TokenRequest sut = new TokenRequest { AccountId = 1, DelegateId = 2, EmployeeId = 3, SecretKey = "frank", SubAccountId = 4, TimeoutMinutes = 5 };

            Assert.Equal(1, sut.AccountId);
            Assert.Equal(2, sut.DelegateId);
            Assert.Equal(3, sut.EmployeeId);
            Assert.Equal(4, sut.SubAccountId);
            Assert.Equal(5, sut.TimeoutMinutes);
            Assert.Equal("frank", sut.SecretKey);
        }

        [Fact]
        public void NotSetProperties_Properties_AreDefault()
        {
            TokenRequest sut = new TokenRequest();

            Assert.Equal(0, sut.AccountId);
            Assert.Null(sut.DelegateId);
            Assert.Equal(0, sut.EmployeeId);
            Assert.Equal(0, sut.SubAccountId);
            Assert.Equal(0, sut.TimeoutMinutes);
            Assert.Null(sut.SecretKey);
        }
    }
}
