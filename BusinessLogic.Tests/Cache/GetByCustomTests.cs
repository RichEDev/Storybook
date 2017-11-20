namespace BusinessLogic.Tests.Cache
{
    using System;
    using BusinessLogic.Cache;
    using Xunit;

    public class GetByCustomTests
    {
        public class Ctor
        {
            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void NullEmptyOrWhiteSpaceHashName_Ctor_ThrowsArgumentNullException(string value)
            {
                Assert.Throws<ArgumentNullException>(() => new GetByCustomStub(value, "testField"));
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void NullEmptyOrWhiteSpaceField_Ctor_ThrowsArgumentNullException(string value)
            {
                Assert.Throws<ArgumentNullException>(() => new GetByCustomStub("testHash", value));
            }

            [Fact]
            public void Valid_Ctor_SetsProperties()
            {
                GetByCustomStub sut = new GetByCustomStub("testHash", "testField");

                Assert.Equal("testHash", sut.HashName);
                Assert.Equal("testField", sut.Field);
            }
        }

        public class GetByCustomStub : GetByCustom
        {
            public GetByCustomStub(string hashName, string field) : base(hashName, field)
            {
            }
        }
    }


}
