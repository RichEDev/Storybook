using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary.FinancialYears
{
    public class FinancialYears
    {
        public static List<FinancialYear> ActiveYears(ICurrentUserBase user)
        {
            var result = new List<FinancialYear>();

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                var sql = "select FinancialYearID, [Description], YearStart, yearend, active, [Primary] from FinancialYears WHERE [Active] = 1";

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
                        var newYear = new FinancialYear();
                        newYear.FinancialYearID = reader.GetInt32(financialYearIDOrd);
                        newYear.Description = reader.GetString(DescriptionOrd);
                        newYear.YearStart = reader.GetDateTime(YearStartOrd);
                        newYear.YearEnd = reader.GetDateTime(YearEndOrd);
                        newYear.Active = reader.GetBoolean(ActiveOrd);
                        newYear.Primary = reader.GetBoolean(PrimaryOrd);
                        result.Add(newYear);
                    }

                    reader.Close();
                }
            }

            return result;
        }


        public static List<FinancialYear> Years(ICurrentUserBase user)
        {
            var result = new List<FinancialYear>();

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                var sql = "select FinancialYearID, [Description], YearStart, yearend, active, [Primary] from FinancialYears order by Description";

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
                        var newYear = new FinancialYear();
                        newYear.FinancialYearID = reader.GetInt32(financialYearIDOrd);
                        newYear.Description = reader.GetString(DescriptionOrd);
                        newYear.YearStart = reader.GetDateTime(YearStartOrd);
                        newYear.YearEnd = reader.GetDateTime(YearEndOrd);
                        newYear.Active = reader.GetBoolean(ActiveOrd);
                        newYear.Primary = reader.GetBoolean(PrimaryOrd);
                        result.Add(newYear);
                    }

                    reader.Close();
                }
            }

            return result;
        }
    }
}
