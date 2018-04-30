namespace SqlDataAccess.Tests.ImportExport
{
    using System;
    using System.Collections.Generic;
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

    public class ImportFilefactoryTests
    {
        public class Constructor:ImportFileFactoryFixture
        {
            [Fact]
            public void WithNulls_ctor_ShouldThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportFileFactory(null, null, null, null, null));
            }

            [Fact]
            public void WithNulls_ctor_ShouldThrowException2()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, null, null, null, null));
            }


            [Fact]
            public void WithNulls_ctor_ShouldThrowException3()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository , null, null, null));
            }

            [Fact]
            public void WithNulls_ctor_ShouldThrowException4()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository , this.BootstrapBuilder.Tables.TableRepository, null, null));
            }

            [Fact]
            public void WithNulls_ctor_ShouldThrowException5()
            {
                Assert.Throws<ArgumentNullException>(() => new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository , this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, null));
            }

            [Fact]
            public void ValidParams_ShouldReturnObject()
            {
                var result = new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository , this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.ProjectCodes.SqlProjectCodesWithUserDefinedValuesFactory);
                Assert.NotNull(result);
            }
        }

        public class New : ImportFileFactoryFixture
        {
            [Fact]
            public void WithEmptyGuid_ReturnsNull()
            {
                var result = this.SUT.New(Guid.Empty);
                Assert.Null(result);
            }

            [Theory]
            [InlineData("e1ef483c-7870-42ce-be54-ecc5c1d5fb34")]
            public void WithGuidReturnsValidObject(string tableId)
            {
                var result = this.SUT.New(new Guid(tableId));
                Assert.NotNull(result);
            }
        }
    }

     public class ImportFileFactoryFixture
    {
        /// <summary>
        /// Gets the System Under Test - <see cref="ImportProjectCodes{T}"/>
        /// </summary>
        public ImportFileFactory SUT { get; }

        /// <summary>
        /// Gets or sets an instance of <see cref="BootstrapBuilder"/>.
        /// </summary>
        public BootstrapBuilder BootstrapBuilder { get; set; }

        public ImportFileFactoryFixture()
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
                                String.Empty,
                                String.Empty,
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
                                String.Empty,
                                String.Empty,
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
            
            this.SUT = new ImportFileFactory(this.BootstrapBuilder.Fields.FieldRepository, this.BootstrapBuilder.Fields.UserDefinedFieldRepository, this.BootstrapBuilder.Tables.TableRepository, this.BootstrapBuilder.System.CustomerDataConnection, this.BootstrapBuilder.ProjectCodes.SqlProjectCodesWithUserDefinedValuesFactory);
        }
    }
}
