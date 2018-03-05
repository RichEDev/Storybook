namespace BusinessLogic.Tests.Receipts
{
    using BusinessLogic.Receipts;

    using System;

    using Xunit;

    public class WalletReceiptTests
    {
        public class Ctor
        {
            [Theory]
            [InlineData("jpg")]
            [InlineData("JPG")]
            public void Initializing_Ctor_CreateWalletReceipt (string fileName)
            {
                WalletReceipt sut = new WalletReceipt(11, fileName, "sbgbgntntym");
                Assert.Equal(11, sut.Id);
                Assert.Equal(fileName, sut.FileExtension);
                Assert.Equal("sbgbgntntym", sut.ReceiptData);
            }

            [Fact]
            public void SetProperties_Ctor_SetsProperties()
            {
                WalletReceipt sut = new WalletReceipt(-1, "tiff", "fgaerrrt")
                {
                    Id = 12,
                    FileExtension = "jpg",
                    ReceiptData = "sbgbgntntym"
                };

                Assert.Equal(12, sut.Id);
                Assert.Equal("jpg", sut.FileExtension);
                Assert.Equal("sbgbgntntym", sut.ReceiptData);
            }

            [Fact]
            public void SetAllProperties_Ctor_SetsProperties()
            {
                WalletReceipt sut = new WalletReceipt(-1, "tiff", "fgaerrrt")
                {
                    Id = 12,
                    FileExtension = "jpg",
                    ReceiptData = "sbgbgntntym",
                    CreatedBy = 2,
                    CreatedOn = DateTime.ParseExact("12/25/2008", "MM/dd/yyyy", null),
                    Date = DateTime.ParseExact("12/25/2012", "MM/dd/yyyy", null),
                    Status = -1,
                    Total = 10
                };

                Assert.Equal(12, sut.Id);
                Assert.Equal("jpg", sut.FileExtension);
                Assert.Equal("sbgbgntntym", sut.ReceiptData);
                Assert.Equal(2, sut.CreatedBy);
                Assert.Equal(DateTime.ParseExact("12/25/2008", "MM/dd/yyyy", null), sut.CreatedOn);
                Assert.Equal(DateTime.ParseExact("12/25/2012", "MM/dd/yyyy", null), sut.Date);
                Assert.Equal(-1, sut.Status);
                Assert.Equal(10, sut.Total);
            }

            [Fact]
            public void SetAllButNullableProperties_Ctor_SetsAllProperties()
            {
                WalletReceipt sut = new WalletReceipt(-1, "tiff", "fgaerrrt")
                {
                    Id = 12,
                    FileExtension = "jpg",
                    ReceiptData = "sbgbgntntym",
                    CreatedBy = 2,
                    CreatedOn = DateTime.ParseExact("12/25/2008", "MM/dd/yyyy", null),
                    Date = null,
                    Status = -1,
                    Total = null
                };

                Assert.Equal(12, sut.Id);
                Assert.Equal("jpg", sut.FileExtension);
                Assert.Equal("sbgbgntntym", sut.ReceiptData);
                Assert.Equal(2, sut.CreatedBy);
                Assert.Equal(DateTime.ParseExact("12/25/2008", "MM/dd/yyyy", null), sut.CreatedOn);
                Assert.Equal(null, sut.Date);
                Assert.Equal(-1, sut.Status);
                Assert.Equal(null, sut.Total);
            }
        }
    }
}
