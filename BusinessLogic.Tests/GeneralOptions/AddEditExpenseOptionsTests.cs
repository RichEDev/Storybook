using BusinessLogic.GeneralOptions;
using BusinessLogic.GeneralOptions.AddEditExpense;
using Xunit;

namespace BusinessLogic.Tests.GeneralOptions
{
    public class AddEditExpenseOptionsTests
    {
        public class Ctor
        {
            [Fact]
            public void ValidCtor_InitializingAddEditExpenseOptions_SetsDefaultProperties()
            {
                IAddEditExpenseOptions sut = new AddEditExpenseOptions();

                Assert.Equal(null, sut.AddressNameEntryMessage);
                Assert.Equal(null, sut.HomeAddressKeyword);
                Assert.Equal(null, sut.WorkAddressKeyword);
                Assert.Equal(false, sut.ClaimantsCanAddCompanyLocations);
                Assert.Equal(false, sut.DisableCarOutsideOfStartEndDate);
                Assert.Equal(false, sut.DisplayEsrAddressesInSearchResults);
                Assert.Equal(false, sut.ExchangeReadOnly);
                Assert.Equal(false, sut.ForceAddressNameEntry);
                Assert.Equal(IncludeEsrDetails.None, sut.IncludeAssignmentDetails);
                Assert.Equal(false, sut.MultipleWorkAddress);
            }
        }
    }
}
