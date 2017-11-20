namespace Spend_Management
{
    using System.Collections;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;

    /// <summary>
    /// Add Edit car record.
    /// </summary>
    public partial class aecar : Page
    {
        /// <summary>
        /// The get journey categories.
        /// </summary>
        /// <param name="uom">
        /// The Unit of measure.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public static object[] getJourneyCats(int uom, int accountid)
        {
            cMileagecats clsmileage = new cMileagecats(accountid);
            ArrayList objlst = new ArrayList();
            object[] items;

            items = clsmileage.CreateMileageCatsDropdown(0, uom);

            return items;
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The events.
        /// </param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            CurrentUser user;
            Title = "Add/Edit Car";
            Master.title = Title;
            Master.PageSubTitle = "Car Details";
            Master.helpid = 1032;
            var editCar = false;
            if (IsPostBack == false)
            {
                this.Master.enablenavigation = false;
                user = cMisc.GetCurrentUser();

                this.ViewState["accountid"] = user.AccountID;
                var clsEmployees = new cEmployees(user.AccountID);
                int employeeid = 0;
                if (Request.QueryString["action"] != null)
                {
                    this.aeCar.Action = aeCarPageAction.Edit;

                    this.aeCar.CarID = int.Parse(Request.QueryString["carid"]);
                    if (Request.QueryString["employeeid"] == null)
                    {
                        employeeid = clsEmployees.GetEmployeeIdFromCarId(this.aeCar.CarID);
                        editCar = true;
                    }
                    else
                    {
                        employeeid = int.Parse(Request.QueryString["employeeid"]);
                    }
                }
                else
                {
                    this.aeCar.Action = aeCarPageAction.Add;
                    employeeid = int.Parse(Request.QueryString["employeeid"]);
                }

                this.ViewState["employeeid"] = employeeid;

                this.aeCar.EmployeeID = employeeid;
                this.aeCar.AccountID = user.AccountID;
                if (Request.QueryString["returnUrl"] != null)
                {
                     var siteMapKey = Request.QueryString["returnUrl"];
                    var result = this.FindPageInSiteMap(siteMapKey, SiteMap.RootNode);

                    if (string.IsNullOrEmpty(result))
                    {
                        this.aeCar.ReturnURL = SiteMap.RootNode.Url;
                    }
                    else
                    {
                        this.aeCar.ReturnURL = result;
                    }
                }
                else
                {
                    this.aeCar.ReturnURL = "~/admin/aeemployee.aspx?action=2&employeeid=" + employeeid;
                }

                this.aeCar.EmployeeAdmin = true;

                if (editCar)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "LoadOnEdit", GetEditScript(this.aeCar.EmployeeID, this.aeCar.CarID), true);
                }
            }
        }

        private string FindPageInSiteMap(string siteMapKey, SiteMapNode startNode)
        {
            if (startNode.Title == siteMapKey)
            {
                return startNode.Url;
            }

            if (startNode.HasChildNodes)
            {
                foreach (SiteMapNode childNode in startNode.ChildNodes)
                {
                    var result = this.FindPageInSiteMap(siteMapKey, childNode);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the get edit script.
        /// </summary>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <param name="carId">
        /// The car Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetEditScript(int employeeId, int carId)
        {
            return string.Format("var editOnly = true;var nEmployeeid = {0};var isAdmin = true;editCar({1});", employeeId, carId);
        }
    }
}