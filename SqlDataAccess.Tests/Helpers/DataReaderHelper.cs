namespace SqlDataAccess.Tests.Helpers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using NSubstitute;

    /// <summary>
    /// Helper to create mock'd <see cref="DbDataReader"/>
    /// </summary>
    public class DataReaderHelper
    {
        /// <summary>
        /// Mock a <see cref="IDataReader"/> making it return the list of <paramref name="rowData"/>.
        /// </summary>
        /// <param name="rowData">The data to make the return, in order.</param>
        /// <returns>A mocked <see cref="IDataReader"/> which will allow allow indexer and GetX usage. FieldCount is set to the <paramref name="rowData"/> length and Read will act as if one row has been returned.</returns>
        public DbDataReader GetSingleRecordReader(params object[] rowData)
        {
            // Mock the IDataReader
            DbDataReader dataReader = Substitute.For<DbDataReader>();

            for (int i = 0; i < rowData.Length; i++)
            {
                dataReader[i].Returns(rowData[i]);

                switch (rowData[i])
                {
                    case string _:
                        dataReader.GetString(Arg.Is(i)).Returns(rowData[i]);
                        break;
                    case int _:
                        dataReader.GetInt32(Arg.Is(i)).Returns(Convert.ToInt32(rowData[i]));
                        break;
                    default:
                        throw new InvalidCastException($"({rowData[i].GetType()}){rowData[i]} is not currently caught, you will need to add support for it.");
                }
            }

            dataReader.Read().Returns(true, false);
            dataReader.FieldCount.Returns(rowData.Length);

            return dataReader;
        }

        /// <summary>
        /// Mock a <see cref="IDataReader"/> making it return nothing.
        /// </summary>
        /// <param name="fieldCount">The number of fields returned by the <see cref="IDataReader"/></param>
        /// <returns></returns>
        public DbDataReader GetEmptyRecordReader(int fieldCount)
        {
            // Mock the IDataReader
            DbDataReader dataReader = Substitute.For<DbDataReader>();

            dataReader.Read().Returns(false);
            dataReader.FieldCount.Returns(fieldCount);

            return dataReader;
        }
    }
}
