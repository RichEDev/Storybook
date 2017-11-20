using Moq;
using SpendManagementLibrary;
using System.Web.SessionState;
using Spend_Management;

namespace SpendManagementUnitTests
{
    internal class Moqs
    {
        internal ICurrentUser CurrentUser()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            currentUser.SetupGet(x => x.AccountID).Returns(cGlobalVariables.AccountID);
            currentUser.SetupGet(x => x.EmployeeID).Returns(cGlobalVariables.EmployeeID);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(cGlobalVariables.SubAccountID);

            return currentUser.Object;
        }
    }
}
