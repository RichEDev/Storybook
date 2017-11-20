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
using SpendManagementLibrary;
using System.Text;


namespace Spend_Management
{
    /// <summary>
    /// Summary description for aereason.
    /// </summary>
    public partial class aereason : Page
    {
        protected System.Web.UI.WebControls.ImageButton cmdhelp;
        int reasonid;
        /// <summary>
        /// reasonid
        /// </summary>
        public int ReasonID
        {
            get { return reasonid; }
            set { reasonid = value; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {

            Master.helpid = 1018;

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true, true);

                if (Request.QueryString["reasonid"] != null)
                {
                    int.TryParse(Request.QueryString["reasonid"], out reasonid);
                }
                ViewState["reasonid"] = reasonid;

                if (reasonid > 0)
                {
                    cReason reqreason;
                    cReasons clsreasons = new cReasons(user.AccountID);

                    reqreason = clsreasons.getReasonById(reasonid);
                    txtreason.Text = reqreason.reason;
                    txtdescription.Text = reqreason.description;
                    txtaccountcodevat.Text = reqreason.accountcodevat;
                    txtaccountcodenovat.Text = reqreason.accountcodenovat;
                    Master.title = "Reason: " + reqreason.reason;
                }
                else
                {
                    Master.title = "Reason: New";
                }
                Master.PageSubTitle = "Reason Details";
            }
        }
    }
}
