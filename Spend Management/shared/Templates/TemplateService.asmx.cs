namespace Spend_Management.shared.webServices
{    
    using System.Web.Script.Services;
    using System.Web.Services;
    using Templates;

    /// <summary>
    /// The template Web service class.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class TemplateService : WebService
    {
        /// <summary>
        /// A template version of a "save" web method.
        /// </summary>
        /// <param name="template">The object to save.</param>
        /// <returns>The ID of the object being saved. Returns -1 if an error occurs.</returns>
        [WebMethod(EnableSession = true)]
        public int SaveTemplateItem(Template template)
        {
            // Calling GetCurrentUser ensures we are authenticated
            var currentUser = cMisc.GetCurrentUser();

            var templates = new Templates(currentUser);

            return templates.Save(template);
        }


        /// <summary>
        /// A template version of a "get" web method.
        /// </summary>
        /// <param name="templateItemId">The ID of the object to get</param>
        /// <returns>The desired object</returns>
        [WebMethod(EnableSession = true)]
        public Template GetTemplateItemById(int templateItemId)
        {
            // Calling GetCurrentUser ensures we are authenticated
            var currentUser = cMisc.GetCurrentUser();

            var templates = new Templates(currentUser);

            return templates.GetById(templateItemId);
        }
    }
}
