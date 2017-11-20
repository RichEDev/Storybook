using System;
using System.Data;
using System.Globalization;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary.DocumentMerge
{
    using System.Linq;

    /// <summary>
    /// Holds the Grouping Field Filter information
    /// </summary>
    [Serializable]
    public class TorchGroupingFieldFilter
    {

        #region Properties
        /// <summary>
        /// The column name the filter is applied to.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// The Filter condition
        /// </summary>
        public ConditionType conditionType { get; set; }

        /// <summary>
        /// The Condition filter value one
        /// </summary>
        public string criterionOne { get; set; }

        /// <summary>
        /// The Condition filter value two
        /// </summary>
        public string criterionTwo { get; set; }

        /// <summary>
        /// The TypeText filter 
        /// </summary>
        public string conditionTypeText { get; set; }


        /// <summary>
        /// The FieldType filter 
        /// </summary>
        public string fieldType { get; set; }

        #endregion

        public TorchGroupingFieldFilter()
        {
        }

        public TorchGroupingFieldFilter(string columnname, ConditionType conditiontype, string valueone, string valuetwo, string typetext, string fieldtype)
        {
            ColumnName = columnname;
            conditionType = conditiontype;
            criterionOne = valueone;
            criterionTwo = valuetwo;
            conditionTypeText = typetext;
            fieldType = fieldtype;
        }

        public string GetReverseFilterQuery(string columnName, int accountId)
        {
            string currentFilterExpression = string.Empty;
            string operatorLike = "%";
            string maybeeeApostropheee = string.Empty;

            switch (this.fieldType)
            {
                // attachment
                case "AT":
                    break;
                // bit
                case "FX":
                case "X":
                case "Y":
                    break;
                // date time
                case "D":
                case "DT":
                    maybeeeApostropheee = "'";
                    break;
                // time
                case "T":
                // decimal
                case "C":
                    break;
                case "FC":
                case "FD":
                case "M":
                case "F":
                    break;
                // guid
                case "FU":
                case "G":
                case "U":
                    maybeeeApostropheee = "'";
                    break;
                // integer
                case "FI":
                case "I":
                case "N":
                    break;
                // string
                case "FS":
                case "LT":
                case "S":
                    maybeeeApostropheee = "'";
                    break;

            }

            string operator1 = string.Format("{0}{1}{0}", maybeeeApostropheee, this.criterionOne);
            operatorLike = string.Format("{0}{1}{2}{1}{0}", maybeeeApostropheee, operatorLike, this.criterionOne);
            string operator2 = string.Format("{0}{1}{0}", maybeeeApostropheee, this.criterionOne);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime startDate;
            DateTime endDate;
            DateTime[] dates;

            switch (conditionType)
            {
                case ConditionType.On:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture);
                        operator2 =
                            Convert.ToDateTime(operator1)
                                .AddDays(1)
                                .ToString(CultureInfo.InvariantCulture)
                                .Apostrophize();
                        operator1 = operator1.Apostrophize();
                    }
                    
                    currentFilterExpression += string.Format("{0} < {1} OR {0} >= {2}", columnName, operator1.Apostrophize(), operator2);
                    break;
                case ConditionType.NotOn:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture);
                        operator2 =
                            Convert.ToDateTime(operator1)
                                .AddDays(1)
                                .ToString(CultureInfo.InvariantCulture)
                                .Apostrophize();
                        operator1 = operator1.Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} >= {1} OR {0} < {2}", columnName, operator1, operator2);
                    break;
                case ConditionType.After:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} <= {1}", columnName, operator1);
                    break;
                case ConditionType.Before:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} > {1}", columnName, operator1);
                    break;
                case ConditionType.OnOrAfter:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} < {1}", columnName, operator1);
                    break;
                case ConditionType.OnOrBefore:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} > {1}", columnName, operator1);
                    break;
                case ConditionType.Equals:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} <> {1}", columnName, operator1);
                    break;
                case ConditionType.DoesNotEqual:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} = {1}", columnName, operator1);
                    break;
                case ConditionType.In:
                    string[] criteria = this.criterionOne.Split(',');
                    currentFilterExpression = criteria.Aggregate(currentFilterExpression, (current, criterion) => current + string.Format("{0},", criterion.Trim().Apostrophize()));
                    currentFilterExpression = currentFilterExpression.RemoveLast(",");
                    currentFilterExpression = string.Format("{0} not in ({1})", columnName, currentFilterExpression);
                    break;
                case ConditionType.GreaterThan:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} <= {1}", columnName, operator1);
                    break;
                case ConditionType.LessThan:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} >= {1}", columnName, operator1);
                    break;
                case ConditionType.GreaterThanEqualTo:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }

                    currentFilterExpression += string.Format("{0} < {1}", columnName, operator1);
                    break;
                case ConditionType.LessThanEqualTo:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Apostrophize();
                    }
                    currentFilterExpression += string.Format("{0} > {1}", columnName, operator1);
                    break;
                case ConditionType.Like:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Likanize().Apostrophize();
                        currentFilterExpression += string.Format("{0} not like {1}", columnName, operator1);
                    }
                    else
                    {
                        currentFilterExpression += string.Format("{0} not like {1}", columnName, operatorLike);    
                    }

                    break;
                case ConditionType.NotLike:
                    if (fieldType == "T")
                    {
                        operator1 = GetDateTimeParameterFromTimeCriteria(operator1).ToString(CultureInfo.InvariantCulture).Likanize().Apostrophize();
                        currentFilterExpression += string.Format("{0} like {1}", columnName, operator1);
                    }
                    else
                    {
                        currentFilterExpression += string.Format("{0} like {1}", columnName, operatorLike);
                    }
                    
                    break;
                case ConditionType.Between:
                    currentFilterExpression += string.Format("{0} not between {1} and {2}", columnName, operator1, operator2);
                    break;
                case ConditionType.ContainsData:
                    currentFilterExpression += string.Format("{0} is null OR {0} = ''", columnName);
                    break;
                case ConditionType.DoesNotContainData:
                    currentFilterExpression += string.Format("{0} is not null or {0} <> ''", columnName);
                    break;
                case ConditionType.Yesterday:
                    currentFilterExpression += string.Format("{0} < {1} or {0} >= {2}", columnName, yesterday.ToString(CultureInfo.InvariantCulture).Apostrophize(), today.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.Today:
                    currentFilterExpression += string.Format("{0} < {1} or {0} >= {2}", columnName, tomorrow.ToString(CultureInfo.InvariantCulture).Apostrophize(), today.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.Tomorrow:
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", tomorrow.AddDays(1).ToString(CultureInfo.InvariantCulture).Apostrophize(), tomorrow.ToString(CultureInfo.InvariantCulture).Apostrophize(), today.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.Next7Days:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(8).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.Last7Days:
                    startDate = DateTime.Today.AddDays(-7);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextWeek:
                    startDate = GetStartOfWeek().AddDays(7);
                    endDate = startDate.AddDays(7).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastWeek:
                    startDate = GetStartOfWeek();
                    startDate = startDate.AddDays(-7);
                    endDate = startDate.AddDays(7).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.ThisWeek:
                    startDate = GetStartOfWeek();
                    endDate = startDate.AddDays(7).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    startDate = startDate.AddMonths(1);
                    endDate = new DateTime(
                                startDate.Year,
                                startDate.Month,
                                DateTime.DaysInMonth(startDate.Year, startDate.Month),
                                23,
                                59,
                                59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    startDate = startDate.AddMonths(-1);
                    endDate = new DateTime(
                        startDate.Year,
                        startDate.Month,
                        DateTime.DaysInMonth(startDate.Year, startDate.Month),
                        23,
                        59,
                        59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.ThisMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    endDate = new DateTime(
                        startDate.Year,
                        startDate.Month,
                        DateTime.DaysInMonth(startDate.Year, startDate.Month),
                        23,
                        59,
                        59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextYear:
                    startDate = new DateTime(DateTime.Today.Year + 1, 01, 01);
                    endDate = new DateTime(DateTime.Today.Year + 1, 12, 31, 23, 59, 59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastYear:
                    startDate = new DateTime(DateTime.Today.Year - 1, 01, 01);
                    endDate = new DateTime(DateTime.Today.Year - 1, 12, 31, 23, 59, 59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.ThisYear:
                    startDate = new DateTime(DateTime.Today.Year, 01, 01);
                    endDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextFinancialYear:
                    dates = GetFinancialYear(accountId);
                    startDate = dates[0];
                    endDate = dates[1];
                    startDate = startDate.AddYears(1);
                    endDate = endDate.AddYears(1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastFinancialYear:
                    dates = GetFinancialYear(accountId);
                    startDate = dates[0];
                    endDate = dates[1];
                    startDate = startDate.AddYears(-1);
                    endDate = endDate.AddYears(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.ThisFinancialYear:
                    dates = GetFinancialYear(accountId);
                    startDate = dates[0];
                    endDate = dates[1];
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastXDays:
                    startDate = DateTime.Today.AddDays(Convert.ToInt32(operator1) / -1);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextXDays:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(Convert.ToInt32(operator1) + 1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastXWeeks:
                    startDate = DateTime.Today;
                    startDate = startDate.AddDays(-7 * Convert.ToInt32(operator1));
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextXWeeks:
                    startDate = DateTime.Today;
                    endDate = startDate.AddDays((7 * Convert.ToInt32(operator1)) + 1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastXMonths:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    startDate = startDate.AddMonths(Convert.ToInt32(operator1) / -1);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextXMonths:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    endDate = startDate.AddMonths(Convert.ToInt32(operator1)).AddSeconds(-1);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastXYears:
                    startDate = new DateTime(DateTime.Today.Year - Convert.ToInt32(operator1), 01, 01);
                    endDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextXYears:
                    startDate = new DateTime(DateTime.Today.Year, 01, 01);
                    endDate = new DateTime(DateTime.Today.Year + Convert.ToInt32(operator1), 12, 31, 23, 59, 59);
                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.NextTaxYear:
                    if (DateTime.Today.Month >= 4)
                    {
                        startDate = new DateTime(DateTime.Today.Year + 1, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year + 2, 04, 05, 23, 59, 59);
                    }
                    else
                    {
                        startDate = new DateTime(DateTime.Today.Year, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                    }

                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.LastTaxYear:
                    if (DateTime.Today.Month >= 4)
                    {
                        startDate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                    }
                    else
                    {
                        startDate = new DateTime(DateTime.Today.Year - 2, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year - 1, 04, 05, 23, 59, 59);
                    }

                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                case ConditionType.ThisTaxYear:
                    if (DateTime.Today.Month >= 4)
                    {
                        startDate = new DateTime(DateTime.Today.Year, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                    }
                    else
                    {
                        startDate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                        endDate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                    }

                    currentFilterExpression += string.Format("{0} < {1} or {0} > {2}", columnName, startDate.ToString(CultureInfo.InvariantCulture).Apostrophize(), endDate.ToString(CultureInfo.InvariantCulture).Apostrophize());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return currentFilterExpression;
        }

        /// <summary>
        /// The get date time parameter from time criteria.
        /// </summary>
        /// <param name="timeCriteria">
        /// The time criteria.
        /// </param>
        /// <returns>
        /// The date time/>.
        /// </returns>
        private static DateTime GetDateTimeParameterFromTimeCriteria(string timeCriteria)
        {
            DateTime now = DateTime.Today;
            string[] hoursAndMinuteStrings = timeCriteria.Split(':');
            var timeSpan = new TimeSpan(Convert.ToInt32(hoursAndMinuteStrings[0]), Convert.ToInt32(hoursAndMinuteStrings[1]), 0);
            return now.Date + timeSpan;
        }

        /// <summary>
        /// Gets the date of the monday in the week.
        /// </summary>
        /// <returns>
        /// The DateTime object for the Monday in this week.
        /// </returns>
        private static DateTime GetStartOfWeek()
        {
            DateTime date = DateTime.Today;

            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        /// <summary>
        /// Gets the financial year information based on the customers settings.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Date array of financial year information</returns>
        private static DateTime[] GetFinancialYear(int accountId)
        {

            string yearStart = "06/04";
            string yearEnd = "05/04";

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.Clear();

                using (IDataReader reader = connection.GetReader("dbo.SelectFinancialYear", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int financialYearStartOrdinal = reader.GetOrdinal("yearstart");
                        int financialYearEndOrdinal = reader.GetOrdinal("yearend");

                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(financialYearStartOrdinal))
                            {
                                yearStart = reader.GetString(financialYearStartOrdinal);
                            }

                            if (!reader.IsDBNull(financialYearEndOrdinal))
                            {
                                yearEnd = reader.GetString(financialYearEndOrdinal);
                            }
                        }
                    }

                    reader.Close();
                }
            }

            string[] startItems = yearStart.Split('/');
            string[] endItems = yearEnd.Split('/');

            var financialYearStart = new DateTime(DateTime.Today.Year, int.Parse(startItems[1]), int.Parse(startItems[0]));
            var financialYearEnd = new DateTime(DateTime.Today.AddYears(1).Year, int.Parse(endItems[1]), int.Parse(endItems[0]), 23, 59, 59);

            if (int.Parse(endItems[1]) > int.Parse(startItems[1]))
            {
                financialYearEnd = financialYearEnd.AddYears(-1);
            }

            return new[] { financialYearStart, financialYearEnd };
        }
    }
}
