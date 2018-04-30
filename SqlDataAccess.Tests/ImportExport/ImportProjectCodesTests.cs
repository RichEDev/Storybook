namespace SqlDataAccess.Tests.ImportExport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Identity;
    using BusinessLogic.ImportExport;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.Tables;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;

    using SqlDataAccess.Tests.Bootstrap;

    using SQLDataAccess.ImportExport;
    using SQLDataAccess.ProjectCodes;

    using Xunit;

    public class SqlImportProjectCodesTests 
    {
        public class Constructor:SqlImportProjectCodeFactoryFixture
        {
            [Fact]
            public void WithNulls_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportProjectCodes<IProjectCodeWithUserDefinedFields>(null, null, null, null, null));
            }

            [Fact]
            public void WithNullFieldRepository_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportProjectCodes<IProjectCodeWithUserDefinedFields>(null, this.BootstrapBuilder.Fields.UserDefinedFieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.ProjectCodes.SqlProjectCodesWithUserDefinedValuesFactory));
            }

            [Fact]
            public void ValidParams_FieldRepository_ctor_ShouldNotThrowException()
            {
                 var sut = new ImportProjectCodes<IProjectCodeWithUserDefinedFields>(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.ProjectCodes.SqlProjectCodesWithUserDefinedValuesFactory);
                Assert.NotNull(sut);
            }
        }

        public class Import : SqlImportProjectCodeFactoryFixture
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

                var fileContent = new List<List<object>> {new List<object> {"Hello world", "goodbye", "udf", "20/6/1960"}, {new List<object>{"Description", string.Empty, "udf2", "31/3/2018"}}};
                var mappedFields = new List<ImportField>
                {
                    new ImportField(new Guid("6d06b15e-a157-4f56-9ff2-e488d7647219"), Guid.Empty, "DefaultValue"),
                    new ImportField(new Guid("0ad6004f-7dfd-4655-95fe-5c86ff5e4be4"), Guid.Empty, "DefaultValue"),
                    new ImportField(new Guid("473F3612-7F3C-4DCE-948E-BD0F8AE17C23"), Guid.Empty, "Udf"),
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

    public class SqlImportProjectCodeFactoryFixture
    {
        /// <summary>
        /// Gets or sets an instance of <see cref="BootstrapBuilder"/>.
        /// </summary>
        public BootstrapBuilder BootstrapBuilder { get; set; }

        /// <summary>
        /// Gets the System Under Test - <see cref="ImportProjectCodes{T}"/>
        /// </summary>
        public ImportProjectCodes<IProjectCodeWithUserDefinedFields> SUT { get; }

        public SqlImportProjectCodeFactoryFixture()
        {
            this.BootstrapBuilder = new BootstrapBuilder().WithSystem().WithFields().WithTables().WithProjectCodes();

            var projectCodeTable = new MetabaseTable("ProjectCodes", 0, "Project Codes", false, false, false, false,new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34"), new Guid("311857E6-33C2-47A4-AD6F-F38708B2A45B"), new Guid("6D06B15E-A157-4F56-9FF2-E488D7647219"), new Guid("CE235F78-82C6-4BA1-8845-034A015C5DCA"), Guid.Empty, ModuleElements.ProjectCodes, false, string.Empty);

            var projectCode = new Field(
                new Guid("6D06B15E-A157-4F56-9FF2-E488D7647219"),
                "projectcode",
                "projectcode",
                "projectcode",
                new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34"),
                string.Empty,
                new FieldAttributes(),
                Guid.Empty,
                0,
                0,
                false);

            var projectCodeId = new Field(
                new Guid("311857E6-33C2-47A4-AD6F-F38708B2A45B"),
                "projectcodeId",
                "projectcodeId",
                "projectcodeId",
                new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34"),
                string.Empty,
                new FieldAttributes(),
                Guid.Empty, 
                0, 
                0,
                false);

            this.BootstrapBuilder.Tables.TableRepository[new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34")].Returns(projectCodeTable);

            var userDefinedField = new Field(
                new Guid("473F3612-7F3C-4DCE-948E-BD0F8AE17C23"),
                "projectcodeId",
                "projectcodeId",
                "projectcodeId",
                new Guid("CE235F78-82C6-4BA1-8845-034A015C5DCA"),
                string.Empty,
                new FieldAttributes(
                    new List<IFieldAttribute>
                        {
                            new UserdefinedAttribute(
                                1,
                                "udf",
                                "udf",
                                "udf",
                                0,
                                0,
                                string.Empty,
                                0,
                                string.Empty,
                                string.Empty,
                                null,
                                0)
                        }),
                Guid.Empty, 
                0, 
                0,
                false);

            var userDefinedField2 = new Field(
                new Guid("1ECA782C-8628-49BC-A22B-0A1F64F6240C"),
                "projectcodeId",
                "projectcodeId",
                "projectcodeId",
                new Guid("CE235F78-82C6-4BA1-8845-034A015C5DCA"),
                string.Empty,
                new FieldAttributes(
                    new List<IFieldAttribute>
                        {
                            new UserdefinedAttribute(
                                1,
                                "udf",
                                "udf",
                                "udf",
                                0,
                                0,
                                string.Empty,
                                0,
                                string.Empty,
                                string.Empty,
                                null,
                                0)
                        }),
                Guid.Empty, 
                0, 
                0,
                false);


            var userDefinedDateField = new DateTimeField(userDefinedField2);

            this.BootstrapBuilder.Fields.UserDefinedFieldRepository[new Guid("473F3612-7F3C-4DCE-948E-BD0F8AE17C23")].Returns(userDefinedField);
            this.BootstrapBuilder.Fields.UserDefinedFieldRepository[new Guid("1ECA782C-8628-49BC-A22B-0A1F64F6240C")].Returns(userDefinedDateField);

            this.BootstrapBuilder.Fields.FieldRepository = Substitute.For<FieldRepository>(this.BootstrapBuilder.System.Logger);
            this.BootstrapBuilder.Fields.FieldRepository[new Guid("6D06B15E-A157-4F56-9FF2-E488D7647219")].Returns(projectCode);
            this.BootstrapBuilder.Fields.FieldRepository[new Guid("311857E6-33C2-47A4-AD6F-F38708B2A45B")].Returns(projectCodeId);

            this.BootstrapBuilder.System.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();
            
            this.SUT = new ImportProjectCodes<IProjectCodeWithUserDefinedFields>(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.ProjectCodes.SqlProjectCodesWithUserDefinedValuesFactory);
        }
    }
}
