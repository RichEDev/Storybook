namespace BusinessLogic.Tests.Announcements
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic.Announcements;

    using NSubstitute;

    using Xunit;

    /// <summary>
    /// Tests for <see cref="Announcement"/>
    /// </summary>
    public class AnnouncementTests
    {
        /// <summary>
        /// Constructor tests for <see cref="PropertySetter"/>
        /// </summary>
        public class PropertySetter : AnnouncementFixture
        {
            /// <summary>
            /// Test to ensure the property setter sets properties as expected
            /// </summary>
            [Fact]
            public void SetProperties_Announcement_SetsValidProperties()
            {
                Announcement sut = new Announcement()
                {
                    Id = this.Guid,
                    Message = "Announcement",
                    ReadReceipts = this.ReadReceiptsList
                };
                Assert.Equal(this.Guid, sut.Id);
                Assert.Equal("Announcement", sut.Message);
                Assert.Equal(this.ReadReceiptsList, sut.ReadReceipts);
            }
        }

        /// <summary>
        /// Fixture for unit testing <see cref="Announcement"/>
        /// </summary>
        public class AnnouncementFixture
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AnnouncementFixture"/> class. 
            /// </summary>
            public AnnouncementFixture()
            {
                this.ReadReceiptsList = new ReadReceipts
                                            {
                                                new ReadReceipt { AccountId = 1234, EmployeeId = 4321 },
                                                new ReadReceipt { AccountId = 0987, EmployeeId = 7890 }
                                            };

                this.Guid = new Guid("FFB37082-708F-44C6-8EE1-2F33F570CCE5");
            }
            
            /// <summary>
            /// Gets mock'd read receipts
            /// </summary>
            public ReadReceipts ReadReceiptsList { get; }

            /// <summary>
            /// Gets mock'd guid
            /// </summary>
            public Guid Guid { get; }
        }
    }
}
