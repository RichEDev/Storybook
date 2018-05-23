namespace BusinessLogic.Tests.P11DCategories
{
    using System;

    using BusinessLogic.Reasons;

    using Xunit;

    public class ReasonsTests
    {
        public class Ctor
        {
            [Fact]
            public void ValidCtor_InitializingReason_SetsProperties()
            {
                IReason sut = new Reason(11, false, "desc", "ref", null, null, null, null, null, null);

                Assert.Equal(11, sut.Id);
                Assert.Equal("ref", sut.Name);
                Assert.Equal("desc", sut.Description);
                Assert.Equal(false, sut.Archived);
                Assert.Equal(null, sut.AccountCodeVat);
                Assert.Equal(null, sut.AccountCodeNoVat);
                Assert.Equal(null, sut.CreatedBy);
                Assert.Equal(null, sut.CreatedOn);
                Assert.Equal(null, sut.ModifiedBy);
                Assert.Equal(null, sut.ModifiedOn);
            }

            [Fact]
            public void SetProperties_Reason_SetsValidProperties()
            {
                IReason sut = new Reason(11, false, "desc", "ref", null, null, null, null, null, null)
                {
                    Id = 12,
                    Archived = true,
                    Name = "new ref",
                    Description = "new desc",
                    AccountCodeVat = string.Empty,
                    AccountCodeNoVat = string.Empty,
                    CreatedBy = 1,
                    CreatedOn = new DateTime(2000, 01, 01),
                    ModifiedBy = 2,
                    ModifiedOn = new DateTime(2000, 01, 02)
                };

                // Check that the properties setter does nothing fancy
                Assert.Equal(12, sut.Id);
                Assert.Equal(true, sut.Archived);
                Assert.Equal("new ref", sut.Name);
                Assert.Equal("new desc", sut.Description);
                Assert.Equal(string.Empty, sut.AccountCodeVat);
                Assert.Equal(string.Empty, sut.AccountCodeNoVat);
                Assert.Equal(1, sut.CreatedBy);
                Assert.Equal(new DateTime(2000, 01, 01), sut.CreatedOn);
                Assert.Equal(2, sut.ModifiedBy);
                Assert.Equal(new DateTime(2000, 01, 02), sut.ModifiedOn);
            }
        }
    }
}
