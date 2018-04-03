(function (SEL, moduleNameHTML, appPath) {
    var scriptName = "ItemRoles";

    function execute() {
        SEL.registerNamespace("SEL.ItemRoles");
        SEL.ItemRoles =
            {
                IDs: {
                    itemRoleId: 0,
                    roleSubcatId: 0,
                    subcatId: 0
                },

                DomIDs:
                {
                    Modal: null,
                    ItemRole: {
                        roleName: null,
                        description: null
                    },
                    AssociatedExpenseItem: {
                        expenseItem: null,
                        limitWithoutReceipt: null,
                        limitWithReceipt: null,
                        addToTemplate: null
                    }
                },
                ShowAssociatedExpenseItemModal: false,
                SetupAssociatedExpenseItemDialog: function () {
                    SEL.ItemRoles.DomIDs.Modal = $("#LinkExpenseItemModal");
                    SEL.ItemRoles.SetupModal();
                },

                SetupModal: function () {
                    SEL.ItemRoles.DomIDs.Modal.dialog({
                        autoOpen: false,
                        resizable: false,
                        width: 900,
                        modal: true,
                        dialogClass: "ui-no-close-button",
                        buttons: [
                            {
                                text: "save",
                                id: "btnSave",
                                "class": "jQueryUIButton",
                                click: function () {
                                    SEL.ItemRoles.AssociatedExpenseItems.Save();
                                }
                            }, {
                                text: "cancel",
                                id: "btnCancel",
                                "class": "jQueryUIButton",
                                click: function () {
                                    SEL.ItemRoles.Modal.Hide();
                                }
                            }
                        ]
                    });
                },

                SetupEnterKeyBindings: function () {
                    // Base Save
                    SEL.Common.BindEnterKeyForSelector('.primaryPage', SEL.Template.TemplateItem.Save);
                    // Sub Item Save
                    SEL.Common.BindEnterKeyForSelector('#' + SEL.Template.DomIDs.TemplateSubItem.Panel, SEL.Template.TemplateSubItem.Save);

                    $(document).keydown(function (e) {
                        if (e.keyCode === 27) // esc
                        {
                            e.preventDefault();
                            if ($g(SEL.Template.DomIDs.TemplateSubItem.Panel).style.display == '') {
                                SEL.Template.TemplateSubItem.Cancel();
                                return;
                            }
                        }
                    });
                },

                ItemRole:
                {
                    Delete: function (id) {
                        if (confirm('Are you sure you would like to delete the selected role?')) {
                            SEL.Data.Ajax({
                                data: {
                                    id: id
                                },
                                url: '/expenses/webservices/itemroles.asmx/DeleteItemRole',
                                success: function (r) {
                                    switch (r.d) {
                                        case -1:
                                            SEL.MasterPopup.ShowMasterPopup('The item role cannot be deleted as it is associated with one or more flag rules.', 'Message from ' + moduleNameHTML);
                                            break;
                                        default:
                                            SEL.ItemRoles.ItemRole.RefreshGrid();
                                            break;
                                    }
                                },
                                error: function (xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    },

                    Save: function () {
                        if (validateform('vgMain') === false) {
                            return false;
                        }

                        var itemRoleId = SEL.ItemRoles.IDs.itemRoleId;
                        var roleName = $(SEL.ItemRoles.DomIDs.ItemRole.roleName).val();
                        var description = $(SEL.ItemRoles.DomIDs.ItemRole.description).val();
                        

                        SEL.Data.Ajax({
                            data: {
                                id: itemRoleId,
                                roleName: roleName,
                                description: description
                            },
                            url: '/expenses/webservices/itemroles.asmx/SaveItemRole',
                            success: function (r) {
                                if (r.d === -1) {
                                    SEL.MasterPopup.ShowMasterPopup('The Name already exists.',
                                        'Message from ' + moduleNameHTML);
                                } else {
                                    if (!SEL.ItemRoles.ShowAssociatedExpenseItemModal) {
                                        document.location = 'itemroles.aspx';
                                    }
                                    else {

                                        if (SEL.ItemRoles.IDs.itemRoleId === 0) {
                                            SEL.Grid.updateGridQueryFilterValues("gridAssociatedExpenseItems",
                                                "01b16558-79df-44d9-914f-ad9092b4d5d2",
                                                [r.d],
                                                []);
                                        }

                                        SEL.ItemRoles.IDs.itemRoleId = r.d;
                                        SEL.ItemRoles.Modal.Show();

                                    }
                                }
                            },
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                    Cancel: function () {
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

                    RefreshGrid: function () {
                        SEL.Grid.refreshGrid('gridItemRoles', SEL.Grid.getCurrentPageNum('gridItemRoles'));
                    }
                },
                AssociatedExpenseItems:
                {
                    Delete: function (id, subcatId)
                    {
                        if (confirm('Are you sure you would like to unassociate the selected expense item?')) {
                            SEL.ItemRoles.IDs.subcatId = subcatId;
                            var index = SEL.ItemRoles.ExpenseItems.SelectedItems.indexOf(SEL.ItemRoles.IDs.subcatId);
                            if (index > -1) {
                                SEL.ItemRoles.ExpenseItems.SelectedItems.splice(index, 1);
                            }
                            SEL.Data.Ajax({
                                data: {
                                    roleSubcatId: id
                                },
                                url: '/expenses/webservices/itemroles.asmx/DeleteExpenseItemToItemRoleAssociation',
                                success: function (r) {
                                    
                                    SEL.ItemRoles.AssociatedExpenseItems.RefreshGrid();
                                }
                                ,
                                error: function (xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    },
                    Save: function ()
                    {
                        if (validateform('vgAssociatedExpenseItem') === false) {
                            return false;
                        }

                        var expenseItem = $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem).val();
                        if (expenseItem == null)
                        {
                            SEL.MasterPopup.ShowMasterPopup('Please create an expense item to associate it to an item role.',
                                'Message from ' + moduleNameHTML);
                            return;
                        }

                        var limitWithoutReceipt = $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithoutReceipt).val();
                        var limitWithReceipt = $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithReceipt).val();
                        var addToTemplate = $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.addToTemplate)[0].checked;

                        if (limitWithoutReceipt == '')
                        {
                            limitWithoutReceipt = 0;
                        }

                        if (limitWithReceipt == '')
                        {
                            limitWithReceipt = 0;
                        }

                        SEL.Data.Ajax({
                            data: {
                                roleSubcatId: SEL.ItemRoles.IDs.roleSubcatId,
                                itemRoleId: SEL.ItemRoles.IDs.itemRoleId,
                                subcatId: expenseItem,
                                maximumLimitWithoutReceipt: limitWithoutReceipt,
                                maximumLimitWithReceipt: limitWithReceipt,
                                addToTemplate: addToTemplate
                            },
                            url: '/expenses/webservices/itemroles.asmx/SaveExpenseItemToItemRoleAssociation',
                            success: function (r) {
                                SEL.ItemRoles.AssociatedExpenseItems.RefreshGrid();
                                SEL.ItemRoles.ExpenseItems.SelectedItems.push(Number($(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem).val()));
                                SEL.ItemRoles.Modal.Hide();
                            }
                            ,
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },
                    Edit: function (itemRoleId, roleSubcatId) {
                        SEL.Data.Ajax({
                            data: {
                                
                                itemRoleId: itemRoleId,
                                roleSubcatId: roleSubcatId
                            },
                            url: '/expenses/webservices/itemroles.asmx/GetRoleSubcat',
                            success: function (r) {
                                SEL.ItemRoles.ExpenseItems.CreateDropDown(r.d.SubcatId);
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem).val(r.d.SubcatId).change();
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem).prop('disabled', true);
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithoutReceipt).val(r.d.MaximumLimitWithoutReceipt);
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithReceipt).val(r.d.MaximumLimitWithReceipt);
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.addToTemplate).prop('checked', r.d.IsAddItem);
                                SEL.ItemRoles.Modal.Show();
                            }
                            ,
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },
                    RefreshGrid: function () {
                        SEL.Grid.refreshGrid('gridAssociatedExpenseItems', SEL.Grid.getCurrentPageNum('gridAssociatedExpenseItems'));
                    }
                },
                
                Modal:
                {
                    Show: function () {
                        SEL.ItemRoles.SetupModal();
                        SEL.ItemRoles.DomIDs.Modal.dialog("open");
                    },

                    Hide: function () {
                        SEL.ItemRoles.Modal.Clear();
                        SEL.ItemRoles.DomIDs.Modal.dialog("close");
                    },

                    Clear: function () {
                        SEL.ItemRoles.Modal.Reset();
                    },

                    Reset: function () {
                        if ($(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem)[0].options.length > 0) {
                            $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem)[0].selectedIndex = 0;
                        }
                        $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem).prop('disabled', false);
                        $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithoutReceipt).val('');
                        $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.limitWithReceipt).val('');
                        $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.addToTemplate).prop('checked', false);
                    }
                },

                ExpenseItems:
                {
                    ItemList: [],
                    SelectedItems: [],
                    GetExpenseItems: function ()
                    {
                        SEL.Data.Ajax({
                            data: {
                                itemRoleId: SEL.ItemRoles.IDs.itemRoleId
                            },
                            url: '/expenses/webservices/itemroles.asmx/GetExpenseItems',
                            success: function (r) {
                                SEL.ItemRoles.ExpenseItems.ItemList = r.d;
                            },
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },
                    CreateDropDown: function (selectedItem)
                    {
                        $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem)[0].options.length = 0;
                        for (var i = 0; i < SEL.ItemRoles.ExpenseItems.ItemList.length; i++) {
                            if (SEL.ItemRoles.ExpenseItems.SelectedItems.indexOf(Number(SEL.ItemRoles.ExpenseItems.ItemList[i].Value)) === -1 || Number(SEL.ItemRoles.ExpenseItems.ItemList[i].Value) == selectedItem) {
                                var option = document.createElement("OPTION");
                                option.value = SEL.ItemRoles.ExpenseItems.ItemList[i].Value;
                                option.text = SEL.ItemRoles.ExpenseItems.ItemList[i].Text;
                                $(SEL.ItemRoles.DomIDs.AssociatedExpenseItem.expenseItem)[0].options.add(option);
                            }
                        }
                    }
                }
                
            };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, moduleNameHTML, appPath));