namespace SpendManagementApi.Common
{
    using System.Collections.Generic;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ServiceResultMessage = SpendManagementLibrary.Mobile.ServiceResultMessage;

    /// <summary>
    /// The class handling authentication methods.
    /// </summary>
    public static class Authenticator
    {
        /// <summary>
        /// Authenticates whether Pairing Key / Serial Key combination is valid
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ServiceResultMessage indicating whether authentication valid or not</returns>
        public static ServiceResultMessage Authenticate(string pairingKey, string serialKey)
        {
            ServiceResultMessage validateCode = ValidatePairingKey(pairingKey, serialKey);
            ServiceResultMessage authResultMsg = new ServiceResultMessage { FunctionName = "Authenticate", ReturnCode = validateCode.ReturnCode };

            if (validateCode.ReturnCode == MobileReturnCode.Success)
            {
                PairingKey paired = new PairingKey(pairingKey);

                cMobileDevices clsdevices = new cMobileDevices(paired.AccountID);
                MobileReturnCode authenticated = clsdevices.authenticate(pairingKey, serialKey, paired.EmployeeID);
                if (authenticated != MobileReturnCode.Success)
                {
                    authResultMsg.ReturnCode = authenticated;
                }
            }

            return authResultMsg;
        }

        /// <summary>
        /// Validates if the current pairing key is correctly associated to an account
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ServiceResultMessage class record</returns>
        public static ServiceResultMessage ValidatePairingKey(string pairingKey, string serialKey)
        {
            PairingKey pKey = new PairingKey(pairingKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ValidatePairingKey", ReturnCode = MobileReturnCode.Success };
            if (!pKey.PairingKeyValid)
            {
                result.ReturnCode = MobileReturnCode.PairingKeyFormatInvalid;
            }
            else
            {
                cAccounts accs = new cAccounts();
                cAccount curAccount = accs.GetAccountByID(pKey.AccountID);
                if (curAccount == null)
                {
                    result.ReturnCode = MobileReturnCode.AccountInvalid;
                }
                else if (curAccount.archived)
                {
                    result.ReturnCode = MobileReturnCode.AccountArchived;
                }
                else
                {
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(pKey.AccountID);
                    cAccountProperties clsProperties = subaccs.getFirstSubAccount().SubAccountProperties;

                    if (!clsProperties.UseMobileDevices)
                    {
                        result.ReturnCode = MobileReturnCode.MobileDevicesDisabled;
                    }
                    else
                    {
                        cMobileDevices mobileDevices = new cMobileDevices(pKey.AccountID);
                        MobileDevice device = mobileDevices.GetDeviceByPairingKey(pKey.Pairingkey);

                        if (device == null)
                        {
                            result.ReturnCode = MobileReturnCode.PairingKeyNotFound;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(serialKey) && device.SerialKey != serialKey)
                            {
                                result.ReturnCode = MobileReturnCode.PairingKeySerialKeyMismatch;
                            }
                        }
                    }

                    if (result.ReturnCode == MobileReturnCode.Success)
                    {
                        // check employee access role
                        cEmployees employees = new cEmployees(pKey.AccountID);
                        Employee employee = employees.GetEmployeeById(pKey.EmployeeID);
                        if (employee.AdminOverride)
                        {
                            return result;
                        }

                        cAccessRoles clsRoles = new cAccessRoles(pKey.AccountID, cAccounts.getConnectionString(pKey.AccountID));
                        Dictionary<int, List<int>> subaccRoles = employee.GetAccessRoles().AllAccessRoles;
                        bool roleFound = false;

                        if (subaccRoles.Count == 0)
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                        else
                        {
                            foreach (KeyValuePair<int, List<int>> kvp in subaccRoles)
                            {
                                if (roleFound)
                                {
                                    break;
                                }

                                List<int> empRoles = kvp.Value;

                                if (empRoles.Count == 0)
                                {
                                    result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                                }
                                else
                                {
                                    MobileReturnCode tmpCode = MobileReturnCode.EmployeeHasInsufficientPermissions;

                                    foreach (int roleId in empRoles)
                                    {
                                        cAccessRole curRole = clsRoles.GetAccessRoleByID(roleId);
                                        if (curRole.AllowMobileAccess)
                                        {
                                            tmpCode = MobileReturnCode.Success;
                                            roleFound = true;
                                            break;
                                        }
                                    }

                                    result.ReturnCode = tmpCode;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}