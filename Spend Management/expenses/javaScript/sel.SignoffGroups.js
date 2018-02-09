(function(SEL, moduleNameHTML, appPath)
{
    var scriptName = "SignoffGroups";

    function execute()
    {
        SEL.registerNamespace("SEL.SignoffGroups");
        SEL.SignoffGroups =
        {
            IDs: {
                SignoffTypes: {
                    None: "0",
                    BudgetHolder: "1",
                    Employee: "2",
                    Team: "3",
                    LineManager: "4",
                    ClaimantSelectsOwnChecker: "5",
                    ApprovalMatrix: "6",
                    DeterminedByClaimantFromApprovalMatrix: "7",
                    CostCodeOwner: "8",
                    AssignmentSupervisor: "9",
                    SELScanAttach: "100",
                    SELValidation: "101"
                },

                StageInclusionType: {
                    Always: "1",
                    ClaimTotalExceeds: "2",
                    ExpenseItemExceeds: "3",
                    IncludesCostCode: "4",
                    ClaimTotalBelow: "5",
                    IncludesExpenseItem: "6",
                    OlderThanDays: "7",
                    IncludesDepartment: "8",
                    ValidationFailedTwice: "9"
                },

                OnHolidayType: {
                    NoAction: "1",
                    SkipStage: "2",
                    AssignToSomeoneElse: "3"
                },

                HolidayApproverType: {
                    BudgetHolder: "1",
                    Employee: "2",
                    Team: "3",
                    LineManager: "4"
                },

                Action: {
                    NotifyUser: "1",
                    UserChecksClaim: "2"
                }
            },

            DomIDs:
            {
                SignoffGroup: {
                    ShowStageModal: false,
                    GroupId: 0,
                    GroupName: null,
                    GroupDescription: null,
                    OneClickAuth: null,
                    ClaimIsInProgress: null
                },

                SignoffStage:
                {
                    StageId: 0,
                    RelId: 0,
                    HolidayListId: 0,
                    IncludeId: 0,
                    SignoffDropdown: null,
                    SignoffValuesDropdown: null,
                    SignoffValuesLabel: null,
                    HolidayDropdown: null,
                    HolidayListDropdown: null,
                    HolidayListLabel: null,
                    OnHolidayDiv: null,
                    HolidayTypeDropdown: null,
                    HoliayTypeLabel: null,
                    CostCodeOwnerDiv: null,
                    CostCodeOwnerLabel: null,
                    CostCodeOwnerDropDown: null,
                    ScanAttachDiv: null,
                    EnvelopeReceivedLabel: null,
                    EnvelopeReceivedCheckBox: null,
                    EnvelopeReceivedTooltip: null,
                    EnvelopeNotReceivedLabel: null,
                    EnvelopeNotReceivedCheckBox: null,
                    EnvelopeNotReceivedTooltip: null,
                    ApprovalMatrixDiv: null,
                    ExtraLevelsLabel: null,
                    ExtraLevelsTextBox: null,
                    ExtraLevelsTooptip: null,
                    FromMyLevelLabel: null,
                    FromMyLevelCheckbox: null,
                    IncludeDropDown: null,
                    AmountLabel: null,
                    AmountTextBox: null,
                    AmountDropDown: null,
                    InvlolvementDropDown: null,
                    ApproverEmailCheckbox: null,
                    ClaimantEmailCheckbox: null,
                    SingleSignoffCheckbox: null,
                    DeclarationCheckbox: null,
                    ApproverJustificationCheckbox: null,
                    Modal: null,
                    AddStageDiv: null,
                    EditValues: null
                },

                Validators: {
                    cmblist: null,
                    cmbholidaylist: null,
                    DropDownLineManagerAssignmentSupervisor: null,
                    txtExtraLevels: null,
                    txtamountRequired: null,
                    txtamountCompare: null
                }
            },

            SetupDialogsStage: function () {
                SEL.SignoffGroups.DomIDs.SignoffStage.Modal = $("#AddEdiStageModal");
                SEL.SignoffGroups.DomIDs.SignoffStage.AddStageDiv = $("#divAddStage");
                SEL.SignoffGroups.DomIDs.SignoffStage.OnHolidayDiv = $("#divOnHoliday");
                SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDiv = $("#divCostCodeOwner");
                SEL.SignoffGroups.DomIDs.SignoffStage.ScanAttachDiv = $("#divScanAttach");
                SEL.SignoffGroups.DomIDs.SignoffStage.ApprovalMatrixDiv = $("#divApprovalMatrix");
                SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeReceivedTooltip = $("#imgtooltip3230");
                SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeNotReceivedTooltip = $("#imgtooltip3231");
                SEL.SignoffGroups.DomIDs.SignoffStage.ExtraLevelsTooptip = $("#imgtooltip589");

                if ($(SEL.SignoffGroups.DomIDs.SignoffGroup.ClaimIsInProgress).is(":visible")) {
                    SEL.SignoffGroups.DomIDs.SignoffStage.AddStageDiv.hide();
                }

                if (SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId === "") {
                    SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId = 0;
                }

                SEL.SignoffGroups.SignoffStage.SetupModal();
                SEL.SignoffGroups.SignoffStage.PopulateSignOffValuesDropdown(1);
            },

            SetupEnterKeyBindings: function()
            {
                // Base Save
                SEL.Common.BindEnterKeyForSelector('.primaryPage', SEL.Template.TemplateItem.Save);
                // Sub Item Save
                SEL.Common.BindEnterKeyForSelector('#' + SEL.Template.DomIDs.TemplateSubItem.Panel, SEL.Template.TemplateSubItem.Save);
                
                $(document).keydown(function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        e.preventDefault();
                        if ($g(SEL.Template.DomIDs.TemplateSubItem.Panel).style.display == '')
                        {
                            SEL.Template.TemplateSubItem.Cancel();
                            return;
                        }
                    }
                });
            },
            
            SignoffGroup:
            {
                Delete: function(groupId)
                {
                    if (confirm('Are you sure you want to delete this Signoff Group?'))
                    {
                        SEL.Data.Ajax({
                            data: {
                                groupId: groupId
                            },
                            url: '/expenses/webservices/SignoffGroups.asmx/DeleteGroup',
                            success: function(r)
                            {
                                switch (r.d) {
                                    case -1:
                                        SEL.MasterPopup.ShowMasterPopup('The signoff group cannot be deleted as it is assigned to an employee(s)', 'Message from ' + moduleNameHTML);
                                        break;
                                    case -2:
                                        SEL.MasterPopup.ShowMasterPopup('The signoff group cannot be deleted as it is assigned to an employee(s) advance group', 'Message from ' + moduleNameHTML);
                                        break;
                                    case -3:
                                        SEL.MasterPopup.ShowMasterPopup('You do not have permission to delete the selected Signoff Group', 'Message from ' + moduleNameHTML);
                                        break;
                                    case -10:
                                        SEL.MasterPopup.ShowMasterPopup('The signoff group cannot be deleted as it is assigned to one or more GreenLights or user defined field records', 'Message from ' + moduleNameHTML);
                                        break;
                                    default:
                                        SEL.SignoffGroups.SignoffGroup.RefreshGrid();
                                        break;
                                }
                            },
                            error: function(xmlHttpRequest, textStatus, errorThrown)
                            {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },
                
                Save: function ()
                {
                    if (validateform('vgMain') === false)
                    {
                        return false;
                    }

                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;
                    var groupName = $(SEL.SignoffGroups.DomIDs.SignoffGroup.GroupName).val();
                    var groupDescription = $(SEL.SignoffGroups.DomIDs.SignoffGroup.GroupDescription).val();
                    var oneClickAuth = $(SEL.SignoffGroups.DomIDs.SignoffGroup.OneClickAuth).is(':checked');
                    var validate = !SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal;

                    if (groupId === 0) {
                        validate = false;
                    }

                    if (groupId === 0 && !SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal) {
                        validate = true;
                    }


                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId,
                            groupName: groupName,
                            groupDescription: groupDescription,
                            oneClickAuth: oneClickAuth,
                            validate: validate
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/SaveGroup',
                        success: function (r) {
                            if (r.d.GroupId === -1) {
                                SEL.MasterPopup.ShowMasterPopup('A Signoff Group with this name already exists.',
                                    'Message from ' + moduleNameHTML);
                            } else if (r.d.GroupId === -2)
                            {
                                SEL.MasterPopup.ShowMasterPopup('You do not have permission to add a Signoff Group',
                                    'Message from ' + moduleNameHTML);
                            } else {

                                if (SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId === 0) {
                                    SEL.Grid.updateGridQueryFilterValues("SignoffStagesGrid",
                                        "161f5786-187b-4bc9-8635-27780bc28321",
                                        [r.d.GroupId],
                                        []);
                                }

                                if (r.d.ValidationMessages === "") {
                                    SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId = r.d.GroupId;

                                    if (SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal) {
                                        SEL.SignoffGroups.SignoffStage.PopulateSignoffTypeDropdown();
                                        SEL.SignoffGroups.SignoffStage.Modal.Show();
                                        if (SEL.SignoffGroups.DomIDs.SignoffStage.StageId > 0) {
                                            SEL.SignoffGroups.SignoffStage.Edit();
                                        } else {
                                            SEL.SignoffGroups.SignoffStage.Add();
                                        }
                                        SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal = false;
                                    } else {
                                        document.location = "/expenses/admin/SignoffGroups.aspx";
                                    }
                                } else {
                                    SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId = r.d.GroupId;
                                    SEL.MasterPopup.ShowMasterPopup(r.d.ValidationMessages,
                                        'Message from ' + moduleNameHTML);
                                }
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown)
                        {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                Cancel: function()
                {
                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;

                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/ValidateGroup',
                        success: function (r) {
                            if (parseInt(r.d) === -1) {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to view this Signoff group",
                                    'Message from ' + moduleNameHTML);
                            } else if (r.d === "") {
                                document.location = "/expenses/admin/SignoffGroups.aspx";
                            } else {
                                SEL.MasterPopup.ShowMasterPopup(r.d,
                                    'Message from ' + moduleNameHTML);
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('SignoffGroupsGrid', SEL.Grid.getCurrentPageNum('SignoffGroupsGrid'));
                }
            },
            
            SignoffStage:
            {
                SetupModal: function () {
                    SEL.SignoffGroups.DomIDs.SignoffStage.Modal.dialog({
                        autoOpen: false,
                        resizable: false,
                        title: "Message from " + window.moduleNameHTML,
                        width:900,
                        modal: true,
                        dialogClass: "ui-no-close-button",
                        buttons: [
                            {
                                text: "save",
                                id: "btnSave",
                                "class": "jQueryUIButton",
                                click: function () {
                                    SEL.SignoffGroups.SignoffStage.Save();
                                }
                            }, {
                                text: "cancel",
                                id: "btnCancel",
                                "class": "jQueryUIButton",
                                click: function () {
                                    SEL.SignoffGroups.SignoffStage.Modal.Hide();
                                }
                            }
                        ]
                    });
                },

                SignoffTypeDropdownOnChange: function () {
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.cmblist), false);
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.DropDownLineManagerAssignmentSupervisor), false);
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.txtExtraLevels), false);
                    SEL.SignoffGroups.SignoffStage.Modal.Reset();
                    var value = $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).val();
                    switch (value) {
                        case SEL.SignoffGroups.IDs.SignoffTypes.BudgetHolder:
                        case SEL.SignoffGroups.IDs.SignoffTypes.Employee:
                        case SEL.SignoffGroups.IDs.SignoffTypes.Team:
                        case SEL.SignoffGroups.IDs.SignoffTypes.ApprovalMatrix:
                            $g(SEL.SignoffGroups.DomIDs.Validators.cmblist).enabled = true;
                            SEL.SignoffGroups.SignoffStage.PopulateSignOffValuesDropdown(value);
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.LineManager:
                        case SEL.SignoffGroups.IDs.SignoffTypes.ClaimantSelectsOwnChecker:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).hide();
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.DeterminedByClaimantFromApprovalMatrix:
                            $g(SEL.SignoffGroups.DomIDs.Validators.txtExtraLevels).enabled = true;
                            SEL.SignoffGroups.SignoffStage.PopulateSignOffValuesDropdown(value);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApprovalMatrixDiv).show();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val(
                                SEL.SignoffGroups.IDs.OnHolidayType.NoAction).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val(
                                SEL.SignoffGroups.IDs.HolidayApproverType.BudgetHolder).change();
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.CostCodeOwner:
                            $g(SEL.SignoffGroups.DomIDs.Validators.DropDownLineManagerAssignmentSupervisor).enabled = true;
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDiv).show();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop('checked', false);
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.AssignmentSupervisor:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop('checked', false);
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.SELScanAttach:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ScanAttachDiv).show();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val(
                                SEL.SignoffGroups.IDs.StageInclusionType.Always).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).val(
                                SEL.SignoffGroups.IDs.Action.UserChecksClaim);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val(
                                SEL.SignoffGroups.IDs.OnHolidayType.NoAction).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val(
                                SEL.SignoffGroups.IDs.HolidayApproverType.BudgetHolder).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ClaimantEmailCheckbox).prop('checked', true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDiv).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).hide();
                            break;
                        case SEL.SignoffGroups.IDs.SignoffTypes.SELValidation:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop('checked', false);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val(
                                SEL.SignoffGroups.IDs.StageInclusionType.Always).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).val(
                                SEL.SignoffGroups.IDs.Action.UserChecksClaim);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val(
                                SEL.SignoffGroups.IDs.OnHolidayType.NoAction).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val(
                                SEL.SignoffGroups.IDs.HolidayApproverType.BudgetHolder).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ClaimantEmailCheckbox).prop('checked', true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).hide();
                            break;
                    }
                },

                OnHolidayOnChange: function () {
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.cmbholidaylist), false);
                    switch ($(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val()) {
                        case SEL.SignoffGroups.IDs.OnHolidayType.NoAction:
                        case SEL.SignoffGroups.IDs.OnHolidayType.SkipStage:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.OnHolidayDiv).hide();
                            break;
                        case SEL.SignoffGroups.IDs.OnHolidayType.AssignToSomeoneElse:
                            $g(SEL.SignoffGroups.DomIDs.Validators.cmbholidaylist).enabled = true;
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.OnHolidayDiv).show();
                            break;
                    }
                },

                WhenToIncludeOnChange: function () {
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.txtamountCompare), false);
                    window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.txtamountRequired), false);
                    var value = $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val();
                    switch (value) {
                        case SEL.SignoffGroups.IDs.StageInclusionType.Always:
                        case SEL.SignoffGroups.IDs.StageInclusionType.ValidationFailedTwice:
                        case SEL.SignoffGroups.IDs.StageInclusionType.ExpenseItemExceeds:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).hide();
                            break;
                        case SEL.SignoffGroups.IDs.StageInclusionType.ClaimTotalExceeds:
                        case SEL.SignoffGroups.IDs.StageInclusionType.ClaimTotalBelow:
                        case SEL.SignoffGroups.IDs.StageInclusionType.OlderThanDays:
                            $g(SEL.SignoffGroups.DomIDs.Validators.txtamountCompare).enabled = true;
                            $g(SEL.SignoffGroups.DomIDs.Validators.txtamountRequired).enabled = true;
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).show();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).hide();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).show();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).text("Amount");
                            break;
                        case SEL.SignoffGroups.IDs.StageInclusionType.IncludesCostCode:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).text("Cost Code");
                            SEL.SignoffGroups.SignoffStage.SetupWhenToIncludeUI(value);
                            break;
                        case SEL.SignoffGroups.IDs.StageInclusionType.IncludesExpenseItem:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).text("Expense item");
                            SEL.SignoffGroups.SignoffStage.SetupWhenToIncludeUI(value);
                            break;
                        case SEL.SignoffGroups.IDs.StageInclusionType.IncludesDepartment:
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).text("Department");
                            SEL.SignoffGroups.SignoffStage.SetupWhenToIncludeUI(value);
                            break;
                    }
                },

                SetupWhenToIncludeUI: function (value) {
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).hide();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).show();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountLabel).show();
                    SEL.SignoffGroups.SignoffStage.PopulateIncludeDropdown(value);
                },

                ApproverJustificationOnChange: function () {
                    if ($(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).val() !== 
                        SEL.SignoffGroups.IDs.SignoffTypes.CostCodeOwner &&
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).val() !==
                        SEL.SignoffGroups.IDs.SignoffTypes.AssignmentSupervisor) {
                        if ($(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).is(':checked')) {
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", true);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("checked", false);
                        } else {
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", false);
                        }
                    }
                },

                OneClickSignOffOnChange: function () {
                    if ($(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).is(':checked')) {
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("disabled", true);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop('checked', false);
                    } else {
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("disabled", false);
                    }
                },

                HolidayApproverOnChange: function () {
                    $g(SEL.SignoffGroups.DomIDs.Validators.cmbholidaylist).enabled = true;
                    var holidayTypeId = $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val();

                    if (holidayTypeId === SEL.SignoffGroups.IDs.HolidayApproverType.LineManager) {
                        window.ValidatorEnable($g(SEL.SignoffGroups.DomIDs.Validators.cmbholidaylist), false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown).hide();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListLabel).hide();
                        return;
                    }

                    $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown).show();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListLabel).show();

                    SEL.Data.Ajax({
                        data: {
                            holidayTypeId: holidayTypeId
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/PopulateHolidayApproverDropdown',
                        success: function (r) {
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown).find('option').remove();

                            $.each(r.d, function (i, items) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown).append(new Option(items.Text, items.Value));
                            });

                            if (SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListId > 0) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown)
                                    .val(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListId);
                                SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListId = 0;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                PopulateIncludeDropdown: function (value) {
                    SEL.Data.Ajax({
                        data: {
                            inclusionId: value
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/PopulateIncludeDropdown',
                        success: function (r) {
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).find('option').remove();

                            $.each(r.d, function (i, items) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).append(new Option(items.Text, items.Value));
                            });

                            if (SEL.SignoffGroups.DomIDs.SignoffStage.IncludeId > 0) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown)
                                    .val(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeId);
                                SEL.SignoffGroups.DomIDs.SignoffStage.IncludeId = 0;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                PopulateSignOffValuesDropdown: function (value) {
                    SEL.Data.Ajax({
                        data: {
                            signoffId: value
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/PopulateSignoffValueDropdown',
                        success: function (r) {
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).find('option').remove();

                            $.each(r.d, function (i, items) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).append(new Option(items.Text, items.Value));
                            });

                            if (SEL.SignoffGroups.DomIDs.SignoffStage.RelId > 0) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown)
                                    .val(SEL.SignoffGroups.DomIDs.SignoffStage.RelId);
                                SEL.SignoffGroups.DomIDs.SignoffStage.RelId = 0;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }});
                },

                PopulateSignoffTypeDropdown: function () {
                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;
                    var stageId = SEL.SignoffGroups.DomIDs.SignoffStage.StageId;

                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId,
                            stageId: stageId
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/PopulateSignoffTypeDropdown',
                        success: function (r) {
                            $.each(r.d, function (i, items) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).append(new Option(items.Text, items.Value));
                            });

                            SEL.SignoffGroups.SignoffStage.SortSignoffTypeDropDown();
                            if (SEL.SignoffGroups.DomIDs.SignoffStage.StageId === 0) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).get(0).selectedIndex = 0;
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown)
                                    .val(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).change();
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                Save: function ()
                {
                    if (validateform('vgStage') === false) {
                        return false;
                    }

                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;
                    var stageId = SEL.SignoffGroups.DomIDs.SignoffStage.StageId;
                    var singleSignoff = $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).is(':checked');
                    var sendClaimantEmail = $(SEL.SignoffGroups.DomIDs.SignoffStage.ClaimantEmailCheckbox).is(':checked');
                    var sendApproverEmail = $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).is(':checked');
                    var envelopesAreReceived = $(SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeReceivedCheckBox).is(":checked");
                    var envelopesAreNotReceived = $(SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeNotReceivedCheckBox).is(":checked");
                    var displayApproverDeclaration = $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).is(":checked");
                    var approverJustificationRequired = $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).is(":checked");
                    var aboveMyLevel = $(SEL.SignoffGroups.DomIDs.SignoffStage.FromMyLevelCheckbox).is(":checked");
                    var approverMatrixLevels = $(SEL.SignoffGroups.DomIDs.SignoffStage.ExtraLevelsTextBox).val();
                    var selectedSignoffType = $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).val();
                    var selectedSignoffValue = $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).val();
                    var noCostCodeOwnerAction = $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDropDown).val();
                    var stageInclusionType = $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val();
                    var stageInclusionValue = $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).val();
                    var stageInclusionDropdownType = $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountDropDown).val();
                    var involvementType = $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).val();
                    var onHolidayType = $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val();
                    var holidayApproverType = $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val();
                    var holidayApproverValue = $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListDropdown).val();

                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId,
                            stageId: stageId,
                            selectedSignoffType: selectedSignoffType,
                            singleSignoff: singleSignoff,
                            sendClaimantEmail: sendClaimantEmail,
                            sendApproverEmail: sendApproverEmail,
                            envelopesAreReceived: envelopesAreReceived,
                            envelopesAreNotReceived: envelopesAreNotReceived,
                            displayApproverDeclaration: displayApproverDeclaration,
                            approverJustificationRequired: approverJustificationRequired,
                            aboveMyLevel: aboveMyLevel,
                            approverMatrixLevels: approverMatrixLevels,
                            selectedSignoffValue: selectedSignoffValue,
                            noCostCodeOwnerAction: noCostCodeOwnerAction,
                            stageInclusionType: stageInclusionType,
                            stageInclusionValue: stageInclusionValue,
                            stageInclusionDropdownType: stageInclusionDropdownType,
                            involvementType: involvementType,
                            onHolidayType: onHolidayType,
                            holidayApproverType: holidayApproverType,
                            holidayApproverValue: holidayApproverValue
                },
                        url: '/expenses/webservices/SignoffGroups.asmx/SaveStage',
                        success: function(r)
                        {
                            if (r.d > 0) {
                                SEL.SignoffGroups.SignoffStage.RefreshGrid();
                                SEL.SignoffGroups.SignoffStage.Modal.Hide();
                            } else {
                                if (r.d === -1) {
                                    SEL.MasterPopup.ShowMasterPopup('You do not have permission to add this selected Signoff Stage',
                                        'Message from ' + moduleNameHTML);
                                }
                            }
                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown)
                        {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },
                
                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('SignoffStagesGrid');
                },

                Delete: function (stageId) {
                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;

                    if (confirm('Are you sure you wish to delete the selected stage?')) {
                        SEL.Data.Ajax({
                            data: {
                                groupId: groupId,
                                stageId: stageId
                            },
                            url: '/expenses/webservices/SignoffGroups.asmx/DeleteStage',
                            success: function (r) {
                                if (r.d === -1) {
                                    SEL.MasterPopup.ShowMasterPopup(
                                        'You do not have permission to delete this selected Signoff Stage',
                                        'Message from ' + moduleNameHTML);
                                } else {
                                    SEL.SignoffGroups.SignoffStage.RefreshGrid();
                                }
                            },
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },

                Edit: function () {
                    var groupId = SEL.SignoffGroups.DomIDs.SignoffGroup.GroupId;
                    var stageId = SEL.SignoffGroups.DomIDs.SignoffStage.StageId;
                    $("#ui-id-1").text("Edit Stage");

                    //get stage
                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId,
                            stageId: stageId
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/GetStage',
                        success: function (r) {
                            if (r.d === "undefined") {
                                SEL.MasterPopup.ShowMasterPopup('You do not have permission to view this selected Signoff Stage',
                                    'Message from ' + moduleNameHTML);
                            }

                            SEL.SignoffGroups.DomIDs.SignoffStage.RelId = r.d.relid;
                            SEL.SignoffGroups.DomIDs.SignoffStage.IncludeId = r.d.includeid;
                            SEL.SignoffGroups.DomIDs.SignoffStage.HolidayListId = r.d.holidayid;
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).val(r.d.signofftype).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val(r.d.include).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val(r.d.onholiday).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val(r.d.holidaytype).change();
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop('checked', r.d.sendmail);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.ClaimantEmailCheckbox).prop('checked', r.d.claimantmail);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop('checked', r.d.displaydeclaration);
                            if (r.d.signofftype ==
                                SEL.SignoffGroups.IDs.SignoffTypes.DeterminedByClaimantFromApprovalMatrix) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.ExtraLevelsTextBox)
                                    .val(r.d.ExtraApprovalLevels);
                            } else {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.ExtraLevelsTextBox).val("");
                            }
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.FromMyLevelCheckbox).prop('checked', r.d.FromMyLevel);
                            if (r.d.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDropDown).val("AssignmentSupervisor");
                            } else {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDropDown).val("LineManager");
                            }
                            switch ($(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val()) {
                                case SEL.SignoffGroups.IDs.StageInclusionType.ClaimTotalBelow:
                                case SEL.SignoffGroups.IDs.StageInclusionType.ClaimTotalExceeds:
                                case SEL.SignoffGroups.IDs.StageInclusionType.OlderThanDays:
                                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).val(r.d.amount);
                                    break;
                                default:
                                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).val("");
                                    break;
                            }
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).val(r.d.notify);
                            if (r.d.signofftype != SEL.SignoffGroups.IDs.SignoffTypes.SELScanAttach
                                && r.d.signofftype != SEL.SignoffGroups.IDs.SignoffTypes.SELValidation) {
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop('checked', r.d.ApproverJustificationsRequired).change();
                                $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop('checked', r.d.singlesignoff).change();
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });

                    //get group
                    SEL.Data.Ajax({
                        data: {
                            groupId: groupId
                        },
                        url: '/expenses/webservices/SignoffGroups.asmx/GetGroup',
                        success: function (r) {
                            if (r.d === "undefined") {
                                SEL.MasterPopup.ShowMasterPopup('You do not have permission to view this selected Signoff Group',
                                    'Message from ' + moduleNameHTML);
                            }
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeReceivedCheckBox).prop('checked', r.d.NotifyClaimantWhenEnvelopeNotReceived);
                            $(SEL.SignoffGroups.DomIDs.SignoffStage.EnvelopeNotReceivedCheckBox).prop('checked', r.d.NotifyClaimantWhenEnvelopeReceived);
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });

                },
                
                SortSignoffTypeDropDown: function() {
                    var select = $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown);
                    select.html(select.find('option').sort(function (x, y) {
                        // to change to descending order switch "<" for ">"
                        return $(x).text() > $(y).text() ? 1 : -1;
                    }));
                },

                Add: function () {
                    $("#ui-id-1").text("New Stage");

                    $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).val(SEL.SignoffGroups.IDs.StageInclusionType.Always).change();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).val(SEL.SignoffGroups.IDs.OnHolidayType.NoAction).change();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.AmountTextBox).val("");
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.ExtraLevelsTextBox).val("");
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayTypeDropdown).val(SEL.SignoffGroups.IDs.HolidayApproverType.BudgetHolder).change();
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("checked", false);
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop("checked", false);
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.ClaimantEmailCheckbox).prop("checked", false);
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop("checked", false);
                    $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("checked", false);
                },
                
                Modal:
                {
                    Show: function () {
                        SEL.SignoffGroups.SignoffStage.SetupModal();
                        SEL.SignoffGroups.DomIDs.SignoffStage.Modal.dialog("open");
                    },
                    
                    Hide: function() {
                        SEL.SignoffGroups.SignoffStage.Modal.Clear();
                        SEL.SignoffGroups.DomIDs.SignoffStage.Modal.dialog("close");
                    },

                    Clear: function() {
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).find('option[value=0]').remove();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).find('option[value=5]').remove();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).find('option[value=6]').remove();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffDropdown).find('option[value=7]').remove();
                        SEL.SignoffGroups.SignoffStage.Modal.Reset();
                    },

                    Reset: function () {
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesDropdown).show();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SignoffValuesLabel).show();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.CostCodeOwnerDiv).hide();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApprovalMatrixDiv).hide();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.FromMyLevelCheckbox).prop("checked", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ScanAttachDiv).hide();
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.SingleSignoffCheckbox).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverEmailCheckbox).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.DeclarationCheckbox).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.ApproverJustificationCheckbox).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.HolidayDropdown).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.IncludeDropDown).prop("disabled", false);
                        $(SEL.SignoffGroups.DomIDs.SignoffStage.InvlolvementDropDown).prop("disabled", false);
                    }
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}(SEL, moduleNameHTML, appPath));