namespace ApiLibrary.ApiObjects.Base
{
    using System;
    using System.Data;
    using System.Reflection;
    using ApiLibrary.Interfaces;

    /// <summary>
    /// The logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Log(Log.MessageLevel level, LogRecord logRecord, IApiDbConnection connection = null)
        {
            if (connection == null)
            {
                using (connection = new LogDbConnection())
                {
                    return LogToDb(level, logRecord, connection);
                }
            }
            return LogToDb(level, logRecord, connection);
        }

        public static bool LogToDb(Log.MessageLevel level, LogRecord logRecord, IApiDbConnection connection)
        {
            if (level <= connection.DebugLevel)
            {
                connection.Sqlexecute.Parameters.Clear();
                connection.ErrorMessage = string.Empty;
                connection.Sqlexecute.Parameters.AddWithValue("@messageLevel", level);
                var propInfo = logRecord.ClassProperties();

                foreach (PropertyInfo propertyInfo in propInfo)
                {
                    var itemValue = propertyInfo.GetValue(logRecord);
                    connection.Sqlexecute.Parameters.AddWithValue(
                        string.Format("@{0}", propertyInfo.Name), itemValue ?? DBNull.Value);
                }

                connection.Sqlexecute.Parameters.Add("@returnCode", SqlDbType.Int);
                connection.Sqlexecute.Parameters["@returnCode"].Direction = ParameterDirection.ReturnValue;
                var returnvalue = connection.ExecuteProc("APIsaveLog");
                try
                {
                    if (connection.Sqlexecute.Parameters["@returnCode"].Value != null)
                    {
                        returnvalue = (int)connection.Sqlexecute.Parameters["@returnCode"].Value;
                    }

                    connection.Sqlexecute.Parameters.Clear();
                    if (returnvalue <= 0)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The get message level.
        /// </summary>
        /// <param name="expdata">
        /// The expdata.
        /// </param>
        /// <returns>
        /// The <see cref="Base.Log.MessageLevel"/>.
        /// </returns>
        public static Log.MessageLevel GetMessageLevel(IApiDbConnection expdata = null)
        {
            if (expdata == null)
            {
                using (expdata = new LogDbConnection())
                {
                    return expdata.DebugLevel;
                }
            }

            return expdata.DebugLevel;
        }
    }
}
