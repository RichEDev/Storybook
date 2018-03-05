namespace BusinessLogic.Tests.Accounts
{
    using BusinessLogic.Accounts;
    using BusinessLogic.Databases;


    using Utilities.Cryptography;

    using NSubstitute;
    
    using Xunit;

    public class AccountTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Initializing_Account_SetsProperties(bool archived)
        {
            DatabaseServer databaseServer = new DatabaseServer(99, "testHost");
            DatabaseCatalogue databaseCatalogue = new DatabaseCatalogue(databaseServer, "myCatalogue", "myUsername", ExpensesCryptography.Encrypt("myPassword"), new ExpensesCryptography());
            Account sut = new Account(43, databaseCatalogue, archived);
            Assert.Equal(43, sut.Id);
            Assert.Equal("myCatalogue", sut.DatabaseCatalogue.Catalogue);
            Assert.Equal("Data Source=testHost;Initial Catalog=myCatalogue;User ID=myUsername;Password=myPassword;Max Pool Size=10000;Application Name=Expenses", sut.DatabaseCatalogue.ConnectionString);
            Assert.Equal("myPassword", sut.DatabaseCatalogue.Password);
            Assert.Equal("myUsername", sut.DatabaseCatalogue.Username);
            Assert.Equal("testHost", sut.DatabaseCatalogue.Server.Hostname);
            Assert.Equal(archived, sut.Archived);
            Assert.Equal(99, sut.DatabaseCatalogue.Server.Id);
        }
    }
}
