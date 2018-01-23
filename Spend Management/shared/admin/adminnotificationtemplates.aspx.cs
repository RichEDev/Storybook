using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SpendManagementLibrary;
using System.Web.Services;

namespace Spend_Management
{
    using System.Web.UI.WebControls;

    public partial class adminnotificationtemplates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Notification Templates";
            Master.title = Title;
            
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, true, true);
                pnlAddTemplate.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Emails, true);
                string[] gridData = CreateGrid();
                string[] sysGridData = CreateSystemGrid( user);
                
                litgrid.Text = gridData[1];
                litsystemgrid.Text = sysGridData[1];

                // set the sel.grid javascript variables
                List<string> jsBlockObjects = new List<string>();
                jsBlockObjects.Add(sysGridData[0]);
                jsBlockObjects.Add(gridData[0]);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "EmailTemplateGridVars", cGridNew.generateJS_init("EmailTemplateGridVars", jsBlockObjects, user.CurrentActiveModule), true);
            }
            // Select the appropriate page option
            string templateType = Request.QueryString["template"];
            if (templateType == "custom")
            {
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:changePage('Custom'); ", true);
            }
        }

        /// <summary>
        /// Create system email template grid.
        /// </summary>
        /// <param name="user">
        /// The <see cref="CurrentUser"/>
        /// </param>
        /// <returns>
        /// The grid HTML.
        /// </returns>
        private static string[] CreateSystemGrid(CurrentUser user)
        {
            const string Sql = "SELECT sendNote, sendEmail, canSendMobileNotification, emailtemplateid, templatename, systemtemplate FROM emailTemplates";
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridSysEmailTemplates", Sql)
                           {
                               EmptyText
                                   =
                                   "No System Templates to display",
                               enablearchiving
                                   =
                                   false,
                               enabledeleting
                                   =
                                   false,
                               enableupdating
                                   =
                                   user
                                   .CheckAccessRole
                                   (
                                       AccessRoleType
                                   .Edit,
                                       SpendManagementElement
                                   .Emails,
                                       true),
                               editlink
                                   =
								   "aenotificationtemplate.aspx?templateid={emailtemplateid}",
                               KeyField
                                   =
                                   "emailtemplateid"
                           };

            grid.getColumnByName("emailtemplateid").hidden = true;
            grid.getColumnByName("systemtemplate").hidden = true;
            grid.getColumnByName("canSendMobileNotification").Width = Unit.Pixel(160);
            grid.getColumnByName("sendNote").Width = Unit.Pixel(160);
            grid.getColumnByName("sendEmail").Width = Unit.Pixel(160);
            grid.addFilter(
                ((cFieldColumn)grid.getColumnByName("systemtemplate")).field,
                ConditionType.Equals,
                new object[] { 1 },
                null,
                ConditionJoiner.None);

            return grid.generateGrid();
        }

        [WebMethod(EnableSession=true)]
        public static string[] CreateGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew newgrid = null;
            const string Sql = "SELECT sendEmail, emailtemplateid, templatename, systemtemplate FROM emailTemplates";
            newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridEmailTemplates", Sql)
                          {
                              EmptyText
                                  =
                                  "No Templates to display",
                              enablearchiving
                                  =
                                  false,
                              enabledeleting
                                  =
                                  user
                                  .CheckAccessRole
                                  (
                                      AccessRoleType
                                  .Delete,
                                      SpendManagementElement
                                  .Emails,
                                      true),
                              enableupdating
                                  =
                                  user
                                  .CheckAccessRole
                                  (
                                      AccessRoleType
                                  .Edit,
                                      SpendManagementElement
                                  .Emails,
                                      true),
                              editlink
                                  =
								  "aenotificationtemplate.aspx?templateid={emailtemplateid}",
                              deletelink
                                  =
                                  "javascript:SEL.NotificationTemplates.DeleteNotificationTemplate({emailtemplateid});",
                              KeyField = "emailtemplateid"
                          };
            newgrid.getColumnByName("emailtemplateid").hidden = true;
            newgrid.getColumnByName("systemtemplate").hidden = true;
            newgrid.getColumnByName("sendEmail").Width = Unit.Pixel(160);
            newgrid.addFilter(
                ((cFieldColumn)newgrid.getColumnByName("systemtemplate")).field,
                ConditionType.Equals,
                new object[] { 0 },
                null,
                ConditionJoiner.None);

            
            return newgrid.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        public static void DeleteNotificationTemplate(int templateid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            NotificationTemplates notifications = new NotificationTemplates(user);
            notifications.DeleteNotificationTemplate(templateid);
        }


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
