namespace SmartDThemeTemplate    
{
    using System;

    using SpendManagementLibrary.Interfaces;

    public partial class SmartDLogon : System.Web.UI.MasterPage, IMasterPage
    {

        /// <summary>
        /// The _start processing time for the page.
        /// </summary>
        private DateTime _startProcessing;

        /// <summary>
        /// The _end processing time for the page.
        /// </summary>
        private DateTime _endProcessing;

        /// <summary>
        /// The page pre innit event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Init(object sender, EventArgs e)
        {
            this._startProcessing = DateTime.Now;
        }

        /// <summary>
        /// The page_ pre render event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this._endProcessing = DateTime.Now;
            this.PageStatistics.Text = string.Format("<!--***Page Processing Time***: Start time: {0}, End time: {1}, {2} Seconds in total -->", this._startProcessing.TimeOfDay, this._endProcessing.TimeOfDay, this._endProcessing.Subtract(this._startProcessing).TotalSeconds);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string title { get; set; }

        public string PageSubTitle { get; set; }

        public int helpid { get; set; }

        public bool showdummymenu { get; set; }

        public string onloadfunc { get; set; }

        public bool home { get; set; }

        public bool stylesheet { get; set; }

        public bool enablenavigation { get; set; }

        public string menutitle { get; set; }

        public bool UseDynamicCSS { get; set; }
    }

 
}
