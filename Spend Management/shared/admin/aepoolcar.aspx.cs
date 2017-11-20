using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    public partial class aepoolcar : System.Web.UI.Page
    {
        int nCarID;
        /// <summary>
        /// CarID for JS
        /// </summary>
        public int CarID
        {
            get { return this.nCarID; }
        }

        /// <summary>
        /// page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
			Title = "Add / Edit Pool Vehicles";
            Master.PageSubTitle = Title;
			Master.showdummymenu = true;

            if (Page.IsPostBack == false)
            {
                Master.enablenavigation = false;

                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PoolCars, true, true);

                var clsPoolCars = new cPoolCars(currentUser.AccountID);

                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;
                aeCar.EmployeeAdmin = true;
                aeCar.AccountID = currentUser.AccountID;
                aeCar.EmployeeID = currentUser.EmployeeID;
                aeCar.isPoolCar = true;
                aeCar.HideButtons = true;

                aeCar.ReturnURL = "~/shared/admin/poolcars.aspx";

                int.TryParse(Request.QueryString["carID"], out this.nCarID);

                if (Request.QueryString["action"] == "2" && (Request.QueryString["carID"] != null && Request.QueryString["carID"] != ""))
                {
                    ViewState["action"] = Request.QueryString["action"];
                    ViewState["carid"] = int.Parse(Request.QueryString["carID"]);
                    ViewState["users"] = clsPoolCars.GetUsersPerPoolCar((int)ViewState["carid"]);
                    aeCar.Action = aeCarPageAction.Edit;
                    aeCar.CarID = int.Parse(Request.QueryString["carID"]);

                    var sbJS = new StringBuilder();

                    sbJS.Append("function pcEditCar()\n{\n");
                    sbJS.Append("\teditCar(" + this.nCarID + ");\n");
                    sbJS.Append("}\n");
                    sbJS.Append("if (window.addEventListener) // W3C standard\n");
                    sbJS.Append("{\n");
                    sbJS.Append("\twindow.addEventListener('load', pcEditCar, false);\n");
                    sbJS.Append("}\n");
                    sbJS.Append("else if (window.attachEvent) // Microsoft\n");
                    sbJS.Append("{\n");
                    sbJS.Append("\twindow.attachEvent('onload', pcEditCar);\n");
                    sbJS.Append("}\n");

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "jsCarEdit", sbJS.ToString(), true);
                }
                else
                {
                    ViewState["action"] = 0;
                    aeCar.Action = aeCarPageAction.Add;

                    // load the existing pool cars so the user can choose to replace old with new
                    aeCar.cmbPreviousCar.Items.AddRange(clsPoolCars.CreateDropDownArray()); 
                }

                string[] poolCarUsersGrid = clsPoolCars.CreatePoolCarsUsersGrid(this.nCarID);
                litPoolCarUsersGrid.Text = poolCarUsersGrid[2];

                string[] poolCarEmployeeGrid = clsPoolCars.CreatePoolCarEmployeeGrid();
                litUsersGrid.Text = poolCarEmployeeGrid[2];
               
                // set the sel.grid javascript variables
                List<string> jsBlockObjects = new List<string>();
                jsBlockObjects.Add(poolCarEmployeeGrid[1]);
                jsBlockObjects.Add(poolCarUsersGrid[1]);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "aePoolCarsGridVars", cGridNew.generateJS_init("aePoolCarsGridVars", jsBlockObjects, currentUser.CurrentActiveModule), true);
            }
        }
    }
}
