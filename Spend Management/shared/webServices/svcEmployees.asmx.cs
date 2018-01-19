using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Runtime.Caching;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;
    using SpendManagementLibrary;
    using SpendManagementLibrary.API;

    /// <summary>
    /// Web service for Employees functions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcEmployees : WebService
    {
        /// <summary>
        /// Change Lock Status on an employee.  Toggles current "Locked" status on and off.
        /// </summary>
        /// <param name="employeeid">Employee Id to toggle</param>  Employee ID to Lock/Unlock
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void changeLockStatus(int employeeid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            employee.ChangeLockedStatus(!employee.Locked, user);

            // Get current user again, to get the correct locked status.
            user = cMisc.GetCurrentUser();

            if (user.Employee.Locked == false)
            {
                employee.ResetLogonRetryCount();
            }     
        }

        /// <summary>
        /// Get ESR details.
        /// </summary>
        /// <param name="esrObjectType">
        /// The ESR object type.
        /// 1 = ESR Persons
        /// 2 = ESR Assignments
        /// 3 = ESR Addresses
        /// 4 = ESR Locations
        /// 5 = ESR Vehicles
        /// </param>
        /// <param name="esrId">
        /// The ESR id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetEsrDetails(int esrObjectType, long esrId)
        {
            var currentTable = new Guid();
            cField keyField = null;
            var result = new StringBuilder();
            CurrentUser user = cMisc.GetCurrentUser();
            var fields = new cFields(user.AccountID);
            string mappingType = string.Empty;
            
            switch (esrObjectType)
            {
                case 1: // Esr Persons
                    currentTable = new Guid("59EBDE1E-3E1D-408A-8D86-5C35CAF7FD5F");
                    keyField = fields.GetBy(currentTable, "EsrPersonID");
                    mappingType = "Employee";
                    break;
                case 2: // Esr Assignments
                    currentTable = new Guid("BF9AA39A-82D6-4960-BFEF-C5943BC0542D");
                    keyField = fields.GetBy(currentTable, "AssignmentID");
                    mappingType = "Assignment";
                    break;
                case 3: // Esr Addresses
                    currentTable = new Guid("4A793F19-E4A8-4DC9-8249-DE20A53E5BCB");
                    keyField = fields.GetBy(currentTable, "EsrAddressID");
                    mappingType = "Address";
                    break;
                case 4: // Esr Locations
                    currentTable = new Guid("1F8A5A08-DEA3-4EE2-A612-C79B9FB4670B");
                    keyField = fields.GetBy(currentTable, "EsrLocationID");
                    mappingType = "Location";
                    break;
                case 5:  // Esr Vehicles
                    currentTable = new Guid("4A02E429-5073-4A8A-ADD9-2BB0C426D973");
                    keyField = fields.GetBy(currentTable, "ESRVehicleAllocationId");
                    mappingType = "Vehicle";
                    break;
            }

            if (currentTable == new Guid())
            {
                return result.ToString();
            }
            
            //var currentTableFields = fields.GetFieldsByTableID(currentTable);

            SortedList<int, Guid> mappingFields = GetTemplateMappingSequence(user.AccountID, mappingType);

            var tables = new cTables(user.AccountID);
            var qb = new cQueryBuilder(
                user.AccountID,
                cAccounts.getConnectionString(user.AccountID),
                ConfigurationManager.ConnectionStrings["metabase"].ConnectionString,
                tables.GetTableByID(currentTable),
                tables,
                fields);
            qb.addFilter(keyField, ConditionType.Equals, new object[]{ esrId }, null, ConditionJoiner.And, null);

            foreach (KeyValuePair<int, Guid> kvp in mappingFields)
            {
                qb.addColumn(fields.GetFieldByID(kvp.Value));
            }

            var column1 = true;
            using (var reader = qb.getReader())
            {
                while (reader.Read())
                {
                    foreach (cQueryField queryField in qb.lstColumns)
                    {
                        var fieldOrd = reader.GetOrdinal(queryField.field.FieldName);
                        if (fieldOrd != null && fieldOrd >= 0)
                        {
                            var dateVal = new DateTime();
                            object fieldVal = null;
                            if (queryField.field.FieldType == "D" && !reader.IsDBNull(fieldOrd))
                            {
                                dateVal = reader.GetDateTime(fieldOrd);
                                fieldVal = dateVal.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                fieldVal = reader.GetValue(fieldOrd);    
                            }
                            
                            if (column1)
                            {
                                result.Append("<div class=\"twocolumn\">");
                            }

                            result.Append(
                                string.Format(
                                    "<label id=\"lbl{0}\" runat=\"server\" for=\"txt{0}\">{1}</label><span class=\"inputs\" ><span id=\"txt{0}\" class=\"lookupdisplayvalue\" >{2}</span>&nbsp;</span>",
                                    queryField.field.FieldName,
                                    queryField.field.Description,
                                    fieldVal));
                            if (column1)
                            {
                                column1 = false;
                            }
                            else
                            {
                                result.Append("</div>");
                                column1 = true;
                            }
                        }
                    }
                }

                if (!column1)
                {
                    result.Append("</div>");
                }
            }

            return result.ToString();
        }

        private SortedList<int, Guid> GetTemplateMappingSequence(int accountId, string mappingType)
        {
            var cache = MemoryCache.Default;
            string cacheKey = string.Format("tMappingSeq_{0}", mappingType);
            if (cache.Contains(cacheKey))
            {
                return (SortedList<int, Guid>)cache[cacheKey];
            }
            
            var clsImportTemplates = new cImportTemplates(accountId);
            Dictionary<string, List<XMLMapFields>> lstMappings = clsImportTemplates.GetApplicationXMLMappings(ApplicationType.ESROutboundImport, 2);

            var fields = new cFields(accountId);
            var tables = new cTables(accountId);
            var fieldSequence = new SortedList<int, Guid>();

            if (lstMappings.ContainsKey(mappingType))
            {
                foreach (XMLMapFields mapping in lstMappings[mappingType])
                {
                    if (string.IsNullOrEmpty(mapping.defaultFieldName) && string.IsNullOrEmpty(mapping.referenceField))
                    {
                        continue;
                    }

                    var table = tables.GetTableByName(mapping.defaultTableName);
                    var field = fields.GetBy(table.TableID, !string.IsNullOrEmpty(mapping.defaultFieldName) ? mapping.defaultFieldName : mapping.referenceField);
                    if (field != null)
                    {
                        fieldSequence.Add(mapping.ColRef, field.FieldID);
                    }
                }
            }

            cache.Add(cacheKey, fieldSequence, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short) });

            return fieldSequence;
        }

        /// <summary>
        /// Gets the employee item role.
        /// </summary>
        /// <param name="itemRoleId">
        /// The item role id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeItemRole"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public EmployeeItemRole GetEmployeeItemRole(int itemRoleId, int employeeId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            EmployeeItemRoles itemRoles = new EmployeeItemRoles(user.AccountID, employeeId);
            EmployeeItemRole itemRole = itemRoles[itemRoleId];
            if (itemRole != null)
            {
                var clsItemRoles = new cItemRoles(user.AccountID);
                cItemRole reqItemRole = clsItemRoles.getItemRoleById(itemRoleId);
                itemRole.ItemRoleName = reqItemRole.rolename;
                return itemRole;
            }

            return null;
        }

        /// <summary>
        /// Updates the employee item role.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="itemRoleId">
        /// The item role id.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>
        /// </returns>
        [WebMethod(EnableSession = true)]
        public bool UpdateEmployeeItemRole(int employeeId, int itemRoleId, string startDate, string endDate)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            EmployeeItemRoles itemRoles = new EmployeeItemRoles(user.AccountID, employeeId);
            DateTime? itemRoleStartDate = null;
            DateTime irStartDate;
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out irStartDate))
            {
                itemRoleStartDate = irStartDate;
            }
            DateTime? itemRoleEndDate = null;
            DateTime irEndDate;
            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out irEndDate))
            {
                itemRoleEndDate = irEndDate;
            }

            return itemRoles.Update(new EmployeeItemRole(itemRoleId, itemRoleStartDate, itemRoleEndDate), user);
        }

        /// <summary>
        /// Gets the employee item role.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeItemRole"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<EmployeeItemRole> GetEmployeeItemRoles(int employeeId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            EmployeeItemRoles itemRoles = new EmployeeItemRoles(user.AccountID, employeeId);
            return itemRoles.ItemRoles;
        }
    }
}
