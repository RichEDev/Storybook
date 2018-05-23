#region Using Directives

using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BusinessLogic.Modules;

using Spend_Management;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Utilities.StringManipulation;

#endregion

/// <summary>
/// The admin_aebroadcastmessage.
/// </summary>
public partial class admin_aebroadcastmessage : Page
{
    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack == false)
        {
            this.Title = "Add/Edit Broadcast Message";
            this.Master.title = this.Title;
            this.Master.enablenavigation = false;
            this.Master.helpid = 1026;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BroadcastMessages, true, true);
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;
            
            //compare end date with today
            cmpminend.ValueToCompare = DateTime.Now.ToShortDateString();

            int broadcastid = 0;

            if (this.Request.QueryString["broadcastid"] != null)
            {
                broadcastid = int.Parse(this.Request.QueryString["broadcastid"]);
            }

            this.ViewState["broadcastid"] = broadcastid;

            if (user.CurrentActiveModule == Modules.Greenlight || user.CurrentActiveModule == Modules.GreenlightWorkforce)
            {
                ListItem item = this.cmblocation.Items.FindByText("Submit Claim");
                if (item != null)
                {
                    this.cmblocation.Items.Remove(item);
                }
            }

            if (broadcastid != 0)
            {
                var clsmessages = new cBroadcastMessages(user.AccountID);
                cBroadcastMessage reqmsg = clsmessages.getBroadcastMessageById(broadcastid);
                this.txttitle.Text = reqmsg.title;
                if (reqmsg.startdate.ToShortDateString() != "01/01/1900")
                {
                    this.txtstartdate.Text = reqmsg.startdate.ToShortDateString();
                }

                if (reqmsg.enddate.ToShortDateString() != "01/01/1900")
                {
                    this.txtenddate.Text = reqmsg.enddate.ToShortDateString();
                }

                if (this.cmblocation.Items.FindByValue(Convert.ToByte(reqmsg.location).ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.cmblocation.Items.FindByValue(Convert.ToByte(reqmsg.location).ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                this.chkexpirewhenread.Checked = reqmsg.expirewhenread;
                this.chkoncepersession.Checked = reqmsg.oncepersession;
                this.txtmessage.Text = reqmsg.message; 
            }

            this.cmdok.Attributes.Add(
                "onclick", "if(validateform('vgBroadcast') == false) { return false; } else { $('#ctl00_contentmain_hiddenHTMLTxt').val(encodeURIComponent($('#ctl00_contentmain_HtmlEditorExtender1_ExtenderContentEditable').html())); $('#ctl00_contentmain_HtmlEditorExtender1_ExtenderContentEditable').html(''); }");
        }
    }

    /// <summary>
    /// The cmdok_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        var clsmessages = new cBroadcastMessages((int)this.ViewState["accountid"]);
        string title = this.txttitle.Text;

        var tmpValue = HttpUtility.UrlDecode(this.hiddenHTMLTxt.Value, Encoding.Default);
        HtmlUtility util = HtmlUtility.Instance;
        string htmlTagWhitelistTemplatePath =
            ConfigurationManager.AppSettings["PermittedHtmlTagsPath"];
        tmpValue = util.SanitizeHtml(tmpValue, htmlTagWhitelistTemplatePath);
        StringManipulators.ReplaceAmpersandInHtmlString(ref tmpValue);
        byte[] data = Encoding.Default.GetBytes(tmpValue);
        string message = Encoding.UTF8.GetString(data);
        
        bool expirewhenread = this.chkexpirewhenread.Checked;
        bool oncepersession = this.chkoncepersession.Checked;

        DateTime startdate = this.txtstartdate.Text == string.Empty ? new DateTime(1900, 01, 01) : DateTime.Parse(this.txtstartdate.Text);
        DateTime enddate = this.txtenddate.Text == string.Empty ? new DateTime(1900, 01, 01) : DateTime.Parse(this.txtenddate.Text);

        var location = (broadcastLocation)Convert.ToByte(this.cmblocation.SelectedValue);

        if ((int)this.ViewState["broadcastid"] != 0)
        {
            clsmessages.updateBroadcastMessage(
                (int)this.ViewState["broadcastid"],
                title,
                message,
                startdate,
                enddate,
                expirewhenread,
                location,
                oncepersession,
                DateTime.Now,
                (int)this.ViewState["employeeid"]);
        }
        else
        {
            clsmessages.addBroadcastMessage(
                title,
                message,
                startdate,
                enddate,
                expirewhenread,
                location,
                oncepersession,
                DateTime.Now,
                (int)this.ViewState["employeeid"]);
        }

        this.Response.Redirect("broadcastmessages.aspx", true);
    }

    #endregion
}