namespace BusinessLogic.GeneralOptions
{
    using System.ComponentModel;

    using BusinessLogic.AccountProperties;

    /// <summary>
    /// An enum to store the key for every <see cref="IAccountProperty"/> via the <see cref="DescriptionAttribute"/> attribute
    /// </summary>
    public enum AccountPropertyKeys
    {
        [Description("applicationURL")]
        ApplicationURL,

        [Description("language")]
        Language,

        [Description("AccountCurrentlyLockedMessage")]
        AccountCurrentlyLockedMessage,

        [Description("AccountLockedMessage")]
        AccountLockedMessage,

        [Description("exchangeReadOnly")]
        ExchangeReadOnly,

        [Description("ClaimantsCanAddCompanyLocations")]
        ClaimantsCanAddCompanyLocations,

        [Description("includeAssignmentDetails")]
        IncludeAssignmentDetails,

        [Description("DisableCarOutsideOfStartEndDate")]
        DisableCarOutsideOfStartEndDate,

        [Description("homeAddressKeyword")]
        HomeAddressKeyword,

        [Description("workAddressKeyword")]
        WorkAddressKeyword,

        [Description("forceAddressNameEntry")]
        ForceAddressNameEntry,

        [Description("addressNameEntryMessage")]
        AddressNameEntryMessage,

        [Description("DisplayEsrAddressesInSearchResults")]
        DisplayEsrAddressesInSearchResults,

        [Description("MultipleWorkAddress")]
        MultipleWorkAddress,

        [Description("retainLabelsTime")]
        RetainLabelsTime,

        [Description("mainAdministrator")]
        MainAdministrator,

        [Description("NumberOfApproversToRememberForClaimantInApprovalMatrixClaim")]
        NumberOfApproversToRememberForClaimantInApprovalMatrixClaim,

        [Description("auditorEmailAddress")]
        AuditorEmailAddress,

        [Description("showMileageCatsForUsers")]
        ShowMileageCatsForUsers,

        [Description("activateCarOnUserAdd")]
        ActivateCarOnUserAdd,

        [Description("allowUsersToAddCars")]
        AllowUsersToAddCars,

        [Description("allowEmployeeToSpecifyCarStartDateOnAdd")]
        AllowEmpToSpecifyCarStartDateOnAdd,

        [Description("allowEmployeeToSpecifyCarDOCOnAdd")]
        AllowEmpToSpecifyCarDOCOnAdd,

        [Description("allowEmployeeToSpecifyCarStartDateOnAddMandatory")]
        EmpToSpecifyCarStartDateOnAddMandatory,

        [Description("VehicleLookup")]
        VehicleLookup,

        [Description("singleClaim")]
        SingleClaim,

        [Description("attachReceipts")]
        AttachReceipts,

        [Description("preApproval")]
        PreApproval,

        [Description("partSubmittal")]
        PartSubmit,

        [Description("onlyCashCredit")]
        OnlyCashCredit,

        [Description("frequencyType")]
        FrequencyType,

        [Description("frequencyValue")]
        FrequencyValue,

        [Description("claimantDeclaration")]
        ClaimantDeclaration,

        [Description("declarationMessage")]
        DeclarationMsg,

        [Description("approverDeclarationMessage")]
        ApproverDeclarationMsg,

        [Description("allowTeamMemberToApproveOwnClaim")]
        AllowTeamMemberToApproveOwnClaim,

        [Description("allowTeamMemberToApproveOwnClaim")]
        AllowEmployeeToApproveOwnClaim,

        [Description("AllowEmployeeInOwnSignoffGroup")]
        AllowEmployeeInOwnSignoffGroup,

        [Description("showFullHomeAddress")]
        ShowFullHomeAddressOnClaims,

        [Description("editPreviousClaims")]
        EditPreviousClaims,

        [Description("BlockUnmachedExpenseItemsBeingSubmitted")]
        BlockUnmatchedExpenseItemsBeingSubmitted,

        [Description("useCostCodes")]
        UseCostCodes,

        [Description("useDepartmentCodes")]
        UseDepartmentCodes,

        [Description("useCostCodeDescription")]
        UseCostCodeDescription,

        [Description("useDepartmentCodeDescription")]
        UseDepartmentDescription,

        [Description("useProjectCodes")]
        UseProjectCodes,

        [Description("useProjectCodeDescription")]
        UseProjectCodeDesc,

        [Description("costCodesOn")]
        CostCodesOn,

        [Description("departmentsOn")]
        DepartmentsOn,

        [Description("projectCodesOn")]
        ProjectCodesOn,

        [Description("autoAssignAllocation")]
        AutoAssignAllocation,

        [Description("useCostCodeOnGenDetails")]
        UseCostCodeOnGenDetails,

        [Description("useDepartmentOnGenDetails")]
        UseDeptOnGenDetails,

        [Description("useProjectCodeOnGenDetails")]
        UseProjectCodeOnGenDetails,

        [Description("defaultCostCodeOwner")]
        CostCodeOwnerBaseKey,

        [Description("coloursHeaderBackground")]
        ColoursHeaderBackground,

        [Description("coloursHeaderBreadcrumbText")]
        ColoursHeaderBreadcrumbText,

        [Description("coloursPageTitleText")]
        ColoursPageTitleText,

        [Description("coloursSectionHeadingUnderline")]
        ColoursSectionHeadingUnderline,

        [Description("coloursSectionHeadingText")]
        ColoursSectionHeadingText,

        [Description("coloursFieldText")]
        ColoursFieldText,

        [Description("coloursPageOptionsBackground")]
        ColoursPageOptionsBackground,

        [Description("coloursPageOptionsText")]
        ColoursPageOptionsText,

        [Description("coloursTableHeaderBackground")]
        ColoursTableHeaderBackground,

        [Description("coloursTableHeaderText")]
        ColoursTableHeaderText,

        [Description("coloursTabOptionBackground")]
        ColoursTabOptionBackground,

        [Description("coloursTabOptionText")]
        ColoursTabOptionText,

        [Description("coloursRowBackground")]
        ColoursRowBackground,

        [Description("coloursRowText")] ColoursRowText,

        [Description("coloursAlternateRowBackground")]
        ColoursAlternateRowBackground,

        [Description("coloursAlternateRowText")]
        ColoursAlternateRowText,

        [Description("coloursMenuOptionHoverText")]
        ColoursMenuOptionHoverText,

        [Description("coloursMenuOptionStandardText")]
        ColoursMenuOptionStandardText,

        [Description("coloursTooltipBackground")]
        ColoursTooltipBackground,

        [Description("coloursTooltipText")]
        ColoursTooltipText,

        [Description("coloursGreenLightField")]
        ColoursGreenLightField,

        [Description("coloursGreenLightSectionText")]
        ColoursGreenLightSectionText,

        [Description("coloursGreenLightSectionBackground")]
        ColoursGreenLightSectionBackground,

        [Description("coloursGreenLightSectionUnderline")]
        ColoursGreenLightSectionUnderline,

        [Description("CompanyPolicy")]
        CompanyPolicy,

        [Description("policyType")]
        PolicyType,

        [Description("homeCountry")]
        HomeCountry,

        [Description("currencyType")]
        CurrencyType,

        [Description("baseCurrency")]
        BaseCurrency,

        [Description("EnableAutoUpdateOfExchangeRates")]
        EnableAutoUpdateOfExchangeRates,

        [Description("EnableAutoUpdateOfExchangeRatesActivatedDate")]
        EnableAutoUpdateOfExchangeRatesActivatedDate,

        [Description("ExchangeRateProvider")]
        ExchangeRateProvider,

        [Description("delSetup")]
        DelSetup,

        [Description("delEmployeeAccounts")]
        DelEmployeeAccounts,

        [Description("delEmployeeAdmin")]
        DelEmployeeAdmin,

        [Description("delReports")]
        DelReports,

        [Description("delReportsClaimants")]
        DelReportsClaimants,

        [Description("delCheckAndPay")]
        DelCheckAndPay,

        [Description("delQEDesign")]
        DelQEDesign,

        [Description("delCorporateCards")]
        DelCorporateCards,

        [Description("delApprovals")]
        DelApprovals,

        [Description("delSubmitClaims")]
        DelSubmitClaim,

        [Description("delExports")]
        DelExports,

        [Description("delAuditLog")]
        DelAuditLog,

        [Description("delOptionsForDelegateAccessRole")]
        EnableDelegateOptionsForDelegateAccessRole,

        [Description("enableAutomaticDrivingLicenceLookup")]
        EnableAutomaticDrivingLicenceLookup,

        [Description("consentReminderFrequency")]
        FrequencyOfConsentRemindersLookup,

        [Description("drivingLicenceLookupFrequency")]
        DrivingLicenceLookupFrequency,

        [Description("blockDrivingLicence")]
        BlockDrivingLicence,
        
        [Description("blockTaxExpiry")]
        BlockTaxExpiry,

        [Description("blockMOTExpiry")]
        BlockMOTExpiry,

        [Description("blockInsuranceExpiry")]
        BlockInsuranceExpiry,

        [Description("blockBreakdownCoverExpiry")]
        BlockBreakdownCoverExpiry,

        [Description("useDateOfExpenseForDutyOfCareChecks")]
        UseDateOfExpenseForDutyOfCareChecks,

        [Description("dutyOfCareEmailReminderForClaimantDays")]
        RemindClaimantOnDOCDocumentExpiryDays,

        [Description("dutyOfCareEmailReminderForApproverDays")]
        RemindApproverOnDOCDocumentExpiryDays,

        [Description("dutyOfCareTeamAsApprover")]
        DutyOfCareTeamAsApprover,

        [Description("dutyOfCareApprover")]
        DutyOfCareApprover,

        [Description("DrivingLicenceReviewPeriodically")]
        EnableDrivingLicenceReview,

        [Description("DrivingLicenceReviewFrequency")]
        DrivingLicenceReviewFrequency,

        [Description("DrivingLicenceReviewReminder")]
        DrivingLicenceReviewReminder,

        [Description("DrivingLicenceReviewReminderDays")]
        DrivingLicenceReviewReminderDays,

        [Description("emailServerAddress")]
        EmailServerAddress,

        [Description("emailServerFromAddress")]
        EmailServerFromAddress,

        [Description("sourceAddress")]
        SourceAddress,

        [Description("errorEmailSubmitAddress")]
        ErrorEmailAddress,

        [Description("errorEmailSubmitFromAddress")]
        ErrorEmailFromAddress,

        [Description("emailAdministrator")]
        EmailAdministrator,

        [Description("searchEmployees")]
        SearchEmployees,

        [Description("autoArchiveType")]
        AutoArchiveType,

        [Description("autoActivateType")]
        AutoActivateType,

        [Description("archiveGracePeriod")]
        ArchiveGracePeriod,

        [Description("importUsernameFormat")]
        ImportUsernameFormat,

        [Description("ImportHomeAddressFormat")]
        ImportHomeAddressFormat,

        [Description("CheckESRAssignmentOnEmployeeSave")]
        CheckESRAssignmentOnEmployeeAdd,

        [Description("enableESRDiagnostics")]
        EnableEsrDiagnostics,

        [Description("esrAutoActivateCar")]
        EsrAutoActivateCar,

        [Description("SummaryESRInboundFile")]
        SummaryEsrInboundFile,

        [Description("EsrRounding")]
        EsrRounding,

        [Description("ESRManualSupervisorAssignment")]
        EnableESRManualAssignmentSupervisor,

        [Description("esrPrimaryAddressOnly")]
        EsrPrimaryAddressOnly,

        [Description("allowViewFundDetails")]
        AllowViewFundDetails,

        [Description("allowExpenseItemsLessThanTheReceiptTotalToPassValidation")]
        AllowReceiptTotalToPassValidation,

        [Description("flagMessage")]
        FlagMessage,

        [Description("documentRepository")]
        DocumentRepository,

        [Description("openSaveAttachments")]
        OpenSaveAttachments,

        [Description("hyperlinkAttachmentsEnabled")]
        EnableAttachmentHyperlink,

        [Description("attachmentUploadEnabled")]
        EnableAttachmentUpload,

        [Description("linkAttachmentDefault")]
        LinkAttachmentDefault,

        [Description("maxUploadSize")]
        MaxUploadSize,

        [Description("allowMenuAddContract")]
        AllowMenuContractAdd,

        [Description("flashingNotesIconEnabled")]
        EnableFlashingNotesIcon,

        [Description("contractNumUpdateEnabled")]
        EnableContractNumUpdate,

        [Description("autoUpdateCV")]
        AutoUpdateAnnualContractValue,

        [Description("contractKey")]
        ContractKey,

        [Description("rechargeUnrecoveredTitle")]
        RechargeUnrecoveredTitle,

        [Description("autoUpdateCVRechargeLive")]
        AutoUpdateCVRechargeLive,

        [Description("penaltyClauseTitle")]
        PenaltyClauseTitle,

        [Description("contractScheduleDefault")]
        ContractScheduleDefault,

        [Description("useCPExtraInfo")]
        UseCPExtraInfo,

        [Description("enableRecharge")]
        EnableRecharge,

        [Description("contractDatesMandatory")]
        ContractDatesMandatory,

        [Description("autoUpdateLicenceTotal")]
        AutoUpdateLicenceTotal,

        [Description("contractCategoryTitle")]
        ContractCategoryTitle,

        [Description("inflatorActive")]
        InflatorActive,

        [Description("invoiceFrequencyActive")]
        InvoiceFreqActive,

        [Description("termTypeActive")]
        TermTypeActive,

        [Description("valueComments")]
        ValueComments,

        [Description("contractDescTitle")]
        ContractDescTitle,

        [Description("contractDescShortTitle")]
        ContractDescShortTitle,

        [Description("contractNumGen")]
        ContractNumGen,

        [Description("contractNumSeq")]
        ContractNumSeq,

        [Description("contractCategoryMandatory")]
        ContractCatMandatory,

        [Description("poNumberGenerate")]
        PONumberGenerate,

        [Description("poNumberSequence")]
        PONumberSequence,

        [Description("poNumberFormat")]
        PONumberFormat,

        [Description("keepInvoiceForecasts")]
        KeepInvoiceForecasts,

        [Description("rechargeReferenceAs")]
        ReferenceAs,

        [Description("employeeReferenceAs")]
        StaffRepAs,

        [Description("rechargePeriod")]
        RechargePeriod,

        [Description("financialYearCommences")]
        FinYearCommence,

        [Description("cpDeleteAction")]
        CPDeleteAction,

        [Description("variationAutoSeqEnabled")]
        EnableVariationAutoSeq,

        [Description("showProductInSearch")]
        ShowProductInSearch,

        [Description("supplierStatusEnforced")]
        StatusEnforced,

        [Description("supplierLastFinStatusEnabled")]
        LastFinStatusEnabled,

        [Description("supplierLastFinCheckEnabled")]
        LastFinCheckEnabled,

        [Description("supplierFYEEnabled")]
        FYEEnabled,

        [Description("supplierNumEmployeesEnabled")]
        NumEmployeesEnabled,

        [Description("supplierTurnoverEnabled")]
        TurnoverEnabled,

        [Description("supplierIntContactEnabled")]
        IntContactEnabled,

        [Description("supplierCategoryTitle")]
        CatTitle,

        [Description("supplierRegionTitle")]
        RegionTitle,

        [Description("supplierPrimaryTitle")]
        PrimaryTitle,

        [Description("supplierVariationTitle")]
        VariationTitle,

        [Description("supplierCategoryMandatory")]
        CatMandatory,

        [Description("taskEscalationRepeat")]
        TaskEscalationRepeat,

        [Description("taskStartDateMandatory")]
        TaskStartDateMandatory,

        [Description("taskEndDateMandatory")]
        TaskEndDateMandatory,

        [Description("taskDueDateMandatory")]
        TaskDueDateMandatory,

        [Description("customerHelpInformation")]
        CustomerHelpInformation,

        [Description("customerHelpContactName")]
        CustomerHelpContactName,

        [Description("customerHelpContactTelephone")]
        CustomerHelpContactTelephone,

        [Description("customerHelpContactFax")]
        CustomerHelpContactFax,

        [Description("customerHelpContactAddress")]
        CustomerHelpContactAddress,

        [Description("customerHelpContactEmailAddress")]
        CustomerHelpContactEmailAddress,

        [Description("EnableInternalSupportTickets")]
        EnableInternalSupportTickets,

        [Description("mandatoryPostcodeForAddresses")]
        MandatoryPostcodeForAddresses,

        [Description("odometerDay")]
        OdometerDay,

        [Description("addLocations")]
        AddLocations,

        [Description("enterOdometerOnSubmit")]
        EnterOdometerOnSubmit,

        [Description("allowMultipleDestinations")]
        AllowMultipleDestinations,

        [Description("useMapPoint")]
        UseMapPoint,

        [Description("homeToOffice")]
        HomeToOffice,

        [Description("autoCalcHomeToLocation")]
        AutoCalcHomeToLocation,

        [Description("mileageCalcType")]
        MileageCalcType,

        [Description("recordOdometer")]
        RecordOdometer,

        [Description("mileage")] Mileage,

        [Description("useMobileDevices")]
        UseMobileDevices,

        [Description("allowEmployeeToNotifyOfChangeOfDetails")]
        AllowEmployeeToNotifyOfChangeOfDetails,

        [Description("editMyDetails")]
        EditMyDetails,
        [Description("pwdMCN")]
        PwdMustContainNumbers,

        [Description("pwdMCU")]
        PwdMustContainUpperCase,

        [Description("pwdSymbol")]
        PwdMustContainSymbol,

        [Description("pwdExpires")]
        PwdExpires,

        [Description("pwdExpiryDays")]
        PwdExpiryDays,

        [Description("pwdConstraint")]
        PwdConstraint,

        [Description("pwdLength1")]
        PwdLength1,

        [Description("pwdLength2")]
        PwdLength2,

        [Description("pwdMaxRetries")]
        PwdMaxRetries,

        [Description("pwdHistoryNum")]
        PwdHistoryNum,

        [Description("globalLocaleID")]
        GlobalLocaleID,

        [Description("drilldownReport")]
        DrilldownReport,

        [Description("allowSelfReg")]
        AllowSelfReg,

        [Description("allowSelfRegRole")]
        AllowSelfRegRole,

        [Description("allowSelfRegItemRole")]
        AllowSelfRegItemRole,

        [Description("allowSelfRegEmpContact")]
        AllowSelfRegEmployeeContact,

        [Description("allowSelfRegHomeAddress")]
        AllowSelfRegHomeAddress,

        [Description("allowSelfRegEmpInfo")]
        AllowSelfRegEmployeeInfo,

        [Description("allowSelfRegSignOff")]
        AllowSelfRegSignOff,

        [Description("allowSelfRegAdvancesSignOff")]
        AllowSelfRegAdvancesSignOff,

        [Description("allowSelfRegDeptCostCode")]
        AllowSelfRegDepartmentCostCode,

        [Description("allowSelfRegBankDetails")]
        AllowSelfRegBankDetails,

        [Description("allowSelfRegCarDetails")]
        AllowSelfRegCarDetails,

        [Description("allowSelfRegUDF")]
        AllowSelfRegUDF,

        [Description("defaultRole")]
        DefaultRole,

        [Description("defaultItemRole")]
        DefaultItemRole,

        [Description("idleTimeout")]
        IdleTimeout,

        [Description("countdownTimer")] CountdownTimer,

        [Description("NotifyWhenEnvelopeNotReceivedDays")]
        NotifyWhenEnvelopeNotReceivedDays,

        [Description("enableCalculationsForAllocatingFuelReceiptVATtoMileage")]
        EnableCalculationsForAllocatingFuelReceiptVatToMileage,

        [Description("cacheTimeout")]
        CacheTimeout,

        [Description("cachePeriodShort")]
        CachePeriodShort,

        [Description("cachePeriodNormal")]
        CachePeriodNormal,

        [Description("cachePeriodLong")]
        CachePeriodLong,

        [Description("corporateDiligenceStartPage")]
        CorporateDStartPage,

        [Description("defaultPageSize")]
        DefaultPageSize,

        [Description("FYStarts")]
        FYStarts,

        [Description("FYEnds")]
        FYEnds,

        [Description("showReviews")]
        ShowReviews,

        [Description("sendReviewRequests")]
        SendReviewRequests,

        [Description("allowArchivedNotesAdd")]
        AllowArchivedNotesAdd,

        [Description("EnableClaimApprovalReminders")]
        EnableClaimApprovalReminders,

        [Description("ClaimApprovalReminderFrequency")]
        ClaimApprovalReminderFrequency,

        [Description("EnableCurrentClaimsReminders")]
        EnableCurrentClaimsReminders,

        [Description("CurrentClaimsReminderFrequency")]
        CurrentClaimsReminderFrequency
    }
}
