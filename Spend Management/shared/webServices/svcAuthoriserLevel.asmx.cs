namespace Spend_Management
{
    using System.Web.Script.Services;
    using System.Web.Services;
    using expenses.code;
    using Spend_Management.shared.code;
    /// <summary>
    /// Summary description for svcAuthoriserLevel
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [ScriptService]
    public class svcAuthoriserLevel : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        /// <summary>
        /// Save authoriser level in database
        /// </summary>
        public int SaveAuthoriserLevel(int authoriserLevelId,decimal amount, string decription)
        {
            var authoriserLevelDetail = new AuthoriserLevelDetail();
            return authoriserLevelDetail.SaveAuthoriserLevel(authoriserLevelId, amount, decription);
        }
        

        /// <summary>
        /// Delete authoriser level by provided id.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteAuthoriserLevelDetail(int authoriserLevelDetailId, bool IsAssignAuthoriserLevel)
        {
            var authoriserLevelDetail = new AuthoriserLevelDetail();
            return authoriserLevelDetail.DeleteAuthoriserLevelDetail(authoriserLevelDetailId, IsAssignAuthoriserLevel);
        }

        /// <summary>
        /// Set employee by id as default authoriser.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int UpdateEmployeeForDefaultAuthoriser(int employeeId)
        {
            var authoriserLevelDetail = new AuthoriserLevelDetail();
            return authoriserLevelDetail.UpdateEmployeeForDefaultAuthoriser(employeeId);
        }

        /// <summary>
        /// Check if any authoriser level assigned to employee(by id)
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool CheckAuthoriserLevelAssignToEmployee(int authoriserLevelDetailId)
        {
            var authoriserLevelDetail = new AuthoriserLevelDetail();
            return authoriserLevelDetail.CheckIfAuthoriserLevelAssignedToEmployees(authoriserLevelDetailId);
        }
    }
}
