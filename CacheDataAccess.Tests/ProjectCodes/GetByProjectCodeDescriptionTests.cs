namespace CacheDataAccess.Tests.ProjectCodes
{
    using CacheDataAccess.ProjectCodes;

    using Xunit;

    public class GetByProjectCodeDescriptionTests
    {
        [Fact]
        public void Initialized_Constructor_SetsProperties()
        {
            GetByProjectCodeDescription sut = new GetByProjectCodeDescription("testDescription");

            Assert.Equal("testDescription", sut.Field);
            Assert.Equal("descriptions", sut.HashName);
        }
    }
}
