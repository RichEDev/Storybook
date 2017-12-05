(function () {
    var scriptName = "BankAccounts";
    function execute() {
        SEL.registerNamespace("SEL.BankAccounts");
        SEL.BankAccounts =
        {
            ModalWindowAccountRegisteredString: "<strong>The account has been registered successfully.</strong><br /><br /><br />",
            ModalWindowDomID: null,
            ModalWindowAccountHeader: null,
            ModalWindowSpanInfoAccount: null,
            ModalWindowSpanAccountInfoImg: null,
            ModalWindowImgAccountInfoDomID: null,
            ModalWindowSpanPairingInfo: null,
            ModalWindowAccountName: null,
            ModalWindowAccountType: null,
            ModalWindowSaveButtonDomID: null,
            ModalInfoWindowDomID: null,
            BankAccountGridDomID: null,
            ModalAccountNameValidatorID: null,
            ModalAccountTypeValidatorID: null,
            DuplicateBankAccountString: "The Account Name already exists.",
            NewBankAccountString: "New Bank Account",
            EditBankAccountString: "Bank Account: {0}",
            CurrentLoadType: null,
            CurrentBankAccountID: null,
            CurrentEmployeeID: null,
            ModalWindowIban: null,
            ModalWindowSwiftCode: null,
            LoadType: { New: 1, Edit: 2 },
            /// <summary>
            /// This should be attached to controls with tooltips and will fetch the tooltip content and attach a 'close' event, the first parameter should be either the database help_text helpid or a custom string to display
            /// </summary>     
            CloseModal: function () {
                SEL.Common.HideModal(SEL.BankAccounts.ModalWindowDomID);
                return false;
            },
            CloseInfoModal: function () {
                SEL.Common.HideModal(SEL.BankAccounts.ModalInfoWindowDomID);
                return false;
            },
            ShowInfoModal: function () {
                SEL.Common.ShowModal(SEL.BankAccounts.ModalInfoWindowDomID);
                return false;
            },
            BankAccount: function () {
                this.AccountID = null;
                this.EmployeeID = null;
                this.AccountNumber = null;
                this.AccountType = null;
                this.SortCode = null;
                this.Reference = null;
                this.CurrencyId = null;
                this.Iban = null;
                this.SwiftCode = null;
            },
            SetModalEditBox: function (message) {
                $g(SEL.BankAccounts.ModalWindow.SpanEditDevice).innerHTML = message;
            },
            LoadBankAccountModal: function LoadBankAccountModal(loadType, bankAccountId) {
                SEL.BankAccounts.SetupModalClean();
                SEL.BankAccounts.CurrentLoadType = loadType;
                SEL.BankAccounts.CurrentBankAccountID = bankAccountId;
                if (loadType === SEL.BankAccounts.LoadType.New) {
                    $g(SEL.BankAccounts.ModalWindowSaveButtonDomID).onclick = function () { SEL.BankAccounts.SaveBankAccount(loadType); return false; };
                    $g(SEL.BankAccounts.ModalWindowAccountHeader).innerHTML = SEL.BankAccounts.NewBankAccountString;
                    $g(SEL.BankAccounts.ModalWindowtxtAccountNumber).disabled = false;
                    Spend_Management.shared.webServices.svcBankAccounts.GetBankPrimaryCountry(SEL.BankAccounts.LoadPrimaryCountry);
                }
                else {
                    $g(SEL.BankAccounts.ModalWindowSaveButtonDomID).onclick = function () { SEL.BankAccounts.SaveBankAccount(loadType, bankAccountId); return false; };

                    Spend_Management.shared.webServices.svcBankAccounts.GetEmployeeBankAccountById(bankAccountId, SEL.BankAccounts.CurrentEmployeeID, SEL.BankAccounts.LoadBankAccountModalComplete);
                }
            },
            LoadBankAccountModalComplete: function (bankAccount) {
                SEL.BankAccounts.SetupModal(bankAccount.AccountName, bankAccount.AccountNumber, bankAccount.AccountType, bankAccount.SortCode, bankAccount.Reference, bankAccount.CurrencyId,bankAccount.CountryId, bankAccount.Iban, bankAccount.SwiftCode);
            },
            LoadPrimaryCountry: function (country) {
                if (country != 0) {
                    $ddlSetSelected(SEL.BankAccounts.ModalWindowddlCountry, null, country);
                }
                SEL.Common.ShowModal(SEL.BankAccounts.ModalWindowDomID);
                if ($('#ddlCountry :selected').text() == 'United Kingdom') {
                    document.getElementById("lblSortCode").className = "mandatory"
                    document.getElementById("lblSortCode").innerText = "Sort Code*"
                    document.getElementById("retxtSortCodeEmptyCheck").enabled = true;
                }
                else {
                    document.getElementById("lblSortCode").className = ""
                    document.getElementById("lblSortCode").innerText = "Sort Code"
                    document.getElementById("retxtSortCodeEmptyCheck").enabled = false;
                }
                $g(SEL.BankAccounts.ModalWindowAccountName).focus();
            },
            RefreshGrid: function () {
                SEL.Grid.refreshGrid(SEL.BankAccounts.BankAccountGridDomID, SEL.Grid.getCurrentPageNum(SEL.BankAccounts.BankAccountGridDomID));
            },
            SetupModalClean: function () {
                $g(SEL.BankAccounts.ModalWindowAccountName).value = "";
                $g(SEL.BankAccounts.ModalWindowtxtAccountNumber).value = "";
                $ddlSetSelected(SEL.BankAccounts.ModalWindowcmbAccounttype, 0);
                $g(SEL.BankAccounts.ModalWindowtxtSortCode).value = "";
                $g(SEL.BankAccounts.ModalWindowtxtReference).value = "";
                $g(SEL.BankAccounts.ModalWindowIban).value = "";
                $g(SEL.BankAccounts.ModalWindowSwiftCode).value = "";
                SEL.Common.Page_ClientValidateReset();
            },
            SetupModal: function (accountname, accountnumber, accounttype, sortcode, reference, currency, country, iban, swiftCode) {


                if (SEL.BankAccounts.CurrentLoadType === SEL.BankAccounts.LoadType.Edit)
                {
                    $g(SEL.BankAccounts.ModalWindowAccountHeader).innerHTML = SEL.BankAccounts.EditBankAccountString.format(accountname);
                }
                else {
                    $g(SEL.BankAccounts.ModalWindowAccountHeader).innerHTML = SEL.BankAccounts.NewBankAccountString;
                }
                if (accountname !== null) {
                    $g(SEL.BankAccounts.ModalWindowAccountName).value = accountname;
                }

                if (iban != null) {
                    $g(SEL.BankAccounts.ModalWindowIban).value = iban;
                }

                if (swiftCode != null) {
                    $g(SEL.BankAccounts.ModalWindowSwiftCode).value = swiftCode;
                }

                if (accountnumber !== null) {
                    $g(SEL.BankAccounts.ModalWindowtxtAccountNumber).value = accountnumber;
                }

                if (accounttype !== null) {
                    $ddlSetSelected(SEL.BankAccounts.ModalWindowcmbAccounttype, null, accounttype);
                }

                if (sortcode !== null) {
                    $g(SEL.BankAccounts.ModalWindowtxtSortCode).value = sortcode;
                }

                if (reference !== null) {
                    $g(SEL.BankAccounts.ModalWindowtxtReference).value = reference;
                }

                if (currency !== null) {
                    $ddlSetSelected(SEL.BankAccounts.ModalWindowddlCurrency, null, currency);
                }

                if (country !== null) {
                    $ddlSetSelected(SEL.BankAccounts.ModalWindowddlCountry, null, country);
                }
                if ($('#ddlCountry :selected').text() == 'United Kingdom') {
                    document.getElementById("lblSortCode").className = "mandatory"
                    document.getElementById("lblSortCode").innerText = "Sort Code*"
                    document.getElementById("retxtSortCodeEmptyCheck").enabled = true;
                    document.getElementById("retxtaccountnumberlength").enabled = true;
                    document.getElementById("retxtSortCodeNumberCheck").enabled = true;
                    document.getElementById("retxtSortCodeCharactersCheck").enabled = true;
                }
                else {
                    document.getElementById("lblSortCode").className = ""
                    document.getElementById("lblSortCode").innerText = "Sort Code"
                    document.getElementById("retxtSortCodeEmptyCheck").enabled = false;
                    document.getElementById("retxtaccountnumberlength").enabled = false;
                    document.getElementById("retxtSortCodeNumberCheck").enabled = false;
                    document.getElementById("retxtSortCodeCharactersCheck").enabled =false;
                }
                SEL.Common.ShowModal(SEL.BankAccounts.ModalWindowDomID);
                $g(SEL.BankAccounts.ModalWindowAccountName).focus();
            },

            SaveBankAccountOnEnter: function () {
                SEL.BankAccounts.SaveBankAccount(SEL.BankAccounts.CurrentLoadType, SEL.BankAccounts.CurrentBankAccountID);
            },
            SaveBankAccount: function (loadType, bankAccountID) {
                SEL.BankAccounts.CurrentLoadType = loadType;

                if (SEL.Common.ValidateForm("vgAddEditAccount") === true) {

                    var bankAccount = new SEL.BankAccounts.BankAccount();

                    if (loadType === SEL.BankAccounts.LoadType.New) {
                        bankAccount.BankAccountId = 0;
                    } else {
                        bankAccount.BankAccountId = bankAccountID;
                    }

                    bankAccount.AccountName = $g(SEL.BankAccounts.ModalWindowAccountName).value;
                    bankAccount.AccountNumber = $g(SEL.BankAccounts.ModalWindowtxtAccountNumber).value;
                    bankAccount.AccountType = $ddlValue(SEL.BankAccounts.ModalWindowcmbAccounttype);
                    bankAccount.SortCode = $g(SEL.BankAccounts.ModalWindowtxtSortCode).value;
                    bankAccount.Reference = $g(SEL.BankAccounts.ModalWindowtxtReference).value;
                    bankAccount.CurrencyId = $ddlValue(SEL.BankAccounts.ModalWindowddlCurrency);
                    bankAccount.EmployeeID = SEL.BankAccounts.CurrentEmployeeID;
                    bankAccount.CountryId = $ddlValue(SEL.BankAccounts.ModalWindowddlCountry);
                    bankAccount.Iban = $g(SEL.BankAccounts.ModalWindowIban).value;
                    bankAccount.SwiftCode = $g(SEL.BankAccounts.ModalWindowSwiftCode).value;

                    Spend_Management.shared.webServices.svcBankAccounts.SaveBankAccount(bankAccount, SEL.BankAccounts.SaveBankAccountComplete, SEL.BankAccounts.ValidateBankAccountError);
                    return false;
                }
                return true;
            },
            SaveBankAccountComplete: function (bankAccountValid)
            {
                if (bankAccountValid && bankAccountValid.IsCorrect === false) {
                    SEL.BankAccounts.ShowValidationError(bankAccountValid);
                } else {
                    SEL.BankAccounts.CloseModal();
                    SEL.BankAccounts.RefreshGrid();
                }
            },
            ValidateBankAccountError: function (data)
            {
                var messageTopic = "Message from " + moduleNameHTML;
                if (data) {
                    SEL.MasterPopup.ShowMasterPopup(data._message, messageTopic);
                } 
            },
            ShowValidationError: function(bankAccountValid) {
                var messageTopic = "Message from " + moduleNameHTML;
                switch (bankAccountValid.StatusInformation) {
                    case "UnknownSortCode":
                        SEL.MasterPopup.ShowMasterPopup("The Sort Code entered is unknown.", messageTopic);
                        break;
                    case "InvalidAccountNumber":
                        SEL.MasterPopup.ShowMasterPopup("The Account Number you have entered is invalid.", messageTopic);
                        break;
                    case "DetailsChanged":
                        SEL.MasterPopup.ShowMasterPopup("The Account and Sortcode should be changed for BACS submission. The Sort Code should be '" + bankAccountValid.CorrectedSortCode + "' and the Account Number should be '" + bankAccountValid.CorrectedAccountNumber + "'", messageTopic);
                        break;
                    default:
                        SEL.MasterPopup.ShowMasterPopup(bankAccountValid.StatusInformation, messageTopic);
                }
            },
            updateGridFilterEmployeeId: function (employeeId) {
                SEL.BankAccounts.CurrentEmployeeID = employeeId;
                var values = [];
                values.push(employeeId);
                SEL.Grid.updateGridQueryFilterValues(SEL.BankAccounts.BankAccountGridDomID, '33873935-c9bc-4436-ad4c-3cf2120c7d4d', values, null); // update the employee ID in the bank account grid filter
            },
            deleteBankAccount: function (bankAccountId, employeeId) {
                currentRowID = bankAccountId;
                if (confirm('Are you sure you wish to delete the selected bank account?')) {
                    Spend_Management.shared.webServices.svcBankAccounts.DeleteBankAccount(bankAccountId, employeeId, SEL.BankAccounts.deleteBankAccountComplete);

                };
            },
            deleteBankAccountComplete: function (data) {
                var messageTopic = "Message from " + moduleNameHTML;
               
                switch (data) {
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('The bank account cannot be deleted as you have only one active account.', messageTopic);
                        break;
                    case -2:
                        SEL.MasterPopup.ShowMasterPopup('The bank account cannot be deleted as it is currently assigned to one or more Expense Items.', messageTopic);
                        break;
                    case -10:
                        SEL.MasterPopup.ShowMasterPopup('The bank account cannot be deleted as it currently assigned to a GreenLight or user defined field record.', messageTopic);
                        break;
                    default:
                        SEL.BankAccounts.RefreshGrid();
                        break;
                };
              
            },
            changeArchiveStatus: function (bankAccountId, employeeId) {
                currentRowID = bankAccountId
                Spend_Management.shared.webServices.svcBankAccounts.ChangeStatus(bankAccountId, employeeId, SEL.BankAccounts.changeArchiveStatusComplete);
            },
            changeArchiveStatusComplete: function (data) {
                var messageTopic = "Message from " + moduleNameHTML;
                switch (data) {
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('The bank account cannot be archived as you have only one active account.', messageTopic);
                        break;                    
                    default:
                        SEL.BankAccounts.RefreshGrid();
                        break;
                };
            }
        }
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}
)();


document.onkeydown = function (e) {
    e = e || window.event;
    if (e.keyCode == 27) {

        if($g('ctl00_pnlMasterPopup').style.display=='')
        {
            SEL.MasterPopup.HideMasterPopup();
            return;
        }
        
        SEL.BankAccounts.CloseModal();
    }
}