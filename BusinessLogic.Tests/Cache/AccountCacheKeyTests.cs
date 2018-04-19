namespace BusinessLogic.Tests.Cache
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.ProjectCodes;

    using NSubstitute;

    using Xunit;

    public class AccountCacheKeyTests
    {
        public class Ctor
        {
            [Fact]
            public void NullAccount_Ctor_ThrowsArgumentException()
            {
                Assert.Throws<ArgumentNullException>(() => new AccountCacheKey<IAccount>(null));
            }

            [Fact]
            public void ValidAccount_Ctor_SetsProperties()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id.Returns(19);

                AccountCacheKey<IAccount> sut = new AccountCacheKey<IAccount>(account);
                
                Assert.Equal(19, sut.AccountId);
            }
        }

        public class GetCacheKey
        {
            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData(" ")]
            public void NullEmptyOrWhiteSpaceArea_GetCacheKey_ThrowsArgumentNullException(string value)
            {
                IAccount account = Substitute.For<IAccount>();
                AccountCacheKey<IAccount> sut = new AccountCacheKey<IAccount>(account) { Key = account, Area = value };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKey());
            }

            [Fact]
            public void NullKey_GetCacheKey_ThrowsArgumentNullException()
            {
                IAccount account = Substitute.For<IAccount>();
                AccountCacheKey<IAccount> sut = new AccountCacheKey<IAccount>(account) { Area = "testArea" };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKey());
            }

            [Fact]
            public void IProjectCodeAccountCacheKey_GetCacheKey_ReturnsExpectedKey()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id.Returns(261);

                AccountCacheKey<int> sut = new AccountCacheKey<int>(account) { Area = typeof(IProjectCode).Name, Key = 99 };

                Assert.Equal("A:261:IProjectCode:99", sut.GetCacheKey());
            }
        }

        public class GetCacheKeyHash
        {
            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData(" ")]
            public void NullEmptyOrWhiteSpaceArea_GetCacheKeyHash_ThrowsArgumentNullException(string value)
            {
                IAccount account = Substitute.For<IAccount>();
                AccountCacheKey<IAccount> sut = new AccountCacheKey<IAccount>(account) { Key = account, Area = value };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKeyHash("test"));
            }

            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData(" ")]
            public void NullEmptyOrWhiteSpaceHashName_GetCacheKeyHash_ThrowsArgumentNullException(string value)
            {
                IAccount account = Substitute.For<IAccount>();
                AccountCacheKey<int> sut = new AccountCacheKey<int>(account) { Key = 11, Area = "testArea" };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKeyHash(value));
            }

            [Fact]
            public void IProjectCodeAccountCacheKey_GetCacheKeyHash_ReturnsExpectedKey()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id.Returns(261);

                AccountCacheKey<int> sut = new AccountCacheKey<int>(account) { Area = typeof(IProjectCode).Name, Key = 99 };

                Assert.Equal("A:261:IProjectCode:test", sut.GetCacheKeyHash("test"));
            }
        }
    }
}
