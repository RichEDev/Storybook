namespace SpendManagementLibrary.Holidays
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
	/// A class concerned with retrieving and maintain an employee's <see cref="Holiday"/>  
	/// </summary>
	public class Holidays
    {
        /// <summary>
        /// An instance of <see cref="IDBConnection"/>
        /// </summary>
        private readonly IDBConnection _connection;

        /// <summary>
        /// The get employee's holidays sql.
        /// </summary>
        private const string GetEmployeesHolidaysSql = "select holidayid, startdate, enddate from holidays where employeeid = @employeeid order by startdate";

        /// <summary>
        /// Initializes a new instance of the <see cref="Holidays"/> class.
        /// </summary>
        /// <param name="connection">
        /// An instance of <see cref="IDBConnection"/> connection.
        /// The connection.
        /// </param>
        public Holidays(IDBConnection connection)
        {
            this._connection = connection;
        }

        /// <summary>
        /// Checks whether the employee has access to holidays.
        /// </summary>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>     
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DoesEmployeeHaveAccessToHolidays(int employeeid)
        {
            int retVal = 0;
            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                _connection.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Int);
                _connection.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;
                _connection.ExecuteProc("employeeInSignoffs");

                retVal = (int)_connection.sqlexecute.Parameters["@returnVal"].Value;

                _connection.sqlexecute.Parameters.Clear();
            }
       
            return retVal > 0;
		}

        /// <summary>
        /// Gets a data set of holidays for the supplied employeeId.
        /// </summary>
        /// <param name="employeeId">
        /// The employeeid.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GetHolidayDataSet(int employeeId)
		{
		    var dataSet = new DataSet();

            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
        
                dataSet = _connection.GetDataSet(GetEmployeesHolidaysSql);

                _connection.sqlexecute.Parameters.Clear();
            }

            return dataSet;
        }

        /// <summary>
        /// Gets all an employee's holidays.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// A list of <see cref="Holiday"/>.
        /// </returns>
        public IList<Holiday> GetAllEmployeeHolidays(int employeeId)
        {
            var holidays = new List<Holiday>();

            using (this._connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);

                using (var reader = this._connection.GetReader(GetEmployeesHolidaysSql, CommandType.Text))
                {
                    int holidayIdOrd = reader.GetOrdinal("holidayid");
                    int startDateOrd = reader.GetOrdinal("startdate");
                    int endDateOrd = reader.GetOrdinal("enddate");
              
                    while (reader.Read())
                    {
                        var holidayId = reader.GetInt32(holidayIdOrd);
                        var startDate = reader.GetDateTime(startDateOrd);
                        var endDate = reader.GetDateTime(endDateOrd);

                        holidays.Add(new Holiday(holidayId, employeeId, startDate, endDate));
                    }
                }             
            }

            return holidays;
        }

        /// <summary>
        /// The get holiday by Id.
        /// </summary>
        /// <param name="holidayId">
        /// The holiday id.
        /// </param>
        /// <returns>
        /// The <see cref="Holiday"/>.
        /// </returns>
        public Holiday GetHolidayById(int holidayId)
        {
            Holiday holiday = null;

            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@holidayid", holidayId);
                const string sql = "select employeeid, startdate, enddate from holidays where holidayid = @holidayid";

                using (IDataReader reader = _connection.GetReader(sql))
                {
                    var employeeIdOrdinal = reader.GetOrdinal("employeeid");
                    var startDateOrdinal = reader.GetOrdinal("startdate");
                    var endDateOrdinal = reader.GetOrdinal("enddate");

                    while (reader.Read())
                    {                  
                        var employeeId = reader.GetInt32(employeeIdOrdinal);
                        var startDate = reader.GetDateTime(startDateOrdinal);                      
                        var endDate = reader.GetDateTime(endDateOrdinal);

                        holiday = new Holiday(holidayId, employeeId, startDate, endDate);
                    }

                    reader.Close();
                }

                _connection.sqlexecute.Parameters.Clear();
            }

            return holiday;
        }

        /// <summary>
        /// Adds a holiday.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> Holiday Id.
        /// </returns>
        public int AddHoliday (int employeeId, DateTime startDate, DateTime endDate)
		{
            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                _connection.sqlexecute.Parameters.AddWithValue("@startdate", startDate.Year + "/" + startDate.Month + "/" + startDate.Day);
                _connection.sqlexecute.Parameters.AddWithValue("@enddate", endDate.Year + "/" + endDate.Month + "/" + endDate.Day);
                _connection.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
                _connection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
                const string sql = "insert into holidays (employeeid, startdate, enddate) " +
                    "values (@employeeid,@startdate,@enddate); set @identity = @@identity;";
                _connection.ExecuteSQL(sql);
                var holidayId = (int)_connection.sqlexecute.Parameters["@identity"].Value;
                _connection.sqlexecute.Parameters.Clear();

                return holidayId;
            }
        }

        /// <summary>
        /// Updates a holiday.
        /// </summary>
        /// <param name="holidayId">
        /// The holiday id.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        public void UpdateHoliday(int holidayId, DateTime startDate, DateTime endDate)
		{
            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@startdate", startDate.Year + "/" + startDate.Month + "/" + startDate.Day);
                _connection.sqlexecute.Parameters.AddWithValue("@enddate", endDate.Year + "/" + endDate.Month + "/" + endDate.Day);
                _connection.sqlexecute.Parameters.AddWithValue("@holidayid", holidayId);
                const string sql = "update holidays set startdate = @startdate, enddate = @enddate where holidayid = @holidayid";
                _connection.ExecuteSQL(sql);
                _connection.sqlexecute.Parameters.Clear();
            }
		}

        /// <summary>
        /// Deletes a holiday.
        /// </summary>
        /// <param name="holidayId">
        /// The holiday id.
        /// </param>
        public void DeleteHoliday(int holidayId)
        {
            using (_connection)
            {
                _connection.sqlexecute.Parameters.AddWithValue("@holidayid", holidayId);
                const string sql = "delete from holidays where holidayid = @holidayid";
                _connection.ExecuteSQL(sql);
                _connection.sqlexecute.Parameters.Clear();
            }
        }
	}
}
