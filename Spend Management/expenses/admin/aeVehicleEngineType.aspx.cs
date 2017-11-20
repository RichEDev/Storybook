namespace Spend_Management
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for aeVehicleEngineType.
    /// </summary>
    public partial class aeVehicleEngineType : Page
    {
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

        }
        #endregion

        protected string RedirectUrl
        {
            get
            {
                return SiteMap.CurrentNode == null ? "adminVehicleEngineTypes.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            this.Master.enablenavigation = false;

            var user = cMisc.GetCurrentUser();
            var vehicleEngineTypeId = this.GetVehicleEngineTypeId();

            this.ValidatePermissions(user, vehicleEngineTypeId);

            this.Populate(user, vehicleEngineTypeId);

        }

        private int? GetVehicleEngineTypeId()
        {
            int vehicleEngineTypeId;

            if (int.TryParse(Request["vehicleEngineTypeId"], out vehicleEngineTypeId))
            {
                return vehicleEngineTypeId;
            }

            return null;
        }

        private void ValidatePermissions(ICurrentUser user, int? vehicleEngineTypeId)
        {
            if (
                user == null || user.Account == null ||
                vehicleEngineTypeId == null && !user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.VehicleEngineType, true) ||
                vehicleEngineTypeId != null && !user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VehicleEngineType, true))
            {
                Response.Redirect("~/shared/restricted.aspx");
            }
        }

        private void Populate(ICurrentUser user, int? vehicleEngineTypeId)
        {
            if (vehicleEngineTypeId != null)
            {
                var vet = VehicleEngineType.Get(user, (int)vehicleEngineTypeId);
                if (vet != null)
                {
                    this.Title = "Vehicle Engine Type: " + vet.Name;
                    this.txtName.Text = vet.Name;
                    this.txtCode.Text = vet.Code;
                    this.litVehicleEngineTypeId.Text = vet.VehicleEngineTypeId.ToString();

                    this.txtCode.Enabled = !user.Account.IsNHSCustomer || !VehicleEngineType.EsrReservedCodes.Contains(vet.Code, StringComparer.InvariantCultureIgnoreCase);

                }
                else
                {
                    Response.Redirect("~/PublicPages/error.aspx?missing");
                }
            }
            else
            {
                this.Title = "New Vehicle Engine Type";
                this.litVehicleEngineTypeId.Text = "null";
            }

            Master.title = this.Title;
            Master.PageSubTitle = "Vehicle Engine Type Details";

        }

    }
}
