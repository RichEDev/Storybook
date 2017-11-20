using System;
using System.Configuration;

using SpendManagementLibrary;

/// <summary>
/// The register_success.
/// </summary>
public partial class register_success : System.Web.UI.Page
{
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
        this.Title = "Register";
        this.Master.Title = this.Title;

        var modules = new cModules();

        Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
        cModule module = modules.GetModuleByID((int)activeModule);

        this.litMsgBrand.Text = (module != null) ? module.BrandNameHTML : "Expenses";
    }
}
