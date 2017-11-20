using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Spend_Management.shared.webServices
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;

    /// <summary>
    /// Summary description for svcOrganisations
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcOrganisations : System.Web.Services.WebService
    {
        /// <summary>
        /// Get an organisation
        /// </summary>
        /// <param name="identifier">The organisation to get</param>
        /// <returns>Organisation object or a dummy containing a status code</returns>
        [WebMethod(EnableSession = true)]
        public Organisation Get(int identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, true, false) == false)
            {
                return new Organisation { Identifier = -999 };
            }

            return Organisation.Get(currentUser.AccountID, identifier);
        }

        /// <summary>
        /// Save an organisation
        /// </summary>
        /// <param name="identifier">The organisation to get</param>
        /// <param name="name">The organisation name</param>
        /// <param name="code">The organisation code, an accounting code or other client identifier</param>
        /// <param name="comment">Any comments about the organisation</param>
        /// <param name="parentOrganisationIdentifier">The parent organisation in the heirarchy</param>
        /// <param name="primaryAddressIdentifier">The main address for the organisation</param>
        /// <returns>Organisation object or a dummy containing a status code</returns>
        [WebMethod(EnableSession = true)]
        public int Save(int identifier, string name, string code, string comment, int parentOrganisationIdentifier, int primaryAddressIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, true, false) == false)
            {
                return -999;
            }

            return Organisation.Save(currentUser, identifier, (parentOrganisationIdentifier <= 0 ? (int?)null : parentOrganisationIdentifier), (primaryAddressIdentifier <= 0 ? (int?)null : primaryAddressIdentifier), name, comment, code);
        }

        /// <summary>
        /// Save an organisation
        /// </summary>
        /// <param name="name">The organisation name</param>
        /// <returns>Organisation object or a dummy containing a status code</returns>
        [WebMethod(EnableSession = true)]
        public int SaveForClaimant(string name)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cAccountProperties properties = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            if (properties.ClaimantsCanAddCompanyLocations)
            {
                return Organisation.Save(currentUser, 0, null, null, name, string.Empty, string.Empty);
            }

            return -1;
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        /// <param name="identifier">The organisation to delete</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true)]
        public int Delete(int identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Organisations, true, false) == false)
            {
                return -999;
            }

            return Organisation.Delete(currentUser, identifier);
        }

        /// <summary>
        /// Archive or unarchive an organisation
        /// </summary>
        /// <param name="identifier">The organisation to toggle archive on</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true)]
        public int ToggleArchive(int identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Organisations, true, false) == false)
            {
                return -999;
            }

            return Organisation.ToggleArchive(currentUser, identifier);
        }
    }
}
