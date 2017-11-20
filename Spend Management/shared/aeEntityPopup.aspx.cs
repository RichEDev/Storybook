using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using SpendManagementLibrary;

namespace Spend_Management.shared
{
    public partial class aeEntityPopup : Page
    {
        public string sGridID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme) { UseDynamicCSS = true };
            favLink.Href = clsMasterPageMethods.GetFavIcon();
            clsMasterPageMethods.SetupDynamicStyles(ref this.Head1);
            clsMasterPageMethods.SetupJQueryReferences(ref this.jQueryCss, ref this.scriptman);
            this.litStyles.Text = cColours.customiseStyles(false);

            int entityId = 0;
            int viewId = 0;
            int formId = 0;
            int relRecordIds = 0;
            int relTabId = 0;
            int relFormId = 0;
            int relEntityId = 0;
            int attributeId = 0;

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");

            #region Process QueryString

            if (this.Request["entityid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["entityid"]), out entityId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            if (this.Request["viewid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["viewid"]), out viewId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }       
            if (this.Request["reltabid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["reltabid"]), out relTabId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            if (this.Request["relrecordid"] != null)
            {

                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["relrecordid"]), out relRecordIds))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            if (this.Request["relformid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["relformid"]), out relFormId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            if (this.Request["relentityid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["relentityid"]), out relEntityId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            if (this.Request["attributeid"] != null)
            {
                if (!int.TryParse(this.GetLastQueryStringRecord(this.Request["attributeid"]), out attributeId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
            }

            #endregion

            CurrentUser user = cMisc.GetCurrentUser();
            var entities = new cCustomEntities(user);

            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            cCustomEntity entity = entities.getEntityById(entityId);
            cCustomEntityView view = entity.getViewById(viewId);
            cCustomEntity parentEntity = entities.getEntityById(relEntityId);
            cAttribute att = parentEntity.getAttributeById(attributeId);
            var oneToMany = (cOneToManyRelationship)att;         
            this.Title = view.viewname;
            this.divViewName.InnerText = entity.entityname;
            Page.Title = this.Title;

            this.createGrid(view, entity, relRecordIds, oneToMany, relEntityId, relFormId, relTabId, viewId);

            string validationGroup = "vgCE_" + entity.entityid.ToString(CultureInfo.InvariantCulture);

            if (view.DefaultAddForm != null)
            {
                formId = view.DefaultAddForm.formid;
                this.lnkNew.Text = "New " + entity.entityname;
                this.lnkNew.ClientIDMode = ClientIDMode.Static;
                this.lnkNew.ValidationGroup = validationGroup;
                this.lnkNew.CommandArgument =
                    relEntityId + "," + relFormId + "," + relRecordIds + "," + relTabId + ","
                    + 1 + "," + entityId + "," + formId + ","
                    + ",0" + "," + viewId + "," + attributeId;
                this.lnkNew.OnClientClick =
                    "javascript:if (!validateform('" + validationGroup + "')) { return true; }";
                this.lnkNew.Click += this.lnkonetomany_Click;
            }
            else
            {
                this.lnkNew.Visible = false;
            }
        }

        private void lnkonetomany_Click(object sender, EventArgs e)
        {
            var lnk = (LinkButton)sender;
            var pgEntity = (aeEntityPopup)lnk.Page;

            int entityID;
            int formID;
            int viewID;
            int tabID;
            int popupviewID;
            int attributeID;

            string returnURL;
            string[] temp = lnk.CommandArgument.Split(',');
            string[] arrRelentityID = temp[0].Split('_');
            string[] arrRelformID = temp[1].Split('_');
            string[] arrRelrecordID = temp[2].Split('_');
            string[] arrReltabID = temp[3].Split('_');
            string[] arrRelviewID = temp[4].Split('_');

            if (arrReltabID.Length > 0)
            {
                HiddenField curTab = pgEntity.hiddenCETab;
                var tabs = (TabContainer)pgEntity.FindControl("tabs" + arrRelformID[arrRelformID.Length - 1]);

                if (tabs != null)
                {
                    for (int i = 0; i < tabs.Tabs.Count; i++)
                    {
                        if (tabs.Tabs[i].ID == "tab" + arrReltabID[arrReltabID.Length - 1])
                        {
                            arrReltabID[arrReltabID.Length - 1] = tabs.Tabs[i].ID.Replace("tab", string.Empty);
                            curTab.Value = i.ToString();
                            break;
                        }
                    }
                }
            }
            var crumbs = new List<sEntityBreadCrumb>();
            for (int i = 0; i < arrRelentityID.GetLength(0); i++)
            {
                var crumb = new sEntityBreadCrumb(Convert.ToInt32(arrRelentityID[i]), 
                                                  Convert.ToInt32(arrRelformID[i]),
                                                  Convert.ToInt32(arrRelrecordID[i]), 
                                                  Convert.ToInt32(arrReltabID[i]),
                                                  Convert.ToInt32(arrRelviewID[i]));
                crumbs.Add(crumb);
            }

            viewID = Convert.ToInt32(temp[4]);
            entityID = Convert.ToInt32(temp[5]);
            formID = Convert.ToInt32(temp[6]);
            tabID = Convert.ToInt32(temp[8]);
            popupviewID = Convert.ToInt32(temp[9]);
            attributeID = Convert.ToInt32(temp[10]);

            {
                returnURL = "~/shared/aeentity.aspx?";
                string entityURL = string.Empty;
                string formURL = string.Empty;
                string recordURL = string.Empty;
                string tabURL = string.Empty;
                foreach (sEntityBreadCrumb c in crumbs)
                {
                    entityURL += "relentityid=" + c.EntityID + "&";
                    formURL += "relformid=" + c.FormID + "&";
                    recordURL += "relrecordid=" + c.RecordID + "&";
                    tabURL += "reltabid=" + c.TabID + "&";
                    tabURL += "relviewid=" + c.ViewID + "&";
                }
                returnURL += entityURL + formURL + recordURL + tabURL + "viewid=" + viewID + "&entityid=" + entityID +
                             "&formid=" + formID + "&tabid=" + tabID + "&popupview=" + popupviewID + "&attributeid=" +
                             attributeID;
                HttpContext.Current.Response.Redirect(returnURL, true);
            }
        }

        private void createGrid(cCustomEntityView view, cCustomEntity entity, int relrecordids, cOneToManyRelationship oneToMany, int relentityids, int relformids, int reltabids, int viewid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsEntities = new cCustomEntities(user);
            var columns = new List<cNewGridColumn>();
            cAttribute keyatt = entity.getKeyField();       

            // ID Field Check -- added to see if the view.fields list already contains the keyatt field
            bool bContainsKeyField = false;
            bool containsArchiveField = false;
            var clsfields = new cFields((int)ViewState["accountid"]);
            cField keyfield = clsfields.GetFieldByID(keyatt.fieldid);
            cAttribute archiveFieldAttribute = entity.GetAttributeByDisplayName("Archived");
            cField archiveField = clsfields.GetFieldByID(archiveFieldAttribute.fieldid);
            foreach (cCustomEntityViewField viewField in view.fields.Values)
            {
                // ID Field Check -- checking the field rather than fieldid as it's the field name in the SQL that we are trying not to duplicate on the table
                if (viewField.Field.FieldName == keyfield.FieldName && viewField.Field.TableID == keyfield.TableID &&
                    viewField.JoinVia == null)
                {
                    bContainsKeyField = true;
                }
                if (viewField.Field.FieldName == archiveField.FieldName && viewField.Field.TableID == archiveField.TableID && viewField.JoinVia == null)
                {
                    containsArchiveField = true;
                }


                if (viewField.JoinVia != null)
                {
                    columns.Add(new cFieldColumn(viewField.Field, viewField.JoinVia));
                }
                else if (viewField.Field.GenList && viewField.Field.ListItems.Count == 0)
                {
                    columns.Add(new cFieldColumn(viewField.Field, viewField.Field.GetLookupField()));
                }
                else
                {
                    columns.Add(new cFieldColumn(viewField.Field));
                }

                if (viewField.Field.ListItems.Count > 0)
                {
                    foreach (int s in viewField.Field.ListItems.Keys)
                    {
                        ((cFieldColumn)columns[columns.Count - 1]).addValueListItem(s,
                                                                                     viewField.Field.ListItems[s]);
                    }
                }

                if (viewField.Field.FieldType == "CL")
                {
                    var clsGCurrencies = new cGlobalCurrencies();
                    var clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

                    foreach (cCurrency c in clsCurrencies.currencyList.Values)
                    {
                        ((cFieldColumn)columns[columns.Count - 1]).addValueListItem(c.currencyid,
                                                                                     clsGCurrencies
                                                                                         .getGlobalCurrencyById(
                                                                                             c.globalcurrencyid).label);
                    }
                }
            }

            bool bHideCurrencyField = false;
            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                cField greenLightCurrencyField = clsfields.GetFieldByID(entity.getGreenLightCurrencyAttribute().fieldid);
                if (
                    !(from x in view.fields.Values where x.Field.FieldID == greenLightCurrencyField.FieldID select x)
                         .Any())
                {
                    columns.Insert(0, new cFieldColumn(greenLightCurrencyField));
                    bHideCurrencyField = true;
                }
            }

            // ID Field Check -- if the id field is not already in the columns list insert it now
            if (bContainsKeyField == false)
            {
                columns.Insert(0, new cFieldColumn(keyfield));
            }
            if (containsArchiveField == false)
            {
                columns.Insert(0, new cFieldColumn(archiveField));
            }
            var clsgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["employeeid"],
                                       "grid" + entity.entityid + view.viewid + oneToMany.attributeid, entity.table,
                                       columns);
            sGridID = clsgrid.GridID;
            clsgrid.KeyField = keyfield.FieldName;
            clsgrid.ArchiveField = archiveField.FieldName;
            #region Set the sorting of the grid

            cNewGridSort employeeSort =
                user.Employee.GetNewGridSortOrders().GetBy("grid" + entity.entityid + view.viewid + keyatt.attributeid);

            // if default sort set for view, use this. Need to let this get overridden by user default though
            if (employeeSort == null && view.SortColumn.FieldID != Guid.Empty)
            {
                cNewGridColumn SortCol = null;

                SortCol = clsgrid.getColumnByID(view.SortColumn.FieldID,
                                                (view.SortColumn.JoinVia != null ? view.SortColumn.JoinVia.JoinViaID : 0));

                if (SortCol != null)
                {
                    clsgrid.SortedColumn = SortCol;
                    clsgrid.SortDirection = view.SortColumn.SortDirection;
                }
            }

            #endregion

            clsgrid.CssClass = "datatbl";
            if (bContainsKeyField == false) // ID Field Check -- if the key field should not be in the grid, hide it
            {
                clsgrid.getColumnByName(clsgrid.KeyField).hidden = true;
            }

            cFields fields;
            fields = new cFields(user.AccountID);
            //filter grid data for the record id
            clsgrid.addFilter(fields.GetFieldByID(oneToMany.fieldid), ConditionType.Equals, new object[] { relrecordids },
                              new object[] { }, ConditionJoiner.None);

            if (view.allowdelete)
            {
                clsgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, CustomEntityElementType.View, view.entityid, view.viewid, false);
                clsgrid.deletelink = "javascript:deleteRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + "," + oneToMany.attributeid.ToString() + ")";
            }

            if (view.allowarchive)
            {
                    clsgrid.ArchiveField = "Archived";
                    clsgrid.enablearchiving = user.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, view.entityid, view.viewid, false);
                    clsgrid.archivelink = "javascript:archiveRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + ", '{" + archiveField.FieldName + "}'," + oneToMany.attributeid.ToString() + ")";
           }
            if (containsArchiveField == false)
            {
                clsgrid.getColumnByName(clsgrid.ArchiveField).hidden = true;
            }
            if (view.allowedit)
            {
                if (view.DefaultEditForm != null && view.DefaultEditForm.fields.Count > 0)
                {
                    clsgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, view.entityid, view.viewid, false);

                    if (clsgrid.enableupdating)
                    {
                        string returnurl = "aeentity.aspx?";
                        string entityurl = string.Empty;
                        string formurl = string.Empty;
                        string recordurl = string.Empty;
                        string taburl = string.Empty;
                 
                        entityurl += "relentityid=" + relentityids + "&";
                        formurl += "relformid=" + relformids + "&";
                        recordurl += "relrecordid=" + relrecordids + "&";
                        taburl += "reltabid=" + reltabids + "&";
                        taburl += "relviewid=" + viewid + "&";                 
                        returnurl += string.Format("{0}{1}{2}{3}viewid={4}&entityid={5}&formid={6}&tabid=0&id={{{7}}}" + "&popupview={8}&attributeid={9}", entityurl, formurl, recordurl, taburl, view.viewid, view.entityid, view.DefaultEditForm.formid, keyfield.FieldName, viewid, oneToMany.attributeid);
                        clsgrid.editlink = returnurl;
                    }
                }
                else
                {
                    clsgrid.enableupdating = false;
                }
            }
            var gridInfo = new SerializableDictionary<string, object>();
            if (entity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceRecStatus =
                clsEntities.GetAudienceRecords(entity.entityid, user.EmployeeID);          
                gridInfo.Add("keyfield", keyfield.FieldName);
                gridInfo.Add("employeeid", user.EmployeeID);
                gridInfo.Add("accountid", user.AccountID);
                gridInfo.Add("gridid", clsgrid.GridID);
                gridInfo.Add("entityid", entity.entityid);
                var hiddenRecs = new List<int>();
                foreach (var kvp in audienceRecStatus)
                {
                    if (((cAudienceRecordStatus)kvp.Value).Status == 0)
                    {
                        hiddenRecs.Add(((cAudienceRecordStatus)kvp.Value).RecordID);
                    }
                }
                clsgrid.HiddenRecords = hiddenRecs;
            }
            if (view.filters != null)
            {
                foreach (var kvp in view.filters)
                {
                    FieldFilter curFilter = kvp.Value;
                    FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(
                        curFilter, user);

                    clsgrid.addFilter(curFilter.Field, filterValues.conditionType, filterValues.valueOne,
                                      filterValues.valueTwo, ConditionJoiner.And, curFilter.JoinVia);
                }
            }

            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                clsgrid.CurrencyColumnName = "GreenLightCurrency";
                clsgrid.getColumnByName("GreenLightCurrency").hidden = bHideCurrencyField;
                clsgrid.CurrencyId = entity.DefaultCurrencyID.Value;
            }
        
            clsgrid.InitialiseRowGridInfo = gridInfo;
            clsgrid.InitialiseRow += clsgrid_InitialiseRow;
            clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.aeEntityPopup";
            clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsgrid_InitialiseRow";
            clsgrid.EmptyText = "There are currently no " + entity.pluralname + " defined.";

            string[] gridData = clsgrid.generateGrid();
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(GetType(), "viewCEGridVars",
                                                    cGridNew.generateJS_init("viewCEGridVars",
                                                                             new List<string> { gridData[0] },
                                                                             user.CurrentActiveModule),
                                                    true);
        }

        private void clsgrid_InitialiseRow(cNewGridRow row, Dictionary<string, object> gridInfo)
        {
            if (row.getCellByID("formfieldcount") != null)
            {
                if (Convert.ToUInt32(row.getCellByID("formfieldcount").Value) == 0)
                {
                    // no form fields exist, so prevent access by hiding the edit icon
                    row.enableupdating = false;
                }
            }
            if (gridInfo.ContainsKey("keyfield") && gridInfo.ContainsKey("gridid"))
            {
                CurrentUser user = cMisc.GetCurrentUser();               
                cCustomEntities entities = new cCustomEntities(user);           
                SerializableDictionary<string, object> audienceStatus = entities.GetAudienceRecords((int)gridInfo["entityid"], (int)gridInfo["employeeid"]);
                cGridNew.InitialiseRowAudienceCheck(ref row, gridInfo["gridid"].ToString(), gridInfo["keyfield"].ToString(), audienceStatus);
            }
        }

        private string GetLastQueryStringRecord(string recordsList)
        {
            var records = recordsList.Split('_');
            string lastRecord = string.Empty;
            foreach (var record in records)
            {
                lastRecord = record;
            }

            return lastRecord;
        }
    }
}