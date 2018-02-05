namespace Spend_Management
{
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;

    using CacheDataAccess.ProjectCodes;

    using SQLDataAccess.ProjectCodes;

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    public class cFilterRules
    {
        private readonly Utilities.DistributedCaching.Cache Cache = new Utilities.DistributedCaching.Cache();
        private const string CacheKey = "filterRules";
        private Dictionary<int, cFilterRule> listFilterRules;
        private readonly int AccountId;
        private readonly cCostcodes CostCodes;

        public cFilterRules(int accountId, cCostcodes costCodes = null)
        {
            AccountId = accountId;
            CostCodes = costCodes ?? new cCostcodes(accountid) ;
            InitialiseData();
        }

        #region properties
        /// <summary>
        /// The AccountId
        /// </summary>
        public int accountid
        {
            get { return AccountId; }
        }
        #endregion

        private void InitialiseData()
        {
            this.listFilterRules = Cache.Get(this.AccountId, CacheKey, string.Empty) as Dictionary<int, cFilterRule>;
             
            if (listFilterRules == null)
            {
                listFilterRules = CacheList();
            }
        }

        private void ResetCache()
        {
            Cache.Delete(AccountId, CacheKey, string.Empty);
            listFilterRules = null;
            InitialiseData();
        }

        private Dictionary<int, cFilterRule> CacheList()
        {
            var list = new Dictionary<int, cFilterRule>();
            SortedList<int, Dictionary<int, cFilterRuleValue>> lstFilterValues = GetFilterRuleValues();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                using (var reader = connection.GetReader("GetFilterRules", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int filterId = reader.GetInt32(reader.GetOrdinal("filterid"));
                        var parent = (FilterType)reader.GetByte(reader.GetOrdinal("parent"));
                        var child = (FilterType)reader.GetByte(reader.GetOrdinal("child"));
                        int parentUserDefinedId = reader.IsDBNull(reader.GetOrdinal("paruserdefineid")) ? 0 : reader.GetInt32(reader.GetOrdinal("paruserdefineid"));
                        int childUserDefinedId = reader.IsDBNull(reader.GetOrdinal("childuserdefineid")) ? 0 : reader.GetInt32(reader.GetOrdinal("childuserdefineid"));
                        bool enabled = reader.GetBoolean(reader.GetOrdinal("enabled"));
                        DateTime createdOn = reader.IsDBNull(reader.GetOrdinal("createdon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                        int createdBy = reader.IsDBNull(reader.GetOrdinal("createdby")) ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                        Dictionary<int, cFilterRuleValue> filterRuleValues;
                        lstFilterValues.TryGetValue(filterId, out filterRuleValues);

                        if (filterRuleValues == null)
                        {
                            filterRuleValues = new Dictionary<int, cFilterRuleValue>();
                        }

                        var filterRule = new cFilterRule(filterId, parent, child, filterRuleValues, parentUserDefinedId, childUserDefinedId, enabled, createdOn, createdBy);

                        list.Add(filterId, filterRule);
                    }

                    connection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            Cache.Add(AccountId, CacheKey, string.Empty, list);

            return list;
        }

        /// <summary>
        /// Gets a Filter Rule by its ID
        /// </summary>
        /// <param name="filterId"></param>
        /// <returns>The<see cref="cFilterRule"> FilterRule</see>></returns>
        public cFilterRule GetFilterRuleById(int filterId)
        {
            cFilterRule filterRule = null;
            listFilterRules.TryGetValue(filterId, out filterRule);
            return filterRule;         
        }

        /// <summary>
        /// Gets a collection of filter rules for the type
        /// </summary>
        /// <param name="filterType">The filter type</param>
        /// <returns>A dictionary of <see cref="cFilterRules"> FilterRules</see>></returns>
        public Dictionary<int, cFilterRule> GetFilterRulesByType(FilterType filterType)
        {
            return this.listFilterRules.Values
                .Where(rule => rule.parent == filterType || filterType == FilterType.All)
                .ToDictionary(rule => rule.filterid);
        }

        /// <summary>
        /// Creates filter rules grid
        /// </summary>
        /// <param name="accountid">The account id of logged in user</param>
        /// <param name="employeeid">The employee id of logged in user</param>
        /// <param name="filterType">The type of filter that must be applied to the grid</param>
        /// <returns>The html and data of the grid</returns>
        public string[] createFilterRuleGrid(int accountid, int employeeid, FilterType filterType)
        {
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("F01356A1-E1A1-419D-AD0C-E0EDC0E654D6")))); // filterid
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("3A0D5614-C591-4F8E-B50C-376610053708")))); // parent
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("681973BC-AF7D-4BF1-8DFA-F918B43B0B17")))); // child
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("21E21F08-AD33-4DA2-B407-9FB02A284823")))); // enabled
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("83D6468C-4390-4DD5-94AF-37D832AD5AC5")))); // childuserdefineid
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("1EAE5079-174C-4EB1-9113-929E77FC15B1")))); // paruserdefineid
            cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridFilterRules", clstables.GetTableByID(new Guid("A5DD2B27-287E-4342-85C3-A6F8EBDF83DD")), columns); // filter_rules
            clsgrid.getColumnByName("filterid").hidden = true;
            clsgrid.getColumnByName("childuserdefineid").hidden = true;
            clsgrid.getColumnByName("paruserdefineid").hidden = true;
            clsgrid.KeyField = "filterid";
            clsgrid.enabledeleting = true;
            clsgrid.deletelink = "javascript:deleteFilterRule(" + accountid + ",{holidayid});";
            clsgrid.enableupdating = true;
            clsgrid.editlink = "aefilterrule.aspx?action=2&filterid={filterid}";
            var filterTypeField = clsfields.GetFieldByID(Guid.Parse("3A0D5614-C591-4F8E-B50C-376610053708")); // parent
            if (filterType != FilterType.All)
            {
                clsgrid.addFilter(filterTypeField, ConditionType.Equals, new object[] { (byte)filterType }, null, ConditionJoiner.None);
            }
            clsgrid.InitialiseRow += this.FilterRulesGrid_InitialiseRow;
            clsgrid.ServiceClassForInitialiseRowEvent = "cFilterRules";
            clsgrid.ServiceClassMethodForInitialiseRowEvent = "FilterRulesGrid_InitialiseRow";
            clsgrid.EmptyText = "There are no filter rules to display.";
            return clsgrid.generateGrid();
        }


        /// <summary>
        /// The initialise row event of the grid
        /// </summary>
        /// <param name="row">The row in the grid</param>
        /// <param name="gridinfo">The grid information</param>
        public void FilterRulesGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridinfo)
        {
            cUserdefinedFields accountUserdefinedFields = new cUserdefinedFields(accountid); ;
            cUserDefinedField field;
            byte parentId = Convert.ToByte(row.getCellByID("parent").Value);
            if (parentId != (byte)FilterType.Userdefined)
            {
                row.getCellByID("parent").Value = ((FilterType)parentId).ToString();
            }
            else
            {
                field = accountUserdefinedFields.GetUserDefinedById((int)row.getCellByID("paruserdefineid").Value);
                row.getCellByID("parent").Value = field.label;
            }

            byte childId = Convert.ToByte(row.getCellByID("child").Value);
            if (childId != (byte)FilterType.Userdefined)
            {
                row.getCellByID("child").Value = ((FilterType)childId).ToString();
            }
            else
            {
                field = accountUserdefinedFields.GetUserDefinedById((int)row.getCellByID("childuserdefineid").Value);
                row.getCellByID("child").Value = field.label;
            }
        }


        public List<ListItem> getUserdefinedListItems()
        {
            List<ListItem> lstfilterudfs = new List<ListItem>();
            cUserdefinedFields clsudf = new cUserdefinedFields(accountid);
            cTables clstables = new cTables(accountid);
            cTable tbl = clstables.GetTableByID(new Guid("65394331-792e-40b8-af8b-643505550783"));
            
            List<cUserDefinedField> lstudf = clsudf.GetFieldsByTableAndType(tbl, FieldType.List);

            foreach (cUserDefinedField udf in lstudf)
            {
                lstfilterudfs.Add(new ListItem(udf.label, udf.userdefineid.ToString()));
            }
            return lstfilterudfs;
        }

        /// <summary>
        /// Adds a filter rule to the database.
        /// </summary>
        /// <param name="parent">The parent filter type</param>
        /// <param name="child">The child filter type</param>
        /// <param name="parentUserDefinedId">The Id of the parent user defined field</param>
        /// <param name="childUserDefinedId">The Id of the child user defined field</param>
        /// <param name="enabled">Is it enabled</param>
        /// <param name="employeeId">The employee |Id</param>
        /// <returns>The <see cref="sFilterRuleExistence">sFilterRuleExistence</see>/></returns>
        public sFilterRuleExistence AddFilterRule(FilterType parent, FilterType child, int parentUserDefinedId, int childUserDefinedId, bool enabled, int employeeId)
        {
            var filterValues = new Dictionary<int, cFilterRuleValue>();
            //Create an instance to trigger the constuctor validation
            var filterRule = new cFilterRule(0, parent, child, filterValues, parentUserDefinedId, childUserDefinedId,enabled, DateTime.Today.Date, employeeId);

            var ruleEx = new sFilterRuleExistence {message = FilterRuleExists(parent, child)};

            if (ruleEx.message != string.Empty)
            {
                ruleEx.returncode = 1;
                return ruleEx;
            }

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {              
                connection.sqlexecute.Parameters.AddWithValue("@parent", Convert.ToByte(parent));
                connection.sqlexecute.Parameters.AddWithValue("@child", Convert.ToByte(child));

                if (parentUserDefinedId != 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@parentUserDefineId", parentUserDefinedId);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@parentUserDefineId", DBNull.Value);
                }

                if (childUserDefinedId != 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@childUserDefineId", childUserDefinedId);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@childUserDefineId", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@enabled", Convert.ToByte(enabled));
                connection.sqlexecute.Parameters.AddWithValue("@createdby", employeeId);

                connection.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
                connection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;

                connection.ExecuteProc("SaveFilterRule");
                ruleEx.returncode = (int)connection.sqlexecute.Parameters["@identity"].Value;
                connection.sqlexecute.Parameters.Clear();
            }

            ResetCache();
    
            string newvalue = string.Format("Parent {0} with Child {1}", parent, child);
          
            var audit = new cAuditLog();
            audit.addRecord(SpendManagementElement.FilterRules, newvalue, ruleEx.returncode);

            return ruleEx;
        }

        private string FilterRuleExists(FilterType parent, FilterType child)
        {
            string message = string.Empty;

            foreach (cFilterRule rule in listFilterRules.Values)
            {
                if (rule.parent == parent && rule.child == child)
                {
                    message = "A filter rule like this already exists";
                    return message;
                }

                if (rule.parent == child && rule.child == parent)
                {
                    message = "An existing rule has this child as its parent and this parent as its child, please select a different child.";
                    return message;
                }
            }
            return message;
        }

        /// <summary>
        /// Deletes a filter rule and its associated values
        /// </summary>
        /// <param name="filterId">The Id of the filter rule</param>
        public void DeleteFilterRule(int filterId)
        {
            cFilterRule rule = GetFilterRuleById(filterId);

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@filterId", (filterId));
                connection.ExecuteProc("DeleteFilterRule");           
            }

            DeleteFilterRuleValues(filterId);
            ResetCache();
           
            string newValue = string.Format("Parent {0} with Child {1}", rule.parent, rule.child);     
            var audit = new cAuditLog();
            audit.deleteRecord(SpendManagementElement.FilterRules, filterId, newValue);
        }

        /// <summary>
        /// Deletes filter rules for the specified filter
        /// </summary>
        /// <param name="filterId">The Id of the filter</param>
        public void DeleteFilterRuleValues(int filterId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@filterId", (filterId));
                connection.ExecuteProc("DeleteFilterRuleValues");
                connection.sqlexecute.Parameters.Clear();            
            }
        }

        private SortedList<int, Dictionary<int, cFilterRuleValue>> GetFilterRuleValues()
        {
            var lst = new SortedList<int, Dictionary<int, cFilterRuleValue>>();
            Dictionary<int, cFilterRuleValue> lstRuleVals;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                using (var reader = connection.GetReader("GetFilterRuleValues", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int filterId = reader.GetInt32(reader.GetOrdinal("filterid"));
                        int filter = reader.GetInt32(reader.GetOrdinal("filterruleid"));
                        int parentId = reader.GetInt32(reader.GetOrdinal("parentid"));
                        int childId = reader.GetInt32(reader.GetOrdinal("childid"));
                        DateTime createdOn = reader.IsDBNull(reader.GetOrdinal("createdon")) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                        int createdBy = reader.IsDBNull(reader.GetOrdinal("createdby")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                        lst.TryGetValue(filterId, out lstRuleVals);

                        if (lstRuleVals == null)
                        {
                            lstRuleVals = new Dictionary<int, cFilterRuleValue>();
                            lst.Add(filterId, lstRuleVals);
                        }

                        var ruleValue = new cFilterRuleValue(filter, parentId, childId, filterId, createdOn, createdBy);

                        lstRuleVals.Add(filter, ruleValue);
                    }

                    reader.Close();
                    connection.sqlexecute.Parameters.Clear();
                }

                return lst;
            }
        }

        /// <summary>
        /// Saves the filter rule values to the database
        /// </summary>
        /// <param name="filterRules">The list of filter rule values for the filter</param>
        /// <param name="employeeId">The employeeId</param>
        public void AddFilterRuleValues(List<cFilterRuleValue> filterRules, int employeeId)
        {
            foreach (var filterRule in filterRules)
            {
                using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
                {
                    connection.sqlexecute.Parameters.AddWithValue("@parentid", filterRule.parentid);
                    connection.sqlexecute.Parameters.AddWithValue("@childid", filterRule.childid);
                    connection.sqlexecute.Parameters.AddWithValue("@filterid", filterRule.filterid);
                    connection.sqlexecute.Parameters.AddWithValue("@createdby", employeeId);
                    connection.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
                    connection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;

                    connection.ExecuteProc("SaveFilterRuleValue");

                    int filterRuleId = (int)connection.sqlexecute.Parameters["@identity"].Value;
                    connection.sqlexecute.Parameters.Clear();

                    cFilterRule rule = GetFilterRuleById(filterRule.filterid);
                    cAuditLog clsaudit = new cAuditLog();
                    clsaudit.addRecord(SpendManagementElement.FilterRules, "Added a value for parent " + rule.parent.ToString() + " with Child " + rule.child.ToString(), filterRuleId);
                }
            }

            ResetCache();
        }

        /// <param name="projectCodes">An instance of IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> to access <see cref="IProjectCodeWithUserDefinedFields"/></param>
        public int getIdOfFilterType(FilterType filtertype, string item, IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> projectCodes)
        {
            int id = 0;
            switch (filtertype)
            {
                case FilterType.Costcode:
                    {
                        cCostCode costcode = this.CostCodes.GetCostcodeByString(item);
                        if (costcode == null)
                        {
                            throw new Exception(String.Format("The Costcode '{0}' could not be found", item));
                        }
                        id = costcode.CostcodeId;
                        break;
                    }
                case FilterType.Department:
                    {
                        cDepartments clsdeps = new cDepartments(accountid);
                        cDepartment dep = clsdeps.GetDepartmentByName(item);
                        if (dep == null)
                        {
                            throw new Exception(String.Format("The Department '{0}' could not be found", item));
                        }
                        id = dep.DepartmentId;
                        break;
                    }
                case FilterType.Projectcode:
                {
                    IProjectCodeWithUserDefinedFields projectCode = projectCodes.GetByCustom(new GetByProjectCodeName(item));
                    if (projectCode == null)
                    {
                        throw new Exception(String.Format("The Projectcode '{0}' could not be found", item));
                    }
                    id = projectCode.Id;
                    break;
                }
                case FilterType.Reason:
                    {
                        cReasons clsreasons = new cReasons(accountid);
                        cReason reason = clsreasons.getReasonByString(item);
                        if (reason == null)
                        {
                            throw new Exception(String.Format("The Reason '{0}' could not be found", item));
                        }
                        id = reason.reasonid;
                        break;
                    }
            }

            return id;
        }

        public sFilterRuleControlAttributes getFilterValueAttributes(FilterType filtertype, IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> projectCodes)
        {
            sFilterRuleControlAttributes filValAtts = new sFilterRuleControlAttributes();
            switch (filtertype)
            {
                case FilterType.Costcode:
                    {
                        filValAtts.labelText = "Costcodes:";
                        filValAtts.serviceMethod = "getCostcodeList";
                        filValAtts.items = this.CostCodes.CreateDropDown(false);
                        filValAtts.itemCount = this.CostCodes.Count;
                        break;
                    }
                case FilterType.Department:
                    {
                        cDepartments clsdeps = new cDepartments(accountid);
                        filValAtts.labelText = "Departments:";
                        filValAtts.serviceMethod = "getDepartmentList";
                        filValAtts.items = clsdeps.CreateDropDown(false);
                        filValAtts.itemCount = clsdeps.Count;
                        break;
                    }
                case FilterType.Projectcode:
                    {
                        filValAtts.labelText = "Projectcodes:";
                        filValAtts.serviceMethod = "getProjectcodeList";

                        filValAtts.items = DropDownCreator.GetProjectCodesListItems(false, projectCodes.Get(pc => pc.Archived == false), true).ToList();
                        filValAtts.itemCount = filValAtts.items.Count;
                        break;
                    }
                case FilterType.Reason:
                    {
                        cReasons clsreasons = new cReasons(accountid);
                        filValAtts.labelText = "Reasons:";
                        filValAtts.serviceMethod = "getReasonList";
                        filValAtts.items = clsreasons.CreateDropDown();
                        filValAtts.itemCount = clsreasons.count;
                        break;
                    }
            }
            return filValAtts;
        }

        public Infragistics.WebUI.UltraWebNavigator.Nodes popFilterRuleValues(cFilterRule rule)
        {
            Infragistics.WebUI.UltraWebNavigator.Nodes nodes = new Infragistics.WebUI.UltraWebNavigator.Nodes();
            Infragistics.WebUI.UltraWebNavigator.Node node;
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);
            bool parExists;
            string item = "";

            foreach (cFilterRuleValue val in rule.rulevals.Values)
            {
                switch (rule.parent)
                {
                    case FilterType.Costcode:
                        item = GetParentOrChildItem(rule.parent, val.parentid, true, clsproperties.usecostcodedesc);
                        break;
                    case FilterType.Department:
                        item = GetParentOrChildItem(rule.parent, val.parentid, true, clsproperties.usedepartmentdesc);
                        break;
                    case FilterType.Projectcode:
                        item = GetParentOrChildItem(rule.parent, val.parentid, true, clsproperties.useprojectcodedesc);
                        break;
                    case FilterType.Reason:
                        item = GetParentOrChildItem(rule.parent, val.parentid, true, false);
                        break;
                    case FilterType.Userdefined:
                        item = GetParentOrChildItem(rule.parent, val.parentid, true, false);
                        break;
                    //case FilterType.Location:
                    //    item = getParentOrChildItem(rule.parent, val.parentid, true, false);
                    //    break;
                }

                parExists = false;
                node = new Infragistics.WebUI.UltraWebNavigator.Node();
                string[] temp = item.Split(',');
                node.Text = temp[0];

                node.Tag = val;
                node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
                node.Images.DefaultImage.Url = "~//icons//delete2.gif";

                foreach (Infragistics.WebUI.UltraWebNavigator.Node nde in nodes)
                {
                    if (node.Text == nde.Text)
                    {
                        parExists = true;
                        break;
                    }
                }

                if (parExists == false)
                {
                    nodes.Add(node);
                }
            }

            foreach (Infragistics.WebUI.UltraWebNavigator.Node nde in nodes)
            {
                foreach (cFilterRuleValue fval in rule.rulevals.Values)
                {
                    cFilterRuleValue temp = (cFilterRuleValue)nde.Tag;
                    if (temp.parentid == fval.parentid)
                    {
                        switch (rule.child)
                        {
                            case FilterType.Costcode:
                                item = GetParentOrChildItem(rule.child, fval.childid, false, clsproperties.usecostcodedesc);
                                break;
                            case FilterType.Department:
                                item = GetParentOrChildItem(rule.child, fval.childid, false, clsproperties.usedepartmentdesc);
                                break;
                            case FilterType.Projectcode:
                                item = GetParentOrChildItem(rule.child, fval.childid, false, clsproperties.useprojectcodedesc);
                                break;
                            case FilterType.Reason:
                                item = GetParentOrChildItem(rule.child, fval.childid, false, false);
                                break;
                            case FilterType.Userdefined:
                                item = GetParentOrChildItem(rule.child, fval.childid, false, false);
                                break;
                        }
                        node = new Infragistics.WebUI.UltraWebNavigator.Node();
                        node.Text = item;
                        node.Tag = fval;
                        node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
                        node.Images.DefaultImage.Url = "~//icons//delete2.gif";
                        nde.Nodes.Add(node);
                    }
                }

            }

            return nodes;
        }

        /// <summary>
        /// Gets the item detail for the inputted criteria
        /// </summary>
        /// <param name="filterType">The filter type</param>
        /// <param name="id">The Id of the we element are insterested in</param>
        /// <param name="parent">Is it a parent</param>
        /// <param name="showDesc">Show the description</param>
        /// <returns></returns>
        public string GetParentOrChildItem(FilterType filterType, int id, bool parent, bool showDesc)
        {
            string item = string.Empty;

            switch (filterType)
            {
                case FilterType.Costcode:
                    {
                        cCostCode costcode = CostCodes.GetCostcodeById(id);

                        if (parent)
                        {
                            item = showDesc
                                ? string.Format("{0},{1}", costcode.Description, costcode.CostcodeId)
                                : string.Format("{0},{1}", costcode.Costcode, costcode.CostcodeId);
                        }
                        else
                        {
                            item = showDesc ? costcode.Description : costcode.Costcode;
                        }
                        break;
                    }
                case FilterType.Department:
                    {
                        var departments = new cDepartments(accountid);
                        var department = departments.GetDepartmentById(id);

                        if (parent)
                        {
                            item = showDesc
                                ? string.Format("{0},{1}", department.Description, department.DepartmentId)
                                : string.Format("{0},{1}", department.Department, department.DepartmentId);
                        }
                        else
                        {
                            item = showDesc ? department.Description : department.Department;
                        }
                        break;
                    }
                case FilterType.Projectcode:
                    {
                        var projects = new cProjectCodes(accountid);
                        var project = projects.getProjectCodeById(id);
                        
                        if (parent)
                        {
                            item = showDesc
                                ? string.Format("{0},{1}", project.description, project.projectcodeid)
                                : string.Format("{0},{1}", project.projectcode, project.projectcodeid);
                        }
                        else
                        {
                            item = showDesc ? project.description : project.projectcode;
                        }
                        break;
                    }
                case FilterType.Reason:
                    {
                        var reasons = new cReasons(accountid);
                        var reason = reasons.getReasonById(id);

                        item = parent ? string.Format("{0},{1}", reason.reason, reason.reasonid) : reason.reason;
                        break;
                    }
                case FilterType.Userdefined:
                    {
                        var userdefinedFields = new cUserdefinedFields(accountid);

                        foreach (cFilterRule rule in listFilterRules.Values)
                        {
                            cUserDefinedField field = null;
                            if (parent)
                            {
                                if (rule.parent == filterType)
                                {
                                    field = userdefinedFields.GetUserDefinedById(rule.paruserdefineid);

                                    foreach (cListAttributeElement element in field.items.Select(kvp => (cListAttributeElement)kvp.Value).Where(element => element.elementValue == id))
                                    {
                                        item = element.elementText;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (rule.child == filterType)
                                {
                                    field = userdefinedFields.GetUserDefinedById(rule.childuserdefineid);

                                    foreach (cListAttributeElement element in field.items.Select(kvp => (cListAttributeElement)kvp.Value).Where(element => element.elementValue == id))
                                    {
                                        item = element.elementText;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
            return item;
        }

        public string getChildTargetControl(FilterType filtertype)
        {
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);
            string controlname = "";

            switch (filtertype)
            {
                case FilterType.Costcode:
                    {
                        if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet == false)
                        {
                            controlname = "cmbcostcode;breakdown";
                        }
                        else if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet)
                        {
                            controlname = "cmbgencostcode;general";
                        }
                        break;
                    }
                case FilterType.Department:
                    {
                        if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet == false)
                        {
                            controlname = "cmbdepartment;breakdown";
                        }
                        else if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet)
                        {
                            controlname = "cmbgendepartment;general";
                        }
                        break;
                    }
                case FilterType.Projectcode:
                    {
                        if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet == false)
                        {
                            controlname = "cmbprojectcode;breakdown";
                        }
                        else if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet)
                        {
                            controlname = "cmbgenprojectcode;general";
                        }
                        break;
                    }
                case FilterType.Reason:
                    {
                        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
                        if (reason.individual)
                        {
                            controlname = "cmbreason;individual";
                        }
                        else
                        {
                            controlname = "cmbreason;general";
                        }
                        break;
                    }
                case FilterType.Userdefined:
                    {


                        break;
                    }
            }
            return controlname;
        }

        /// <summary>
        /// Gets a list items, dependant on the filter type
        /// </summary>
        /// <param name="filtertype"></param>
        /// <param name="useDescription"></param>
        /// <returns>A list of items</returns>
        public List<ListItem> GetItems(FilterType filtertype, bool useDescription)
        {
            var lstItems = new List<ListItem>();

            switch (filtertype)
            {
                case FilterType.Costcode:
                    {
                        lstItems = this.CostCodes.CreateDropDown(useDescription);

                        break;
                    }
                case FilterType.Department:
                    {
                        var department = new cDepartments(accountid);
                        lstItems = department.CreateDropDown(useDescription);

                        break;
                    }
                case FilterType.Projectcode:
                    {
                        var projectCodes = new cProjectCodes(accountid);
                        lstItems = projectCodes.CreateDropDown(useDescription);
                        break;
                    }
                case FilterType.Reason:
                    {
                        var reasons = new cReasons(accountid);
                        lstItems = reasons.CreateDropDown();
                        break;
                    }
            }

            return lstItems;
        }

        /// <summary>
        /// Determines what filters should be applied to the drop down list
        /// </summary>
        /// <param name="filtertype">The filter type</param>
        /// <param name="ctlIndex">The index of the control</param>
        /// <param name="ddlId">The Id of the drop down list</param>
        /// <returns></returns>
        public string[] FilterDropdown(FilterType filtertype, string ctlIndex, string ddlId)
        {
            Dictionary<int, cFilterRule> filterrules = GetFilterRulesByType(filtertype);

            int type = 0;
            string sType = "";
            string sFilterid = "";

            foreach (cFilterRule rule in filterrules.Values)
            {
                if (rule.rulevals.Count > 0 && rule.enabled)
                {
                    type = (int)rule.child;
                    sType += type.ToString() + ";";
                    sFilterid += rule.filterid.ToString() + ";";
                }
            }

            string values = string.Format("popChildDropDowns('{0}','{1}','{2}','{3}',{4})", ddlId, sType, sFilterid, ctlIndex, accountid);
            string[] attribute = new string[] { "onchange", values };
          
            return attribute;
        }

        public void filterTextbox(ref TextBox txtbox, FilterType filtertype, string ctlindex)
        {
            Dictionary<int, cFilterRule> filterrules = GetFilterRulesByType(filtertype);

            int type = 0;
            string sType = "";
            string sFilterid = "";

            foreach (cFilterRule rule in filterrules.Values)
            {
                if (rule.rulevals.Count > 0 && rule.enabled)
                {
                    type = (int)rule.child;
                    sType += type.ToString() + ";";
                    sFilterid += rule.filterid.ToString() + ";";
                }
            }
            txtbox.Attributes.Add("onchange", "popChildDropDowns('" + txtbox.ID + "','" + sType + "','" + sFilterid + "','" + ctlindex + "'," + accountid + ")");
        }

        public void ruleCheck()
        {
            FilterArea parentArea;
            FilterArea childArea;

            foreach (cFilterRule rule in listFilterRules.Values)
            {
                parentArea = getArea(rule.parent, true, rule.paruserdefineid, 0);
                childArea = getArea(rule.child, false, 0, rule.childuserdefineid);

                switch (parentArea)
                {
                    case FilterArea.General:
                        {
                            if (childArea == FilterArea.General || childArea == FilterArea.Breakdown || childArea == FilterArea.Individual)
                            {
                                UpdateRuleStatus(rule.filterid, true);
                            }
                            break;
                        }
                    case FilterArea.Breakdown:
                        {
                            if (childArea == FilterArea.Breakdown)
                            {
                                UpdateRuleStatus(rule.filterid, true);
                            }
                            else if (childArea == FilterArea.General || childArea == FilterArea.Individual)
                            {
                                UpdateRuleStatus(rule.filterid, false);
                            }
                            break;
                        }
                    case FilterArea.Individual:
                        {
                            if (childArea == FilterArea.Individual)
                            {
                                UpdateRuleStatus(rule.filterid, true);
                            }
                            else if (childArea == FilterArea.General || childArea == FilterArea.Breakdown)
                            {
                                UpdateRuleStatus(rule.filterid, false);
                            }
                            break;
                        }
                }
            }
        }

        public FilterArea getArea(FilterType filtertype, bool isParent, int paruserdefineid, int childuserdefineid)
        {        
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);
            cUserdefinedFields clsudf = new cUserdefinedFields(accountid);
            FilterArea area = FilterArea.General;

            switch (filtertype)
            {
                case FilterType.Costcode:
                    {
                        if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet == false)
                        {
                            area = FilterArea.Breakdown;
                        }
                        else if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet)
                        {
                            area = FilterArea.General;
                        }
                        break;
                    }
                case FilterType.Department:
                    {
                        if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet == false)
                        {
                            area = FilterArea.Breakdown;
                        }
                        else if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet)
                        {
                            area = FilterArea.General;
                        }
                        break;
                    }
                case FilterType.Projectcode:
                    {
                        if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet == false)
                        {
                            area = FilterArea.Breakdown;
                        }
                        else if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet)
                        {
                            area = FilterArea.General;
                        }
                        break;
                    }
                case FilterType.Reason:
                    {
                        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
                        if (reason.individual)
                        {
                            area = FilterArea.Individual;
                        }
                        else
                        {
                            area = FilterArea.General;
                        }
                        break;
                    }
                case FilterType.Userdefined:
                    {
                        cUserDefinedField field;

                        if (isParent)
                        {
                            field = clsudf.GetUserDefinedById(paruserdefineid);
                        }
                        else
                        {
                            field = clsudf.GetUserDefinedById(childuserdefineid);
                        }

                        if (field.itemspecific == false)
                        {
                            area = FilterArea.General;
                        }
                        else
                        {
                            area = FilterArea.Individual;
                        }
                        break;
                    }
            }
            return area;
        }


        /// <summary>
        /// Updates the status of the filter rule. 
        /// </summary>
        /// <param name="filterId">The filter Id</param>
        /// <param name="enabled">If rule is enabled</param>
        public void UpdateRuleStatus(int filterId, bool enabled)
        {

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@enabled", enabled);
                connection.sqlexecute.Parameters.AddWithValue("@filterid", filterId);           
                connection.ExecuteProc("UpdateFilterRule");
                connection.sqlexecute.Parameters.Clear();
            }
        }

        public sFilterRuleInfo getModifiedFilterRules(DateTime date)
        {
            sFilterRuleInfo filterinfo = new sFilterRuleInfo();

            Dictionary<int, cFilterRule> lstfilterrules = new Dictionary<int, cFilterRule>();
            List<int> lstfilterruleids = new List<int>();
            Dictionary<int, cFilterRuleValue> lstfilterrulevalues = new Dictionary<int, cFilterRuleValue>();
            List<int> lstfilterrulevalueids = new List<int>();

            foreach (cFilterRule rule in listFilterRules.Values)
            {
                lstfilterruleids.Add(rule.filterid);

                if (rule.createdon > date)
                {
                    lstfilterrules.Add(rule.filterid, rule);
                }

                foreach (cFilterRuleValue val in rule.rulevals.Values)
                {
                    lstfilterrulevalueids.Add(val.filterruleid);

                    if (val.createdon > date)
                    {
                        lstfilterrulevalues.Add(val.filterruleid, val);
                    }
                }
            }

            filterinfo.lstfilterrules = lstfilterrules;
            filterinfo.lstfilterruleids = lstfilterruleids;
            filterinfo.lstfilterrulevalues = lstfilterrulevalues;
            filterinfo.lstfilterrulevalueids = lstfilterrulevalueids;

            return filterinfo;
        }
    }
}
