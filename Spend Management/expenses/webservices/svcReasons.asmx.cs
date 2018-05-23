namespace Spend_Management
{
    using System;
    using System.Web.Script.Services;
    using System.Web.Services;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Reasons;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for svcReasons
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcReasons : System.Web.Services.WebService
    {
        private readonly IDataFactoryArchivable<IReason, int, int> _reasonsFactory = FunkyInjector.Container.GetInstance<IDataFactoryArchivable<IReason, int, int>>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReasonID"></param>
        /// <param name="ReasonName"></param>
        /// <param name="Description"></param>
        /// <param name="CodeWithVAT"></param>
        /// <param name="CodeWithoutVAT"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveReason(int ReasonID, string ReasonName, string Description, string CodeWithVAT, string CodeWithoutVAT, bool Archived)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int returncode;

            BusinessLogic.Reasons.Reason reason;

            if (ReasonID > 0) //update
            {
                var oldReason = this._reasonsFactory[ReasonID];
                reason = new BusinessLogic.Reasons.Reason(ReasonID, Archived, Description, ReasonName, CodeWithVAT, CodeWithoutVAT, oldReason.CreatedBy, oldReason.CreatedOn, currentUser.EmployeeID, DateTime.Now);
            }
            else
            {
                reason = new BusinessLogic.Reasons.Reason(ReasonID, Archived, Description, ReasonName, CodeWithVAT, CodeWithoutVAT, currentUser.EmployeeID, DateTime.Now, null, null);
            }

            returncode = this._reasonsFactory.Save(reason).Id;

            return returncode;
        }
    }
}
