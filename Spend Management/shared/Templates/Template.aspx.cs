namespace Spend_Management.shared.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using SpendManagementLibrary;

    /// <summary>
    /// Level based matrix summary
    /// </summary>
    public partial class TemplatePage : Page
    {
        /// <summary>
        /// The page_ load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            const string ElementName = "Template";
            const string ElementId = "templateid";
            this.Master.enablenavigation = false;
            this.Master.UseDynamicCSS = true;
            
            // Set help id for this page
            this.Master.helpid = 99999;

            if (this.IsPostBack)
            {
                return;
            }

            CurrentUser currentUser = cMisc.GetCurrentUser();

            // Check access role allows access
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.None, true, true);

            var templates = new ClassMethods();

            int templateid;
            bool addingNew = false;

            if (this.Request.QueryString[ElementId] != null)
            {
                if (!int.TryParse(this.Request.QueryString[ElementId], out templateid))
                {
                    this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                var currentTemplate = templates.GetById(templateid);

                if (currentTemplate == null)
                {
                    this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                if (!currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.None, true))
                {
                    this.btnTemplateItemSave.Visible = false;
                    this.btnTemplateItemSubItemSave.Visible = false;
                    this.lnkNewTemplateItem.Visible = false;
                }

                this.Master.title = string.Format("{0}: {1}", ElementName, currentTemplate);

                this.txtTemplateItemName.Text = currentTemplate;
                this.txtTemplateItemDescription.Text = currentTemplate;
            }
            else
            {
                templateid = 0;
                currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.None, true, true);
                this.Master.title = string.Format("New {0}", ElementName);
                addingNew = true;
            }

            this.Master.PageSubTitle = string.Format("{0} Details", ElementName);

            this.ClientScript.RegisterStartupScript(this.GetType(), "variables", "SEL.Templates.IDs.templateid = " + templateid + ";\n", true);

            var gridData = templates.GetTemplateItemSubItemGrid(templateid, addingNew);
            this.litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "gridVars", cGridNew.generateJS_init("gridVars", new List<string> { gridData[0] }, currentUser.CurrentActiveModule), true);
        }

        /// <summary>
        /// Methods that will be moved to data class when this object goes live.
        /// </summary>
        internal class ClassMethods
        {
            /// <summary>
            /// The get by id.
            /// </summary>
            /// <param name="templateid">
            /// The template id.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string GetById(int templateid)
            {
                return "ObjectFromId";
            }

            /// <summary>
            /// Insert Description.
            /// </summary>
            /// <param name="templateId">
            /// The template id.
            /// </param>
            /// <param name="addingNew">
            /// The adding new.
            /// </param>
            /// <returns>
            /// The return parameter/>.
            /// </returns>
            public string[] GetTemplateItemSubItemGrid(int templateId, bool addingNew)
            {
                return new string[1];
            }
        }
    }
}