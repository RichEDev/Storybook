namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using SpendManagementHelpers.TreeControl;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.FinancialYears;
    using SpendManagementLibrary.Flags;

    using expenses.code;
    using shared.code.Helpers;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Summary description for svcFlagRules
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcFlagRules : WebService
    {
        /// <summary>
        /// The get financial year list items.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public List<ListItem> GetFinancialYearListItems()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            return FinancialYears.ActiveYears(currentUser).Select(year => new ListItem(year.Description, year.FinancialYearID.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        /// <summary>
        /// The save flag rule.
        /// </summary>
        /// <param name="flagid">
        /// The flag id.
        /// </param>
        /// <param name="flagtype">
        /// The type of flag. Duplicate limit with a receipt etc.
        /// </param>
        /// <param name="action">
        /// The action to take if the flag is breached.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="invaliddatetype">
        /// Whether it is a set date or number of months
        /// </param>
        /// <param name="date">
        /// The set date if using the Initial Date dateflagtype
        /// </param>
        /// <param name="months">
        /// The number of months if using the number of months date flag type.
        /// </param>
        /// <param name="ambertolerance">
        /// The percentage over the limit a claimant has gone where an amber flag is displayed rather than red.
        /// </param>
        /// <param name="frequency">
        /// The frequency.
        /// </param>
        /// <param name="frequencyType">
        /// The frequency the flag will be checked on. Every/In the last
        /// </param>
        /// <param name="period">
        /// The period the flag will be checked against.
        /// </param>
        /// <param name="periodtype">
        /// The period type. Daily, weekly, monthly etc
        /// </param>
        /// <param name="limit">
        /// The monetary limit of the flag.
        /// </param>
        /// <param name="description">
        /// The flag Description.
        /// </param>
        /// <param name="active">
        /// Whether the flag is currently active.
        /// </param>
        /// <param name="claimantJustificationRequired">
        /// The claimant justification required.
        /// </param>
        /// <param name="displayFlagImmediately">
        /// The display flag immediately.
        /// </param>
        /// <param name="flagTolerancePercentage">
        /// If the limit is breached, it will not be flagged if below the no flag tolerance percentage
        /// </param>
        /// <param name="financialYear">
        /// The financial year.
        /// </param>
        /// <param name="tipLimit">
        /// The tip Limit.
        /// </param>
        /// <param name="flagLevel">
        /// The severity level of the flag.
        /// </param>
        /// <param name="approverJustificationRequired">
        /// Whether an approver needs to provide a justification in order to authorise the claim.
        /// </param>
        /// <param name="increaseLimitByNumOthers">
        /// Whether to increase the limit by the number of others fields as well as the number of employees field.
        /// </param>
        /// <param name="displayLimit">
        /// Whether to display the claimant their limit when adding an expense.
        /// </param>
        /// <param name="reportCriteria">
        /// The custom criteria to validate.
        /// </param>
        /// <param name="notesforauthoriser">
        /// Notes seen by the authoriser to guide them on how to deal with the flag.
        /// </param>
        /// <param name="itemroleinclusiontype">
        /// The item roles this flag applies to.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The expense items this flag applies to.
        /// </param>
        /// <param name="validateSelectedExpenseItems">Validate selected expense items</param>
        /// <param name="dailyMileageLimit">The daily mileage limit (if any) use for <seealso cref="RestrictDailyMileageFlag"/> flags</param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod]
        public int SaveFlagRule(int flagid, FlagType flagtype, FlagAction action, string customFlagText, InvalidDateFlagType invaliddatetype, DateTime? date, byte? months, decimal? ambertolerance, byte? frequency, FlagFrequencyType frequencyType, byte? period, FlagPeriodType periodtype, decimal? limit, string description, bool active, bool claimantJustificationRequired, bool displayFlagImmediately, decimal? flagTolerancePercentage, int? financialYear, decimal? tipLimit, FlagColour flagLevel, bool approverJustificationRequired, bool increaseLimitByNumOthers, bool displayLimit, JavascriptTreeData reportCriteria, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, int? passengerLimit, bool validateSelectedExpenseItems, decimal? dailyMileageLimit)
        {
            var user = cMisc.GetCurrentUser();
            var flagManager  = new FlagManagement(user.AccountID);

            return flagManager.Save(flagid, flagtype, action, customFlagText, invaliddatetype, date, months, ambertolerance, frequency, frequencyType, period, periodtype, limit, description, active, claimantJustificationRequired, displayFlagImmediately, flagTolerancePercentage, financialYear, tipLimit, flagLevel, approverJustificationRequired, increaseLimitByNumOthers, displayLimit, reportCriteria, notesforauthoriser, itemroleinclusiontype, expenseiteminclusiontype, passengerLimit, validateSelectedExpenseItems, dailyMileageLimit);
        }
             
        /// <summary>
        /// The delete flag rule.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod]
        public int DeleteFlagRule(int flagId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (!currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.FlagsAndLimits, true))
            {
                return -1;
            }

            FlagManagement clsflags = new FlagManagement(currentUser.AccountID);
            return clsflags.DeleteFlagRule(flagId, currentUser);
        }

        /// <summary>
        /// The create flags grid.
        /// </summary>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <param name="expenseIds">
        /// The expense id.
        /// </param>
        /// <param name="pageSource">
        /// The page source
        /// </param>
        /// <param name="accountID">
        /// The accountId
        /// </param>
        /// <param name="employeeID">
        /// The employeeId
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see>.
        /// </returns>
        [WebMethod]
        public FlaggedItemsManager CreateFlagsGrid(int claimId, string expenseIds, string pageSource, int? accountID = null, int? employeeID = null)
        {
            int accountId;
            int employeeId;

            CurrentUser user = cMisc.GetCurrentUser();

            if (user == null && accountID.HasValue)
            {
                accountId = accountID.Value;
            }
            else
            {
                accountId = user.AccountID;
            }
            
          
            if (user == null && employeeID.HasValue)
            {
                employeeId = employeeID.Value;
            }
            else
            {
                employeeId = user.EmployeeID;
            }

            var flagManager = new FlagManagement(accountId);
            return flagManager.CreateFlagsGrid(claimId, expenseIds.SplitStringOfNumbersToIntList(), pageSource, employeeId);      
        }

        /// <summary>
        /// The create roles grid.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] CreateRolesGrid(int flagID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "d5cf46fa-b420-49e9-81e9-cd01e1f1d1bb"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "54825039-9125-4705-B2D4-EB340D1D30DE"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "DCC4C3E7-1ED8-40B9-94BC-F5C52897FD86")))
                                               };

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridRoles", clstables.GetTableByID(new Guid("fea2c660-25e6-487f-b65c-0b2a04bc55dc")), columns);
            grid.addFilter(clsfields.GetFieldByID(new Guid("46830b36-b5f2-4dbf-8867-36ae064ba3cf")), ConditionType.Equals, new object[] { flagID }, null, ConditionJoiner.None);
            grid.KeyField = "roleID";
            grid.enabledeleting = true;
            grid.CssClass = "datatbl";
            grid.pagesize = 10;
            grid.EmptyText = "There are no item roles associated with this flag rule";
            grid.getColumnByName("roleID").hidden = true;
            grid.deletelink = "javascript:SEL.FlagsAndLimits.deleteAssociatedItemRole({roleID});";
            return grid.generateGrid();
        }

        /// <summary>
        /// The get roles.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] GetRoles(int flagID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement clsflags = new FlagManagement(user.AccountID);
            Flag flag = clsflags.GetBy(flagID);
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "F3016E05-1832-49D1-9D33-79ED893B4366"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "54825039-9125-4705-B2D4-EB340D1D30DE"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "DCC4C3E7-1ED8-40B9-94BC-F5C52897FD86")))
                                               };

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridModalRoles", clstables.GetTableByID(new Guid("DB7D42FD-E1FA-4A42-84B4-E8B95C751BDA")), columns);
            if (flag != null)
            {
                object[] existingRoles = new object[flag.AssociatedItemRoles.Count];
                foreach (int i in flag.AssociatedItemRoles)
                {
                    existingRoles[flag.AssociatedItemRoles.IndexOf(i)] = i;
                }

                if (existingRoles.GetLength(0) > 0)
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(new Guid("F3016E05-1832-49D1-9D33-79ED893B4366")), ConditionType.DoesNotEqual, existingRoles, null, ConditionJoiner.None);
                }
            }

            clsgrid.getColumnByName("itemroleid").hidden = true;
            clsgrid.EnableSelect = true;
            clsgrid.KeyField = "itemroleid";
            clsgrid.EmptyText = "There are no item roles available to add to this flag rule.";
            clsgrid.pagesize = 10;
            clsgrid.Width = Unit.Percentage(100);
            clsgrid.CssClass = "datatbl";
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// The save roles.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod]
        public int SaveItemRoles(int flagID, List<int> roles)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement clsflags = new FlagManagement(user.AccountID);
            return clsflags.SaveItemRoles(flagID, roles, user);
        }

        /// <summary>
        /// The get expense items.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] GetExpenseItems(int flagID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            FlagManagement clsflags = new FlagManagement(user.AccountID);
            Flag flag = clsflags.GetBy(flagID);
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "D4ED76BD-605C-45CE-B075-4C6018A50B08"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "ABFE0BB2-E6AC-40D0-88CE-C5F7B043924D"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "379B67DD-654E-43CD-B55D-B9B5262EEEEE")))
                                               };

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridModalExpenseItems", clstables.GetTableByID(new Guid("401B44D7-D6D8-497B-8720-7FFCC07D635D")), columns);
            if (flag != null)
            {
                object[] existingExpenseItems = new object[flag.AssociatedExpenseItems.Count];
                foreach (AssociatedExpenseItem item in flag.AssociatedExpenseItems)
                {
                    existingExpenseItems[flag.AssociatedExpenseItems.IndexOf(item)] = item.SubcatID;
                }

                if (existingExpenseItems.GetLength(0) > 0)
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(new Guid("D4ED76BD-605C-45CE-B075-4C6018A50B08")), ConditionType.DoesNotEqual, existingExpenseItems, null, ConditionJoiner.None);
                }
            }

            clsgrid.getColumnByName("subcatid").hidden = true;
            clsgrid.EnableSelect = true;
            clsgrid.KeyField = "subcatid";
            clsgrid.EmptyText = "There are no expense items available to add to this flag rule.";
            clsgrid.pagesize = 10;
            clsgrid.Width = Unit.Percentage(100);
            clsgrid.CssClass = "datatbl";
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// The create expense item grid.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] CreateExpenseItemGrid(int flagID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "9580b453-1ac0-41ae-9f1a-66e5b2342c3c"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "ABFE0BB2-E6AC-40D0-88CE-C5F7B043924D"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "379B67DD-654E-43CD-B55D-B9B5262EEEEE")))
                                               };

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridExpenseItems", clstables.GetTableByID(new Guid("afb7d2b2-04a1-4951-978d-c7236529f1dd")), columns);
            grid.addFilter(clsfields.GetFieldByID(new Guid("CC73C0A1-79B8-4773-B320-FD37F2A78F95")), ConditionType.Equals, new object[] { flagID }, null, ConditionJoiner.None);
            grid.KeyField = "subCatID";
            grid.enabledeleting = true;
            grid.CssClass = "datatbl";
            grid.pagesize = 10;
            grid.EmptyText = "There are no expense items associated with this flag rule";
            grid.getColumnByName("subCatID").hidden = true;
            grid.deletelink = "javascript:SEL.FlagsAndLimits.deleteAssociatedExpenseItem({subCatID});";
            return grid.generateGrid();
        }

        /// <summary>
        /// The save expense items.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <param name="expenseitems">
        /// The expenseitems.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod]
        public int SaveFlagRuleExpenseItems(int flagID, List<int> expenseitems)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement clsflags = new FlagManagement(user.AccountID);
            return clsflags.SaveFlagRuleExpenseItems(flagID, expenseitems, user);
            
        }

        /// <summary>
        /// The get fields.
        /// </summary>
        /// <param name="contextKey">
        /// The context key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] GetFields(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int flagid;
            int.TryParse(contextKey, out flagid);

            FlagManagement clsflags = new FlagManagement(user.AccountID);
            DuplicateFlag flag = (DuplicateFlag)clsflags.GetBy(flagid);
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "7b41baef-a51d-4c54-81e5-0f1cb594195d"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "327d7ee7-c11a-4b16-8c05-babf7efe1f71")))
                                               };

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridModalFields", clstables.GetTableByID(new Guid("5B32610E-35DB-492A-B6D1-5F392CA4C040")), columns);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("2d0c6b06-39fc-416f-b639-ba853e059af1")), ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.None);

            if (flag != null)
            {
                object[] existingFields = new object[flag.AssociatedFields.Count];
                foreach (Guid i in flag.AssociatedFields)
                {
                    existingFields[flag.AssociatedFields.IndexOf(i)] = i;
                }

                if (existingFields.GetLength(0) > 0)
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(new Guid("7b41baef-a51d-4c54-81e5-0f1cb594195d")), ConditionType.DoesNotEqual, existingFields, null, ConditionJoiner.And);
                }
            }

            clsgrid.getColumnByName("fieldid").hidden = true;
            clsgrid.EnableSelect = true;
            clsgrid.KeyField = "fieldid";
            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are no fields available to add to this flag rule.";
            clsgrid.pagesize = 10;
            clsgrid.Width = Unit.Percentage(100);
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// The create fields grid.
        /// </summary>
        /// <param name="contextKey">
        /// The context key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod]
        public string[] CreateFieldsGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int flagid = 0;
            if (contextKey != string.Empty)
            {
                int.TryParse(contextKey, out flagid);
            }

            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "54de7d21-a8cf-4174-a9df-cb63e28ae6e2"))),
                                                   new cFieldColumn(
                                                       clsfields.GetFieldByID(
                                                           new Guid(
                                                       "327d7ee7-c11a-4b16-8c05-babf7efe1f71")))
                                               };

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridFlagFields", clstables.GetTableByID(new Guid("75cf2927-f03b-4939-8e3b-0a9ecf22cf59")), columns);
            grid.addFilter(clsfields.GetFieldByID(new Guid("3B3DF32D-56BD-4087-9A77-79E9A2DEF5FF")), ConditionType.Equals, new object[] { flagid }, null, ConditionJoiner.None);
            grid.KeyField = "fieldID";
            grid.enabledeleting = true;
            grid.CssClass = "datatbl";
            grid.EmptyText = "There are no fields associated with this flag rule";
            grid.getColumnByName("fieldID").hidden = true;
            grid.pagesize = 10;
            grid.deletelink = "javascript:SEL.FlagsAndLimits.DeleteFlagField('{fieldID}');";
            return grid.generateGrid();
        }

        /// <summary>
        /// The save fields.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod]
        public bool SaveFields(int flagId, List<Guid> fields)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement clsflags = new FlagManagement(user.AccountID);
            clsflags.SaveFields(flagId, fields, user);
            return true;
        }

        /// <summary>
        /// Deletes an item role association from a flag
        /// </summary>
        /// <param name="flagID">The id of the flag to remove the association from</param>
        /// <param name="itemRoleID">The id of the item role to delete</param>
        [WebMethod]
        public void DeleteAssociatedItemRole(int flagID, int itemRoleID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement flags = new FlagManagement(user.AccountID);
            flags.DeleteAssociatedItemRole(flagID, itemRoleID, user);
        }

        /// <summary>
        /// Deletes an item role association from a flag
        /// </summary>
        /// <param name="flagID">The id of the flag to remove the association from</param>
        /// <param name="subcatID">The id of the expense item to delete</param>
        [WebMethod]
        public void DeleteAssociatedExpenseItem(int flagID, int subcatID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement flags = new FlagManagement(user.AccountID);
            flags.DeleteAssociatedExpenseItem(flagID, subcatID, user);
        }

        /// <summary>
        /// Get the current reports criteria nodes for reports editor.
        /// </summary>
        /// <param name="reportGuidString">
        /// The report GUID as a string.
        /// </param>
        /// <returns>
        /// The <see cref="JavascriptTreeData"/>.
        /// JAVASCRIPT tree data for use with jQuery tree control.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedFilterData(int flagID)
        {
            var filterData = new JavascriptTreeData();
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            var user = cMisc.GetCurrentUser();
            var clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
            var fields = new cFields(user.AccountID);
            
            FlagManagement flags = new FlagManagement(user.AccountID);
            Flag flag = flags.GetBy(flagID);

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            var duplicateFilters = new List<string>();

            if (flag != null && flag.GetType() == typeof(CustomFlag))
            {
                CustomFlag custom = (CustomFlag)flag;
                foreach (cReportCriterion filter in custom.Criteria.Values)
                {
                    if (filter.field != null)
                    {
                        var node = TreeViewNodes.CreateCustomEntityFilterJavascriptNode(filter, duplicateFilters, user, fields);
                        lstNodes.Add(node);
                    }
                }
            }

            filterData.data = lstNodes;

            return filterData;
        }

        [WebMethod]
        public int DeleteFlagField(int flagID, Guid fieldID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FlagsAndLimits, true))
            {
                return -1;
            }

            FlagManagement flags = new FlagManagement(user.AccountID);
            flags.DeleteFlagField(flagID, fieldID, user);

            return 0;
        }

        /// <summary>
        /// Gets the flag inclusion type of the given flagtype
        /// </summary>
        /// <param name="flagtype">
        /// The flag type to get the inclusion type for.
        /// </param>
        /// <returns>
        /// The <see cref="FlagInclusionType"/>.
        /// </returns>
        [WebMethod]
        public FlagInclusionType GetFlagInclusionType(FlagType flagtype)
        {
            return FlagInclusionTypeAttribute.Get(flagtype);
        }

    }
}
