using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTest2012Ultimate.DatabaseMock
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Moq;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The reader.
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// The normal database.
        /// </summary>
        /// <param name="mockReaderData">
        /// The data to return from reader operations.
        /// </param>
        /// <param name="execProcReturnValue">
        /// The execute procedure return value.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IDBConnection> NormalDatabase(IEnumerable<MockReaderData> mockReaderData, int execProcReturnValue = 0)
        {
            var currentDatabase = new Mock<IDBConnection>();
            currentDatabase.SetupAllProperties();

            foreach (MockReaderData mockData in mockReaderData)
            {
                //Mock<IDataReader> mockReader = CreateMockReader(mockData);
                var sql = mockData.Sql;
                MockReaderData data = mockData; //ensure each call returns corresponding data
                if (string.IsNullOrEmpty(sql))
                {
                    currentDatabase.Setup(x => x.GetReader(It.IsAny<string>(), CommandType.StoredProcedure)).Returns(() => CreateMockReader(data).Object);
                    currentDatabase.Setup(x => x.GetReader(It.IsAny<string>(), CommandType.Text)).Returns(() => CreateMockReader(data).Object);
                }
                else
                {
                    currentDatabase.Setup(x => x.GetReader(sql, CommandType.StoredProcedure)).Returns(() => CreateMockReader(data).Object);
                    currentDatabase.Setup(x => x.GetReader(sql, CommandType.Text)).Returns(() => CreateMockReader(data).Object);
                }
            }

            currentDatabase.Setup(x => x.sqlexecute).Returns(new SqlCommand());

            return currentDatabase;
        }

        /// <summary>
        /// The create mock reader.
        /// </summary>
        /// <param name="mockData">
        /// The fields.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        internal static Mock<IDataReader> CreateMockReader(MockReaderData mockData)
        {
            var mockReader = new Mock<IDataReader>();

            int current = -1;
            //get the number of values in the fields, making sure they've all got the same number of values (throws if not):
            int rowCount = mockData.Count;

            //make Read increment the row number, and return whether it's reached the end:
            mockReader.Setup(r => r.Read()).Returns(() => ++current < rowCount);

            //set GetOrdinal to return the index of the field that has the same name as the string passed in:
            mockReader.Setup(r => r.GetOrdinal(It.IsAny<string>())).Returns<string>(mockData.GetOrdinal);

            //set the indexer to return the current value of the field that has the name passed in:
            mockReader.Setup(r => r[It.IsAny<string>()]).Returns<string>(s => mockData.GetValue(s, current));

            //set all the get by type methods to return the current value of the field with the index passed in
            mockReader.Setup(r => r.GetInt32(It.IsAny<int>())).Returns<int>(i => (int) mockData.GetValue(i, current));
            mockReader.Setup(r => r.GetInt64(It.IsAny<int>())).Returns<int>(i => (long)mockData.GetValue(i, current));
            mockReader.Setup(r => r.GetString(It.IsAny<int>())).Returns<int>(i => (string)mockData.GetValue(i, current));
            mockReader.Setup(r => r.GetDateTime(It.IsAny<int>())).Returns<int>(i => (DateTime)mockData.GetValue(i, current));
            mockReader.Setup(r => r.GetByte(It.IsAny<int>())).Returns<int>(i => (byte) Convert.ChangeType(mockData.GetValue(i, current), typeof(byte))); //use Convert as it is sometimes an enum, which doesn't cast directly to a byte
            mockReader.Setup(r => r.GetBoolean(It.IsAny<int>())).Returns<int>(i => (bool) mockData.GetValue(i, current));
            mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns<int>(i => mockData.GetValue(i, current) == null);
            
            return mockReader;
        }

        /// <summary>
        /// The mock reader data from class data.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <returns>
        /// The <see cref="MockReaderData"/>.
        /// </returns>
        public static MockReaderData MockReaderDataFromClassData(string sql, List<object> objects)
        {
            return new MockReaderData(sql, objects);
                    }
                }

    /// <summary>
    /// The mock reader data.
    /// </summary>
    public class MockReaderData
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MockReaderData"/> class.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <param name="objects">The data returned by the reader</param>
        public MockReaderData(string sql, List<object> objects) 
        {
            this.Sql = sql;
            this.objects = objects;
            var ordinalsAndFields = Regex.Match(sql, @"(?<=\bselect\b).+(?=\bfrom\b)", RegexOptions.IgnoreCase).Value //get the bit between 'select' and 'from'
                                  .Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().Trim('[', ']').ToLower()) // remove spaces and any [] brackets, and lower-case-ise
                                  .Select((value, ordinal) => new { value, ordinal }) //get the ordinal
                                  .ToArray();
            fieldsByOrdinal = ordinalsAndFields.ToDictionary(a => a.ordinal, a => a.value.ToLower());
            ordinalsByField = ordinalsAndFields.ToDictionary(a => a.value.ToLower(), a => a.ordinal);
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        public string Sql { get; private set; }

        public int Count { get { return objects.Count; } }

        /// <summary>
        /// Gets  the fields.
        /// </summary>
        private readonly List<object> objects;
        private readonly Dictionary<int, string> fieldsByOrdinal;
        private readonly Dictionary<string, int> ordinalsByField;

        private Dictionary<string, List<object>> extraFields = new Dictionary<string, List<object>>();
        private Dictionary<string, Delegate> resolvers = new Dictionary<string, Delegate>();
        

        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="values">
        /// The objects.
        /// </param>
        public MockReaderData AddField(string fieldName, List<object> values)
        {
            if(values.Count != objects.Count) throw new ArgumentException("Wrong number of values - there are " + objects.Count + " objects in the data", "values");
            extraFields.Add(fieldName, values);
            return this;
        }


    /// <summary>
        /// Adds a 'converter' to specify how to resolve the field value from the object. Useful when the SQL field name doesn't quite match the property name on the object, or when it's a property of a child object.
    /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The field name in SQL.</param>
        /// <param name="resolver">The function that specifies how to resolve the return value of reader["fieldName"] from the object.</param>
        public MockReaderData AddAlias<T>(string fieldName, Func<T, object> resolver)
    {
            resolvers.Add(fieldName.ToLower(), resolver);
            return this;
        }

        public object GetValue(int ordinal, int row)
        {
            return GetValue(fieldsByOrdinal[ordinal], row);
        }

        public int GetOrdinal(string field)
        {
            int ordinal;
            if (!ordinalsByField.TryGetValue(field.ToLower(), out ordinal))
            {
                throw new KeyNotFoundException("The field " + field + " was not present in the SQL string.");
            }
            return ordinal;
        }

        public object GetValue(string fieldName, int row)
        {
            //if we've explicitly specified a list of objects that we want to return for a particular field, return the value from that list.
            List<object> extraFieldObjects;
            if (extraFields.TryGetValue(fieldName, out extraFieldObjects))
            {
                return extraFieldObjects[row];
            }

            //otherwise, it's going to be from the object in the data.
            Delegate alias;
            if (resolvers.TryGetValue(fieldName, out alias))
            {
                //... but if a custom converter has been specified, invoke that.
                return alias.DynamicInvoke(objects[row]);
            }
            else
            {
                //otherwise, assume the sql field name corresponds to a property.
                var type = objects[row].GetType();

                var properties = type.GetProperties();
                try
                {
                    return (from propertyInfo in properties where propertyInfo.Name.ToLower() == fieldName select propertyInfo.GetValue(objects[row])).FirstOrDefault();
                }
                catch
                {
                    throw new InvalidOperationException("Attempt to access reader field '" + fieldName +
                                                        "' but the object type '" + type.Name +
                                                        "' does not have a matching property, and an alias has not been specified. Use MockDataReader.AddAlias to specify how to resolve this field from the object.");
                }
            }
        }
    }
}
