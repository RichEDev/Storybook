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
using AjaxControlToolkit;

namespace Spend_Management
{
    public partial class auto_complete : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                //TextBox txtBox = new TextBox();
                //txtBox.ID = "txtAutoComplete";
                //txtBox.Style.Add("visibility", "hidden");

                //pnlAutoCompleteControls.Controls.Add(txtBox);

                //Panel pnl = new Panel();
                //pnl.ID = "pnlAutoComplete";
                //pnlAutoCompleteControls.Controls.Add(pnl);

                //HyperLink hyperLnk = new HyperLink();
                //hyperLnk.ID = "lnkAutoComplete";
                //hyperLnk.NavigateUrl = "javascript:void(0);";
                //hyperLnk.Text = "&nbsp;";
                //hyperLnk.Style.Add("visibility", "hidden");
                //pnlAutoCompleteControls.Controls.Add(hyperLnk);

                //PopupControlExtender popupControl = new PopupControlExtender();
                //popupControl.ID = "pceAutoComplete";
                //popupControl.TargetControlID = hyperLnk.ClientID;
                //popupControl.PopupControlID = pnl.ClientID;
                //popupControl.OffsetY = 20;
                //pnlAutoCompleteControls.Controls.Add(popupControl);
        }
    }
}