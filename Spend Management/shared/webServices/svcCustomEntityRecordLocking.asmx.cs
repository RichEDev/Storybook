namespace Spend_Management.shared.webServices
{
    using System.Web.Services;
    using Spend_Management.shared.code.Helpers;

    /// <summary>
    /// Webservice access to the Custom Entity Locking methods.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
     [System.Web.Script.Services.ScriptService]
    public class svcCustomEntityRecordLocking : System.Web.Services.WebService
    {

        /// <summary>
        /// Returns true if the element + id is locked by a user.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool IsRecordLocked(int customEntityId, int id)
        {
            var result = CustomEntityRecordLocking.IsRecordLocked(customEntityId, id, cMisc.GetCurrentUser());
            return result.EmployeeId != 0;
        }

        /// <summary>
        /// Update current lock datatime to prevent another user stealing it.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="id"></param>
        [WebMethod(EnableSession = true)]
        public void KeepRecordLocked(int customEntityId, int id )
        {
            CustomEntityRecordLocking.UpdateLocking(customEntityId, id, cMisc.GetCurrentUser());
        }

        /// <summary>
        /// Unlock element of element id and id.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="id"></param>
        [WebMethod(EnableSession = true)]
        public void UnlockRecord(int customEntityId, int id)
        {
            CustomEntityRecordLocking.UnlockElement(customEntityId, id, cMisc.GetCurrentUser());
        }
    }
}
