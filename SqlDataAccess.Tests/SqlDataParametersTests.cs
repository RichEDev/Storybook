namespace SqlDataAccess.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using BusinessLogic.Identity;
    using SQLDataAccess;
    using Xunit;

    public class SqlDataParametersTests
    {
        public class Indexer
        {
            [Fact]
            public void ExistingParameter_Indexer_ReturnsParameter()
            {
                SqlDataParameters sut = new SqlDataParameters();
                sut.Add(new SqlParameter("@test", SqlDbType.Bit) { Value = 10 });

                Assert.Equal("@test", sut["@test"].ParameterName);
                Assert.Null(sut["test"]);
                Assert.Null(sut["Test"]);
            }

            [Fact]
            public void NonExistingParameter_Indexer_ReturnsNull()
            {
                SqlDataParameters sut = new SqlDataParameters();

                SqlParameter parameter = sut["test"];

                Assert.Null(parameter);
            }
        }

        public class Add
        {
            [Fact]
            public void NullValue_Add_DoesNotAdd()
            {
                SqlDataParameters sut = new SqlDataParameters
                {
                    (SqlParameter) null
                };
                
                Assert.Equal(0, sut.Count());
            }

            [Fact]
            public void ExistingParameter_Add_OverwritesExisting()
            {
                SqlDataParameters sut = new SqlDataParameters
                {
                    new SqlParameter("@test", SqlDbType.Int) { Value = 99, Direction = ParameterDirection.Input }
                };

                Assert.Equal(SqlDbType.Int, sut["@test"].SqlDbType);
                Assert.Equal(99, sut["@test"].Value);
                Assert.Equal(ParameterDirection.Input, sut["@test"].Direction);

                sut.Add(new SqlParameter("@test", SqlDbType.Bit) { Value = false, Direction = ParameterDirection.ReturnValue} );

                Assert.Equal(SqlDbType.Bit, sut["@test"].SqlDbType);
                Assert.Equal(false, sut["@test"].Value);
                Assert.Equal(ParameterDirection.ReturnValue, sut["@test"].Direction);
            }
        }

        public class AddCollection
        {
            [Fact]
            public void NullValues_Add_DoesNotAdd()
            {
                SqlDataParameters sut = new SqlDataParameters
                {
                    (IEnumerable<SqlParameter>) null
                };

                Assert.Equal(0, sut.Count());
            }

            [Fact]
            public void ExistingParameters_Add_OverwritesExisting()
            {
                SqlDataParameters sut = new SqlDataParameters
                {
                    new SqlParameter("@test", SqlDbType.Int) { Value = 99, Direction = ParameterDirection.Input }
                };

                Assert.Equal(SqlDbType.Int, sut["@test"].SqlDbType);
                Assert.Equal(99, sut["@test"].Value);
                Assert.Equal(ParameterDirection.Input, sut["@test"].Direction);

                sut.Add(new []
                {
                    new SqlParameter("@test", SqlDbType.Bit) { Value = false, Direction = ParameterDirection.ReturnValue }
                });

                Assert.Equal(SqlDbType.Bit, sut["@test"].SqlDbType);
                Assert.Equal(false, sut["@test"].Value);
                Assert.Equal(ParameterDirection.ReturnValue, sut["@test"].Direction);
            }
        }

        public class AddAuditing
        {
            [Fact]
            public void NullCurrentUser_AddAuditing_ThrowsArgumentNullException()
            {
                SqlDataParameters sut = new SqlDataParameters();

                Assert.Throws<ArgumentNullException>(() => sut.AddAuditing(null));
            }

            [Fact]
            public void ZeroEmployeeIdCurrentUser_AddAuditing_ThrowsArgumentOutOfRangeException()
            {
                SqlDataParameters sut = new SqlDataParameters();

                Assert.Throws<ArgumentOutOfRangeException>(() => sut.AddAuditing(new UserIdentity(0, 0)));
            }
            
            [Fact]
            public void ValidCurrentUser_AddAuditing_AddsCUemployeeID()
            {
                SqlDataParameters sut = new SqlDataParameters();
                sut.AddAuditing(new UserIdentity(1, 121));

                Assert.NotNull(sut["@CUemployeeID"]);
                Assert.Equal(121, sut["@CUemployeeID"].Value);
                Assert.Equal(SqlDbType.Int, sut["@CUemployeeID"].SqlDbType);
            }

            [Fact]
            public void ValidCurrentUserWithDelegate_AddAuditing_AddsCUdelegateIDWithDelegateId()
            {
                SqlDataParameters sut = new SqlDataParameters();
                sut.AddAuditing(new UserIdentity(1, 121, 99));

                Assert.NotNull(sut["@CUdelegateID"]);
                Assert.Equal(99, sut["@CUdelegateID"].Value);
                Assert.Equal(SqlDbType.Int, sut["@CUdelegateID"].SqlDbType);
            }

            [Fact]
            public void ValidCurrentUserNotADelegate_AddAuditing_AddCUdelegateIDWithDBNull()
            {
                SqlDataParameters sut = new SqlDataParameters();
                sut.AddAuditing(new UserIdentity(1, 121));
                Assert.NotNull(sut["@CUdelegateID"]);
                Assert.Equal(DBNull.Value, sut["@CUdelegateID"].Value);
                Assert.Equal(SqlDbType.Int, sut["@CUdelegateID"].SqlDbType);
            }
        }
    }
}
