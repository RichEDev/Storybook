using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;
using Moq;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Employees;

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
            
            var mockAccount = new cAccount() { ReceiptServiceEnabled = true, ValidationServiceEnabled = true};

            currentUser.SetupGet(x => x.Account).Returns(mockAccount);
            currentUser.SetupGet(x => x.AccountID).Returns(GlobalTestVariables.AccountId);
            currentUser.SetupGet(x => x.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountId);
            currentUser.SetupGet(x => x.CurrentActiveModule).Returns(GlobalTestVariables.ActiveModule);

            return currentUser;
        }

        public static Mock<ICurrentUser> CurrentUserDelegateMock()
        {
            var delegateUser = new Mock<Employee>();
            delegateUser.SetupAllProperties();
            delegateUser.Object.EmployeeID= GlobalTestVariables.DelegateId;

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            currentUser.SetupGet(x => x.AccountID).Returns(GlobalTestVariables.AccountId);
            currentUser.SetupGet(x => x.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountId);
            currentUser.SetupGet(x => x.CurrentActiveModule).Returns(GlobalTestVariables.ActiveModule);
            currentUser.SetupGet(x => x.isDelegate).Returns(true);
            currentUser.SetupGet(x => x.Delegate).Returns(delegateUser.Object);

            return currentUser;
        }

        public static Mock<ICurrentUser> CurrentUserMobileDevicesMock()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            currentUser.SetupGet(x => x.AccountID).Returns(GlobalTestVariables.AccountId);
            currentUser.SetupGet(x => x.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
            currentUser.SetupGet(x => x.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountId);
            currentUser.SetupGet(x => x.CurrentActiveModule).Returns(GlobalTestVariables.ActiveModule);
            currentUser.Setup(x => x.CheckAccessRole(AccessRoleType.View, SpendManagementElement.MobileDevices, false)).Returns(true);

            return currentUser;
        }
    }
}
