namespace Spend_Management.shared.admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using code;
    using code.Logon;
    using SpendManagementLibrary;
    using LogonBase = code;

    /// <summary>
    /// Add/Edit Logon Messages and their associated images and content.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class aeLogonMessages : Page
    {
        private readonly LogonBase.LogonMessages _logonMessages = new LogonBase.LogonMessages();
        private static string initialBackgroundImage;
        private static bool moduleActiveState;
        private static List<string> initialActiveModules;

        /// <summary>
        /// Gets or sets the list of modules which can't be activated
        /// </summary>
        public string ListOfModules { get; set; }

        /// <summary>
        /// Gets or sets the value of whether an icon is valid
        /// </summary>
        public bool ValidIconCheck { get; set; }

        /// <summary>
        /// Gets or sets the value of whether an background image is valid
        /// </summary>
        public bool ValidBackgroundCheck { get; set; }

        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        /// <summary>
        /// Page load function of aeLogonMessages
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LogonMessages, false, true);
            var availableModules = new AvailableModules(user.AccountID);
            this.moduleRepeater.DataSource = availableModules.GetAllModules(); 
            this.moduleRepeater.DataBind();

            if (this.IsPostBack)
            {
                return;
            }

            this.cmdSave.Attributes.Add("onclick", " if (validateform('vgAddEditLogonMessages') == false) { return; }");
            this.Master.enablenavigation = false;
            this.ValidBackgroundCheck = true;
            this.ValidIconCheck = true;
            int messageId;
            int.TryParse(this.Request.QueryString["messageid"], out messageId);

            if (messageId == 0)
            {
                initialBackgroundImage = null;
                moduleActiveState = true;
                this.Title = @"Add/Edit Marketing Information";
                this.Master.title = this.Title;
            }
            else
            {
                var logonMessage = this._logonMessages.GetActiveLogOnMessagesById(messageId);

                if (logonMessage == null)
                {
                    this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    return;
                }

                this.Title = @"Marketing Information: " + logonMessage.HeaderText;
                this.Master.title = this.Title;

                this.txtCategoryTitle.Text = logonMessage.CategoryTitle;
                this.txtCategoryColorCode.Text = logonMessage.CategoryTitleColourCode;
                this.txtBody.Text = logonMessage.BodyText.Replace("<br />", Environment.NewLine);
                this.txtBodyColorCode.Text = logonMessage.BodyTextColourCode;
                this.txtHeader.Text = logonMessage.HeaderText;
                this.txtHeaderColorCode.Text = logonMessage.HeaderTextColourCode;
                this.txtButtonText.Text = logonMessage.ButtonText;
                this.txtButtonTextColor.Text = logonMessage.ButtonForeColour;
                this.txtButtonBackGroundColor.Text = logonMessage.ButtonBackGroundColour;
                this.txtLink.Text = logonMessage.ButtonLink;
                initialBackgroundImage = logonMessage.BackgroundImage;
                this.BackgroundImageValidator.Enabled = false;
                var moduleList = logonMessage.MessageModules;
                this.referenceModuleList.Value = string.Join(",", moduleList.ToArray());
                moduleActiveState = logonMessage.Archived;
                initialActiveModules = new List<string>(this.referenceModuleList.Value.Split(','));
                this.hdInitialBackgroundImage.Value = logonMessage.BackgroundImage;
                this.logonBackgroundDetailsWrapper.InnerHtml = logonMessage.BackgroundImage.Length > 25 ? logonMessage.BackgroundImage.Substring(0, 10) + "..." + logonMessage.BackgroundImage.Substring((logonMessage.BackgroundImage.Length - 8), 8) : logonMessage.BackgroundImage;
                if (string.IsNullOrEmpty(logonMessage.Icon)) return;
                this.lblFileNameHolder.Value = logonMessage.Icon;
                this.fileNameHolder.InnerHtml= logonMessage.Icon.Length > 25? logonMessage.Icon.Substring(0, 10) + "..." + logonMessage.Icon.Substring((logonMessage.Icon.Length - 8), 8): logonMessage.Icon;
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdSave.Click += this.CmdOk_Click;
            this.cmdCancel.Click += this.cmdCancel_Click;
        }

        /// <summary>
        /// Cancel event of a button
        /// </summary>
        private void cmdCancel_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect("LogonMessages.aspx", true);
        }

        /// <summary>
        /// Save operations
        /// </summary>
        private void CmdOk_Click(object sender, ImageClickEventArgs e)
        {
            var currentUser = cMisc.GetCurrentUser();
            var messageId = 0;
            var logMessage = new LogonMessage();

            if (this.Request.QueryString["messageid"] != null)
            {
                int.TryParse(this.Request.QueryString["messageid"], out messageId);
                logMessage.MessageId = messageId;
            }
            logMessage.MessageId = messageId;
            logMessage.Archived = true;
            logMessage.CategoryTitle = this.txtCategoryTitle.Text;
            logMessage.CategoryTitleColourCode = this.txtCategoryColorCode.Text;
            logMessage.BodyText = this.txtBody.Text.Replace(Environment.NewLine, "<br />");  
            logMessage.BodyTextColourCode = this.txtBodyColorCode.Text;
            logMessage.HeaderText = this.txtHeader.Text;
            logMessage.HeaderTextColourCode = this.txtHeaderColorCode.Text;
            logMessage.ButtonText = this.txtButtonText.Text;
            logMessage.ButtonLink = this.txtLink.Text;
            logMessage.ButtonForeColour = this.txtButtonTextColor.Text;
            logMessage.ButtonBackGroundColour = this.txtButtonBackGroundColor.Text;
            logMessage.BackgroundImage = string.IsNullOrEmpty(this.backGroundImage.Value) ? initialBackgroundImage : this.backGroundImage.Value;
            logMessage.Icon = string.IsNullOrEmpty(this.iconForTitle.Value) ? this.lblFileNameHolder.Value : this.iconForTitle.Value;
            this.ValidIconCheck = true;
            this.ValidBackgroundCheck = true;
            var moduleList = new List<string>(this.referenceModuleList.Value.Split(','));
            if (moduleList.Count > 0 && !string.IsNullOrEmpty(moduleList[0]))
            {
                logMessage.MessageModules = moduleList.Select(int.Parse).ToList();
                if (initialActiveModules != null)
                {
                    moduleList.RemoveAll(x => initialActiveModules.Exists(y => y == x));
                }
                if (!moduleActiveState && moduleList.Count > 0 && !string.IsNullOrEmpty(moduleList[0]))
                {
                    var checkForCurrentActiveMessages = this._logonMessages.CheckCheckLogonMessagesCanArchivedOrDeleted();
                    checkForCurrentActiveMessages.AddRange(from item in moduleList select this.ProductModuleFactory[(Modules)int.Parse(item)] into currentModule where currentModule != null select currentModule.BrandName);
                    var modulesWhichCannotBeActivated = checkForCurrentActiveMessages.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);
                    IEnumerable<string> whichCannotBeActivated = modulesWhichCannotBeActivated as string[] ?? modulesWhichCannotBeActivated.ToArray();
                    if (whichCannotBeActivated.Any())
                    {
                        this.ListOfModules = string.Join(", ", whichCannotBeActivated.ToArray());
                        return;
                    }
                }
            }
            var clsDatabases = new Databases();
            var requiredDatabase = clsDatabases.GetDatabaseByID(currentUser.Account.dbserverid);
            if (this.iconForTitle?.Value.Length > 0)
            {
                this.ValidIconCheck = false;
                var stream = this.iconForTitle.PostedFile.InputStream;
                var image = System.Drawing.Image.FromStream(stream);
                int height = image.Height;
                int width = image.Width;
                if (height == 37 && width == 37)
                {
                    var uploadIconfile = System.IO.Path.Combine(requiredDatabase.LogoPath + "/MarketingInformation/icons/", this.iconForTitle.Value);
                    this.iconForTitle.PostedFile.SaveAs(uploadIconfile);
                    this.ValidIconCheck = true;
                }
            }
            if (this.backGroundImage?.Value.Length > 0)
            {
                this.ValidBackgroundCheck = false;
                var stream = this.backGroundImage.PostedFile.InputStream;
                var image = System.Drawing.Image.FromStream(stream);
                int height = image.Height;
                int width = image.Width;
                if (height == 1050 && width == 840)
                {
                    string uploadBgfile = System.IO.Path.Combine(requiredDatabase.LogoPath + "/MarketingInformation/",
                    this.backGroundImage.Value);
                    this.backGroundImage.PostedFile.SaveAs(uploadBgfile);
                    this.ValidBackgroundCheck = true;
                }
            }

            if (!this.ValidBackgroundCheck || !this.ValidIconCheck)
            {
                return;
            }
            
            this._logonMessages.AddOrUpdateLogonMessage(logMessage, currentUser.EmployeeID);
            this.Response.Redirect("LogonMessages.aspx", true);

        }

    }
}