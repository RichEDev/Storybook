namespace SqlDataAccess.Tests.ImportExport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.ImportExport;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using NSubstitute;

    using SqlDataAccess.Tests.Bootstrap;

    using SQLDataAccess.ImportExport;

    using Xunit;

    using Reasons = BusinessLogic.Constants.Tables.Reasons;

    public class SqlImportReasonsTests 
    {
        public class Constructor : SqlImportReasonFactoryFixture
        {
            [Fact]
            public void WithNulls_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportReasons(null, null, null, null));
            }

            [Fact]
            public void WithNullFieldRepository_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportReasons(null, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.Reasons.SqlReasonsFactory));
            }

            [Fact]
            public void WithNullTablesRepository_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportReasons(this.BootstrapBuilder.Fields.FieldRepository, null, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.Reasons.SqlReasonsFactory));
            }

            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportReasons(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Tables.TableRepository, null, this.BootstrapBuilder.Reasons.SqlReasonsFactory));
            }

            [Fact]
            public void ValidParams_FieldRepository_ctor_ShouldNotThrowException()
            {
                 var sut = new ImportReasons(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.Reasons.SqlReasonsFactory);
                Assert.NotNull(sut);
            }
        }

        public class Import : SqlImportReasonFactoryFixture
        {
            [Fact]
            public void WithNullParams_Throws_Exception()
            {
                Assert.Throws<ArgumentNullException>(() => this.SUT.Import(null, null, null));
            }

            [Fact]
            public void WithEmpty_Params_ImportReturnsValidMessage()
            {
                var result = this.SUT.Import(new SortedList<Guid, string>(), new List<ImportField>(), new List<List<object>>());
                Assert.NotEmpty(result);
                Assert.Equal(result[0], "File not imported no key field selected.");
            }

            [Fact]
            public void WithFileContent_Params_ImportReturnsValidMessage()
            {
                var fileContent = new List<List<object>> {new List<object> {"Hello world", "goodbye"}, {new List<object>{"Description"}}};
                var result = this.SUT.Import(new SortedList<Guid, string>(), new List<ImportField>(), fileContent);
                Assert.NotEmpty(result);
                Assert.Equal(result[0], "File not imported no key field selected.");
            }

            [Fact]
            public void WithFileContentAndMappedFields_Params_ImportReturnsValidMessage()
            {
                this.BootstrapBuilder.System.CustomerDataConnection.GetReader(Arg.Any<string>()).Returns(new DataTableReader(new DataTable()));

                var fileContent = new List<List<object>> {new List<object> {"Hello world", "goodbye", "AccountCodeVat", "AccountCodeNoVat", "20/6/1960"}, {new List<object>{"Description", string.Empty, "AccountCodeVat", "AccountCodeNoVat", "31/3/2018"}}};
                var mappedFields = new List<ImportField>
                {
                    new ImportField(Reasons.Fields.Reason, Guid.Empty, "DefaultValue"),
                    new ImportField(Reasons.Fields.Description, Guid.Empty, "DefaultValue"),
                    new ImportField(Reasons.Fields.AccountCodeVat, Guid.Empty, "DefaultValue"),
                    new ImportField(Reasons.Fields.AccountCodeNoVat, Guid.Empty, "DefaultValue"),
                    new ImportField(new Guid("1ECA782C-8628-49BC-A22B-0A1F64F6240C"), Guid.Empty, DateTime.Now.ToLongDateString())
                };

                var result = this.SUT.Import(new SortedList<Guid, string>(), mappedFields, fileContent);
                Assert.NotEmpty(result);
                Assert.Equal(result[0], "Row 0 imported successfully.");
                Assert.Equal(result[1], "Row 1 imported successfully.");
                Assert.Equal(result[2], "File imported successfully.");
            }
        }
    }

    public class SqlImportReasonFactoryFixture
    {
        /// <summary>
        /// Gets or sets an instance of <see cref="BootstrapBuilder"/>.
        /// </summary>
        public BootstrapBuilder BootstrapBuilder { get; set; }

        /// <summary>
        /// Gets the System Under Test - <see cref="ImportReasons"/>
        /// </summary>
        public ImportReasons SUT { get; }

        public SqlImportReasonFactoryFixture()
        {
            this.BootstrapBuilder = new BootstrapBuilder().WithSystem().WithFields().WithTables().WithReasons();

            var reasonsTable = new MetabaseTable("Reasons", 0, "Reasons", false, false, false, false, Reasons.TableId, Reasons.Fields.ReasonId, Reasons.Fields.Reason, new Guid("CE235F78-82C6-4BA1-8845-034A015C5DCA"), Guid.Empty, ModuleElements.Reasons, false, string.Empty);

            var reason = new Field(
                Reasons.Fields.Reason,
                "Reason",
                "Reason",
                "Reason",
                Reasons.TableId,
                string.Empty,
                new FieldAttributes(),
                Guid.Empty,
                0,
                0,
                false);

            var reasonId = new Field(
               Reasons.Fields.ReasonId,
                "ReasonId",
                "ReasonId",
                "ReasonId",
                Reasons.TableId,
                string.Empty,
                new FieldAttributes(),
                Guid.Empty, 
                0, 
                0,
                false);

            var description = new Field(
                Reasons.Fields.Description,
                "Description",
                "Description",
                "Description",
                Reasons.TableId,
                string.Empty,
                new FieldAttributes(),
                Guid.Empty,
                0,
                0,
                false);

            var accountCodeVat = new Field(
                Reasons.Fields.AccountCodeVat,
                "AccountCodeVat",
                "AccountCodeVat",
                "AccountCodeVat",
                Reasons.TableId,
                string.Empty,
                new FieldAttributes(),
                Guid.Empty,
                0,
                0,
                false);

            var accountCodeNoVat = new Field(
                Reasons.Fields.AccountCodeNoVat,
                "AccountCodeNoVat",
                "AccountCodeNoVat",
                "AccountCodeNoVat",
                Reasons.TableId,
                string.Empty,
                new FieldAttributes(),
                Guid.Empty,
                0,
                0,
                false);

            this.BootstrapBuilder.Tables.TableRepository[Reasons.TableId].Returns(reasonsTable);

            this.BootstrapBuilder.Fields.FieldRepository = Substitute.For<FieldRepository>(this.BootstrapBuilder.System.Logger);
            this.BootstrapBuilder.Fields.FieldRepository[Reasons.Fields.Reason].Returns(reason);
            this.BootstrapBuilder.Fields.FieldRepository[Reasons.Fields.ReasonId].Returns(reasonId);
            this.BootstrapBuilder.Fields.FieldRepository[Reasons.Fields.Description].Returns(description);
            this.BootstrapBuilder.Fields.FieldRepository[Reasons.Fields.AccountCodeVat].Returns(accountCodeVat);
            this.BootstrapBuilder.Fields.FieldRepository[Reasons.Fields.AccountCodeNoVat].Returns(accountCodeNoVat);

            this.BootstrapBuilder.System.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();
            
            this.SUT = new ImportReasons(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.Reasons.SqlReasonsFactory);
        }
    }
}
