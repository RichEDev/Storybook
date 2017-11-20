Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports SpendManagementLibrary

Public Class cUtilSubAccounts
    Protected lst As Dictionary(Of Integer, cAccountSubAccount)

    Public Sub New()
        Dim db As New DBConnection(FWEmail.ConnectionString)
        lst = New Dictionary(Of Integer, cAccountSubAccount)

        Using reader As SqlDataReader = db.GetReader("select subaccountid, description from accountssubaccounts where archived = 0")
            While reader.Read
                Dim subaccountid As Integer = reader.GetInt32(0)
                Dim desc As String = reader.GetString(1)

                If lst.ContainsKey(subaccountid) = False Then
                    lst.Add(subaccountid, New cAccountSubAccount(subaccountid, desc, False, getUtilProperties(subaccountid), Now, 0, Nothing, Nothing))
                End If
            End While
            reader.Close()
        End Using

    End Sub

    Public Function getSubAccountById(ByVal subAccountId As Integer) As cAccountSubAccount
        If lst.ContainsKey(subAccountId) Then
            Return lst(subAccountId)
        Else
            Return Nothing
        End If
    End Function

    Private Function getUtilProperties(ByVal subaccountid As Integer) As cAccountProperties
        Dim lstAccountProperties As New cAccountProperties()
        Dim db As New DBConnection(FWEmail.ConnectionString)
        Dim stringKey, stringValue As String
        Dim createdOn, modifiedOn As DateTime
        Dim createdBy, modifiedBy As Integer

        For globalIdx As Integer = 0 To 1
            db.sqlexecute.Parameters.Clear()
            db.sqlexecute.Parameters.AddWithValue("@isGlobal", globalIdx)
            Dim sql As String = "SELECT subAccountID, stringKey, stringValue, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.accountProperties where isGlobal = @isGlobal"

            If globalIdx = 0 Then
                sql += " and subAccountID = @subAccId"
                db.sqlexecute.Parameters.AddWithValue("@subAccId", subaccountid)
            End If

            lstAccountProperties.SubAccountID = subaccountid

            Using reader As SqlDataReader = db.GetReader(sql)
                While reader.Read
                    stringKey = reader.GetString(reader.GetOrdinal("stringKey")).Trim()
                    If reader.IsDBNull(reader.GetOrdinal("stringValue")) Then
                        stringValue = ""
                    Else
                        stringValue = reader.GetString(reader.GetOrdinal("stringValue")).Trim()
                    End If

                    If reader.IsDBNull(reader.GetOrdinal("createdOn")) Then
                        createdOn = Nothing
                    Else
                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"))
                    End If

                    If reader.IsDBNull(reader.GetOrdinal("createdBy")) Then
                        createdBy = Nothing
                    Else
                        createdBy = reader.GetInt32(reader.GetOrdinal("createdBy"))
                    End If
                    If reader.IsDBNull(reader.GetOrdinal("modifiedOn")) Then
                        modifiedOn = Nothing
                    Else
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"))
                    End If
                    If reader.IsDBNull(reader.GetOrdinal("modifiedBy")) Then
                        modifiedBy = Nothing
                    Else
                        modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedBy"))
                    End If

                    Select Case stringKey
                        Case "emailServerAddress"
                            lstAccountProperties.EmailServerAddress = stringValue
                        Case "emailServerFromAddress"
                            lstAccountProperties.EmailServerFromAddress = stringValue
                        Case "showProductInSearch"
                            lstAccountProperties.ShowProductInSearch = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "documentRepository"
                            lstAccountProperties.DocumentRepository = stringValue
                        Case "auditorEmailAddress"
                            lstAccountProperties.AuditorEmailAddress = stringValue
                        Case "keepInvoiceForecasts"
                            lstAccountProperties.KeepInvoiceForecasts = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowMenuAddContract"
                            lstAccountProperties.AllowMenuContractAdd = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowArchivedNotesAdd"
                            lstAccountProperties.AllowArchivedNotesAdd = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "pwdMCN"
                            lstAccountProperties.PwdMustContainNumbers = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "pwdMCU"
                            lstAccountProperties.PwdMustContainUpperCase = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "pwdSymbol"
                            lstAccountProperties.PwdMustContainSymbol = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "pwdExpires"
                            lstAccountProperties.PwdExpires = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "pwdExpiryDays"
                            lstAccountProperties.PwdExpiryDays = Convert.ToInt32(stringValue)
                        Case "pwdConstraint"
                            lstAccountProperties.PwdConstraint = CType(Convert.ToByte(stringValue), PasswordLength)
                        Case "pwdLength1"
                            lstAccountProperties.PwdLength1 = Convert.ToInt32(stringValue)
                        Case "pwdLength2"
                            lstAccountProperties.PwdLength2 = Convert.ToInt32(stringValue)
                        Case "pwdMaxRetries"
                            lstAccountProperties.PwdMaxRetries = Convert.ToByte(stringValue)
                        Case "pwdHistoryNum"
                            lstAccountProperties.PwdHistoryNum = Convert.ToInt32(stringValue)
                        Case "contractKey"
                            lstAccountProperties.ContractKey = stringValue
                        Case "autoUpdateCV"
                            lstAccountProperties.AutoUpdateAnnualContractValue = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "defaultPageSize"
                            lstAccountProperties.DefaultPageSize = Convert.ToInt32(stringValue)
                        Case "applicationURL"
                            lstAccountProperties.ApplicationURL = stringValue
                        Case "mileage"
                            lstAccountProperties.Mileage = Convert.ToInt32(stringValue)
                        Case "currencyType"
                            lstAccountProperties.currencyType = CType(Convert.ToInt32(stringValue), CurrencyType)
                        Case "dbVersion"
                            lstAccountProperties.DBVersion = Convert.ToInt16(stringValue)
                        Case "standards"
                            lstAccountProperties.Standards = stringValue
                        Case "broadcast"
                            lstAccountProperties.Broadcast = stringValue
                        Case "homeCountry"
                            lstAccountProperties.HomeCountry = Convert.ToInt32(stringValue)
                        Case "policyType"
                            lstAccountProperties.PolicyType = Convert.ToByte(stringValue)
                        Case "limits"
                            lstAccountProperties.Limits = Convert.ToByte(stringValue)
                        Case "duplicates"
                            lstAccountProperties.Duplicates = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "curImportId"
                            lstAccountProperties.CurImportId = Convert.ToInt32(stringValue)
                        Case "compMileage"
                            lstAccountProperties.CompMileage = Convert.ToByte(stringValue)
                        Case "weekend"
                            lstAccountProperties.Weekend = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useCostCodes"
                            lstAccountProperties.UseCostCodes = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useDepartmentCodes"
                            lstAccountProperties.UseDepartmentCodes = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "importCC"
                            lstAccountProperties.ImportCC = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "ccAdmin"
                            lstAccountProperties.CCAdmin = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "singleClaim"
                            lstAccountProperties.SingleClaim = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useCostCodeDescription"
                            lstAccountProperties.UseCostCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useDepartmentCodeDescription"
                            lstAccountProperties.UseDepartmentCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "attachReceipts"
                            lstAccountProperties.AttachReceipts = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "ccUserSettles"
                            lstAccountProperties.CCUserSettles = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "limitDates"
                            lstAccountProperties.LimitDates = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "initialDate"
                            If stringValue = "" Then
                                lstAccountProperties.InitialDate = Nothing
                            Else
                                lstAccountProperties.InitialDate = Convert.ToDateTime(stringValue)
                            End If
                        Case "limitMonths"
                            If stringValue = "" Then
                                lstAccountProperties.LimitMonths = Nothing
                            Else
                                lstAccountProperties.LimitMonths = Convert.ToInt32(stringValue)
                            End If
                        Case "flagDate"
                            lstAccountProperties.FlagDate = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "limitsReceipt"
                            lstAccountProperties.LimitsReceipt = Convert.ToByte(stringValue)
                        Case "mainAdministrator"
                            lstAccountProperties.MainAdministrator = Convert.ToInt32(stringValue)
                        Case "increaseOthers"
                            lstAccountProperties.IncreaseOthers = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "searchEmployees"
                            lstAccountProperties.SearchEmployees = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "preApproval"
                            lstAccountProperties.PreApproval = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "showReviews"
                            lstAccountProperties.ShowReviews = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "showBankDetails"
                            lstAccountProperties.ShowBankDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "mileagePrev"
                            lstAccountProperties.MileagePrev = Convert.ToInt32(stringValue)
                        Case "minClaimAmount"
                            lstAccountProperties.MinClaimAmount = Convert.ToDecimal(stringValue)
                        Case "maxClaimAmount"
                            lstAccountProperties.MaxClaimAmount = Convert.ToDecimal(stringValue)
                        Case "tipLimit"
                            lstAccountProperties.TipLimit = Convert.ToInt32(stringValue)
                        Case "exchangeReadOnly"
                            lstAccountProperties.ExchangeReadOnly = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useProjectCodes"
                            lstAccountProperties.UseProjectCodes = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useProjectCodeDescription"
                            lstAccountProperties.UseProjectCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "odometerDay"
                            lstAccountProperties.OdometerDay = Convert.ToByte(stringValue)
                        Case "addLocations"
                            lstAccountProperties.AddLocations = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "rejectTip"
                            lstAccountProperties.RejectTip = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "costCodesOn"
                            lstAccountProperties.CostCodesOn = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "departmentsOn"
                            lstAccountProperties.DepartmentsOn = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "projectCodesOn"
                            lstAccountProperties.ProjectCodesOn = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "partSubmittal"
                            lstAccountProperties.PartSubmit = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "onlyCashCredit"
                            lstAccountProperties.OnlyCashCredit = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "locationSearch"
                            lstAccountProperties.LocationSearch = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "language"
                            lstAccountProperties.Language = stringValue
                        Case "limitFrequency"
                            lstAccountProperties.LimitFrequency = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "frequencyType"
                            If stringValue = "" Then
                                lstAccountProperties.FrequencyType = 0
                            Else
                                lstAccountProperties.FrequencyType = Convert.ToByte(stringValue)
                            End If
                        Case "frequencyValue"
                            If stringValue = "" Then
                                lstAccountProperties.FrequencyValue = 0
                            Else
                                lstAccountProperties.FrequencyValue = Convert.ToInt32(stringValue)
                            End If
                        Case "overrideHome"
                            lstAccountProperties.OverrideHome = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "sourceAddress"
                            lstAccountProperties.SourceAddress = Convert.ToByte(stringValue)
                        Case "editMyDetails"
                            lstAccountProperties.EditMyDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "autoAssignAllocation"
                            lstAccountProperties.AutoAssignAllocation = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "enterOdometerOnSubmit"
                            lstAccountProperties.EnterOdometerOnSubmit = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "displayFlagAdded"
                            lstAccountProperties.DisplayFlagAdded = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "flagMessage"
                            lstAccountProperties.FlagMessage = stringValue
                        Case "baseCurrency"
                            If stringValue = "" Then
                                lstAccountProperties.BaseCurrency = Nothing
                            Else
                                lstAccountProperties.BaseCurrency = Convert.ToInt32(stringValue)
                            End If
                        Case "importPurchaseCard"
                            lstAccountProperties.ImportPurchaseCard = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfReg"
                            lstAccountProperties.AllowSelfReg = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegRole"
                            lstAccountProperties.AllowSelfRegRole = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegItemRole"
                            lstAccountProperties.AllowSelfRegItemRole = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegEmpContact"
                            lstAccountProperties.AllowSelfRegEmployeeContact = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegHomeAddress"
                            lstAccountProperties.AllowSelfRegHomeAddress = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegEmpInfo"
                            lstAccountProperties.AllowSelfRegEmployeeInfo = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegSignOff"
                            lstAccountProperties.AllowSelfRegSignOff = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegAdvancesSignOff"
                            lstAccountProperties.AllowSelfRegAdvancesSignOff = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegDeptCostCode"
                            lstAccountProperties.AllowSelfRegDepartmentCostCode = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegBankDetails"
                            lstAccountProperties.AllowSelfRegBankDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegCarDetails"
                            lstAccountProperties.AllowSelfRegCarDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowSelfRegUDF"
                            lstAccountProperties.AllowSelfRegUDF = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "purchaseCardSubCatId"
                            If stringValue = "" Then
                                lstAccountProperties.PurchaseCardSubCatId = 0
                            Else
                                lstAccountProperties.PurchaseCardSubCatId = Convert.ToInt32(stringValue)
                            End If
                        Case "defaultRole"
                            If stringValue = "" Then
                                lstAccountProperties.DefaultRole = Nothing
                            Else
                                lstAccountProperties.DefaultRole = Convert.ToInt32(stringValue)
                            End If
                        Case "defaultItemRole"
                            If stringValue = "" Then
                                lstAccountProperties.DefaultItemRole = Nothing
                            Else
                                lstAccountProperties.DefaultItemRole = Convert.ToInt32(stringValue)
                            End If
                        Case "singleClaimCC"
                            lstAccountProperties.SingleClaimCC = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "singleClaimPC"
                            lstAccountProperties.SingleClaimPC = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "displayLimits"
                            lstAccountProperties.DisplayLimits = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "drilldownReport"
                            If String.IsNullOrEmpty(stringValue) = True Then
                                lstAccountProperties.DrilldownReport = Nothing
                            Else
                                lstAccountProperties.DrilldownReport = New Guid(stringValue)
                            End If
                        Case "blockCashCC"
                            lstAccountProperties.BlockCashCC = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "blockCashPC"
                            lstAccountProperties.BlockCashPC = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "blockLicenceExpiry"
                            lstAccountProperties.BlockLicenceExpiry = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "blockTaxExpiry"
                            lstAccountProperties.BlockTaxExpiry = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "blockMOTExpiry"
                            lstAccountProperties.BlockMOTExpiry = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "blockInsuranceExpiry"
                            lstAccountProperties.BlockInsuranceExpiry = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delSetup"
                            lstAccountProperties.DelSetup = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delEmployeeAccounts"
                            lstAccountProperties.DelEmployeeAccounts = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delEmployeeAdmin"
                            lstAccountProperties.DelEmployeeAdmin = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delReports"
                            lstAccountProperties.DelReports = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delReportsClaimants"
                            lstAccountProperties.DelReportsClaimants = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delCheckAndPay"
                            lstAccountProperties.DelCheckAndPay = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delQEDesign"
                            lstAccountProperties.DelQEDesign = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delCorporateCards"
                            lstAccountProperties.DelCorporateCards = Convert.ToBoolean(Convert.ToByte(stringValue))
                            'Case "delPurchaseCards"
                            '    lstAccountProperties.DelPurchaseCards = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delApprovals"
                            lstAccountProperties.DelApprovals = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delSubmitClaims"
                            lstAccountProperties.DelSubmitClaim = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delExports"
                            lstAccountProperties.DelExports = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "delAuditLog"
                            lstAccountProperties.DelAuditLog = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "sendReviewRequests"
                            lstAccountProperties.SendReviewRequests = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "claimantDeclaration"
                            lstAccountProperties.ClaimantDeclaration = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "declarationMessage"
                            lstAccountProperties.DeclarationMsg = stringValue
                        Case "approverDeclarationMessage"
                            lstAccountProperties.ApproverDeclarationMsg = stringValue
                        Case "allowMultipleDestinations"
                            lstAccountProperties.AllowMultipleDestinations = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useMapPoint"
                            lstAccountProperties.UseMapPoint = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useCostCodeOnGenDetails"
                            lstAccountProperties.UseCostCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useDepartmentOnGenDetails"
                            lstAccountProperties.UseDeptOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "useProjectCodeOnGenDetails"
                            lstAccountProperties.UseProjectCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "homeToOffice"
                            lstAccountProperties.HomeToOffice = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "calcHomeToLocation"
                            lstAccountProperties.CalcHomeToLocation = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "showMileageCatsForUsers"
                            lstAccountProperties.ShowMileageCatsForUsers = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "activateCarOnUserAdd"
                            lstAccountProperties.ActivateCarOnUserAdd = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "autoCalcHomeToLocation"
                            lstAccountProperties.AutoCalcHomeToLocation = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "enableAutoLog"
                            lstAccountProperties.EnableAutolog = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowUsersToAddCars"
                            lstAccountProperties.AllowUsersToAddCars = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "mileageCalcType"
                            lstAccountProperties.MileageCalcType = Convert.ToByte(stringValue)
                        Case "productFieldType"
                            lstAccountProperties.ProductFieldType = CType(Convert.ToByte(stringValue), FieldType)
                        Case "supplierFieldType"
                            lstAccountProperties.SupplierFieldType = CType(Convert.ToByte(stringValue), FieldType)
                        Case "poNumberName"
                            lstAccountProperties.PONumberName = stringValue
                        Case "poNumberGenerate"
                            lstAccountProperties.PONumberGenerate = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "poNumberSequence"
                            lstAccountProperties.PONumberSequence = Convert.ToInt32(stringValue)
                        Case "poNumberFormat"
                            lstAccountProperties.PONumberFormat = stringValue
                        Case "dateApprovedName"
                            lstAccountProperties.DateApprovedName = stringValue
                        Case "totalName"
                            lstAccountProperties.TotalName = stringValue
                        Case "orderRecurrenceName"
                            lstAccountProperties.OrderRecurrenceName = stringValue
                        Case "orderEndDateName"
                            lstAccountProperties.OrderEndDateName = stringValue
                        Case "commentsName"
                            lstAccountProperties.CommentsName = stringValue
                        Case "productName"
                            lstAccountProperties.ProductName = stringValue
                        Case "countryName"
                            lstAccountProperties.CountryName = stringValue
                        Case "currencyName"
                            lstAccountProperties.CurrencyName = stringValue
                        Case "exchangeRateName"
                            lstAccountProperties.ExchangeRateName = stringValue
                        Case "allowRecurring"
                            lstAccountProperties.AllowRecurring = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "autoArchiveType"
                            lstAccountProperties.AutoArchiveType = CType(Convert.ToByte(stringValue), AutoArchiveType)
                        Case "autoActivateType"
                            lstAccountProperties.AutoActivateType = CType(Convert.ToByte(stringValue), AutoActivateType)
                        Case "archiveGracePeriod"
                            lstAccountProperties.ArchiveGracePeriod = Convert.ToInt16(stringValue)
                        Case "importUsernameFormat"
                            lstAccountProperties.ImportUsernameFormat = stringValue
                        Case "ImportHomeAddressFormat"
                            lstAccountProperties.ImportHomeAddressFormat = stringValue
                        Case "globalLocaleID"
                            lstAccountProperties.GlobalLocaleID = Convert.ToInt32(stringValue)
                            'Case "rechargeReferenceAs"
                            '    rsRefAs = stringValue
                            'Case "employeeReferenceAs"
                            '    rsEmpRefAs = stringValue
                            'Case "rechargePeriod"
                            '    rsRechargePd = Convert.ToInt32(stringValue)
                            'Case "financialYearCommences"
                            '    rsFinYear = Convert.ToInt32(stringValue)
                            'Case "cpDeleteAction"
                            '    rsCPAction = Convert.ToInt32(stringValue)
                        Case "taskStartDateMandatory"
                            lstAccountProperties.TaskStartDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "taskEndDateMandatory"
                            lstAccountProperties.TaskEndDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "taskDueDateMandatory"
                            lstAccountProperties.TaskDueDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "autoUpdateLicenceTotal"
                            lstAccountProperties.AutoUpdateLicenceTotal = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "contractCategoryTitle"
                            lstAccountProperties.ContractCategoryTitle = stringValue
                        Case "inflatorActive"
                            lstAccountProperties.InflatorActive = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "invoiceFrequencyActive"
                            lstAccountProperties.InvoiceFreqActive = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "termTypeActive"
                            lstAccountProperties.TermTypeActive = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "valueComments"
                            lstAccountProperties.ValueComments = stringValue
                        Case "contractDescTitle"
                            lstAccountProperties.ContractDescTitle = stringValue
                        Case "contractDescShortTitle"
                            lstAccountProperties.ContractDescShortTitle = stringValue
                        Case "contractNumGen"
                            lstAccountProperties.ContractNumGen = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "contractNumSeq"
                            lstAccountProperties.ContractNumSeq = Convert.ToInt32(stringValue)
                        Case "supplierStatusEnforced"
                            lstAccountProperties.SupplierStatusEnforced = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "contractCategoryMandatory"
                            lstAccountProperties.ContractCatMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierLastFinStatusEnabled"
                            lstAccountProperties.SupplierLastFinStatusEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierLastFinCheckEnabled"
                            lstAccountProperties.SupplierLastFinCheckEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierFYEEnabled"
                            lstAccountProperties.SupplierFYEEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierNumEmployeesEnabled"
                            lstAccountProperties.SupplierNumEmployeesEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierTurnoverEnabled"
                            lstAccountProperties.SupplierTurnoverEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierIntContactEnabled"
                            lstAccountProperties.SupplierIntContactEnabled = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierCategoryTitle"
                            lstAccountProperties.SupplierCatTitle = stringValue
                        Case "openSaveAttachments"
                            lstAccountProperties.OpenSaveAttachments = Convert.ToByte(stringValue)
                        Case "hyperlinkAttachmentsEnabled"
                            lstAccountProperties.EnableAttachmentHyperlink = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "attachmentUploadEnabled"
                            lstAccountProperties.EnableAttachmentUpload = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "cacheTimeout"
                            lstAccountProperties.CacheTimeout = Convert.ToInt32(stringValue)
                        Case "flashingNotesIconEnabled"
                            lstAccountProperties.EnableFlashingNotesIcon = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "contractNumUpdateEnabled"
                            lstAccountProperties.EnableContractNumUpdate = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "FYStarts"
                            lstAccountProperties.FYStarts = stringValue
                        Case "FYEnds"
                            lstAccountProperties.FYEnds = stringValue
                        Case "rechargeUnrecoveredTitle"
                            lstAccountProperties.RechargeUnrecoveredTitle = stringValue
                        Case "supplierRegionTitle"
                            lstAccountProperties.SupplierRegionTitle = stringValue
                        Case "supplierPrimaryTitle"
                            lstAccountProperties.SupplierPrimaryTitle = stringValue
                        Case "supplierVariationTitle"
                            lstAccountProperties.SupplierVariationTitle = stringValue
                        Case "taskEscalationRepeat"
                            lstAccountProperties.TaskEscalationRepeat = Convert.ToInt32(stringValue)
                        Case "autoUpdateCVRechargeLive"
                            lstAccountProperties.AutoUpdateCVRechargeLive = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "linkAttachmentDefault"
                            lstAccountProperties.LinkAttachmentDefault = Convert.ToByte(stringValue)
                        Case "penaltyClauseTitle"
                            lstAccountProperties.PenaltyClauseTitle = stringValue
                        Case "contractScheduleDefault"
                            lstAccountProperties.ContractScheduleDefault = stringValue
                        Case "variationAutoSeqEnabled"
                            lstAccountProperties.EnableVariationAutoSeq = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "maxUploadSize"
                            lstAccountProperties.MaxUploadSize = Convert.ToInt32(stringValue)
                        Case "useCPExtraInfo"
                            lstAccountProperties.UseCPExtraInfo = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "supplierCategoryMandatory"
                            lstAccountProperties.SupplierCatMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "errorEmailSubmitAddress"
                            lstAccountProperties.ErrorEmailAddress = stringValue
                        Case "errorEmailSubmitFromAddress"
                            lstAccountProperties.ErrorEmailFromAddress = stringValue
                        Case "recordOdometer"
                            lstAccountProperties.RecordOdometer = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "enableRecharge"
                            lstAccountProperties.EnableRecharge = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "allowTeamMemberToApproveOwnClaim"
                            lstAccountProperties.AllowTeamMemberToApproveOwnClaim = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "contractDatesMandatory"
                            lstAccountProperties.ContractDatesMandatory = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "logoPath"
                            lstAccountProperties.LogoPath = stringValue
                        Case "emailAdministrator"
                            lstAccountProperties.EmailAdministrator = stringValue
                        Case "cachePeriodShort"
                            lstAccountProperties.cachePeriodShort = Convert.ToInt32(stringValue)
                        Case "cachePeriodNormal"
                            lstAccountProperties.cachePeriodNormal = Convert.ToInt32(stringValue)
                        Case "cachePeriodLong"
                            lstAccountProperties.cachePeriodLong = Convert.ToInt32(stringValue)
                        Case "customerHelpInformation"
                            lstAccountProperties.CustomerHelpInformation = stringValue
                        Case "customerHelpContactName"
                            lstAccountProperties.CustomerHelpContactName = stringValue
                        Case "customerHelpContactTelephone"
                            lstAccountProperties.CustomerHelpContactTelephone = stringValue
                        Case "customerHelpContactFax"
                            lstAccountProperties.CustomerHelpContactFax = stringValue
                        Case "customerHelpContactAddress"
                            lstAccountProperties.CustomerHelpContactAddress = stringValue
                        Case "customerHelpContactEmailAddress"
                            lstAccountProperties.CustomerHelpContactEmailAddress = stringValue
                        Case "allowClaimantSelectHomeAddress"
                            lstAccountProperties.AllowClaimantSelectHomeAddress = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "mandatoryPostcodeForAddresses"
                            lstAccountProperties.MandatoryPostcodeForAddresses = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "ClaimantsCanAddCompanyLocations"
                            lstAccountProperties.ClaimantsCanAddCompanyLocations = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "CheckESRAssignmentOnEmployeeSave"
                            lstAccountProperties.CheckESRAssignmentOnEmployeeAdd = Convert.ToBoolean(Convert.ToByte(stringValue))
                        Case "coloursMenuBarBackground"
                            lstAccountProperties.ColoursMenuBarBackground = stringValue
                        Case "coloursMenuBarForeground"
                            lstAccountProperties.ColoursMenuBarForeground = stringValue
                        Case "coloursTitleBarBackground"
                            lstAccountProperties.ColoursTitleBarBackground = stringValue
                        Case "coloursTitleBarForeground"
                            lstAccountProperties.ColoursTitleBarForeground = stringValue
                        Case "coloursFieldBackground"
                            lstAccountProperties.ColoursFieldBackground = stringValue
                        Case "coloursFieldForeground"
                            lstAccountProperties.ColoursFieldForeground = stringValue
                        Case "coloursRowBackground"
                            lstAccountProperties.ColoursRowBackground = stringValue
                        Case "coloursRowForeground"
                            lstAccountProperties.ColoursRowForeground = stringValue
                        Case "coloursAlternateRowBackground"
                            lstAccountProperties.ColoursAlternateRowBackground = stringValue
                        Case "coloursAlternateRowForeground"
                            lstAccountProperties.ColoursAlternateRowForeground = stringValue
                        Case "coloursHover"
                            lstAccountProperties.ColoursHover = stringValue
                        Case "coloursPageOptionForeground"
                            lstAccountProperties.ColoursPageOptionForeground = stringValue
                        Case Else

                    End Select
                End While
                reader.Close()

            End Using
        Next

        Return lstAccountProperties
    End Function

    Public Function CreateDropDown() As System.Web.UI.WebControls.ListItem()
        Dim items As New List(Of System.Web.UI.WebControls.ListItem)

        For Each s As KeyValuePair(Of Integer, cAccountSubAccount) In lst
            Dim subacc As cAccountSubAccount = s.Value
            Dim item As New System.Web.UI.WebControls.ListItem(subacc.Description, subacc.SubAccountID.ToString)
            items.Add(item)
        Next

        Return items.ToArray
    End Function

    Public Function GetFirstSubAccount() As cAccountSubAccount
        Dim ret As cAccountSubAccount = Nothing

        For Each s As KeyValuePair(Of Integer, cAccountSubAccount) In lst
            ret = s.Value
            Exit For
        Next

        Return ret
    End Function

    Public ReadOnly Property Count As Integer
        Get
            Return lst.Count
        End Get
    End Property
End Class
