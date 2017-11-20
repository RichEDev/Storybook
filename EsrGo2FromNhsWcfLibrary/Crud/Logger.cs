namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Data;
    using System.Reflection;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Interfaces;

    /// <summary>
    /// The logger.
    /// </summary>
    internal class Logger
    {
        /// <summary>
        /// The connection.
        /// </summary>
        private IApiDbConnection expdata;

        /// <summary>
        /// Initialises a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        internal Logger(IApiDbConnection connection)
        {
            this.expdata = connection;
            this.ConnectionValid = this.CheckConnection();
        }

        /// <summary>
        /// Gets or sets a value indicating whether connection valid.
        /// </summary>
        internal bool ConnectionValid { get; set; }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Log(ApiLog.MessageLevel level, LogRecord logRecord)
        {
            if (level <= (ApiLog.MessageLevel)this.expdata.DebugLevel)
            {
                this.expdata.Sqlexecute.Parameters.Clear();
                this.expdata.ErrorMessage = string.Empty;
                this.expdata.Sqlexecute.Parameters.AddWithValue("@messageLevel", level);
                var propInfo = logRecord.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in propInfo)
                {
                    var itemValue = propertyInfo.GetValue(logRecord);
                    this.expdata.Sqlexecute.Parameters.AddWithValue(
                        string.Format("@{0}", propertyInfo.Name), itemValue ?? DBNull.Value);
                }

                this.expdata.Sqlexecute.Parameters.Add("@returnCode", SqlDbType.Int);
                this.expdata.Sqlexecute.Parameters["@returnCode"].Direction = ParameterDirection.ReturnValue;
                this.expdata.ExecuteProc("APIsaveLog");
                try
                {
                    this.expdata.Sqlexecute.Parameters.Clear();
                    return string.IsNullOrEmpty(this.expdata.ErrorMessage);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// The check connection.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckConnection()
        {
            try
            {
                if (this.expdata == null)
                {
                    this.expdata = new LogDbConnection();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return this.expdata.ConnectionStringValid;
        }
    }
}