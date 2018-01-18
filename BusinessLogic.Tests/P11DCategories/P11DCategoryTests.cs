using System;

namespace BusinessLogic.Tests.P11DCategories
{
    using BusinessLogic.P11DCategories;
    using Xunit;

    public class P11DCategoryTests
    {
        public class Ctor
        {
            [Fact]
            public void ValidCtor_InitializingP11DCategory_SetsProperties()
            {
                P11DCategory sut = new P11DCategory(11, "ref");
                Assert.Equal(11, sut.Id);
                Assert.Equal("ref", sut.Name);
            }

            [Fact]
            public void SetProperties_P11DCategory_SetsValidProperties()
            {
                P11DCategory sut = new P11DCategory(-1, "bobobo")
                {
                    Id = 12,
                    Name = "new ref"
                };

                // Check that the properties setter does nothing fancy
                Assert.Equal(12, sut.Id);
                Assert.Equal("new ref", sut.Name);
            }
        }
    }
}
