namespace Spend_Management
{
    using System;

    /// <summary>
    /// Mileage Map User Control
    /// </summary>
    public partial class Map : System.Web.UI.UserControl
    {
        public string height;

        /// <summary>
        /// The script tag URL for Google maps for business
        /// </summary>
        public string GoogleMapsScriptUrl { get; set; }

        /// <summary>
        /// The current users Account ID, must be set before adding this control to anything
        /// </summary>
        public int AccountId { get; set; }
        
        /// <summary>
        /// The current users company name, must be set before adding this control to anything
        /// </summary>
        public string CompanyId {get; set; }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccountId == null || CompanyId == null)
            {
                throw new Exception("Unable to load the control, AccountId and CompanyName must be set before the control is added.");
            }
            else
            {
                GoogleMapsScriptUrl = String.Format("https://maps.googleapis.com/maps/api/js?client=gme-softwareeuropeltd&sensor=false&callback=SEL.GoogleMaps.Initialise&channel={0}-{1}",
                                                    CompanyId,
                                                    AccountId.ToString());

                if (Request != null && Request.Browser != null && Request.Browser.Type != null && Request.Browser.Type == "IE6")
                {
                    height = "700px";
                }
                else
                {
                    height = "96%";
                }
            }
        }
    }
}