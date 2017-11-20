(function (SEL, $, $g, $f, $e, $ddlValue, $ddlText, $ddlSetSelected, cu, ValidatorEnable, ValidatorUpdateDisplay, moduleNameHTML)
{
    var scriptName = "CustomEntityAdministration";

    function execute()
    {
        SEL.registerNamespace("SEL.CustomEntityAdministration");

        SEL.CustomEntityAdministration = {
            /* Entity Information */
            IDs:
            {
                Entity: -1,
                Attribute: -1,
                Form: -1,
                View: -1,
                Sort:
                {
                    Column: null,
                    Direction: null
                },
                DateFormat: 'dd/mm/yy',
                Relationship:
                {
                    RelationshipType: null,
                    RelatedTable: null,
                    SelectedDisplayField: null,
                    SelectedMatchFields: null,
                    SelectedRelationshipEntity: null,
                    SelectedViewID: null,
                    SelectedAutocompleteDisplayFields: null
                }
            },

            /* Dom ID's */
            DomIDs:
            {
                Base: {
                    FormSelectionAttribute: null
                },

                Attributes:
                {
                    Grid: null,
                    Summary:
                    {
                        Modal:
                        {
                            Control: null,
                            Panel: null,
                            PanelHeader: null,
                            TabContainer: null,
                            General:
                            {
                                Tab: null,
                                Name: null,
                                Description: null,
                                SummarySource: null
                            },
                            Relationships:
                            {
                                Tab: null,
                                Control: null
                            },
                            Columns:
                            {
                                Tab: null,
                                Control: null
                            },
                            DisplayFieldModal:
                            {
                                Control: null,
                                Panel: null,
                                Tree: null
                            }
                        }
                    },
                    Relationship:
                    {
                        NtoOne:
                        {
                            Modal:
                            {
                                Control: null,
                                Panel: null,
                                PanelHeader: null,
                                TabContainer: null,
                                General:
                                {
                                    Tab: null,
                                    RelationshipName: null,
                                    RelationshipNameReqValidator: null,
                                    Description: null,
                                    Mandatory: null,
                                    MandatoryLabel: null,
                                    RelationshipEntity: null,
                                    RelationshipEntityReqValidator: null,
                                    Tooltip: null,
                                    RelationshipView: null,
                                    RelationshipViewReqValidator: null
                                },
                                Fields:
                                {
                                    Tab: null,
                                    DisplayField: null,
                                    DisplayFieldReqValidator: null,
                                    MatchFields: null,
                                    MatchFieldsReqValidator: null,
                                    MaxRows: null,
                                    MaxRowsCmpValidator: null,
                                    MatchFieldSelector:
                                    {
                                        FieldItems: null,
                                        Modal:
                                        {
                                            Control: null,
                                            Panel: null
                                    }
                                    }
                                },
                                AutocompleteFields:
                               {
                                   Tab: null,
                                   AutocompleteDisplayFields: null,
                                   AutocompleteFieldSelector:
                                   {
                                       FieldItems: null,
                                       Modal:
                                       {
                                           Control: null,
                                           Panel: null
                                       }
                                   }
                               },
                                Filters:
                                {
                                    Tab: null,
                                    TreeContainer: null,
                                    Tree: null,
                                    Drop: null
                                },
                                LookupDisplayFields:
                                {
                                    Tab: null,
                                    TreeContainer: null,
                                    Tree: null,
                                    Drop: null
                                }
                            }
                        },
                        OnetoN:
                        {
                            Modal:
                            {
                                Control: null,
                                Panel: null,
                                PanelHeader: null,
                                TabContainer: null,
                                General:
                                {
                                    Tab: null,
                                    RelationshipName: null,
                                    RelationshipNameReqValidator: null,
                                    Description: null,
                                    Mandatory: null,
                                    MandatoryLabel: null,
                                    RelationshipEntity: null,
                                    RelationshipEntityReqValidator: null,
                                    Tooltip: null,
                                    RelationshipView: null,
                                    RelationshipViewReqValidator: null
                                }
                            }
                        }
                    }
                },

                Forms: {
                    RichTextEditor: null,
                    EditorExtender: null
                },

                Views:
                {
                    Grid: null,
                    Modal:
                    {
                        Control: null,
                        Panel: null,
                        PanelHeader: null,
                        TabContainer: null,
                        General:
                        {
                            Tab: null,
                            Name: null,
                            Description: null,
                            Menu: null,
                            MenuDescription: null,
                            ShowRecordCount:null,
                            AddForm: null,
                            // !!! current
                            AddFormSelectionMappings: null,
                            EditForm: null,
                            // !!! current
                            EditFormSelectionMappings: null,
                            AllowDelete: null,
                            AllowApproval: null,
                            AllowArchive: null,
                            // !!! current
                            FormSelectionMappings:
                            {
                                Panel: null,
                                Modal: null,
                                Header: null,
                                Body: null
                            },

                            CustomMenuStructure:
                            {
                                Panel: null,
                                Modal: null,
                                Header: null,
                                Body: null,
                                MenuTreeData: null
                            }

                        },
                        Fields:
                        {
                            Tab: null,
                            TreeContainer: null,
                            Tree: null,
                            Drop: null
                        },
                        Filters:
                        {
                            Tab: null,
                            TreeContainer: null,
                            Panel: null
                        },
                        Sort:
                        {
                            Tab: null,
                            Column: null,
                            Direction: null,
                            OrderValidator: null
                        },
                        Icon:
                        {
                            SearchName: null
                        }
                    }
                }
            },

            /* z-index's */
            zIndices:
            {
                Base: {},
                Attributes:
                {
                    Relationship:
                    {
                        Modal: 10026
                    }
                },

                Forms:
                {
                    Modal: 10000,
                    Tabs: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 5; },
                    Sections: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 10; },
                    Fields: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 15; },
                    ContextMenu: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 20; },
                    AvailableFieldsModal: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 25; },
                    AvailableFields: function () { return SEL.CustomEntityAdministration.zIndices.Forms.Modal + 30; }
                },

                Views:
                {
                    Modal: 10000,
                    TreeNodes: function () { return SEL.CustomEntityAdministration.zIndices.Views.Modal + 5; },
                    HelpIcon: function () { return SEL.CustomEntityAdministration.zIndices.Views.Modal + 10; },
                    ListItemInformation: function () { return SEL.CustomEntityAdministration.zIndices.Views.Modal + 15; },
                    ViewFilterHelpIcon: function () { return SEL.CustomEntityAdministration.zIndices.Views.Modal + 20; }
                },

                Misc:
                {
                    InformationMessage: function () { return SEL.CustomEntityAdministration.zIndices.Views.Modal + 25; }
                }
            },

            /* Message strings */
            Messages:
            {
                ModalTitle: 'Message from ' + moduleNameHTML,
                NoneOption: '[None]',

                Base: {},

                Attributes:
                {
                    Relationship:
                    {
                        DuplicateName: 'An attribute or relationship with this Display name already exists.',
                        RecursionWarning: 'This relationship cannot be saved as it would create a loop in the relationships.',
                        SelectRemovalItem: 'Please select a lookup field in the list to remove.',
                        NoRemovalItems: 'There are no lookup fields in the list to remove.',
                        SelectAutocompleteRemovalItem: 'Please select a autocomplete display field in the list to remove.',
                        NoAutocompleteRemovalItems: 'There are no autocomplete display fields in the list to remove.'
                    }
                },

                Forms: {},

                Views:
                {
                    DuplicateName: 'The View name you have entered already exists.',
                    DeleteSure: 'Are you sure you wish to delete the selected view?',
                    DeleteAbortInUse: 'This view cannot be deleted as it is currently in use within a relationship.',
                    DeleteAbortPopupViewInUse: 'This view cannot be deleted as it is currently in use within a Pop-up window view.',
                    MenuInstructions: 'Select the menu you would like this view to appear on.',
                    DeleteBuiltIn: 'This view cannot be deleted as it is a system view.'
                }
            },

            ParentFilter: {
                IsParentFilter: false,
                ChildElementId:0
            },

            /* Misc Functions */
            Misc:
            {
                LoadingScreenCancelled: false,

                ShowInformationMessage: function (loadingText)
                {
                    if (SEL.CustomEntityAdministration.Misc.LoadingScreenCancelled === false && $('#loadingArea').length === 0)
                    {
                        var loadingTextObj = $('<span id="loadingArea">' + loadingText + '</span>');

                        loadingTextObj.css('zIndex', SEL.CustomEntityAdministration.zIndices.Misc.InformationMessage());

                        loadingTextObj.css('left', ($(window).width() / 2) - 75).css('top', ($(window).height() / 2) - 90);

                        $('#divPages').append(loadingTextObj);
                    }
                },

                SetupEnterKeyBindings: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration;

                    // Base Save
                    SEL.Common.BindEnterKeyForSelector('.primaryPage .formpanel .inputs INPUT', thisNs.Base.Save);
                    // Attribute Save
                    SEL.Common.BindEnterKeyForSelector('#' + txtattributenameid, saveAttribute, '#' + txtlistitemid);
                    SEL.Common.BindEnterKeyForSelector('#' + chkattributemandatoryid, saveAttribute, '#' + txtlistitemid);
                    SEL.Common.BindEnterKeyForSelector('#' + txtmaxlengthid, saveAttribute, '#' + txtlistitemid);
                    SEL.Common.BindEnterKeyForSelector('#' + cmbdefaultvalueid, saveAttribute, '#' + txtlistitemid);
                    SEL.Common.BindEnterKeyForSelector('#' + cmbtextformatid, saveAttribute, '#' + txtlistitemid);
                    SEL.Common.BindEnterKeyForSelector('#' + lstitemsid, saveAttribute, '#' + txtlistitemid);
                    // Add attribute list item
                    SEL.Common.BindEnterKeyForSelector('#' + txtlistitemid, addListItem);
                    SEL.Common.BindEnterKeyForSelector('#' + chkarchiveitemid, addListItem);
                    // N To One Relationship Save
                    SEL.Common.BindEnterKeyForSelector('#' + thisNs.DomIDs.Attributes.Relationship.NtoOne.Modal.Panel + ' .inputs .fillspan', thisNs.Attributes.Relationship.ManyToOne.Save);
                    // One To N Relationship Save
                    SEL.Common.BindEnterKeyForSelector('#' + thisNs.DomIDs.Attributes.Relationship.OnetoN.Modal.Panel + ' .inputs .fillspan', thisNs.Attributes.Relationship.OneToMany.Save);
                    // Summary Save
                    SEL.Common.BindEnterKeyForSelector('#' + thisNs.DomIDs.Attributes.Summary.Modal.General.Name, thisNs.Attributes.Summary.saveSummary);
                    // Form Save
                    SEL.Common.BindEnterKeyForSelector('#' + tabConFormsID + ' .inputs INPUT', thisNs.Forms.SaveForm);
                    // Tab Save
                    SEL.Common.BindEnterKeyForSelector('#' + txttabid, thisNs.Forms.SaveTabWithModalSave);
                    // Section Save
                    SEL.Common.BindEnterKeyForSelector('#' + txtsectionid, thisNs.Forms.SaveSectionWithModalSave);
                    // Field Label Save
                    SEL.Common.BindEnterKeyForSelector('#' + txtfieldlabelID, thisNs.Forms.UpdateFieldLabel);
                    // Default Text Save
                    SEL.Common.BindEnterKeyForSelector('#txtDefaultText', thisNs.Forms.UpdateDefaultText);

                    // Form Copy Save
                    SEL.Common.BindEnterKeyForSelector('#' + txtcopyformnameid, thisNs.Forms.Copy);
                    // View Save
                    SEL.Common.BindEnterKeyForSelector('#' + thisNs.DomIDs.Views.Modal.TabContainer + ' .inputs INPUT', thisNs.Views.SaveWithModalCheck);
                    // View Form Mappings Save
                    SEL.Common.BindEnterKeyForSelector('#' + thisNs.DomIDs.Views.Modal.General.FormSelectionMappings.Panel + ' INPUT', thisNs.Views.General.FormSelectionMappings.Save);
                },

                ErrorHandler: function (data)
                {
                    SEL.CustomEntityAdministration.Misc.LoadingScreenCancelled = true;
                    $('#loadingArea').remove();

                    SEL.Common.WebService.ErrorHandler(data);
                }
            },

            /* Main functions */

            /// <comment>
            /// This contains the methods and parameters relevant to the Base entity section of custom entity admin
            /// </comment>
            Base:
            {
                // TODO: Moved, NOT refactored
                Save: function ()
                {
                    if ($g(chkEnableCurrenciesID).checked)
                        ValidatorEnable(document.getElementById(cvDefaultCurrencyID), true);

                    if (validateform('vgMain') === false)
                        return false;

                    var entityname = document.getElementById(txtentitynameid).value;
                    var pluralname = document.getElementById(txtpluralnameid).value;
                    var description = document.getElementById(txtdescriptionid).value;
                    var enableattachments = document.getElementById(chkenableattachmentsid).checked;
                    var enableAudiences = document.getElementById(chkEnableAudiencesID).checked;
                    var audienceBehaviour = $('#' + ddlAudienceBehaviourID).val();
                    var allowdocmerge = document.getElementById(chkallowdocmergeid).checked;
                    var enableCurrencies = document.getElementById(chkEnableCurrenciesID).checked;
                    var enableLocking = document.getElementById(chkEnableLockingID).checked;

                    var defaultCurrencySelect = document.getElementById(ddlDefaultCurrencyID);
                    var defaultCurrency = null;
                    if (defaultCurrencySelect !== null)
                    {
                        defaultCurrency = defaultCurrencySelect.options[defaultCurrencySelect.selectedIndex].value;
                        if (defaultCurrency === "0")
                            defaultCurrency = null;
                    }

                    var enableNewPopupWindow = document.getElementById(chkEnablePopupWindowID).checked;
                    var defaultPopupViewSelect = document.getElementById(ddlPopupWindowViewID);
                    var defaultPopupView = null;

                    if (defaultPopupViewSelect !== null) {
                        if (defaultPopupViewSelect.options.length > 0 && defaultPopupViewSelect.options[defaultPopupViewSelect.selectedIndex].value != -1) {
                            defaultPopupView = defaultPopupViewSelect.options[defaultPopupViewSelect.selectedIndex].value;
                        } else {
                            defaultPopupView = null;
                        }                                         
                    }

                    var formSelectionAttribute = $("#" + SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute);
                    var formSelectionAttributeId = 0;
                    if (typeof formSelectionAttribute !== "undefined" && formSelectionAttribute !== null && formSelectionAttribute.length > 0)
                    {
                        formSelectionAttributeId = formSelectionAttribute.val();
                    }

                    var ownerId = +$("#ctl00_contentmain_Owner_SelectinatorText_ID").val() || null;
                    var supportContactId = +$("#ctl00_contentmain_SupportContact_SelectinatorText_ID").val() || null;
                    var supportQuestion = $("#ctl00_contentmain_txtSupportQuestion").val();
                    var builtIn = $("#ctl00_contentmain_chkBuiltIn").is(":checked");

                    if (entityid === 0)
                    {
                        document.getElementById(hdnAuditAttributeID).value = "0";
                        document.getElementById(hdnAuditAttributeDisplayNameID).value = "id";
                    }

                    Spend_Management.svcCustomEntities.saveEntity(cu.AccountID, employeeid, entityid, entityname, pluralname, description, enableattachments, audienceBehaviour, allowdocmerge, enableCurrencies, defaultCurrency, enableNewPopupWindow, defaultPopupView, formSelectionAttributeId, ownerId, supportContactId, supportQuestion, enableLocking, builtIn, SEL.CustomEntityAdministration.Base.SaveEntityComplete);
                },

                SaveEntityComplete: function (data)
                {
                    if (data === -1)
                    {
                        SEL.MasterPopup.ShowMasterPopup('The GreenLight name you have provided already exists.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                        return;
                    }
                    else if (data === -2)
                    {
                        SEL.MasterPopup.ShowMasterPopup('The Plural name you have provided already exists.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                        return;
                    }
                    else if (data === -3)
                    {
                        SEL.MasterPopup.ShowMasterPopup('The GreenLight name and Plural name you have provided already exist.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                        return;
                    }

                    entityid = data;
                    SEL.CustomEntityAdministration.IDs.Entity = data;

                    // TODO: Update all the below to namespaced
                    setMonetaryState();

                    switch (currentAction)
                    {
                        case 'addAttribute':
                            currentAction = '';
                            showAttributeModal();
                            break;
                        case 'addRelationship1':
                            currentAction = '';
                            SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Modal.Show();
                            break;
                        case 'addRelationship2':
                            currentAction = '';
                            SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.Modal.Show();
                            break;
                        case 'addSummary':
                            currentAction = '';
                            SEL.CustomEntityAdministration.Attributes.Summary.Add();
                            break;
                        case 'addForm':
                            currentAction = '';
                            SEL.CustomEntityAdministration.Forms.NewForm();
                            break;
                        case 'addView':
                            currentAction = '';
                            SEL.CustomEntityAdministration.Views.Add();
                            break;
                        case 'formSelectionAttributeChanged':
                            currentAction = '';
                            break;
                        default:
                            document.location = "custom_entities.aspx";
                            break;
                    }
                },

                AddShortCuts: function (shortcutSet)
                {
                    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();

                    switch (shortcutSet)
                    {
                        case 'listAttribute':
                            shortcut.add("Ctrl+1", function () { showListItemModal(false); });
                            shortcut.add("Ctrl+2", function () { removeListItem(); });
                            shortcut.add("Ctrl+3", function () { editListItem(); });
                            break;
                        case 'relationshipModal':
                            shortcut.add("Ctrl+1", function () { SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Modal.Show(false); });
                            shortcut.add("Ctrl+2", function () { SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Remove(); });
                            break;
                        case 'formDesigner':
                            shortcut.add("Ctrl+1", function () { SEL.CustomEntityAdministration.Forms.ShowTabModal(null, true); }); //new tab
                            shortcut.add("Ctrl+2", function () { SEL.CustomEntityAdministration.Forms.ShowSectionModal(null, true); }); //new section
                            shortcut.add("Ctrl+3", function () { SEL.CustomEntityAdministration.Forms.ShortcutTabEdit(); }); //edit tab
                            shortcut.add("Ctrl+4", function () { SEL.CustomEntityAdministration.Forms.ToggleAvailableFields('expand'); }); //toggle available fields
                            shortcut.add("Ctrl+5", function () { SEL.CustomEntityAdministration.Forms.ToggleAvailableFields('dockleft'); }); //dock available fields left
                            shortcut.add("Ctrl+6", function () { SEL.CustomEntityAdministration.Forms.ToggleAvailableFields('dockright'); }); //dock available fields right
                            break;
                    }
                },

                RemoveAllShortCuts: function ()
                {
                    shortcut.remove("Ctrl+1");
                    shortcut.remove("Ctrl+2");
                    shortcut.remove("Ctrl+3");
                    shortcut.remove("Ctrl+4");
                    shortcut.remove("Ctrl+5");
                    shortcut.remove("Ctrl+6");
                },

                // Use this function to assign blank shortcuts to Ctrl 1 - Ctrl 6
                // This will avoid the browser performing any default shortcut action
                AssignDummyShortCuts: function ()
                {
                    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();

                    shortcut.add("Ctrl+1", function (){});
                    shortcut.add("Ctrl+2", function (){});
                    shortcut.add("Ctrl+3", function (){});
                    shortcut.add("Ctrl+4", function (){});
                    shortcut.add("Ctrl+5", function (){});
                    shortcut.add("Ctrl+6", function (){});
                },

                /// <comment>
                /// This refreshes the drop down list of pop-up views when a view is added or deleted with an associated entity.
                /// </comment>
                RefreshPopupViewDDL: function (data) {

                    var cmbpopupview = document.getElementById(ddlPopupWindowViewID);
                    var defaultpopupID = cmbpopupview.value;

                    cmbpopupview.options.length = 0;

                    for (var i = 0; i < data.length; i++) {
                        var option = document.createElement("OPTION");
                        option.text = data[i].Text;
                        option.value = data[i].Value;
                        if (option.value == defaultpopupID) {
                            option.selected = true;
                        }
                        cmbpopupview.options.add(option);                    
                    }
                return;
                },

                /// <comment>
                /// Add to the FormSelectionAttribute dropdown.
                /// </comment>
                AddFormSelectionAttribute: function (text, value)
                {
                    var controlId = SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute,
                        control = $("#" + controlId);

                    if (typeof text !== "undefined" && typeof value !== "undefined" && text !== null && value !== null
                        && !isNaN(parseInt(value, 10)) && control !== null && control.length === 1 && control.find("option[value=" + value + "]").length === 0)
                    {
                        control.append("<option value=\"" + value + "\">" + text + "</option>");
                }
                },

                /// <comment>
                /// Delete from the FormSelectionAttribute dropdown.
                /// </comment>
                DeleteFormSelectionAttribute: function (value)
                {
                    var controlId = SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute,
                        control = $("#" + controlId);

                    if (typeof value !== "undefined" && value !== null && !isNaN(parseInt(value, 10)) && control !== null
                        && control.length === 1 && control.find("option[value=" + value + "]").length === 1)
                    {
                        control.find("option[value=" + value + "]").remove();
                    }
                },

                /// <comment>
                /// Shallow-save the custom entity if the form selection attribute changes
                FormSelectionAttributeChanged: function() {

                    if (entityid > 0) {
                        currentAction = "formSelectionAttributeChanged";
                        SEL.CustomEntityAdministration.Base.Save();
                    }
                },
                SetAudienceListState: function (checkBox) {
                    var audienceBehaviour = $('.audienceBehaviour');
                    var labelText = audienceBehaviour.text();
                    if ($(checkBox).is(":checked")) {
                        $('#' + ddlAudienceBehaviourID).prop('disabled', false);
                        $('#' + cmpAudienceID).prop('disabled', false);
                        ValidatorEnable($('#' + cmpAudienceID)[0], true);
                        audienceBehaviour.addClass('mandatory');
                        audienceBehaviour.text(labelText + '*');

                    } else {
                        $('#' + ddlAudienceBehaviourID).prop('disabled', true);
                        $('#' + ddlAudienceBehaviourID).val(0);
                        $('#' + cmpAudienceID).prop('disabled', true);
                        audienceBehaviour.removeClass('mandatory');
                        labelText = labelText.replace('*', '');
                        audienceBehaviour.text(labelText);
                        ValidatorEnable($('#' + cmpAudienceID)[0], false);
                    }
                }
            },

            /// <comment>
            /// This contains the methods and parameters relevant to the Attributes and relationships section of custom entity admin
            /// </comment>
            Attributes:
            {
                Summary:
                {
                    lstSummarySrcChanged: false,
                    summaryEditData: null,
                    availableRels: null,
                    columnRels: null,
                    Modal:
                    {
                        Hide: function ()
                        {
                            currentAction = '';
                            SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Control);
                        },
                        Show: function ()
                        {
                            var summaryAttributeModal = SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Control;
                            $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.TabContainer).set_activeTabIndex(0);

                            if (SEL.CustomEntityAdministration.IDs.Attribute === 0)
                            {
                                currentAction = 'addSummary';
                                $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.PanelHeader).innerHTML = 'New Summary Attribute';
                            }
                            SEL.Common.ShowModal(summaryAttributeModal);
                            $(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Name).select();
                        }
                    },
                    DisplayField:
                    {
                        currentDisplayFieldAttributeId: null,
                        Modal:
                        {
                            Hide: function ()
                            {
                                SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.DisplayFieldModal.Control);
                            },
                            Show: function ()
                            {
                                SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.RefreshTree();
                                SEL.Common.ShowModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.DisplayFieldModal.Control);
                            }
                        },
                        RefreshTree: function ()
                        {
                            Spend_Management.svcCustomEntities.GetInitialTreeNodesForDisplayField(SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId,
                                function (r)
                                {
                                    var displayFieldTreeId = SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.DisplayFieldModal.Tree;

                                    SEL.Trees.Tree.Clear(displayFieldTreeId);

                                    if (r !== null)
                                    {
                                        SEL.Trees.Tree.Data.Set(displayFieldTreeId, r);
                                    }
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        }
                    },
                    ClearSummaryForm: function ()
                    {
                        SEL.Common.Page_ClientValidateReset();

                        $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Name).value = '';
                        $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Description).value = '';
                        $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.SummarySource).selectedIndex = 0;
                        $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Tab).set_enabled(false);
                        $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Tab).set_enabled(false);
                    },
                    GetSummarySources: function ()
                    {
                        var lst = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.SummarySource);
                        var rel_entityid = 0;
                        if (lst !== null)
                        {
                            rel_entityid = lst[lst.selectedIndex].value;
                        }
                        
                        if (lst.selectedIndex !== 0)
                        {
                            $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Tab).set_enabled(true);
                            $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Tab).set_enabled(true);
                        }
                        else
                        {
                            $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Tab).set_enabled(false);
                            $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Tab).set_enabled(false);
                        }
                        var availRelationships = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Control);
                        if (availRelationships !== null)
                        {
                            availRelationships.innerHTML = '<img src="/shared/images/ajax-loader.gif" alt="Loading..." />';
                        }

                        var relColumns = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Control);
                        if (relColumns !== null)
                        {
                            relColumns.innerHTML = '<img src="/shared/images/ajax-loader.gif" alt="Loading..." />';
                        }

                        if (rel_entityid !== 0)
                        {
                            Spend_Management.svcCustomEntities.getSummarySources(SEL.CustomEntityAdministration.IDs.Entity, rel_entityid, SEL.CustomEntityAdministration.Attributes.Summary.onSourceSuccess, SEL.CustomEntityAdministration.Attributes.Summary.onSourceFail);
                        }
                        else
                        {
                            $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Control).innerHTML = '<div class="onecolumnpanel">No current selections available.</div>';
                            $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Control).innerHTML = '<div class="onecolumnpanel">No current selections available.</div>';
                        }
                    },
                    onSourceSuccess: function (data)
                    {
                        var emptData = '<div class="onecolumnpanel">No current selections available.</div>';
                        var availRelationships = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Control);
                        if (availRelationships !== null)
                        {
                            if (data.length != 0)
                            {
                                availRelationships.innerHTML = parseScript(data[0]);
                            }
                            else
                            {
                                availRelationships.innerHTML = emptData;
                        }
                        }
                        else
                        {
                            availRelationships.innerHTML = emptData;
                        }

                        var relColumns = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Control);
                        if (relColumns !== null)
                        {
                            if (data.length !== 0)
                            {
                                relColumns.innerHTML = parseScript(data[1]);
                            }
                            else
                            {
                                relColumns.innerHTML = emptData;
                        }
                        }
                        else
                        {
                            relColumns.innerHTML = emptData;
                        }

                        if (data.length !== 0)
                        {
                            parseScript(data[2]);
                        }
                    },
                    onSourceFail: function (error)
                    {
                        SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to retrieve the summary source information for the selected record\n' + error, SEL.CustomEntityAdministration.Messages.ModalTitle);
                    },
                    getSummary: function ()
                    {
                        Spend_Management.svcCustomEntities.getRelationshipDropDown(entityid, SEL.CustomEntityAdministration.Forms.RelationshipType.OneToMany, true, false, SEL.CustomEntityAdministration.Attributes.Summary.getSummaryInfo, SEL.CustomEntityAdministration.Misc.ErrorHandler);
                    },
                    getSummaryInfo: function (data)
                    {
                        SEL.CustomEntityAdministration.Attributes.Summary.updateSummarySourceDropdown(data);
                        Spend_Management.svcCustomEntities.getSummary(cu.AccountID, SEL.CustomEntityAdministration.IDs.Entity, SEL.CustomEntityAdministration.IDs.Attribute, SEL.CustomEntityAdministration.Attributes.Summary.onSummaryComplete, SEL.CustomEntityAdministration.Attributes.Summary.onSummaryFail);
                    },
                    onSummaryComplete: function (data)
                    {
                        SEL.CustomEntityAdministration.Attributes.Summary.ClearSummaryForm();

                        SEL.CustomEntityAdministration.Attributes.summaryEditData = data;

                        $g('divSummaryHeading').innerHTML = 'Summary Attribute: ' + data[0];
                        $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Name).value = data[0];
                        $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Description).value = data[1];
                        var lst = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.SummarySource);
                        if (lst !== null)
                        {
                            for (var idx = 0; idx < lst.options.length; idx++)
                            {
                                if (lst[idx].value === data[2])
                                {
                                    lst.selectedIndex = idx;
                                    SEL.CustomEntityAdministration.Attributes.lstSummarySrcChanged = true;

                                    var availRelationships = $g('divavailablerelationships');
                                    if (availRelationships != null)
                                        availRelationships.innerHTML = '<img src="/shared/images/ajax-loader.gif" alt="Loading..." />';

                                    var relColumns = $g('divrelationshipcolumns');
                                    if (relColumns !== null)
                                        relColumns.innerHTML = '<img src="/shared/images/ajax-loader.gif" alt="Loading..." />';

                                    Spend_Management.svcCustomEntities.getSummarySources(entityid, data[2], SEL.CustomEntityAdministration.Attributes.Summary.onEditFetchSuccess, SEL.CustomEntityAdministration.Attributes.Summary.onSourceFail);
                                    break;
                                }
                            }
                        }
                        $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Tab).set_enabled(true);
                        $f(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Tab).set_enabled(true);
                        SEL.CustomEntityAdministration.Attributes.Summary.Modal.Show();
                    },
                    onEditFetchSuccess: function (data)
                    {
                        SEL.CustomEntityAdministration.Attributes.Summary.onSourceSuccess(data);
                        var chk, seq;
                        if (SEL.CustomEntityAdministration.Attributes.lstSummarySrcChanged)
                        {
                            var relRec = new SEL.CustomEntityAdministration.Attributes.Summary.JsonObjects.sSummary();
                            for (var rel = 0; rel < SEL.CustomEntityAdministration.Attributes.summaryEditData[3].length; rel++)
                            {
                                relRec = SEL.CustomEntityAdministration.Attributes.summaryEditData[3][rel];

                                chk = $g('chkRel_' + relRec.otm_attributeid);
                                if (chk !== null)
                                {
                                    chk.checked = true;
                                    $g('hiddenRelSAID_' + relRec.otm_attributeid).value = relRec.summary_attributeid;
                                    seq = $g('txtRelSeq_' + relRec.otm_attributeid);
                                    seq.value = relRec.order;
                                    seq.disabled = false;
                                }
                            }

                            var colRec = new SEL.CustomEntityAdministration.Attributes.Summary.JsonObjects.sSummaryColumn();
                            for (var col = 0; col < SEL.CustomEntityAdministration.Attributes.summaryEditData[4].length; col++)
                            {
                                colRec = SEL.CustomEntityAdministration.Attributes.summaryEditData[4][col];

                                chk = $g('chkCol_' + colRec.columnAttributeID);
                                if (chk !== null)
                                {
                                    chk.checked = true;
                                    $g('hiddenColID_' + colRec.columnAttributeID).value = colRec.columnid;
                                    if (colRec.ismtoattribute)
                                    {
                                        var displayfield = $g('mtoFieldLink_' + colRec.columnAttributeID);
                                        displayfield.style.display = '';
                                        $g('txtMTODisplayField_' + colRec.columnAttributeID).innerHTML = (colRec.displayFieldName == '' ? 'Not Selected' : colRec.displayFieldName);
                                        $g('hidMTODisplayFieldId_' + colRec.columnAttributeID).value = colRec.displayFieldId;
                                        $g('hidMTOJoinViaId_' + colRec.columnAttributeID).value = colRec.JoinViaID;
                                    }
                                    seq = $g('txtColSequence_' + colRec.columnAttributeID);
                                    seq.value = colRec.order;
                                    seq.disabled = false;
                                    var alth = $g('txtColAltHeader_' + colRec.columnAttributeID);
                                    alth.value = colRec.alt_header;
                                    alth.disabled = false;
                                    var colwidthinput = $g('txtColWidth_' + colRec.columnAttributeID);
                                    if (colRec.width !== 0)
                                    {
                                        colwidthinput.value = colRec.width;
                                    }
                                    else
                                    {
                                        colwidthinput.value = '';
                                    }
                                    colwidthinput.disabled = false;
                                    var colfilter = $g('txtColFilter_' + colRec.columnAttributeID);
                                    colfilter.value = colRec.filterVal;
                                    colfilter.disabled = false;
                                    var sortradio = $g('optDefaultSort_' + colRec.columnAttributeID);
                                    sortradio.disabled = false;
                                    sortradio.checked = colRec.default_sort;
                                }
                            }

                            SEL.CustomEntityAdministration.Attributes.lstSummarySrcChanged = false;
                            SEL.CustomEntityAdministration.Attributes.summaryEditData = null;
                        }
                    },
                    onSummaryFail: function (error)
                    {
                        SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to retrieve the summary record information\n' + error, SEL.CustomEntityAdministration.Messages.ModalTitle);
                    },
                    updateSummarySourceDropdown: function (data)
                    {
                        var cmbsummary = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.SummarySource);
                        cmbsummary.options.length = 0;

                        for (var i = 0; i < data.length; i++)
                        {
                            var option = document.createElement("OPTION");
                            option.text = data[i].Text;
                            option.value = data[i].Value;
                            cmbsummary.options.add(option);
                        }

                        var availRelationships = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Relationships.Control);
                        if (availRelationships !== null)
                        {
                            availRelationships.innerHTML = '<div class="onecolumnpanel">No current selections available.</div>';
                        }

                        var relColumns = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Columns.Control);
                        if (relColumns !== null)
                        {
                            relColumns.innerHTML = '<div class="onecolumnpanel">No current selections available.</div>';
                        }
                    },
                    Add: function ()
                    {
                        SEL.CustomEntityAdministration.IDs.Attribute = 0;
                        SEL.CustomEntityAdministration.Attributes.Summary.ClearSummaryForm();
                        if (entityid === 0)
                        {
                            currentAction = 'addSummary';
                            SEL.CustomEntityAdministration.Base.Save();
                            return;
                        }
                        Spend_Management.svcCustomEntities.getRelationshipDropDown(entityid, SEL.CustomEntityAdministration.Forms.RelationshipType.OneToMany, true, false, SEL.CustomEntityAdministration.Attributes.Summary.updateSummarySourceDropdown, SEL.CustomEntityAdministration.Misc.ErrorHandler);
                        SEL.CustomEntityAdministration.Attributes.Summary.Modal.Show();
                    },
                    Edit: function (attributeId)
                    {
                        SEL.CustomEntityAdministration.IDs.Attribute = attributeId;
                        SEL.CustomEntityAdministration.Attributes.Summary.getSummary();
                    },
                    SelectSummaryMTODisplayField: function (attId)
                    {
                        SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId = attId;
                        SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.Modal.Show();
                    },
                    ColSelectAll: function ()
                    {
                        var checked = $g('chkColSAll').checked;

                        for (var i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.columnRels.length; i++)
                        {
                            var chk = $g('chkCol_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]);
                            if (chk !== null)
                            {
                                chk.checked = checked;
                                SEL.CustomEntityAdministration.Attributes.Summary.RelColRowSwapState(chk.id.replace('chkCol_', ''));
                            }
                        }
                    },
                    RelSelectAll: function ()
                    {
                        var checked = $g('chkRelSAll').checked;

                        for (var i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.availableRels.length; i++)
                        {
                            var chk = $g('chkRel_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]);
                            if (chk !== null)
                            {
                                chk.checked = checked;
                                SEL.CustomEntityAdministration.Attributes.Summary.SwapRelState(chk.id.replace('chkRel_', ''));
                            }
                        }

                    },
                    RelColRowSwapState: function (attid, isMTO)
                    {
                        var chk = $g('chkCol_' + attid);
                        var seq = $g('txtColSequence_' + attid);
                        var alth = $g('txtColAltHeader_' + attid);
                        var colwidthinput = $g('txtColWidth_' + attid);
                        var colfilter = $g('txtColFilter_' + attid);
                        var sortradio = $g('optDefaultSort_' + attid);

                        if (chk.checked)
                        {
                            seq.disabled = false;
                            alth.disabled = false;
                            colwidthinput.disabled = false;
                            colfilter.disabled = false;
                            sortradio.disabled = false;
                        }
                        else
                        {
                            seq.disabled = true;
                            alth.disabled = true;
                            alth.value = '';
                            colwidthinput.disabled = true;
                            colwidthinput.value = '';
                            colfilter.disabled = true;
                            colfilter.value = '';
                            sortradio.disabled = true;
                            sortradio.checked = false;
                        }

                        if (isMTO)
                        {
                            var mtofieldselect = $g('mtoFieldLink_' + attid);
                            mtofieldselect.style.display = (chk.checked ? '' : 'none');
                        }
                    },
                    SwapRelState: function (attid)
                    {
                        var chk = $g('chkRel_' + attid);
                        var seq = $g('txtRelSeq_' + attid);

                        if (chk.checked)
                        {
                            seq.disabled = false;
                        }
                        else
                        {
                            seq.disabled = true;
                        }
                    },
                    saveSummary: function ()
                    {
                        if (validateform('vgSummary') == false)
                        {
                            return;
                        }

                        // validate both editors
                        var validatormessage = '';
                        var relselected = false;
                        var i, chk;

                        for (i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.availableRels.length; i++)
                        {
                            chk = $g('chkRel_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]);
                            if (chk !== null)
                            {
                                if (chk.checked)
                                {
                                    relselected = true;
                                    var relseq = $g('txtRelSeq_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]).value;
                                    if (isNaN(relseq))
                                    {
                                        validatormessage += '<li>Please enter a valid number for Sequence for the selected relationship on row ' + (i + 1) + '.</li>';
                                    }
                                    else
                                    {
                                        if (relseq <= 0)
                                        {
                                            validatormessage += '<li>Please enter a number greater than 0 for Sequence for the selected relationship on row ' + (i + 1) + '.</li>';
                                        }
                                        if (relseq.indexOf('.') > -1)
                                        {
                                            validatormessage += '<li>Please enter a whole number for Sequence for the selected relationship on row ' + (i + 1) + '.</li>';
                                        }
                                    }
                                }
                            }
                        }
                        var relattributesselected = false;
                        for (i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.columnRels.length; i++)
                        {
                            var colseq = $g('txtColSequence_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value;
                            if (isNaN(colseq))
                            {
                                validatormessage += '<li>Please enter a valid number for Column Sequence for the selected attribute on row ' + (i + 1) + '.</li>';
                            }
                            else
                            {
                                if (colseq !== '')
                                {
                                    if (colseq <= 0)
                                    {
                                        validatormessage += '<li>Please enter a number greater than 0 for the Column Sequence for the selected attribute on row ' + (i + 1) + '.</li>';
                                    }
                                    if (colseq.indexOf('.') > -1)
                                    {
                                        validatormessage += '<li>Please enter a whole number for Sequence for the selected attribute on row ' + (i + 1) + '.</li>';
                                    }
                                }
                                else
                                {
                                    validatormessage += '<li>Please enter a valid number for Column Sequence for the selected attribute on row ' + (i + 1) + '.</li>';
                                }
                            }
                            chk = $g('chkCol_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]);
                            if (chk !== null)
                            {
                                if (chk.checked)
                                {
                                    relattributesselected = true;
                                    var colwidth = $g('txtColWidth_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value;
                                    if (isNaN(colwidth))
                                    {
                                        validatormessage += '<li>Please enter a valid number for Column Width for the selected attribute on row ' + (i + 1) + '.</li>';
                                    }
                                    else
                                    {
                                        if (colwidth !== '')
                                        {
                                            if (colwidth <= 0)
                                            {
                                                validatormessage += '<li>Please enter a number greater than 0 for the Column Width for the selected attribute on row ' + (i + 1) + '.</li>';
                                            }
                                            if (colwidth.indexOf('.') > -1)
                                            {
                                                validatormessage += '<li>Please enter a whole number for Column Width for the selected attribute on row ' + (i + 1) + '.</li>';
                                            }
                                        }
                                    }
                                    if ($e('hidMTODisplayFieldId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]))
                                    {
                                        if ($g('hidMTODisplayFieldId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value === '')
                                        {
                                            validatormessage += '<li>Please select a Display Field for the selected attribute on row ' + (i + 1) + '.</li>';
                                        }
                                    }
                                }
                            }
                        }
                        if (!relselected)
                        {
                            validatormessage += '<li>Please select at least one relationship for the summary.</li>';
                        }
                        if (!relattributesselected)
                        {
                            validatormessage += '<li>Please select at least one attribute column for the summary.</li>';
                        }
                        if (validatormessage.length > 0)
                        {
                            validatormessage = "<ul style=\"margin:0; padding: 0;list-style-type: none;\">" + validatormessage + "</ul>";
                            SEL.MasterPopup.ShowMasterPopup(validatormessage, SEL.CustomEntityAdministration.Messages.ModalTitle);
                            return;
                        }

                        var attributename = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Name).value;
                        var description = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Description).value;
                        var summarysourcelst = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.SummarySource);
                        var summarysource = 0;
                        if (summarysourcelst !== null)
                        {
                            summarysource = summarysourcelst[summarysourcelst.selectedIndex].value;
                        }
                        var selected_rel_attributeids = []; //getSelectedItemsFromGrid('gridRelationships');
                        var Rec;
                        for (i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.availableRels.length; i++)
                        {
                            chk = $g('chkRel_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]);
                            if (chk !== null)
                            {
                                if (chk.checked)
                                {
                                    Rec = new SEL.CustomEntityAdministration.Attributes.Summary.JsonObjects.sSummary();
                                    Rec.summary_attributeid = new Number($g('hiddenRelSAID_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]).value);
                                    Rec.attributeid = SEL.CustomEntityAdministration.IDs.Attribute;
                                    Rec.otm_attributeid = SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i];
                                    Rec.order = new Number($g('txtRelSeq_' + SEL.CustomEntityAdministration.Attributes.Summary.availableRels[i]).value);

                                    selected_rel_attributeids.push(Rec);
                                }
                            }
                        }
                        var selected_cols = []; //getSelectedItemsFromGrid('gridColumns');
                        var joinViaData = [];

                        for (i = 0; i < SEL.CustomEntityAdministration.Attributes.Summary.columnRels.length; i++)
                        {
                            chk = $g('chkCol_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]);
                            if (chk !== null)
                            {
                                if (chk.checked)
                                {
                                    Rec = new SEL.CustomEntityAdministration.Attributes.Summary.JsonObjects.sSummaryColumn();
                                    Rec.columnid = new Number($g('hiddenColID_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value);
                                    Rec.attributeid = SEL.CustomEntityAdministration.IDs.Attribute;
                                    Rec.columnAttributeID = SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i];
                                    Rec.alt_header = $g('txtColAltHeader_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value;
                                    Rec.width = new Number($g('txtColWidth_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value);
                                    Rec.order = new Number($g('txtColSequence_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value);
                                    Rec.filterVal = $g('txtColFilter_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value;
                                    Rec.default_sort = $g('optDefaultSort_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).checked;
                                    Rec.displayFieldName = ($e('txtMTODisplayField_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]) ? $g('txtMTODisplayField_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).innerHTML : '');
                                    Rec.displayFieldId = ($e('hidMTODisplayFieldId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]) ? $g('hidMTODisplayFieldId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value : '');
                                    Rec.joinViaId = ($e('hidMTOJoinViaId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]) ? $g('hidMTOJoinViaId_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value : 0);
                                    Rec.joinViaId = Rec.joinViaId === "" ? 0 : Rec.joinViaId;
                                    selected_cols.push(Rec);
                                    Rec = new SEL.CustomEntityAdministration.Attributes.Summary.JsonObjects.sJoinViaColumnData();
                                    Rec.columnid = new Number($g('hiddenColID_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value);
                                    Rec.joinViaCrumbs = ($e('hidMTOJoinViaCrumbs_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]) ? $g('hidMTOJoinViaCrumbs_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value : '');
                                    Rec.joinViaPath = ($e('hidMTOJoinViaPath_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]) ? $g('hidMTOJoinViaPath_' + SEL.CustomEntityAdministration.Attributes.Summary.columnRels[i]).value : '');

                                    if (Rec.joinViaCrumbs !== '' && Rec.joinViaPath !== '')
                                        joinViaData.push(Rec);
                                }
                            }
                        }

                        Spend_Management.svcCustomEntities.saveSummary(SEL.CustomEntityAdministration.IDs.Entity, SEL.CustomEntityAdministration.IDs.Attribute, attributename, description, summarysource, selected_rel_attributeids, selected_cols, joinViaData, SEL.CustomEntityAdministration.Attributes.Summary.onSaveSummaryComplete, SEL.CustomEntityAdministration.Attributes.Summary.onSaveSummaryFail);
                    },
                    onSaveSummaryComplete: function (data)
                    {
                        if (data === -1)
                        {
                            SEL.MasterPopup.ShowMasterPopup('An attribute or relationship with this Display name already exists.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                        }
                        else
                        {
                            SEL.CustomEntityAdministration.Attributes.Summary.Modal.Hide();
                            Spend_Management.svcCustomEntities.getAttributesGrid(entityid, RefreshAttributeGridComplete);
                        }
                    },
                    onSaveSummaryFail: function (error)
                    {
                        SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to save the summary record information\n' + error, SEL.CustomEntityAdministration.Messages.ModalTitle);
                    },
                    JsonObjects: {
                        sSummary: function ()
                        {
                            this.summary_attributeid = null;
                            this.attributeid = null;
                            this.otm_attributeid = null;
                            this.order = null;
                        },
                        sSummaryColumn: function ()
                        {
                            this.columnid = null;
                            this.attributeid = null;
                            this.columnAttributeID = null;
                            this.alt_header = null;
                            this.width = null;
                            this.order = null;
                            this.default_sort = null;
                            this.filterVal = null;
                            this.isMTO = null;
                            this.displayFieldId = null;
                            this.displayFieldName = null;
                            this.joinViaId = null;
                        },
                        sJoinViaColumnData: function ()
                        {
                            this.columnid = null;
                            this.joinViaCrumbs = null;
                            this.joinViaPath = null;
                        }
                    },
                    selectDisplayField: function ()
                    {
                        var displayField = SEL.Trees.Tree.SelectedNode(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.DisplayFieldModal.Tree);
                        if (displayField.length === 0)
                        {
                            SEL.MasterPopup.ShowMasterPopup('A display field has not been selected.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                        }
                        else
                        {
                            var displayFieldName = SEL.Trees.Node.GetText(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.DisplayFieldModal.Tree, displayField.attr('id'));

                            $g('txtMTODisplayField_' + SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId).innerHTML = displayFieldName;
                            $g('hidMTODisplayFieldId_' + SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId).value = displayField.attr('fieldid');
                            $g('hidMTOJoinViaCrumbs_' + SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId).value = displayField.attr('crumbs');
                            $g('hidMTOJoinViaPath_' + SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.currentDisplayFieldAttributeId).value = displayField.attr('id');

                            SEL.CustomEntityAdministration.Attributes.Summary.DisplayField.Modal.Hide();
                        }
                    }
                },

                /*
                *  Relationships - N:1 and 1:N combined modal javascript
                */
                
                Relationship:
                {
                    Get: function (attid, formid) {
                        var ns = SEL.CustomEntityAdministration,
                            thisNs = ns.Attributes.Relationship,
                            relationshipDomIds = ns.DomIDs.Attributes.Relationship,
                            manyToOneGeneralTabDomIds = relationshipDomIds.NtoOne.Modal.General,
                            oneToManyGeneralTabDomIds = relationshipDomIds.OnetoN.Modal.General,
                            relationshipIds = ns.IDs.Relationship;

                        ns.IDs.Attribute = attid;
                        if (formid == null) {
                            ns.IDs.Form = 0;
                        } else {
                            ns.IDs.Form = formid;
                        }

                        Spend_Management.svcCustomEntities.getRelationship(cu.AccountID, entityid, attid,
                            function (data)
                            {
                                var tmpns;

                                if (data[0] === 2)
                                {
                                    tmpns = thisNs.OneToMany;

                                    tmpns.Form.Clear();

                                    $g('divOneToManyRelationshipHeading').innerHTML = '1:n Relationship Attribute: ' + data[3];

                                    $g(oneToManyGeneralTabDomIds.RelationshipName).value = data[3];
                                    $g(oneToManyGeneralTabDomIds.Description).value = data[4];
                                    relationshipIds.SelectedViewID = data[8];

                                    $g(oneToManyGeneralTabDomIds.BuiltIn).checked = data[9];
                                    if (CurrentUserInfo.AdminOverride) {
                                        $g(oneToManyGeneralTabDomIds.BuiltIn).disabled = false;
                                }

                                    if ($g(oneToManyGeneralTabDomIds.BuiltIn).checked) {
                                        $g(oneToManyGeneralTabDomIds.BuiltIn).disabled = true;
                                    }
                                }
                                else
                                {
                                    tmpns = thisNs.ManyToOne;

                                    tmpns.Form.Clear();

                                    $g('divManyToOneRelationshipHeading').innerHTML = 'n:1 Relationship Attribute: ' + data[3];

                                    $g(manyToOneGeneralTabDomIds.RelationshipName).value = data[3];
                                    $g(manyToOneGeneralTabDomIds.Description).value = data[4];
                                    $g(manyToOneGeneralTabDomIds.Tooltip).value = data[7];
                                    $g(manyToOneGeneralTabDomIds.Mandatory).checked = data[5];
                                    $g(manyToOneGeneralTabDomIds.BuiltIn).checked = data[11];

                                    if (CurrentUserInfo.AdminOverride) {
                                        $g(manyToOneGeneralTabDomIds.BuiltIn).disabled = false;
                                    }

                                    if ($g(manyToOneGeneralTabDomIds.BuiltIn).checked) {
                                        $g(manyToOneGeneralTabDomIds.BuiltIn).disabled = true;
                                    }

                                    relationshipIds.SelectedViewID = null;
                                    relationshipIds.SelectedDisplayField = data[8];
                                    relationshipIds.SelectedMatchFields = data[9];
                                    $g(relationshipDomIds.NtoOne.Modal.Fields.MaxRows).value = data[10];

                                    relationshipIds.RelatedTable = data[6];

                                    tmpns.Tabs.Filters.Tree.Refresh();
                                    SEL.Trees.Tree.Data.Load.DroppedNodes(relationshipDomIds.NtoOne.Modal.Filters.Drop, { entityId: ns.IDs.Entity, attributeId: ns.IDs.Attribute, formId: ns.IDs.Form, isParentFilter: SEL.CustomEntityAdministration.ParentFilter.IsParentFilter });
                                    tmpns.Tabs.LookupDisplayFields.Tree.Refresh();
                                    SEL.Trees.Tree.Data.Load.DroppedNodes(relationshipDomIds.NtoOne.Modal.LookupDisplayFields.Drop, { entityId: ns.IDs.Entity, attributeId: ns.IDs.Attribute });

                                }

                                ns.IDs.Relationship.SelectedRelationshipEntity = data[6];
                                tmpns.Modal.Show();
                            },
                            SEL.Common.WebService.ErrorHandler
                        );
                    },

                    ManyToOne:
                    {
                        Add: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                thisNs = ns.Attributes.Relationship.ManyToOne;

                            ns.IDs.Attribute = 0;
                            $g(ns.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity).disabled = false;

                            thisNs.Form.Clear();
                            thisNs.Modal.Show();
                        },

                        Form:
                        {
                            Clear: function ()
                            {
                                var ns = SEL.CustomEntityAdministration,
                                    relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.NtoOne.Modal,
                                    generalTabDomIds = relationshipModalDomIds.General,
                                    fieldsTabDomIds = relationshipModalDomIds.Fields,
                                    relationshipIds = ns.IDs.Relationship;

                                SEL.Common.Page_ClientValidateReset();

                                var cmbentity = $g(generalTabDomIds.RelationshipEntity);
                                cmbentity.selectedIndex = 0;
                                $g(generalTabDomIds.RelationshipName).value = '';
                                $g(generalTabDomIds.Description).value = '';
                                $g(generalTabDomIds.Tooltip).value = '';
                                $g(generalTabDomIds.Mandatory).checked = false;
                                $g(generalTabDomIds.BuiltIn).checked = false;
                                $g(generalTabDomIds.BuiltIn).disabled = !CurrentUserInfo.AdminOverride;

                                //                                if ($e(generalTabDomIds.RelationshipView))
                                //                                {
                                //                                    $g(generalTabDomIds.RelationshipView).options.length = 0;
                                //                                    $g(generalTabDomIds.RelationshipView).selectedIndex = -1;
                                //                                }
                                if ($e(fieldsTabDomIds.MatchFields))
                                {
                                    $g(fieldsTabDomIds.MatchFields).options.length = 0;
                                    $g(fieldsTabDomIds.MatchFields).selectedIndex = -1;
                                }
                                if ($e(fieldsTabDomIds.DisplayField))
                                {
                                    $g(fieldsTabDomIds.DisplayField).options.length = 0;
                                    $g(fieldsTabDomIds.DisplayField).selectedIndex = -1;
                                }
                                relationshipIds.SelectedRelationshipEntity = null;
                                relationshipIds.SelectedDisplayField = null;
                                relationshipIds.SelectedMatchFields = null;
                                relationshipIds.SelectedViewID = null;
                                $("#txtAutoLookupDisplayField").text("");
                                $g(fieldsTabDomIds.MaxRows).value = '';

                                var tabContain = $f(relationshipModalDomIds.TabContainer);
                                if (tabContain !== null)
                                {
                                    $f(relationshipModalDomIds.TabContainer).get_tabs()[1].set_enabled(false);
                                    $f(relationshipModalDomIds.TabContainer).get_tabs()[2].set_enabled(false);
                                    $f(relationshipModalDomIds.TabContainer).get_tabs()[3].set_enabled(false);
                                    $f(relationshipModalDomIds.TabContainer).get_tabs()[4].set_enabled(false);
                                }

                                SEL.Trees.Tree.Clear(relationshipModalDomIds.Filters.Tree);
                                SEL.Trees.Tree.Clear(relationshipModalDomIds.Filters.Drop);
                                SEL.Trees.Tree.Clear(relationshipModalDomIds.LookupDisplayFields.Tree);
                                SEL.Trees.Tree.Clear(relationshipModalDomIds.LookupDisplayFields.Drop);
                                return;
                            }
                        },

                        Modal:
                        {
                            Show: function ()
                            {
                                var ns = SEL.CustomEntityAdministration,
                                    thisNs = ns.Attributes.Relationship.ManyToOne,
                                    ids = ns.IDs,
                                    relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.NtoOne.Modal,
                                    generalTabDomIds = relationshipModalDomIds.General,
                                    relationshipIds = ids.Relationship,
                                    excludeExistingOneToManys = false,
                                    imgFilterHelp = $('#imgManyToOneFilterHelp'),
                                    imgLookupDisplayFieldHelp = $('#imgDisplayFieldHelp'),
                                    lookupDisplayFieldsHelp = $('#displayFieldHelpArea'),
                                    filterHelp = $('#relFilterHelpArea');
                                
                                ns.Base.RemoveAllShortCuts();
                                relationshipIds.RelationshipType = ns.Forms.RelationshipType.ManyToOne;

                                if (ids.Entity === 0)
                                {
                                    currentAction = 'addRelationship1';
                                    ns.Base.Save();
                                    return;
                                }

                                imgLookupDisplayFieldHelp.css('display', 'none');
                                imgLookupDisplayFieldHelp.mouseenter(
                                    function ()
                                {
                                    lookupDisplayFieldsHelp.css('zIndex', SEL.CustomEntityAdministration.zIndices.Views.HelpIcon());
                                    lookupDisplayFieldsHelp.css('left', $(this).offset().left - lookupDisplayFieldsHelp.outerWidth());
                                    lookupDisplayFieldsHelp.css('top', $(this).offset().top - lookupDisplayFieldsHelp.outerHeight());
                                    lookupDisplayFieldsHelp.css('position', 'absolute');
                                    lookupDisplayFieldsHelp.stop(true, true).fadeIn(400);
                                    }
                                ).mouseleave(
                                    function ()
                                    {
                                         lookupDisplayFieldsHelp.stop(true, true).fadeOut(200);
                                    }
                                );

                                imgFilterHelp.css('display', 'none');
                                imgFilterHelp.mouseenter(
                                    function ()
                                {
                                    filterHelp.css('zIndex', SEL.CustomEntityAdministration.zIndices.Views.HelpIcon());
                                    filterHelp.css('left', $(this).offset().left - filterHelp.outerWidth());
                                    filterHelp.css('top', $(this).offset().top - filterHelp.outerHeight());
                                    filterHelp.css('position', 'absolute');
                                    filterHelp.stop(true, true).fadeIn(400);
                                    }
                                ).mouseleave(
                                    function ()
                                    {
                                         filterHelp.stop(true, true).fadeOut(200);
                                    }
                                );

                                // $g('divManyToOneRelationshipTooltip').style.display = '';

                                if (ids.Attribute === 0)
                                {
                                    $g('divManyToOneRelationshipHeading').innerHTML = 'New n:1 Relationship Attribute';
                                }

                                $g(generalTabDomIds.MandatoryLabel).style.display = '';

                                $('#RelModContainer').css('height', '340px');
                                $("#txtAutoLookupDisplayField").keydown(false);
                                Spend_Management.svcCustomEntities.getRelationshipDropDown(ids.Entity, 1, false, excludeExistingOneToManys,
                                    function (data) {
                                        // populate ddlist with available relationships
                                        if ($e(generalTabDomIds.RelationshipEntity) === true)
                                        {
                                            var cmbrelationshiplist = $g(generalTabDomIds.RelationshipEntity);
                                            cmbrelationshiplist.options.length = 0;

                                            for (var i = 0; i < data.length; i++)
                                            {
                                                var option = document.createElement("OPTION");
                                                option.text = data[i].Text;
                                                option.value = data[i].Value;
                                                cmbrelationshiplist.options.add(option);

                                                if (data[i].Value === relationshipIds.SelectedRelationshipEntity)
                                                {
                                                    cmbrelationshiplist.selectedIndex = i;
                                                }
                                            }
                                        }

                                        var relModalID = relationshipModalDomIds.Control;
                                        var relModalZIndex = ns.zIndices.Attributes.Relationship.Modal;

                                        thisNs.GetLookupOptions();
                                        SEL.Common.ShowModal(relModalID);
                                        if (sessionStorage.getItem('formIdforN1') != null) 
                                        {
                                            $('#tabLDF_tab a[class="ajax__tab_tab"]').addClass('ajax__tab_disabled');
                                            $('#tabAutocompleteSearchResultsFields_tab a[class="ajax__tab_tab"]').addClass('ajax__tab_disabled');
                                            $('#tabRelFields_tab a[class="ajax__tab_tab"]').addClass('ajax__tab_disabled');
                                            $('#ctl00_contentmain_tabConRelFields_tabRelDefinition_tab').removeClass('ajax__tab_active');
                                            $('#ctl00_contentmain_tabConRelFields_tabRelDefinition_tab a[class="ajax__tab_tab"]').addClass('ajax__tab_disabled');
                                            $('#ctl00_contentmain_tabConRelFields_body > div').hide();
                                            $('#ctl00_contentmain_tabConRelFields_body > div').css('visibility', 'hidden');
                                            $('#ctl00_contentmain_pnlntoOnerelationship').addClass('nToManyRelationshipModalPopUp');
                                            $('#tabRelFilters').css('visibility', 'visible');
                                            $('#RelModContainer').css('height','480px'); 
                                            $('#tabRelFilters').show();
                                        }
                                        else {
                                            $('#ctl00_contentmain_tabConRelFields_tabRelDefinition_tab').addClass('ajax__tab_active');
                                            $('#ctl00_contentmain_tabConRelFields_tabRelDefinition_tab a[class="ajax__tab_tab ajax__tab_disabled"]').removeClass('ajax__tab_disabled');
                                            $('#ctl00_contentmain_pnlntoOnerelationship').removeClass('nToManyRelationshipModalPopUp');
                                            $('#ctl00_contentmain_tabConRelFields_body > div').css('visibility', 'visible');
                                            $('#ctl00_contentmain_tabConRelFields_tabRelDefinition').css('display', 'block');
                                        }
                                        $f(relModalID)._backgroundElement.style.zIndex = relModalZIndex - 5;
                                        $f(relModalID)._popupElement.style.zIndex = relModalZIndex;

                                        $('#' + generalTabDomIds.RelationshipName).select();

                                        $('#' + generalTabDomIds.RelationshipEntity).change(
                                            function ()
                                            {
                                            thisNs.Tabs.Filters.Tree.Refresh();
                                            thisNs.Tabs.LookupDisplayFields.Tree.Refresh();
                                            return false;
                                            }
                                        );
                                    },
                                    SEL.Common.WebService.ErrorHandler
                                );
                                // for ie7 modal display fix
                                $('#' + relationshipModalDomIds.TabContainer + '_body').css("height", '');
                            },

                            Close: function ()
                            {
                                // BUG FIX - Setting 'display none' has to be done here as well as on Modal Show for IE7
                                $('#imgManyToOneFilterHelp').css('display', 'none');
                                $('#imgDisplayFieldHelp').css('display', 'none');
                                SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
                                SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Control);
                            },

                            TabChange: function (sender)
                            {
                                var modalContainer = $('#RelModContainer');

                                if (modalContainer.is(':visible'))
                                {
                                    var ns = SEL.CustomEntityAdministration,
                                        modalNS = ns.DomIDs.Attributes.Relationship.NtoOne.Modal,
                                        newHeight = '340px',
                                        showInfoIcon = false,
                                        divToFade = [],
                                        imgFilterHelp = $('#imgManyToOneFilterHelp'),
                                        imgLookupDisplayFieldHelp = $('#imgDisplayFieldHelp');

                                    ns.Base.RemoveAllShortCuts();

                                    imgFilterHelp.css('display', 'none');
                                    imgLookupDisplayFieldHelp.css('display', 'none');

                                    switch (sender.get_activeTabIndex())
                                    {
                                    case 1:
                                        // Fields Tab
                                        ns.Base.AddShortCuts('relationshipModal');
                                        newHeight = '265px';
                                        divToFade.push(modalNS.Fields.Tab);
                                        showInfoIcon = false;
                                        break;
                                    case 2:
                                        // Lookup Display Fields Tab
                                        newHeight = '495px';
                                        divToFade.push(modalNS.LookupDisplayFields.Tab);
                                        divToFade.push(modalNS.LookupDisplayFields.Tree);
                                        divToFade.push(modalNS.LookupDisplayFields.Drop);
                                        showInfoIcon = 'displayfield';
                                        break;
                                    case 3:
                                        // Filters Tab
                                        newHeight = '495px';
                                        divToFade.push(modalNS.Filters.Tab);
                                        divToFade.push(modalNS.Filters.Tree);
                                        divToFade.push(modalNS.Filters.Drop);
                                        showInfoIcon = 'filter';
                                        break;
                                    case 4:
                                            // Filters Tab
                                            newHeight = '170px';
                                            divToFade.push(modalNS.AutocompleteFields.Tab);
                                            showInfoIcon = false;
                                            break;
                                    default:
                                        divToFade.push(modalNS.General.Tab);
                                        break;
                                    }

                                    for (var divElementIdx = 0; divElementIdx < divToFade.length; divElementIdx++)
                                    {
                                        $('#' + divToFade[divElementIdx]).stop(true, true).css('display', 'none');
                                    }

                                    modalContainer.stop(true, true).animate({ 'height': newHeight }, '600',
                                        function ()
                                        {
                                            if (showInfoIcon === 'filter') {
                                                imgFilterHelp.css('display', 'inline-block');
                                                imgFilterHelp.css('left', imgFilterHelp.parent().outerWidth() - 25);
                                                imgFilterHelp.css('top', imgFilterHelp.parent().outerHeight() + 55);
                                            }
                                            else if (showInfoIcon === 'displayfield')
                                            {
                                                imgLookupDisplayFieldHelp.css('display', 'inline-block');
                                                imgLookupDisplayFieldHelp.css('left', imgLookupDisplayFieldHelp.parent().outerWidth() - 25);
                                                imgLookupDisplayFieldHelp.css('top', imgLookupDisplayFieldHelp.parent().outerHeight() + 55);
                                            }
                                            
                                            // to correct ie7 animation issues we can remove the height attribute from the tab panels.
                                            $('#' + modalNS.TabContainer + '_body').css("height", '');
                                        }
                                    );

                                    for (divElementIdx = 0; divElementIdx < divToFade.length; divElementIdx++)
                                    {
                                        $('#' + divToFade[divElementIdx]).fadeIn(600);
                                    }

                                    if (sender.get_activeTabIndex() == 2)
                                    {
                                        SEL.Trees.Node.Disable(modalNS.LookupDisplayFields.Tree, null, 'refresh');
                                    }
                                } 
                            }
                        },

                        GetLookupOptions: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                ids = ns.IDs,
                                relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.NtoOne.Modal,
                                generalTabDomIds = relationshipModalDomIds.General,
                                fieldsTabDomIds = relationshipModalDomIds.Fields,
                                relationshipIds = ids.Relationship;

                            if ($e(generalTabDomIds.RelationshipEntity) === true)
                            {
                                var cmbrelationshiplist = $g(generalTabDomIds.RelationshipEntity);

                                var enable = (cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value !== '0');
                                $f(relationshipModalDomIds.TabContainer).get_tabs()[1].set_enabled(enable);
                                $f(relationshipModalDomIds.TabContainer).get_tabs()[2].set_enabled(enable);
                                $f(relationshipModalDomIds.TabContainer).get_tabs()[3].set_enabled(enable);
                                $f(relationshipModalDomIds.TabContainer).get_tabs()[4].set_enabled(enable);

                                Spend_Management.svcAutoComplete.getRelationshipLookupOptions(cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value, [],
                                        function (data)
                                        {
                                            if (data === null)
                                                return;

                                            var idx;
                                            var option;

                                            if ($e(fieldsTabDomIds.DisplayField) === true)
                                            {
                                                var displayCntl = $g(fieldsTabDomIds.DisplayField);
                                                if (displayCntl !== null)
                                                {
                                                    displayCntl.options.length = 0;
                                                    $g(fieldsTabDomIds.MatchFields).options.length = 0;
                                                    option = document.createElement("OPTION");
                                                    option.text = ns.Messages.NoneOption;
                                                    option.value = '0';
                                                    displayCntl.options.add(option);

                                                    for (idx = 0; idx < data.length; idx++)
                                                    {
                                                        option = document.createElement("OPTION");
                                                        option.text = data[idx].Text;
                                                        option.value = data[idx].Value;
                                                        if (data[idx].Value === relationshipIds.SelectedDisplayField)
                                                            option.selected = true;

                                                        displayCntl.options.add(option);
                                                    }
                                                }
                                            }
                                            
                                            if (ids.Attribute !== 0)
                                            {
                                                Spend_Management.svcCustomEntities.getRelationshipMatchSelections(ids.Attribute,
                                                            function (d)
                                                            {
                                                                if ($e(fieldsTabDomIds.MatchFields) === true)
                                                                {
                                                                    var matchCntl = $g(fieldsTabDomIds.MatchFields);
                                                                    if (matchCntl !== null)
                                                                    {
                                                                        matchCntl.options.length = 0;

                                                                        for (var idx = 0; idx < d.length; idx++)
                                                                        {
                                                                            var option = document.createElement("OPTION");
                                                                            option.text = d[idx].Text;
                                                                            option.value = d[idx].Value;
                                                                            matchCntl.options.add(option);
                                                                        }
                                                                    }
                                                                }
                                                            },
                                                    SEL.Common.WebService.ErrorHandler
                                                );
                                                Spend_Management.svcCustomEntities.GetAutocompleteResultDisplaySelection(ids.Attribute,
                                                           function (d) {
                                                               var fields = '';
                                                               for (var idx = 0; idx < d.length; idx++) {
                                                                          fields = fields + "[" + d[idx].Text + "]" + ", ";
                                                                       }
                                                                       $('#txtAutoLookupDisplayField').text(fields.substring(0, fields.length - 2));
                                                           },
                                                   SEL.Common.WebService.ErrorHandler
                                               );
                                            }
                                        },
                                    SEL.Common.WebService.ErrorHandler
                                );
                            }
                        },

                        Save: function ()
                        {
                            EnableAttributeValidators('relationship');

                            if (validateform('vgManyToOneRelationship') === false)
                            {
                                return;
                            }

                            var ns = SEL.CustomEntityAdministration,
                                ids = SEL.CustomEntityAdministration.IDs,
                                entityId = ids.Entity,
                                attributeId = ids.Attribute,
                                formId = ids.Form,
                                    misc = ns.Misc,
                                    messages = ns.Messages,
                                    relationshipMessages = messages.Attributes.Relationship,
                                relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.NtoOne.Modal,
                                generalTabDomIds = relationshipModalDomIds.General,
                                fieldsTabDomIds = relationshipModalDomIds.Fields,
                                    attributename,
                                    description,
                                    tooltip,
                                    mandatory,
                                    builtIn,
                                    cmbrelationshipentity,
                                    tableid,
                                    filtersDropId = relationshipModalDomIds.Filters.Drop,
                                    filters,
                                    displayFieldCntl,
                                    displayfieldId = null,
                                    matchFieldIDs = [],
                                    autocompleteFieldIds = [],
                                    maxRows = 15,
                                    lookupDisplayFieldsDropId = relationshipModalDomIds.LookupDisplayFields.Drop,
                                    lookupdisplayfields;

                            misc.LoadingScreenCancelled = false;
                            setTimeout(function ()
                            {
                                misc.ShowInformationMessage('Saving...');
                            },
                            300);

                            filters = SEL.Trees.Tree.Data.Get(filtersDropId, ['metadata']);
                            lookupdisplayfields = SEL.Trees.Tree.Data.Get(lookupDisplayFieldsDropId, ['metadata']);
                            attributename = $g(generalTabDomIds.RelationshipName).value;
                            description = $g(generalTabDomIds.Description).value;
                            tooltip = $g(generalTabDomIds.Tooltip).value;
                            mandatory = $g(generalTabDomIds.Mandatory).checked;
                            builtIn = $g(generalTabDomIds.BuiltIn).checked;
                            cmbrelationshipentity = $g(generalTabDomIds.RelationshipEntity);
                            tableid = cmbrelationshipentity.options[cmbrelationshipentity.selectedIndex].value;

                            if ($e(fieldsTabDomIds.DisplayField) === true)
                            {
                                displayFieldCntl = $g(fieldsTabDomIds.DisplayField);
                                displayfieldId = displayFieldCntl.options[displayFieldCntl.selectedIndex].value;
                            }
                            if ($e(fieldsTabDomIds.DisplayField) === true) {
                                displayFieldCntl = $g(fieldsTabDomIds.DisplayField);
                                displayfieldId = displayFieldCntl.options[displayFieldCntl.selectedIndex].value;
                            }
                            var textFields = $('#txtAutoLookupDisplayField').text();
                            textFields = textFields.replace(/\[/g, '').replace(/\' '/g, '');
                            var array = textFields.split(/(?:], )+/);
                            array[array.length - 1] = array[array.length - 1].replace(']', '');
                            for (var fieldIds = 0; fieldIds < array.length; fieldIds++) {
                                autocompleteFieldIds.push(array[fieldIds]);
                            }
                            if ($e(fieldsTabDomIds.MatchFields) === true)
                            {
                                var matchFieldsCntl = $g(fieldsTabDomIds.MatchFields);
                                for (var idx = 0; idx < matchFieldsCntl.options.length; idx++)
                                {
                                    matchFieldIDs.push(matchFieldsCntl[idx].value);
                                }
                            }

                            if ($e(fieldsTabDomIds.MaxRows) === true)
                            {
                                var maxRowsCntl = $g(fieldsTabDomIds.MaxRows);
                                if (maxRowsCntl.value !== '' && maxRowsCntl.value !== '0')
                                {
                                    maxRows = new Number(maxRowsCntl.value);
                                }
                            }

                            Spend_Management.svcCustomEntities.saveManyToOneRelationship(cu.AccountID, employeeid, entityId, attributeId, attributename, description, tooltip, mandatory, builtIn, tableid, displayfieldId, matchFieldIDs, autocompleteFieldIds, maxRows, filters, lookupdisplayfields, formId, SEL.CustomEntityAdministration.ParentFilter.IsParentFilter,
                                function (data)
                                {
                                    if (data === -1)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup(relationshipMessages.DuplicateName, messages.ModalTitle);
                                    }
                                    else if (data === -2)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup(relationshipMessages.RecursionWarning, messages.ModalTitle);
                                    }
                                    else
                                    {
                                        SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Modal.Close();
                                        Spend_Management.svcCustomEntities.getAttributesGrid(entityId, RefreshAttributeGridComplete);
                                    }

                                    misc.LoadingScreenCancelled = true;
                                    $('#loadingArea').fadeOut(600, function ()
                                    {
                                        $('#loadingArea').remove();
                                    });                                

                                    // if the attribute is built-in/system the GreenLight will be too (if it wasn't already, it will be now), so update the checkbox for the GreenLight automatically.
                                    if (builtIn && !$("#ctl00_contentmain_chkBuiltIn").is(":checked")) {
                                        $("#ctl00_contentmain_chkBuiltIn").prop("checked", true).prop("disabled", true);
                                    }
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        },

                        FieldItem:
                        {
                            Modal:
                            {
                                Show: function (bEditMode)
                                {
                                    if (bEditMode)
                                    {
                                        SEL.Common.Page_ClientValidateReset();
                                    }
                                    var arrSelections = [];

                                    if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields) === true && $e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity) === true)
                                    {
                                        var fielditemcntl = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields);
                                        var cmbrelationshiplist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity);

                                        for (var idx = 0; idx < fielditemcntl.length; idx++)
                                        {
                                            arrSelections.push(fielditemcntl.options[idx].value);
                                        }
                                        Spend_Management.svcAutoComplete.getRelationshipLookupOptions(cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value, arrSelections,
                                                                function (data)
                                                                {
                                                                    if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.FieldItems) === true)
                                                                    {
                                                                        var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.FieldItems);
                                                                        fielditemlist.options.length = 0;

                                                                        var option = document.createElement("OPTION");
                                                                        option.text = SEL.CustomEntityAdministration.Messages.NoneOption;
                                                                        option.value = '0';
                                                                        fielditemlist.options.add(option);

                                                                        for (var idx = 0; idx < data.length; idx++)
                                                                        {
                                                                            option = document.createElement("OPTION");
                                                                            option.text = data[idx].Text;
                                                                            option.value = data[idx].Value;
                                                                            fielditemlist.options.add(option);
                                                                        }
                                                                    }
                                                                    SEL.Common.ShowModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.Modal.Control);
                                                                },
                                            SEL.Common.WebService.ErrorHandler
                                        );
                                    }
                                },

                                Close: function ()
                                {
                                    SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.Modal.Control);
                                }
                            },

                            Save: function ()
                            {
                                if (validateform('vgFieldItemList') === false)
                                    return false;

                                var selection = '';

                                if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.FieldItems) === true)
                                {
                                    var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.FieldItems);
                                    selection = fielditemlist.options[fielditemlist.selectedIndex];
                                }

                                if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields) === true)
                                {
                                    var matchCntl = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields);
                                    if (matchCntl !== null)
                                    {
                                        var option = document.createElement('OPTION');
                                        option.text = selection.text;
                                        option.value = selection.value;
                                        matchCntl.options.add(option);
                                        var req = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldsReqValidator);
                                        req.isvalid = true;
                                        ValidatorUpdateDisplay(req);
                                    }
                                }

                                SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Modal.Close();

                                return false;
                            },

                            Remove: function ()
                            {
                                if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields) === true)
                                {
                                    var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields);

                                    if (fielditemlist.selectedIndex !== -1)
                                    {
                                        fielditemlist.remove(fielditemlist.selectedIndex);
                                        if (fielditemlist.length === 0)
                                        {
                                            var req = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldsReqValidator);
                                            req.isvalid = false;
                                            ValidatorUpdateDisplay(req);
                                        }
                                    }
                                    else
                                    {
                                        var msg = SEL.CustomEntityAdministration.Messages.Attributes.Relationship.SelectRemovalItem;

                                        if (fielditemlist.length === 0)
                                        {
                                            msg = SEL.CustomEntityAdministration.Messages.Attributes.Relationship.NoRemovalItems;
                                        }

                                        SEL.MasterPopup.ShowMasterPopup(msg, SEL.CustomEntityAdministration.Messages.ModalTitle);
                                    }
                                }
                            }
                        },

                        AutocompleteDisplayFieldItem:
                       {
                           Modal:
                           {
                               Show: function (bEditMode) {
                                   if (bEditMode) {
                                       SEL.Common.Page_ClientValidateReset();
                                   }
                                   var arrSelections = [];
                                   var cmbrelationshiplist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity);
                                   
                                   var textFields = $('#txtAutoLookupDisplayField').text();
                                   textFields = textFields.replace(/\[/g, '').replace(/\]/g, '').replace(/\' '/g, '');

                                   //Remove the field from selection list if the field is already added as Display field , to avoid repeated data in display field and additional display field 
                                   var fieldsTabDomIds = SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship
                                   .NtoOne.Modal.Fields;
                                   var selectedDisplayField;
                                   if ($e(fieldsTabDomIds.DisplayField) && $("#cmbmtodisplayfield option:selected").text() !== SEL.CustomEntityAdministration.Messages.NoneOption) {
                                       selectedDisplayField = $("#cmbmtodisplayfield option:selected").text();
                                       textFields = textFields + ", " + selectedDisplayField;
                                   }
                                  
                                   var array = textFields.split(', ');
                                       for (var fieldIds = 0; fieldIds < array.length; fieldIds++) {
                                           arrSelections.push(array[fieldIds]);
                                       }
                                       Spend_Management.svcAutoComplete.GetAutocompleteSearchResultsFields(cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value, arrSelections,
                                                               function (data) {
                                                                   if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems) === true) {
                                                                       var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems);
                                                                       fielditemlist.options.length = 0;
                                                                      var option = document.createElement("OPTION");
                                                                       option.text = SEL.CustomEntityAdministration.Messages.NoneOption;
                                                                       option.value = '0';
                                                                       fielditemlist.options.add(option);

                                                                       for (var idx = 0; idx < data.length; idx++) {
                                                                           option = document.createElement("OPTION");
                                                                           option.text = data[idx].Text;
                                                                           option.value = data[idx].Value;
                                                                           fielditemlist.options.add(option);
                                                                       }
                                                                   }
                                                                   SEL.Common.ShowModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Control);
                                                               },
                                           SEL.Common.WebService.ErrorHandler
                                       );
                               },

                               Close: function () {
                                   SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Control);
                               }
                           },

                           Save: function () {
                               var selection = '';
                               if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems) === true) {
                                   var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems);
                                   selection = fielditemlist.options[fielditemlist.selectedIndex];                                  
                               }
                               if (selection.text !== SEL.CustomEntityAdministration.Messages.NoneOption) {

                                   if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal
                                           .AutocompleteFields.AutocompleteDisplayFields) ===
                                       true) {
                                       var matchCntl = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship
                                           .NtoOne.Modal.AutocompleteFields.AutocompleteDisplayFields);
                                       if (matchCntl !== null) {
                                           var fields = $('#txtAutoLookupDisplayField').text();
                                           if (fields.length > 0)
                                               fields = fields + ", ";
                                           $('#txtAutoLookupDisplayField').text(fields + "[" + selection.text + "]");
                                       }
                                   }
                               }

                               SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.AutocompleteDisplayFieldItem.Modal.Close();

                               return false;
                           },

                           Remove: function () {
                               if ($e(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteDisplayFields) === true) {
                                   var fielditemlist = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteDisplayFields);
                                   var start = $("#txtAutoLookupDisplayField").prop("selectionStart");
                                   // obtain the index of the last selected character
                                   var finish = $('#txtAutoLookupDisplayField').prop("selectionEnd");
                                   var sel;
                                   if (start !== finish) {
                                       if (fielditemlist.value.substring(start, finish).length ===
                                           $('#txtAutoLookupDisplayField').text().length) {
                                           sel = fielditemlist.value.substring(start, finish);
                                       } else if (finish === $("#txtAutoLookupDisplayField").text().length)
                                           sel = ", " + fielditemlist.value.substring(start, finish);
                                       else
                                           sel = fielditemlist.value.substring(start, finish) + ", ";
                                       $("#txtAutoLookupDisplayField").text($('#txtAutoLookupDisplayField').text().replace(sel, ""));
                                   }
                                   else {
                                       var msg = SEL.CustomEntityAdministration.Messages.Attributes.Relationship.SelectAutocompleteRemovalItem;

                                       if ($('#txtAutoLookupDisplayField').text().length === 0) {
                                           msg = SEL.CustomEntityAdministration.Messages.Attributes.Relationship.NoAutocompleteRemovalItems;
                                       }

                                       SEL.MasterPopup.ShowMasterPopup(msg, SEL.CustomEntityAdministration.Messages.ModalTitle);
                                   }
                               }
                           }
                       },

                        ValidateFieldMatchList: function (sender, args)
                        {
                            var options = $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields).options;
                            if (options.length > 0)
                            {
                                args.IsValid = true;
                            }
                            else
                            {
                                args.IsValid = false;
                            }
                        },

                        Tabs: {
                            Filters: {
                                Tree: {
                                    Refresh: function ()
                                    {
                                        var ns = SEL.CustomEntityAdministration,
                                            treeId = ns.DomIDs.Attributes.Relationship.NtoOne.Modal.Filters.Tree;

                                        var dropDown = $('#' + ns.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity);

                                        if (dropDown.val() !== null && dropDown.val() !== '0')
                                        {
                                            ns.IDs.Relationship.RelatedTable = dropDown.val();
                                        }

                                        SEL.Trees.Tree.Clear(treeId);

                                        if (ns.IDs.Relationship.RelatedTable !== null)
                                        {
                                            SEL.Trees.Tree.Data.Load.InitialNodes(treeId, { relatedTable: ns.IDs.Relationship.RelatedTable }, null, ns.Misc.ErrorHandler);
                                        }
                                    }
                                },

                                Drop: {
                                    Refresh: function () {
                                        var ns = SEL.CustomEntityAdministration,
                                            ids = ns.IDs,
                                            dropId = ns.DomIDs.Attributes.Relationship.NtoOne.Modal.Filters.Drop;

                                        SEL.Trees.Tree.Clear(dropId);
                                        SEL.Trees.Tree.Data.Load.DroppedNodes(dropId, { entityId: ids.Entity, attributeId: ids.Attribute, formId: ids.Form }, ns.Attributes.Relationship.ManyToOne.Modal.Show, ns.Misc.ErrorHandler);
                                    }
                                }
                            },

                            LookupDisplayFields: {
                                Tree: {
                                    Refresh: function ()
                                    {
                                        var ns = SEL.CustomEntityAdministration,
                                                treeId = ns.DomIDs.Attributes.Relationship.NtoOne.Modal.LookupDisplayFields.Tree;

                                        var dropDown = $('#' + ns.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity);

                                        if (dropDown.val() !== null && dropDown.val() !== '0')
                                        {
                                            ns.IDs.Relationship.RelatedTable = dropDown.val();
                                        }

                                        SEL.Trees.Tree.Clear(treeId);

                                        if (ns.IDs.Relationship.RelatedTable !== null)
                                        {
                                            SEL.Trees.Tree.Data.Load.InitialNodes(treeId, { relatedTable: ns.IDs.Relationship.RelatedTable }, null, ns.Misc.ErrorHandler);
                                        }
                                    }
                                },

                                Drop: {
                                    Refresh: function ()
                                    {
                                        var ns = SEL.CustomEntityAdministration,
                                            ids = ns.IDs,
                                            dropId = ns.DomIDs.Attributes.Relationship.NtoOne.Modal.LookupDisplayFields.Drop;

                                        SEL.Trees.Tree.Clear(dropId);
                                        SEL.Trees.Tree.Data.Load.DroppedNodes(dropId, { entityId: ids.Entity, attributeId: ids.Attribute }, ns.Attributes.Relationship.ManyToOne.Modal.Show, ns.Misc.ErrorHandler);
                                    }
                                }
                            }
                        }
                    },

                    OneToMany:
                    {
                        Add: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                            thisNs = ns.Attributes.Relationship.OneToMany;

                            ns.IDs.Attribute = 0;
                            $g(ns.DomIDs.Attributes.Relationship.OnetoN.Modal.General.RelationshipEntity).disabled = false;

                            thisNs.Form.Clear();
                            thisNs.Modal.Show();
                        },

                        Form: {
                            Clear: function ()
                            {
                                var ns = SEL.CustomEntityAdministration,
                                relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.OnetoN.Modal,
                                generalTabDomIds = relationshipModalDomIds.General,
                                relationshipIds = ns.IDs.Relationship;

                                SEL.Common.Page_ClientValidateReset();
                                var cmbentity = $g(generalTabDomIds.RelationshipEntity);
                                cmbentity.selectedIndex = 0;
                                $g(generalTabDomIds.RelationshipName).value = '';
                                $g(generalTabDomIds.Description).value = '';

                                $g(generalTabDomIds.BuiltIn).checked = false;
                                $g(generalTabDomIds.BuiltIn).disabled = !CurrentUserInfo.AdminOverride;

                                if ($e(generalTabDomIds.RelationshipView))
                                {
                                    $g(generalTabDomIds.RelationshipView).options.length = 0;
                                    $g(generalTabDomIds.RelationshipView).selectedIndex = -1;
                                }

                                relationshipIds.SelectedRelationshipEntity = null;
                                relationshipIds.SelectedViewID = null;
                                return;
                            }
                        },

                        Modal: {
                            Show: function () {
                                var ns = SEL.CustomEntityAdministration,
                                    thisNs = ns.Attributes.Relationship.OneToMany,
                                    ids = ns.IDs,
                                    relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.OnetoN.Modal,
                                    generalTabDomIds = relationshipModalDomIds.General,
                                    relationshipIds = ids.Relationship,
                                    excludeExistingOneToManys = false;

                                ns.Base.RemoveAllShortCuts();
                                relationshipIds.RelationshipType = ns.Forms.RelationshipType.OneToMany;

                                if (ids.Entity === 0)
                                {
                                    currentAction = 'addRelationship2';
                                    ns.Base.Save();
                                    return;
                                }

                                $('#imgManyToOneFilterHelp').css('display', 'none');

                                //                                $g('divonetonRelationshipTooltip').style.display = 'none';

                                if (ids.Attribute === 0)
                                {
                                    $g('divOneToManyRelationshipHeading').innerHTML = 'New 1:n Relationship Attribute';
                                    excludeExistingOneToManys = true;
                                }

                                $('#RelModContainer').css('height', '280px');
                               
                                Spend_Management.svcCustomEntities.getRelationshipDropDown(ids.Entity, relationshipIds.RelationshipType, false, excludeExistingOneToManys,
                                    function (data) {
                                        // populate ddlist with available relationships
                                        if ($e(generalTabDomIds.RelationshipEntity) === true)
                                        {
                                            var cmbrelationshiplist = $g(generalTabDomIds.RelationshipEntity);
                                            cmbrelationshiplist.options.length = 0;

                                            for (var i = 0; i < data.length; i++)
                                            {
                                                var option = document.createElement("OPTION");
                                                option.text = data[i].Text;
                                                option.value = data[i].Value;
                                                cmbrelationshiplist.options.add(option);

                                                if (data[i].Value === relationshipIds.SelectedRelationshipEntity)
                                                    cmbrelationshiplist.selectedIndex = i;
                                            }
                                        }

                                        var relModalID = relationshipModalDomIds.Control;
                                        var relModalZIndex = ns.zIndices.Attributes.Relationship.Modal;

                                        thisNs.GetViews();
                                        SEL.Common.ShowModal(relModalID);

                                        $f(relModalID)._backgroundElement.style.zIndex = relModalZIndex - 5;
                                        $f(relModalID)._popupElement.style.zIndex = relModalZIndex;

                                        $('#' + generalTabDomIds.RelationshipName).select();
                                    },
                                    SEL.Common.WebService.ErrorHandler
                                );
                            },

                            Close: function ()
                            {
                                // BUG FIX - Setting 'display none' has to be done here as well as on Modal Show for IE7
                                $('#imgManyToOneFilterHelp').css('display', 'none');
                                SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
                                SEL.Common.HideModal(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.OnetoN.Modal.Control);
                            }
                        },

                        GetViews: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                ids = ns.IDs,
                            relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.OnetoN.Modal,
                            generalTabDomIds = relationshipModalDomIds.General,
                            relationshipIds = ids.Relationship;

                            if (relationshipIds.RelationshipType === null || relationshipIds.RelationshipType === ns.Forms.RelationshipType.ManyToOne)
                            {
                                return;
                            }
                            var entityid = $g(generalTabDomIds.RelationshipEntity).options[$g(generalTabDomIds.RelationshipEntity).selectedIndex].value;
                            Spend_Management.svcCustomEntities.getRelationshipViews(cu.AccountID, entityid,ns.IDs.Attribute,
                                function (data)
                                {
                                    var cmbviews = document.getElementById(generalTabDomIds.RelationshipView);
                                    var selectedIdx = -1;

                                    cmbviews.options.length = 0;

                                    for (var i = 0; i < data.length; i++)
                                    {
                                        var option = document.createElement("OPTION");
                                        option.text = data[i].Text;
                                        option.value = data[i].Value;
                                        cmbviews.options.add(option);

                                        var checkId = parseInt(data[i].Value, 10);
                                        if (checkId === relationshipIds.SelectedViewID)
                                            selectedIdx = i;
                                    }

                                    if (selectedIdx > -1)
                                    {
                                        cmbviews.selectedIndex = selectedIdx;
                                    }
                                },
                                SEL.Common.WebService.ErrorHandler
                            );
                        },

                        Save: function ()
                        {
                            EnableAttributeValidators('relationship');
                            if (validateform('vgOneToManyRelationship') === false)
                            {
                                return;
                            }

                            var ns = SEL.CustomEntityAdministration,
                                ids = SEL.CustomEntityAdministration.IDs,
                                entityId = ns.IDs.Entity,
                                attributeId = ids.Attribute,
                                relationshipModalDomIds = ns.DomIDs.Attributes.Relationship.OnetoN.Modal,
                                generalTabDomIds = relationshipModalDomIds.General,
                                messages = ns.Messages,
                                relationshipMessages = messages.Attributes.Relationship,
                                attributename = $g(generalTabDomIds.RelationshipName).value,
                                description = $g(generalTabDomIds.Description).value,
                                builtIn = $g(generalTabDomIds.BuiltIn).checked,
                                tooltip = '',
                                mandatory = false,
                                cmbrelationshipentity = $g(generalTabDomIds.RelationshipEntity),
                                tableid = cmbrelationshipentity.options[cmbrelationshipentity.selectedIndex].value,
                                viewid = 0;
                            //                            fieldsTabDomIds = relationshipModalDomIds.Fields,

                            if ($g(generalTabDomIds.RelationshipView).options.length > 0)
                            {
                                viewid = $g(generalTabDomIds.RelationshipView).options[$g(generalTabDomIds.RelationshipView).selectedIndex].value;
                            }

                            Spend_Management.svcCustomEntities.saveOneToManyRelationship(cu.AccountID, employeeid, ns.IDs.Entity, attributeId, attributename, description, builtIn, tooltip, mandatory, tableid, viewid,SEL.CustomEntityAdministration.ParentFilter.IsParentFilter,
                                function (data)
                                {
                                    if (data === -1)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup(relationshipMessages.DuplicateName, messages.ModalTitle);
                                    }
                                    else if (data === -2)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup(relationshipMessages.RecursionWarning, messages.ModalTitle);
                                    }
                                    else
                                    {
                                        ns.Attributes.Relationship.OneToMany.Modal.Close();
                                        Spend_Management.svcCustomEntities.getAttributesGrid(ns.IDs.Entity, RefreshAttributeGridComplete);

                                        // if the attribute is built-in/system the GreenLight will be too (if it wasn't already, it will be now), so update the checkbox for the GreenLight automatically.
                                        if (builtIn && !$("#ctl00_contentmain_chkBuiltIn").is(":checked")) {
                                            $("#ctl00_contentmain_chkBuiltIn").prop("checked", true).prop("disabled", true);
                                        }
                                    }
                                },
                                SEL.Common.WebService.ErrorHandler
                            );
                        }
                    }
                },
                
                /// <comment>
                /// Determines whether or not an attribute is the form selection attribute for the custom entity
                /// </comment>
                IsFormSelectionAttribute: function (attributeId)
                {
                    return (+$("#" + SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute).val() === attributeId);
                }
            },

            /// <comment>
            /// This contains the methods and parameters relevant to the Forms section of custom entity admin
            /// </comment>
            Forms: {
                //TODO: Get the number of global variables down. WAY DOWN.
                FormObj: null,
                FieldsUsedOnForm: [],
                FormFieldDetails: [],
                NumberOfSpacers: 1,
                NumberOfTabs: 0,
                NumberOfSections: 0,
                SelectedTab: null,
                SelectedSection: null,
                EditTabObject: null,
                EditTabMode: false,
                EditSectionMode: false,
                EditSectionObject: null,
                EditLabelField: null,
                CurrentlyDragging: false,
                SortEventCancelled: false,
                ControlTypeObject: { Tab: 0, Section: 1, Field: 2 },
                //TODO: Put variables such as '...ID' into custom attributes
                CurrentFormID: 0,

                FormsWebServiceError: function (data)//(ಠ_ಠ)
                {
                    SEL.CustomEntityAdministration.Misc.LoadingScreenCancelled = true;
                    $('#loadingArea').remove();

                    SEL.MasterPopup.ShowMasterPopup(
                        'An error has occurred processing your request.<span style="display:none;">' +
                            (data._message ? data._message : data) + '</span>',
                        'Message from ' + moduleNameHTML
                    );
                },

                // TODO: Update this
                NewForm: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;
                    sessionStorage.setItem('formIdforN1', -1);
                    if (entityid === 0)
                    {
                        currentAction = 'addForm';
                        SEL.CustomEntityAdministration.Base.Save();
                        return;
                    }

                    //$g('formTabs').innerHTML = '';
                    thisNs.ResetFormButtonTextLabels();
                    thisNs.EditTabObject = null;
                    thisNs.EditTabMode = false;
                    thisNs.ShowFormModal(true);
                    thisNs.FieldsUsedOnForm = [];
                    thisNs.NumberOfSpacers = 1;
                    thisNs.NumberOfTabs = 0;
                    thisNs.NumberOfSections = 0;
                    thisNs.SelectedTab = null;
                    thisNs.SelectedSection = null;
                    $('#divFormSectionHeader').text('New Form');
                    thisNs.CreateNoTabsAndSectionsComments();

                    if (CurrentUserInfo.AdminOverride) {
                        $('#' + chkFormBuiltIn).prop('disabled', false);
                    }
                },

                EditForm: function (formid)
                {
                    sessionStorage.setItem('formIdforN1', formid);
                    var misc = SEL.CustomEntityAdministration.Misc;
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    document.getElementById('formTabs').innerHTML = '';
                    document.getElementById('tabBar').innerHTML = '';
                    document.getElementById('addTabHolder').innerHTML = '';

                    // TODO: As per the comment at the top, change the line below to set a custom attribute on... say... the save button
                    thisNs.CurrentFormID = formid;

                    misc.LoadingScreenCancelled = false;

                    setTimeout(function () { misc.ShowInformationMessage('Loading...'); }, 300);

                    Spend_Management.svcCustomEntities.getForm(cu.AccountID, entityid, formid, thisNs.GetFormComplete, thisNs.FormsWebServiceError);
                },

                ShowCopyModal: function (formid)
                {
                    // Reset the required field validator
                    var validator = $g(reqNewFormNameID);
                    validator.isvalid = true;
                    ValidatorUpdateDisplay(validator);

                    var formNameObj = $('#' + txtcopyformnameid);

                    Spend_Management.svcCustomEntities.GetFormNameById(entityid, formid,
                        function (data)
                        {
                            // Take a substring so the name does not exceed 100 characters
                            formNameObj.val(data.substring(0, 93) + ' (Copy)');

                            formNameObj.attr('copyformid', formid);

                            SEL.Common.ShowModal(modcopyformid);

                            formNameObj.select();
                        },
                        SEL.CustomEntityAdministration.Misc.ErrorHandler
                    );
                },

                HideCopyModal: function ()
                {
                    // Clear the stored 'copyformid' attribute
                    $('#' + txtcopyformnameid).attr('copyformid', '');
                    SEL.Common.HideModal(modcopyformid);
                    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
                },

                Copy: function ()
                {
                    if (validateform('vgCopyForm') === false)
                    {
                        return;
                    }
                    var misc = SEL.CustomEntityAdministration.Misc;
                    var thisNs = SEL.CustomEntityAdministration.Forms;
                    var formNameObj = $('#' + txtcopyformnameid);
                    var newFormName = EscapeHTMLInString(formNameObj.val());

                    misc.LoadingScreenCancelled = false;

                    setTimeout(function () { misc.ShowInformationMessage('Creating copy...'); }, 50);

                    Spend_Management.svcCustomEntities.CopyForm(entityid, formNameObj.attr('copyformid'), newFormName,
                        function (data)
                        {
                            if (data < 0)
                            {
                                misc.LoadingScreenCancelled = true;
                                $('#loadingArea').remove();

                                switch (data)
                                {
                                    case -1:
                                        SEL.MasterPopup.ShowMasterPopup('The Form name you have entered already exists.',
                                            'Message from ' + moduleNameHTML);
                                        break;
                                    case -6:
                                        SEL.MasterPopup.ShowMasterPopup('Please enter a New Form name.',
                                            'Message from ' + moduleNameHTML);
                                        break;
                                    case -9:
                                        SEL.MasterPopup.ShowMasterPopup('This Form cannot be copied as it is invalid.',
                                            'Message from ' + moduleNameHTML);
                                        break;
                                }
                                return;
                            }

                            Spend_Management.svcCustomEntities.getFormGrid(entityid, thisNs.RefreshFormGridComplete);

                            misc.LoadingScreenCancelled = true;

                            thisNs.HideCopyModal();

                            $('#loadingArea').fadeOut(600, function ()
                            {
                                $('#loadingArea').remove();
                            });
                        },
                        SEL.CustomEntityAdministration.Misc.ErrorHandler
                    );
                },

                // TODO: Moved, NOT refactored
                GetFormComplete: function (data)
                {
                    var _misc = SEL.CustomEntityAdministration.Misc;
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var form = data[0];
                    var numberOfFields = data[1];

                    if (numberOfFields > 30)
                    {
                        _misc.ShowInformationMessage('Loading...');
                    }
                    else
                    {
                        _misc.LoadingScreenCancelled = true;
                    }

                    // Uses setTimeout to ensure that the loading message has time to display
                    setTimeout(function ()
                    {
                        thisNs.FieldsUsedOnForm = [];
                        thisNs.NumberOfSpacers = 1;
                        thisNs.NumberOfTabs = 0;
                        thisNs.NumberOfSections = 0;
                        thisNs.SelectedTab = null;
                        thisNs.EditTabObject = null;
                        thisNs.SelectedSection = null;
                        thisNs.EditTabMode = false;

                        var fields = null;
                        var sectionsInCurrentTab = 0;

                        $g(txtformnameid).value = form.formName;
                        $('#divFormSectionHeader').text('Form: ' + form.formName);
                        $g(txtformdescriptionid).value = form.description;
                        $g(txtformsavebuttontextid).value = form.saveButtonText;
                        $g(txtformsaveandduplicatebuttontextid).value = form.saveAndDuplicateButtonText;
                        $g(txtformsaveandstaybuttontextid).value = form.saveAndStayButtonText;
                        $g(txtformsaveandnewbuttontextid).value = form.saveAndNewButtonText;
                        $g(txtformcancelbuttontextid).value = form.cancelButtonText;
                        $g(chkshowsubmenuid).checked = form.showSubMenus;
                        $g(chkshowbreadcrumbsid).checked = form.showBreadcrumbs;
                        $('#' + chkHideTorch).prop('checked', form.hideTorch);
                        $('#' + chkHideAttachments).prop('checked', form.hideAttachments);
                        $('#' + chkHideAudiences).prop('checked', form.hideAudiences);
                        $('#' + chkFormBuiltIn).prop('checked', form.builtIn);
                        if (CurrentUserInfo.AdminOverride) {
                            $('#' + chkFormBuiltIn).prop('disabled', false);
                        }

                        if (form.builtIn) {
                            $('#' + chkFormBuiltIn).prop('disabled', true);
                        }

                        var lblsavebuttontext = $g(lblsavebuttontextid);
                        var lblsaveandduplicatebuttontext = $g(lblsaveandduplicatebuttontextid);
                        var lblsaveandstaybuttontext = $g(lblsaveandstaybuttontextid);
                        var lblsaveandnewbuttontext = $g(lblsaveandnewbuttontextid);
                        var lblcancelbuttontext = $g(lblcancelbuttontextid);

                        lblsavebuttontext.className = '';
                        lblsaveandduplicatebuttontext.className = '';
                        lblsaveandstaybuttontext.className = '';
                        lblsaveandnewbuttontext.className = '';
                        lblcancelbuttontext.className = '';
                        lblsavebuttontext.innerHTML = lblsavebuttontext.innerHTML.replace('*', '');
                        lblsaveandduplicatebuttontext.innerHTML = lblsaveandduplicatebuttontext.innerHTML.replace('*', '');
                        lblsaveandstaybuttontext.innerHTML = lblsaveandstaybuttontext.innerHTML.replace('*', '');
                        lblsaveandnewbuttontext.innerHTML = lblsaveandnewbuttontext.innerHTML.replace('*', '');
                        lblcancelbuttontext.innerHTML = lblcancelbuttontext.innerHTML.replace('*', '');

                        thisNs.FormObj = form;

                        SEL.CustomEntityAdministration.Forms.CreateNoTabsAndSectionsComments();

                        $(form.tabs).each(function (t, currentTabObject)
                        {
                            thisNs.AddTab(currentTabObject.TabName, false);
                            thisNs.sectionsInCurrentTab = 0;

                            $(currentTabObject.Sections).each(function (s, currentSectionObject)
                            {
                                thisNs.AddSection(currentSectionObject.SectionName, false, null, currentSectionObject.SectionControlName);

                                fields = currentSectionObject.Fields;

                                // TODO: This adds about a second to the loading time on my computer. Do the working out server side
                                thisNs.DisplaySectionFields(currentSectionObject.SectionControlName + '_FieldsArea', fields);

                                sectionsInCurrentTab++;
                            });
                        });

                        if (form.tabs.length > 0)
                        {
                            // TODO: This adds a lot of time to the load, too
                            thisNs.ChangeTab(form.tabs[0].TabControlName);
                        }

                        thisNs.ShowFormModal(false);

                        _misc.LoadingScreenCancelled = true;

                        $('#loadingArea').fadeOut(600, function ()
                        {
                            $('#loadingArea').remove();
                        });

                    }, 50);
                },

                // TODO: Moved, NOT refactored.
                AddSection: function (sectionHeader, modalSave, tabName, ControlID)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    // TODO: The start of this function is crazy - this can probably refactor to a few lines

                    //override if adding section to another tab
                    var bCheckComments = true;
                    if (thisNs.EditTabObject !== null && thisNs.EditTabObject !== undefined && thisNs.EditTabObject !== '' && thisNs.EditTabObject !== thisNs.SelectedTab)
                    {
                        tabName = thisNs.EditTabObject.attr('id');

                        if (tabName !== thisNs.SelectedTab)
                        {
                            bCheckComments = false;
                        }
                    }

                    // This is the new, refactored way of doing things. Eventually, edit[x]Object gloabal variables will all be replaced by code like this
                    var targetTab = $('#' + btnSaveSectionid).attr('targetTab'); 
                    if (targetTab !== undefined && targetTab !== "") // IE6/compat mode returns "" rather than undefined on an empty attr, so check for an empty string too
                    {
                        tabName = targetTab;
                        $('#' + btnSaveSectionid).removeAttr('targetTab');
                        bCheckComments = true;
                    }

                    if (tabName === undefined || tabName === null)
                    {
                        tabName = thisNs.SelectedTab;
                    }

                    if (ControlID === undefined || ControlID === null)
                    {
                        ControlID = tabName + '_Section_' + thisNs.NumberOfSections;
                    }

                    if (modalSave)
                    {
                        if (validateform('vgSectionHeader') == false)
                        {
                            return;
                        }
                        var bDuplicateCheck = true;
                        if (thisNs.EditSectionObject !== null && thisNs.EditSectionObject !== undefined && thisNs.EditSectionObject !== '')
                        {
                            if (sectionHeader === thisNs.EditSectionObject.attr('sectionname')) //ensure tab name has changed
                            {
                                bDuplicateCheck = false;
                            }
                        }

                        if (bDuplicateCheck)
                        {
                            if (thisNs.CheckSectionNameExists(sectionHeader, tabName))
                            {
                                SEL.MasterPopup.ShowMasterPopup('Cannot add a section with this name as one already exists on this tab.', SEL.CustomEntityAdministration.Messages.ModalTitle);
                                var validator = $g(reqSectionHeaderID);
                                validator.isvalid = false;
                                ValidatorUpdateDisplay(validator);
                                return;
                            }
                        }
                    }

                    // If editing an existing section
                    if (thisNs.EditSectionMode)
                    {
                        thisNs.EditSection(sectionHeader);
                        return;
                    }

                    var sectionDiv = document.createElement('div');
                    sectionDiv.setAttribute("class", "customentityformsection");
                    sectionDiv.setAttribute("className", "customentityformsection");
                    sectionDiv.setAttribute("sectionname", sectionHeader);
                    sectionDiv.id = ControlID;
                    var header = document.createElement('div');
                    header.setAttribute("class", "sectiontitle");
                    header.setAttribute("className", "sectiontitle");
                    header.id = ControlID + '_Title';
                    header.style.position = 'relative';

                    header.innerHTML = sectionHeader;

                    sectionDiv.appendChild(header);

                    var sectionArea = document.createElement('div');
                    sectionArea.setAttribute("class", "sectionsortables");
                    sectionArea.setAttribute("className", "sectionsortables");
                    sectionArea.id = ControlID + '_FieldsArea';

                    sectionDiv.appendChild(sectionArea);
                    document.getElementById(tabName).appendChild(sectionDiv);

                    thisNs.SelectedSection = sectionArea.id;

                    var optionsImg = document.createElement('img');

                    optionsImg.setAttribute('id', 'options_img_' + ControlID);
                    optionsImg.setAttribute('src', '../images/icons/16/Plain/gear.png');
                    optionsImg.setAttribute('alt', 'Options');
                    optionsImg.setAttribute('title', 'Options');
                    optionsImg.className = 'sm_form_editor_section_btn';
                    header.appendChild(optionsImg);

                    optionsImg.onclick = function ()
                    {
                        thisNs.ShowFormOptions(this.id, thisNs.ControlTypeObject.Section, sectionDiv.id, null);
                        thisNs.SelectedSection = sectionArea.id;
                    };

                    if (modalSave)
                    {
                        var sectionObj = new thisNs.SectionObject();

                        sectionObj.SectionName = sectionHeader;
                        sectionObj.SectionControlName = ControlID;

                        var addTab = tabName != '' ? tabName : thisNs.SelectedTab;
                        for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                        {
                            if (thisNs.FormObj.tabs[i].TabControlName === addTab)
                            {
                                sectionObj.Order = thisNs.FormObj.tabs[i].Sections.length;
                                thisNs.FormObj.tabs[i].Sections.push(sectionObj);
                                break;
                            }
                        }
                    }

                    thisNs.NumberOfSections++;

                    if (bCheckComments)
                    {
                        thisNs.CreateNoTabsAndSectionsComments();
                    }

                    header.onclick = function ()
                    {
                        thisNs.SelectedSection = sectionArea.id;
                    };

                    if (modalSave)
                    {
                        thisNs.MakeSectionFieldsSortable();

                        thisNs.MakeSpacerFieldDraggable();

                        thisNs.MakeAvailableFieldsDraggable();

                        thisNs.MakeAvailableFieldsDroppable();

                        thisNs.HideSectionModal();
                    }
                },

                // TODO: Moved, NOT refactored
                EditSection: function (sectionName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    sectionName = EscapeHTMLInString(sectionName).replace(/</g, '').replace(/>/g, '');

                    var sectionID = thisNs.EditSectionObject.attr('id');

                    thisNs.EditSectionObject.attr('sectionname', sectionName);

                    var sectionTitle = $('#' + sectionID + '_Title');

                    sectionTitle.text(sectionName);

                    var optionsImg = document.createElement('img');
                    optionsImg.setAttribute('id', 'options_img_' + sectionID);
                    optionsImg.setAttribute('src', '../images/icons/16/Plain/gear.png');
                    optionsImg.setAttribute('alt', 'Options');
                    optionsImg.setAttribute('title', 'Options');
                    optionsImg.className = 'sm_form_editor_section_btn';

                    sectionTitle.append(optionsImg);

                    optionsImg.onclick = function ()
                    {
                        thisNs.ShowFormOptions(this.id, thisNs.ControlTypeObject.Section, sectionID, null);
                        thisNs.SelectedSection = sectionID;
                    };

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === thisNs.SelectedTab)
                        {
                            for (var s = 0; s < thisNs.FormObj.tabs[i].Sections.length; s++)
                            {
                                if (thisNs.FormObj.tabs[i].Sections[s].SectionControlName === sectionID)
                                {
                                    // Set the new Section name within formObj                    
                                    thisNs.FormObj.tabs[i].Sections[s].SectionName = sectionName;

                                    break;
                                }
                            }
                        }
                    }

                    thisNs.HideSectionModal();
                },

                // TODO: Moved, NOT refactored
                CheckSectionNameExists: function (sectionName, tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (tabName === undefined)
                    {
                        tabName = thisNs.SelectedTab;
                    }

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tabName)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                if (thisNs.FormObj.tabs[i].Sections[x].SectionName.toLowerCase() === sectionName.toLowerCase())
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                },

                // TODO: Moved, NOT refactored
                //Sort and display all of the fields within a section
                DisplaySectionFields: function (section, fields)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var currentRow = 0;
                    var currentColumn = 0;
                    var spacerField = thisNs.SpacerField();

                    for (var i = 0; i < fields.length; i++)
                    {
                        var curFormField = fields[i];

                        while (currentRow < curFormField.Row)
                        {
                            if (curFormField.ColumnSpan === 2 && currentColumn === 1 && (curFormField.Row - currentRow === 1))
                            {
                                currentColumn = 0;
                                currentRow++;
                            }
                            else
                            {
                                spacerField.AttributeID = thisNs.NumberOfSpacers;
                                thisNs.NewField(section, spacerField);

                                if (currentColumn === 0)
                                {
                                    currentColumn = 1;
                                }
                                else
                                {
                                    currentRow++;
                                    currentColumn = 0;
                                }
                                thisNs.NumberOfSpacers++;
                            }
                        }

                        while (currentColumn < curFormField.Column)
                        {
                            if (curFormField.ColumnSpan === 2 && currentColumn === 1 && (curFormField.Row - currentRow === 1))
                            {
                                currentColumn = 0;
                                currentRow++;
                            }
                            else
                            {
                                spacerField.AttributeID = thisNs.NumberOfSpacers;
                                thisNs.NewField(section, spacerField);

                                if (currentColumn === 0)
                                {
                                    currentColumn = 1;
                                }
                                else
                                {
                                    currentRow++;
                                    currentColumn = 0;
                                }
                                thisNs.NumberOfSpacers++;
                            }
                        }

                        thisNs.NewField(section, curFormField);

                        if (curFormField.ColumnSpan === 1)
                        {
                            if (currentColumn === 0)
                            {
                                currentColumn = 1;
                            }
                            else
                            {
                                currentRow++;
                                currentColumn = 0;
                            }
                        }
                        else
                        {
                            if (currentColumn === 1)
                            {
                                currentRow++;
                                currentColumn = 0;
                            }
                            currentRow++;
                        }
                    }
                },

                HideSectionModal: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    thisNs.EditSectionMode = false;
                    thisNs.EditSectionObject = null;
                    $('#' + btnSaveSectionid).removeAttr("targetTab");
                    SEL.Common.HideModal(modsectionid);

                    $g(txtsectionid).blur();

                    SEL.CustomEntityAdministration.Base.AddShortCuts('formDesigner');
                },

                // TODO: Move, NOT refactored
                ShowFormModal: function (isAdd)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    SEL.Common.Page_ClientValidateReset();
                    $(document).click(function (e)
                    {
                        if (e.target.id.indexOf('options_img_') === -1)
                        {
                            thisNs.HideFormOptions();
                        }
                    });

                    $find(tabConFormsID).set_activeTabIndex(0);

                    thisNs.FormFieldDetails = [];

                    if (isAdd)
                    {
                        //Reset array when loading up the editor
                        thisNs.FormObj = new thisNs.FormObject();

                        thisNs.CurrentFormID = 0;
                        document.getElementById('tabBar').innerHTML = '';
                        document.getElementById('addTabHolder').innerHTML = '';
                        document.getElementById('formTabs').innerHTML = '';
                        thisNs.ClearFormModal();
                    }

                    if (entityid == 0)
                    {
                        SEL.CustomEntityAdministration.Base.Save();
                    }

                    //add new tab
                    var tabbaritem = document.createElement('span');
                    var tabholder = $g('addTabHolder');

                    tabbaritem.innerHTML = '<span id="tab_middle_img_add" class="sm_tabheader_middle">&nbsp;<img alt="New tab" title="New tab" src="' + StaticLibPath + '/icons/16/new-icons/greenplus.png" class="sm_form_editor_tab_btn" />New Tab&nbsp;</span>';

                    tabbaritem.setAttribute('class', 'sm_tabheader');
                    tabbaritem.setAttribute('className', 'sm_tabheader');
                    tabbaritem.id = 'add_new_tab';
                    tabbaritem.onclick = function ()
                    {
                        thisNs.ShowTabModal();
                    };

                    tabholder.appendChild(tabbaritem);

                    thisNs.CreateNoTabsComments();

                    //Get all field objects that can be used on the form editor
                    thisNs.GetFieldDetails();
                    SEL.Common.ShowModal(modformid);

                    thisNs.SetupFormModalInformationIcon();

                    //SEL.CustomEntityAdministration.zIndices.Forms.Modal = $('#'+ $f(modformid)._popupElement.id).css('zIndex');
                    $f(modformid)._backgroundElement.style.zIndex = SEL.CustomEntityAdministration.zIndices.Forms.Modal - 5;
                    $f(modformid)._popupElement.style.zIndex = SEL.CustomEntityAdministration.zIndices.Forms.Modal;

                    //Create the Form Field Spacer object
                    var spacerArea = $('#spacerArea');
                    spacerArea.html('');
                    var dragHandleHolder = document.createElement('span');
                    dragHandleHolder.id = 'spacerDragHandle';
                    var dragHandleImg = document.createElement('img');

                    dragHandleImg.setAttribute('id', 'draghandle_spacer');
                    dragHandleImg.setAttribute('src', StaticLibPath + '/icons/Custom/HandleBar.png');
                    dragHandleImg.setAttribute('alt', '');
                    dragHandleImg.setAttribute('title', 'Click here to drag a spacer onto the form');
                    $(dragHandleImg).css('width', '19px').css('height', '16px').css('padding', '7px 0px');
                    dragHandleHolder.appendChild(dragHandleImg);

                    var spacerContainer = document.createElement('span');
                    spacerContainer.className = 'sm_field_spacer';
                    spacerContainer.id = 'sm_field_spacer';

                    var spacerText = document.createElement('span');
                    spacerText.id = 'spacerAreaText';
                    spacerText.innerHTML = 'Spacer';
                    spacerText.setAttribute('isSpacer', 'true');

                    spacerContainer.appendChild(dragHandleHolder);

                    spacerContainer.appendChild(spacerText);

                    spacerArea.append(spacerContainer);

                    // Create the 'Spacer' form field
                    $("#availableFieldDoms").html('');

                    var spacerField = new thisNs.FieldObject();
                    spacerField.AttributeID = 0;
                    spacerField.ControlName = 'form_attribute_spacer';
                    spacerField.DisplayName = 'Spacer';
                    spacerField.FieldType = 20;
                    spacerField.Format = 1;
                    spacerField.ColumnSpan = 1;

                    thisNs.NewField('availableFieldDoms', thisNs.SpacerField());

                    $('#form_attribute_spacer_0_optionsmenu').remove();

                    thisNs.MakeSpacerFieldDraggable();

                    thisNs.MakeFormModalDroppable();

                    $('#' + txtformnameid).select();

                    // TODO: Investigate the use of 'on' here, see if it can be used elsewhere/if it should be used
                    $('#sm_field_spacer, #tabBar').on("mousedown", function (e)
                    {
                        $(this).addClass("closedhand");
                    });

                    $('#availableFields').on("mousedown", function (e)
                    {
                        $('#availableFields > span').addClass("closedhand");
                    });

                    $('#' + chkHideAttachments).prop('disabled', true);
                    if ($('#' + chkenableattachmentsid).is(':checked')) {
                        $('#' + chkHideAttachments).prop('disabled', false);
                    }

                    $('#' + chkHideAudiences).prop('disabled', true);
                    if ($('#' + chkEnableAudiencesID).is(':checked')) {
                        $('#' + chkHideAudiences).prop('disabled', false);
                    }

                    $('#' + chkHideTorch).prop('disabled', true);
                    if ($('#' + chkallowdocmergeid).is(':checked')) {
                        $('#' + chkHideTorch).prop('disabled', false);
                    }
                },

                SetupFormModalInformationIcon: function ()
                {
                    // IE7 Bug fix - without the line below the information image would not hide on modal show
                    $('#imgFormDesignerHelp').css('display', 'inline-block');

                    var formDesignHelpIcon = $('#imgFormDesignerHelp');

                    formDesignHelpIcon.css('display', 'none');

                    formDesignHelpIcon.css('left', formDesignHelpIcon.parent().outerWidth() - 38);
                    formDesignHelpIcon.css('top', formDesignHelpIcon.parent().outerHeight() - 58);

                    $('#imgFormDesignerHelp').mouseenter(function ()
                    {
                        var formDesignerHelp = $('#formDesignerHelpArea');
                        formDesignerHelp.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields());

                        formDesignerHelp.css('left', $(this).offset().left - formDesignerHelp.outerWidth());
                        formDesignerHelp.css('top', $(this).offset().top - formDesignerHelp.outerHeight());
                        formDesignerHelp.css('position', 'absolute');
                        formDesignerHelp.stop(true, true).fadeIn(400);

                    }).mouseleave(function ()
                    {
                        $('#formDesignerHelpArea').stop(true, true).fadeOut(200);
                    });
                },

                ClearFormModal: function ()
                {
                    document.getElementById(txtformnameid).value = '';
                    document.getElementById(txtformdescriptionid).value = '';
                    document.getElementById(txtformsavebuttontextid).value = '';
                    document.getElementById(txtformsaveandduplicatebuttontextid).value = '';
                    document.getElementById(txtformsaveandstaybuttontextid).value = '';
                    document.getElementById(txtformsaveandnewbuttontextid).value = '';
                    document.getElementById(txtformcancelbuttontextid).value = '';
                    document.getElementById(chkshowsubmenuid).checked = false;
                    document.getElementById(chkshowbreadcrumbsid).checked = false;
                    document.getElementById(chkFormBuiltIn).checked = false;
                },

                //Get all fields for the entity that can be added to the forms
                GetFieldDetails: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    Spend_Management.svcCustomEntities.GetFieldDetails(cu.AccountID, entityid, formid, thisNs.GetFieldDetailsComplete, thisNs.FormsWebServiceError);
                },

                //Set the global field details collection 
                GetFieldDetailsComplete: function (data)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    thisNs.FormFieldDetails = data;

                    thisNs.SetAdditionalInformationInFormFieldDetails();

                    thisNs.GetAvailableFieldsForSelectionArea();
                },

                ShowTabModal: function (tabname, fromHotKey)
                {
                    SEL.CustomEntityAdministration.Base.AssignDummyShortCuts();

                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    thisNs.ClearTabModal();

                    if (fromHotKey !== undefined && fromHotKey === true)
                    {
                        thisNs.EditTabMode = false;
                        thisNs.EditTabObject = null;
                    }

                    SEL.Common.ShowModal(modtabid);
                    if (tabname !== null && tabname !== undefined)
                    {
                        $('#lblTabModalTitle').html('Tab : ' + tabname);
                        $g(txttabid).value = tabname;
                    }
                    else
                    {
                        $('#lblTabModalTitle').html('New Tab');
                    }
                    $g(txttabid).select();
                },

                ClearTabModal: function ()
                {
                    validator = $g(reqTabHeaderID);
                    validator.isvalid = true;
                    ValidatorUpdateDisplay(validator);
                    $g(txttabid).value = '';
                },

                // TODO: This, in particular, needs a good refactoring
                AddTab: function (tabHeader, modalSave)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    tabHeader = EscapeHTMLInString(tabHeader).replace(/</g, '').replace(/>/g, '');

                    if (modalSave)
                    {
                        if (validateform('vgTabHeader') === false)
                        {
                            return;
                        }
                        var bDuplicateCheck = true;
                        if (thisNs.EditTabObject !== null && thisNs.EditTabObject !== undefined && thisNs.EditTabObject !== '')
                        {
                            if (tabHeader === $('#' + thisNs.EditTabObject.attr('id')).attr('tabname')) //ensure tab name has changed
                            {
                                bDuplicateCheck = false;
                            }
                        }
                        if (bDuplicateCheck)
                        {
                            if (thisNs.CheckTabNameExists(tabHeader))
                            {
                                SEL.MasterPopup.ShowMasterPopup('Cannot add a tab with this name as one already exists on this form.', 'Message from ' + moduleNameHTML);
                                var validator = $g(reqTabHeaderID);
                                validator.isvalid = false;
                                ValidatorUpdateDisplay(validator);
                                return;
                            }
                        }
                    }

                    // If editing an existing tab
                    if (thisNs.EditTabMode)
                    {
                        thisNs.EditTab(tabHeader);
                        return;
                    }

                    var tabName = 'Tab_' + thisNs.NumberOfTabs;

                    thisNs.SelectedTab = tabName;

                    var tabbaritem = document.createElement('span');

                    //Middle style of the tab
                    var tabMiddle = document.createElement('span');
                    tabMiddle.setAttribute('id', 'tab_middle_img_' + tabName);
                    tabMiddle.setAttribute('class', 'sm_tabheader_middle');
                    tabMiddle.setAttribute('className', 'sm_tabheader_middle');
                    //tabMiddle.style.background = 'url(../images/backgrounds/tabs/tab_selected_middle_img.png) repeat-x';    

                    tabbaritem.appendChild(tabMiddle);

                    tabMiddle.innerHTML += tabHeader;

                    var optionsImg = document.createElement('img');
                    optionsImg.setAttribute('id', 'tab_options_img_' + tabName);
                    optionsImg.setAttribute('src', '../images/icons/16/Plain/gear.png');
                    optionsImg.setAttribute('alt', 'Options');
                    optionsImg.setAttribute('title', 'Options');
                    optionsImg.className = 'sm_form_editor_tab_btn';

                    tabMiddle.appendChild(optionsImg);

                    optionsImg.onclick = function ()
                    {
                        thisNs.ShowFormOptions(this.id, thisNs.ControlTypeObject.Tab, tabName, null);
                    };


                    tabbaritem.setAttribute('class', 'sm_tabheader');
                    tabbaritem.setAttribute('className', 'sm_tabheader');
                    tabbaritem.setAttribute('tabname', tabHeader);
                    tabbaritem.id = 'tabbar_' + tabName;
                    $(tabbaritem).click(function (e)
                    {
                        if (e.target.className !== 'sm_form_editor_tab_btn')
                        {
                            thisNs.ChangeTab(tabName);
                        }
                    });

                    document.getElementById('tabBar').appendChild(tabbaritem);

                    var tabpane = document.createElement('div');

                    tabpane.id = tabName;
                    tabpane.style.overflow = 'auto';
                    tabpane.style.overflowX = 'hidden';

                    document.getElementById('formTabs').appendChild(tabpane);

                    $('#tab_middle_img_' + tabName).addClass('selected_tab');

                    var tab = null;
                    var currentTabName;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        tab = document.getElementById(thisNs.FormObj.tabs[i].TabControlName);

                        if (tab !== null)
                        {
                            if (thisNs.FormObj.tabs[i].TabControlName !== tabName)
                            {
                                currentTabName = thisNs.FormObj.tabs[i].TabControlName;
                                tab.style.display = 'none';
                                $('#tab_middle_img_' + currentTabName).removeClass('selected_tab');
                            }
                        }
                    }

                    if (modalSave)
                    {
                        var tabObject = new thisNs.TabObject();

                        tabObject.TabName = tabHeader;
                        tabObject.TabControlName = tabName;
                        tabObject.Order = thisNs.FormObj.tabs.length;

                        thisNs.FormObj.tabs.push(tabObject);
                    }

                    // TODO: Put this into a seperate 'MakeTabsSortable' function
                    // Setup sortable on tabs
                    $('#tabBar').sortable({
                        opacity: 0.7,
                        revert: 200,
                        placeholder: 'placeholder-highlight',
                        distance: 15,
                        helper: function (e, ui)
                        {
                            return ui.clone();
                        },
                        start: function (e, ui)
                        {
                            ui.placeholder.height(ui.item.height() - 4);
                            ui.placeholder.width(ui.item.width() - 4);
                            ui.helper.css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            ui.helper.children().css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                        },
                        stop: function (e, ui)
                        {
                            $('#tabBar').removeClass("closedhand");
                            $('#tabBar').children().removeClass("closedhand");
                            $('#' + thisNs.SelectedTab + ' .sectionsortables').removeClass("closedhand");
                        }
                    });
                    $('#tabBar').disableSelection();

                    // TODO: Put this into a seperate 'MakeTabsDroppable' function
                    // Setup the ability to drop sections onto tabs
                    $('#tabbar_' + tabName).droppable({
                        hoverClass: 'drophover',
                        accept: '.customentityformsection',
                        tolerance: 'pointer',
                        drop: function (event, ui)
                        {
                            $('.placeholder-highlight').css('display', 'none');
                            thisNs.MoveSectionToNewTab(ui.draggable.attr('id'), ui.draggable.parent().attr('id'), $(this).attr('id').replace('tabbar_', ''));
                        }
                    });

                    // TODO: Put this into a seperate 'MakeSectionsSortable' function
                    // Setup sortable on sections
                    $('#' + tabName).sortable({
                        opacity: 0.5,
                        revert: 200,
                        axis: 'y',
                        placeholder: 'placeholder-highlight',
                        distance: 15,
                        helper: function (e, ui)
                        {
                            var helperObj = ui.clone();
                            helperObj.children().each(function (x, childElement)
                            {
                                $(childElement).attr('id', 'cloned_' + $(childElement).attr('id'));
                            });

                            return helperObj;
                        },
                        start: function (e, ui)
                        {
                            thisNs.EditTabObject = null;
                            ui.placeholder.height(ui.item.height() - 4);
                            ui.placeholder.width(845);
                            ui.helper.css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            ui.helper.children().css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                        },
                        stop: function (e, ui)
                        {
                            $('#' + thisNs.SelectedTab).removeClass("closedhand");
                            $('#' + thisNs.SelectedTab).children().removeClass("closedhand");
                            $('#' + thisNs.SelectedTab + ' .sectionsortables').removeClass("closedhand");
                        }
                    });
                    $('#' + tabName).disableSelection();

                    thisNs.NumberOfTabs++;

                    if (thisNs.EditTabObject === null || thisNs.EditTabObject === undefined || thisNs.EditTabObject === '')
                    {
                        thisNs.CreateNoTabsAndSectionsComments();
                    }

                    if (modalSave)
                    {
                        thisNs.HideTabModal();
                    }
                },

                EditTab: function (newName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    newName = EscapeHTMLInString(newName).replace(/</g, '').replace(/>/g, '');

                    var tabID = thisNs.EditTabObject.attr('id').replace('tabbar_', '');

                    thisNs.EditTabObject.attr('tabname', newName);

                    var tabMiddle = $('#tab_middle_img_' + tabID);

                    tabMiddle.text(newName);

                    var optionsImg = document.createElement('img');
                    optionsImg.setAttribute('id', 'tab_options_img_' + tabID);
                    optionsImg.setAttribute('src', '../images/icons/16/Plain/gear.png');
                    optionsImg.setAttribute('alt', 'Options');
                    optionsImg.setAttribute('title', 'Options');
                    optionsImg.className = 'sm_form_editor_tab_btn';

                    tabMiddle.append(optionsImg);

                    optionsImg.onclick = function ()
                    {
                        //SelectedTab = currentTab;
                        thisNs.ShowFormOptions(this.id, thisNs.ControlTypeObject.Tab, tabID, null);
                    };

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tabID)
                        {
                            thisNs.FormObj.tabs[i].TabName = newName;

                            break;
                        }
                    }

                    thisNs.HideTabModal();
                },

                RemoveTab: function (tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (confirm('Are you sure you want to delete this tab?'))
                    {
                        $('#' + tabName + ' .customentityformsection').each(function (s, sectionObj)
                        {
                            $('#' + sectionObj.id + '_FieldsArea').children().each(function (f, fieldObj)
                            {
                                var fieldID = $(fieldObj).attr('id');

                                if (fieldID.indexOf('spacer') === -1)
                                {
                                    thisNs.RemoveFormField(fieldID, thisNs.GetFieldByID(fieldID));
                                }
                                else
                                {
                                    thisNs.RemoveFormField(fieldID, thisNs.SpacerField());
                                }
                            });
                        });

                        var tabHeader = document.getElementById('tabbar_' + tabName);
                        var tabPane = document.getElementById(tabName);


                        if (thisNs.SelectedTab === tabName)
                        {
                            var tableft = $('#tabbar_' + tabName).prev();
                            if (tableft.length > 0)
                            {
                                thisNs.SelectedTab = tableft.attr('id').replace('tabbar_', '');
                            } else
                            {
                                var tabright = $('#tabbar_' + tabName).next();
                                if (tabright.length > 0)
                                {
                                    thisNs.SelectedTab = tabright.attr('id').replace('tabbar_', '');
                                }
                                else
                                {
                                    thisNs.SelectedTab = null;
                                }
                            }
                        }

                        tabHeader.parentNode.removeChild(tabHeader);
                        tabPane.parentNode.removeChild(tabPane);
                        thisNs.RemoveTabInFormObj(tabName);

                        thisNs.CreateNoTabsAndSectionsComments();

                        if (thisNs.SelectedTab !== null)
                        {
                            thisNs.ChangeTab(thisNs.SelectedTab);
                        }
                    }
                },

                GetTabByName: function (tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tabName)
                        {
                            return thisNs.FormObj.tabs[i];
                        }
                    }

                    return null;
                },

                MoveSectionToNewTab: function (sectionName, previousTab, newTab)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (newTab !== previousTab)
                    {
                        var newSection = new thisNs.SectionObject();
                        var oldSection = thisNs.GetSectionByName(sectionName);

                        newSection.SectionControlName = newTab + '_Section_' + thisNs.NumberOfSections;
                        newSection.SectionName = oldSection.SectionName;
                        newSection.Fields = oldSection.Fields;

                        var renameAttempts = 1;

                        while (thisNs.CheckSectionNameExists(newSection.SectionName, newTab))
                        {
                            if (renameAttempts > 1)
                            {
                                newSection.SectionName = newSection.SectionName.substring(0, newSection.SectionName.length - 3) + '[' + renameAttempts + ']';
                            }
                            else
                            {
                                newSection.SectionName = newSection.SectionName + ' [' + renameAttempts + ']';
                            }

                            renameAttempts++;
                        }

                        var fields = [];

                        $('#' + oldSection.SectionControlName + '_FieldsArea > span').each(function (f, fieldObj)
                        {
                            if (fieldObj.id.indexOf('spacer') === -1)
                            {
                                fields.push(thisNs.GetFieldByID(fieldObj.id));
                            }
                            else
                            {
                                fields.push(thisNs.SpacerField(fieldObj.id));
                            }
                        });

                        thisNs.MoveSectionInFormObj(newSection, previousTab, newTab);

                        // Remove from the current tab
                        var sectionArea = document.getElementById(sectionName);

                        sectionArea.parentNode.removeChild(sectionArea);
                        thisNs.RemoveSectionInFormObj(sectionName);

                        // Add to the new tab
                        thisNs.AddSection(newSection.SectionName, false, newTab, newSection.SectionControlName);

                        // Add fields back onto the section
                        for (var i = 0; i < fields.length; i++)
                        {
                            thisNs.NewField(newSection.SectionControlName + '_FieldsArea', fields[i]);
                        }
                    }
                },

                GetAvailableFieldsForSelectionArea: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    // Clear the availableFields div of any previous fields
                    var fieldsListArea = document.getElementById('availableFields');
                    fieldsListArea.innerHTML = '';

                    var sortedFieldsList = thisNs.FormFieldDetails;

                    if (thisNs.FieldsUsedOnForm.length === thisNs.FormFieldDetails.length)
                    {
                        fieldsListArea.innerHTML = 'There are no remaining fields for this form.';
                        fieldsListArea.style.paddingTop = '10px';
                    }
                    else
                    {
                        // Sort the list of available fields alphabetically
                        sortedFieldsList.sort(function (a, b) { return ((a.SortDisplayName.toLowerCase() < b.SortDisplayName.toLowerCase()) ? -1 : ((a.SortDisplayName.toLowerCase() > b.SortDisplayName.toLowerCase()) ? 1 : 0)); });

                        for (var y = 0; y < sortedFieldsList.length; y++)
                        {
                            if (thisNs.FieldsUsedOnForm.contains(sortedFieldsList[y].AttributeID) === false)
                            {
                                thisNs.AddFieldToAvailableFields(sortedFieldsList[y], null, false);
                            }
                        }
                    }

                    // Make the Available Fields area draggable
                    thisNs.MakeAvailableFieldsDraggable();

                    // Make the Available Fields area droppable
                    thisNs.MakeAvailableFieldsDroppable();
                },

                // TODO: Idealy, this function would not be needed. Bring the info down from webservice?
                // Update the global FormFieldDetails array with read-only and field text information
                SetAdditionalInformationInFormFieldDetails: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var tab;
                    var fields;

                    for (var j = 0; j < thisNs.FormObj.tabs.length; j++)
                    {
                        tab = thisNs.FormObj.tabs[j];

                        for (var x = 0; x < tab.Sections.length; x++)
                        {
                            fields = tab.Sections[x].Fields;

                            for (var y = 0; y < fields.length; y++)
                            {
                                thisNs.FieldsUsedOnForm.push(fields[y].AttributeID);
                                thisNs.UpdateFieldInFormFieldDetails(fields[y], thisNs.FormObj.tabs[j].TabControlName);
                            }
                        }
                    }
                },

                HideTabModal: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    thisNs.EditTabMode = false;
                    thisNs.EditTabObject = null;
                    SEL.Common.HideModal(modtabid);
                    SEL.CustomEntityAdministration.Base.AddShortCuts('formDesigner');
                },

                // TODO: Refactor this to go through each tab in the dom, rather than formObj
                CheckTabNameExists: function (tabName)
                {
                    for (var i = 0; i < SEL.CustomEntityAdministration.Forms.FormObj.tabs.length; i++)
                    {
                        if (SEL.CustomEntityAdministration.Forms.FormObj.tabs[i].TabName === tabName)
                        {
                            return true;
                        }
                    }

                    return false;
                },

                CreateNoTabsAndSectionsComments: function ()
                {
                    SEL.CustomEntityAdministration.Forms.CreateNoTabsComments();
                    SEL.CustomEntityAdministration.Forms.CreateNoSectionsComments();
                },

                CreateNoTabsComments: function ()
                {
                    if (!$e('divnotab'))
                    {
                        //comment to show when no tabs are created
                        var notabitem = document.createElement('div');
                        notabitem.className = 'onecolumnpanel';
                        notabitem.innerHTML = 'There are currently no tabs added to this form. Use the "New Tab" button above to create your first tab.';
                        notabitem.id = 'divnotab';
                        $g('formMsgs').appendChild(notabitem);
                    }
                    if (SEL.CustomEntityAdministration.Forms.FormObj.tabs.length == 0)
                    {
                        $g('divnotab').style.display = '';
                    }
                    else
                    {
                        $g('divnotab').style.display = 'none';
                    }
                },

                CreateNoSectionsComments: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (!$e('divnosection'))
                    {
                        //comment to show when no sections are created for the tab
                        var nosectionitem = document.createElement('div');
                        nosectionitem.className = 'onecolumnpanel';
                        nosectionitem.innerHTML = 'There are currently no sections added to this tab. Use the "New Section" icon in the options cog menu on the tab to create a section.';
                        nosectionitem.id = 'divnosection';
                        $g('formMsgs').appendChild(nosectionitem);
                    }
                    if (thisNs.FormObj.tabs.length !== 0)
                    {
                        if (thisNs.SelectedTab !== null)
                        {
                            //locate the tab
                            for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                            {
                                if (thisNs.FormObj.tabs[i].TabControlName === thisNs.SelectedTab)
                                {
                                    if (thisNs.FormObj.tabs[i].Sections.length == 0)
                                    {
                                        $g('divnosection').style.display = '';
                                    }
                                    else
                                    {
                                        $g('divnosection').style.display = 'none';
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            $g('divnosection').style.display = 'none';
                        }
                    }
                    else
                    {
                        $g('divnosection').style.display = 'none';
                    }
                },

                // TODO: Rename FieldObject to something else, FieldObject is being used
                //Show the form editor options
                ShowFormOptions: function (ControlID, ControlType, ControlName, FieldObject)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms,
                        optionsArea,
                        optionsAreaButtons;

                    if (ControlType === thisNs.ControlTypeObject.Field)
                    {
                        var formField = $('#' + ControlID);

                        // TODO: When refactoring, investigate replacing the use of 'CurrentlyDragging' with ':animated' checks
                        if (formField.is(':animated') === false)
                        {
                            optionsArea = formField.children('.sm_form_field_options').first();
                            optionsAreaButtons = optionsArea.children('.sm_btn');

                            optionsArea.stop(true, true);

                            optionsArea.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.ContextMenu());

                            optionsArea.css('top', '0px');

                            optionsAreaButtons.unbind('mouseenter').unbind('mouseleave').unbind('click');

                            optionsAreaButtons.mouseenter(function ()
                            {
                                $(this).addClass('field_options_image_hover');
                            })
                            .mouseleave(function ()
                            {
                                $(this).removeClass('field_options_image_hover');
                            })
                            .click(function ()
                            {
                                $(this).removeClass('field_options_image_hover');
                            });

                            optionsArea.fadeIn(250);
                        }
                    }
                    else
                    {
                        optionsArea = $('#' + popupFormOptionsID);
                        var parentControl = $('#' + ControlID);

                        //Clear the form options
                        optionsArea.html('');

                        //populate the form options
                        thisNs.CreateOptionsMenu(ControlType, ControlName, FieldObject);

                        optionsAreaButtons = optionsArea.children('.sm_btn');

                        $('#maindiv').append(optionsArea);

                        optionsArea.stop(true, true);
                        optionsArea.css('filter', '');
                        optionsArea.css('display', '');

                        if (parentControl.offset().left + optionsArea.outerWidth() >= $(window).width())
                        {
                            optionsArea.css('left', parentControl.offset().left - optionsArea.outerWidth());
                        }
                        else
                        {
                            optionsArea.css('left', parentControl.offset().left + parentControl.outerWidth());
                        }
                        optionsArea.css('top', parentControl.offset().top + parentControl.outerHeight());
                        optionsArea.css('right', 'auto');
                        optionsArea.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.ContextMenu());
                        optionsArea.css('position', 'absolute');
                        optionsArea.fadeIn(250);

                        optionsAreaButtons.unbind('mouseenter').unbind('mouseleave').unbind('click');

                        optionsAreaButtons.mouseenter(function ()
                        {
                            $(this).addClass('field_options_image_hover');
                        }).mouseleave(function ()
                        {
                            $(this).removeClass('field_options_image_hover');
                        }).click(function ()
                        {
                            $(this).removeClass('field_options_image_hover');
                        });
                    }
                },

                //Hide the form options menu from the form designer
                HideFormOptions: function (optionMenuID)
                {
                    var optionsArea;

                    if (optionMenuID === undefined)
                    {
                        optionsArea = $('#' + popupFormOptionsID);
                    }
                    else
                    {
                        optionsArea = $('#' + optionMenuID);
                    }

                    optionsArea.fadeOut(200);

                    return;
                },

                MakeSectionFieldsSortable: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    $('#' + thisNs.SelectedTab + ' .sectionsortables').sortable({
                        opacity: 0.7,
                        revert: 200,
                        connectWith: '#' + thisNs.SelectedTab + ' .sectionsortables',
                        zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields(),
                        placeholder: 'placeholder-highlight-fields',
                        distance: 15,
                        start: function (e, ui)
                        {
                            ui.helper.css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            ui.helper.children().css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            thisNs.SortEventCancelled = false;
                            $('#dialog').addClass('ui-state-disabled').attr('id', 'dialogDisabled');

                            $('#temporaryPlaceholder').removeClass('placeholder-highlight-fields');

                            thisNs.CurrentlyDragging = true;

                            $('#' + ui.item.attr('id') + '_optionsmenu').css('display', 'none');

                            ui.item.unbind('mouseenter');
                            ui.item.unbind('mouseleave');

                            if (ui.item.attr('class').indexOf('sm_field_spacer') !== -1)
                            {
                                ui.placeholder.height(28);
                                ui.placeholder.width(402);
                            }
                            else
                            {
                                ui.placeholder.width(ui.item.width() - 8);
                                ui.placeholder.height(ui.item.height() - 8);
                            }

                            ui.helper.height(ui.item.height());
                            ui.helper.width(ui.item.width());
                        },
                        stop: function (event, ui)
                        {
                            thisNs.CurrentlyDragging = false;
                            var optionsMenu;

                            if (thisNs.SortEventCancelled === false)
                            {
                                if (ui.item.attr('id') === undefined)
                                {
                                    if (ui.item.text() === 'Spacer')
                                    {
                                        var spacerElement = $('#form_attribute_spacer_0').clone(true);
                                        var spacerID = 'form_attribute_spacer_' + thisNs.NumberOfSpacers;

                                        spacerElement.attr('id', spacerID);

                                        ui.item.replaceWith(spacerElement);

                                        optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, spacerID, thisNs.SpacerField(thisNs.NumberOfSpacers));
                                        spacerElement.append(optionsMenu);

                                        spacerElement.unbind('mouseenter');
                                        spacerElement.unbind('mouseleave');

                                        spacerElement.mouseenter(function ()
                                        {
                                            if (thisNs.CurrentlyDragging === false)
                                            {
                                                $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                                                thisNs.selectedControl = $g(spacerID);
                                                thisNs.ShowFormOptions(spacerID, thisNs.ControlTypeObject.Field, spacerID, thisNs.SpacerField());
                                            }
                                        }).mouseleave(function ()
                                        {
                                            thisNs.HideFormOptions(spacerID + '_optionsmenu');
                                        });

                                        thisNs.NumberOfSpacers++;
                                    }
                                    else
                                    {
                                        var fieldID = ui.item.attr('fieldid').replace("cloned_", "");
                                        var availableFieldID = ui.item.attr('availablefieldid');
                                        var fieldElement = $('#' + fieldID);
                                        var fieldPlaceholder = $("#availableFields [availablefieldid='" + availableFieldID + "']");
                                        var currentField = thisNs.GetFieldByID(fieldID);
                                        fieldElement.attr("id", fieldID);

                                        ui.item.replaceWith(fieldElement);

                                        // Remove field from Available fields
                                        // Not done using jQuery to avoid a known 'remove' issue
                                        var availableFields = $g('availableFields');
                                        availableFields.removeChild(fieldPlaceholder[0]);

                                        if ($('#availableFields').children().length === 0)
                                        {
                                            $('#availableFields').html('There are no remaining fields for this form.');
                                            $('#availableFields').css('paddingTop', '10px');
                                        }

                                        $('#' + fieldID + '_optionsmenu').html('');
                                        $('#' + fieldID + '_optionsmenu').remove();

                                        optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, fieldID, currentField);
                                        fieldElement.append(optionsMenu);

                                        fieldElement.unbind('mouseenter');
                                        fieldElement.unbind('mouseleave');

                                        fieldElement.mouseenter(function ()
                                        {
                                            if (thisNs.CurrentlyDragging === false)
                                            {
                                                $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                                                thisNs.selectedControl = $g(fieldID);
                                                thisNs.ShowFormOptions(fieldID, thisNs.ControlTypeObject.Field, fieldID, currentField);
                                            }
                                        }).mouseleave(function ()
                                        {
                                            thisNs.HideFormOptions(fieldID + '_optionsmenu');
                                        });
                                    }
                                }
                                else
                                {
                                    var currentFieldID = ui.item.attr('id');

                                    $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');

                                    ui.item.unbind('mouseenter').unbind('mouseleave');

                                    ui.item.mouseenter(function ()
                                    {
                                        if (thisNs.CurrentlyDragging === false)
                                        {
                                            $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                                            thisNs.selectedControl = ui.item;
                                            thisNs.ShowFormOptions(currentFieldID, thisNs.ControlTypeObject.Field, currentFieldID, null);
                                        }
                                    }).mouseleave(function ()
                                    {
                                        thisNs.HideFormOptions(currentFieldID + '_optionsmenu');
                                    });
                                }

                                $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');
                                $('#' + thisNs.SelectedTab + ' .sectionsortables').removeClass("closedhand");
                                $('#' + thisNs.SelectedTab + ' .sectionsortables').children().removeClass("closedhand");
                                $('#availableFields > span').removeClass("closedhand");
                                $('#availableFields > span').children().removeClass("closedhand");
                                $('#temporaryPlaceholder').remove();
                            }
                        },
                        receive: function (event, ui)
                        {
                            // TODO: When refactoring, remove this completely - why update the field position on formObj?
                            if (thisNs.SortEventCancelled === false)
                            {
                                var fieldID;

                                if (ui.item.attr('availablefieldid'))
                                {
                                    fieldID = ui.item.attr('availablefieldid').replace('availableField', 'form_attribute_');
                                }
                                else
                                {
                                    fieldID = "spacer";
                                }

                                if (fieldID.indexOf('spacer') === -1)
                                {
                                    thisNs.MoveFieldToNewSection(thisNs.GetFieldByID(fieldID), ui.sender.attr('id'), event.target.id);
                                }
                            }
                        },
                        beforeStop: function (event, ui)
                        {
                            // As the placeholder is not on the page, cancel the current sort event if one is occurring
                            if ($(ui.placeholder).position().top === 0 && $(ui.placeholder).position().left === 0)
                            {
                                thisNs.SortEventCancelled = true;
                            }
                        },
                        helper: function (event, ui)
                        {
                            var cloneNode = document.getElementById(ui.attr('id')).cloneNode(true);
                            
                            // Default text 'textarea' Fix for Chrome and Firefox                            
                            $(cloneNode).find('textarea').val($('#' + ui.attr('id') + ' textarea').val());

                            cloneNode.id = 'cloned_' + cloneNode.id;
                            var availableFieldHolder = document.getElementById('dragFieldHolder');

                            availableFieldHolder.appendChild(cloneNode);

                            return cloneNode;
                        }
                    });
                    $('#' + thisNs.SelectedTab + ' .sectionsortables').disableSelection();
                },

                // Enable the spacer field within the Available Fields area to be dragged and dropped onto the form designer
                MakeSpacerFieldDraggable: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    $('#sm_field_spacer').draggable({
                        opacity: 0.7,
                        connectToSortable: '#' + thisNs.SelectedTab + ' .sectionsortables',
                        zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields(),
                        revert: 200,
                        helper: function (event)
                        {
                            var cloneNode = document.getElementById('form_attribute_spacer_0').cloneNode(true);
                            cloneNode.id = 'cloned_' + cloneNode.id;
                            cloneNode.style.zIndex = SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields();
                            var availableFieldHolder = document.getElementById('dragFieldHolder');
                            availableFieldHolder.appendChild(cloneNode);
                            return cloneNode;
                        },
                        stop: function (event, ui)
                        {
                            if ($('.placeholder-highlight-fields').length === 0)
                            {
                                thisNs.SortEventCancelled = true;
                                $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');

                                $('#sm_field_spacer').removeClass("closedhand");
                                $('#sm_field_spacer').children().removeClass("closedhand");
                                $('#' + thisNs.SelectedTab + ' .sectionsortables').removeClass("closedhand");
                                $('#temporaryPlaceholder').remove();
                                $(ui.helper).remove();
                            }

                            thisNs.CurrentlyDragging = false;
                        },
                        start: function (event, ui)
                        {
                            ui.helper.css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            ui.helper.children().css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            thisNs.SortEventCancelled = false;
                            $('#dialog').addClass('ui-state-disabled').attr('id', 'dialogDisabled');

                            thisNs.CurrentlyDragging = true;
                        }
                    });
                },

                // Enable all fields within the Available Fields area to be dragged and dropped onto the form designer
                MakeAvailableFieldsDraggable: function (divID)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (divID === undefined)
                    {
                        divID = '#availableFields > span';
                    }

                    $(divID).draggable({
                        opacity: 0.7,
                        connectToSortable: '#' + thisNs.SelectedTab + ' .sectionsortables',
                        cancel: '#dialogDisabled',
                        zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields(),
                        revert: 200,
                        helper: function (event)
                        {
                            var fieldID = $(this).attr('fieldid');
                            var cloneNode = $('#' + fieldID).clone();

                            // Default text 'textarea' Fix for Chrome and Firefox                               
                            cloneNode.find('textarea').val($('#' + fieldID + ' textarea').val());

                            cloneNode.attr('id', 'cloned_' + fieldID);
                            cloneNode.attr('availablefieldid', 'invalid');
                            cloneNode.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.AvailableFields());
                            $('#dragFieldHolder').append(cloneNode);
                            return cloneNode;
                        },
                        stop: function (event, ui)
                        {
                            if ($('.placeholder-highlight-fields').length === 0)
                            {
                                thisNs.SortEventCancelled = true;
                                $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');

                                $(divID).removeClass("closedhand");
                                $(divID).children().removeClass("closedhand");
                                $('#' + thisNs.SelectedTab + ' .sectionsortables').removeClass("closedhand");
                                $('#temporaryPlaceholder').remove();

                                $(ui.helper).remove();
                            }

                            thisNs.CurrentlyDragging = false;
                        },
                        start: function (event, ui)
                        {
                            ui.helper.css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            ui.helper.children().css('cursor', 'url(' + StaticLibPath + '/cursors/closedhand.cur), default !important');
                            thisNs.SortEventCancelled = false;
                            $('#dialog').addClass('ui-state-disabled').attr('id', 'dialogDisabled');

                            thisNs.CurrentlyDragging = true;
                            $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                        }
                    });

                    $('#' + thisNs.SelectedTab + ' .sectionsortables').disableSelection();
                    $('#tabBar').disableSelection();
                },

                // When a form field is dropped onto the Available Fields area, remove it from the form and add it back into Available Fields
                MakeAvailableFieldsDroppable: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    $('#dialog').droppable({
                        // TODO: Investigate if the selector for 'accept' can be more specific
                        accept: '#' + thisNs.SelectedTab + ' span[id^="form_attribute_"]',
                        tolerance: 'pointer',
                        hoverClass: 'availablefieldhover',
                        drop: function (event, ui) {
                            if (ui.draggable.attr('id').indexOf('spacer') === -1)
                            {
                                // If the field is not a Spacer, remove it from the form
                                $(thisNs.FormFieldDetails).each(function (f, currentFormField)
                                {
                                    if (currentFormField.AttributeID.toString() === ui.draggable.attr('id').replace('form_attribute_', ''))
                                    {
                                        $('.placeholder-highlight-fields').css('display', 'none');
                                        thisNs.RemoveFormField(currentFormField.ControlName, currentFormField, ui.helper);

                                        // Break out of the loop if a field has been found
                                        return false;
                                    }
                                });
                            }
                            else
                            {
                                // If the field is a Spacer, remove it from the form
                                $('.placeholder-highlight-fields').css('display', 'none');
                                thisNs.RemoveFormField(ui.draggable.attr('id'), thisNs.SpacerField(), ui.helper);
                            }
                        },
                        over: function ()
                        {
                            $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');
                        },
                        out: function ()
                        {
                            $('#dialog').addClass('ui-state-disabled').attr('id', 'dialogDisabled');
                        }
                    });
                },

                //Add the field to the editor area
                NewField: function (containerControl, field)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms,
                        fldType = thisNs.SetFieldType(field.FieldType),
                        fieldTag = fldType.fieldTag,
                        idName = field.DisplayName.toString().replace(/\s/g, '').toLowerCase(),
                        labelText,
                        i,
                        tableCell;

                    if (field.LabelText)
                    {
                        labelText = EscapeHTMLInString(field.LabelText);
                    }
                    else
                    {
                        labelText = EscapeHTMLInString(field.DisplayName.toString());
                    }

                    if (labelText.length > 30)
                    {
                        labelText = labelText.substring(0, 30) + '...';
                    }

                    var containerNode = document.createElement('span');
                    containerNode.setAttribute('id', 'sm_field__' + idName);
                    containerNode.className = fldType.className;

                    containerNode.setAttribute('fielddisplayname', field.DisplayName);
                    containerNode.id = 'form_attribute_' + field.AttributeID;

                    field.ColumnSpan = fldType.columnSpan;

                    if (field.FieldType === thisNs.FieldTypeEnum.OTMSummary.intVal || (field.FieldType === thisNs.FieldTypeEnum.Relationship.intVal && field.RelationshipType === thisNs.RelationshipType.OneToMany))
                    {
                        containerNode.className = 'sm_field_grid';

                        field.ColumnSpan = 2;

                        // Create the table information
                        var imageInfo = function (a, b, c) { return { alt: a, src: b, text: c }; };

                        var lstCells = [new imageInfo('Edit', '/shared/images/icons/edit_blue.gif', '')];
                        lstCells.push(new imageInfo('Delete', '/shared/images/icons/delete2_blue.gif', ''));
                        lstCells.push(new imageInfo('View', '/shared/images/icons/view.png', ''));
                        lstCells.push(new imageInfo('Arrow', '/shared/images/whitearrow_up.gif', fldType.attributeName + ' Column 1 '));
                        for (i = 2; i < 6; i++)
                        {
                            lstCells.push(new imageInfo('', '', fldType.attributeName + ' Column ' + i));
                        }

                        // Create the table object
                        var tableObj = document.createElement("table");
                        tableObj.className = 'cGrid';
                        tableObj.setAttribute('id', 'sm_table__' + idName);
                        tableObj.setAttribute('width', '800');

                        var tableBodyObj = document.createElement("tbody");

                        // Create the first row
                        var firstRow = document.createElement("tr");

                        for (i = 0; i < lstCells.length; i++)
                        {
                            tableCell = document.createElement("th");

                            if (lstCells[i].src == '')
                            {
                                tableCell.innerHTML = lstCells[i].text;
                            }
                            else
                            {
                                var cellImageObj = document.createElement("img");
                                cellImageObj.setAttribute('alt', lstCells[i].alt);
                                cellImageObj.setAttribute('src', lstCells[i].src);
                                tableCell.innerHTML = lstCells[i].text + ' ';
                                tableCell.appendChild(cellImageObj);
                            }
                            firstRow.appendChild(tableCell);
                        }

                        tableBodyObj.appendChild(firstRow);

                        // Create the second row
                        var secondRow = document.createElement("tr");

                        tableCell = document.createElement("td");
                        tableCell.className = 'row1';
                        tableCell.colSpan = '8';
                        tableCell.align = 'center';
                        tableCell.innerHTML = labelText + ' information will be displayed here.';

                        secondRow.appendChild(tableCell);

                        tableBodyObj.appendChild(secondRow);

                        tableObj.appendChild(tableBodyObj);

                        containerNode.appendChild(tableObj);
                    }
                    else if (field.FieldType === thisNs.FieldTypeEnum.Comment.intVal)
                    {
                        containerNode.innerHTML = labelText + ' - Note that the size of this comment block will increase as the amount of text increases.';
                    }
                    else if (field.FieldType === thisNs.FieldTypeEnum.Spacer.intVal)
                    {
                        containerNode.id = 'form_attribute_spacer_' + field.AttributeID;
                        containerNode.innerHTML = field.DisplayName;
                    }
                    else
                    {
                        switch (field.Format)
                        {
                            case thisNs.Format.MultiLine:
                                if (field.FieldType === thisNs.FieldTypeEnum.Text.intVal)
                                {
                                    fieldTag = 'textarea';
                                    $(containerNode).attr('class', 'sm_field_tall onecolumn');
                                    field.ColumnSpan = 2;
                                }
                                break;
                            case thisNs.Format.FormattedText:
                                fieldTag = 'span';                                
                                break;
                            case thisNs.Format.SingleLineWide:
                                $(containerNode).removeClass('sm_field').addClass('sm_field_wide');
                                field.ColumnSpan = 2;
                                break;
                            case thisNs.Format.ListWide:
                                $(containerNode).removeClass('sm_field').addClass('sm_field_wide');
                                field.ColumnSpan = 2;
                                break;
                            default:
                                break;
                        }

                        var labelNode = document.createElement('label');

                        if (field.Mandatory)
                        {
                            labelNode.className = 'mandatory';
                        }

                        labelNode.setAttribute('id', 'lbl_' + field.AttributeID);

                        if (field.Mandatory)
                        {
                            labelText = labelText + '*';
                        }

                        if (field.ReadOnly)
                        {
                            labelText = labelText + ' (Read only)';
                        }

                        labelNode.innerHTML = labelText;

                        var inputNode = document.createElement('span');
                        inputNode.className = 'sm_input';

                        var inputElement = document.createElement(fieldTag);
                        inputElement.setAttribute('id', fldType.idPrefix + idName);
                        inputElement.setAttribute('readonly', true);
                        switch (field.FieldType)
                        {
                            case thisNs.FieldTypeEnum.Text.intVal:
                            case thisNs.FieldTypeEnum.TextWide.intVal:                                
                                inputElement.value = field.DefaultValue ? field.DefaultValue : '';
                                break;
                            case thisNs.FieldTypeEnum.Number.intVal:
                                inputElement.setAttribute('type', fldType.inputType);
                                inputElement.setAttribute('maxlength', '250');
                                break;
                            case thisNs.FieldTypeEnum.List.intVal:
                                inputElement.options[inputElement.length] = new Option('List Items', '0');
                                break;
                            case thisNs.FieldTypeEnum.LargeText.intVal:                                
                                if (field.Format === 6)
                                {
                                    inputElement.className = 'richTextBox';
                                    inputElement.innerHTML = field.DefaultValue ? field.DefaultValue : '';
                                }
                                else
                                {
                                    inputElement.setAttribute('cols', '100');
                                    inputElement.setAttribute('rows', '3');
                                    inputElement.value = field.DefaultValue ? field.DefaultValue : '';
                                }
                                break;
                            case thisNs.FieldTypeEnum.TickBox.intVal:
                                inputElement.options[inputElement.length] = new Option('Yes', '0');
                                inputElement.options[inputElement.length] = new Option('No', '1');
                                break;
                            case thisNs.FieldTypeEnum.LookupDisplayField.intVal:
                                inputElement.innerHTML = "&nbsp;" + field.FieldValue;
                                inputElement.title = field.FieldValue;
                                inputNode.className = 'ellide sm_input';
                                break;
                            case thisNs.FieldTypeEnum.Attachment.intVal:
                                inputElement.setAttribute('type', fldType.inputType);
                                inputElement.setAttribute('style', 'width: 80px;');
                                inputElement.value = 'browse ...'
                                break;
                        }

                        var iconNode = document.createElement('span');
                        iconNode.className = 'sm_icon';

                        var tooltipNode = document.createElement('span');
                        tooltipNode.className = 'sm_tooltip';

                        var validatorNode = document.createElement('span');
                        validatorNode.className = 'sm_validators';

                        containerNode.appendChild(labelNode);
                        inputNode.appendChild(inputElement);
                        containerNode.appendChild(inputNode);
                        containerNode.appendChild(iconNode);
                        containerNode.appendChild(tooltipNode);
                        containerNode.appendChild(validatorNode);
                    }

                    var optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, containerNode.id, field);
                    containerNode.appendChild(optionsMenu);

                    $(containerNode).mouseenter(function ()
                    {
                        if (thisNs.CurrentlyDragging === false)
                        {
                            $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                            thisNs.selectedControl = containerNode;
                            thisNs.ShowFormOptions(containerNode.id, thisNs.ControlTypeObject.Field, field.ControlName, field);
                        }
                    }).mouseleave(function ()
                    {
                        thisNs.HideFormOptions(containerNode.id + '_optionsmenu');
                    });

                    containerNode.setAttribute('fieldclass', containerNode.className);

                    var appendToNode = document.getElementById(containerControl);
                    appendToNode.appendChild(containerNode);

                    return containerNode.className;
                },

                SaveForm: function ()
                {
                    sessionStorage.clear();
                    var _misc = SEL.CustomEntityAdministration.Misc;
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (validateform('vgFormEdit') === false)
                    {
                        return;
                    }

                    _misc.LoadingScreenCancelled = false;

                    setTimeout(function () { _misc.ShowInformationMessage('Saving...'); }, 450);

                    var txtSave = $g(txtformsavebuttontextid).value;
                    var txtsaveandduplicate = $g(txtformsaveandduplicatebuttontextid).value;
                    var txtSaveAndStay = $g(txtformsaveandstaybuttontextid).value;
                    var txtSaveAndNew = $g(txtformsaveandnewbuttontextid).value;
                    var txtCancel = $g(txtformcancelbuttontextid).value;
                    var tmpTab = new thisNs.TabObject();
                    var tmpSection = new thisNs.SectionObject();

                    //TODO: Put this block into a seperate function
                    thisNs.FormObj.formName = document.getElementById(txtformnameid).value;
                    thisNs.FormObj.description = document.getElementById(txtformdescriptionid).value;
                    thisNs.FormObj.showSave = txtSave == '' ? false : true;
                    thisNs.FormObj.saveButtonText = txtSave;
                    thisNs.FormObj.showSaveAndDuplicate = txtsaveandduplicate == '' ? false : true;
                    thisNs.FormObj.saveAndDuplicateButtonText = txtsaveandduplicate;
                    thisNs.FormObj.showSaveAndStay = txtSaveAndStay == '' ? false : true;
                    thisNs.FormObj.showSaveAndNew = txtSaveAndNew == '' ? false : true;
                    thisNs.FormObj.saveAndStayButtonText = txtSaveAndStay;
                    thisNs.FormObj.saveAndNewButtonText = txtSaveAndNew;
                    thisNs.FormObj.showCancel = txtCancel == '' ? false : true;
                    thisNs.FormObj.cancelButtonText = txtCancel;
                    thisNs.FormObj.showSubMenus = $('#' + chkshowsubmenuid).is('checked');
                    thisNs.FormObj.showBreadcrumbs = $('#' + chkshowbreadcrumbsid).is(':checked');
                    thisNs.FormObj.hideTorch = $('#' + chkHideTorch).is(':checked');
                    thisNs.FormObj.hideAttachments = $('#' + chkHideAttachments).is(':checked');
                    thisNs.FormObj.hideAudiences = $('#' + chkHideAudiences).is(':checked');
                    thisNs.FormObj.builtIn = $("#" + chkFormBuiltIn).is(":checked");

                    thisNs.FormObj.tabs = [];

                    $('#tabBar .sm_tabheader').each(function (t, tabObj)
                    {
                        tmpTab = new thisNs.TabObject();
                        tmpTab.Order = t;
                        tmpTab.TabControlName = tabObj.id;
                        tmpTab.TabName = EscapeHTMLInString($(tabObj).attr('tabname'));

                        $('#' + tabObj.id.replace('tabbar_', '') + ' .customentityformsection').each(function (s, sectionObj)
                        {
                            tmpSection = new thisNs.SectionObject();
                            tmpSection.Order = s;
                            tmpSection.SectionControlName = sectionObj.id;
                            tmpSection.SectionName = $(sectionObj).attr('sectionname');

                            tmpSection.Fields = thisNs.SortFormFieldOrder(sectionObj.id);

                            tmpTab.Sections.push(tmpSection);
                        });

                        thisNs.FormObj.tabs.push(tmpTab);
                    });

                    Spend_Management.svcCustomEntities.saveForm(cu.AccountID, employeeid, entityid, thisNs.CurrentFormID, thisNs.FormObj, thisNs.SaveFormComplete, thisNs.FormsWebServiceError);
                },

                SaveFormComplete: function (data)
                {
                    var _misc = SEL.CustomEntityAdministration.Misc;

                    // TODO: Change all Ifs to === or !== etc etc
                    if (data == -1)
                    {
                        _misc.LoadingScreenCancelled = true;
                        $('#loadingArea').remove();
                        SEL.MasterPopup.ShowMasterPopup('The Form name you have entered already exists.', 'Message from ' + moduleNameHTML);
                        return;
                    }

                    // if the form is built-in/system the GreenLight will be too (if it wasn't already, it will be now), so update the checkbox for the GreenLight automatically.
                    if ($("#" + chkFormBuiltIn).is(":checked") && !$("#ctl00_contentmain_chkBuiltIn").is(":checked")) {
                        $("#ctl00_contentmain_chkBuiltIn").prop("checked", true).prop("disabled", true);
                    }

                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    thisNs.HideFormModal();
                    $("#dialog").dialog('close');

                    Spend_Management.svcCustomEntities.getFormGrid(entityid, thisNs.RefreshFormGridComplete);

                    _misc.LoadingScreenCancelled = true;

                    $('#loadingArea').fadeOut(600, function ()
                    {
                        $('#loadingArea').remove();
                    });

                    return;
                },

                RefreshFormGridComplete: function (data)
                {
                    if ($e(pnlFormID) === true)
                    {
                        $g(pnlFormID).innerHTML = data[1];
                        SEL.Grid.updateGrid(data[0]);
                    }
                    return;
                },

                ResetFormButtonTextLabels: function ()
                {
                    var lblsavebuttontext = $g(lblsavebuttontextid);
                    var lblsaveandduplicatebuttontext = $g(lblsaveandduplicatebuttontextid);
                    var lblsaveandstaybuttontext = $g(lblsaveandstaybuttontextid);
                    var lblsaveandnewbuttontext = $g(lblsaveandnewbuttontextid);
                    var lblcancelbuttontext = $g(lblcancelbuttontextid);

                    lblsavebuttontext.className = 'mandatory';
                    lblsaveandduplicatebuttontext.className = 'mandatory';
                    lblsaveandstaybuttontext.className = 'mandatory';
                    lblsaveandnewbuttontext.className = 'mandatory';
                    lblcancelbuttontext.className = 'mandatory';
                    if (lblsavebuttontext.innerHTML.indexOf('*') <= 0)
                    {
                        lblsavebuttontext.innerHTML = lblsavebuttontext.innerHTML + '*';
                        lblsaveandduplicatebuttontext.innerHTML = lblsaveandduplicatebuttontext.innerHTML + '*';
                        lblsaveandstaybuttontext.innerHTML = lblsaveandstaybuttontext.innerHTML + '*';
                        lblsaveandnewbuttontext.innerHTML = lblsaveandnewbuttontext.innerHTML + '*';
                        lblcancelbuttontext.innerHTML = lblcancelbuttontext.innerHTML + '*';
                    }
                },

                DeleteForm: function (formid)
                {
                    if (confirm('Are you sure you wish to delete the selected form?'))
                    {
                        var thisNs = SEL.CustomEntityAdministration.Forms;

                        Spend_Management.svcCustomEntities.deleteForm(formid, thisNs.DeleteFormComplete, thisNs.FormsWebServiceError);
                    }
                },

                DeleteFormComplete: function (data)
                {
                    if (data === 0)
                    {
                        Spend_Management.svcCustomEntities.getFormGrid(entityid, SEL.CustomEntityAdministration.Forms.RefreshFormGridComplete);
                    }
                    else if (data === -2)
                    {
                        SEL.MasterPopup.ShowMasterPopup('This form cannot be deleted as it is presently in use by a form selection mapping.', 'Message from ' + moduleNameHTML);
                    }
                    else if (data === -3) {
                        SEL.MasterPopup.ShowMasterPopup('This form cannot be deleted as it is a system form.', 'Message from ' +moduleNameHTML);
                    }
                    else
                    {
                        SEL.MasterPopup.ShowMasterPopup('This form cannot be deleted as it is presently in use by a view.', 'Message from ' + moduleNameHTML);
                    }

                    return;
                },
                
                SelectText: function (element) {
                    var cursorPosition = $(element).prop("selectionStart");
                    var characterAtCursorPosition = ($("#txtAutoLookupDisplayField")
                        .text()
                        .substring(cursorPosition - 1, cursorPosition));
                    if ((characterAtCursorPosition === ",") ||(characterAtCursorPosition === " "))
                       return;
                    var textTillCursor= $("#txtAutoLookupDisplayField").text().substring(0, cursorPosition);
                    var startPos = textTillCursor.lastIndexOf("[");
                    var endPos = $("#txtAutoLookupDisplayField").text().indexOf("]", cursorPosition);
                  
                 $(element).selectRange(startPos, endPos+1);
                },
        
                // TODO: See if this can be done inside the webservice to speed things up
                SortFormFieldOrder: function (sectionID)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;
                    var column = 0;
                    var row = 0;
                    var curFormField;
                    var fieldsList = [];

                    $('#' + sectionID + '_FieldsArea > span').each(function (f, fieldObj)
                    {
                        if (fieldObj.id.indexOf('spacer') === -1)
                        {
                            curFormField = thisNs.GetFieldByID(fieldObj.id);
                        }
                        else
                        {
                            curFormField = thisNs.SpacerField();
                        }

                        if (curFormField.ColumnSpan === 1) // If the field spans across one column
                        {
                            if (column === 0)
                            {
                                curFormField.Column = column;
                                curFormField.Row = row;
                                column = 1;
                            }
                            else
                            {
                                curFormField.Column = column;
                                curFormField.Row = row;
                                column = 0;
                                row++;
                            }
                        }
                        else // If the field spans across two columns
                        {
                            if (column === 1) // If on the right column, move one row down and go back to column 0
                            {
                                row++;
                                column = 0;
                            }

                            curFormField.Column = column;
                            curFormField.Row = row;
                            row++;
                        }

                        if (fieldObj.id.indexOf('spacer') === -1)
                        {
                            fieldsList.push(curFormField);
                        }
                    });

                    return fieldsList;
                },

                ChangeTab: function (tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var currentTabName;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        currentTabName = thisNs.FormObj.tabs[i].TabControlName;
                        document.getElementById(thisNs.FormObj.tabs[i].TabControlName).style.display = 'none';
                        //set the style of the unselected tabs
                        $('#tab_middle_img_' + currentTabName).removeClass('selected_tab');
                    }

                    document.getElementById(tabName).style.display = '';

                    //set the style to selected
                    $('#tab_middle_img_' + tabName).addClass('selected_tab');

                    thisNs.SelectedTab = tabName;

                    thisNs.CreateNoTabsAndSectionsComments();

                    thisNs.MakeSectionFieldsSortable();

                    thisNs.MakeAvailableFieldsDraggable();

                    thisNs.MakeSpacerFieldDraggable();

                    thisNs.MakeAvailableFieldsDroppable();

                    $('#' + thisNs.SelectedTab + ' .sectionsortables').bind("mousedown", function ()
                    {
                        $(this).addClass("closedhand");
                    });

                    $('#' + thisNs.SelectedTab).bind("mousedown", function ()
                    {
                        $('#' + thisNs.SelectedTab).addClass("closedhand");
                    });
                },

                SaveTabWithModalSave: function ()
                {
                    SEL.CustomEntityAdministration.Forms.AddTab($g(txtTabHeaderID).value, true);
                },

                UpdateSectionInFormObj: function (section, tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (tabName === undefined)
                    {
                        tabName = thisNs.SelectedTab;
                    }

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tabName)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                if (thisNs.FormObj.tabs[i].Sections[x].SectionControlName === section.SectionControlName)
                                {
                                    thisNs.FormObj.tabs[i].Sections[x] = section;
                                    return;
                                }
                            }
                        }
                    }
                    return;
                },

                MoveSectionInFormObj: function (section, previousTabName, newTabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === previousTabName)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                if (thisNs.FormObj.tabs[i].Sections[x].SectionControlName === section.SectionControlName)
                                {
                                    thisNs.FormObj.tabs[i].Sections.splice(x, 1);
                                }
                            }
                        }

                        if (thisNs.FormObj.tabs[i].TabControlName === newTabName)
                        {
                            thisNs.FormObj.tabs[i].Sections.push(section);
                        }
                    }
                    return;
                },

                RemoveSectionInFormObj: function (sectionDiv)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;
                    var foundSection = false;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === thisNs.SelectedTab)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                if (foundSection == false && thisNs.FormObj.tabs[i].Sections[x].SectionControlName === sectionDiv.replace('_section', ''))
                                {
                                    thisNs.FormObj.tabs[i].Sections.splice(x, 1);
                                    foundSection = true;
                                }

                                if (foundSection && x < thisNs.FormObj.tabs[i].Sections.length)
                                {
                                    thisNs.FormObj.tabs[i].Sections[x].Order = thisNs.FormObj.tabs[i].Sections[x].Order - 1;
                                }
                            }

                            if (foundSection)
                            {
                                break;
                            }
                        }
                    }
                    return;
                },

                RemoveSection: function (sectionDiv)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (confirm('Are you sure you want to delete this section?'))
                    {
                        $('#' + sectionDiv + '_FieldsArea').children().each(function (f, fieldObj)
                        {
                            var fieldID = $(fieldObj).attr('id');

                            if (fieldID.indexOf('spacer') === -1)
                            {
                                thisNs.RemoveFormField(fieldID, thisNs.GetFieldByID(fieldID));
                            }
                            else
                            {
                                thisNs.RemoveFormField(fieldID, thisNs.SpacerField());
                            }
                        });

                        var sectionArea = document.getElementById(sectionDiv);
                        sectionArea.parentNode.removeChild(sectionArea);
                        thisNs.RemoveSectionInFormObj(sectionDiv);

                        if (thisNs.SelectedSection === sectionDiv)
                        {
                            thisNs.SelectedSection = null;
                        }
                        thisNs.CreateNoTabsAndSectionsComments();
                    }
                },

                // Remove the specified field from the form designer and into the available fields section
                RemoveFormField: function (ControlID, field, helper)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (field.FieldType === thisNs.FieldTypeEnum.Spacer.intVal)
                    {
                        // Remove the spacer from the form
                        $('#' + ControlID).remove();

                        // Remove the temporary helper object from the form
                        if (helper !== undefined)
                        {
                            helper.remove();
                        }
                    }
                    else
                    {
                        if ($('#availableFields').html() === 'There are no remaining fields for this form.')
                        {
                            $('#availableFields').html('');
                        }

                        var formField = $('#' + ControlID);

                        var oldSection = formField.parent().attr('id');

                        // Clone the desired field
                        var cloneField = formField.clone(true);

                        // Default text 'textarea' Fix for Chrome and Firefox                          
                        cloneField.find('textarea').val(formField.find('textarea').val());

                        cloneField.css('display', '');

                        // Add the field back into the Available Fields section
                        thisNs.AddFieldToAvailableFields(field, $(cloneField).attr('fieldclass'), true);

                        // Remove the field from the form
                        formField.remove();

                        // Remove the temporary helper object from the form
                        if (helper !== undefined)
                        {
                            helper.remove();
                        }

                        // Add the field into the availableFieldDom area for future use
                        $('#availableFieldDoms').append(cloneField);

                        // Remove the field from the formObj
                        thisNs.MoveFieldToNewSection(field, oldSection, '');

                        // Set the newly added Available Field to use .draggable
                        thisNs.MakeAvailableFieldsDraggable('#availableField' + field.AttributeID);

                        // Remove custom label text from the field
                        thisNs.EditLabelField = field;
                        thisNs.ResetFieldLabel();
                    }

                    return;
                },

                GetSectionByName: function (sectionName, tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (tabName === undefined)
                    {
                        tabName = thisNs.SelectedTab;
                    }

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tabName)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                if (thisNs.FormObj.tabs[i].Sections[x].SectionControlName === sectionName)
                                {
                                    return thisNs.FormObj.tabs[i].Sections[x];
                                }
                            }
                        }
                    }

                    return null;
                },

                // Remove the specified Spacer field from the form
                RemoveSpacerField: function (field, helper)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    // TODO: Is this supposed to be field.remove()?
                    // Remove the spacer field from the form
                    //formField.remove();

                    // Remove the temporary helper object from the form
                    if (helper !== undefined)
                    {
                        helper.remove();
                    }

                    // TODO: Also, this bit?
                    // Add the field into the availableFieldDom area for future use
                    //$('#availableFieldDoms').append(cloneField);

                    // TODO: I've changed this to pass through '', but really can we not find a better way of taking the spacer out
                    // Remove the field from the formObj
                    thisNs.MoveFieldToNewSection(field, '', '');

                    // Set the newly added Available Field to use .draggable
                    thisNs.MakeAvailableFieldsDraggable('#availableField' + field.AttributeID);

                    // Remove custom label text from the field
                    thisNs.EditLabelField = field;
                    thisNs.ResetFieldLabel();

                    return;
                },

                GetFieldByID: function (fieldID)
                {
                    var field = null;

                    $(SEL.CustomEntityAdministration.Forms.FormFieldDetails).each(function (f, formField)
                    {
                        if (formField.ControlName.toString() === fieldID.toString())
                        {
                            field = formField;

                            // Break out of the loop if a field has been found
                            return false;
                        }
                    });

                    return field;
                },

                RemoveFieldInFormObj: function (fieldName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === thisNs.SelectedTab)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                for (var y = 0; y < thisNs.FormObj.tabs[i].Sections[x].Fields.length; y++)
                                {
                                    if (thisNs.FormObj.tabs[i].Sections[x].Fields[y].ControlName === fieldName)
                                    {
                                        thisNs.FormObj.tabs[i].Sections[x].Fields.splice(y, 1);
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    return null;
                },

                UpdateFieldInFormFieldDetails: function (field, tab)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (tab === undefined)
                    {
                        tab = thisNs.SelectedTab;
                    }

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tab)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                for (var y = 0; y < thisNs.FormObj.tabs[i].Sections[x].Fields.length; y++)
                                {
                                    if (thisNs.FormObj.tabs[i].Sections[x].Fields[y].ControlName === field.ControlName)
                                    {
                                        thisNs.FormObj.tabs[i].Sections[x].Fields[y] = field;

                                        $(thisNs.FormFieldDetails).each(function (f, formField)
                                        {
                                            if (formField.ControlName === field.ControlName)
                                            {
                                                thisNs.FormFieldDetails[f] = field;

                                                // Break out of the loop if a field has been found
                                                return false;
                                            }
                                        });
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    return;
                },

                //set the field read only value as well as update the label text with the word read only
                SetFieldToReadOnly: function (labelCtl)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var field = thisNs.GetFieldByID(thisNs.selectedControl.id);

                    var labelText;

                    if (field.LabelText)
                    {
                        labelText = EscapeHTMLInString(field.LabelText);
                    }
                    else
                    {
                        labelText = EscapeHTMLInString(field.DisplayName.toString());
                    }

                    // If the new label is too large, dotonate it
                    if (labelText.length > 30)
                    {
                        labelText = labelText.substring(0, 30) + '...';
                    }

                    if (field.Mandatory)
                    {
                        labelText = labelText + '*';
                    }

                    if (field.ReadOnly)
                    {
                        field.ReadOnly = false;
                        labelCtl.innerHTML = labelText;
                    }
                    else
                    {
                        field.ReadOnly = true;
                        labelCtl.innerHTML = labelText + ' (Read only)';
                    }

                    thisNs.UpdateFieldInFormFieldDetails(field);
                },

                MoveFieldInFormObj: function (fieldName, moveUp)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === thisNs.SelectedTab)
                        {
                            for (var x = 0; x < thisNs.FormObj.tabs[i].Sections.length; x++)
                            {
                                for (var y = 0; y < thisNs.FormObj.tabs[i].Sections[x].Fields.length; y++)
                                {
                                    if (thisNs.FormObj.tabs[i].Sections[x].Fields[y].ControlName === fieldName)
                                    {
                                        if (moveUp)
                                        {
                                            thisNs.FormObj.tabs[i].Sections[x].Fields.splice(y - 1, 0, thisNs.FormObj.tabs[i].Sections[x].Fields[y]);
                                            thisNs.FormObj.tabs[i].Sections[x].Fields.splice(y + 1, 1);
                                        }
                                        else
                                        {
                                            thisNs.FormObj.tabs[i].Sections[x].Fields.splice(y + 2, 0, thisNs.FormObj.tabs[i].Sections[x].Fields[y]);
                                            thisNs.FormObj.tabs[i].Sections[x].Fields.splice(y, 1);
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    }
                    return;
                },

                UpdateFieldLabel: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (validateform('vgLabel') == false)
                    {
                        return;
                    }

                    if (thisNs.EditLabelField !== undefined)
                    {
                        var labelText = EscapeHTMLInString($('#' + txtfieldlabelID).val());

                        thisNs.EditLabelField.LabelText = labelText;

                        var fieldLabel = $('#' + 'lbl_' + thisNs.EditLabelField.AttributeID);

                        // If the new label is too large, dotonate it
                        if (labelText.length > 30)
                        {
                            fieldLabel.html(labelText.substring(0, 30) + '...');
                        }
                        else
                        {
                            fieldLabel.html(labelText);
                        }

                        // If the field is Read Only or Mandatory, update the label to reflect this
                        if (thisNs.EditLabelField.Mandatory)
                        {
                            fieldLabel.append('*');
                        }

                        if (thisNs.EditLabelField.ReadOnly)
                        {
                            fieldLabel.append(' (Read only)');
                        }

                        // Update the options menu to show the eraser icon
                        var optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, thisNs.EditLabelField.ControlName, thisNs.EditLabelField);
                        $('#' + thisNs.EditLabelField.ControlName).append(optionsMenu);

                        // Update the field within the FormFieldDetails list to reflect the new label
                        thisNs.UpdateFieldInFormFieldDetails(thisNs.EditLabelField);

                        thisNs.EditLabelField = '';

                        thisNs.HideFieldLabelModal();
                    }

                    return;
                },
                
                UpdateDefaultText: function()
                {
                    var nameSpace = SEL.CustomEntityAdministration.Forms;
                    var inputBox = $('#defaultTextFieldContainer .active');
                    var field = inputBox.data('field');

                    if (field)
                    {
                        if (field.Format === 6) // Formatted Text
                        {
                            if (inputBox.get(0))
                            {
                                var editorControl = $f(SEL.CustomEntityAdministration.DomIDs.Forms.EditorExtender);
                                field.DefaultValue = editorControl._editableDiv.innerHTML;
                                $('#form_attribute_' + field.AttributeID).find('.richTextBox').html(field.DefaultValue);
                            }
                        }
                        else
                        {
                            var inputElement = 'textarea';

                            if (field.FieldType === 10)
                            {
                                field.DefaultValue = inputBox.val();
                            }
                            else
                            {
                                if (field.Format !== 2) inputElement = 'input';
                                field.DefaultValue = inputBox.val();
                            }

                            $('#form_attribute_' + field.AttributeID).find(inputElement).val(field.DefaultValue);
                        }

                        nameSpace.UpdateFieldInFormFieldDetails(field);
                    }

                    nameSpace.HideDefaultTextModal();
                },

                ResetFieldLabel: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (thisNs.EditLabelField !== undefined)
                    {
                        var originalText = EscapeHTMLInString(thisNs.EditLabelField.DisplayName);

                        var fieldLabel = $('#' + 'lbl_' + thisNs.EditLabelField.AttributeID);

                        thisNs.EditLabelField.LabelText = null;

                        if (originalText.length > 30)
                        {
                            fieldLabel.html(originalText.substring(0, 30) + '...');
                        }
                        else
                        {
                            fieldLabel.html(originalText);
                        }

                        if (thisNs.EditLabelField.Mandatory)
                        {
                            fieldLabel.append('*');
                        }

                        if (thisNs.EditLabelField.ReadOnly)
                        {
                            fieldLabel.append(' (Read only)');
                        }

                        // Update the option menu to no longer show the eraser icon
                        var optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, thisNs.EditLabelField.ControlName, thisNs.EditLabelField);
                        $('#' + thisNs.EditLabelField.ControlName).append(optionsMenu);

                        thisNs.UpdateFieldInFormFieldDetails(thisNs.EditLabelField);

                        thisNs.EditLabelField = '';
                    }
                },

                // Allow fields to be added to the last section of the current tab by being dropped anywhere on the form
                MakeFormModalDroppable: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    $('#formDesignContents').droppable({
                        accept: '#sm_field_spacer, #availableFields .ui-draggable',
                        over: function (e, ui)
                        {
                            if (thisNs.SelectedTab !== null)
                            {
                                var lastSection = $('#' + thisNs.SelectedTab + ' .sectionsortables').last();

                                if (lastSection.length > 0)
                                {
                                    var pHeight;
                                    var pWidth;

                                    if (ui.helper.attr('class').indexOf('sm_field_spacer') === -1)
                                    {
                                        pWidth = ui.helper.width() - 8;
                                        pHeight = ui.helper.height() - 8;
                                    }
                                    else
                                    {
                                        pHeight = 28;
                                        pWidth = 402;
                                    }

                                    var temporaryPlaceholder = $('<span />').addClass('placeholder-highlight-fields').height(pHeight).width(pWidth).attr('id', 'temporaryPlaceholder');

                                    lastSection.append(temporaryPlaceholder);
                                }
                            }
                        },
                        out: function (e, ui)
                        {
                            $('#temporaryPlaceholder').remove();
                        },
                        drop: function (e, ui)
                        {
                            $('#dialog').addClass('ui-state-disabled').attr('id', 'dialogDisabled');

                            var newOffset,
                                startingPosition,
                                availableFieldHolder,
                                fieldId;

                            if ($('.placeholder-highlight-fields').length > 0 && thisNs.SelectedTab !== null)
                            {
                                var lastSection = $('#' + thisNs.SelectedTab + ' .sectionsortables').last();

                                if (lastSection.length > 0)
                                {
                                    if (ui.draggable.attr('id').indexOf('spacer') === -1)
                                    {
                                        fieldId = '';

                                        if (ui.draggable.attr('id').indexOf('availableField') != -1)
                                        {
                                            fieldId = ui.draggable.attr('id').replace('availableField', 'form_attribute_');
                                        }
                                        else
                                        {
                                            fieldId = ui.draggable.attr('id');
                                        }

                                        thisNs.MoveFieldToNewSection(thisNs.GetFieldByID(fieldId), null, $('#' + thisNs.SelectedTab + ' .sectionsortables').last().attr('id').replace('_FieldsArea', ''));
                                    }

                                    if (ui.draggable.text() === 'Spacer')
                                    {
                                        var spacerElement = $('#form_attribute_spacer_0').clone(true);
                                        var spacerId = 'form_attribute_spacer_' + thisNs.NumberOfSpacers;

                                        spacerElement.attr('id', spacerId);

                                        newOffset = $('#temporaryPlaceholder').offset();

                                        startingPosition = ui.helper.offset();

                                        availableFieldHolder = $('#dragFieldHolder');
                                        availableFieldHolder.append(spacerElement);

                                        spacerElement.css('position', 'absolute').css('top', startingPosition.top).css('left', startingPosition.left);

                                        spacerElement.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.Fields());

                                        spacerElement.animate({ 'top': newOffset.top, 'left': newOffset.left }, '200', function ()
                        {
                            spacerElement.css('position', 'relative').css('top', '').css('left', '');
                            lastSection.append(spacerElement);

                                            var optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, spacerId, thisNs.SpacerField(thisNs.NumberOfSpacers));
                            $(optionsMenu).css('display', 'none');
                            spacerElement.append(optionsMenu);

                            spacerElement.unbind('mouseenter');
                            spacerElement.unbind('mouseleave');

                            spacerElement.mouseenter(function ()
                            {
                                if (thisNs.CurrentlyDragging === false)
                                {
                                    $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                                                    thisNs.selectedControl = $g(spacerId);
                                                    thisNs.ShowFormOptions(spacerId, thisNs.ControlTypeObject.Field, spacerId, thisNs.SpacerField());
                                }
                            })
                            .mouseleave(function ()
                            {
                                                thisNs.HideFormOptions(spacerId + '_optionsmenu');
                            });

                            thisNs.NumberOfSpacers++;
                            thisNs.CurrentlyDragging = false;
                            $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');

                            $('#temporaryPlaceholder').remove();
                        });
                                    }
                                    else
                                    {
                                        fieldId = ui.draggable.attr('fieldid');
                                        var availableFieldId = ui.draggable.attr('availablefieldid');
                                        var fieldElement = $('#' + fieldId);
                                        var fieldPlaceholder = $g(availableFieldId);
                                        var currentField = thisNs.GetFieldByID(fieldId);

                                        newOffset = $('#temporaryPlaceholder').offset();

                                        startingPosition = ui.helper.offset();

                                        availableFieldHolder = $('#dragFieldHolder');
                                        availableFieldHolder.append(fieldElement);

                                        fieldElement.css('position', 'absolute').css('top', startingPosition.top).css('left', startingPosition.left);

                                        fieldElement.css('zIndex', SEL.CustomEntityAdministration.zIndices.Forms.Fields());

                                        fieldElement.animate({ 'top': newOffset.top, 'left': newOffset.left }, '200',
                                            function ()
                                            {
                                                fieldElement.css('position', 'relative').css('top', '').css('left', '');
                                                lastSection.append(fieldElement);

                                                // Remove field from Available fields
                                                var availableFields = $g('availableFields');
                                                availableFields.removeChild(fieldPlaceholder);

                                                if ($('#availableFields').children().length === 0)
                                                {
                                                    $('#availableFields').html('There are no remaining fields for this form.');
                                                    $('#availableFields').css('paddingTop', '10px');
                                                }

                                                $('#' + fieldId + '_optionsmenu').html('');
                                                $('#' + fieldId + '_optionsmenu').remove();

                                                var optionsMenu = thisNs.CreateOptionsMenu(thisNs.ControlTypeObject.Field, fieldId, currentField);

                                                fieldElement.append(optionsMenu);
                                                fieldElement.unbind('mouseenter');
                                                fieldElement.unbind('mouseleave');

                                                fieldElement.mouseenter(function ()
                                                {
                                                    if (thisNs.CurrentlyDragging === false)
                                                    {
                                                        $('#' + thisNs.SelectedTab + ' .sm_form_field_options, #' + popupFormOptionsID).css('display', 'none');
                                                        thisNs.selectedControl = $g(fieldId);
                                                        thisNs.ShowFormOptions(fieldId, thisNs.ControlTypeObject.Field, fieldId, currentField);
                                                    }
                                                }).mouseleave(function ()
                                                {
                                                    thisNs.HideFormOptions(fieldId + '_optionsmenu');
                                                });

                                                thisNs.CurrentlyDragging = false;
                                                $('#dialogDisabled').removeClass('ui-state-disabled').attr('id', 'dialog');

                                                $('#temporaryPlaceholder').remove();
                                            });
                                    }
                                }
                            }
                        }
                    });
                },

                AddFieldToAvailableFields: function (field, fieldClass, sortAlphabetically)
                {
                    var fieldsList = document.getElementById('availableFields');

                    if (fieldClass === undefined || fieldClass === null)
                    {
                        fieldClass = SEL.CustomEntityAdministration.Forms.NewField('availableFieldDoms', field);
                    }

                    var currentFieldInfo = document.createElement('span');
                    currentFieldInfo.id = 'availableField' + field.AttributeID;
                    currentFieldInfo.className = fieldClass;
                    currentFieldInfo.setAttribute('fieldid', 'form_attribute_' + field.AttributeID);
                    currentFieldInfo.setAttribute('availablefieldid', 'availableField' + field.AttributeID);
                    currentFieldInfo.setAttribute('fieldclass', fieldClass);

                    var linkImg = document.createElement('img');
                    linkImg.setAttribute('id', 'lookupfield_' + field.AttributeID);
                    linkImg.setAttribute('src', StaticLibPath + '/icons/16/plain/link.png');
                    linkImg.setAttribute('alt', '');
                    linkImg.setAttribute('title', 'Lookup Display Field');
                    $(linkImg).css('width', '16px').css('height', '16px');

                    var fieldDetails = document.createElement('span');
                    $(fieldDetails).css('display', 'inline-block').css('width', '200').css('height', 'auto').css('line-height', 'normal').css('vertical-align', 'middle');
                    if (field.FieldValue === "" || field.FieldValue === null)
                    {
                        fieldDetails.innerHTML = field.DisplayName;
                    }
                    else
                    {
                        fieldDetails.innerHTML = "<div title=\"" + field.FieldValue + "\" class=\"sm_availablefield_linkedfield ellide\">" + linkImg.outerHTML + "&nbsp;" + field.FieldValue + "</div>" + field.DisplayName;
                    }

                    var dragHandleHolder = document.createElement('span');
                    $(dragHandleHolder).css('display', 'inline-block').css('height', 'auto');
                    $(dragHandleHolder).css('padding-left', '5px').css('padding-right', '15px').css('vertical-align', 'middle');


                    var dragHandleImg = document.createElement('img');
                    dragHandleImg.setAttribute('id', 'draghandle_' + field.AttributeID);
                    dragHandleImg.setAttribute('src', StaticLibPath + '/icons/Custom/HandleBar.png');
                    dragHandleImg.setAttribute('alt', '');
                    dragHandleImg.setAttribute('title', 'Click here to drag this field');
                    $(dragHandleImg).css('width', '19px').css('height', '16px');
                    dragHandleHolder.appendChild(dragHandleImg);

                    currentFieldInfo.appendChild(dragHandleHolder);
                    currentFieldInfo.appendChild(fieldDetails);

                    if (sortAlphabetically === undefined || sortAlphabetically === false)
                    {
                        // If the fields have already been sorted alphabetically, do not re-sort
                        fieldsList.appendChild(currentFieldInfo);
                    }
                    else
                    {
                        if ($("#availableFields > span").length === 0)
                        {
                            // If there is nothing inside Available Fields, append the desired field
                            fieldsList.appendChild(currentFieldInfo);
                        }
                        else
                        {
                            $("#availableFields > span").each(function (x, tmpField)
                            {

                                var currentFieldText = $(tmpField).text();
                                var from = "";

                                if (currentFieldText.substr(currentFieldText.length - 1) === ']')
                                {
                                    currentFieldText = currentFieldText.substr(1, currentFieldText.length - 2);
                                    for (var sourcePos = currentFieldText.length; sourcePos > 0; sourcePos--)
                                    {
                                        if (currentFieldText.substr(sourcePos, 1) === '[')
                                        {
                                            from = currentFieldText.substr(sourcePos + 1);
                                        }
                                    }
                                }

                                currentFieldText = from + currentFieldText;
                                if (field.SortDisplayName.toLowerCase() < currentFieldText.toLowerCase())
                                {
                                    $(currentFieldInfo).insertBefore($(tmpField));

                                    return false;
                                }
                                else if (x === $("#availableFields > span").length - 1)
                                {
                                    // If we are at the end of the list and the field has not been added, append it to the end
                                    fieldsList.appendChild(currentFieldInfo);
                                }
                            });
                        }
                    }
                },

                FormModalTabChange: function (sender, args)
                {
                    SEL.CustomEntityAdministration.Forms.SetDialogTextAndButtonColor();
                    var dialogElement = $('#dialog');
                    var titleBar = $('#formdesigntitle');

                    switch (sender.get_activeTabIndex())
                    {
                        case 0:
                            $('#imgFormDesignerHelp').css('display', 'none');
                            dialogElement.dialog('close');
                            SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
                            break;
                        case 1:
                            $('#imgFormDesignerHelp').css('display', 'inline-block');
                            dialogElement.dialog('open');
                            SEL.CustomEntityAdministration.Base.AddShortCuts('formDesigner');

                            if (dialogElement.dialog('option', 'shownState') === 'docked')
                            {
                                dialogElement.dialog('restore');
                            }
                            else
                            {
                                dialogElement.dialog('collapse');
                            }

                            dialogElement.parent().css('left', titleBar.offset().left + titleBar.outerWidth() - 305);
                            dialogElement.parent().css('top', titleBar.offset().top);
                            break;
                        default:
                            break;
                    }
                },

                // openState: one of -
                //      expand, dock, dockright, dockleft
                ToggleAvailableFields: function (openState)
                {
                    var dialogElement = $('#dialog');
                    var state = dialogElement.dialog('option', 'shownState');

                    if (state === 'collapsed')
                    {
                        switch (openState)
                        {
                            case 'expand':
                                dialogElement.dialog('expand');
                                break;
                            case 'dock':
                                dialogElement.dialog('dock');
                                break;
                            case 'dockleft':
                                dialogElement.dialog('dock', 'left');
                                break;
                            case 'dockright':
                                dialogElement.dialog('dock', 'right');
                                break;
                        }
                    }
                    else if (state === 'docked')
                    {
                        dialogElement.dialog('restore');
                    }
                    else
                    {
                        dialogElement.dialog('collapse');
                    }
                },

                HideFormModal: function ()
                {
                    sessionStorage.clear();
                    SEL.Common.HideModal(modformid);
                    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
                    $("#dialog").dialog('close');
                    $("#availableFieldDoms").html('');

                    // TODO: Investigate doing the line below, does it clear out all .sortable .draggable and .droppable stuff?
                    //$('#formDesignContents').html('');
                    return;
                },

                SetDialogTextAndButtonColor: function ()
                {
                    var titleText = $('.ui-dialog-title');

                    var color = titleText.parent().css('background-color');

                    var colorR;
                    var colorG;
                    var colorB;

                    if (color.indexOf('rgb') === -1)
                    {
                        color = (color.charAt(0) == "#") ? color.substring(1, 7) : color;
                        colorR = parseInt((color).substring(0, 2), 16);
                        colorG = parseInt((color).substring(2, 4), 16);
                        colorB = parseInt((color).substring(4, 6), 16);
                    }
                    else
                    {
                        color = color.replace(/[^0-9,]+/g, "");
                        colorR = color.split(",")[0];
                        colorG = color.split(",")[1];
                        colorB = color.split(",")[2];
                    }

                    var a = 1 - (0.299 * colorR + 0.587 * colorG + 0.114 * colorB) / 255;

                    if (a < 0.5)
                    {
                        $('.ui-dialog-titlebar .ui-icon').removeClass('ui-icon-light').addClass('ui-icon-dark');
                    }
                    else
                    {
                        $('.ui-dialog-titlebar .ui-icon').removeClass('ui-icon-dark').addClass('ui-icon-light');
                    }
                },

                MoveFieldToNewSection: function (field, oldSectionId, newSectionId)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var oldSection = thisNs.GetSectionByName(oldSectionId);
                    var newSection = thisNs.GetSectionByName(newSectionId);

                    if (typeof oldSection !== "undefined" && oldSection !== null)
                    {
                        // Remove the field from the old section
                        for (var y = 0; y < oldSection.Fields.length; y++)
                        {
                            if (oldSection.Fields[y].AttributeID === field.AttributeID)
                            {
                                oldSection.Fields.splice(y, 1);
                                thisNs.UpdateSectionInFormObj(oldSection);
                                break;
                            }
                        }
                    }

                    // Add the field to the new section
                    if (typeof newSection !== "undefined" && newSection !== null)
                    {
                        newSection.Fields.push(field);
                        thisNs.UpdateSectionInFormObj(newSection);
                    }
                },

                ShowSectionModal: function (sectionname, useSelectedTab)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (thisNs.SelectedTab !== null)
                    {
                        SEL.CustomEntityAdministration.Base.AssignDummyShortCuts();

                        if (useSelectedTab !== undefined && useSelectedTab === true)
                        {
                            $('#' + btnSaveSectionid).attr("targetTab", thisNs.SelectedTab);
                        }

                        thisNs.ClearSectionModal();
                        
                        if (typeof sectionname !== "undefined" && sectionname !== null)
                        {
                            $('#lblSectionModalTitle').html('Section : ' + sectionname);
                            $g(txtsectionid).value = sectionname;
                        }
                        else
                        {
                            $('#lblSectionModalTitle').html('New Section');
                        }

                        SEL.Common.ShowModal(modsectionid);
                    }

                    $g(txtsectionid).select();
                },

                ClearSectionModal: function ()
                {
                    validator = $g(reqSectionHeaderID);
                    validator.isvalid = true;
                    ValidatorUpdateDisplay(validator);
                    $g(txtsectionid).value = '';
                },

                HideFieldLabelModal: function ()
                {
                    SEL.Common.Page_ClientValidateReset();
                    SEL.Common.HideModal(modfieldid);
                },

                ShowFieldLabelModal: function ()
                {
                    SEL.Common.ShowModal(modfieldid);

                    $g(txtfieldlabelID).select();
                },

                ClearFieldLabelModal: function ()
                {
                    document.getElementById(txtfieldid).value = '';
                },
                
                HideDefaultTextModal: function()
                {                    
                    SEL.Common.HideModal('modDefaultText');
                    
                    $('#defaultTextFieldContainer .active').val('');
                },

                ShowDefaultTextModal: function()
                {
                    SEL.Common.ShowModal('modDefaultText');

                    $('#defaultTextFieldContainer .active').select();
                },

                ShortcutTabEdit: function ()
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    if (thisNs.SelectedTab)
                    {
                        SEL.CustomEntityAdministration.Base.AssignDummyShortCuts();

                        thisNs.EditTabMode = true;
                        thisNs.EditTabObject = $('#tabbar_' + thisNs.SelectedTab);
                        thisNs.ShowTabModal((thisNs.EditTabObject.length > 0 ? thisNs.EditTabObject.attr('tabname') : null));
                    }
                    return;
                },

                UpdateTabInFormObj: function (tab)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (thisNs.FormObj.tabs[i].TabControlName === tab.TabControlName)
                        {
                            thisNs.FormObj.tabs[i] = tab;
                            return;
                        }
                    }
                    return;
                },

                RemoveTabInFormObj: function (tabName)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var foundTab = false;

                    for (var i = 0; i < thisNs.FormObj.tabs.length; i++)
                    {
                        if (foundTab == false && thisNs.FormObj.tabs[i].TabControlName === tabName)
                        {
                            thisNs.FormObj.tabs.splice(i, 1);
                            foundTab = true;
                        }

                        if (foundTab && i < thisNs.FormObj.tabs.length)
                        {
                            thisNs.FormObj.tabs[i].Order = thisNs.FormObj.tabs[i].Order - 1;
                        }
                    }

                    return;
                },

                SaveSectionWithModalSave: function ()
                {
                    SEL.CustomEntityAdministration.Forms.AddSection(document.getElementById(txtSectionHeaderID).value, true);
                },

                AddSectionFromFormButton: function ()
                {
                    SEL.CustomEntityAdministration.Forms.AddSection($g(txtSectionHeaderID).value, true);
                },

                //Move the selected field control right on the form
                MoveElementRight: function (ControlName)
                {
                    var field = $('#' + ControlName);

                    if (field.next().length > 0)
                    {
                        field.insertAfter(field.next());
                    }
                },

                //Move the selected field control left on the form
                MoveElementLeft: function (ControlName)
                {
                    var field = $('#' + ControlName);

                    if (field.prev().length > 0)
                    {
                        field.insertBefore(field.prev());
                    }
                },

                // Create the options menu for a Tab, Section or Field on the Form Designer
                CreateOptionsMenu: function (ControlType, ControlName, field)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;
                    var controlsNode;

                    if (field === null)
                    {
                        controlsNode = document.getElementById(popupFormOptionsID);
                        controlsNode.className = 'sm_form_field_options';
                    }
                    else
                    {
                        // Remove any previous options menu, if it exists        
                        $('#' + ControlName + '_optionsmenu').remove();

                        controlsNode = document.getElementById(popupFieldOptionsID).cloneNode(true);
                        controlsNode.id = ControlName + '_optionsmenu';
                        controlsNode.className = 'sm_form_field_options';

                        $(controlsNode).html('');
                        $(controlsNode).css('display', 'none');
                        $(controlsNode).css('height', '26px');
                    }

                    // ========= Create Left/Right or Up/Down buttons =============

                    if (ControlType === thisNs.ControlTypeObject.Section)
                    {
                        var moveUpBtn = document.createElement('img');

                        moveUpBtn.onclick = function ()
                        {
                            thisNs.MoveElementLeft(ControlName);
                            $('#' + popupFormOptionsID).css('display', 'none');
                            return;
                        };

                        moveUpBtn.setAttribute('src', '../images/icons/16/Plain/arrow_up_blue.png');
                        moveUpBtn.setAttribute('alt', 'Move up');
                        moveUpBtn.setAttribute('title', 'Move up');
                        moveUpBtn.className = 'sm_btn';
                        controlsNode.appendChild(moveUpBtn);

                        var moveDownBtn = document.createElement('img');
                        moveDownBtn.onclick = function ()
                        {
                            thisNs.MoveElementRight(ControlName);
                            $('#' + popupFormOptionsID).css('display', 'none');
                            return;
                        };

                        moveDownBtn.setAttribute('src', '../images/icons/16/Plain/arrow_down_blue.png');
                        moveDownBtn.setAttribute('alt', 'Move down');
                        moveDownBtn.setAttribute('title', 'Move down');
                        moveDownBtn.className = 'sm_btn';
                        controlsNode.appendChild(moveDownBtn);
                    }
                    else
                    {
                        var moveLeftBtn = document.createElement('img');

                        moveLeftBtn.onclick = function ()
                        {
                            switch (ControlType)
                            {
                                case thisNs.ControlTypeObject.Tab:
                                    thisNs.MoveElementLeft('tabbar_' + ControlName);
                                    thisNs.EditTabObject = null;
                                    thisNs.EditTabMode = false;
                                    $('#' + popupFormOptionsID).css('display', 'none');
                                    break;
                                case thisNs.ControlTypeObject.Field:
                                    thisNs.MoveElementLeft(ControlName);
                                    $('#' + ControlName + '_optionsmenu').css('display', 'none');
                                    break;
                                default:
                                    break;
                            }

                            return;
                        };

                        moveLeftBtn.setAttribute('src', '../images/icons/16/Plain/arrow_left_blue.png');
                        moveLeftBtn.setAttribute('alt', 'Move left');
                        moveLeftBtn.setAttribute('title', 'Move left');
                        moveLeftBtn.className = 'sm_btn';
                        controlsNode.appendChild(moveLeftBtn);

                        //Create the move field right button for the field
                        var moveRightBtn = document.createElement('img');

                        moveRightBtn.onclick = function ()
                        {
                            switch (ControlType)
                            {
                                case thisNs.ControlTypeObject.Tab:
                                    thisNs.MoveElementRight('tabbar_' + ControlName);
                                    thisNs.EditTabObject = null;
                                    thisNs.EditTabMode = false;
                                    $('#' + popupFormOptionsID).css('display', 'none');
                                    break;
                                case thisNs.ControlTypeObject.Field:
                                    thisNs.MoveElementRight(ControlName);
                                    $('#' + ControlName + '_optionsmenu').css('display', 'none');
                                    break;
                                default:
                                    break;
                            }
                            return;
                        };

                        moveRightBtn.setAttribute('src', '../images/icons/16/Plain/arrow_right_blue.png');
                        moveRightBtn.setAttribute('alt', 'Move right');
                        moveRightBtn.setAttribute('title', 'Move right');
                        moveRightBtn.className = 'sm_btn';
                        controlsNode.appendChild(moveRightBtn);
                    }

                    // =========== Create Edit button ================

                    if (ControlType === thisNs.ControlTypeObject.Section)
                    {
                        var addSectionEditBtn = document.createElement('img');
                        addSectionEditBtn.onclick = function ()
                        {
                            thisNs.EditSectionMode = true;

                            var tmpSection = $('#' + ControlName);

                            thisNs.EditSectionObject = tmpSection;

                            thisNs.ShowSectionModal(tmpSection.attr('sectionname'));

                            $('#' + popupFormOptionsID).css('display', 'none');
                            return;
                        };

                        addSectionEditBtn.setAttribute('src', '../images/icons/16/Plain/edit.png');
                        addSectionEditBtn.setAttribute('alt', 'Edit section name');
                        addSectionEditBtn.setAttribute('title', 'Edit section name');
                        addSectionEditBtn.className = 'sm_btn';
                        controlsNode.appendChild(addSectionEditBtn);
                    }
                    else if (ControlType === thisNs.ControlTypeObject.Tab)
                    {
                        var addTabEditBtn = document.createElement('img');
                        addTabEditBtn.onclick = function ()
                        {
                            thisNs.EditTabMode = true;
                            // TODO: Check that the variable below should be SelectedTab, as that may well conflict with current global vars
                            var selectedTab = $('#tabbar_' + ControlName);
                            thisNs.EditTabObject = selectedTab;

                            thisNs.ShowTabModal(selectedTab.attr('tabname'));
                            $('#' + popupFormOptionsID).css('display', 'none');

                            return;
                        };

                        addTabEditBtn.setAttribute('src', '../images/icons/16/Plain/edit.png');
                        addTabEditBtn.setAttribute('alt', 'Edit tab name');
                        addTabEditBtn.setAttribute('title', 'Edit tab name');
                        addTabEditBtn.className = 'sm_btn';
                        controlsNode.appendChild(addTabEditBtn);
                    }
                    else
                    {
                        // If the field is a comment
                        if ((field.FieldType === thisNs.FieldTypeEnum.Comment.intVal ||
                            // or if the field is a summary
                            field.FieldType === thisNs.FieldTypeEnum.OTMSummary.intVal ||
                            // or if the field is a one to many relationship
                                (field.FieldType === thisNs.FieldTypeEnum.Relationship.intVal && field.RelationshipType === thisNs.RelationshipType.OneToMany) ||
                            // or if the field is a spacer
                                    field.FieldType === thisNs.FieldTypeEnum.Spacer.intVal) === false)
                            // Then don't add the edit label image
                        {
                            //Create the Edit Label Text button for the field
                            var addFieldEditBtn = document.createElement('img');
                            addFieldEditBtn.onclick = function ()
                            {
                                $('#' + ControlName + '_optionsmenu').css('display', 'none');

                                thisNs.EditLabelField = field;

                                if (field.LabelText)
                                {
                                    $('#' + txtfieldlabelID).val(field.LabelText);
                                }
                                else
                                {
                                    $('#' + txtfieldlabelID).val(field.DisplayName);
                                }

                                $('#lblFieldLabelModalTitle').html('Field label : ' + field.DisplayName);

                                thisNs.ShowFieldLabelModal();

                                return;
                            };

                            addFieldEditBtn.setAttribute('src', '../images/icons/16/Plain/edit.png');
                            addFieldEditBtn.setAttribute('alt', 'Edit label text');
                            addFieldEditBtn.setAttribute('title', 'Edit label text');
                            addFieldEditBtn.className = 'sm_btn';
                            controlsNode.appendChild(addFieldEditBtn);
                        }
                    }

                    // ==========Create other buttons ==============

                    if (ControlType === thisNs.ControlTypeObject.Tab)
                    {
                        // Add Section button
                        var addSectionBtn = document.createElement('img');
                        addSectionBtn.onclick = function ()
                        {
                            thisNs.ShowSectionModal();
                            $('#' + popupFormOptionsID).css('display', 'none');
                            return;
                        };

                        addSectionBtn.setAttribute('src', '../images/icons/16/Plain/row_add_before.png');
                        addSectionBtn.setAttribute('alt', 'New section');
                        addSectionBtn.setAttribute('title', 'New section');
                        addSectionBtn.className = 'sm_btn';
                        controlsNode.appendChild(addSectionBtn);
                        thisNs.EditTabObject = $('#' + ControlName);
                    }

                    if (ControlType === thisNs.ControlTypeObject.Field)
                    {
                        //Create the Reset Label Text button for the field
                        if (field.LabelText && field.LabelText !== field.DisplayName)
                        {
                            thisNs.EditLabelField = field;

                            var addFieldResetLabelBtn = document.createElement('img');
                            addFieldResetLabelBtn.onclick = function ()
                            {
                                $('#' + ControlName + '_optionsmenu').css('display', 'none');
                                thisNs.EditLabelField = field;
                                thisNs.ResetFieldLabel();
                            };

                            addFieldResetLabelBtn.setAttribute('src', '../images/icons/16/Plain/erase.png');
                            addFieldResetLabelBtn.setAttribute('alt', 'Remove label text');
                            addFieldResetLabelBtn.setAttribute('title', 'Remove label text');
                            addFieldResetLabelBtn.className = 'sm_btn';
                            controlsNode.appendChild(addFieldResetLabelBtn);
                        }

                        //Create the Filter Field button for n:1 field
                        if (field.RelationshipType === thisNs.RelationshipType.ManyToOne) {
                            //Create the Edit Label Text button for the field
                            var addFieldFilterBtn = document.createElement('img');
                            addFieldFilterBtn.onclick = function () {
                                $('#' + ControlName + '_optionsmenu').css('display', 'none');

                                thisNs.addFieldFilterBtn = field;

                                editAttribute(field.AttributeID, field.FieldType,false);
                                return;
                            };

                            addFieldFilterBtn.setAttribute('src', '../images/icons/16/Plain/filter.png');
                            addFieldFilterBtn.setAttribute('alt', 'Edit filter');
                            addFieldFilterBtn.setAttribute('title', 'Edit filter');
                            addFieldFilterBtn.className = 'sm_btn';
                            controlsNode.appendChild(addFieldFilterBtn);
                        }


                        //Create the parent Filter Field button for n:1 field
                        if (field.RelationshipType === thisNs.RelationshipType.ManyToOne) {
                            //Create the Edit Label Text button for the field
                            var addParentFilterBtn = document.createElement('img');
                            
                            addParentFilterBtn.onclick = function () {
                                $('#' + ControlName + '_optionsmenu').css('display', 'none');

                                thisNs.addFieldFilterBtn = field;
                                SEL.CustomEntityAdministration.ParentFilter.ChildElementId = field.AttributeID;
                                editAttribute(field.AttributeID, field.FieldType, true);
                              
                                return;
                            };
                            
                            addParentFilterBtn.setAttribute('src', '../images/icons/16/Plain/funnel_add.png');
                            addParentFilterBtn.setAttribute('alt', 'Add a parent filter');
                            addParentFilterBtn.setAttribute('title', 'Add a parent filter');
                            addParentFilterBtn.className = 'sm_btn';
                            controlsNode.appendChild(addParentFilterBtn);
                        }


                        if ((field.FieldType === thisNs.FieldTypeEnum.Comment.intVal || field.FieldType === thisNs.FieldTypeEnum.OTMSummary.intVal || field.FieldType === thisNs.FieldTypeEnum.Spacer.intVal || field.FieldType === thisNs.FieldTypeEnum.LookupDisplayField.intVal ||
                            (field.FieldType === thisNs.FieldTypeEnum.Relationship.intVal && field.RelationshipType === thisNs.RelationshipType.OneToMany)) === false)
                        {                            
                            if (field.FieldType === thisNs.FieldTypeEnum.Text.intVal || field.FieldType === thisNs.FieldTypeEnum.LargeText.intVal)
                            {
                                var defaultText = $('#txtDefaultText');
                                var defaultTextLarge = $('#txtDefaultTextLarge');
                                var defaultTextFormatted = $('#' + SEL.CustomEntityAdministration.DomIDs.Forms.RichTextEditor);
                                var defaultTextExtender = $find(SEL.CustomEntityAdministration.DomIDs.Forms.EditorExtender);
                                var htmlEditorContainer = $('#HtmlEditorContainer');
                                $('.popupDiv').hide();

                                var defaultValBtn = document.createElement('img');
                                defaultValBtn.onclick = function()
                                {
                                    defaultText.add(defaultTextLarge).hide().removeClass('active');
                                    defaultTextLarge.add(defaultTextLarge).hide().removeClass('active');
                                    defaultTextFormatted.removeClass('active');
                                    htmlEditorContainer.hide();
                                    
                                    var fieldId = 'form_attribute_' + field.AttributeID;
                                    var currentDefaultText;
                                    var modalClass = 'modalpanel formpanel formpanelsmall';
                                    var fieldClass = 'twocolumn';

                                    if (field.Format === thisNs.Format.FormattedText)
                                    {
                                        modalClass = 'modalpanel formpanel richTextModal';
                                        fieldClass = 'sm_field_tall onecolumn richTextField';

                                        htmlEditorContainer.show();
                                        defaultTextFormatted.addClass('active');

                                        currentDefaultText = $('#' + fieldId).find('.richTextBox').html();
                        
                                        if (defaultTextFormatted.length >= 1)
                                        {
                                            defaultTextExtender._editableDiv.innerHTML = currentDefaultText;
                                        }                                        
                                    }
                                    else if (field.Format === thisNs.Format.MultiLine || (field.Format === thisNs.Format.SingleLine && field.ColumnSpan === 2))
                                    {
                                        modalClass = 'modalpanel formpanel';
                                        fieldClass = 'sm_field_tall onecolumn';

                                        defaultTextLarge.show().addClass('active').off('keyup blur');

                                        var maxLength = field.MaxLength > 0 ? field.MaxLength : 4000;
                                        
                                        defaultTextLarge.on('keyup blur', function()
                                        {
                                            var val = $(this).val();
                                            // Trim the field if it has content over the maxlength. 
                                            if (val.length > maxLength)
                                            {
                                                $(this).val(val.slice(0, maxLength));
                                            }
                                        });                                        

                                        currentDefaultText = $('#' + fieldId).find('textarea').val();
                                    }
                                    else
                                    {
                                        if (field.Format === thisNs.Format.SingleLineWide)
                                        {
                                            modalClass = 'modalpanel formpanel';
                                            fieldClass = 'onecolumnsmall';
                                        }

                                        defaultText.show().addClass('active').attr('maxlength', field.MaxLength > 0 ? field.MaxLength : 500);
                                        currentDefaultText = $('#' + fieldId).find('input').val();
                                    }

                                    $('#defaultTextFieldContainer .active').val(currentDefaultText).data('field', field);

                                    $('#defaultTextModalTitle').text('Default text : ' + field.DisplayName);

                                    $('#pnlDefaultText').attr('class', modalClass);
                                    $('#defaultTextFieldContainer').attr('class', fieldClass);

                                    thisNs.ShowDefaultTextModal();
                                };
                                defaultValBtn.setAttribute('src', '../images/icons/16/plain/document_new.png');
                                defaultValBtn.setAttribute('alt', 'Edit default field value');
                                defaultValBtn.setAttribute('title', 'Edit default field value');
                                defaultValBtn.className = 'sm_btn';
                                controlsNode.appendChild(defaultValBtn);
                            }

                            //Create the mandatory button for the field
                            var mandatoryButton = document.createElement('img');
                            var mandatoryIcon = (field.Mandatory) ? 'mandatory_check.png' : 'mandatory_check_disable.png';

                            mandatoryButton.onclick = function () {
                                field.MandatoryCheckOverride = true;
                                if ($('#' + field.ControlName + ' label').hasClass('mandatory')) {
                                    mandatoryIcon = 'mandatory_check_disable.png';
                                    $(this).attr('alt', 'Assign mandatory check');
                                    $(this).attr('title', 'Assign mandatory check');
                                    $('#' + field.ControlName + ' label').removeClass('mandatory');
                                    $('#lbl_' + field.AttributeID).text($('#lbl_' + field.AttributeID).text().slice(0, -1));
                                    field.Mandatory = false;
                                } else {
                                    mandatoryIcon = 'mandatory_check.png';
                                    $(this).attr('alt', 'Remove mandatory check');
                                    $(this).attr('title', 'Remove mandatory check');
                                    $('#' + field.ControlName + ' label').addClass('mandatory');
                                    $('#lbl_' + field.AttributeID).append('*');
                                    field.Mandatory = true;
                                }

                                $(this).attr('src', '../images/icons/16/Plain/' + mandatoryIcon);
                                return;
                            };

                            if (field.Mandatory) {
                                mandatoryButton.setAttribute('alt', 'Remove mandatory check');
                                mandatoryButton.setAttribute('title', 'Remove mandatory check');
                            }
                            else {
                                mandatoryButton.setAttribute('alt', 'Assign mandatory check');
                                mandatoryButton.setAttribute('title', 'Assign mandatory check');
                            }

                            mandatoryButton.setAttribute('src', '../images/icons/16/Plain/' + mandatoryIcon);
                            mandatoryButton.className = 'sm_btn';
                            controlsNode.appendChild(mandatoryButton);


                            //Create the read only button for the field
                            var readOnlyBtn = document.createElement('img');
                            readOnlyBtn.onclick = function ()
                            {
                                thisNs.selectedControl = $g(field.ControlName);

                                thisNs.SetFieldToReadOnly(document.getElementById('lbl_' + field.AttributeID));
                                return;
                            };

                            if (field.FieldType != thisNs.FieldTypeEnum.Attachment.intVal) {
                            readOnlyBtn.setAttribute('src', '../images/icons/16/Plain/text_lock.png');
                            readOnlyBtn.setAttribute('alt', 'Read only');
                            readOnlyBtn.setAttribute('title', 'Read only');
                            readOnlyBtn.className = 'sm_btn';
                            controlsNode.appendChild(readOnlyBtn);
                        }
                    }
                    }

                    // ========== Create Delete button ===========

                    var deleteBtn = document.createElement('img');

                    deleteBtn.onclick = function ()
                    {
                        switch (ControlType)
                        {
                            case thisNs.ControlTypeObject.Tab:
                                thisNs.RemoveTab(ControlName);
                                thisNs.EditTabObject = null;
                                thisNs.EditTabMode = false;
                                $('#' + popupFormOptionsID).css('display', 'none');
                                break;
                            case thisNs.ControlTypeObject.Section:
                                thisNs.RemoveSection(ControlName);
                                $('#' + popupFormOptionsID).css('display', 'none');
                                break;
                            case thisNs.ControlTypeObject.Field:
                                thisNs.RemoveFormField(ControlName, field);
                                break;
                            default:
                                break;
                        }
                        return;
                    };

                    deleteBtn.setAttribute('src', '../images/icons/16/Plain/delete2.png');
                    deleteBtn.setAttribute('alt', 'Delete');
                    deleteBtn.setAttribute('title', 'Delete');
                    deleteBtn.className = 'sm_btn';
                    controlsNode.appendChild(deleteBtn);

                    return controlsNode;
                },

                // TODO: Either put the following into an 'Forms.Objects' namespace, or just refactor them out.           
                // TODO: All the below needs converting to object lit
                //####Form Object Literal####
                FormObject: function ()
                {
                    this.formName = null;
                    this.description = null;
                    this.showSave = false;
                    this.saveButtonText = null;
                    this.showSaveAndDuplicate = false;
                    this.saveAndDuplicateButtonText = null;
                    this.showSaveAndStay = false;
                    this.saveAndStayButtonText = null;
                    this.showSaveAndNew = false;
                    this.saveAndNewButtonText = null;
                    this.showCancel = false;
                    this.cancelButtonText = null;
                    this.showSubMenus = false;
                    this.showBreadcrumbs = false;
                    this.hideTorch = false;
                    this.hideAudiences = false;
                    this.hideAttachments = false;
                    this.tabs = [];
                },

                //####Tab Object Literal####
                TabObject: function ()
                {
                    this.TabName = null;
                    this.TabControlName = null;
                    this.Sections = [];
                    this.Order = null;
                },

                //####Section Object Literal####
                SectionObject: function ()
                {
                    this.SectionName = null;
                    this.SectionControlName = null;
                    this.Fields = [];
                    this.Order = null;
                },

                //####Field Object Literal#####
                FieldObject: function ()
                {
                    this.AttributeID = null;
                    this.ControlName = null;
                    this.DisplayName = null;
                    this.Tooltip = null;
                    this.Mandatory = null;
                    this.FieldType = null;
                    this.ReadOnly = null;
                    this.Row = null;
                    this.Column = null;
                    this.Format = null;
                    this.CommentText = null;
                    this.LabelText = null;
                    this.ColumnSpan = null;
                    this.DefaultValue = null;
                    this.MaxLength = null;
                    this.MandatoryCheckOverride = null;
                },

                //Create a new Spacer object for use when saving a Form
                SpacerField: function (AttributeID)
                {
                    if (AttributeID === undefined)
                    {
                        AttributeID = 0;
                    }

                    var spacerField = new SpendManagementLibrary.sCEFieldDetails();

                    spacerField.AttributeID = AttributeID;
                    spacerField.Column = 0;
                    spacerField.ColumnSpan = 0;
                    spacerField.CommentText = null;
                    spacerField.ControlName = 'form_attribute_spacer_' + AttributeID;
                    spacerField.Description = '';
                    spacerField.DisplayName = 'Spacer';
                    spacerField.FieldType = 20;
                    spacerField.Format = 1;
                    spacerField.ColumnSpan = 1;
                    spacerField.Row = 0;
                    spacerField.Tooltip = '';

                    return spacerField;
                },

                SetFieldType: function (fieldType)
                {
                    var thisNs = SEL.CustomEntityAdministration.Forms;

                    var fldType = null;

                    switch (fieldType)
                    {
                        case 1:
                            fldType = thisNs.FieldTypeEnum.Text;
                            break;
                        case 2:
                            fldType = thisNs.FieldTypeEnum.Integer;
                            break;
                        case 3:
                            fldType = thisNs.FieldTypeEnum.DateTime;
                            break;
                        case 4:
                            fldType = thisNs.FieldTypeEnum.List;
                            break;
                        case 5:
                            // YesNo fields should be shown as a drop down list
                            fldType = thisNs.FieldTypeEnum.List;
                            break;
                        case 6:
                            fldType = thisNs.FieldTypeEnum.Currency;
                            break;
                        case 7:
                            fldType = thisNs.FieldTypeEnum.Number;
                            break;
                        case 8:
                            fldType = thisNs.FieldTypeEnum.Hyperlink;
                            break;
                        case 9:
                            fldType = thisNs.FieldTypeEnum.Relationship;
                            break;
                        case 10:
                            fldType = thisNs.FieldTypeEnum.LargeText;
                            break;
                        case 11:
                            fldType = thisNs.FieldTypeEnum.RunWorkflow;
                            break;
                        case 12:
                            fldType = thisNs.FieldTypeEnum.RelationshipTextBox;
                            break;
                        case 13:
                            fldType = thisNs.FieldTypeEnum.AutoCompleteTextbox;
                            break;
                        case 15:
                            fldType = thisNs.FieldTypeEnum.OTMSummary;
                            break;
                        case 16:
                            fldType = thisNs.FieldTypeEnum.DynamicHyperlink;
                            break;
                        case 17:
                            fldType = thisNs.FieldTypeEnum.List;
                            break;
                        case 19:
                            fldType = thisNs.FieldTypeEnum.Comment;
                            break;
                        case 20:
                            fldType = thisNs.FieldTypeEnum.Spacer;
                            break;
                        case 21:
                            fldType = thisNs.FieldTypeEnum.LookupDisplayField;
                            break;
                        case 22:
                            fldType = thisNs.FieldTypeEnum.Attachment;
                            break;
                        case 23:
                            fldType = thisNs.FieldTypeEnum.Contact;
                            break;
                        default:
                            break;
                    }

                    return fldType;
                },

                //####Field Type enumerable type####
                FieldTypeEnum: {
                    None: { intVal: 0 },
                    Text: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 1 },
                    Integer: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 2 },
                    DateTime: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 3 },
                    List: { idPrefix: 'ddl', className: 'sm_field twocolumn', fieldTag: 'select', columnSpan: 1, intVal: 4 },
                    TickBox: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'checkbox', columnSpan: 1, intVal: 5 },
                    Currency: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 6 },
                    Number: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 7 },
                    Hyperlink: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 8 },
                    Relationship: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 9, attributeName: 'One To Many' },
                    LargeText: { idPrefix: 'txt', className: 'sm_field_tall onecolumn', fieldTag: 'textarea', columnSpan: 2, intVal: 10 },
                    RunWorkflow: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 11 },
                    RelationshipTextBox: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', columnSpan: 1, inputType: 'text', intVal: 12 },
                    AutoCompleteTextbox: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', columnSpan: 1, inputType: 'text', intVal: 13 },
                    OTMSummary: { idPrefix: 'tbl', className: 'sm_field_grid onecolumn', fieldTag: 'table', intVal: 15, columnSpan: 2, attributeName: 'Summary' },
                    TextWide: { idPrefix: 'txt', className: 'sm_field_wide onecolumn', fieldTag: 'input', inputType: 'text', columnSpan: 2, intVal: 16 },
                    ListWide: { idPrefix: 'ddl', className: 'sm_field_wide onecolumn', fieldTag: 'select', columnSpan: 2, intVal: 17 },
                    Comment: { idPrefix: 'pnl', className: 'sm_field_comment onecolumn', fieldTag: 'span', columnSpan: 2, intVal: 19 },
                    Spacer: { className: 'sm_field_spacer', fieldTag: 'span', intVal: 20 },
                    LookupDisplayField: { idPrefix: 'ldf', className: 'sm_field twocolumn', fieldTag: 'span', inputType: 'text', columnSpan: 1, intVal: 21 },
                    Attachment: { idPrefix: 'fileUpload', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'button', columnSpan: 1, intVal: 22 },
                    Contact: { idPrefix: 'txt', className: 'sm_field twocolumn', fieldTag: 'input', inputType: 'text', columnSpan: 1, intVal: 23 }
                },

                //####Field Format enumerable type####
                Format: {
                    None: 0,
                    SingleLine: 1,
                    MultiLine: 2,
                    DateTime: 3,
                    DateOnly: 4,
                    TimeOnly: 5,
                    FormattedText: 6,
                    SingleLineWide: 7,
                    ListStandard: 8,
                    ListWide: 9
                },

                //####Relationship type enumerable####
                RelationshipType: {
                    ManyToOne: 1,
                    OneToMany: 2
                }
            },

            /// <comment>
            /// This contains the methods and parameters relevant to the Views section of custom entity admin
            /// </comment>
            Views: {
                Grid: {
                    Refresh: function (data)
                    {
                        if ($e(SEL.CustomEntityAdministration.DomIDs.Views.Grid) === true)
                        {
                            $g(SEL.CustomEntityAdministration.DomIDs.Views.Grid).innerHTML = data[1];
                            SEL.Grid.updateGrid(data[0]);
                        }
                        return;
                    }
                },

                Modal: {
                    // show the view modal
                    Show: function ()
                    {
                        SEL.Common.Page_ClientValidateReset();
                        var viewModal = SEL.CustomEntityAdministration.DomIDs.Views.Modal,
                            viewModalZIndex = SEL.CustomEntityAdministration.zIndices.Views.Modal,
                            _misc = SEL.CustomEntityAdministration.Misc,
                            imgViewFilterHelp,
                            imgViewColumnHelp,
                            viewHelp;

                        $f(viewModal.TabContainer).set_activeTabIndex(0);
                        SEL.Common.ShowModal(viewModal.Control);
                        $f(viewModal.Control)._backgroundElement.style.zIndex = viewModalZIndex - 5;
                        $f(viewModal.Control)._popupElement.style.zIndex = viewModalZIndex;
                        ValidatorEnable($g(viewModal.Sort.OrderValidator), false);

                        imgViewFilterHelp = $('#imgViewFilterHelp');
                        imgViewFilterHelp.css('display', 'none');
                        imgViewFilterHelp.css('left', imgViewFilterHelp.parent().outerWidth() - 58);
                        imgViewFilterHelp.css('top', imgViewFilterHelp.parent().outerHeight() - 88);
                        imgViewFilterHelp.mouseenter(function ()
                        {
                            viewHelp = $('#viewFilterHelpArea');
                            viewHelp.css('zIndex', SEL.CustomEntityAdministration.zIndices.Views.HelpIcon());
                            viewHelp.css('left', $(this).offset().left - viewHelp.outerWidth());
                            viewHelp.css('top', $(this).offset().top - viewHelp.outerHeight());
                            viewHelp.css('position', 'absolute');
                            viewHelp.stop(true, true).fadeIn(400);
                        }).mouseleave(function ()
                        {
                            $('#viewFilterHelpArea').stop(true, true).fadeOut(200);
                        });
                        imgViewColumnHelp = $('#imgViewColumnHelp');
                        imgViewColumnHelp.css('display', 'none');
                        imgViewColumnHelp.css('left', imgViewColumnHelp.parent().outerWidth() - 58);
                        imgViewColumnHelp.css('top', imgViewColumnHelp.parent().outerHeight() - 88);
                        imgViewColumnHelp.mouseenter(function ()
                        {
                            viewHelp = $('#viewColumnHelpArea');
                            viewHelp.css('zIndex', SEL.CustomEntityAdministration.zIndices.Views.HelpIcon());
                            viewHelp.css('left', $(this).offset().left - viewHelp.outerWidth());
                            viewHelp.css('top', $(this).offset().top - viewHelp.outerHeight());
                            viewHelp.css('position', 'absolute');
                            viewHelp.stop(true, true).fadeIn(400);
                        }).mouseleave(function ()
                        {
                            $('#viewColumnHelpArea').stop(true, true).fadeOut(200);
                        });

                        _misc.LoadingScreenCancelled = true;

                        $('#loadingArea').fadeOut(600, function ()
                        {
                            $('#loadingArea').remove();
                        });
                    },

                    // hide the view modal
                    Hide: function ()
                    {
                        var viewModal = SEL.CustomEntityAdministration.DomIDs.Views.Modal;

                        $f(viewModal.TabContainer).set_activeTabIndex(0);
                        SEL.Common.HideModal(viewModal.Control);
                    },

                    // reset all fields and info in this modal todo fields/filters
                    Clear: function ()
                    {
                        var viewModal = SEL.CustomEntityAdministration.DomIDs.Views.Modal;

                        // General Tab
                        $g(viewModal.General.Name).value = '';
                        $g(viewModal.General.Description).value = '';
                        $('#' + viewModal.General.Menu).attr('data-val', 0);
                        $('#' + viewModal.General.Menu + ' span').text(SEL.CustomEntityAdministration.Messages.Views.MenuInstructions);
                        $('.clearMenu').hide();
                        $g(viewModal.General.MenuDescription).value = '';
                        $g(viewModal.General.ShowRecordCount).checked = false;
                        $("input[name=chkMenuDisabledModule]").prop("checked", true);
                        $("#" + viewModal.General.AddForm + " option").prop("disabled", false);
                        $g(viewModal.General.AddForm).selectedIndex = 0;
                        $("#" + viewModal.General.AddFormSelectionMappings).data("mappings", []);
                        $("#" + viewModal.General.EditForm + " option").prop("disabled", false);
                        $g(viewModal.General.EditForm).selectedIndex = 0;
                        $("#" + viewModal.General.EditFormSelectionMappings).data("mappings", []);
                        $g(viewModal.General.AllowApproval).checked = false;
                        $g(viewModal.General.AllowDelete).checked = false;
                        $g(viewModal.General.AllowArchive).checked = false;
                        $g(viewModal.General.BuiltIn).checked = false;
                        $("#" + viewModal.General.BuiltIn).prop("disabled", !CurrentUserInfo.AdminOverride);
                        SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.FormSelectionAttribute = null;

                        // Fields Tab
                        SEL.CustomEntityAdministration.Views.Fields.Tree.Refresh();
                        SEL.Trees.Tree.Clear(viewModal.Fields.Drop);

                        // Filters Tab
                        SEL.CustomEntityAdministration.Views.Filters.Tree.Refresh();
                        SEL.Trees.Tree.Clear(viewModal.Filters.Drop);

                        // Sort Tab
                        $f(viewModal.Sort.Tab).set_enabled(false);
                        $g(viewModal.Sort.Column).selectedIndex = 0;
                        $g(viewModal.Sort.Direction).selectedIndex = 0;

                        // Custom Icon Area
                        $('#' + viewModal.Icon.SearchFileName).val('Search...');
                        $('#viewIconResults').html('');

                        return;
                    }
                },

                // Adding a new view
                Add: function ()
                {
                    var ids = SEL.CustomEntityAdministration.IDs;
                    ids.View = 0;
                    ids.Sort.Column = null;
                    ids.Sort.Direction = null;


                    if (SEL.CustomEntityAdministration.IDs.Entity === 0)
                    {
                        // if the base entity hasn't been saved yet, we need to so that we have a valid entity id to attach to

                        currentAction = 'addView';

                        SEL.CustomEntityAdministration.Base.Save();

                        return;
                    }

                    SEL.CustomEntityAdministration.Views.Modal.Clear();

                    $('#' + SEL.CustomEntityAdministration.DomIDs.Views.Modal.PanelHeader).text('New View');

                    Spend_Management.svcCustomEntities.getViewForms(SEL.CustomEntityAdministration.IDs.Entity,
                        function (forms)
                        {
                            SEL.CustomEntityAdministration.Views.General.PopulateForms(forms);

                            SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.GetAttribute();

                            var iconUrl = StaticLibPath + '/icons/48/new/window_dialog.png';

                            SEL.CustomEntityAdministration.Views.Icon.SetSelectedIcon(iconUrl, 'window_dialog.png');

                            SEL.CustomEntityAdministration.Views.Icon.SearchFileName('', 0);

                            SEL.CustomEntityAdministration.Views.Modal.Show();
                        },
                        SEL.CustomEntityAdministration.Misc.ErrorHandler
                    );
                },

                // editing an existing view todo convert
                Edit: function (id)
                {
                    var _cea = SEL.CustomEntityAdministration,
                        _ids = _cea.IDs,
                        _doms = _cea.DomIDs,
                        _views = _cea.Views,
                        _domsViewModal = _doms.Views.Modal,
                        _misc = _cea.Misc;

                    _misc.LoadingScreenCancelled = false;

                    setTimeout(function () { _misc.ShowInformationMessage('Loading...'); }, 300);

                    _ids.View = id;

                    _views.Modal.Clear();

                    Spend_Management.svcCustomEntities.getView(cu.AccountID, _ids.Entity, _ids.View,
                        function (data)
                        {
                            var i, divMenu, cmbaddform, cmbeditform;
                            $('#' + _domsViewModal.PanelHeader).text('View: ' + data.viewName);
                            $g(_domsViewModal.General.Name).value = data.viewName;
                            $g(_domsViewModal.General.Description).value = data.description;
                             $g(_domsViewModal.General.ShowRecordCount).checked = data.showrecordcount;
                            $g(_domsViewModal.General.BuiltIn).checked = data.builtIn;
                            $("#" + _domsViewModal.General.BuiltIn).prop("disabled", data.builtIn || !CurrentUserInfo.AdminOverride);

                            _views.General.PopulateForms(data.formDropDownOptions);

                            _views.General.FormSelectionMappings.GetAttribute();

                            divMenu = _domsViewModal.General.Menu;

                            if (data.nMenuid === null) {
                                $("#" + divMenu + ' span').text(SEL.CustomEntityAdministration.Messages.Views.MenuInstructions);
                            } else {
                                var easytree = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0];
                                var nodes = easytree.getAllNodes();
                                var element = SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.GetNodeByInternalId(nodes, data.nMenuid);

                                $('#' + divMenu).attr('data-val', data.nMenuid);
                                $('#' + divMenu + ' span').text($('#' + element.id + ' .easytree-title').html());
                                $('.clearMenu').show();
                            }

                            $g(_domsViewModal.General.MenuDescription).value = data.MenuDescription;
                            $g(_domsViewModal.General.ShowRecordCount).checked = data.ShowRecordCount;
                            // uncheck any disabled modules for the view's menu item
                            $.each(data.MenuDisabledModuleIds, function (index, item) {
                                $("input[name=chkMenuDisabledModule][value=" + item + "]").prop("checked", false);
                            });

                            cmbaddform = $g(_domsViewModal.General.AddForm);
                            for (i = 0; i < cmbaddform.options.length; i = i + 1)
                            {
                                if (cmbaddform.options[i].value === data.addFormID.toString())
                                {
                                    cmbaddform.selectedIndex = i;
                                    break;
                                }
                            }
                            
                            if (data.addFormID > 0 && data.AddFormMappings !== null)
                            {
                                $("#" + _domsViewModal.General.AddFormSelectionMappings).data("mappings", data.AddFormMappings);
                            }

                            cmbeditform = $g(_domsViewModal.General.EditForm);
                            for (i = 0; i < cmbeditform.options.length; i = i + 1)
                            {
                                if (cmbeditform.options[i].value === data.editFormID.toString())
                                {
                                    cmbeditform.selectedIndex = i;
                                    break;
                                }
                            }

                            if (data.editFormID > 0 && data.EditFormMappings !== null)
                            {
                                $("#" + _domsViewModal.General.EditFormSelectionMappings).data("mappings", data.EditFormMappings);
                            }

                            $g(_domsViewModal.General.AllowDelete).checked = data.allowDelete;
                            $g(_domsViewModal.General.AllowApproval).checked = data.allowApproval;
                            $g(_domsViewModal.General.AllowArchive).checked = data.allowArchive;

                            _ids.Sort.Column = data.SortedColumnID;
                            _ids.Sort.Direction = data.SortOrderDirection;

                            $('#' + _domsViewModal.General.Name).select();

                            // Call the Fields refresh, which calls the Filters refresh and shows the modal
                            _views.Fields.Drop.Refresh();

                            SEL.CustomEntityAdministration.Views.Icon.SetSelectedIcon(data.MenuIcon.IconUrl, data.MenuIcon.IconName);

                            SEL.CustomEntityAdministration.Views.Icon.SearchFileName('', 0);
                        },
                        SEL.CustomEntityAdministration.Misc.ErrorHandler
                    );

                    return;
                },

                SaveWithModalCheck: function ()
                {
                    if (SEL.CustomEntityAdministration.Misc.LoadingScreenCancelled)
                    {
                        SEL.CustomEntityAdministration.Views.Save();
                    }
                },

                // save the view
                Save: function () {
                    var _misc = SEL.CustomEntityAdministration.Misc;

                    _misc.LoadingScreenCancelled = false;

                    SEL.CustomEntityAdministration.Views.Sort.Refresh();

                    if (validateform('vgView') === false)
                    {
                        return;
                    }

                    setTimeout(function () { _misc.ShowInformationMessage('Saving...'); }, 450);

                    var _ces = SEL.CustomEntityAdministration,
                        _domsViewModal = _ces.DomIDs.Views.Modal,
                        _ids = _ces.IDs,
                        viewname = $g(_domsViewModal.General.Name).value,
                        description = $g(_domsViewModal.General.Description).value,
                        builtIn = $g(_domsViewModal.General.BuiltIn).checked,
                        menu = $('#' + _domsViewModal.General.Menu).attr('data-val'),
                        menudescription = $g(_domsViewModal.General.MenuDescription).value,
                        showrecordcount = $g(_domsViewModal.General.ShowRecordCount).checked,
                        menuDisabledModuleIds = [],
                        ddlAddForm = $g(_domsViewModal.General.AddForm),
                        ddlEditForm = $g(_domsViewModal.General.EditForm),
                        addformid = 0,
                        editformid = 0,
                        allowdelete = $g(_domsViewModal.General.AllowDelete).checked,
                        allowapproval = $g(_domsViewModal.General.AllowApproval).checked,
                        allowarchive = $g(_domsViewModal.General.AllowArchive).checked,
                        fields = SEL.Trees.Tree.Data.Get(_domsViewModal.Fields.Drop),
                        filters = SEL.Trees.Tree.Data.Get(_domsViewModal.Filters.Drop, ['metadata']),
                        ddlSortColumn = $g(_domsViewModal.Sort.Column),
                        ddlSortDirection = $g(_domsViewModal.Sort.Direction),
                        selectedSortColumn = {
                            SortID: '00000000-0000-0000-0000-000000000000_0',
                            FieldID: '00000000-0000-0000-0000-000000000000',
                            JoinViaID: 0,
                            JoinViaPath: '',
                            JoinViaCrumbs: ''
                        },
                        selectedSortDirection = '0',
                        oSortColumnItem,
                        jqSortColumnItem,
                        addMappings,
                        editMappings;

                    $("input[name=chkMenuDisabledModule]:not(:checked)").each(function (index, item) {
                        menuDisabledModuleIds.push(+$(item).val());
                    });
                    
                    if (ddlAddForm.selectedIndex > 0)
                    {
                        addformid = ddlAddForm.options[ddlAddForm.selectedIndex].value;
                    }

                    if (ddlEditForm.selectedIndex > 0)
                    {
                        editformid = ddlEditForm.options[ddlEditForm.selectedIndex].value;
                    }

                    if (ddlSortColumn.selectedIndex !== -1)
                    {
                        oSortColumnItem = ddlSortColumn.options[ddlSortColumn.selectedIndex];
                        jqSortColumnItem = $(oSortColumnItem);
                        selectedSortColumn.SortID = oSortColumnItem.value;
                        selectedSortColumn.FieldID = jqSortColumnItem.attr('FieldID');
                        selectedSortColumn.JoinViaID = jqSortColumnItem.attr('JoinViaID');
                        selectedSortColumn.JoinViaPath = jqSortColumnItem.attr('JoinViaPath');
                        selectedSortColumn.JoinViaCrumbs = jqSortColumnItem.attr('JoinViaCrumbs');
                        selectedSortDirection = ddlSortDirection.options[ddlSortDirection.selectedIndex].value;
                    }

                    var viewIcon = null;

                    if ($('#selectedIconSpan .selectedIcon').length === 1)
                    {
                        viewIcon = $('#selectedIconSpan .selectedIcon').data('iconName');
                    }

                    addMappings = $("#" + _domsViewModal.General.AddFormSelectionMappings).data("mappings");
                    addMappings = addMappings === null ? [] : addMappings;
                    editMappings = $("#" + _domsViewModal.General.EditFormSelectionMappings).data("mappings");
                    editMappings = editMappings === null ? [] : editMappings;

                    Spend_Management.svcCustomEntities.SaveView(cu.AccountID, cu.EmployeeID, _ids.Entity, _ids.View, viewname, description, builtIn, menu, menudescription,showrecordcount ,fields, addformid, editformid, allowdelete, allowapproval,allowarchive, filters, selectedSortColumn, selectedSortDirection, viewIcon, addMappings, editMappings, menuDisabledModuleIds,
                        function (data) {
                            if (data === -1)
                            {
                                
                                _misc.LoadingScreenCancelled = true;
                                $('#loadingArea').remove();
                                SEL.MasterPopup.ShowMasterPopup(_ces.Messages.Views.DuplicateName);
                                return;
                            }

                            _ces.Views.Modal.Hide();

                            _misc.LoadingScreenCancelled = true;

                            $('#loadingArea').fadeOut(600, function ()
                            {
                                $('#loadingArea').remove();
                            });

                            // if the attribute is built-in/system the GreenLight will be too (if it wasn't already, it will be now), so update the checkbox for the GreenLight automatically.
                            if (builtIn && !$("#ctl00_contentmain_chkBuiltIn").is(":checked")) {
                                $("#ctl00_contentmain_chkBuiltIn").prop("checked", true).prop("disabled", true);
                            }

                            Spend_Management.svcCustomEntities.getViewGrid(_ids.Entity,
                                _ces.Views.Grid.Refresh,
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                            Spend_Management.svcCustomEntities.getPopupViews(_ids.Entity, _ids.View, cu.AccountID,
                               _ces.Base.RefreshPopupViewDDL,
                               SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );

                            _ces.Views.General.FormSelectionMappings.RefreshFormSelectionAttribute(_ids.Entity);
                        },
                        SEL.CustomEntityAdministration.Misc.ErrorHandler
                    );

                    return;
                },

                // deleting a view
                Delete: function (id, accountid, entityid) {
                    if (confirm(SEL.CustomEntityAdministration.Messages.Views.DeleteSure)) {
                        Spend_Management.svcCustomEntities.checkViewDoesNotBelongToPopupView(id, accountid,
                            function (data) {
                                if (data === 0)
                                    Spend_Management.svcCustomEntities.DeleteView(id,
                                        function (data) {
                                            if (data === 0) {
                                                Spend_Management.svcCustomEntities.getViewGrid(SEL.CustomEntityAdministration.IDs.Entity,
                                                    SEL.CustomEntityAdministration.Views.Grid.Refresh,
                                                    SEL.CustomEntityAdministration.Misc.ErrorHandler
                                                );
                                                Spend_Management.svcCustomEntities.getPopupViews(entityid, id, accountid,
                                                    SEL.CustomEntityAdministration.Base.RefreshPopupViewDDL,
                                                    SEL.CustomEntityAdministration.Misc.ErrorHandler
                                                );

                                                SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.RefreshFormSelectionAttribute(entityid);
                                            } else if (data === -2) {
                                                SEL.MasterPopup.ShowMasterPopup(SEL.CustomEntityAdministration.Messages.Views.DeleteBuiltIn);
                                            } else {
                                                SEL.MasterPopup.ShowMasterPopup(SEL.CustomEntityAdministration.Messages.Views.DeleteAbortInUse);
                                            }
                                        },
                                        SEL.CustomEntityAdministration.Misc.ErrorHandler

                                    );
                                else SEL.MasterPopup.ShowMasterPopup(SEL.CustomEntityAdministration.Messages.Views.DeleteAbortPopupViewInUse);
                            }
                        );
                    }
                },

                // methods relating to the general tab
                General:
                {
                    PopulateForms: function (forms)
                    {
                        var i,
                            viewModal = SEL.CustomEntityAdministration.DomIDs.Views.Modal,
                            ddlAddForm = $g(viewModal.General.AddForm),
                            ddlEditForm = $g(viewModal.General.EditForm),
                            ddlMappingForms = $("select.mapping-forms"),
                            newOpt1,
                            newOpt2,
                            newOpt3 = "";

                        ddlAddForm.options.length = 0;
                        ddlEditForm.options.length = 0;
                        ddlMappingForms.empty();
                        
                        var formObj = {};

                        for (i = 0; i < forms.length; i = i + 1)
                        {
                            newOpt1 = document.createElement('option');
                            newOpt1.value = forms[i].Value;
                            newOpt1.text = forms[i].Text;

                            newOpt2 = document.createElement('option');
                            newOpt2.value = forms[i].Value;
                            newOpt2.text = forms[i].Text;

                            if (forms[i].Value !== "0" && forms[i].Value !== 0)
                            {
                                newOpt3 += "<option value=\"" + forms[i].Value + "\">" + forms[i].Text + "</option>";
                            }
                            
                            formObj[forms[i].Value] = forms[i].Text;
                            
                            ddlAddForm.add(newOpt1);
                            ddlEditForm.add(newOpt2);
                        }
                        
                        ddlMappingForms.append(newOpt3);
                        this.FormSelectionMappings.Forms = formObj;

                        return;
                    },
                    
                    FormSelectionMappings:
                    {
                        IsAdd: null,

                        Forms: null,
                        
                        FormSelectionAttribute: null,

                        FormSelectionAttributeListItems: null,

                        RefreshFormSelectionAttribute: function(entityId) {
                            SEL.Data.Ajax({
                                serviceName: "svcCustomEntities",
                                methodName: "HasFormSelectionMappings",
                                data: {
                                    entityId: entityId
                                },
                                success: function (response) {
                                    if ("d" in response) {
                                        $("#" + SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute).prop("disabled", response.d);
                                    }
                                }
                            });
                        },
                        
                        Show: function(control)
                        {
                            var ns = SEL.CustomEntityAdministration,
                                dom = ns.DomIDs.Views.Modal.General,
                                fsm = ns.Views.General.FormSelectionMappings,
                                mappings;

                            if (this.FormSelectionAttribute === null) {
                                SEL.MasterPopup.ShowMasterPopup("A Form selection attribute must be defined before any form selection mappings can be created.<br />You can define a Form selection attribute on the Greenlight Details page.");
                                return;
                            }

                            if ($(control).data("isadd") && ($("#" + dom.AddForm).val() === 0 || $("#" + dom.AddForm).val() === "0"))
                            {
                                SEL.MasterPopup.ShowMasterPopup("A Default add form must be selected before form selection mappings can be created.");
                                return;
                            }
                            
                            if ($(control).data("isadd") === false && ($("#" + dom.EditForm).val() === 0 || $("#" + dom.EditForm).val() === "0"))
                            {
                                SEL.MasterPopup.ShowMasterPopup("A Default edit form must be selected before form selection mappings can be created.");
                                return;
                            }
                            
                            fsm.Clear();

                            fsm.IsAdd = $(control).data("isadd");
                            fsm.SetTitle((fsm.IsAdd ? "New Add Form Selection" : "New Edit Form Selection"));
                            
                            mappings = fsm.GetMappings(control);
                            
                            fsm.AddRows(mappings);
                            
                            if (this.FormSelectionAttribute.attributeType === "list")
                            {
                                $(".mapping-listrow").show();
                                $(".mapping-textrow").hide();
                            }
                            else
                            {
                                $(".mapping-listrow").hide();
                                $(".mapping-textrow").show();
                            }

                            $f(ns.DomIDs.Views.Modal.General.FormSelectionMappings.Modal).show();
                            return;
                        },
                        
                        Save: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                fsm = ns.Views.General.FormSelectionMappings,
                                mappings = fsm.GetRows();

                            fsm.SetMappings(mappings);

                            $f(ns.DomIDs.Views.Modal.General.FormSelectionMappings.Modal).hide();
                        },
                        
                        Cancel: function ()
                        {
                            var ns = SEL.CustomEntityAdministration;
                            $f(ns.DomIDs.Views.Modal.General.FormSelectionMappings.Modal).hide();
                        },
                        
                        Clear: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                dom = ns.DomIDs.Views.Modal.General.FormSelectionMappings;
                            
                            $("#" + dom.Panel + " .mapping-title").empty();
                            $("#" + dom.Panel + " .mapping-text").val("");
                            $("#" + dom.Panel + " .mapping-list option").prop({ "disabled": false, "selected": false });
                            $("#" + dom.Panel + " .mapping-list option:eq(0), #" + dom.Panel + " .mapping-forms option:eq(0)").prop("selected", true);
                            $("#" + dom.Panel + " .mapping-tbody").empty();
                            $("#" + dom.Panel + " .mapping-empty").show();
                        },
                        
                        SetTitle: function (titleText)
                        {
                            $("#" + SEL.CustomEntityAdministration.DomIDs.Views.Modal.General.FormSelectionMappings.Header).text(titleText);
                        },

                        New: function (control)
                        {
                            var ns = SEL.CustomEntityAdministration.Views.General.FormSelectionMappings,
                                lv = $(".mapping-list").val(),
                                formMappingList = $(control).closest("tr").find(".mapping-forms"),
                                rows = ns.FormatRow(0, (ns.FormSelectionAttribute.attributeType === "list"), lv, $(".mapping-text").val(), formMappingList.val());

                            if (ns.FormSelectionAttribute.attributeType === "text") {
                                var isDuplicateTextValue = false;

                                $(".mapping-tbody tr").each(function (index, element) {
                                    element = $(element).children("td:first").children("span");

                                    if ($(".mapping-text").val().toLowerCase() === $(element).html().toLowerCase()) {
                                        isDuplicateTextValue = true;
                                        return;
                                    }
                                });

                                if (isDuplicateTextValue) {
                                    SEL.MasterPopup.ShowMasterPopup("The Form Selection Attribute Value could not be added because it is not unique.");
                                    return;
                                }

                                $(".mapping-text").val("");
                            }

                            if ($(".mapping-list option:selected").prop("disabled"))
                            {
                                return;
                            }

                            $(".mapping-tbody").append(rows);
                            
                            if (!isNaN(parseInt(lv, 10)) && parseInt(lv, 10) > 0)
                            {
                                $(".mapping-list option[value=" + lv + "]").prop("disabled", true);
                                $(".mapping-list option:enabled:first").prop("selected", true);
                            }

                            formMappingList.val(formMappingList.children("option:first").val());

                            ns.RefreshMappingRows();
                        },
                        
                        RefreshMappingRows: function ()
                        {
                            if ($(".mapping-tbody tr").length > 0)
                            {
                                $(".mapping-empty").hide();

                                $(".mapping-tbody tr:even td").addClass("row1");
                                $(".mapping-tbody tr:odd td").addClass("row2");
                            }
                            else
                            {
                                $(".mapping-empty").show();
                            }
                        },
                        
                        GetAttribute: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                fsm = ns.Views.General.FormSelectionMappings,
                                attributeId = $("#" + ns.DomIDs.Base.FormSelectionAttribute).val(),
                                itemString = "",
                                val = 0;

                            if (!isNaN(parseInt(attributeId)) && parseInt(attributeId, 10) > 0)
                            {
                                SEL.Data.Ajax({
                                    data: { "entityId": ns.IDs.Entity, "attributeId": parseInt(attributeId, 10) },
                                    serviceName: "svcCustomEntities",
                                    methodName: "GetFormSelectionAttribute",
                                    success: function(r)
                                    {
                                        if ("d" in r)
                                        {
                                            fsm.FormSelectionAttribute = r.d;

                                            if (r.d.attributeType === "list") 
                                            {
                                                $(".mapping-list").empty();
                                                fsm.FormSelectionAttributeListItems = {};

                                                $(r.d.items).each(function (i, o)
                                                {
                                                    val = parseInt(o.Value, 10);
                                                    if (!isNaN(val))
                                                    {
                                                        fsm.FormSelectionAttributeListItems[val] = o.Text;
                                                    }

                                                    itemString += "<option value= \"" + o.Value + "\">" + o.Text + "</option>";
                                                });

                                                $(".mapping-list").append(itemString);
                                            } 
                                            else 
                                            {
                                                $(".mapping-text").attr("maxlength", r.d.maxLength || 500);
                                            }
                                        }
                                    }
                                });
                            }
                        },
                        
                        GetMappings: function (control)
                        {
                            if (typeof control !== "object")
                            {
                                return [];
                            }
                            
                            var mappings = $(control).data("mappings");

                            if (mappings === null)
                            {
                                return [];
                            }

                            return mappings;
                        },

                        SetMappings: function (mappings)
                        {
                            if (mappings === null)
                            {
                                mappings = [];
                            }
                            
                            var ns = SEL.CustomEntityAdministration,
                                dom = ns.DomIDs.Views.Modal.General;
                            
                            if (ns.Views.General.FormSelectionMappings.IsAdd)
                            {
                                $("#" + dom.AddFormSelectionMappings).data("mappings", mappings);
                                
                                if (mappings.length > 0)
                                {
                                    $("#" + dom.AddForm + " option[value=0]").prop("disabled", true);
                                }
                                else
                                {
                                    $("#" + dom.AddForm + " option[value=0]").prop("disabled", false);
                                }
                            }
                            else
                            {
                                $("#" + dom.EditFormSelectionMappings).data("mappings", mappings);
                                
                                if (mappings.length > 0)
                                {
                                    $("#" + dom.EditForm + " option[value=0]").prop("disabled", true);
                                }
                                else
                                {
                                    $("#" + dom.EditForm + " option[value=0]").prop("disabled", false);
                                }
                            }
                        },
                        
                        AddRows: function (mappings)
                        {
                            if ("length" in mappings && mappings.length > 0)
                            {
                                var ns = SEL.CustomEntityAdministration.Views.General.FormSelectionMappings,
                                    rows = "",
                                    formatRow = ns.FormatRow;

                                $(mappings).each(function (i, o)
                                {
                                    rows += formatRow(o.FormSelectionMappingId, (o.TextValue === null), o.ListValue, o.TextValue, o.FormId);

                                    if (!isNaN(parseInt(o.ListValue, 10)) && parseInt(o.ListValue, 10) > 0)
                                    {
                                        $(".mapping-list option[value=" + o.ListValue + "]").prop("disabled", true);
                                    }
                                });
                                
                                $(".mapping-tbody").append(rows);

                                ns.RefreshMappingRows();
                            }
                        },
                        
                        FormatRow: function (mappingId, isList, listVal, textVal, formId)
                        {
                            var ns = SEL.CustomEntityAdministration.Views.General.FormSelectionMappings,
                                listValues = ns.FormSelectionAttributeListItems,
                                enc = ns.EncodeValueText,
                                forms = ns.Forms;
                            
                            return "<tr data-mappingid=\"" + mappingId + "\"><td>" + (isList ? "<span class=\"mapping-listvalue\">" + listVal + "</span>" + enc(listValues[listVal]) : "<span class=\"mapping-textvalue\">" + enc(textVal)) + "</span></td><td><span class=\"mapping-formid\">" + formId + "</span>" + enc(forms[formId]) + "</td><td class=\"cgridnew-icon\"><img class=\"btn mapping-deletebutton\" src=\"/static/icons/16/plain/delete.png\" alt=\"delete form mapping\" onclick=\"SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Delete(this);\" /></td></tr>";
                        },
                        
                        GetRows: function ()
                        {
                            var ns = SEL.CustomEntityAdministration,
                                fsm = ns.Views.General.FormSelectionMappings,
                                mappings = [],
                                listValue,
                                textValue;

                            $(".mapping-tbody tr").each(function (i, o)
                            {
                                o = $(o);
                                
                                if (isNaN(parseInt(o.find(".mapping-formid").text(), 10)))
                                {
                                    return;
                                }        
                                
                                if (fsm.FormSelectionAttribute.attributeType === "text")
                                {
                                    listValue = -1;
                                    textValue = fsm.DecodeValueText(o.find(".mapping-textvalue").text());
                                }
                                else
                                {
                                    listValue = isNaN(parseInt(o.find(".mapping-listvalue").text(), 10)) ? 0 : parseInt(o.find(".mapping-listvalue").text(), 10);
                                    textValue = null;
                                }

                                mappings.push({
                                    FormSelectionMappingId: o.data("mappingid"),
                                    ViewId: ns.IDs.View,
                                    IsAdd: fsm.IsAdd,
                                    FormId: parseInt(o.find(".mapping-formid").text(), 10),
                                    ListValue: listValue,
                                    TextValue: textValue
                                });
                            });

                            return mappings;
                        },
                        
                        Delete: function (control)
                        {
                            var lv,
                                lvSpan = $(control).closest("tr").find(".mapping-listvalue");
                            
                            if (lvSpan !== null && lvSpan.length > 0)
                            {
                                lv = parseInt(lvSpan.text(), 10);
                                if (!isNaN(lv) && lv > 0)
                                {
                                    $(".mapping-list option[value=" + lv + "]").prop("disabled", false);
                                }
                            }
                            
                            $(control).closest("tr").remove();

                            SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.RefreshMappingRows();
                        },
                        
                        EncodeValueText: function (text)
                        {
                            return (typeof text === "string") ? text.replace("&", "&#38;").replace("<", "&#60;") : text;
                        },

                        DecodeValueText: function (text)
                        {
                            return (typeof text === "string") ? text.replace("&#60;", "<").replace("&#38;", "&") : text;
                        }
                    },

                    CustomMenuStructure:
                    {
                            SelectNode: function (nodes, id) {
                                    var i = 0;
                                    var easytree = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0];
                                    for (i = 0; i < nodes.length; i++) {
                                        var currentNode = nodes[i];
                                        if (currentNode.id === id) {
                                            var requiredNodeBreadcrumbs = currentNode.crumbs;
                                            var breadcrumbArray = requiredNodeBreadcrumbs.split(':');
                                        var d = 0;
                                        for (d = 1; d < breadcrumbArray.length; d++) {
                                            var parentNodeId = $('span.easytree-title:contains(' + breadcrumbArray[d] + ')').parent().attr('id');
                                            var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode(parentNodeId);
                                            node.isExpanded = true;
                                            }
                                            currentNode.isExpanded = false;
                                            easytree.activateNode(id);
                                        }

                                        var hasChildren = currentNode.children && currentNode.children.length > 0;
                                        if (hasChildren) {
                                            this.SelectNode(currentNode.children, id);
                                        }
                                    }
                                    easytree.rebuildTree();
                            },

                            CloseNodes: function(nodes) {
                                var i = 0;
                                for(i = 0; i < nodes.length; i++) {
                                    var currentNode = nodes[i];
                                    currentNode.isExpanded = false;
                                    var hasChildren = currentNode.children && currentNode.children.length > 0;
                                    if (hasChildren) {
                                        this.CloseNodes(currentNode.children);
                                    }
                                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                }
                            },

                            DeactivateNodes: function(nodes) {
                                var i = 0;
                                for (i = 0; i < nodes.length; i++) {
                                    var currentNode = nodes[i];
                                    currentNode.isActive = false;
                                    $('#' + currentNode.id).removeClass('easytree-active');
                                    var hasChildren = currentNode.children && currentNode.children.length > 0;
                                    if (hasChildren) {
                                        this.DeactivateNodes(currentNode.children);
                                    }
                                    }
                            },

                            GetNodeByInternalId: function(nodes, internalId) {
                                    var i = 0;
                                    for (i = 0; i < nodes.length; i++) {

                                        var currentNode = nodes[i];
                                        if (currentNode.internalId == internalId) {
                                            return currentNode;
                                        }
                                        var hasChildren = currentNode.children && currentNode.children.length > 0;
                                        if(hasChildren) {
                                            var node = this.GetNodeByInternalId(currentNode.children, internalId);
                                            if (node) {
                                                return node;
                                            }
                                        }
                                     }

                                    return null;

                            },

                            Show: function () {
                                var ns = SEL.CustomEntityAdministration.DomIDs.Views.Modal.General;
                                var internalId = $('#' + ns.Menu).attr('data-val');
                                var easytree = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0];
                                var nodes = easytree.getAllNodes();
                                if (internalId != 0) {
                                    var node = this.GetNodeByInternalId(nodes, internalId);
                                    this.SelectNode(nodes, node.id);
                                }
                                else {
                                    this.CloseNodes(nodes);
                                }

                                $f(ns.CustomMenuStructure.Modal).show();
                            },

                            Save: function () {
                                var ns = SEL.CustomEntityAdministration.DomIDs.Views.Modal.General;
                                var id = $('.easytree-active').attr('id');
                                var node = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode(id);
                                if (node != null) {
                                    $('#' + ns.Menu).attr('data-val', node.internalId);
                                    $('#' + ns.Menu + ' span').text($('.easytree-active .easytree-title').html());
                                    $('.clearMenu').show();
                                    $f(ns.CustomMenuStructure.Modal).hide();
                                } else {
                                    SEL.MasterPopup.ShowMasterPopup("Please select a menu.");
                                }
                            },

                            Cancel: function () {
                                var ns = SEL.CustomEntityAdministration.DomIDs.Views.Modal.General;
                                $f(ns.CustomMenuStructure.Modal).hide();
                            },

                            PageLoad: function() {
                                var ns = SEL.CustomEntityAdministration.DomIDs.Views.Modal.General;
                                SEL.CustomMenuStructure.Tree.CreateEasyTree('menuTree', $('#' + ns.CustomMenuStructure.MenuTreeData).val());
                            },

                            ClearMenu: function () {

                                var ns = SEL.CustomEntityAdministration.DomIDs.Views.Modal.General;
                                $('#' + ns.Menu).attr('data-val', 0);
                                $('#' + ns.Menu + ' span').text(SEL.CustomEntityAdministration.Messages.Views.MenuInstructions);
                               $('.clearMenu').hide();

                                var easytree = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0];
                                var nodes = easytree.getAllNodes();
                                this.DeactivateNodes(nodes);
                            }
                  }
                },

                Tabs: {
                    TabChange: function (sender, args)
                    {
                        $('#imgViewFilterHelp').css('display', 'none');
                        $('#imgViewColumnHelp').css('display', 'none');
                        $('#filtermetadata').css('display', 'none');
                        switch (sender.get_activeTabIndex())
                        {
                            case 1:
                                $('#imgViewColumnHelp').css('display', 'inline-block');
                                break;
                            case 2:
                                $('#imgViewFilterHelp').css('display', 'inline-block');
                                $('#filtermetadata').css('display', '');
                                break;
                            case 3:
                                SEL.CustomEntityAdministration.Views.Icon.SetupIconSearch();
                                break;
                            case 4:
                                SEL.CustomEntityAdministration.Views.Sort.Refresh();
                                break;
                            default:
                                break;
                        }
                    }
                },

                Fields: {
                    Tree: {
                        Refresh: function ()
                        {
                            Spend_Management.svcCustomEntities.GetInitialTreeNodes(SEL.CustomEntityAdministration.IDs.Entity,
                                function (r)
                                {
                                    var treeID = SEL.CustomEntityAdministration.DomIDs.Views.Modal.Fields.Tree;

                                    SEL.Trees.Tree.Clear(treeID);

                                    if (r !== null)
                                    {
                                        SEL.Trees.Tree.Data.Set(treeID, r);
                                    }
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        }
                    },

                    Drop: {
                        Refresh: function ()
                        {
                            var _ids = SEL.CustomEntityAdministration.IDs,
                                thisNs = SEL.CustomEntityAdministration.DomIDs.Views.Modal.Fields;

                            Spend_Management.svcCustomEntities.GetSelectedNodes(_ids.Entity, _ids.View,
                                function (r)
                                {
                                    SEL.Trees.Tree.Clear(thisNs.Drop);

                                    if (r !== null)
                                    {
                                        SEL.Trees.Tree.Data.Set(thisNs.Drop, r);
                                    }

                                    SEL.Trees.Node.Disable(thisNs.Tree, null, 'refresh');
                                    SEL.CustomEntityAdministration.Views.Sort.Tab.Refresh();

                                    SEL.CustomEntityAdministration.Views.Filters.Drop.Refresh();
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        }
                    }
                },

                Filters: {
                    Tree: {
                        Refresh: function ()
                        {
                            Spend_Management.svcCustomEntities.GetInitialTreeNodes(SEL.CustomEntityAdministration.IDs.Entity,
                                function (r)
                                {
                                    var treeID = SEL.CustomEntityAdministration.DomIDs.Views.Modal.Filters.Tree;

                                    SEL.Trees.Tree.Clear(treeID);

                                    if (r !== null)
                                    {
                                        SEL.Trees.Tree.Data.Set(treeID, r);
                                    }
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        }
                    },

                    Drop: {
                        Refresh: function ()
                        {
                            Spend_Management.svcCustomEntities.GetSelectedFilterNodes(SEL.CustomEntityAdministration.IDs.Entity, SEL.CustomEntityAdministration.IDs.View,
                                function (r)
                                {
                                    var dropID = SEL.CustomEntityAdministration.DomIDs.Views.Modal.Filters.Drop;

                                    SEL.Trees.Tree.Clear(dropID);

                                    if (r !== null)
                                    {
                                        SEL.Trees.Tree.Data.Set(dropID, r);
                                    }

                                    SEL.CustomEntityAdministration.Views.Modal.Show();
                                    SEL.CustomEntityAdministration.Misc.LoadingScreenCancelled = true;

                                    $('#loadingArea').fadeOut(600, function ()
                                    {
                                        $('#loadingArea').remove();
                                    });
                                },
                                SEL.CustomEntityAdministration.Misc.ErrorHandler
                            );
                        }
                    }
                },
                
                Sort: {
                    Tab: {
                        Refresh: function ()
                        {
                            var doms = SEL.CustomEntityAdministration.DomIDs.Views.Modal,
                                ids = SEL.CustomEntityAdministration.IDs.Sort;

                            if (SEL.Trees.Tree.NodeCount(doms.Fields.Drop) > 0)
                            {
                                $f(doms.Sort.Tab).set_enabled(true);
                            }
                            else
                            {
                                $f(doms.Sort.Tab).set_enabled(false);
                                ids.Column = null;
                                ids.Direction = null;
                            }
                        }
                    },

                    Refresh: function ()
                    {
                        var thisNs = SEL.CustomEntityAdministration.Views.Sort,
                            doms = SEL.CustomEntityAdministration.DomIDs.Views.Modal,
                            rawItems = SEL.Trees.Tree.Data.Get(doms.Fields.Drop),
                            selectedColumnID = SEL.CustomEntityAdministration.IDs.Sort.Column,
                            selectedDirection = SEL.CustomEntityAdministration.IDs.Sort.Direction,
                            oSortColumns = $g(doms.Sort.Column),
                            option = document.createElement('option'),
                            localizer = SEL.CustomEntityAdministration.Messages;

                        // Populate the sorted column drop down list from the selected fields
                        oSortColumns.options.length = 0;
                        option.value = '00000000-0000-0000-0000-000000000000_0';
                        option.text = localizer.NoneOption;
                        oSortColumns.options[0] = option;

                        $(rawItems.data).each(function (i, o)
                        {
                            option = document.createElement('option');
                            if (typeof o.data === 'object')
                            {
                                option.text = o.attr.crumbs === undefined ? o.data.title : o.data.title + ' [' + o.attr.crumbs + ']';
                            }
                            else
                            {
                                option.text = o.attr.crumbs === undefined ? o.data : o.data + ' [' + o.attr.crumbs + ']';
                            }
                            option.value = (o.attr.joinviaid > 0) ? o.attr.fieldID + '_' + o.attr.joinviaid : option.value = o.attr.id + '_' + o.attr.joinviaid;

                            $(option).attr({
                                FieldID: o.attr.fieldID,
                                JoinViaID: o.attr.joinviaid,
                                JoinViaPath: o.attr.id,
                                JoinViaCrumbs: (o.attr.crumbs === undefined ? '' : o.attr.crumbs)
                            });

                            try { oSortColumns.add(option, oSortColumns.options[null]); } /* for IE earlier than version 8 */
                            catch (e) { oSortColumns.add(option, null); }
                        });

                        if (selectedColumnID !== null && $('#' + doms.Sort.Column + ' option[value=' + selectedColumnID + ']').length > 0)
                        {
                            $('#' + doms.Sort.Column).val(selectedColumnID);
                        }
                        if (selectedDirection !== null && oSortColumns.selectedIndex > 0)
                        {
                            $('#' + doms.Sort.Direction).val(selectedDirection);
                        }

                        thisNs.ChangeColumnSortOrderState();
                        thisNs.Tab.Refresh();
                    },

                    ChangeColumnSortOrderState: function ()
                    {
                        var viewModal = SEL.CustomEntityAdministration.DomIDs.Views.Modal,
                            ddlSC = $g(viewModal.Sort.Column),
                            ddlSD = $g(viewModal.Sort.Direction),
                            ddlSDLabel = $(ddlSD).parent().prevAll('label'),
                            ddlSOV = $g(viewModal.Sort.OrderValidator);

                        SEL.CustomEntityAdministration.IDs.Sort.Column = ddlSC.selectedIndex > 0 ? ddlSC.options[ddlSC.selectedIndex].value : null;

                        if (ddlSC.selectedIndex > 0)
                        {
                            ddlSD.disabled = false;
                            ddlSDLabel.addClass('mandatory');
                            ddlSDLabel.text(ddlSDLabel.text().replace('*', '') + '*');
                            ValidatorEnable(ddlSOV, true);
                        }
                        else
                        {
                            ddlSD.selectedIndex = 0;
                            ddlSD.disabled = true;
                            ddlSDLabel.removeClass('mandatory');
                            ddlSDLabel.text(ddlSDLabel.text().replace('*', ''));
                            ValidatorEnable(ddlSOV, false);
                        }

                        ddlSOV.isvalid = true;
                        ValidatorUpdateDisplay(ddlSOV);
                    },

                    ChangeDirectionState: function ()
                    {
                        SEL.CustomEntityAdministration.IDs.Sort.Direction = $ddlValue(SEL.CustomEntityAdministration.DomIDs.Views.Modal.Sort.Direction);
                    }
                },
                
                Icon: {
                    SetupIconSearch: function ()
                    {
                        var thisNs = SEL.CustomEntityAdministration.Views.Icon;
                        var searchName = $('#' + SEL.CustomEntityAdministration.DomIDs.Views.Modal.Icon.SearchFileName);
                        var searchButton = $('#iconSearchButton');
                        var clearSearchButton = $('#iconSearchRemoveButton');

                        clearSearchButton.unbind('click').click(function ()
                        {
                            searchName.val('Search...');
                            thisNs.SearchFileName('', 0);
                            clearSearchButton.stop(true, true).fadeOut(200);
                        });

                        searchButton.unbind('click').click(function ()
                        {
                            if (searchName.val() !== 'Search...' && searchName.val() !== '')
                            {
                                thisNs.SearchFileName(searchName.val(), 0);
                            }
                        });

                        searchName.unbind('click').click(function ()
                        {
                            if (searchName.val() === 'Search...')
                            {
                                searchName.val('');
                            }
                        });

                        searchName.unbind('keypress.iconSearch').bind('keypress.iconSearch', function (e)
                        {
                            if (e.which == 13) //Enter keycode
                            {
                                // Prevent the Enter key from performing any default action
                                e.preventDefault();

                                thisNs.SearchFileName(searchName.val(), 0);

                                if ($.trim(searchName.val()) === '')
                                {
                                    clearSearchButton.stop(true, true).fadeOut(200);
                                }
                                else
                                {
                                    clearSearchButton.stop(true, true).fadeIn(200);
                                }
                            }
                        });

                        searchName.unbind('blur').blur(function ()
                        {
                            if ($.trim(searchName.val()) === '')
                            {
                                searchName.val('Search...');
                                clearSearchButton.stop(true, true).fadeOut(200);
                            }
                            else
                            {
                                clearSearchButton.stop(true, true).fadeIn(200);
                            }
                        });

                        $('#iconResultsLeft, #iconResultsRight').disableSelection().unbind('click').click(function ()
                        {
                            if ($(this).hasClass('active') && $(this).data('fileName') !== undefined && $(this).data('startFrom') !== undefined)
                            {
                                thisNs.SearchFileName($(this).data('fileName'), $(this).data('startFrom'));
                            }
                        });

                        $('#iconResultsLeft, #iconResultsRight').unbind('hover').hover(function ()
                        {
                            if ($(this).hasClass('active'))
                            {
                                $(this).stop(true, false).animate({ 'font-size': '38pt' }, 200).data('large', true);
                            }
                        }, function ()
                        {
                            if ($(this).hasClass('active'))
                            {
                                $(this).stop(true, false).animate({ 'font-size': '28pt' }, 200).data('large', false);
                            }
                            else
                            {
                                $(this).stop(true, false).animate({ 'font-size': '20pt' }, 200).data('large', false);
                            }
                        });
                    },

                    SearchFileName: function (fileName, startFrom)
                    {
                        $.ajax({
                            type: "POST",
                            url: window.appPath + "/shared/webServices/svcCustomEntities.asmx/SearchStaticIconsByFileName",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            data: '{ "fileName":"' + fileName.replace(/\\/g, '').replace(/\"/g, '') + '", "searchStartNumber":"' + startFrom + '" }',
                            success: function (data)
                            {
                                var thisNs = SEL.CustomEntityAdministration.Views.Icon;
                                var resultsArea = $('#viewIconResults');
                                var newResults = $(document.createElement('span')).css('display', 'none');

                                resultsArea.fadeOut(200);

                                thisNs.Refresh(fileName, data.d.FurtherResults, startFrom, data.d.ResultEndNumber);

                                if (data.d.MenuIcons.length < 11)
                                {
                                    newResults.css('margin-top', '90px');
                                }
                                else if (data.d.MenuIcons.length < 21)
                                {
                                    newResults.css('margin-top', '55px');
                                }
                                else
                                {
                                    newResults.css('margin-top', '15px');
                                }

                                var icons = data.d.MenuIcons;

                                // The following is done in native js to try and keep IE running as fast as possible
                                for (var i = 0, len = icons.length; i < len; i++)
                                {
                                    var img = document.createElement('img');

                                    img.setAttribute('src', icons[i].IconUrl);
                                    img.setAttribute('class', 'viewPreviewIcon');
                                    img.setAttribute('className', 'viewPreviewIcon');

                                    $(img).data('iconName', icons[i].IconName);

                                    var imgSpan = document.createElement('span');

                                    imgSpan.setAttribute('class', 'iconContainer');
                                    imgSpan.setAttribute('className', 'iconContainer');

                                    imgSpan.appendChild(img);

                                    newResults.append(imgSpan);
                                }

                                resultsArea.promise().done(function ()
                                {
                                    $('#viewIconResults').remove();
                                    newResults.attr('id', 'viewIconResults').appendTo($('#viewCustomIconContainer')).fadeIn(200, function ()
                                    {
                                        thisNs.SetupPreviewIcons();
                                    });
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown)
                            {
                                SEL.Common.WebService.ErrorHandler(errorThrown);
                            }
                        });
                    },

                    SetupPreviewIcons: function ()
                    {
                        var previewIcons = $('#viewIconResults .viewPreviewIcon');

                        previewIcons.unbind('hover').hover(function ()
                        {
                            $(this).stop(true, true).animate(
                                {
                                    'margin-top': '-8px',
                                    'margin-left': '-8px',
                                    'height': '64px',
                                    'width': '64px'
                                }, 200);
                        },
                            function ()
                            {
                                $(this).stop(true, false).animate(
                                    {
                                        'margin-top': '0px',
                                        'margin-left': '0px',
                                        'height': '48px',
                                        'width': '48px'
                                    }, 200);
                            });

                        previewIcons.unbind('click').click(function ()
                        {
                            var selectedIconArea = $('#selectedIconSpan');
                            var currentIcon = $('#selectedIconSpan .selectedIcon');

                            if (currentIcon.length === 1)
                            {
                                var iconName = $(this).data('iconName');
                                var iconNameSpan = $('#selectedIconName');

                                iconNameSpan.stop(true, true).fadeOut(150, function ()
                                {
                                    iconNameSpan.html(iconName);
                                    iconNameSpan.fadeIn(150);
                                });

                                var menuIcon = $('<img></img>').attr('src', $(this).attr('src')).addClass('selectedIcon');

                                menuIcon.css('display', 'none').data('iconName', iconName);

                                currentIcon.css('position', 'absolute');

                                selectedIconArea.append(menuIcon);

                                currentIcon.fadeOut(500, function ()
                                {
                                    currentIcon.remove();
                                });

                                menuIcon.fadeIn(500);
                            }
                        });
                    },

                    Refresh: function (fileName, furtherResults, resultStartNumber, resultEndNumber)
                    {
                        var resultsRight = $('#iconResultsRight');
                        var resultsLeft = $('#iconResultsLeft');

                        if (furtherResults)
                        {
                            if (resultsRight.data('large') !== true)
                            {
                                resultsRight.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                            }
                        }
                        else
                        {
                            resultsRight.removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
                        }

                        if (resultEndNumber > 30)
                        {
                            if (resultsLeft.data('large') !== true)
                            {
                                resultsLeft.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                            }
                        }
                        else
                        {
                            $('#iconResultsLeft').removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
                        }

                        resultsRight.data('startFrom', resultEndNumber);
                        resultsLeft.data('startFrom', resultStartNumber - 30);
                        $('#iconResultsRight, #iconResultsLeft').data('fileName', fileName);
                    },

                    SetSelectedIcon: function (iconUrl, iconName)
                    {
                        var menuIcon = $('<img></img').attr('src', iconUrl).addClass('selectedIcon');

                        menuIcon.data('iconName', iconName);

                        $('#selectedIconSpan').html('').append(menuIcon);

                        $('#selectedIconName').html(iconName);
                    }
                }
            }
        };
    }
    $.fn.selectRange = function (start, end) {
        $(this).each(function () {
            var el = $(this)[0];
            if (el) {
                el.focus();

                if (el.setSelectionRange) {
                    el.setSelectionRange(start, end);

                } else if (el.createTextRange) {
                    var range = el.createTextRange();
                    range.collapse(true);
                    range.moveEnd("]", end);
                    range.moveStart("[", start);
                    range.select();

                } else if (el.selectionStart) {
                    el.selectionStart = start;
                    el.selectionEnd = end;
                }
            }
        });
    };



    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }

}(SEL, jQuery, $g, $f, $e, $ddlValue, $ddlText, $ddlSetSelected, CurrentUserInfo, ValidatorEnable, ValidatorUpdateDisplay, moduleNameHTML));

