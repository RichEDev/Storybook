namespace Spend_Management
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using shared.webServices;

    using SpendManagementLibrary;

    /// <summary>
    /// Single Sign-on admin page
    /// </summary>
    public partial class SsoAdmin : System.Web.UI.Page
    {
        private string _moduleName;

        protected string ModuleName
        {
            get
            {
                if (this._moduleName == null)
                {
                    this._moduleName = new cModules().GetModuleByID((int)cMisc.GetCurrentUser().CurrentActiveModule).BrandNamePlainText;
                }
                return this._moduleName;
            }
        }

        protected string RedirectUrl
        {
            get
            {
                return SiteMap.CurrentNode == null ? "~/shared/menu.aspx?area=systemoptions" : SiteMap.CurrentNode.ParentNode.Url;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE *out* of compatibility mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");

            CurrentUser user = cMisc.GetCurrentUser();

            this.ValidatePermissions(user);

            if (this.IsPostBack && Request["__EVENTTARGET"] == "btnSave")
            {
                this.Save();
            }
            else
            {
                if (SiteMap.CurrentNode != null)
                {
                    Page.Title = SiteMap.CurrentNode.Title;
                    Master.title = SiteMap.CurrentNode.Title;
                }

                this.Populate(user);
            }
        }

        private void ValidatePermissions(ICurrentUser user)
        {
            if (user == null || user.Account == null || !user.Account.HasLicensedElement(SpendManagementElement.SingleSignOn))
            {
                Response.Redirect("~/shared/restricted.aspx");
            }

            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, true, true);
        }

        private void Save()
        {
            Response.Clear();
            Response.ContentType = "text/plain";

            svcSso.Response response;
            try
            {
                byte[] cer = null;
                if (this.Request.Files.Count > 0)
                {
                    using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                    {
                        cer = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                    }
                }

                response = new svcSso().Save(new SingleSignOn
                {
                    IssuerUri = this.Request[this.GetClientName(this.txtIssuerUri)],
                    PublicCertificate = cer,
                    CompanyIdAttribute = this.Request[this.GetClientName(this.txtCompanyIdAttribute)],
                    IdAttribute = this.Request[this.GetClientName(this.txtIdentifierAttribute)],
                    IdLookupFieldId = Guid.Parse(this.Request[this.GetClientName(this.ddlIdentifierLookupField)]),
                    LoginErrorUrl = this.Request[this.GetClientName(this.txtLoginErrorUrl)],
                    TimeoutUrl = this.Request[this.GetClientName(this.txtTimeoutUrl)],
                    ExitUrl = this.Request[this.GetClientName(this.txtExitUrl)]
                });
            }
            catch (Exception ex)
            {
                response = new svcSso.Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }

            Response.Write(new JavaScriptSerializer().Serialize(response));
            Response.End();

        }

        private string GetClientName(Control ctrl)
        {
            return ctrl.ClientID.Replace("_", "$");
        }

        private void Populate(ICurrentUser user)
        {
            this.spnCompanyId.InnerText = user.Account.companyid;

            if (!user.Account.IsNHSCustomer)
            {
                var liEsrAssNo = this.ddlIdentifierLookupField.Items.FindByValue("C23858B8-7730-440E-B481-C43FE8A1DBEF");
                if (liEsrAssNo != null)
                {
                    this.ddlIdentifierLookupField.Items.Remove(liEsrAssNo);
                }
            }

            var sso = SingleSignOn.Get(user);

            if (sso != null)
            {

                this.txtIssuerUri.Text = sso.IssuerUri;
                this.txtCompanyIdAttribute.Text = sso.CompanyIdAttribute;
                this.txtIdentifierAttribute.Text = sso.IdAttribute;
                this.txtLoginErrorUrl.Text = sso.LoginErrorUrl;
                this.txtTimeoutUrl.Text = sso.TimeoutUrl;
                this.txtExitUrl.Text = sso.ExitUrl;

                // Identifier lookup field
                var idLookupFieldListItem = this.ddlIdentifierLookupField.Items.FindByValue(sso.IdLookupFieldId.ToString().ToUpper());
                if (idLookupFieldListItem != null)
                {
                    this.ddlIdentifierLookupField.SelectedValue = idLookupFieldListItem.Value;
                }

                // Identity provider public certificate
                if (sso.PublicCertificate != null && sso.PublicCertificate.Length > 0)
                {
                    try
                    {
                        var cerIpPublic = new X509Certificate2();
                        cerIpPublic.Import(sso.PublicCertificate);
                        this.spnIpCertificateInfo.InnerHtml = String.Format("{0}<br />Valid: {1:d} - {2:d}", cerIpPublic.Subject.Replace(",", ",<br />"), cerIpPublic.NotBefore, cerIpPublic.NotAfter);
                    }
                    catch
                    {
                        this.spnIpCertificateInfo.InnerText = "An invalid certificate has been saved";
                        this.spnIpCertificateInfo.Attributes.Add("class", "invalid");
                    }
                }
            }
        }

        protected void btnSpDownloadCertificate_OnClick(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/x-x509-ca-cert";
            Response.AppendHeader("Content-Disposition", "attachment; filename=software-(europe)-limited.cer");
            Response.WriteFile(GlobalVariables.StaticContentFolderPath + GlobalVariables.GetAppSetting("SelPublicCertificateSCPath"));
            Response.End();
        }
    }
}
