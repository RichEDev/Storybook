namespace Spend_Management
{
    using System.Web.Services;
    using System.Web.Script.Services;

    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for svcColours
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [System.Web.Script.Services.ScriptService]
    public class svcColours : System.Web.Services.WebService
    {
        private readonly IDataFactory<IAccountProperty, AccountPropertyCacheKey> _accountPropertiesFactory = FunkyInjector.Container.GetInstance<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>();

        /// <summary>
        /// Restores the default colours
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void Restore()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cColours clscolours = new cColours(curUser.AccountID, curUser.CurrentSubAccountId, curUser.CurrentActiveModule);
            clscolours.RestoreDefaults(this._accountPropertiesFactory);
        }
    }
}
