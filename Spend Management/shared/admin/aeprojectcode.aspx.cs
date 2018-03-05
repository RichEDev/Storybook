namespace Spend_Management
{
    using System;
    using System.Text;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.UserDefinedFields;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for aeprojectcode.
    /// </summary>
    public partial class aeprojectcode : Page
    {
        [Dependency]
        public IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> ProjectCodesRepository { get; set; }

        private CurrentUser _currentUser;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this._currentUser = cMisc.GetCurrentUser();
            this._currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProjectCodes, true, true);

            Master.title = this.Title;
            Master.helpid = 1017;
            int projectCodeId = 0;
            UserDefinedFieldValueCollection udfValues = null;
            if (this.IsPostBack == false)
            {
                this.cmdok.Attributes.Add("onclick", "if (validateform(null) == false) {return;}");
                this.Master.enablenavigation = false;
                
                if (this.Request.QueryString["projectcodeid"] != null)
                {
                    projectCodeId = Convert.ToInt32(this.Request.QueryString["projectcodeid"]);
                }

                this.ViewState["projectcodeid"] = projectCodeId;

                if (projectCodeId > 0)
                {
                    var reqCode = this.ProjectCodesRepository[projectCodeId];

                    if (reqCode == null)
                    {
                        this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    this.txtprojectcode.Text = reqCode.Name;
                    this.txtdescription.Text = reqCode.Description;
                    this.chkrechargeable.Checked = reqCode.Rechargeable;
                    this.Master.title = "Project Code: " + reqCode.Name;
                    udfValues = reqCode.UserDefinedFieldValues;
                }
                else
                {
                    this.Master.title = "Project Code: New";
                }

                this.Master.PageSubTitle = "Project Code Details";
            }

            cUserdefinedFields clsuserdefined = new cUserdefinedFields(this._currentUser.AccountID);
            cTables clstables = new cTables(this._currentUser.AccountID);
            cTable tbl = clstables.GetTableByID(new Guid("e1ef483c-7870-42ce-be54-ecc5c1d5fb34"));
            StringBuilder udfscript;
            clsuserdefined.createFieldPanel(ref this.holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), string.Empty, out udfscript);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "udfscript", udfscript.ToString(), true);

            if (udfValues != null)
            {
                clsuserdefined.populateRecordDetails(ref this.holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), udfValues.ToSortedList());
            }

        }

        #region Web Form Designer generated code

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdok.Click += new ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new ImageClickEventHandler(this.cmdcancel_Click);

        }

        #endregion

        private void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect("adminprojectcodes.aspx", true);
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            int projectcodeid = (int)this.ViewState["projectcodeid"];
            string projectCodeName = this.txtprojectcode.Text;
            string description = this.txtdescription.Text;

            bool rechargeable = this.chkrechargeable.Checked;
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(this._currentUser.AccountID);
            cTables clstables = new cTables(this._currentUser.AccountID);
            cTable tbl = clstables.GetTableByID(new Guid("e1ef483c-7870-42ce-be54-ecc5c1d5fb34"));

            var udf = new UserDefinedFieldValueCollection(clsuserdefined.GetUserDefinedFieldsFromPage(ref this.holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID)));

            bool archived;
            if (projectcodeid > 0)
            {
                var oldCode = this.ProjectCodesRepository[projectcodeid];
                archived = oldCode.Archived;
            }
            else
            {
                archived = false;
            }

            var projectcode = new ProjectCodeWithUserDefinedFields(new ProjectCode(projectcodeid, projectCodeName, description, archived, rechargeable), udf);
            var createdProjectCode = this.ProjectCodesRepository.Save(projectcode);

            if (createdProjectCode.Id == -1)
            {
                this.ClientScript.RegisterStartupScript(this.Page.GetType(), "alert", "alert('The project code name you have entered already exists')", true);
            }
            else if (createdProjectCode.Id == -2)
            {
                this.ClientScript.RegisterStartupScript(this.Page.GetType(), "alert", "alert('The description you have entered already exists')", true);
            }
            else
            {
                this.Response.Redirect("adminprojectcodes.aspx", true);
            }
        }
    }
}