namespace BusinessLogic.Tests.Accounts.Elements
{
    using System.Collections.Generic;

    using BusinessLogic.Databases;
    using BusinessLogic.Accounts.Elements;
    
    using Utilities.Cryptography;

    using Xunit;

    public class AccountWithElementsTests
    {
        public class Ctor
        {
            [Fact]
            public void InitializingNoElements_Ctor_SetsProperties()
            {
                DatabaseServer databaseServer = new DatabaseServer(99, "testHost");
                DatabaseCatalogue databaseCatalogue = new DatabaseCatalogue(databaseServer, "myCatalogue", "myUsername", ExpensesCryptography.Encrypt("myPassword"), new ExpensesCryptography());
                AccountWithElements sut = new AccountWithElements(43, databaseCatalogue, false, new List<IElement>());

                Assert.Equal(43, sut.Id);
                Assert.Equal("myCatalogue", sut.DatabaseCatalogue.Catalogue);
                Assert.Equal("Data Source=testHost;Initial Catalog=myCatalogue;User ID=myUsername;Password=myPassword;Max Pool Size=10000;Application Name=Expenses", sut.DatabaseCatalogue.ConnectionString);
                Assert.Equal("myPassword", sut.DatabaseCatalogue.Password);
                Assert.Equal("myUsername", sut.DatabaseCatalogue.Username);
                Assert.Equal("testHost", sut.DatabaseCatalogue.Server.Hostname);
                Assert.Equal(false, sut.Archived);
                Assert.Equal(99, sut.DatabaseCatalogue.Server.Id);
            }

            [Fact]
            public void InitializingWithElements_Ctor_SetsProperties()
            {
                DatabaseServer databaseServer = new DatabaseServer(99, "testHost");
                DatabaseCatalogue databaseCatalogue = new DatabaseCatalogue(databaseServer, "myCatalogue", "myUsername", ExpensesCryptography.Encrypt("myPassword"), new ExpensesCryptography());

                List<IElement> elements = new List<IElement>
            {
                new Element(1, 1, "Test", "For unit testing", false, false, false, "Testing", false)
            };

                AccountWithElements sut = new AccountWithElements(43, databaseCatalogue, false, elements);

                Assert.IsType<List<IElement>>(sut.LicencedElements);
                Assert.Equal(43, sut.Id);
                Assert.Equal("myCatalogue", sut.DatabaseCatalogue.Catalogue);
                Assert.Equal("Data Source=testHost;Initial Catalog=myCatalogue;User ID=myUsername;Password=myPassword;Max Pool Size=10000;Application Name=Expenses", sut.DatabaseCatalogue.ConnectionString);
                Assert.Equal("myPassword", sut.DatabaseCatalogue.Password);
                Assert.Equal("myUsername", sut.DatabaseCatalogue.Username);
                Assert.Equal("testHost", sut.DatabaseCatalogue.Server.Hostname);
                Assert.Equal(false, sut.Archived);
                Assert.Equal(99, sut.DatabaseCatalogue.Server.Id);
            }
        }
    }
}
