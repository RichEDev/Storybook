using System;
using System.Configuration;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using SpendManagementLibrary;

/// <summary>
/// The register_success.
/// </summary>
public partial class register_success : System.Web.UI.Page
{
    /// <summary>
    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

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

        Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
        var module = this.ProductModuleFactory[activeModule];

        this.litMsgBrand.Text = (module != null) ? module.BrandNameHtml : "Expenses";
    }
}
