namespace BusinessLogic.Tests.Database
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using Databases;
    using Configuration.Interface;
    using Utilities.Cryptography;
    using Xunit;

    public class MetabaseCatalogueTests
    {
        public class Ctor : MetabaseCatalogueFixture
        {
            [Fact]
            public void NullCrytography_Ctor_ThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new MetabaseCatalogue(null, this.ConfigurationManager));
            }

            [Fact]
            public void NullConfigurationManager_Ctor_ThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new MetabaseCatalogue(this.Cryptography, null));
            }

            [Fact]
            public void Valid_Ctor_SetsProperties()
            {
                MetabaseCatalogue sut = new MetabaseCatalogue(this.Cryptography, this.ConfigurationManager);

                Assert.Equal("DevelopmentMetabaseExpenses", sut.Catalogue);
                Assert.Equal("Data Source=localhost;Initial Catalog=DevelopmentMetabaseExpenses;User ID=spenduser;Password=testPassword;Max Pool Size=10000;Application Name=Expenses", sut.ConnectionString);
                Assert.Equal("testPassword", sut.Password);
                Assert.Equal("spenduser", sut.Username);
                Assert.Equal("localhost", sut.Server.Hostname);
                Assert.Equal(0, sut.Server.Id);
            }
        }

        public class MetabaseCatalogueFixture
        {
            public ExpensesCryptography Cryptography => new ExpensesCryptography();

            public IConfigurationManager ConfigurationManager { get; }

            public MetabaseCatalogueFixture()
            {
                this.ConfigurationManager = new MockedConfigurationManager();
            }
        }
    }

    public class MockedConfigurationManager : IConfigurationManager
    {
        public MockedConfigurationManager()
        {
            this.AppSettings = new NameValueCollection();
            this.ConnectionStrings = new ConnectionStringSettingsCollection
            {
                new ConnectionStringSettings("metabase", "Data Source=localhost;Initial Catalog=DevelopmentMetabaseExpenses;User ID=spenduser;Password=qdnvRRtkbjetJ24EGR/BCQ==;Max Pool Size=10000;Application Name=Expenses")
            };
        }

        public T GetSection<T>(string sectionName)
        {
            throw new NotImplementedException();
        }

        public NameValueCollection AppSettings { get; }
        public ConnectionStringSettingsCollection ConnectionStrings { get; }
    }

}
