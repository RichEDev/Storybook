namespace BusinessLogic.Tests.ImportExport
{
    using System;

    using BusinessLogic.ImportExport;

    using Xunit;

    public class ImportFieldTests
    {
        [Fact]
        public void InitializingConstructor_CreatesDefaultImportField()
        {
            ImportField sut = new ImportField(Guid.Empty, Guid.Empty, string.Empty);
            Assert.Equal(Guid.Empty, sut.DestinationColumn);
            Assert.Equal(Guid.Empty, sut.LookupColumn);
            Assert.Equal(string.Empty, sut.DefaultValue);
        }

        [Fact]
        public void Initializing_ImportField_WithNullDefault_SetsProperties()
        {
            ImportField sut = new ImportField(Guid.Empty, Guid.Empty, null);
            Assert.Equal(Guid.Empty, sut.DestinationColumn);
            Assert.Equal(Guid.Empty, sut.LookupColumn);
            Assert.Null(sut.DefaultValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Hello World")]
        [InlineData("24/06/1886")]
        [InlineData("True")]
        public void Set_ImportFieldProperties_SetsProperties(string testValue)
        {
            ImportField sut = new ImportField(Guid.Empty, Guid.Empty, testValue);
            Assert.Equal(Guid.Empty, sut.DestinationColumn);
            Assert.Equal(Guid.Empty, sut.LookupColumn);
            Assert.Equal(testValue, sut.DefaultValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Hello World")]
        [InlineData("24/06/1886")]
        [InlineData("True")]
        public void Set_ImportFieldProperties_GetsProperties(string testValue)
        {
            ImportField sut = new ImportField(Guid.Empty, Guid.Empty, null) {DefaultValue = testValue};
            Assert.Equal(Guid.Empty, sut.DestinationColumn);
            Assert.Equal(Guid.Empty, sut.LookupColumn);
            Assert.Equal(testValue, sut.DefaultValue);
        }
    }
}
