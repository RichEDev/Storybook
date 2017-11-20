using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using SpendManagementLibrary;

using Spend_Management;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for aedepartment.
    /// </summary>
    public partial class aedepartment : Page
    {

        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        private cDepartments clsdepartments;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Department";
            Master.title = Title;
            Master.PageSubTitle = "Department Details";
            Master.helpid = 1021;


            if (IsPostBack == false)
            {
                cmdok.Attributes.Add("onclick", "if (validateform(null) == false) {return;}");
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Departments, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                clsdepartments = new cDepartments(user.AccountID);

                int departmentid = 0;
                if (Request.QueryString["departmentid"] != null)
                {
                    departmentid = Convert.ToInt32(Request.QueryString["departmentid"]);
                }
                ViewState["departmentid"] = departmentid;

                if (departmentid > 0) //update
                {


                    cDepartment reqdepartment;
                    reqdepartment = clsdepartments.GetDepartmentById(departmentid);
                    if (reqdepartment == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                    txtdepartment.Text = reqdepartment.Department;
                    txtdescription.Text = reqdepartment.Description;
                    ViewState["record"] = reqdepartment.UserdefinedFields;
                    Master.title = "Department: " + reqdepartment.Department;
                }
                else
                {
                    Master.title = "Department: New";
                }

            }

            cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
            cTables clstables = new cTables((int)ViewState["accountid"]);
            cTable tbl = clstables.GetTableByID(new Guid("a0f31cb0-16bb-4ace-aaea-69a7189d9599"));
            StringBuilder udfscript;
            clsuserdefined.createFieldPanel(ref holderUserdefined, tbl.GetUserdefinedTable(), "vgDep", out udfscript);
            if (ViewState["record"] != null)
            {
                clsuserdefined.populateRecordDetails(ref holderUserdefined, tbl.GetUserdefinedTable(), (SortedList<int, object>)ViewState["record"]);
            }

            if (udfscript.Length > 0)
            {
                ClientScript.RegisterStartupScript(GetType(), "udfautocomplete", udfscript.ToString(), true);
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
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
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }

        #endregion

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string department;
            string description;

            int departmentid = (int)ViewState["departmentid"];
            department = txtdepartment.Text;
            description = txtdescription.Text;



            clsdepartments = new cDepartments((int)ViewState["accountid"]);

            cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
            ;
            ;
            cTables clstables = new cTables((int)ViewState["accountid"]);
            cTable tbl = clstables.GetTableByID(new Guid("a0f31cb0-16bb-4ace-aaea-69a7189d9599"));
            SortedList<int, object> udf = clsuserdefined.getItemsFromPanel(ref holderUserdefined, tbl.GetUserdefinedTable());

            DateTime createdon;
            int createdby;
            int? modifiedby;
            DateTime? modifiedon;
            bool archived;
            if (departmentid > 0)
            {
                cDepartment olddep = clsdepartments.GetDepartmentById(departmentid);
                createdon = olddep.CreatedOn;
                createdby = olddep.CreatedBy;
                modifiedby = (int)ViewState["employeeid"];
                modifiedon = DateTime.Now;
                archived = olddep.Archived;
            }
            else
            {
                createdon = DateTime.Now;
                createdby = (int)ViewState["employeeid"];
                modifiedby = null;
                modifiedon = null;
                archived = false;
            }
            cDepartment dep = new cDepartment(departmentid, department, description, archived, createdon, createdby, modifiedon, modifiedby, udf);
            departmentid = clsdepartments.SaveDepartment(dep);
            if (departmentid == -1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The department name you have entered already exists')", true);
            }
            else if (departmentid == -2)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The description you have entered already exists')", true);
            }
            else
            {
                Response.Redirect("admindepartments.aspx", true);
            }
        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("admindepartments.aspx", true);
        }
    }
}