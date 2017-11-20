namespace Spend_Management.shared.webServices
{
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;

    using Spend_Management.shared.code.ApprovalMatrix;

    /// <summary>
    /// The approval matrix Web service.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class ApprovalMatrixSvc : WebService
    {
        /// <summary>
        /// The save matrix method.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix Id.
        /// </param>
        /// <param name="matrixName">
        /// The matrix Name.
        /// </param>
        /// <param name="matrixDescription">
        /// The matrix Description.
        /// </param>
        /// <param name="defaultApproverKey"></param>
        /// <returns>
        /// The <see cref="int"/>.
        /// The created or updated matrix id, negative if failed.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int SaveMatrix(int matrixId, string matrixName, string matrixDescription, string defaultApproverKey)
        {
            var user = cMisc.GetCurrentUser();
            if ((matrixId == 0 && user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ApprovalMatrix, true)) || (matrixId > 0 && user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true)))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                var newApprovalMatrix = new ApprovalMatrix(matrixId, matrixName, matrixDescription, defaultApproverKey, null);
                return approvalMatrices.Save(newApprovalMatrix);
            }

            return -999;
        }

        /// <summary>
        /// The get level grid.
        /// </summary>
        /// <param name="entityid">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>string[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        //[WebMethod(EnableSession = true)]
        //public string[] GetLevelGrid(int entityid)
        //{
        //    var user = cMisc.GetCurrentUser();
        //    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
        //    {
        //        var approvalMatrices = new ApprovalMatrices(user.AccountID);
        //        return approvalMatrices.GetLevelGrid(entityid);
        //    }

        //    return new string[0];
        //}

        /// <summary>
        /// The save level.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="matrixLevelId">
        /// The matrix level id.
        /// </param>
        /// <param name="approvalLimit">
        /// The approval limit.
        /// </param>
        /// <param name="approverKey">
        /// The approver key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int SaveLevel(int matrixId, int matrixLevelId, decimal approvalLimit, string approverKey)
        {
            var user = cMisc.GetCurrentUser();
            if ((matrixLevelId == 0 && user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ApprovalMatrix, true)) || (matrixLevelId > 0 && user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true)))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                var newApprovalMatrixLevel = new ApprovalMatrixLevel(matrixLevelId, matrixId, approvalLimit, approverKey);
                return approvalMatrices.SaveLevel(newApprovalMatrixLevel);
            }

            return -999;
        }

        /// <summary>
        /// The get matrix level grid.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix Id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>string[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        //[WebMethod(EnableSession = true)]
        //public string[] GetMatrixLevelGrid(int matrixId)
        //{
        //    var user = cMisc.GetCurrentUser();
        //    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
        //    {
        //        var approvalMatrices = new ApprovalMatrices(user.AccountID);
        //        return approvalMatrices.GetLevelGrid(matrixId);
        //    }

        //    return new string[0];
        //}

        /// <summary>
        /// Get a matrix level.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="matrixLevelId">
        /// The matrix level id.
        /// </param>
        /// <returns>
        /// The <see cref="ApprovalMatrixLevel"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public ApprovalMatrixLevel GetLevel(int matrixId, int matrixLevelId)
        {
            var user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                var matrix = approvalMatrices.GetById(matrixId);
                var result = matrix.ApprovalMatrixLevels.FirstOrDefault(matrixLevel => matrixLevel.ApprovalMatrixLevelId == matrixLevelId);
                if (result != null)
                {
                    result.ApproverFriendlyName = approvalMatrices.GetFriendlyNameOfApprover(result.ApproverKey);
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Get friendly name for approver.
        /// </summary>
        /// <param name="approver">
        /// The approver.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string GetFriendlyNameforApprover(string approver)
        {
            var user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                return approvalMatrices.GetFriendlyNameOfApprover(approver);
            }

            return string.Empty;
        }

        /// <summary>
        /// The delete matrix.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int DeleteMatrix(int matrixId)
        {
            var user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ApprovalMatrix, true))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                return approvalMatrices.Delete(matrixId);
            }

            return -999;
        }

        /// <summary>
        /// The delete level method.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="matrixLevelId">
        /// The matrix level id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int DeleteLevel(int matrixId, int matrixLevelId)
        {
            var user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ApprovalMatrix, true) || user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true))
            {
                var approvalMatrices = new ApprovalMatrices(user.AccountID);
                return approvalMatrices.DeleteLevel(matrixId, matrixLevelId);
            }

            return -999;
        }
    }
}
