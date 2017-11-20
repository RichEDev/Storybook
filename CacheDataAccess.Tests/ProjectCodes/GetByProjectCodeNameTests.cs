namespace CacheDataAccess.Tests.ProjectCodes
{
    using CacheDataAccess.ProjectCodes;

    using Xunit;

    public class GetByProjectCodeNameTests
    {
        [Fact]
        public void Initialized_Constructor_SetsProperties()
        {
            GetByProjectCodeName sut = new GetByProjectCodeName("testName");

            Assert.Equal("testName", sut.Field);
            Assert.Equal("names", sut.HashName);
        }
    }
}
