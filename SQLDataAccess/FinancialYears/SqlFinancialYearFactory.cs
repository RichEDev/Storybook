namespace SQLDataAccess.FinancialYears
{
    using System;
    using System.Data.SqlClient;

    using BusinessLogic.DataConnections;
    using BusinessLogic.FinancialYears;

    /// <summary>
    /// The sql financial year factory.
    /// </summary>
    public class SqlFinancialYearFactory : FinancialYearRepository
    {
        /// <summary>
        /// The _customer database connection.
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDatabaseConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFinancialYearFactory"/> class.
        /// </summary>
        /// <param name="customerDataConnection">
        /// The customer data connection.
        /// </param>
        public SqlFinancialYearFactory(ICustomerDataConnection<SqlParameter> customerDataConnection)
        {
            this._customerDatabaseConnection = customerDataConnection;
        }

        /// <summary>
        /// Gets the FincialYear
        /// </summary>
        /// <returns>The date time of the start and end of the financial year</returns>
        public override DateTime[] GetFinancialYear()
        {
            string start = "06/04";
            string end = "05/04";

            string strsql = "select yearstart, yearend from FinancialYears where [Primary] = 1";

            using (var reader = this._customerDatabaseConnection.GetReader(strsql))
            {
                int financialYearStartOrd = reader.GetOrdinal("yearstart");
                int financialYearEndOrd = reader.GetOrdinal("yearend");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(financialYearStartOrd))
                    {
                        start = reader.GetString(financialYearStartOrd);
                    }

                    if (!reader.IsDBNull(financialYearEndOrd))
                    {
                        end = reader.GetString(financialYearEndOrd);
                    }
                }

                reader.Close();
            }

            string[] startItems = start.Split('/');
            string[] endItems = end.Split('/');

            DateTime financialyearstart = new DateTime(DateTime.Today.Year, int.Parse(startItems[1]), int.Parse(startItems[0]));
            DateTime financialyearend = new DateTime(DateTime.Today.AddYears(1).Year, int.Parse(endItems[1]), int.Parse(endItems[0]), 23, 59, 59);
            if (int.Parse(endItems[1]) > int.Parse(startItems[1]))
            {
                financialyearend = financialyearend.AddYears(-1);
            }

            return new[] { financialyearstart, financialyearend };
        }
    }
}
