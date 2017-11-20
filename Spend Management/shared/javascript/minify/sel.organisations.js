(function (SEL, moduleNameHtml)
{
    var scriptName = "organisations";

    function execute()
    {
        SEL.registerNamespace("SEL.Organisations");
        SEL.Organisations =
        {
            Type:
            {
                Organisation: "SpendManagementLibrary.Organisations.Organisation"
            },

            Identifiers:
            {
                Organisation: null
            },

            Dom:
            {
                Summary:
                {
                    Grid: null
                },
                
                Organisation:
                {
                    Modal: null,
                    Tabs: null,
                    Panel: null,
                    PanelTitle: null,

                    Name: null,
                    Code: null,
                    Comment: null,
                    PrimaryAddress: null,
                    PrimaryAddressID: null,
                    ParentOrganisationID: null
                }
            },

            Enum:
            {
                CreationMethod:
                {
                    None: 0
                }
            },

            Strings:
            {
                Organisation: {
                    ConfirmDelete: "Are you sure you want to delete this organisation?",
                    Duplicate: "The Organisation name already exists.",
                    DeleteFailed: "The organisation could not be deleted.",
                    
                    Permission: {
                        ViewDenied: "You do not have permission to view this organisation.",
                        AddDenied: "You do not have permission to add this organisation.",
                        EditDenied: "You do not have permission to edit this organisation.",
                        DeleteDenied: "You do not have permission to delete this organisation."
                    }
                }
            },

            Page:
            {
                Close: function () {

                    var documentLocation = (window.CurrentUserInfo.Module.Name === "contracts") ? "/MenuMain.aspx?menusection=sandl" : "/categorymenu.aspx";
                    document.location = window.appPath + documentLocation;
                },

                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('gridOrganisations', SEL.Grid.getCurrentPageNum('gridOrganisations'));
                }
            },

            Setup: function ()
            {
                var ns = SEL.Organisations,
                    doms = ns.Dom.Organisation,
                    identifiers = ns.Identifiers;

                window.Sys.Application.add_load(function ()
                {
                    ns.SetupKeyBindings();

                    $("#" + doms.PrimaryAddress).address({
                        "appendTo": "#" + doms.Panel,
                        "enableFavourites": false,
                        "enableLabels": false,
                        "enableAddingAddresses": false
                    });
                });
            },

            SetupKeyBindings: function ()
            {
                SEL.Common.SetTextAreaMaxLength();

                // Base Save
                SEL.Common.BindEnterKeyForSelector("#" + SEL.Organisations.Dom.Organisation.Panel + " .twocolumn input", SEL.Organisations.Organisation.Save);
                SEL.Common.BindEnterKeyForSelector("#" + SEL.Organisations.Dom.Organisation.Panel + " .onecolumn textarea", SEL.Organisations.Organisation.Save);

                $(document).keydown(function (e)
                {
                    if (e.keyCode === $.ui.keyCode.ESCAPE)
                    {
                        e.preventDefault();
                        
                        if ($("#" + SEL.Organisations.Dom.Organisation.Panel).filter(":visible").length > 0)
                        {
                            SEL.Organisations.Organisation.Cancel();
                            return;
                        }
                    }
                });
            },

            Organisation:
            {
                New: function ()
                {
                    // variable shortening
                    var ns = SEL.Organisations,
                        identifiers = ns.Identifiers;

                    // identifier reset
                    identifiers.Organisation = 0;

                    // clear modal / reset grids
                    ns.Organisation.Modal.Clear.All();

                    // prep grid yet-to-be-filters
                    //ns.Organisation.Addresses.GetGrid(0);
                    
                    // show modal
                    ns.Organisation.Modal.Show();

                    // set default input
                    $("#" + ns.Dom.Organisation.Name).focus();
                },

                Edit: function (organisationIdentifier)
                {
                    // validation
                    if (organisationIdentifier <= 0)
                    {
                        return false;
                    }

                    // variable shortening, declaration and assignment
                    var organisation = null,
                        ns = SEL.Organisations,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.Organisation;

                    // update identifier
                    identifiers.Organisation = organisationIdentifier;

                    // modal setup
                    ns.Organisation.Modal.Clear.All();

                    // attempt to retrieve item
                    SEL.Data.Ajax({
                        data: { "identifier": organisationIdentifier },
                        methodName: "Get",
                        serviceName: "svcOrganisations",

                        success: function (data)
                        {
                            if ("d" in data)
                            {
                                if (data.d === -999)
                                {
                                    SEL.MasterPopup.ShowMasterPopup("You do not have permission to view this organisation.", "Message from " + moduleNameHtml);
                                    return;
                                }

                                organisation = data.d;

                                if (typeof organisation !== "undefined" && organisation !== null)
                                {
                                    // update modal title
                                    $("#" + doms.PanelTitle).html("Organisation: " + organisation.Name);

                                    // update edited id
                                    identifiers.Organisation = organisation.Identifier;

                                    // update inputs
                                    $("#" + doms.Name).val(organisation.Name);
                                    $("#" + doms.Code).val(organisation.Code);
                                    $("#" + doms.ParentOrganisationID + "_ID").val(organisation.ParentOrganisationIdentifier);
                                    $("#" + doms.Comment).val(organisation.Comment);
                                    if (organisation.PrimaryAddress !== null)
                                    {
                                        $("#" + doms.PrimaryAddressID).val(organisation.PrimaryAddress.Identifier);
                                        $("#" + doms.PrimaryAddress).val(organisation.PrimaryAddress.FriendlyName);
                                    }
                                                
                                    if (organisation.ParentOrganisationIdentifier > 0)
                                    {
                                        // populate the autocomplete name field
                                        SEL.Data.Ajax({
                                            data: { "identifier": organisation.ParentOrganisationIdentifier },
                                            methodName: "Get",
                                            serviceName: "svcOrganisations",

                                            success: function (poData)
                                            {
                                                if ("d" in poData && "Name" in poData.d)
                                                {
                                                    $("#" + doms.ParentOrganisationID).val(poData.d.Name);
                                                }
                                            }
                                        });
                                    }

                                    // show modal
                                    ns.Organisation.Modal.Show();

                                    // set default input
                                    $("#" + doms.Name).focus();
                                }
                            }
                        }
                    });
                },

                Delete: function (organisationIdentifier)
                {
                    var messages = SEL.Organisations.Strings.Organisation;
                    
                    if (confirm(messages.ConfirmDelete))
                    {
                        SEL.Data.Ajax({
                            data: { "identifier": organisationIdentifier },
                            methodName: "Delete",
                            serviceName: "svcOrganisations",

                            success: function (r)
                            {
                                if (r.d === -999)
                                {
                                    SEL.MasterPopup.ShowMasterPopup(messages.Permission.DeleteDenied, "Message from " + moduleNameHtml);
                                    return;
                                }

                                if (r.d < 0)
                                {
                                    switch (r.d)
                                    {
                                        case -1:
                                            SEL.MasterPopup.ShowMasterPopup("This organisation cannot be deleted because it does not exist.", "Message from " + moduleNameHtml);
                                            return;
                                        case -2:
                                            SEL.MasterPopup.ShowMasterPopup("This organisation cannot be deleted because it is used in an expense claim.", "Message from " + moduleNameHtml);
                                            return;
                                        case -3:
                                            SEL.MasterPopup.ShowMasterPopup("This organisation cannot be deleted because it is linked as a parent organisation.", "Message from " + moduleNameHtml);
                                            return;
                                        case -10:
                                            SEL.MasterPopup.ShowMasterPopup("This organisation cannot be deleted because it is linked to a GreenLight.", "Message from " + moduleNameHtml);
                                            return;
                                        default:
                                            SEL.MasterPopup.ShowMasterPopup("This organisation cannot be deleted because it is in use.", "Message from " + moduleNameHtml);
                                            return;
                                    }
                                }

                                if (r.d !== 0)
                                {
                                    SEL.MasterPopup.ShowMasterPopup(messages.DeleteFailed, "Message from " + moduleNameHtml);
                                    return;
                                }

                                // refresh main grid
                                SEL.Organisations.Page.RefreshGrid();
                            }
                        });
                    }
                },

                Save: function (closeModal)
                {
                    if (typeof SEL.Organisations.Identifiers.Organisation === "undefined" || SEL.Organisations.Identifiers.Organisation === null || SEL.Organisations.Identifiers.Organisation < 0)
                    {
                        return false;
                    }

                    if (validateform("vgOrganisation") === false)
                    {
                        return false;
                    }

                    var ns = SEL.Organisations,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.Organisation,
                        messages = ns.Strings.Organisation,
                        name = $("#" + doms.Name).val(),
                        code = $("#" + doms.Code).val(),
                        comment = $("#" + doms.Comment).val(),
                        parentOrganisationIdentifier = $("#" + doms.ParentOrganisationID + "_ID").val(),
                        primaryAddressIdentifier = $("#" + doms.PrimaryAddressID).val();

                    parentOrganisationIdentifier = isNaN(parseInt(parentOrganisationIdentifier, 10)) ? 0 : parseInt(parentOrganisationIdentifier, 10);
                    primaryAddressIdentifier = isNaN(parseInt(primaryAddressIdentifier, 10)) ? 0 : parseInt(primaryAddressIdentifier, 10);

                    SEL.Data.Ajax({
                        data: { "identifier": identifiers.Organisation, "name": name, "code": code, "comment": comment, "parentOrganisationIdentifier": parentOrganisationIdentifier, "primaryAddressIdentifier": primaryAddressIdentifier },
                        methodName: "Save",
                        serviceName: "svcOrganisations",

                        success: function (r)
                        {
                            if ("d" in r)
                            {
                                if (r.d === -999)
                                {
                                    SEL.MasterPopup.ShowMasterPopup((identifiers.Organisation === 0 ? messages.Permission.AddDenied : messages.Permission.EditDenied), "Message from " + moduleNameHtml);
                                    return false;
                                }

                                if (r.d < 0)
                                {
                                    SEL.MasterPopup.ShowMasterPopup(messages.Duplicate, "Message from " + moduleNameHtml);
                                    return false;
                                }

                                ns.Page.RefreshGrid();
                                
                                if (closeModal === false)
                                {
                                    identifiers.Organisation = r.d;
                                }
                                else
                                {
                                    ns.Organisation.Modal.Hide();
                                }
                            }
                        }
                    });
                },

                Cancel: function ()
                {
                    SEL.Organisations.Organisation.Modal.Hide();
                },
                
                ToggleArchive: function (identifier)
                {
                    SEL.Data.Ajax({
                        data: { "identifier": identifier },
                        methodName: "ToggleArchive",
                        serviceName: "svcOrganisations",

                        success: function (r)
                        {
                            if (r.d === -999)
                            {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to archive this organisation.", "Message from " + moduleNameHtml);
                                return;
                            }

                            if (r.d < 0)
                            {
                                SEL.MasterPopup.ShowMasterPopup("The organisation provided cannot be archived.", "Message from " + moduleNameHtml);
                                return;
                            }
                            
                            // no need to stop them using archived organisations for parent

                            SEL.Organisations.Page.RefreshGrid();
                        }
                    });
                },

                Modal:
                {
                    Clear:
                    {
                        All: function ()
                        {
                            var ns = SEL.Organisations,
                                modal = ns.Organisation.Modal,
                                tabs = modal.Tabs;

                            modal.Clear.Title();
                            modal.Clear.Organisation();

                            // deal with tabs
                            //tabs.Addresses.Enabled(false);
                        },

                        Title: function ()
                        {
                            $("#" + SEL.Organisations.Dom.Organisation.PanelTitle).html("New Organisation");
                        },

                        Organisation: function ()
                        {
                            var doms = SEL.Organisations.Dom.Organisation;

                            // //////////////////////////////////////////////////
                            // Organisation tab inputs
                            // //////////////////////////////////////////////////
                            $("#" + doms.PanelTitle).html("New Organisation");
                            $("#" + doms.Name).val("");
                            $("#" + doms.Code).val("");
                            $("#" + doms.ParentOrganisationID + "_ID").val("");
                            $("#" + doms.ParentOrganisationID).val("");
                            $("#" + doms.Comment).val("");
                            $("#" + doms.PrimaryAddressID).val("");
                            $("#" + doms.PrimaryAddress).address("reset").val("");

                            // Clear validation
                            SEL.Common.Page_ClientValidateReset();
                        },

                        Addresses: function ()
                        {
                        }
                    },

                    Show: function ()
                    {
                        SEL.Common.ShowModal(SEL.Organisations.Dom.Organisation.Modal);
                    },

                    Hide: function ()
                    {
                        SEL.Common.HideModal(SEL.Organisations.Dom.Organisation.Modal);
                    },

                    Tabs:
                    {
                        Change: function (sender)
                        {
                            var ns = SEL.Organisations;

                            if (ns.Identifiers.Organisation === 0)
                            {
                                if (ns.Organisation.Save(false) === false)
                                {
                                    $f(SEL.Organisations.Dom.Organisation.Tabs).set_activeTabIndex(0);
                                    return false;
                                }
                            }
                        },

                        SetEnabled: function (tab, enabled)
                        {
                            $f(SEL.Organisations.Dom.Organisation.Tabs).get_tabs()[tab].set_enabled(enabled);
                        },

                        Organisation:
                        {
                            Enabled: function (enabled)
                            {
                                SEL.Organisations.Organisation.Modal.Tabs.SetEnabled(SEL.Organisations.Enum.OrganisationModalTabs.Organisation, enabled);
                            }
                        },

                        Addresses:
                        {
                            Enabled: function (enabled)
                            {
                                SEL.Organisations.Organisation.Modal.Tabs.SetEnabled(SEL.Organisations.Enum.OrganisationModalTabs.Addresses, enabled);
                            }
                        }
                    }
                },

                Addresses:
                {
                }
            },
            
            Validate:
            {
                ParentOrganisation: function (sender, args)
                {
                    var namespace = SEL.Organisations,
                        value = $("#" + namespace.Dom.Organisation.ParentOrganisationID + "_ID").val();

                    if (value === "")
                    {
                        args.IsValid = true;
                        return;
                    }

                    value = parseInt(value, 10);

                    if (!isNaN(value) && value > 0)
                    {
                        args.IsValid = value !== namespace.Identifiers.Organisation;
                        return;
                    }
                    else
                    {
                        args.IsValid = true;
                        return;
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
}(SEL, moduleNameHTML));