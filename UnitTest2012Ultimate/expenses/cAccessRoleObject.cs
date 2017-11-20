using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace UnitTest2012Ultimate
{
    internal class cAccessRoleObject
    {
        public static cAccessRole New(cAccessRole accessRole)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cAccessRoles roles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));


            //roles.SaveAccessRole(currentUser.EmployeeID, accessRole.RoleID, accessRole.RoleName, accessRole.Description, accessRole.AccessLevel, accessRole.e)

            return null;
        }

        public static cAccessRole Template(Modules activeModule, bool ViewAccess = true, bool AddAccess = true, bool EditAccess = true, bool DeleteAccess = true)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cElements elements = new cElements();
            cModules modules = new cModules();
            modules.GetCategoryElements(currentUser.AccountID, (int)activeModule);

            return null;
        }

        public static void TearDown(int accessRoleId)
        {

        }
    }
}
