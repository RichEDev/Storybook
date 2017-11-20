using Spend_Management;
using SpendManagementLibrary;
using Moq;

namespace tempMobileUnitTests
{
    public class Moqs
    {
        public static ICurrentUser CurrentUser()
        {
            return Moqs.CurrentUserMock().Object;
        }

        public static Mock<ICurrentUser> CurrentUserMock()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            currentUser.SetupGet(x => x.AccountID).Returns(GlobalTestVariables.AccountID);
            currentUser.SetupGet(x => x.EmployeeID).Returns(GlobalTestVariables.EmployeeID);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountID);
            currentUser.SetupGet(x => x.CurrentActiveModule).Returns(GlobalTestVariables.ActiveModule);

            return currentUser;
        }

        public static Mock<ICurrentUser> CurrentUserDelegateMock()
        {
            var delegateUser = new Mock<cEmployee>();
            delegateUser.SetupAllProperties();
            delegateUser.Object.employeeid = GlobalTestVariables.DelegateID;

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            currentUser.SetupGet(x => x.AccountID).Returns(GlobalTestVariables.AccountID);
            currentUser.SetupGet(x => x.EmployeeID).Returns(GlobalTestVariables.EmployeeID);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountID);
            currentUser.SetupGet(x => x.CurrentActiveModule).Returns(GlobalTestVariables.ActiveModule);
            currentUser.SetupGet(x => x.isDelegate).Returns(true);
            currentUser.SetupGet(x => x.Delegate).Returns(delegateUser.Object);

            return currentUser;
        }
    }
}
