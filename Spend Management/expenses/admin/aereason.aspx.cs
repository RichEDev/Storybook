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
    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Reasons;

    /// <summary>
    /// Summary description for aereason.
    /// </summary>
    public partial class aereason : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IReason"/>
        /// </summary>
        [Dependency]
        public IDataFactoryArchivable<IReason, int, int> ReasonFactory { get; set; }

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

            this.Master.helpid = 1018;

            if (this.IsPostBack == false)
            {
                this.Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true, true);

                if (this.Request.QueryString["reasonid"] != null)
                {
                    int.TryParse(this.Request.QueryString["reasonid"], out this.reasonid);
                }
                this.ViewState["reasonid"] = this.reasonid;

                if (this.reasonid > 0)
                {
                    var reason = this.ReasonFactory[this.reasonid];

                    if (reason == null)
                    {
                        this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    this.txtreason.Text = reason?.Name;
                    this.txtdescription.Text = reason?.Description;
                    this.txtaccountcodevat.Text = reason?.AccountCodeVat;
                    this.txtaccountcodenovat.Text = reason?.AccountCodeNoVat;
                    this.Master.title = "Reason: " + reason?.Name;
                }
                else
                {
                    this.Master.title = "Reason: New";
                }
                this.Master.PageSubTitle = "Reason Details";
            }
        }
    }
}
