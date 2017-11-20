namespace Spend_Management
{
    using System;
   

    /// <summary>
    /// Journey Details user control
    /// </summary>
    public partial class journey_details : System.Web.UI.UserControl
    {
        /// <summary>
        /// The current users account id, must be set before the control is added to anything
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The current users company name, must be set before the control is added to anything
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// The current users account MapsEnabled value, must be set before the control is added to anything
        /// </summary>
        public bool MapsEnabled { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccountId == null || CompanyId == null || MapsEnabled == null)
            {
                throw new Exception("Unable to load the control, AccountId, CompanyName and MapsEnabled must be set before the control is added.");
            }
            else if (MapsEnabled)
            {
                var mapControl = (Map)LoadControl("~/shared/usercontrols/Map.ascx");
                mapControl.AccountId = AccountId;
                mapControl.CompanyId = CompanyId;
                mapContainer.Controls.Add(mapControl);
            }
        }
    }
}