using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI.HtmlControls;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class aeUserdefinedOrdering : System.Web.UI.Page
    {
        public string sUnorderedListID = string.Empty;
        public string sUnorderedListNonGroupedID = string.Empty;
        public string sAppliesToID = string.Empty;
        public cTable clsAppliesTo;
        public CurrentUser currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = cMisc.GetCurrentUser();
            #region Check Access Roles - Need view on fields or groupings to view this page
            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, false) == false && currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, false) == false)
            {
                switch (currentUser.CurrentActiveModule)
                {
                    case Modules.SmartDiligence:
                    case Modules.SpendManagement:
                    case Modules.Contracts:
                        Response.Redirect("~/MenuMain.aspx?menusection=tailoring", true);
                        break;
                    default:
                        Response.Redirect("~/tailoringmenu.aspx", true);
                        break;
                }
            }
            #endregion Check Access Roles - Need view on fields or groupings to view this page

            if (IsPostBack == false)
            {
                if (string.IsNullOrEmpty(Request.QueryString["appliesTo"]) == true)
                {
                    // Redirect to notfound
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
                Guid appliesTo = Guid.Empty;



                Guid.TryParse(Request.QueryString["appliesTo"], out appliesTo);
                sAppliesToID = appliesTo.ToString();
                cTables clsTables = new cTables(currentUser.Account.accountid);
                clsAppliesTo = clsTables.GetTableByID(appliesTo);

                if (clsAppliesTo == null)
                {
                    // Redirect to not found
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                Master.PageSubTitle = "Edit " + clsAppliesTo.Description;
                Title = "Userdefined Ordering";
                Master.title = Title;
                Master.enablenavigation = false;


                bool hasFieldsOrGroups = false;

                cUserdefinedFields clsUserdefinedFields = new cUserdefinedFields(currentUser.Account.accountid);

                bool bHaveNonGroupedUDFS = false;

                #region Get userdefined fields and add to lstSortedUserdefinedFields in display order


                cTable reqUDFTable = clsTables.GetTableByID(clsAppliesTo.UserDefinedTableID);
                List<cUserDefinedField> lstUserdefinedFields = clsUserdefinedFields.GetFieldsByTable(reqUDFTable);
                //SortedList<string, cUserDefinedField> lstSortedUserdefinedFields = new SortedList<string, cUserDefinedField>();
                //SortedList<string, cUserDefinedField> lstSortedUserdefinedFieldsNonGrouped = new SortedList<string, cUserDefinedField>();

                List<cUserDefinedField> lstSortedUserdefinedFields = new List<cUserDefinedField>();
                List<cUserDefinedField> lstSortedUserdefinedFieldsNonGrouped = new List<cUserDefinedField>();

                if (lstUserdefinedFields.Count > 0)
                {
                    hasFieldsOrGroups = true;
                }

                foreach (cUserDefinedField tmpUDF in lstUserdefinedFields)
                {


                    if (tmpUDF.Grouping == null)
                    {
                        lstSortedUserdefinedFieldsNonGrouped.Add(tmpUDF);
                    }
                    else
                    {
                        lstSortedUserdefinedFields.Add(tmpUDF);
                    }

                    if (bHaveNonGroupedUDFS == false && tmpUDF.Grouping == null)
                    {
                        bHaveNonGroupedUDFS = true;
                    }
                }

                //Sort the userdefined fields into their specific order
                lstSortedUserdefinedFields.Sort(delegate(cUserDefinedField item1, cUserDefinedField item2) { return item1.order.CompareTo(item2.order); });
                lstSortedUserdefinedFieldsNonGrouped.Sort(delegate(cUserDefinedField item1, cUserDefinedField item2) { return item1.order.CompareTo(item2.order); });

                #endregion Get userdefined fields and add to lstSortedUserdefinedFields in display order


                cUserdefinedFieldGroupings clsUserdefinedGroupings = new cUserdefinedFieldGroupings(currentUser.Account.accountid);

                #region Get groups and add to lstSortedGroups in display order

                Dictionary<int, cUserdefinedFieldGrouping> lstUnsortedGroups = clsUserdefinedGroupings.GetGroupingByAssocTable(clsAppliesTo.TableID);

                if (lstUnsortedGroups.Count > 0)
                {
                    hasFieldsOrGroups = true;
                }

                if (hasFieldsOrGroups == true)
                {
                    
                #endregion Get groups and add to lstSortedGroups in display order

                    List<cUserdefinedFieldGrouping> lstSortedGroups = lstUnsortedGroups.Values.ToList();

                    #region Set sort order of the groupings

                    lstSortedGroups.Sort(delegate(cUserdefinedFieldGrouping item1, cUserdefinedFieldGrouping item2) { return item1.Order.CompareTo(item2.Order); });

                    #endregion

                    if (bHaveNonGroupedUDFS == true)
                    {
                        //lstSortedGroups.Add(Int32.MaxValue, new cUserdefinedFieldGrouping(Int32.MaxValue, "Ungrouped Userdefined Fields", Int32.MaxValue, clsAppliesTo, new Dictionary<int, List<int>>(), DateTime.Now, currentUser.Employee.employeeid, null, null));

                    }

                    #region Create the list items etc and add to the page - groups and grouped userdefined fields
                    HtmlGenericControl groupList = new HtmlGenericControl("ul");
                    groupList.ID = "udfGroupList";
                    groupList.Attributes.Add("class", " userdefinedGroupingsContainer");
                    udfFieldGroupings.Controls.Add(groupList);
                    /// Set the string that is used in the javascript - set here as the contols have just been added to the page
                    sUnorderedListID = groupList.ClientID;


                    HtmlGenericControl groupLi;
                    HtmlGenericControl fieldList;
                    HtmlGenericControl fieldLi;
                    Literal litNewLine;
                    List<string> lstSortableFieldLists = new List<string>();
                    foreach (cUserdefinedFieldGrouping tmpUDFGroup in lstSortedGroups)
                    {
                        #region new line
                        litNewLine = new Literal();
                        litNewLine.Text = "\r\n";
                        udfFieldGroupings.Controls.Add(litNewLine);
                        #endregion new line

                        #region create the grouping list item

                        groupLi = new HtmlGenericControl("li");
                        groupLi.ID = tmpUDFGroup.UserdefinedGroupID.ToString();
                        groupLi.InnerHtml = "<span> " + tmpUDFGroup.GroupName + "</span>";
                        groupList.Controls.Add(groupLi);
                        #endregion create the grouping list item

                        fieldList = new HtmlGenericControl("ul");
                        fieldList.ID = "group" + tmpUDFGroup.UserdefinedGroupID + "_fields";
                        fieldList.Attributes.Add("class", "userdefinedField");
                        foreach (cUserDefinedField tmpUDF in lstSortedUserdefinedFields)
                        {
                            if ((tmpUDF.Grouping != null && tmpUDF.Grouping.UserdefinedGroupID == tmpUDFGroup.UserdefinedGroupID) || (tmpUDF.Grouping == null && tmpUDFGroup.UserdefinedGroupID == Int32.MaxValue))
                            {
                                #region tab
                                litNewLine = new Literal();
                                litNewLine.Text = "\t";
                                groupLi.Controls.Add(litNewLine);
                                #endregion tab

                                fieldLi = new HtmlGenericControl("li");
                                fieldLi.Attributes.Add("listtype", "UserdefinedField");
                                fieldLi.InnerHtml = "<span class=\"userdefinedFieldSpan\">" + tmpUDF.label + "</span>";
                                fieldLi.ID = tmpUDF.userdefineid.ToString();
                                fieldList.Controls.Add(fieldLi);
                                #region new line
                                litNewLine = new Literal();
                                litNewLine.Text = "\r\n";
                                fieldList.Controls.Add(litNewLine);
                                #endregion new line
                            }
                        }
                        groupLi.Controls.Add(fieldList);
                        lstSortableFieldLists.Add(fieldList.ClientID);
                    }
                    #endregion Create the list items etc and add to the page - groups and grouped userdefined fields




                    #region Create the list items etc and add to the page - non grouped userdefined fields
                    HtmlGenericControl groupListNonGrouped = new HtmlGenericControl("ul");
                    groupListNonGrouped.ID = "udfGroupListNonGrouped";
                    groupListNonGrouped.Attributes.Add("class", "userdefinedGroupingsContainer");
                    udfNonGroupedFields.Controls.Add(groupListNonGrouped);
                    /// Set the string that is used in the javascript - set here as the contols have just been added to the page
                    sUnorderedListNonGroupedID = groupListNonGrouped.ClientID;


                    HtmlGenericControl groupLiNonGrouped;
                    HtmlGenericControl fieldListNonGrouped;
                    HtmlGenericControl fieldLiNonGrouped;
                    Literal litNewLineNonGrouped;

                    List<string> lstSortableFieldListsNonGrouped = new List<string>();

                        #region new line
                        litNewLineNonGrouped = new Literal();
                        litNewLineNonGrouped.Text = "\r\n";
                        udfNonGroupedFields.Controls.Add(litNewLineNonGrouped);
                        #endregion new line

                        #region create the grouping list item
                        groupLiNonGrouped = new HtmlGenericControl("li");
                        groupLiNonGrouped.ID = Int32.MaxValue.ToString();
                        groupLiNonGrouped.InnerHtml = "<span>Non-Grouped Userdefined Fields</span>";
                        groupListNonGrouped.Controls.Add(groupLiNonGrouped);
                        #endregion create the grouping list item

                        fieldListNonGrouped = new HtmlGenericControl("ul");
                        fieldListNonGrouped.ID = "group" + Int32.MaxValue.ToString() + "_fields";
                        fieldListNonGrouped.Attributes.Add("class", "userdefinedField");

                        foreach (cUserDefinedField tmpUDF in lstSortedUserdefinedFieldsNonGrouped)
                        {
                                #region tab
                                litNewLineNonGrouped = new Literal();
                                litNewLineNonGrouped.Text = "\t";
                                groupLiNonGrouped.Controls.Add(litNewLineNonGrouped);
                                #endregion tab
                                fieldLiNonGrouped = new HtmlGenericControl("li");
                                fieldLiNonGrouped.Attributes.Add("listtype", "UserdefinedField");
                                fieldLiNonGrouped.InnerHtml = "<span class=\"userdefinedFieldSpan\">" + tmpUDF.label + "</span>";
                                fieldLiNonGrouped.ID = tmpUDF.userdefineid.ToString();
                                fieldListNonGrouped.Controls.Add(fieldLiNonGrouped);
                                #region new line
                                litNewLineNonGrouped = new Literal();
                                litNewLineNonGrouped.Text = "\r\n";
                                fieldListNonGrouped.Controls.Add(litNewLineNonGrouped);
                                #endregion new line
                        }
                        groupLiNonGrouped.Controls.Add(fieldListNonGrouped);

                        lstSortableFieldListsNonGrouped.Add(fieldListNonGrouped.ClientID);




                   
                    #endregion Create the list items etc and add to the page - non grouped userdefined fields

                    StringBuilder sbHelpSection = new StringBuilder("Drag and drop an item to change its display order.<br />");


                    #region Add the startup script to make them draggable and check access roles for specifics
                    StringBuilder sb = new StringBuilder();
                    string sUnGroupedUDFS = string.Empty;
                    /// Check if the current person can edit/add fields - decides if they can edit orders for fields
                    if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserDefinedFields, false) == true || currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserDefinedFields, false) == true)
                    {
                        if (lstSortedUserdefinedFieldsNonGrouped.Count > 1)
                        {
                            sUnGroupedUDFS = "Userdefined fields that are not part of a group will always appear at the bottom of the page."; 
                            foreach (string str in lstSortableFieldListsNonGrouped)
                            {
                                sb.Append("Sortable.create('" + str + "', { constraint: false, dropOnEmpty: true, containment: ['" + str + "'] });\n");
                            }
                        }


                        if (lstSortedUserdefinedFields.Count > 1)
                        {
                            sbHelpSection.Append("A Userdefined field can be re-arranged to alter its display order within its own group.<br />");
                            foreach (string str in lstSortableFieldLists)
                            {
                                sb.Append("Sortable.create('" + str + "', { constraint: false, dropOnEmpty: true, containment: ['" + str + "'] });\n");
                            }
                        }


                    }


                    /// Check if the current person can edit/add groupings - decides if they can edit orders for groupings
                    if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserdefinedGroupings, false) == true || currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserdefinedGroupings, false) == true)
                    {
                        if (lstUnsortedGroups.Count > 1)
                        {
                            sbHelpSection.Append("A Userdefined grouping can be re-arranged to alter its display order on the page.<br />");
                            sb.Append("Sortable.create('" + sUnorderedListID + "', { constraint: false, dropOnEmpty: true, containment: ['" + sUnorderedListID + "'] });\n");
                        }
                    }
                    if (sUnGroupedUDFS != string.Empty)
                    {
                        sbHelpSection.Append(sUnGroupedUDFS);
                    }

                    if (sb.Length > 0)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "js", sb.ToString(), true);
                        litHelpGuide.Text = sbHelpSection.ToString();

                        if (sbHelpSection.Length > 0)
                        {
                            divComments.Attributes.Add("class", "comment");
                        }
                    }
                    #endregion Add the startup script to make them draggable and check access roles for specifics
                }
                else
                {
                    Literal lit = new Literal();
                    lit.Text = "There are no userdefined groups or fields to modify for this area.";
                    udfFieldGroupings.Controls.Add(lit);
                }
            }
        }


        protected void cmbcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("userdefinedOrdering.aspx", true);
        }
    }
}
