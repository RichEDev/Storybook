namespace Spend_Management.shared.webServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;

    using Infragistics.WebUI.UltraWebCalcManager;

    using SpendManagementHelpers.TreeControl;

    using Spend_Management.shared.code; 
    using Spend_Management.shared.code.EasyTree;
    using Spend_Management.shared.code.Helpers;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;

    using Spend_Management.shared.reports;
    using SpendManagementLibrary.Report;
    using System.Configuration;
    using System.Data;
    using System.Globalization;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Logic_Classes.Tables;

    /// <summary>
    ///     WEBSERVICE for AEREPORT.ASPX
    ///     Returns tree data for reports
    ///     Saves reports data.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcReports : WebService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get the branch nodes for the selected node on the reports tree view.
        /// </summary>
        /// <param name="fieldID">
        /// The GUID of the field selected on the tree.
        /// </param>
        /// <param name="crumbs">
        /// The "crumbs" attribute of the node selected.
        /// </param>
        /// <param name="nodeID">
        /// The selected node id.
        /// </param>
        /// <returns>
        /// The <see cref="JavascriptTreeData"/>.
        /// The JAVASCRIPT tree data for use with jQuery tree control.
        /// </returns>
        /// <exception cref="FormatException">
        /// Exception if the base table cannot be converted to a GUID.
        /// </exception>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetBranchNodes(string fieldID, string crumbs, string nodeID)
        {
            var user = cMisc.GetCurrentUser();
            Guid baseGuid;
            if (string.IsNullOrWhiteSpace(fieldID) || !Guid.TryParseExact(fieldID, "D", out baseGuid))
            {
                return new JavascriptTreeData();
            }

            if (string.IsNullOrWhiteSpace(nodeID) == false)
            {
                foreach (string s in nodeID.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Guid tmpGuid;
                    if (!Guid.TryParseExact(s.Substring(1), "D", out tmpGuid))
                    {
                        if (!Guid.TryParseExact(s.Substring(0), "D", out tmpGuid))
                        {
                            throw new FormatException("baseTableID can not be converted to a Guid");
                        }
                    }
                }
            }

            var clsCustomEntities = new cCustomEntities(user);
            JavascriptTreeData result = clsCustomEntities.GetNodeData(baseGuid, crumbs, nodeID, false);
            var fields = new cFields(user.AccountID);
            result.data.AddRange(this.AddRelatedTables(fields, new cTables(user.AccountID), baseGuid, nodeID ));
            result.data.AddRange(this.AddManyToOneTables(baseGuid, user, clsCustomEntities, fields, nodeID, crumbs, user));
            foreach (JavascriptTreeData.JavascriptTreeNode node in result.data)
            {
                node.metadata = TreeViewNodes.SetMetadataDefaults();
            }

            return result;
        }

        private IEnumerable<JavascriptTreeData.JavascriptTreeNode> AddManyToOneTables(Guid baseGuid, ICurrentUserBase user, cCustomEntities customEntities, cFields fields, string prefixID, string prefixCrumbs, ICurrentUser currentUser)
        {
            var result = new List<JavascriptTreeData.JavascriptTreeNode>();
            var field = fields.GetFieldByID(baseGuid);

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var relabler = new FieldRelabler(accountProperties);

            if (field != null)
            {
                var customEntity = customEntities.getEntityByTableId(field.RelatedTableID != Guid.Empty ? field.RelatedTableID : field.TableID);
                if (customEntity != null)
                {
                    string preCrumb = (string.IsNullOrWhiteSpace(prefixCrumbs)) ? string.Empty : new StringBuilder(prefixCrumbs).Append(": ").ToString();
                    foreach (KeyValuePair<int, cAttribute> attribute in customEntity.attributes)
                    {
                        if (attribute.Value.fieldtype == FieldType.Relationship && attribute.Value.GetType() == typeof(cOneToManyRelationship))
                        {
                            Guid fieldid = attribute.Value.fieldid;
                            string description = relabler.Relabel(fields.GetFieldByID(attribute.Value.fieldid)).Description;
                            const string GuidPrefix = "k";
                            var node = new JavascriptTreeData.JavascriptTreeNode
                                           {
                                               attr =
                                                   new JavascriptTreeData.
                                                   JavascriptTreeNode.LiAttributes
                                                       {
                                                           rel = "nodelink",
                                                           id = prefixID + "_" + GuidPrefix + fieldid.ToString(),
                                                           crumbs = preCrumb + field.Description,
                                                           fieldid = fieldid.ToString(),
                                                           fieldtype = attribute.Value.fieldtype.ToString(),
                                                           comment = string.Empty
                                                       },
                                               state = "closed",
                                               data = description,
                                               metadata = TreeViewNodes.SetMetadataDefaults()
                                           };

                            result.Add(node);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get lowest level tree nodes for reports editor.
        /// </summary>
        /// <param name="baseTableString">
        /// The base table GUID as a string.
        /// </param>
        /// <returns>
        /// The <see cref="JavascriptTreeData"/>.
        ///     JAVASCRIPT tree data for use with jQuery tree control.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetInitialTreeNodes(string baseTableString)
        {
            var baseTable = new Guid(baseTableString);
            var javascriptData = new JavascriptTreeData();
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            var user = cMisc.GetCurrentUser();
            var customEntities = new cCustomEntities(user);
            cCustomEntity customEntity = customEntities.getEntityByTableId(baseTable);

            if (customEntity != null)
            {
                foreach (cAttribute a in customEntity.attributes.Values)
                {
                    if (a.GetType() == typeof(cSummaryAttribute) || a.fieldtype == FieldType.LookupDisplayField
                        || a.fieldtype == FieldType.Comment)
                    {
                        continue;
                    }
                    var node = TreeViewNodes.CreateJavascriptNode(a);
                    node.metadata = TreeViewNodes.SetMetadataDefaults();
                    lstNodes.Add(node);
                }
            }
            else
            {
                // system entity
                var fields = new cFields(user.AccountID);
                var tables = new cTables(user.AccountID);
                var entityFields = fields.GetFieldsByTableIDForViews(baseTable);
                foreach (cField entityField in entityFields)
                {
                    var node = TreeViewNodes.CreateJavascriptNode(entityField, true);
                    if (node != null)
                    {
                        node.metadata = TreeViewNodes.SetMetadataDefaults();
                        lstNodes.Add(node);
                    }
                }

                lstNodes.AddRange(this.AddRelatedTables(fields, tables, baseTable, baseTableString));
            }

                lstNodes = lstNodes.OrderBy(x => x.data).ToList();

            javascriptData.data = lstNodes;

            return javascriptData;
        }



        /// <summary>
        /// Add related tables (via related table) to the list.  This will skip custom entities as they are delt with by cCustomEntities.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tables"></param>
        /// <param name="baseTable"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private IEnumerable<JavascriptTreeData.JavascriptTreeNode> AddRelatedTables(cFields fields, cTables tables, Guid baseTable, string nodeId)
        {
            var table = tables.GetTableByID(baseTable);
            var relatedTable = Guid.Empty;
            var result = new List<cField>();
            if (table == null)
            {
                var thisField = fields.GetFieldByID(baseTable);
                table = thisField.GetParentTable();
                if (table.TableSource == cTable.TableSourceType.CustomEntites)
                {
                    return new List<JavascriptTreeData.JavascriptTreeNode>();
                }

                relatedTable = thisField.RelatedTableID;
                baseTable = table.TableID;
            }
            else
            {

                if (table.TableSource == cTable.TableSourceType.CustomEntites)
                {
                    return new List<JavascriptTreeData.JavascriptTreeNode>();
                }
            }

            result.AddRange(fields.GetAllRelatedFields(table.TableID));
            if (relatedTable != Guid.Empty)
            {
                result.AddRange(fields.getFieldsByViewGroup(relatedTable).Values);
            }

            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            var nodes = nodeId.Split(new string[] {"_"}, StringSplitOptions.RemoveEmptyEntries);

            if (nodes.GetUpperBound(0) > 0 && baseTable.ToString() == nodes[0].ToString())
            {
                nodeId = "n" + nodeId;
            }

            foreach (cField field in result)
            {
                var node = TreeViewNodes.CreateRelatedTableJavascriptNode(nodeId, field);
                if (!string.IsNullOrEmpty(node.state))
                {
                    lstNodes.Add(node);
                }
            }

            return lstNodes;
        }

        /// <summary>
        /// Get the current reports criteria nodes for reports editor.
        /// </summary>
        /// <param name="reportGuidString">
        /// The report GUID as a string.
        /// </param>
        /// <param name="requestNumber">
        /// The request Number if editing during runtime.
        /// </param>
        /// <returns>
        /// The <see cref="JavascriptTreeData"/>.
        /// JAVASCRIPT tree data for use with jQuery tree control.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedFilterData(string reportGuidString, int requestNumber)
        {
            var filterData = new JavascriptTreeData();
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            var user = cMisc.GetCurrentUser();
            var clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
            var fields = new cFields(user.AccountID);
            cReport report = this.GetReport(reportGuidString, requestNumber, clsreports);

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            var duplicateFilters = new List<string>();

            if (report != null)
            {
                foreach (cReportCriterion filter in report.criteria)
                {
                    if (filter.field != null && filter.field.FieldName.ToLower() != "subaccountid")
                    {
                        var node = TreeViewNodes.CreateCustomEntityFilterJavascriptNode(filter, duplicateFilters, user, fields);
                        lstNodes.Add(node);
                    }
                }
            }

            filterData.data = lstNodes;

            return filterData;
        }

        /// <summary>
        /// Return the current columns from the saved report.
        /// </summary>
        /// <param name="reportGuidString">
        /// the report GUID as a string.
        /// </param>
        /// <param name="requestNumber"></param>
        /// <returns>
        /// The <see cref="JavascriptTreeData"/>.
        /// JAVASCRIPT tree data for use with jQuery tree control.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedNodes(string reportGuidString, int requestNumber)
        {
            var selectedNodes = new JavascriptTreeData();
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            CurrentUser user = cMisc.GetCurrentUser();
            var clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
            cReport report = this.GetReport(reportGuidString, requestNumber, clsreports);

            if (report != null)
            {
                this.GetNodesFromReport(report, lstNodes, selectedNodes, report.basetable.TableID);
            }

            return selectedNodes;
        }

        /// <summary>
        /// Get report from request or guid.
        /// </summary>
        /// <param name="reportGuidString">
        /// The report guid string.
        /// </param>
        /// <param name="requestNumber">
        /// The request number.
        /// </param>
        /// <param name="clsreports">
        /// The report class.
        /// </param>
        /// <returns>
        /// The <see cref="cReport"/>.
        /// The selected report either by request number or guid.
        /// </returns>
        private cReport GetReport(string reportGuidString, int requestNumber, cReports clsreports)
        {
            cReport report = null;
            if (requestNumber != 0)
            {
                var request = (cReportRequest)this.Session["request" + requestNumber];
                report = request.report;
            }
            else
            {
                Guid reportGuid;
                if (Guid.TryParse(reportGuidString, out reportGuid))
                {
                    report = clsreports.getReportById(reportGuid);
                }
            }
            return report;
        }

        private void GetNodesFromReport(cReport report, List<JavascriptTreeData.JavascriptTreeNode> lstNodes, JavascriptTreeData selectedNodes, Guid baseTable)
        {
            if (report != null)
            {
                foreach (cReportColumn column in report.columns)
                {
                    if (column is cStandardColumn && !column.system)
                    {
                    var standard = (cStandardColumn)column;
                    if (standard.field != null)
                    {
                            var node = TreeViewNodes.CreateReportCriteriaJavascriptNode(standard, baseTable);

                        lstNodes.Add(node);
                    }
                }

                    if (column is cCalculatedColumn)
                    {
                        var calculated = (cCalculatedColumn)column;
                        var node = this.FormatCalculatedField(
                            calculated.reportcolumnid.ToString(),
                            "W",
                            calculated.columnname,
                            calculated.formattedFormula);
                        lstNodes.Add(node);
            }

                    if (column is cStaticColumn)
                    {
                        var staticColumn = (cStaticColumn)column;
                        var node = this.FormatCalculatedField(
                            staticColumn.reportcolumnid.ToString(),
                            "Z",
                            staticColumn.literalname,
                            staticColumn.literalvalue,
                            staticColumn.runtime);
                        lstNodes.Add(node);
                    }
                }
            }

            selectedNodes.data = lstNodes;
        }

        /// <summary>
        /// Save report from AEREPORT
        /// </summary>
        /// <param name="reportid">
        /// The report GUID as a string.
        /// </param>
        /// <param name="requestNumber">The current request  number if editing during runtime.</param>
        /// <param name="reportName">
        /// The report name.
        /// </param>
        /// <param name="reportDescription">
        /// The report description.
        /// </param>
        /// <param name="category">
        /// The category (report folder).
        /// </param>
        /// <param name="reportOn">
        /// BASETABLE GUID as a string..
        /// </param>
        /// <param name="claimant">
        /// Report is for the current user only.
        /// </param>
        /// <param name="fields">
        /// The report columns
        /// </param>
        /// <param name="reportCriteria">
        /// The report criteria.
        /// </param>
        /// <param name="criteriaLoaded">
        /// If false, use the criteria from the existing report (if any) otherwise use the report criteria.
        /// </param>
        /// <param name="columnsLoaded">
        /// If false, use the columns from the existing report (if any) otherwise use the report columns.
        /// </param>
        /// <param name="showChart">Show chart only, data only or both.</param>
        /// <param name="limitReport">Limit the report to x lines (defaults to 10,000 if zero)</param>
        /// <param name="reportChart"></param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public Guid SaveReport(
            string reportid,
            int requestNumber,
            string reportName,
            string reportDescription,
            string category,
            string reportOn,
            bool claimant,
            List<SelectedField> fields,
            List<SelectedCriteria> reportCriteria,
            bool criteriaLoaded,
            bool columnsLoaded, 
            ShowChartFlag showChart, 
            short limitReport,
            ReportChart reportChart)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties =
                clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var tables = new SubAccountTables(new cTables(user.AccountID), new TableRelabler(accountProperties));
            cTable table = tables.GetTableByID(new Guid(reportOn));
            var joinVias = new JoinVias(cMisc.GetCurrentUser());

            var columns = new ArrayList();
            var criteria = new ArrayList();
            Guid reportGuid = Guid.Parse(reportid);
            cReport existingReport = null;
            if (reportGuid == Guid.Empty)
            {
                reportGuid = new Guid();
            }
            else
            {
                existingReport = clsreports.getReportById(reportGuid);
            }

            var disableColumnAndCriteriaUpdate = existingReport == null ? false : clsreports.ReportIsScheduled(reportGuid);
            Guid categoryGuid;
            Guid.TryParse(category, out categoryGuid);

            var clsFields = new SubAccountFields(new cFields(user.AccountID), new FieldRelabler(accountProperties));
            const bool ReadOnlyReport = false;
            var columnOrder = SaveReportColumns(fields, reportGuid, clsFields, joinVias, columns);
            cReports.AddSystemColumns(columns, clsFields, reportGuid, joinVias, table);
            var criteriaOrder = SaveReportCriteria(reportCriteria, clsFields, joinVias, reportGuid, criteria);
            this.AddSubAccountCriteria(criteria, reportOn, reportGuid, clsFields, user, criteriaOrder);

            if (requestNumber > 0)
            {
                var request = (cReportRequest) this.Session["request" + requestNumber];
                request.report.updateColumns(columns);
                request.report.updateCriteria(criteria);
                request.ReportChart = reportChart;
                this.Session["runtimeUpdate"] = true;
                return request.report.reportid;
            }

            var rpt = new cReport(
                user.AccountID,
                user.CurrentSubAccountId,
                reportGuid,
                user.EmployeeID,
                reportName,
                reportDescription,
                categoryGuid,
                table,
                claimant,
                ReadOnlyReport,
                columns,
                criteria,
                limitReport,
                user.CurrentActiveModule,
                true,
                showChart);

            if (rpt.reportid == Guid.Empty)
            {
                reportGuid = clsreports.addReport(rpt);
                if (reportGuid != Guid.Empty)
                {
                    if (reportChart != null)
                    {
                        reportChart.Reportid = reportGuid;
                        reportChart.Save(user);
                    }
                    return reportGuid;
                }
            }

            reportChart.Save(user);
            return clsreports.updateReport(rpt) == 0 ? rpt.reportid : Guid.Empty;
        }

        private void AddSubAccountCriteria(ArrayList reportCriteria, string reportOn, Guid reportGuid, IFields fields, CurrentUser user, int criteriaOrder)
        {
            cField field;
            switch (reportOn.ToUpper())
            {
                case ReportTable.ContractDetails:
                case ReportTable.SupplierDetails:
                case ReportTable.ProductDetails:
                    field = fields.GetBy(new Guid(reportOn), "subaccountid");
                    if (field != null)
                    {
                        var criteria = new cReportCriterion(
                            Guid.NewGuid(),
                            reportGuid,
                            field,
                            ConditionType.Equals,
                            new object[] { user.CurrentSubAccountId },
                            null,
                            ConditionJoiner.And,
                            criteriaOrder,
                            false,
                            0);
                        reportCriteria.Add(criteria);
                    }

                    break;
                case ReportTable.ContractProductDetails:
                    field = AddJoinAndCriteria(reportCriteria, reportGuid, fields, user, criteriaOrder, ReportFields.ContractProductDetailsContractId, "ContractProductDetails : ContractDetails");
                    break;
                case ReportTable.InvoiceDetails:
                    field = AddJoinAndCriteria(reportCriteria, reportGuid, fields, user, criteriaOrder, ReportFields.InvoiceDetailsContractId, "Invoice : ContractDetails");
                    break;
                case ReportTable.InvoiceForecasts:
                    field = AddJoinAndCriteria(reportCriteria, reportGuid, fields, user, criteriaOrder, ReportFields.InvoiceForecastsContractId, "InvoiceForecasts : ContractDetails");
                    break;

                case ReportTable.SupplierContacts:
                    field = fields.GetBy(new Guid(ReportTable.SupplierDetails), "subaccountid");
                    if (field != null)
                    {
                        var joinVias = new JoinVias(user);
                        var joinVia = cReports.CreatejoinVia(ReportFields.SupplierContactsSupplierId, JoinViaPart.IDType.Field, "Supplier Contacts : SupplierDetails", joinVias);

                        var criteria = new cReportCriterion(
                                    new Guid(),
                                    reportGuid,
                                    field,
                                    ConditionType.Equals,
                                    new object[] { user.CurrentSubAccountId },
                                    null,
                                    ConditionJoiner.And,
                                    criteriaOrder,
                                    false,
                                    0,
                                    joinVia);
                        reportCriteria.Add(criteria);
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Add required join and criteria
        /// </summary>
        /// <param name="reportCriteria">The list of current criteria</param>
        /// <param name="reportGuid">The ID of the current <see cref="cReport"/></param>
        /// <param name="fields">An instance of <see cref="IFields"/> </param>
        /// <param name="user">The current user <see cref="CurrentUser"/></param>
        /// <param name="criteriaOrder">The current highest oder umber of criteria</param>
        /// <param name="joinId">The IF of the field to join via</param>
        /// <param name="joinDescription">The description of the new Join Via</param>
        /// <returns></returns>
        private static cField AddJoinAndCriteria(ArrayList reportCriteria, Guid reportGuid, IFields fields, CurrentUser user, int criteriaOrder, string joinId, string joinDescription)
        {
            cField field = fields.GetBy(new Guid(ReportTable.ContractDetails), "subaccountid");
            if (field != null)
            {
                var joinVias = new JoinVias(user);
                var joinVia = cReports.CreatejoinVia(joinId, JoinViaPart.IDType.Field, joinDescription, joinVias);
                var criteria = new cReportCriterion(
                            new Guid(),
                            reportGuid,
                            field,
                            ConditionType.Equals,
                            new object[] { user.CurrentSubAccountId },
                            null,
                            ConditionJoiner.And,
                            criteriaOrder,
                            false,
                            0,
                            joinVia);
                reportCriteria.Add(criteria);
            }

            return field;
        }

        /// <summary>
        /// Convert selected criteria into cReportCriteria
        /// </summary>
        /// <param name="reportCriteria">The selected criteria</param>
        /// <param name="clsFields">The fields object</param>
        /// <param name="joinVias">The join vias object</param>
        /// <param name="reportGuid">The current report Guid</param>
        /// <param name="criteria">The criteria Array List that is updated.</param>
        /// <returns></returns>
        private static int SaveReportCriteria(
           List<SelectedCriteria> reportCriteria,
           IFields clsFields,
           JoinVias joinVias,
           Guid reportGuid,
           ArrayList criteria)
            {
            int order = 1, group = 0, currentGroup, previousGroup = 0;
            foreach (SelectedCriteria treeNode in reportCriteria)
                {
                Guid reportFieldGuid = treeNode.FieldId;
                int joinViaID = treeNode.JoinViaId;
                string joinViaDescription = treeNode.Crumbs;
                string joinViaIDs = treeNode.Id;
                    var joinViaList = new SortedList<int, JoinViaPart>();
                
                var reportField = clsFields.GetFieldByID(reportFieldGuid);

                    if (joinViaID < 1)
                    {
                        joinViaList = joinVias.JoinViaPartsFromCompositeGuid(joinViaIDs, reportField);
                        joinViaID = 0; // 0 causes the save on the joinVia list
                    }


                    JoinVia joinVia = null;
                currentGroup = treeNode.Group;
                if (currentGroup != 0 && currentGroup != previousGroup)
                {
                    group++;
                }
                previousGroup = currentGroup;
                var joiner = treeNode.Joiner;

                object[] value2 = treeNode.Value2;
                object[] value1 = treeNode.Value1;
                DateTime tempDate;
                switch (reportField.FieldType)
                    {
                    case "DT":
                        if (DateTime.TryParseExact(treeNode.Value1[0].ToString().TrimEnd(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value1[0] = tempDate;
                        }

                        if (DateTime.TryParseExact(treeNode.Value2[0].ToString().TrimEnd(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value2[0] = tempDate;
                        }

                        break;
                    case "D":
                    case "FD":
                        if (DateTime.TryParseExact(treeNode.Value1[0].ToString().TrimEnd(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value1[0] = tempDate;
                        }

                        if (DateTime.TryParseExact(treeNode.Value2[0].ToString().TrimEnd(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value2[0] = tempDate;
                        }

                        break;
                    case "T":
                    case "FT":
                        if (DateTime.TryParseExact(treeNode.Value1[0].ToString().TrimEnd(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value1[0] = tempDate;
                        }

                        if (DateTime.TryParseExact(treeNode.Value2[0].ToString().TrimEnd(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            value2[0] = tempDate;
                        }

                        break;
                    case "A": //Currency
                    case "C"://Currency
                    case "FC"://Currency
                    case "M"://Money
                    case "N"://Number
                    case "B":// Long
                        decimal tempDecimal;
                            if (reportField.ValueList || reportField.GenList)
                            {
                                break;
                            }

                            if (decimal.TryParse(treeNode.Value1[0].ToString(), out tempDecimal))
                            {
                                value1[0] = tempDecimal;
                            }

                            if (decimal.TryParse(treeNode.Value2[0].ToString(), out tempDecimal))
                            {
                                value2[0] = tempDecimal;
                            }

                        break;
                    case "F"://Float
                        double tempDouble;
                        if (double.TryParse(treeNode.Value1[0].ToString(), out tempDouble))
                        {
                            value1[0] = tempDouble;
                        }

                        if (double.TryParse(treeNode.Value2[0].ToString(), out tempDouble))
                        {
                            value2[0] = tempDouble;
                        }
                        break;
                    case "FI"://Integer
                    case "I"://Integer
                        if (reportField.ValueList || reportField.GenList)
                        {
                            break;
                        }
                        int tempInt;
                        if (int.TryParse(treeNode.Value1[0].ToString(), out tempInt))
                        {
                            value1[0] = tempInt;
                        }

                        if (int.TryParse(treeNode.Value2[0].ToString(), out tempInt))
                        {
                            value2[0] = tempInt;
                        }
                        break;
                    case "FX"://Bit
                    case "X"://Bit
                    case "Y"://Bit
                        bool tempBool;
                        if (bool.TryParse(treeNode.Value1[0].ToString(), out tempBool))
                        {
                            value1[0] = tempBool;
                        }

                        if (bool.TryParse(treeNode.Value2[0].ToString(), out tempBool))
                        {
                            value2[0] = tempBool;
                        }
                        break;
                    }
                var conditionType = treeNode.Condition;
                    switch (conditionType)
                    {
                        case ConditionType.Yesterday:
                        case ConditionType.Today:
                        case ConditionType.Tomorrow:
                        case ConditionType.Next7Days:
                        case ConditionType.Last7Days:
                        case ConditionType.NextWeek:
                        case ConditionType.LastWeek:
                        case ConditionType.ThisWeek:
                        case ConditionType.NextMonth:
                        case ConditionType.LastMonth:
                        case ConditionType.ThisMonth:
                        case ConditionType.NextYear:
                        case ConditionType.LastYear:
                        case ConditionType.ThisYear:
                        case ConditionType.NextTaxYear:
                        case ConditionType.LastTaxYear:
                        case ConditionType.NextFinancialYear:
                        case ConditionType.LastFinancialYear:
                        case ConditionType.ThisFinancialYear:
                        case ConditionType.AnyTime:
                            value1 = null;
                            value2 = null;
                            break;
                    default:
                        break;
                    }

                bool runtime = treeNode.RunTime;
                    Guid criteriaid = Guid.Empty;
                    if (joinViaList.Count > 0)
                    {
                        joinVia = new JoinVia(joinViaID, joinViaDescription, Guid.Empty, joinViaList);
                        joinViaID = joinVias.SaveJoinVia(joinVia);
                    }

                    if (joinViaID > 0)
                    {
                        joinVia = joinVias.GetJoinViaByID(joinViaID);
                    }

                    var newCriteria = new cReportCriterion(
                        criteriaid,
                        reportGuid,
                        reportField,
                        conditionType,
                        value1,
                        value2,
                    (ConditionJoiner)joiner,
                        order,
                        runtime,
                        currentGroup == 0 ? Convert.ToByte(0): (byte)group,
                        joinVia);
                    criteria.Add(newCriteria);
                if (reportField != null)
                {
                    order++;
                }
            }

            return order;
        }


        private static int SaveReportColumns(
            List<SelectedField> fields,
            Guid reportGuid,
            IFields clsFields,
            JoinVias joinVias,
            ArrayList columns)
        {
            int order = 1;
            foreach (SelectedField treeNode in fields)
            {
                int sortOrder = treeNode.sort;
                var columnSort = ColumnSort.None;
                switch (sortOrder)
                {
                    case 0:
                        columnSort = ColumnSort.None;
                        break;
                    case 1:
                        columnSort = ColumnSort.Ascending;
                        break;
                    case 2:
                        columnSort = ColumnSort.Descending;
                        break;
                }
                cReportColumn column = null;
                Guid columnId = Guid.Empty;
                Guid.TryParse(treeNode.columnid, out columnId);
                if (treeNode.id.StartsWith("W"))
                {
                    column = new cCalculatedColumn(
                        columnId,
                        reportGuid,
                        ReportColumnType.Calculated,
                        columnSort,
                        order,
                        treeNode.literalName,
                        treeNode.literalValue);
            }
                else if (treeNode.id.StartsWith("Z"))
                {
                    var runTime = false;
                    var literalValue = treeNode.literalValue;
                    if (treeNode.literalValue == "<EnterAtRunTime>")
                    {
                        runTime = true;
                        literalValue = string.Empty;
                    }

                    column = new cStaticColumn(
                        columnId,
                        reportGuid,
                        ReportColumnType.Static,
                        columnSort,
                        order,
                        treeNode.literalName,
                        literalValue,
                        runTime,
                        treeNode.hide);
                }
            else
            {
                    cField reportField = clsFields.GetFieldByID(treeNode.fieldid);

                    int joinViaID = treeNode.joinviaid;
                    string joinViaDescription = reportField.Description;
                    string joinViaIDs = treeNode.id;
                    var joinViaList = new SortedList<int, JoinViaPart>();
                    if (joinViaID < 1)
                    {
                        joinViaList = new JoinVias(cMisc.GetCurrentUser()).JoinViaPartsFromCompositeGuid(joinViaIDs, reportField);
                        joinViaID = 0; // 0 causes the save on the joinVia list
                   }

                    
                    bool groupBy = treeNode.groupby;
                    bool hidden = treeNode.hide;
                    bool funcCount = treeNode.count;
                    bool funcAvg = treeNode.average;
                    bool funcSum = treeNode.sum;
                    bool funcMax = treeNode.max;
                    bool funcMin = treeNode.min;

                    switch (reportField.FieldType)
            {
                        case "A":
                        case "C":
                        case "FC":
                        case "M":
                        case "FD":
                        case "FI":
                        case "I":
                        case "N":
                        case "B":
                            break;
                        default:
                            funcAvg = false;
                            funcSum = false;
                            break;
                    }

                    column = new cStandardColumn(
                        columnId,
                        reportGuid,
                        ReportColumnType.Standard,
                        columnSort,
                        order,
                        reportField,
                        groupBy,
                        funcSum,
                        funcMin,
                        funcMax,
                        funcAvg,
                        funcCount,
                        hidden,
                        displayName: treeNode.attributename);
                    if (joinViaList.Count > 0)
                {
                        var joinVia = new JoinVia(joinViaID, joinViaDescription, Guid.Empty, joinViaList);
                        joinViaID = joinVias.SaveJoinVia(joinVia);
                }

                    if (joinViaID > 0)
                    {
                        column.JoinVia = joinVias.GetJoinViaByID(joinViaID);
            }
                }

                columns.Add(column);
                order++;
        }

            return order;
        }

        [WebMethod(EnableSession = true)]
        public ReportChart GetChart(Guid reportId)
        {
            var user = cMisc.GetCurrentUser();
            return ReportChart.Get(reportId, user);
        }

        [WebMethod(EnableSession = true)]
        public List<Function> GetAvailableFunctions()
        {
            return Functions.Get().ToList();
        }


        [WebMethod(EnableSession = true)]
        public string ValidateCalcuation(string calculation)
        {
            var calcManager = new UltraWebCalcManager();
            var clstext = new cText();
            var clsexcel = new cExcel();
            var clsRow = new cRowFunction();
            var clsColumn = new cColumnFunction();
            var column = new ColFunction();
            var clsAddress = new cAddressFunction();
            var clsCarriage = new cCarriageReturn();
            var clsReplaceText = new cReplaceTextFunction();

            calcManager.RegisterUserDefinedFunction(clstext);
            calcManager.RegisterUserDefinedFunction(clsexcel);
            calcManager.RegisterUserDefinedFunction(clsRow);
            calcManager.RegisterUserDefinedFunction(clsColumn);
            calcManager.RegisterUserDefinedFunction(column);
            calcManager.RegisterUserDefinedFunction(clsAddress);
            calcManager.RegisterUserDefinedFunction(clsCarriage);
            calcManager.RegisterUserDefinedFunction(clsReplaceText);
            
            try
            {
                var value = calcManager.Calculate(calculation.TrimEnd());
            }
            catch (ArgumentNullException e)
            {
                return string.Format("Error with calculation {0} \r\n {1}", calculation, e.Message);
            }

            return string.Empty;
        }

        [WebMethod(EnableSession = true)]
        public JavascriptTreeData.JavascriptTreeNode FormatCalculatedField(string id, string type, string columnName, string formattedCalculation)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
            else
            {
                if (id.StartsWith(type))
                {
                    id = id.Substring(1);
                }
            }

            var result = new JavascriptTreeData.JavascriptTreeNode
            {
                attr =
                    new JavascriptTreeData.
                    JavascriptTreeNode.LiAttributes
                    {
                        rel = "node",
                        id = type + id,
                        internalId = type + id,
                        crumbs = columnName,
                        fieldid = id,
                        fieldtype = type,
                        comment = string.Empty
                    },
                state = "closed",
                data = columnName,
                metadata = TreeViewNodes.SetMetadataDefaults()
            };
            result.metadata.Add("formattedCalculation", formattedCalculation);
            
            return result;
        }

        /// <summary>
        /// Get the tree data down to a specified level for the east tree control.
        /// </summary>
        /// <param name="baseTableString">The guid of the base table</param>
        /// <returns>EasyTree Nodes</returns>
        [WebMethod(EnableSession = true)]
        public List<EasyTreeNode> GetEasyTreeNodes(string baseTableString)
        {
            var user = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var nodes = new Nodes(user, new SubAccountFields(new cFields(user.AccountID), new FieldRelabler(accountProperties)), new SubAccountTables(new cTables(user.AccountID), new TableRelabler(accountProperties)) , 2, new cCustomEntities(user), new AllowedTables(), new TreeGroups());
            return nodes.GetInitialTreeNodes(baseTableString);
        }

        /// <summary>
        /// Get the branch nodes only at the next level
        /// </summary>
        /// <param name="fieldID">The field to expand from</param>
        /// <param name="crumbs">The current bread crumbs</param>
        /// <param name="nodeID">the current node</param>
        /// <returns>Easy tree nodes</returns>
        [WebMethod(EnableSession = true)]
        public List<EasyTreeNode> GetBranchEasyTreeNodes(string fieldID, string crumbs, string nodeID)
        {
            var user = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            var fields = new SubAccountFields(new cFields(user.AccountID), new FieldRelabler(accountProperties));
            var tables = new SubAccountTables(new cTables(user.AccountID), new TableRelabler(accountProperties));
            var nodes = new Nodes(user, fields, tables, 1, new cCustomEntities(user), new AllowedTables(), new TreeGroups());
            Guid baseGuid;
            if (string.IsNullOrWhiteSpace(fieldID) || !Guid.TryParseExact(fieldID, "D", out baseGuid))
            {
                return new List<EasyTreeNode>();
            }
            string prefix = string.Empty;

            prefix = this.ValidateNodeAndSetBaseTable(nodeID, prefix, nodes, fields);
            var result = new List<EasyTreeNode>();

            var table = tables.GetTableByID(baseGuid);
            if (table == null || table.TableID == Guid.Empty)
            {
                baseGuid = prefix == "k" ? fields.GetFieldByID(baseGuid).RelatedTableID : fields.GetFieldByID(baseGuid).TableID;
                
            }
            var exportedTables = new List<Guid>();
            var level = 0;
            nodes.GetNodesForTable(baseGuid, result, nodeID, exportedTables, level);
            result = result.OrderBy(x => x.text).ToList();
            return result;
        }

    /// <summary>
    /// Validate the ID's in the node and set the base table in the node class.
    /// </summary>
    /// <param name="nodeId">The node to validate</param>
    /// <param name="prefix">The current prefic</param>
    /// <param name="nodes">An instance of the <see cref="Nodes"/>class to update</param>
    /// <param name="fields">An instance of the <see cref="IFields"/>class.</param>
    /// <returns></returns>
        private string ValidateNodeAndSetBaseTable(string nodeId, string prefix, Nodes nodes, IFields fields)
        {
            if (string.IsNullOrWhiteSpace(nodeId) == false)
            {
                foreach (string s in nodeId.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Guid tmpGuid;
                    prefix = s.Substring(0, 1);
                    if (!Guid.TryParseExact(s.Substring(1), "D", out tmpGuid))
                    {
                        if (!Guid.TryParseExact(s.Substring(0), "D", out tmpGuid))
                        {
                            throw new FormatException("baseTableID can not be converted to a Guid");
                        }
                    }

                    if (nodes.baseTable != Guid.Empty)
                    {
                        continue;
                    }

                    switch (prefix)
                    {
                        case "k":
                            nodes.baseTable = fields.GetFieldByID(tmpGuid).TableID;
                            break;
                        case "x":
                            nodes.baseTable = fields.GetFieldByID(tmpGuid).RelatedTableID;
                            break;
                        default:
                            nodes.baseTable = tmpGuid;
                            break;
                    }
                }
            }
            return prefix;
        }

        /// <summary>
        /// Get a field comment for a given field.
        /// </summary>
        /// <param name="fieldGuid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetFieldComment(string fieldGuid)
        {
            var user = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var fields = new SubAccountFields(new cFields(user.AccountID), new FieldRelabler(accountProperties));
            
            Guid fieldguid;
            if (Guid.TryParse(fieldGuid, out fieldguid))
            {
                var field = fields.GetFieldByID(fieldguid);
                if (field != null)
                {
                    return field.Comment;
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// Start a report request for preview
        /// </summary>
        /// <param name="reportName">The current reportname</param>
        /// <param name="reportOn">A string version of the Report Table GUID</param>
        /// <param name="claimant">If this is a claimant report</param>
        /// <param name="reportFields">A list of selected fields for this preview.</param>
        /// <param name="reportCriteria">A list of selected criteria for this preview.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int CreateReportPreview(string reportName, string reportOn, bool claimant, List<SelectedField> reportFields, List<SelectedCriteria> reportCriteria, ReportChart reportChart)
        {
            var user = cMisc.GetCurrentUser();
            Guid reportGuid;
            if (Guid.TryParse(reportOn, out reportGuid))
            {
                if (Session["currentrequestnum"] == null)
                {
                    Session["currentrequestnum"] = 0;
                }

                int currentRequestNum = (int)Session["currentrequestnum"];
                var requestNum = currentRequestNum + 1;
                Session["currentrequestnum"] = requestNum;

                ArrayList columns = new ArrayList();
                var fields = new cFields(user.AccountID);
                var joinVias = new JoinVias(user);
                var reportTable = new cTables(user.AccountID).GetTableByID(reportGuid);
                SaveReportColumns(reportFields, reportGuid, fields, joinVias, columns);
                ArrayList criteria = new ArrayList();
                var criteriaOrder = SaveReportCriteria(reportCriteria.Where(c => !c.RunTime).ToList(), fields, joinVias, reportGuid, criteria);
                this.AddSubAccountCriteria(criteria, reportOn, reportGuid, fields, user, criteriaOrder);
                var report = new cReport(Guid.NewGuid(), user.CurrentSubAccountId, reportName, string.Empty, Guid.Empty, reportTable, claimant, false, columns, criteria, string.Empty, 10, user.CurrentActiveModule, useJoinVia: true);
                report.accountid = user.AccountID;
                var request = new cReportRequest(user.AccountID, user.CurrentSubAccountId, requestNum, report, ExportType.Preview, new cExportOptions(), claimant, user.EmployeeID, user.HighestAccessLevel);
                request.ReportChart = reportChart;
                IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                if (clsreports.createReport(request))
                {
                    Session["request" + requestNum] = request;
                    return requestNum;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the report data if finished, if not return the current status
        /// </summary>
        /// <param name="requestnum">The users request number</param>
        /// <returns>Report Response object</returns>
        [WebMethod(EnableSession = true)]
        public ReportResponse GetReportData(int requestnum)
        {
            IReports reports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            cReportRequest request = (cReportRequest)Session["request" + requestnum];
            if (request == null)
            {
                return new ReportAborted();
            }

            object[] data = reports.getReportProgress(request);
            if (data == null)
            {
                return new ReportResponseError();
            }

            switch (data[0].ToString())
            {
                case "BeingProcessed":
                    var inProgress = new ReportResponseInProgress();
                    inProgress.RequestNumber = requestnum;
                    inProgress.RowCount = (int)data[3];
                    inProgress.PercentageProcessed = (int)data[1];
                    return inProgress;
                case "Complete":
                    var result =  this.ReportData(request, reports);
                    if (result == null)
                    {
                        return new ReportResponseError();
                    }
                    else
                    {
                        return result;
                    }
                case "Failed":
                    var failed = new ReportResponseError();
                    return failed;
                case "Queued":
                    var queued = new ReportResponseInProgress();
                    queued.RequestNumber = requestnum;
                    queued.Progress = ReportProgress.Queued;
                    return queued;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Update the drilldown report foe a given report ID.
        /// </summary>
        /// <param name="reportId">
        /// The reportid.
        /// </param>
        /// <param name="drilldown">
        /// The drilldown report ID.
        /// </param>
        [WebMethod(EnableSession = true)]
        public void UpdateDrilldownReport(Guid reportId, Guid drilldown)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            clsreports.updateDrillDownReport(currentUser.AccountID, currentUser.EmployeeID, reportId, drilldown);
        }

        /// <summary>
        /// Save a copy of an already existing report..
        /// </summary>
        /// <param name="reportId">
        /// The ID of the <see cref="cReport"/> to copy.
        /// </param>
        /// <param name="reportName">
        /// The name of the new report.
        /// </param>
        /// <param name="folderId">
        /// The ID of the folder to save the report into..
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/> of the new report
        /// Guid.Empty if the same has failed.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public Guid SaveAs(Guid reportId, string reportName, Guid? folderId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cReports clsReports = new cReports(currentUser.AccountID, currentUser.CurrentSubAccountId);
            return clsReports.SaveReportAs(reportId, currentUser.EmployeeID, reportName, folderId, currentUser);
        }

        /// <summary>
        /// Get Claim details for viewing a claim.
        /// </summary>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <returns>
        /// An instacne of <see cref="ViewClaimDetails"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public ViewClaimDetails ViewClaim(int claimId)
        {
            var user = cMisc.GetCurrentUser();
            var clsclaims = new cClaims(user.AccountID);
            var result = new ViewClaimDetails();
            cClaim reqclaim = clsclaims.getClaimById(claimId);
            Employee claimemp = Employee.Get(reqclaim.employeeid, user.AccountID);

            string empname = claimemp.Title + " " + claimemp.Forename + " " + claimemp.Surname;

            result.EmployeeName = empname;
            result.ClaimNumber = reqclaim.claimno.ToString();
            if (reqclaim.paid == true)
            {
                result.DatePaid = reqclaim.datepaid.ToShortDateString();
            }
            result.Description = reqclaim.description;

            return result;
        }

        /// <summary>
        /// Get the Image path for Charts.
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>The path for the image store.</returns>
        public static string GetImagePath(CurrentUser user)
        {
            var sharedDirectory = ConfigurationManager.AppSettings["HostedEntityImageLocation"].Split('\\');
            var sharedUrl = sharedDirectory.LastOrDefault();
            return $"/{sharedUrl}/";
        }

        /// <summary>
        /// Return the report data
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="reports">The reports service.</param>
        /// <returns>The report Response containing the report data and columns.</returns>
        private ReportResponseComplete ReportData(cReportRequest request, IReports reports)
        {
            var customEntityImageData = new CustomEntityImageData(request.accountid);
            var data = reports.getReportData(request);
            if (data is DataSet)
            {
                var reportData = (DataSet)data;
                var rows = new List<Dictionary<string, object>>();
                var columns = new Dictionary<string, string>();
                GetColumnNames(request, columns);

                foreach (DataRow dataRow in reportData.Tables[0].Rows)
                {
                    var row = new Dictionary<string, object>();
                    var index = 0;
                    foreach (DataColumn column in reportData.Tables[0].Columns)
                    {
                        if (dataRow.ItemArray[index] is Guid)
                        {
                            var fileData = customEntityImageData.GetHtmlImageData(dataRow.ItemArray[index].ToString());
                            if (fileData != null)
                            {
                                row.Add(column.ColumnName, fileData.FileName);
                            }
                        }
                        else
                        {
                            row.Add(column.ColumnName, dataRow.ItemArray[index]);
                        }
                        
                        index++;
                    }

                    rows.Add(row);
                }

                var result = new ReportResponseComplete
                {
                    Columns = (from cReportColumn reportColumn in request.report.columns select new ReportDataColumn(reportColumn)).ToList(),
                    Data = rows
                };

                if (reportData.Tables.Count > 1)
                {
                    var user = cMisc.GetCurrentUser();
                    result.ChartPath = svcReports.GetImagePath(user) + reportData.Tables[1].Rows[0].ItemArray[0].ToString();
                }

                reports.cancelRequest(request);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Get the column names from the report.
        /// </summary>
        /// <param name="request">The current request</param>
        /// <param name="columns">A dictionary of the column order and name.</param>
        private static void GetColumnNames(cReportRequest request, Dictionary<string, string> columns)
        {
            foreach (cReportColumn reportColumn in request.report.columns)
            {
                if (reportColumn is cStandardColumn)
                {
                    columns.Add(reportColumn.order.ToString(), ((cStandardColumn)reportColumn).field.Description);
                }

                if (reportColumn is cStaticColumn)
                {
                    columns.Add(reportColumn.order.ToString(), ((cStaticColumn)reportColumn).literalname);
                }

                if (reportColumn is cCalculatedColumn)
                {
                    columns.Add(reportColumn.order.ToString(), ((cCalculatedColumn)reportColumn).columnname);
                }
            }
        }

        internal JavascriptTreeData.JavascriptTreeNode FormatCalculatedField(string id, string type, string columnName, string formattedCalculation, bool enterAtRuntime)
        {
            var result = this.FormatCalculatedField(id, type, columnName, formattedCalculation);
            return result;
        }

        #endregion
    }
}