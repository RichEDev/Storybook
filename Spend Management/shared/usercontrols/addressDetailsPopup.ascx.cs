using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    /// <summary>
    /// Usercontrol containing a PopupControlExtender to display an addresses details
    /// </summary>
    public partial class addressDetailsPopup : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Adds an onclick, AlternateText, ToolTip, ImageUrl and css cursor: pointer onto the webcontrol
        /// </summary>
        /// <param name="image"></param>
        /// <param name="hdn"></param>
        /// <param name="prefixTag"></param>
        public static void AddEvents(System.Web.UI.WebControls.WebControl image, System.Web.UI.WebControls.HiddenField hdn, string prefixTag, bool overRideImageTags)
        {
            if (image.GetType() == typeof(Image) && overRideImageTags == true)
            {
                ((Image)image).AlternateText = "View address details";
                ((Image)image).ToolTip = "Address Details";
                ((Image)image).ImageUrl = "~/shared/images/icons/16/environment_view.png";
            }

            if (image.Attributes["onmouseover"] != null)
            {
                image.Attributes.Add("onmouseover", "addressDetailsPopup.displayAddress('" + prefixTag + image.ClientID + "', '" + prefixTag + hdn.ClientID + "', event);" + image.Attributes["onmouseover"]);
            }
            else
            {
                image.Attributes.Add("onmouseover", "addressDetailsPopup.displayAddress('" + prefixTag + image.ClientID + "', '" + prefixTag + hdn.ClientID + "', event);");
            }

            if (image.Attributes["onmouseout"] != null)
            {
                image.Attributes.Add("onmouseout", "addressDetailsPopup.hide();" + image.Attributes["onmouseout"]);
            }
            else
            {
                image.Attributes.Add("onmouseout", "addressDetailsPopup.hide();");
            }
            image.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
        }
    }
}