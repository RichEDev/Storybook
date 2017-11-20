namespace SpendManagementLibrary.FinancialYears
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using SpendManagementLibrary.Helpers;
    
    /// <summary>
    /// Financial Year Class.
    /// Year Start and Year End are a 1900 based date to determine the financial year.
    /// </summary>
    public class FinancialYear
    {
        public int FinancialYearID { get; set; }

        public string Description { get; set; }

        public DateTime YearStart { get; set; }

        public DateTime YearEnd { get; set; }

        public bool Active { get; set; }

        public bool Primary { get; set; }

        public string YearStartMonthDay{ 
            get { return this.YearStart.ToString("dd/MM"); }
        }

        public string YearEndMonthDay
        {
            get { return this.YearEnd.ToString("dd/MM"); }
        }

        public FinancialYear()
        { }

        public FinancialYear(int financialYearID, string description, string yearStart, string yearEnd, bool active, bool primary)
        {
            this.FinancialYearID = financialYearID;
            this.Description = description;
            this.YearEnd = this.ConvertStringToDate(yearEnd);
            this.YearStart = this.ConvertStringToDate(yearStart);
            if (this.YearStart > this.YearEnd)
            {
                this.YearEnd = this.YearEnd.AddYears(1);
            }

            this.Active = active;
            this.Primary = primary;
        }

        public FinancialYear(int financialYearID, string description, DateTime yearStart, DateTime yearEnd, bool active, bool primary)
        {
            this.FinancialYearID = financialYearID;
            this.Description = description;
            this.YearEnd = yearEnd;
            this.YearStart = yearStart;
            this.Active = active;
            this.Primary = primary;
        }

        /// <summary>
        /// Saves/Updates a Financial Year
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Save(ICurrentUserBase user)
        {
            int returnValue = 0;
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string sql = "dbo.SaveFinancialYear";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeId", user.EmployeeID);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateId", user.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateId", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@SubAccountId", user.CurrentSubAccountId);        
                databaseConnection.sqlexecute.Parameters.AddWithValue("@FinancialYearId", this.FinancialYearID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@Description", this.Description);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@YearStart", this.YearStart);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@YearEnd", this.YearEnd);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@Active", this.Active);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@Primary", this.Primary);
                databaseConnection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc(sql);
                try
                {
                    returnValue = (int)databaseConnection.sqlexecute.Parameters["@identity"].Value;
                }
                catch (Exception)
                {
                    return -3;
                }
            }
            return returnValue;
        }

        public static FinancialYear Get(int financialYearID, ICurrentUserBase user)
        {
            var result = new FinancialYear();

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                var sql = "select FinancialYearID, [Description], YearStart, yearend, active, [Primary] from FinancialYears WHERE FinancialYearID = @financialYearID";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@financialYearID", financialYearID);
                using (var reader = databaseConnection.GetReader(sql))
                {
                    var financialYearIDOrd = reader.GetOrdinal("FinancialYearID");
                    var DescriptionOrd = reader.GetOrdinal("Description");
                    var YearStartOrd = reader.GetOrdinal("YearStart");
                    var YearEndOrd = reader.GetOrdinal("YearEnd");
                    var ActiveOrd = reader.GetOrdinal("Active");
                    var PrimaryOrd = reader.GetOrdinal("Primary");
                    while (reader.Read())
                    {
                        result.FinancialYearID = reader.GetInt32(financialYearIDOrd);
                        result.Description = reader.GetString(DescriptionOrd);
                        result.YearStart= reader.GetDateTime(YearStartOrd);
                        result.YearEnd = reader.GetDateTime(YearEndOrd);
                        result.Active = reader.GetBoolean(ActiveOrd);
                        result.Primary = reader.GetBoolean(PrimaryOrd);
                    }

                    reader.Close();
                }
            }

            return result;
        }

        public static FinancialYear GetPrimary(ICurrentUserBase user)
        {
            return GetPrimary(user.AccountID);
        }

        public static FinancialYear GetPrimary(int AccountID)
        {
            var result = new FinancialYear();

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                var sql = "select FinancialYearID, [Description], YearStart, yearend, active, [Primary] from FinancialYears WHERE [Primary] = 1";

                using (var reader = databaseConnection.GetReader(sql))
                {
                    var financialYearIDOrd = reader.GetOrdinal("FinancialYearID");
                    var DescriptionOrd = reader.GetOrdinal("Description");
                    var YearStartOrd = reader.GetOrdinal("YearStart");
                    var YearEndOrd = reader.GetOrdinal("YearEnd");
                    var ActiveOrd = reader.GetOrdinal("Active");
                    var PrimaryOrd = reader.GetOrdinal("Primary");
                    while (reader.Read())
                    {
                        result.FinancialYearID = reader.GetInt32(financialYearIDOrd);
                        result.Description = reader.GetString(DescriptionOrd);
                        result.YearStart = reader.GetDateTime(YearStartOrd);
                        result.YearEnd = reader.GetDateTime(YearEndOrd);
                        result.Active = reader.GetBoolean(ActiveOrd);
                        result.Primary = reader.GetBoolean(PrimaryOrd);
                    }

                    reader.Close();
                }
            }

            return result;
            
        }

        /// <summary>
        /// Deletes a Financial Year
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns>The result of the delete</returns>
        public static int Delete(int id, ICurrentUserBase user)
        {
            int returnValue = 0;
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string sql = "dbo.DeleteFinancialYear";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeId", user.EmployeeID);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateId", user.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateId", DBNull.Value);     
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@SubAccountId", user.CurrentSubAccountId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@FinancialYearId", id);
                databaseConnection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc(sql);
                try
                {
                    returnValue = (int)databaseConnection.sqlexecute.Parameters["@identity"].Value;
                }
                catch (Exception)
                {
                    return -3;
                }
            }

            return returnValue;
        }

        private DateTime ConvertStringToDate(string date)
        {
            var result = new DateTime();
            var splitString = date.Split('/');
            if (splitString.GetUpperBound(0) == 1)
            {
                int month;
                int day;
                if (int.TryParse(splitString[1], out month))
                {
                    if (int.TryParse(splitString[0], out day))
                    {
                        result = new DateTime(1900, month, day);
                    }
                }
            }

            return result;
        }
    }
}
