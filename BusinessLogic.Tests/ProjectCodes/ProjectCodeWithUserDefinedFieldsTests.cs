namespace BusinessLogic.Tests.ProjectCodes
{
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.UserDefinedFields;
    using Xunit;

    public class ProjectCodeWithUserDefinedFieldsTests
    {
        public class Ctor
        {
            [Fact]
            public void InitializingDefaultConstructor_CreatesDefaultProjectCodeWithUserDefinedFields()
            {
                ProjectCodeWithUserDefinedFields sut = new ProjectCodeWithUserDefinedFields();
                Assert.Equal(0, sut.Id);
                Assert.Null(sut.Name);
                Assert.Null(sut.Description);
                Assert.Equal(false, sut.Archived);
                Assert.Equal(false, sut.Rechargeable);

                // Should be initialized with an empty collection.
                Assert.Equal(0, sut.UserDefinedFieldValues.Count);
            }

            [Fact]
            public void Initializing_ProjectCode_SetsProperties()
            {
                ProjectCode projectCode = new ProjectCode(11, "ref", "desc", true, false);
                UserDefinedFieldValueCollection userDefinedFieldValueCollection = new UserDefinedFieldValueCollection();
                ProjectCodeWithUserDefinedFields sut = new ProjectCodeWithUserDefinedFields(projectCode, userDefinedFieldValueCollection);

                // Checks the properties are calling correctly
                Assert.Equal(11, sut.Id);
                Assert.Equal("ref", sut.Name);
                Assert.Equal("desc", sut.Description);
                Assert.Equal(true, sut.Archived);
                Assert.Equal(false, sut.Rechargeable);
                Assert.Equal(userDefinedFieldValueCollection, sut.UserDefinedFieldValues);

                // switch the bools just to make sure
                projectCode = new ProjectCode(11, "ref", "desc", false, true);
                sut = new ProjectCodeWithUserDefinedFields(projectCode, userDefinedFieldValueCollection);
                Assert.Equal(false, sut.Archived);
                Assert.Equal(true, sut.Rechargeable);
            }

            [Fact]
            public void Set_ProjectCodeProperties_SetsProperties()
            {
                ProjectCode projectCode = new ProjectCode(11, "ref", "desc", true, false);
                UserDefinedFieldValueCollection userDefinedFieldValueCollection = new UserDefinedFieldValueCollection();
                ProjectCodeWithUserDefinedFields sut = new ProjectCodeWithUserDefinedFields(projectCode, userDefinedFieldValueCollection)
                {
                    Id = 12,
                    Name = "new ref",
                    Description = "new desc",
                    Archived = false
                };

                Assert.Equal(12, projectCode.Id);
                Assert.Equal("new ref", projectCode.Name);
                Assert.Equal("new desc", projectCode.Description);
                Assert.Equal(false, projectCode.Archived);

                Assert.Equal(12, sut.Id);
                Assert.Equal("new ref", sut.Name);
                Assert.Equal("new desc", sut.Description);
                Assert.Equal(false, sut.Archived);
            }
        }

        public class ToString
        {
            [Fact]
            public void ToStringUseDescription_ToString_UsesDescription()
            {
                ProjectCodeWithUserDefinedFields sut = new ProjectCodeWithUserDefinedFields {Description = "testDescription"};

                Assert.Equal("testDescription", sut.ToString(true));
            }

            [Fact]
            public void ToStringUseName_ToString_UsesName()
            {
                ProjectCodeWithUserDefinedFields sut = new ProjectCodeWithUserDefinedFields { Name = "testName" };

                Assert.Equal("testName", sut.ToString(false));

            }
        }
    }
}
