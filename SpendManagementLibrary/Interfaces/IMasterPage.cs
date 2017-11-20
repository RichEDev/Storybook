namespace SpendManagementLibrary.Interfaces
{
    using System.Collections;

    public interface IMasterPage
    {
        /// <summary>
        /// Master Page title, when setting also sets the Page.Title
        /// </summary>
        string title { get; set; }

        /// <summary>
        /// Master Page subtitle
        /// </summary>
        string PageSubTitle { get; set; }

        /// <summary>
        /// Help ID associated with the page
        /// </summary>
        int helpid { get; set; }

        /// <summary>
        /// Show the menu ---
        /// </summary>
        bool showdummymenu { get; set; }

        /// <summary>
        /// JavaScript function to place in the body onload attribute
        /// </summary>
        string onloadfunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool home { get; set; }

        bool stylesheet { get; set; }

        bool enablenavigation { get; set; }

        string menutitle { get; set; }

        /// <summary>
        /// Outputs the styles.aspx and layout.css link elements if true
        /// </summary>
        bool UseDynamicCSS { get; set; }
    }
}