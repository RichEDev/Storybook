namespace UnitTest2012Ultimate.API.CRUD.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;

    using EsrGo2FromNhsWcfLibrary.Base;

    using Moq;
    using Moq.Language.Flow;

    using IApiDbConnection = EsrGo2FromNhsWcfLibrary.Interfaces.IApiDbConnection;

    /// <summary>
    /// The API database MOQ object. 
    /// </summary>
    public static class ApiDataBaseMoq
    {
        /// <summary>
        /// The database.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="dataClassType">
        /// The data Class Type.
        /// </param>
        /// <param name="execProcReturnValue">
        /// The exec Proc Return Value.
        /// </param>
        /// <returns>
        /// The <see cref="IApiDbConnection"/>.
        /// </returns>
        public static IApiDbConnection Database(SortedList<string, List<object>> fields, Type dataClassType, int execProcReturnValue = 0)
        {
            return NormalDatabase(fields, dataClassType, execProcReturnValue).Object;
        }

        /// <summary>
        /// The normal database.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="dataClassType">
        /// The data Class Type.
        /// </param>
        /// <param name="execProcReturnValue">
        /// The exec procedure Return Value.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IApiDbConnection> NormalDatabase(SortedList<string, List<object>> fields, Type dataClassType, int execProcReturnValue = 0)
        {
            var currentDatabase = new Mock<IApiDbConnection>();
            currentDatabase.SetupAllProperties();
            currentDatabase.Setup(x => x.ConnectionStringValid).Returns(true);
            currentDatabase.Setup(x => x.GetAccountDetails(GlobalTestVariables.AccountId)).Returns(true);
            var mockReader = new Mock<IDataReader>();

            int rowCount = 0;
            if (fields.Values.Count != 0)
            {
                rowCount = fields.Values[0].Count;
            }

            var recordReads = new bool[rowCount + 1];
            for (int fieldCount = 0; fieldCount < rowCount; fieldCount++)
            {
                recordReads[fieldCount] = true;
            }

            recordReads[rowCount] = false;
            mockReader.Setup(reader => reader.Read()).ReturnsInOrder(recordReads);
            int itemNo = 0;
            foreach (KeyValuePair<string, List<object>> keyValuePair in fields)
            {
                itemNo += 1;
                mockReader.Setup(reader => reader.GetOrdinal(keyValuePair.Key)).Returns(itemNo);
                if (keyValuePair.Value[0] is int)
                {
                    var integerItems = new int[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (int o in keyValuePair.Value)
                    {
                        integerItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetInt32(itemNo)).ReturnsInOrder(integerItems);
                }

                if (keyValuePair.Value[0] is long)
                {
                    var integerItems = new long[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (long o in keyValuePair.Value)
                    {
                        integerItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetInt64(itemNo)).ReturnsInOrder(integerItems);
                }


                if (keyValuePair.Value[0] is string)
                {
                    var stringItems = new string[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (string o in keyValuePair.Value)
                    {
                        stringItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetString(itemNo)).ReturnsInOrder(stringItems);
                }

                if (keyValuePair.Value[0] is DateTime)
                {
                    var dateTimeItems = new DateTime[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (DateTime o in keyValuePair.Value)
                    {
                        dateTimeItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetDateTime(itemNo)).ReturnsInOrder(dateTimeItems);
                }

                if (keyValuePair.Value[0] is byte)
                {
                    var byteItems = new byte[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (byte o in keyValuePair.Value)
                    {
                        byteItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetByte(itemNo)).ReturnsInOrder(byteItems);
                }

                if (keyValuePair.Value[0] is bool)
                {
                    var boolItems = new bool[keyValuePair.Value.Count];
                    int itemCount = 0;
                    foreach (bool o in keyValuePair.Value)
                    {
                        boolItems[itemCount] = o;
                        itemCount += 1;
                    }

                    mockReader.Setup(reader => reader.GetBoolean(itemNo)).ReturnsInOrder(boolItems);
                }

                mockReader.Setup(reader => reader[keyValuePair.Key]).ReturnsInOrder(keyValuePair.Value);
            }

            currentDatabase.Setup(x => x.Sqlexecute).Returns(new SqlCommand());

            var currentType = Activator.CreateInstance(dataClassType) as DataClassBase;

            currentDatabase.Setup(x => x.GetStoredProcReader(currentType.ReadStoredProcedure)).Returns(mockReader.Object);
            currentDatabase.Setup(x => x.GetStoredProcReader(currentType.SaveStoredProcedure)).Returns(mockReader.Object);
            currentDatabase.Setup(x => x.GetStoredProcReader(currentType.ReadBatchStoredProcedure)).Returns(mockReader.Object);
            currentDatabase.Setup(x => x.GetStoredProcReader(currentType.SaveStoredProcedureBatch)).Returns(mockReader.Object);
            currentDatabase.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(execProcReturnValue);
            return currentDatabase;
        }
    }

    /// <summary>
    /// The moq extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class MoqExtensions
    {
        /// <summary>
        /// The returns values in order for mock objects.
        /// </summary>
        /// <param name="setup">
        /// The setup.
        /// </param>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TResult">
        /// </typeparam>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, IEnumerable<TResult> results)
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}
