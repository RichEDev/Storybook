namespace Spend_Management
{
    #region Using Directives

using System;
using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    #endregion

	/// <summary>
    ///     Summary description for departments.
    ///     [departmentslist + accountid] = sorted list of departments
    ///     [departmentsdd + accountid = string of departments dropdown;
    ///     [departmentsvlist + accountid] = value list of departments;
    ///     [departmentsddown + accountid] = System.web.ui.webcontrols.listitem[]
	/// </summary>
	public class cDepartments
	{
        #region Fields 

        private SortedList<int, cDepartment> _departmentList;
        
        private string _sql;
		
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Departments class.
        /// </summary>
        /// <param name="accountId"></param>
        public cDepartments(int accountId)
		{
            this.AccountId = accountId;
            this.InitialiseData();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Account ID for the department
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// The number of departments in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this._departmentList.Count;
        }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates a dropdown web control for the department information.
        /// </summary>
        /// <param name="useDescription">Whether to use the department name or description.</param>
        /// <param name="includeNoneOption">Whether to include the standard [None] option.</param>
        /// <returns>A collection of ListItem representing the department data.</returns>
        public List<ListItem> CreateDropDown(bool useDescription, bool includeNoneOption = false)
		{
            SortedList<string, cDepartment> sortedDepartments = useDescription ? this.SortDepartmentListByDescription() : this.SortDepartmentList();

            var drodownItems = sortedDepartments.Values.Where(department => !department.Archived).Select(department => useDescription 
                ? new ListItem(department.Description, department.DepartmentId.ToString(CultureInfo.InvariantCulture)) 
                : new ListItem(department.Department, department.DepartmentId.ToString(CultureInfo.InvariantCulture))).ToList();

            if (includeNoneOption)
            {
                drodownItems.Insert(0, new ListItem("[None]", "0"));
            }

            return drodownItems;
        }
			
        /// <summary>
        /// Returns a html string representing a select / dropdown with the department information.
        /// </summary>
        /// <param name="departmentId">The department id.</param>
        /// <param name="readOnly">Whether top make a readonly dropdown.</param>
        /// <param name="blank">Whether to provide a blank entry in the dropdown.</param>
        /// <returns></returns>
        public string CreateStringDropDown(int departmentId, bool readOnly, bool blank)
        {
            var sortedlst = new SortedList<string, cDepartment>();
            var output = new StringBuilder();
            var misc = new cMisc(this.AccountId);
            var properties = misc.GetGlobalProperties(this.AccountId);
            bool useDescription = properties.usedepartmentdesc;

            output.Append("<select name=\"department\" id=\"department\"");

            if (readOnly)
            {
                output.Append(" disabled");
            }

            output.Append(">");

            if (blank)
            {
                output.Append("<option value=\"0\"");
                if (departmentId == 0)
                {
                    output.Append(" selected");
                    }
                output.Append("></option>");
                    }

            foreach (cDepartment reqdepartment in this._departmentList.Values.Where(department => !department.Archived))
                    {
                sortedlst.Add(useDescription ? reqdepartment.Description : reqdepartment.Department, reqdepartment);
                    }

            foreach (cDepartment reqdepartment in sortedlst.Values)
                    {
                output.Append("<option value=\"" + reqdepartment.DepartmentId + "\"");

                if (reqdepartment.DepartmentId == departmentId)
                    {
                    output.Append(" selected");
                    }

                output.Append(">");
                output.Append(useDescription == false ? reqdepartment.Department : reqdepartment.Description);
                output.Append("</option>");
                    }

            output.Append("</select>");

            return output.ToString();
                    }

        /// <summary>
        /// Gets a department instance via the department id.
        /// </summary>
        /// <param name="departmentid">The department id to look for.</param>
        /// <returns>A department instance or null.</returns>
        public virtual cDepartment GetDepartmentById(int departmentid)
                    {
            cDepartment department;
            this._departmentList.TryGetValue(departmentid, out department);
            return department;
                    }

        /// <summary>
        /// Returns an ArrayList of departmentIds in the system.
        /// </summary>
        /// <returns>An ArrayList of department instances.</returns>
        public ArrayList GetDepartmentIds()
                    {
            var ids = new ArrayList();
            
            foreach (cDepartment val in this._departmentList.Values)
                    {
                ids.Add(val.DepartmentId);
                    }

            return ids;
		}

        /// <summary>
        /// Returns an ArrayList of department instances that have had the modified
        /// </summary>
        /// <param name="date">The date from which the filter will run.</param>
        /// <returns>An ArrayList of department instances.</returns>
        public ArrayList GetCreatedOrModifiedDepartments(DateTime date)
		{
            var lst = new ArrayList();

            foreach (cDepartment val in this._departmentList.Values.Where(val => val.CreatedOn > date || val.ModifiedOn > date))
			{
                lst.Add(val);
			}
			
            return lst;
		}
       
        /// <summary>
        /// Creates the list of departments in the system.
        /// </summary>
        public SortedList<int, cDepartment> InitialiseData()
		{
            this._departmentList = new SortedList<int, cDepartment>();
            
            var userdefined = new cUserdefinedFields(this.AccountId);
			
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                var tables = new cTables(this.AccountId);
                var fields = new cFields(this.AccountId);
                cTable table = tables.GetTableByID(new Guid("a0f31cb0-16bb-4ace-aaea-69a7189d9599"));
                cTable userdefinedTable = tables.GetTableByID(table.UserDefinedTableID);
                SortedList<int, SortedList<int, object>> userdefinedFieldsList = userdefined.GetAllRecords(
                    userdefinedTable,
                    tables,
                    fields);

                this._sql =
                    "SELECT departmentid, department, description, archived, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.departments";
                connection.sqlexecute.CommandText = this._sql;

                using (IDataReader reader = connection.GetReader(this._sql))
        {
                    connection.sqlexecute.Parameters.Clear();
                    while (reader.Read())
            {
                        int departmentId = reader.GetInt32(reader.GetOrdinal("departmentid"));
                        string department = reader.GetString(reader.GetOrdinal("department"));
                        string description = reader.IsDBNull(reader.GetOrdinal("description"))
                                             == false
                            ? reader.GetString(reader.GetOrdinal("description"))
                            : string.Empty;
                        bool archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                        DateTime createdOn = reader.IsDBNull(reader.GetOrdinal("createdon"))
                            ? new DateTime(1900, 01, 01)
                            : reader.GetDateTime(reader.GetOrdinal("createdon"));
                        int createdBy = reader.IsDBNull(reader.GetOrdinal("createdby"))
                            ? 0
                            : reader.GetInt32(reader.GetOrdinal("createdby"));
                        DateTime? modifiedOn;
                        if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                        {
                            modifiedOn = null;
            }
            else
            {
                            modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
            }

                        int? modifiedBy;
                        if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
            {
                            modifiedBy = null;
                }
                else
                {
                            modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }

                        SortedList<int, object> userdefinedFields;
                        userdefinedFieldsList.TryGetValue(departmentId, out userdefinedFields);

                        if (userdefinedFields == null)
            {
                            userdefinedFields = new SortedList<int, object>();
            }

                        var curdepartment = new cDepartment(
                            departmentId,
                            department,
                            description,
                            archived,
                            createdOn,
                            createdBy,
                            modifiedOn,
                            modifiedBy,
                            userdefinedFields);
                        this._departmentList.Add(departmentId, curdepartment);
                    }
                    reader.Close();
                }
            }

            return this._departmentList;
            }

        /// <summary>
        ///  Gets a list of active department data.
        /// </summary>
        /// <returns>A list of active <see cref="cDepartment">cCostCode</see>.</returns>
        public List<cDepartment> GetAllActiveDepartments()
        {
            return this._departmentList.Values.Where(deparment => !deparment.Archived).OrderBy(deparment => deparment.Department).ToList();
        }

        /// <summary>
        /// Changes the status of a department.
        /// </summary>
        /// <param name="departmentId">The department id.</param>
        /// <param name="archive">Whether to archive the department.</param>
        public void ChangeStatus(int departmentId, bool archive)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@departmentid", departmentId);
                connection.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                CurrentUser currentUser = cMisc.GetCurrentUser();
                connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            
                if (currentUser.isDelegate)
            {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

                connection.ExecuteProc("changeDepartmentStatus");
                connection.sqlexecute.Parameters.Clear();
                cDepartment department = this.GetDepartmentById(departmentId);
                department.Archived = !department.Archived;
            }
        }
 
        /// <summary>
        /// Deletes a department definition from the database
        /// </summary>
        /// <param name="departmentId">ID of the department to be removed</param>
        /// <param name="employeeId">Employee ID requesting the deletion</param>
        /// <returns>
        ///     0 if successful, -10 if assigned to CE or UDF, -2 if assigned to Employee, -4 if assigned to Saved Expenses
        ///     Costcodes
        /// </returns>
        public int DeleteDepartment(int departmentId, int employeeId)
        {
            int retCode;
            
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@departmentid", departmentId);
                connection.sqlexecute.Parameters.AddWithValue("@userid", employeeId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
                connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);

                if (currentUser.isDelegate)
            {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

                connection.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                connection.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("deleteDepartment");
                
                retCode = (int)connection.sqlexecute.Parameters["@retCode"].Value;
                connection.sqlexecute.Parameters.Clear();

            if (retCode == 0)
            {
                    this._departmentList.Remove(departmentId);
                } 
            }

            return retCode;
        }

        /// <summary>
        /// Gets the Column List of departments
        /// </summary>
        /// <returns>A cColumnList</returns>
        public cColumnList GetColumnList()
		{
            var misc = new cMisc(this.AccountId);
            var properties = misc.GetGlobalProperties(this.AccountId);
            bool useDescription = properties.usedepartmentdesc;
            var columnList = new cColumnList();

            columnList.addItem(0, string.Empty);

            foreach (cDepartment reqdepartment in this._departmentList.Values)
            {
                columnList.addItem(
                    reqdepartment.DepartmentId,
                    useDescription ? reqdepartment.Description : reqdepartment.Department);
            }
			
            return columnList;
		}

        /// <summary>
        /// Gets a department instance via the specified department description.
        /// </summary>
        /// <param name="description">The description to look for.</param>
        /// <returns>An instance of cDeparrment or null.</returns>
        public cDepartment GetDepartmentByDescription(string description)
		{
            return this._departmentList.Values.FirstOrDefault(department => department.Description == description);
		}

        /// <summary>
        /// Gets a department instance from a department name.
        /// </summary>
        /// <param name="departmentName">The name of the department to search for.</param>
        /// <returns>An instance of the department if it can be found or null.</returns>
        public cDepartment GetDepartmentByName(string departmentName)
                {
            return this._departmentList.Values.FirstOrDefault(department => department.Department == departmentName);
        }

        /// <summary>
        /// Gets the sql required for the departments grid.
        /// </summary>
        /// <returns>A string of sql.</returns>
        public string GetGridSql()
                {
            return "select departmentid, archived, department, description from dbo.departments";
        }

        /// <summary>
        /// Saves a department instance.
        /// </summary>
        /// <param name="department">The department to save.</param>
        /// <returns>An integer code to represent the state of success.</returns>
        public int SaveDepartment(cDepartment department)
		{
            int returnId;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
			{
                connection.sqlexecute.Parameters.AddWithValue("@departmentid", department.DepartmentId);
                connection.sqlexecute.Parameters.AddWithValue("@department", department.Department);
                connection.sqlexecute.Parameters.AddWithValue("@description", department.Description);
                if (department.ModifiedBy != null)
				{
                    connection.sqlexecute.Parameters.AddWithValue("@userid", department.ModifiedBy);
                    connection.sqlexecute.Parameters.AddWithValue("@date", department.ModifiedOn);
				}
				else
				{
                    connection.sqlexecute.Parameters.AddWithValue("@userid", department.CreatedBy);
                    connection.sqlexecute.Parameters.AddWithValue("@date", department.CreatedOn);
		}
                CurrentUser currentUser = cMisc.GetCurrentUser();
		
                if (currentUser != null)
		{
                    connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
			{
                        connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
			}
                    else
				{
                        connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
			}
					}
					else
					{
                    connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
			}

                connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                connection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("saveDepartment");
                returnId = (int)connection.sqlexecute.Parameters["@returnvalue"].Value;
				
                if (returnId < 0)
				{
                    return returnId;
				}

                var tables = new cTables(this.AccountId);
                var fields = new cFields(this.AccountId);
                cTable table = tables.GetTableByID(new Guid("a0f31cb0-16bb-4ace-aaea-69a7189d9599"));
                var userdefined = new cUserdefinedFields(this.AccountId);
                userdefined.SaveValues(
                    tables.GetTableByID(table.UserDefinedTableID),
                    returnId,
                    department.UserdefinedFields,
                    tables,
                    fields,
                    currentUser, elementId: (int)SpendManagementElement.Departments,
                    record: department.Department);
                    var target = new cDepartment(
                    returnId,
                    department.Department,
                    department.Description,
                    department.Archived,
                    department.CreatedOn,
                    department.CreatedBy,
                    department.ModifiedOn,
                    department.ModifiedBy,
                    department.UserdefinedFields);

                if (this._departmentList.ContainsKey(returnId))
				{
                    this._departmentList[returnId] = target;
				}
				else
				{
                    this._departmentList.Add(returnId, target);
				}
			}

            return returnId;
		}

        /// <summary>
        /// Sorts the internal list of departments.
        /// </summary>
        /// <returns>A sorted list of the departments.</returns>
        public SortedList<string, cDepartment> SortDepartmentList()
        {
            var sorted = new SortedList<string, cDepartment>();
            
            foreach (cDepartment dep in this._departmentList.Values.Where(department => !sorted.ContainsKey(department.Department)))
                {
                sorted.Add(dep.Department, dep);
        }
       
            return sorted;
        }
        
        /// <summary>
        /// Sorts the internal list of departments by description.
        /// </summary>
        /// <returns>A sorted list of the departments by description.</returns>
        private SortedList<string, cDepartment> SortDepartmentListByDescription()
            {
            var sorted = new SortedList<string, cDepartment>();
		
            foreach (cDepartment dep in this._departmentList.Values.Where(dep => !sorted.ContainsKey(dep.Description)))
        {
                sorted.Add(dep.Description, dep);
        }

            return sorted;
        }

        #endregion
	}
}
