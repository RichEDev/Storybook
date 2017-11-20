using System;
using System.Data;
using System.Data.SqlTypes;

namespace SpendManagementLibrary.Helpers
{
    public static class ReaderExtensions
    {
        public static T? GetNullable<T>(this IDataReader reader, string name) where T : struct
        {
            object value = reader[name];
            return DBNull.Value.Equals(value) ? null : (T?) value;
        }

        public static T GetNullableRef<T>(this IDataReader reader, string name) where T : class
        {
            return reader.GetValueOrDefault<T>(name, null);
        }

        public static T GetValueOrDefault<T>(this IDataReader reader, string name, T defaultValue)
        {
            object value = reader[name];
            return DBNull.Value.Equals(value) ? defaultValue : (T) value;
        }

        public static T GetRequiredValue<T>(this IDataReader reader, string name) 
        {
            object value = reader[name];
            if (DBNull.Value.Equals(value))
            {
                throw new SqlNullValueException();
            }
            return (T) value;
        }

        public static T GetRequiredEnumValue<T>(this IDataReader reader, string name) where T : struct 
        {
            object value = reader[name];
            if (DBNull.Value.Equals(value))
            {
                throw new SqlNullValueException();
            }
            return (T) Enum.Parse(typeof (T), value.ToString());
        }

    }
}