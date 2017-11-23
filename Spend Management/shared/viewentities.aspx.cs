using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Spend_Management;
using SpendManagementLibrary;
using System.Web.UI.MobileControls;
using System.Collections.Generic;
using System.Web.Services;
using System.Text;
using SpendManagementLibrary.Helpers;
using Spend_Management.shared.code;
using Spend_Management.shared.code.GreenLight;

namespace Spend_Management
{
    using System.Globalization;
    using System.Web.Script.Serialization;

    using Utilities.StringManipulation;

    /// <summary>
    /// viewentities class
    /// </summary>
    public partial class viewentities : System.Web.UI.Page
    {
        /// <summary>
        /// Stores the grid ID generated - this is used with approve workflow
        /// </summary>
        public string sGridID = string.Empty;
        /// <summary>
        /// Stores the custommenu list 
        /// </summary>
        private Dictionary<int, CustomMenuStructureItem> customMenuList;
        /// <summary>
        /// Primary Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.UseDynamicCSS = true;

            if (!IsPostBack)
            {
                CurrentUser user = cMisc.GetCurrentUser();

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                int entityid = 0;
                int viewid = 0;

                if (Request.QueryString["entityid"] != null)
                {
                    if (!Int32.TryParse(Request.QueryString["entityid"], out entityid))
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                }
                
                if (Request.QueryString["viewid"] != null)
                {
                    if (!Int32.TryParse(Request.QueryString["viewid"], out viewid))
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                }

                cCustomEntities clsentities = new cCustomEntities(user);
                cCustomEntity entity = clsentities.getEntityById(entityid);
                if (entity == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                cCustomEntityView view = entity.getViewById(viewid);
                if (view == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                if (user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, entityid, viewid, true))
                {
                    Title = view.viewname;
                    divviewname.InnerText = view.viewname;
                    Master.title = Title;
                    Master.PageSubTitle = entity.entityname;

                    var jsStr = string.Empty;
                    var serializer = new JavaScriptSerializer();

                    if (view.allowadd && user.CheckAccessRole(AccessRoleType.Add, CustomEntityElementType.View, entityid, viewid, false) && (view.DefaultAddForm != null && view.DefaultAddForm.fields.Count > 0))
                    {
                        if (entity.FormSelectionAttributeId.HasValue && view.AddFormMappings.Any())
                        {
                            cAttribute attribute = entity.getAttributeById(entity.FormSelectionAttributeId.Value);

                            var listAttribute = attribute as cListAttribute;
                            if (listAttribute != null)
                            {
                                lblFormSelectionAttributeListValue.Text = listAttribute.displayname;
                                ddlFormSelectionAttributeListValue.Items.AddRange(listAttribute.items.OrderBy(x => x.Value.elementOrder).Where(x => x.Value.Archived == false).Select(x => new ListItem(x.Value.elementText, x.Value.elementValue.ToString(CultureInfo.InvariantCulture))).ToArray());
                                textFormSelectionAttributeValue.Style.Add(HtmlTextWriterStyle.Display, "none");
                            }
                            else
                            {
                                var textAttribute = attribute as cTextAttribute;
                                if (textAttribute != null)
                                {
                                    lblFormSelectionAttributeTextValue.Text = textAttribute.displayname;
                                    txtFormSelectionAttributeTextValue.MaxLength = textAttribute.maxlength.HasValue ? textAttribute.maxlength.Value : 500;
                                    listFormSelectionAttributeValue.Style.Add(HtmlTextWriterStyle.Display, "none");
                                }
                            }

                            litadd.Text = string.Format("<a class=\"submenuitem\" href=\"javascript:SEL.CustomEntities.FormSelection.Attribute.ViewAdd({0}, {1}, {2}, {3})\">New {4}</a>", entity.entityid, viewid, 0, view.DefaultAddForm.formid, StringManipulators.HyphenateWordsLongerThanMaxWordLengthInString(entity.entityname, 11, "-<br/>"));
                            jsStr = string.Format("SEL.CustomEntities.FormSelection.Mappings.Add = {0};", serializer.Serialize(view.AddFormMappings).Replace("\"", "'"));
                        }
                        else
                        {
                            litadd.Text = string.Format("<a class=\"submenuitem\" href=\"aeentity.aspx?viewid={0}&entityid={1}&formid={2}&tabid=0\">New {3}</a>", viewid, entity.entityid, view.DefaultAddForm.formid, StringManipulators.HyphenateWordsLongerThanMaxWordLengthInString(entity.entityname, 11, "-<br/>"));
                        }
                    }

                    if (view.EditFormMappings.Any())
                    {
                        jsStr += string.Format("SEL.CustomEntities.FormSelection.Mappings.Edit = {0};", serializer.Serialize(view.EditFormMappings).Replace("\"", "'"));
                    }

                    ClientScript.RegisterStartupScript(this.GetType(), "mappingsJs", jsStr, true);

                    createGrid(view, entity);
                }

                // construct correct breadcrumbs for sitemap according to menu accessed from
                StringBuilder sb = new StringBuilder();
                sb.Append("<ol class=\"breadcrumb\">");

                // create our path back to the home page
                sb.Append("<li><a title=\"Home page\" href=\"");
                sb.Append(cMisc.Path);
                sb.Append("/Home.aspx\"> <i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\"></i>Home</a></li> ");

                // need to detect menu where in the menu structure the entity is accessed from to create the correct breadcrumb start
                cCustomEntity rootE = clsentities.getEntityById(entityid);
                sb.Append(clsentities.GetInnerBreadcrumbs(rootE, viewid));

                //create our path back
                sb.Append("<li><a title=\"");
                sb.Append(rootE.description);
				sb.Append("\" href=\"");
                sb.Append("viewentities.aspx?entityid=");
                sb.Append(rootE.entityid);
                sb.Append("&viewid=");
                sb.Append(viewid);
                sb.Append("\"><label class=\"breadcrumb_arrow\">/</label>");
                sb.Append(view.viewname);
                sb.Append("</a></li></ol>");
                Master.enablenavigation = false;
                Master.EntityBreadCrumbText = sb.ToString();
            }
        }

        /// <summary>
        /// createGrid: create the custom entity grid for display
        /// </summary>
        /// <param name="view">Custom Entity View to display</param>
        /// <param name="entity">Custom Entity to display table for</param>
        private void createGrid(cCustomEntityView view, cCustomEntity entity)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cGridNew clsgrid = new cCustomEntities(user).getEntityRecordsGrid(user, view, entity, false);
            sGridID = clsgrid.GridID;

            string[] gridData = clsgrid.generateGrid();
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "viewCEGridVars", cGridNew.generateJS_init("viewCEGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="recordid"></param>
        /// <param name="entityID"></param>
        [WebMethod(EnableSession = true)]
        public static void runWorkflow(int accountid, int recordid, int entityID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsworkflows = new cWorkflows(currentUser);
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            cWorkflow reqWorkflow = clsworkflows.GetWorkflowByID(clsworkflows.GetWorkflowIDForEntity(clsCustomEntities.getEntityById(entityID).table, recordid));

            if (clsworkflows.EntityInWorkflow(recordid, reqWorkflow.workflowid) == false)
            {
                clsworkflows.InsertIntoWorkflow(recordid, reqWorkflow.workflowid, currentUser.EmployeeID);
            }

            cWorkflowNextStep reqNextStep = clsworkflows.GetNextWorkflowStep(recordid, reqWorkflow.workflowid);
            //clsworkflows.GetNextWorkflowStep(recordid, reqWorkflow.workflowid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="recordid"></param>
        /// <param name="entityID"></param>
        [WebMethod(EnableSession = true)]
        public static void approve(int accountid, int recordid, int entityID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsworkflows = new cWorkflows(currentUser);
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

            int workflowID = clsworkflows.GetWorkflowIDForEntity(clsCustomEntities.getEntityById(entityID).table, recordid);

            cWorkflow reqWorkflow = clsworkflows.GetWorkflowByID(workflowID);
            clsworkflows.UpdateApprovalStep(recordid, reqWorkflow.workflowid, true, string.Empty);
        }


        [WebMethod(EnableSession = true)]
        public static void updateDecision(int accountid, int recordid, int entityID, bool response)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsworkflows = new cWorkflows(currentUser);
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

            cWorkflow reqWorkflow = clsworkflows.GetWorkflowByID(clsworkflows.GetWorkflowIDForEntity(clsCustomEntities.getEntityById(entityID).table, recordid));
            clsworkflows.UpdateDecisionStep(recordid, reqWorkflow.workflowid, response);
        }

        /// <summary>
        /// Close Button page event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            int viewId = 0;
            int entityId = 0;
            int.TryParse(Request.QueryString["viewid"], out viewId);
            int.TryParse(Request.QueryString["entityid"], out entityId);

            string menuURL = string.Empty;
            string menuDesc = string.Empty;

            if (viewId != 0 && entityId != 0)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cCustomEntities clsEntities = new cCustomEntities(currentUser);
                cCustomEntity entity = clsEntities.getEntityById(entityId);

                if (entity.getViewById(viewId).menuid != null)
                {
                    var customMenu = new CustomMenuStructure(currentUser.AccountID);
                    var customMenuItem = customMenu.GetCustomMenuById(entity.getViewById(viewId).menuid.Value);                  
                    clsEntities.GetMenuUrl(entity.getViewById(viewId).menuid.Value, ref menuDesc, ref menuURL, customMenuItem.SystemMenu);                    
                }
            }

            if (string.IsNullOrEmpty(menuURL))
                menuURL = cMisc.Path + "/home.aspx";

            //string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(menuURL, true);
        }
    }


}
