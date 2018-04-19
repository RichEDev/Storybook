namespace BusinessLogic.Tests.Cache
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.ProjectCodes;

    using NSubstitute;

    using Xunit;

    public class MetabaseCacheKeyTests
    {
        public class Ctor
        {
            [Fact]
            public void Valid_Ctor_SetsProperties()
            {
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode>();
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
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode> { Key = Substitute.For<IProjectCode>(), Area = value };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKey());
            }

            [Fact]
            public void NullKey_GetCacheKey_ThrowsArgumentNullException()
            {
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode> { Area = "testArea" };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKey());
            }

            [Fact]
            public void IProjectCodeAccountCacheKey_GetCacheKey_ReturnsExpectedKey()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id.Returns(261);

                Guid guid = Guid.NewGuid();

                MetabaseCacheKey<Guid> sut = new MetabaseCacheKey<Guid> { Area = typeof(IProjectCode).Name, Key = guid };

                Assert.Equal($"M:IProjectCode:{guid}", sut.GetCacheKey());
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
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode> { Key = Substitute.For<IProjectCode>(), Area = value };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKeyHash("test"));
            }

            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData(" ")]
            public void NullEmptyOrWhiteSpaceHashName_GetCacheKeyHash_ThrowsArgumentNullException(string value)
            {
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode> { Key = Substitute.For<IProjectCode>(), Area = "testArea" };

                Assert.Throws<ArgumentNullException>(() => sut.GetCacheKeyHash(value));
            }

            [Fact]
            public void IProjectCodeAccountCacheKey_GetCacheKeyHash_ReturnsExpectedKey()
            {
                MetabaseCacheKey<IProjectCode> sut = new MetabaseCacheKey<IProjectCode> { Area = typeof(IProjectCode).Name, Key = Substitute.For<IProjectCode>() };

                Assert.Equal($"M:IProjectCode:test", sut.GetCacheKeyHash("test"));
            }
        }
    }
}
