(function (moduleNameHtml, appPath)
{
    var scriptName = "addresses";

    function execute()
    {
        SEL.registerNamespace("SEL.Addresses");
        SEL.Addresses =
        {
            Type:
            {
                AddressDistance: "SpendManagementLibrary.Addresses.AddressDistance"
            },
            
            Identifiers:
            {
                Address: null,
                CapturePlus: null,
                CreationMethod: null,
                Latitude: null,
                Longitude: null,
                EsrAddressIdentifier: null,
                EsrLocationIdentifier: null,
                PostcodeMandatory: null,
                DefaultGlobalCountry: null,
                AccountWideLabel: null
            },

            Dom:
            {
                Summary:
                {
                    Grid: null
                },

                Address:
                {
                    Modal: null,
                    Panel: null,
                    Search: null,
                    Tabs: null,

                    AddressName: null,
                    Line1: null,
                    Line1Validator: null,
                    Line2: null,
                    Line3: null,
                    City: null,
                    County: null,
                    Country: null,
                    Postcode: null,
                    PostcodeValidator: null,
                    Favourited: null,
                    
                    AccountWideLabels:
                    {
                        Grid: null,
                        Modal: null,
                        Panel: null,
                        Label: null,
                        PrimaryLabel: null
                    },
                    
                    RecommendedDistances:
                    { 
                        NewDestination: null,
                        DestinationSearch: null,
                        Destinations: null,
                        DestinationsContainer: null
                    }
                }
            },

            Enum:
            {
                CreationMethod:
                {
                    None: 0,
                    CapturePlus: 1,
                    Global: 2,
                    EsrOutbound: 3,
                    ImplementationImportRoutine: 4,
                    DataImportWizard: 5,
                    ManualByAdministrator: 6,
                    ManualByClaimant: 7
                },
                
                AddressModalTabs:
                {
                    Address: 0,
                    AccountWideLabels: 1,
                    RecommendedDistances: 2
                }
            },
            
            Strings:
            {
                Address: {},

                AccountWideLabel: {
                    ConfirmDelete: "Are you sure you wish to delete this account-wide label?"
                },
                
                RecommendedDistance: {
                    ConfirmDelete: "Are you sure you wish to delete this distance?"
                }
            },

            Page:
            {
                Close: function ()
                {
                    var documentLocation = (window.CurrentUserInfo.Module.Name === "contracts") ? "/MenuMain.aspx?menusection=sandl" : "/categorymenu.aspx";
                    document.location = window.appPath + documentLocation;
                },

                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('gridAddresses', SEL.Grid.getCurrentPageNum('gridAddresses'));
                }
            },

            Setup: function ()
            {
                var ns = SEL.Addresses,
                    doms = ns.Dom.Address,
                    identifiers = ns.Identifiers;

                window.Sys.Application.add_load(function ()
                {
                    ns.SetupKeyBindings();

                    $("#" + doms.RecommendedDistances.DestinationsContainer).css({ "height": "305px", "overflow-y": "auto" });
                    $("#gridAwabels").css({ "height": "393px", "overflow-y": "auto" });
                    
                    $("#" + doms.Search).address({
                        "appendTo": "#" + doms.Panel, 
                        "enableFavourites": false,
                        "enableAccountWideFavourites": false,
                        "enableLabels": false,
                        "enableAccountWideLabels": false,
                        "enableManualAddresses": false,
                        "forceAddressNameEntry": identifiers.ForceAddressNameEntry,
                        "strings": {
                            "addressNameEntryMessage": identifiers.AddressNameEntryMessage
                        }
                    })
                    .on("addressnotfoundclick", function ()
                    {
                        identifiers.CapturePlus = "";
                        identifiers.Latitude = "";
                        identifiers.Longitude = "";
                        identifiers.EsrAddressIdentifier = null;
                        identifiers.EsrLocationIdentifier = null;
                        identifiers.CreationMethod = ns.Enum.CreationMethod.ManualByAdministrator;
                        
                        ns.Address.Modal.MakeAddressFieldsEditable(true);
                        ns.Address.Modal.Tabs.AccountWideLabels.Enabled(true);
                        ns.Address.Modal.Tabs.RecommendedDistances.Enabled(true);
                        
                        $(this).address("close").address("reset").val("");
                        $("#" + doms.Line1).focus();
                        
                        return false;
                    })
                    .on("addressmatched", function (event, addressData)
                    {
                        ns.Address.Edit(addressData.Identifier);
                    });

                    $("#" + doms.RecommendedDistances.DestinationSearch).address({
                        "appendTo": "#" + doms.Panel,
                        "enableFavourites": false,
                        "enableLabels": false,
                        "enableAddingAddresses": false,
                        "enableAccountWideFavourites": false,
                        "enableAccountWideLabels": false,
                        "enableManualAddresses": true,
                        "forceAddressNameEntry": identifiers.ForceAddressNameEntry,
                        "strings": {
                            "addressNameEntryMessage": identifiers.AddressNameEntryMessage
                        }
                    })
                    .on("addressmatched", function (event, addressData)
                    {
                        if ("d" in addressData)
                        {
                            addressData = addressData.d;
                        }
                        
                        ns.Address.RecommendedDistances.Get(addressData.Identifier);
                    });
                });
            },
            
            SetupKeyBindings: function ()
            {
                // Base Save
                SEL.Common.BindEnterKeyForSelector("#" + SEL.Addresses.Dom.Address.Panel + ".formpanel .twocolumn", SEL.Addresses.Address.Save);

                $("#" + SEL.Addresses.Dom.Address.AccountWideLabels.Label).on("keypress", function (e)
                {
                    if (e.which === $.ui.keyCode.ENTER)
                    {
                        SEL.Addresses.Address.AccountWideLabels.Save();
                    }
                });

                $(document).keydown(function (e)
                {
                    if (e.keyCode === $.ui.keyCode.ESCAPE)
                    {
                        e.preventDefault();
                        
                        if ($("#" + SEL.Addresses.Dom.Address.AccountWideLabels.Panel).filter(":visible").length > 0)
                        {
                            SEL.Addresses.Address.AccountWideLabels.Cancel();
                            return;
                        }
                        
                        if ($("#" + SEL.Addresses.Dom.Address.Panel).filter(":visible").length > 0)
                        {
                            SEL.Addresses.Address.Cancel();
                            return;
                        }
                    }
                });
            },

            Address:
            {
                New: function ()
                {
                    var ns = SEL.Addresses,
                        identifiers = ns.Identifiers;
                    
                    identifiers.Address = 0;
                    identifiers.CreationMethod = ns.Enum.CreationMethod.None;
                    identifiers.CapturePlus = "";
                    identifiers.Latitude = "";
                    identifiers.Longitude = "";
                    identifiers.EsrAddressIdentifier = null;
                    identifiers.EsrLocationIdentifier = null;

                    ns.Address.Modal.Clear.All();
                    
                    ns.Address.AccountWideLabels.GetGrid(0);

                    $("#" + ns.Dom.Address.Country).val(identifiers.DefaultGlobalCountry);

                    ns.Address.Modal.Show();
                    
                    $("#" + ns.Dom.Address.Search).focus();
                },

                Edit: function (addressIdentifier)
                {
                    // validation
                    if (addressIdentifier <= 0)
                    {
                        return false;
                    }

                    // variable declaration and assignment
                    var address = null,
                        ns = SEL.Addresses,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.Address,
                        creationMethod = ns.Enum.CreationMethod,
                        preserveAddressInformation = false;

                    identifiers.Address = addressIdentifier;
                    identifiers.CreationMethod = creationMethod.None;
                    identifiers.CapturePlus = "";
                    identifiers.Latitude = "";
                    identifiers.Longitude = "";
                    identifiers.EsrAddressIdentifier = null;
                    identifiers.EsrLocationIdentifier = null;
                    
                    // modal setup
                    ns.Address.Modal.Clear.All();

                    // attempt to retrieve address
                    SEL.Data.Ajax({
                        data: { "addressIdentifier": addressIdentifier, "labelId": 0 },
                        methodName: "Get",
                        serviceName: "svcAddresses",

                        success: function(data)
                        {
                            if (data.d === -999)
                            {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to view this address.", "Message from " + moduleNameHtml);
                                return;
                            }

                            address = data.d;

                            if (typeof address !== "undefined" && address !== null)
                            {
                                $("#" + doms.PanelTitle).html("Address: " + address.Line1 + (address.Postcode !== null && address.Postcode !== "" ? ", " + address.Postcode : ""));

                                preserveAddressInformation = address.CreationMethod === creationMethod.CapturePlus
                                    || address.CreationMethod === creationMethod.EsrOutbound
                                    || address.CreationMethod === creationMethod.Global;

                                ns.Address.Modal.MakeAddressFieldsEditable(!preserveAddressInformation);

                                identifiers.CreationMethod = address.CreationMethod;
                                identifiers.CapturePlus = address.GlobalIdentifier;
                                identifiers.Latitude = address.Latitude;
                                identifiers.Longitude = address.Longitude;
                                identifiers.EsrAddressIdentifier = address.EsrAddressIdentifier;
                                identifiers.EsrLocationIdentifier = address.EsrLocationIdentifier;

                                $("#" + doms.Search).prop("disabled", true);
                                $("#" + doms.AddressName).val(address.AddressName);
                                $("#" + doms.Line1).val(address.Line1);
                                $("#" + doms.Line2).val(address.Line2);
                                $("#" + doms.Line3).val(address.Line3);
                                $("#" + doms.City).val(address.City);
                                $("#" + doms.County).val(address.County);
                                $("#" + doms.Country).val(address.CountryAlpha3Code);
                                $("#" + doms.Postcode).val(address.Postcode);
                                $("#" + doms.Favourited).prop({ "disabled": false, "checked": address.AccountWideFavourite });

                                ns.Address.AccountWideLabels.GetGrid(addressIdentifier);
                                ns.Address.Modal.Tabs.AccountWideLabels.Enabled(true);
                                ns.Address.RecommendedDistances.GetGrid(addressIdentifier);

                                ns.Address.Modal.Show();

                                $("#" + (preserveAddressInformation ? doms.Favourited : doms.Line1)).focus();
                            }
                        }
                    });
                },

                Delete: function (addressIdentifier)
                {
                    if (confirm("Are you sure you want to delete this address?"))
                    {
                        SEL.Data.Ajax({
                            data: { "addressIdentifier": addressIdentifier },
                            methodName: "Delete",
                            serviceName: "svcAddresses",

                            success: function (r)
                            {
                                if (r.d === -999)
                                {
                                    SEL.MasterPopup.ShowMasterPopup("You do not have permission to delete this address.", "Message from " + moduleNameHtml);
                                    return;
                                }

                                if (r.d < 0)
                                {
                                    switch(r.d)
                                    {
                                        case -1:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is used in an expense claim.", "Message from " + moduleNameHtml);
                                            return;
                                        case -2:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is linked to an ESR Address.", "Message from " + moduleNameHtml);
                                            return;
                                        case -3:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is linked to an ESR Location.", "Message from " + moduleNameHtml);
                                            return;
                                        case -4:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is an employee's home address.", "Message from " + moduleNameHtml);
                                            return;
                                        case -5:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is an employee's work address.", "Message from " + moduleNameHtml);
                                            return;
                                        case -6:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is linked to an organisation.", "Message from " + moduleNameHtml);
                                            return;
                                        default:
                                            SEL.MasterPopup.ShowMasterPopup("This address cannot be deleted because it is in use.", "Message from " + moduleNameHtml);
                                            return;
                                    }
                                }

                                if (r.d !== 0)
                                {
                                    SEL.MasterPopup.ShowMasterPopup("The address could not be deleted.", "Message from " + moduleNameHtml);
                                    return;
                                }

                                SEL.Addresses.Page.RefreshGrid();
                            }
                        });
                    }
                },
                
                ToggleAccountWideFavourite: function (addressIdentifier)
                {
                    SEL.Data.Ajax({
                        data: { "addressIdentifier": addressIdentifier },
                        methodName: "ToggleAccountWideFavourite",
                        serviceName: "svcAddresses",

                        success: function (r)
                        {
                            if (r.d === -999)
                            {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to edit this address.", "Message from " + moduleNameHtml);
                                return;
                            }

                            if (r.d < 0)
                            {
                                SEL.MasterPopup.ShowMasterPopup("The address provided cannot be set as an account-wide favourite.", "Message from " + moduleNameHtml);
                                return;
                            }

                            SEL.Addresses.Page.RefreshGrid();
                        }
                    });
                },
                
                ToggleArchive: function (addressIdentifier)
                {
                    SEL.Data.Ajax({
                        data: { "addressIdentifier": addressIdentifier },
                        methodName: "ToggleArchive",
                        serviceName: "svcAddresses",

                        success: function (r)
                        {
                            if (r.d === -999)
                            {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to archive this address.", "Message from " + moduleNameHtml);
                                return;
                            }

                            if (r.d < 0)
                            {
                                SEL.MasterPopup.ShowMasterPopup("The address provided cannot be archived.", "Message from " + moduleNameHtml);
                                return;
                            }

                            SEL.Addresses.Page.RefreshGrid();
                        }
                    });
                },

                Save: function (closeModal)
                {
                    if (validateform("vgAddress") === false)
                    {
                        return false;
                    }

                    var ns = SEL.Addresses,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.Address,
                        addressName = $("#" + doms.AddressName).val(),
                        line1 = $("#" + doms.Line1).val(),
                        line2 = $("#" + doms.Line2).val(),
                        line3 = $("#" + doms.Line3).val(),
                        city = $("#" + doms.City).val(),
                        county = $("#" + doms.County).val(),
                        alpha3CountryCode = $("#" + doms.Country).val(),
                        postcode = $("#" + doms.Postcode).val(),
                        favourited = $("#" + doms.Favourited).prop("checked"),
                        recommendedDistances = ns.Address.RecommendedDistances.GetRecommendedDistancesFromGrid();
                    
                    SEL.Data.Ajax({
                        data: { "addressIdentifier": identifiers.Address, "addressName": addressName, "line1": line1, "line2": line2, "line3": line3, "city": city, "county": county, "alpha3CountryCode": alpha3CountryCode, "postcode": postcode, "latitude": identifiers.Latitude, "longitude": identifiers.Longitude, "capturePlusIdentifier": identifiers.CapturePlus, "esrLocationIdentifier": identifiers.EsrLocationIdentifier, "esrAddressIdentifier": identifiers.EsrAddressIdentifier, "accountWideFavourite": favourited, "recommendedDistances": recommendedDistances, accessRoleCheck: true },
                        methodName: "Save",
                        serviceName: "svcAddresses",
                        
                        success: function (r)
                        {
                            if ("d" in r)
                            {
                                if (r.d === -999)
                                {
                                    SEL.MasterPopup.ShowMasterPopup("You do not have permission to " + (identifiers.Address === 0 ? "add" : "edit") + " this address.", "Message from " + moduleNameHtml);
                                    return false;
                                }

                                if (r.d < 0)
                                {
                                    SEL.MasterPopup.ShowMasterPopup("The Address already exists.", "Message from " + moduleNameHtml);
                                    return false;
                                }

                                ns.Page.RefreshGrid();

                                if (closeModal === false)
                                {
                                    identifiers.Address = r.d;
                                    ns.Address.AccountWideLabels.GetGrid(r.d);
                                    ns.Address.RecommendedDistances.GetGrid(r.d);
                                }
                                else
                                {
                                    ns.Address.Modal.Hide();
                                }
                            }
                        }
                    });
                },

                Cleanse: function (addressId)
                {
                    if (confirm("This will remove all labels, favourites and recommended distances for this address, are you sure you want to proceed?")) {
                        
                        SEL.Data.Ajax({
                            serviceName: "svcAddresses",
                            methodName: "Cleanse",
                            data: {
                                addressId: addressId
                            },
                            success: function (r) {
                                if ("d" in r) {
                                    if (r.d === -999) {
                                        SEL.MasterPopup.ShowMasterPopup("You do not have permission to cleanse this address.", "Message from " + moduleNameHtml);
                                        return false;
                                    }
                                    SEL.Addresses.Page.RefreshGrid();
                                }
                            }
                        });
                    }
                },

                Cancel: function ()
                {
                    SEL.Addresses.Address.Modal.Hide();
                },

                Modal:
                {
                    Clear:
                    {
                        All: function ()
                        {
                            var ns = SEL.Addresses,
                                modal = ns.Address.Modal,
                                tabs = modal.Tabs;
                            
                            modal.Clear.Title();
                            modal.Clear.Address();
                            modal.Clear.AccountWideLabels();
                            modal.Clear.RecommendedDistances();

                            tabs.AccountWideLabels.Enabled(false);
                            tabs.RecommendedDistances.Enabled(false);
                        },

                        Title: function ()
                        {
                            $("#" + SEL.Addresses.Dom.Address.PanelTitle).html("New Address");
                        },
                        
                        Address: function ()
                        {
                            var doms = SEL.Addresses.Dom.Address;

                            // //////////////////////////////////////////////////
                            // Address tab inputs
                            // //////////////////////////////////////////////////
                            $("#" + doms.Search).address("reset").prop("disabled", false).val("");
                            $("#" + doms.AddressName).prop("disabled", true).val("");
                            $("#" + doms.Line1).prop("disabled", true).val("");
                            $("#" + doms.Line2).prop("disabled", true).val("");
                            $("#" + doms.Line3).prop("disabled", true).val("");
                            $("#" + doms.City).prop("disabled", true).val("");
                            $("#" + doms.County).prop("disabled", true).val("");
                            $("#" + doms.Country).prop("disabled", true).val(0);
                            $("#" + doms.Postcode).prop("disabled", true).val("");
                            $("#" + doms.Favourited).prop({ "disabled": false, "checked": false });

                            // //////////////////////////////////////////////////
                            // Clear validation
                            // //////////////////////////////////////////////////
                            $("#" + doms.Line1Validator)[0].isvalid = true;
                            ValidatorUpdateDisplay($("#" + doms.Line1Validator)[0]);
                            $("#" + doms.PostcodeValidator)[0].isvalid = true;
                            ValidatorUpdateDisplay($("#" + doms.PostcodeValidator)[0]);
                        },

                        AccountWideLabels: function ()
                        {
                            var ns = SEL.Addresses;

                            $("#" + ns.Dom.Address.AccountWideLabels.Grid).empty();
                        },

                        RecommendedDistances: function ()
                        {
                            var ns = SEL.Addresses;

                            // //////////////////////////////////////////////////
                            // Recommended Distances tab search inputs
                            // //////////////////////////////////////////////////
                            ns.Address.RecommendedDistances.ClearNewDestinationAddress();
                            ns.Address.RecommendedDistances.ClearNewDestinationDistances();
                            
                            $("#" + ns.Dom.Address.RecommendedDistances.Destinations).empty();
                        }
                    },

                    Show: function ()
                    {
                        SEL.Common.ShowModal(SEL.Addresses.Dom.Address.Modal);
                    },

                    Hide: function ()
                    {
                        SEL.Common.HideModal(SEL.Addresses.Dom.Address.Modal);
                    },
                    
                    MakeAddressFieldsEditable: function (addressFieldsAreEditable)
                    {
                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address;
                        $("#" + doms.AddressName).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.Line1).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.Line2).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.Line3).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.City).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.County).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.Country).prop("disabled", !addressFieldsAreEditable);
                        $("#" + doms.Postcode).prop("disabled", !addressFieldsAreEditable);

                        ValidatorEnable($("#" + doms.PostcodeValidator)[0], ns.Identifiers.PostcodeMandatory && addressFieldsAreEditable);
                        $("#" + doms.PostcodeValidator)[0].isvalid = true;
                        ValidatorUpdateDisplay($("#" + doms.PostcodeValidator)[0]);
                    },
                    
                    Tabs:
                    {
                        Change: function (sender)
                        {
                            var ns = SEL.Addresses;

                            if (ns.Identifiers.Address === 0 && sender.get_activeTabIndex() !== 0)
                            {
                                if (ns.Address.Save(false) === false)
                                {
                                    $f(SEL.Addresses.Dom.Address.Tabs).set_activeTabIndex(0);
                                    return false;
                                }
                            }
                        },
                        
                        SetEnabled: function (tab, enabled)
                        {
                            $f(SEL.Addresses.Dom.Address.Tabs).get_tabs()[tab].set_enabled(enabled);
                        },
                        
                        Address:
                        {
                            Enabled: function (enabled)
                            {
                                SEL.Addresses.Address.Modal.Tabs.SetEnabled(SEL.Addresses.Enum.AddressModalTabs.Address, enabled);
                            }
                        },

                        AccountWideLabels:
                        {
                            Enabled: function (enabled)
                            {
                                SEL.Addresses.Address.Modal.Tabs.SetEnabled(SEL.Addresses.Enum.AddressModalTabs.AccountWideLabels, enabled);
                            }
                        },

                        RecommendedDistances:
                        {
                            Enabled: function (enabled)
                            {
                                SEL.Addresses.Address.Modal.Tabs.SetEnabled(SEL.Addresses.Enum.AddressModalTabs.RecommendedDistances, enabled);
                            }
                        }
                    }
                },
                
                AccountWideLabels:
                {
                    GetGrid: function (addressIdentifier)
                    {
                        SEL.Grid.updateGridQueryFilterValues("gridAwabels", "2ba5481d-219f-470c-8001-e079eae4d76e", [addressIdentifier], []);

                        var ns = SEL.Addresses.Address;
                        
                        ns.AccountWideLabels.RefreshGrid();
                    },
                    
                    RefreshGrid: function ()
                    {
                        SEL.Grid.refreshGrid("gridAwabels", 1);
                    },
                    
                    Modal:
                    {
                        Clear: function ()
                        {
                            var ns = SEL.Addresses,
                                doms = ns.Dom.Address.AccountWideLabels;

                            ns.Identifiers.AccountWideLabel = 0;

                            $("#" + doms.Label).val("");
                            $("#" + doms.PrimaryLabel).prop({ "checked": false });

                            SEL.Common.Page_ClientValidateReset();
                        },
                        
                        Show: function()
                        {
                            var dom = SEL.Addresses.Dom.Address.AccountWideLabels;
                            SEL.Common.ShowModal(dom.Modal);
                            $("#" + dom.Label).focus();
                        },
                        
                        Hide: function()
                        {
                            SEL.Common.HideModal(SEL.Addresses.Dom.Address.AccountWideLabels.Modal);
                        }
                    },
                    
                    New: function()
                    {
                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address.AccountWideLabels;

                        ns.Address.AccountWideLabels.Modal.Clear();

                        $("#awabelTitle").text("New Account-wide Label");

                        ns.Address.AccountWideLabels.Modal.Show();
                    },

                    Edit: function (awabelIdentifier)
                    {
                        if (awabelIdentifier <= 0)
                        {
                            return;
                        }

                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address.AccountWideLabels,
                            awabel;

                        ns.Address.AccountWideLabels.Modal.Clear();
                        
                        ns.Identifiers.AccountWideLabel = awabelIdentifier;

                        SEL.Data.Ajax({
                            data: { "identifier": awabelIdentifier },
                            methodName: "GetAccountWideLabel",
                            serviceName: "svcAddresses",

                            success: function (data)
                            {
                                if ("d" in data)
                                {
                                    if (data.d === -999)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup("You do not have permission to view the account-wide label information.", "Message from " + moduleNameHtml);
                                        return;
                                    }

                                    awabel = data.d;

                                    if (typeof awabel !== "undefined" && awabel !== null && "Text" in awabel && "Primary" in awabel)
                                    {
                                        $("#awabelTitle").text("Account-wide Label: " + awabel.Text.replace("<", "&lt;"));
                                        
                                        $("#" + doms.Label).val(awabel.Text);
                                        $("#" + doms.PrimaryLabel).prop({ "checked": awabel.Primary });
                                    }

                                    ns.Address.AccountWideLabels.Modal.Show();
                                }
                            }
                        });
                    },

                    Save: function ()
                    {
                        if (validateform("vgAwabel") === false)
                        {
                            return false;
                        }

                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address.AccountWideLabels,
                            label = $("#" + doms.Label).val(),
                            primary = $("#" + doms.PrimaryLabel).prop("checked");

                        SEL.Data.Ajax({
                            data: { "addressIdentifier": ns.Identifiers.Address, "identifier": ns.Identifiers.AccountWideLabel, "label": label, "primary": primary },
                            methodName: "SaveAccountWideLabel",
                            serviceName: "svcAddresses",

                            success: function (data)
                            {
                                if ("d" in data)
                                {
                                    if (data.d === -999)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup("You do not have permission to save account-wide labels.", "Message from " + moduleNameHtml);
                                        return;
                                    }

                                    if (data.d === -1 || data.d === -2 || data.d === -3)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup("The Account-Wide Label text is already in use as a label or a home/office address keyword.", "Message from " + moduleNameHtml);
                                        return;
                                    }

                                    if (data.d > 0)
                                    {
                                        ns.Address.AccountWideLabels.RefreshGrid();
                                        ns.Address.AccountWideLabels.Modal.Hide();
                                    }
                                }
                            }
                        });
                    },

                    Cancel: function ()
                    {
                        var modal = SEL.Addresses.Address.AccountWideLabels.Modal;

                        modal.Hide();
                        modal.Clear();
                    },
                    
                    Delete: function(labelIdentifier)
                    {
                        if (labelIdentifier <= 0)
                        {
                            return;
                        }

                        var ns = SEL.Addresses;

                        if (confirm(ns.Strings.AccountWideLabel.ConfirmDelete))
                        {
                            SEL.Data.Ajax({
                                data: { "identifier": labelIdentifier },
                                methodName: "DeleteAccountWideLabel",
                                serviceName: "svcAddresses",

                                success: function (data)
                                {
                                    if ("d" in data)
                                    {
                                        if (data.d === -999)
                                        {
                                            SEL.MasterPopup.ShowMasterPopup("You do not have permission to delete account-wide labels.", "Message from " + moduleNameHtml);
                                            return;
                                        }

                                        ns.Address.AccountWideLabels.RefreshGrid();
                                    }
                                }
                            });
                        }
                    }
                },
                
                RecommendedDistances:
                {
                    Get: function (addressIdentifier)
                    {
                        // validation
                        if (addressIdentifier <= 0)
                        {
                            return;
                        }

                        // variable declaration and assignment
                        var recommendedDistance = null,
                            ns = SEL.Addresses,
                            doms = ns.Dom.Address,
                            newDestinationRow;

                        // attempt to retrieve recommended distances for address
                        SEL.Data.Ajax({
                            data: { "originIdentifier": ns.Identifiers.Address, "destinationIdentifier": addressIdentifier },
                            methodName: "GetAddressDistance",
                            serviceName: "svcAddresses",

                            success: function (data)
                            {
                                if (data.hasOwnProperty("d") && typeof data.d !== "undefined" && data.d !== null
                                    && data.d.hasOwnProperty("__type") && data.d.__type === SEL.Addresses.Type.AddressDistance)
                                {
                                    recommendedDistance = data.d;
                                    newDestinationRow = $("#" + doms.RecommendedDistances.NewDestination);
                                        
                                    if (recommendedDistance.DestinationIdentifier > 0)
                                    {
                                        newDestinationRow.data("di", recommendedDistance.DestinationIdentifier);
                                    }
                                        
                                    if (recommendedDistance.OutboundIdentifier > 0)
                                    {
                                        newDestinationRow.data("oi", recommendedDistance.OutboundIdentifier);
                                            
                                        if (recommendedDistance.Outbound > 0)
                                        {
                                            newDestinationRow.find(".addresses-outbound").val(recommendedDistance.Outbound);
                                        }

                                        if (recommendedDistance.OutboundFastest !== null)
                                        {
                                            newDestinationRow.find("span.addresses-outboundfastest").text(recommendedDistance.OutboundFastest);
                                        }

                                        if (recommendedDistance.OutboundShortest !== null)
                                        {
                                            newDestinationRow.find("span.addresses-outboundshortest").text(recommendedDistance.OutboundShortest);
                                        }
                                    }
                                        
                                    if (recommendedDistance.ReturnIdentifier > 0)
                                    {
                                        newDestinationRow.data("ri", recommendedDistance.ReturnIdentifier);
                                            
                                        if (recommendedDistance.Return > 0)
                                        {
                                            newDestinationRow.find(".addresses-return").val(recommendedDistance.Return);
                                        }

                                        if (recommendedDistance.ReturnFastest !== null)
                                        {
                                            newDestinationRow.find("span.addresses-returnfastest").text(recommendedDistance.ReturnFastest);
                                        }

                                        if (recommendedDistance.ReturnShortest !== null)
                                        {
                                            newDestinationRow.find("span.addresses-returnshortest").text(recommendedDistance.ReturnShortest);
                                        }
                                    }
                                }
                                else
                                {
                                    ns.Address.RecommendedDistances.ClearNewDestinationDistances();
                                }
                            }
                        });
                    },
                    
                    GetGrid: function (addressIdentifier)
                    {
                        // validation
                        if (addressIdentifier <= 0)
                        {
                            return;
                        }

                        // variable declaration and assignment
                        var recommendedDistances = null,
                            ns = SEL.Addresses,
                            doms = ns.Dom.Address;

                        // attempt to retrieve recommended distances for address
                        SEL.Data.Ajax({
                            data: { "addressIdentifier": addressIdentifier },
                            methodName: "GetRelatedAddressDistances",
                            serviceName: "svcAddresses",

                            success: function (data)
                            {
                                if (data.hasOwnProperty("d"))
                                {
                                    if (data.d === -999)
                                    {
                                        SEL.MasterPopup.ShowMasterPopup("You do not have permission to view the related distance information.", "Message from " + moduleNameHtml);
                                        return;
                                    }

                                    recommendedDistances = data.d;

                                    if (typeof recommendedDistances === "undefined" || recommendedDistances === null || recommendedDistances.length === 0)
                                    {
                                        $("#" + doms.RecommendedDistances.Destinations).html("<tr class=\"emptymessage\"><td colspan=\"7\" style=\"text-align: center;\">There are no Address Distances to display.</td></tr>");
                                    }
                                    else
                                    {
                                        $("#" + doms.RecommendedDistances.Destinations).append(ns.Address.RecommendedDistances.MakeGridRows(recommendedDistances));
                                    }

                                    $(recommendedDistances).each(function(i, o) {
                                        SEL.Input.Filter.Decimal("outd" + o.DestinationIdentifier);
                                        SEL.Input.Filter.Decimal("retd" + o.DestinationIdentifier);
                                    });

                                    ns.Address.RecommendedDistances.ColouriseGridRows();

                                    ns.Address.Modal.Tabs.RecommendedDistances.Enabled(true);
                                }
                            }
                        });
                    },
                    
                    GetRecommendedDistancesFromGrid: function ()
                    {
                        // variable declaration and assignment
                        var recommendedDistances = [],
                            e = null,
                            destinationIdentifier,
                            outboundIdentifier,
                            outboundDistance,
                            returnIdentifier,
                            returnDistance;

                        $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr").not(".emptymessage").each(function (i, o)
                        {
                            e = $(o);

                            if (!isNaN(parseInt(e.data("di"), 10)) && !isNaN(parseInt(e.data("oi"), 10)) && !isNaN(parseFloat(e.data("ri"), 10)))
                            {
                                destinationIdentifier = parseInt(e.data("di"), 10) < 0 ? 0 : parseInt(e.data("di"), 10);
                                outboundIdentifier = parseInt(e.data("oi"), 10) < 0 ? 0 : parseInt(e.data("oi"), 10);
                                outboundDistance = isNaN(parseFloat(e.find(".addresses-outbound input").val())) ? 0 : parseFloat(e.find(".addresses-outbound input").val());
                                returnIdentifier = parseInt(e.data("ri"), 10) < 0 ? 0 : parseInt(e.data("ri"), 10);
                                returnDistance = isNaN(parseFloat(e.find(".addresses-return input").val())) ? 0 : parseFloat(e.find(".addresses-return input").val());

                                recommendedDistances.push({
                                    "DestinationIdentifier": destinationIdentifier,
                                    "DestinationFriendlyName": "",
                                    "OutboundIdentifier": outboundIdentifier,
                                    "Outbound": outboundDistance,
                                    "OutboundFastest": null,
                                    "OutboundShortest": null,
                                    "ReturnIdentifier": returnIdentifier,
                                    "Return": returnDistance,
                                    "ReturnFastest": null,
                                    "ReturnShortest": null
                                });
                            }
                        });

                        return recommendedDistances;
                    },
                    
                    MakeGridRows: function (recommendedDistances)
                    {
                        /*
                        This is how each recommended distance object should end up being rendered

                            <tr id="dst{DestinationIdentifier}" data-di="{DestinationIdentifier}" data-oi="{OutboundIdentifier}" data-ri="{ReturnIdentifier}">
                                <td>{DestinationFriendlyName}</td>
                                <td class="addresses-outbound">
                                    <input id="outd{DestinationIdentifier}" type="text" maxlength="8" value="{Outbound}" class="validate-decimal" />
                                </td>
                                <td>{OutboundFastest}</td>
                                <td>{OutboundShortest}</td>
                                <td class="addresses-return">
                                    <input id="retd{DestinationIdentifier}" type="text" maxlength="8" value="{Return}" class="validate-decimal" />
                                </td>
                                <td>{ReturnFastest}</td>
                                <td>{ReturnShortest}</td>
                            </tr>
                        */
                        return $.map(recommendedDistances, function (o, i)
                            {
                            return $("<tr id=\"dst" + o.DestinationIdentifier + "\" data-di=\"" + o.DestinationIdentifier + "\" data-oi=\"" + o.OutboundIdentifier + "\" data-ri=\"" + o.ReturnIdentifier + "\"><td>" + o.DestinationFriendlyName + "</td><td class=\"addresses-outbound\"><input id=\"outd" + o.DestinationIdentifier + "\" type=\"text\" maxlength=\"8\" value=\"" + (o.Outbound === 0 ? "" : o.Outbound) + "\" class=\"validate-decimal\"/></td><td>" + (o.OutboundFastest === null ? "" : o.OutboundFastest) + "</td><td>" + (o.OutboundShortest === null ? "" : o.OutboundShortest) + "</td><td class=\"addresses-return\"><input id=\"retd" + o.DestinationIdentifier + "\" type=\"text\" maxlength=\"8\" value=\"" + (o.Return === 0 ? "" : o.Return) + "\" class=\"validate-decimal\"/></td><td>" + (o.ReturnFastest === null ? "" : o.ReturnFastest) + "</td><td>" + (o.ReturnShortest === null ? "" : o.ReturnShortest) + "</td></tr>");
                            });
                    },
                    
                    ColouriseGridRows: function ()
                    {
                        $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr>td").removeClass("row1 row2");
                        $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr:odd>td").addClass("row1");
                        $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr:even>td").addClass("row2");
                    },

                    ClearNewDestinationAddress: function ()
                    {
                        var recommendedDistances = SEL.Addresses.Dom.Address.RecommendedDistances;

                        $("#" + recommendedDistances.NewDestination + " input").not("[type=button]").val("");
                        $("#" + recommendedDistances.DestinationSearch).address("reset");
                    },
                    
                    ClearNewDestinationDistances: function ()
                    {
                        var newDestinationRow = $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.NewDestination);

                        newDestinationRow.data("di", 0);
                        newDestinationRow.data("oi", 0);
                        newDestinationRow.find("input.addresses-outbound").val("");
                        newDestinationRow.find("span.addresses-outboundfastest").empty();
                        newDestinationRow.find("span.addresses-outboundshortest").empty();
                        newDestinationRow.data("ri", 0);
                        newDestinationRow.find("input.addresses-return").val("");
                        newDestinationRow.find("span.addresses-returnfastest").empty();
                        newDestinationRow.find("span.addresses-returnshortest").empty();
                    },
                    
                    ClearAddressDistance: function (deleteButtonElement)
                    {
                        if (typeof deleteButtonElement === "object")
                        {
                            $(deleteButtonElement).prev().val("");
                            $(deleteButtonElement).remove();
                        }
                    },
                    
                    Add: function (bypassWarning)
                    {
                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address,
                            newDistance = [],
                            newDestinationRow = $("#" + doms.RecommendedDistances.NewDestination),
                            destinationIdentifier = $("#" + doms.RecommendedDistances.DestinationSearchIdentifier).val(),
                            outboundDistance,
                            returnDistance,
                            existingDestination;
                        
                        if (typeof bypassWarning !== "boolean")
                        {
                            bypassWarning = false;
                        }

                        if (typeof destinationIdentifier === "undefined" || destinationIdentifier === null || destinationIdentifier.replace(" ", "") === "")
                        {
                            return;
                        }

                        existingDestination = $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr#dst" + destinationIdentifier);

                        if (existingDestination.length === 1 && !bypassWarning && !confirm("This destination already has distances specified with this address, if you continue they will be overwritten. Outbound = " + existingDestination.find(".addresses-outbound input").val() + " and Return = " + existingDestination.find(".addresses-return input").val() + ".\n\nDo you want to continue?"))
                        {
                            return;
                        }

                        outboundDistance = isNaN(parseFloat(newDestinationRow.find("input.addresses-outbound").val())) ? 0 : parseFloat(newDestinationRow.find("input.addresses-outbound").val());
                        returnDistance = isNaN(parseFloat(newDestinationRow.find("input.addresses-return").val())) ? 0 : parseFloat(newDestinationRow.find("input.addresses-return").val());

                        newDistance.push({
                            "DestinationIdentifier": destinationIdentifier,
                            "DestinationFriendlyName": $("#" + doms.RecommendedDistances.DestinationSearch).val(),
                            "OutboundIdentifier": isNaN(parseInt(newDestinationRow.data("oi"), 10)) ? 0 : parseInt(newDestinationRow.data("oi"), 10),
                            "Outbound": outboundDistance,
                            "OutboundFastest": newDestinationRow.find("span.addresses-outboundfastest").text(),
                            "OutboundShortest": newDestinationRow.find("span.addresses-outboundshortest").text(),
                            "ReturnIdentifier": isNaN(parseInt(newDestinationRow.data("ri"), 10)) ? 0 : parseInt(newDestinationRow.data("ri"), 10),
                            "Return": returnDistance,
                            "ReturnFastest": newDestinationRow.find("span.addresses-returnfastest").text(),
                            "ReturnShortest": newDestinationRow.find("span.addresses-returnshortest").text()
                        });

                        if ($("#" + doms.RecommendedDistances.Destinations + " tr.emptymessage").length === 1)
                        {
                            $("#" + doms.RecommendedDistances.Destinations).empty();
                        }

                        existingDestination.remove();
                        $("#" + doms.RecommendedDistances.Destinations).prepend(ns.Address.RecommendedDistances.MakeGridRows(newDistance));
                        
                        SEL.Input.Filter.Decimal("outd" + newDistance[0].DestinationIdentifier); // attach a decimal input characters validator
                        SEL.Input.Filter.Decimal("retd" + newDistance[0].DestinationIdentifier); // attach a decimal input characters validator
                        
                        ns.Address.RecommendedDistances.ColouriseGridRows();

                        ns.Address.RecommendedDistances.ClearNewDestinationAddress();
                        ns.Address.RecommendedDistances.ClearNewDestinationDistances();
                    },

                    CheckAddAndSave: function ()
                    {
                        var ns = SEL.Addresses,
                            doms = ns.Dom.Address,
                            newDestinationRow = $("#" + doms.RecommendedDistances.NewDestination),
                            destinationIdentifier = $("#" + doms.RecommendedDistances.DestinationSearchIdentifier).val(),
                            outboundDistancePopulated,
                            returnDistancePopulated,
                            existingDestination;
                        
                        // check to see if the destination is populated
                        if (typeof destinationIdentifier !== "undefined" && destinationIdentifier !== null && destinationIdentifier.replace(" ", "") !== "")
                        {
                            // check to see if either distance is populated
                            outboundDistancePopulated = !isNaN(parseFloat(newDestinationRow.find("input.addresses-outbound").val()));
                            returnDistancePopulated = !isNaN(parseFloat(newDestinationRow.find("input.addresses-return").val()));
                            
                            if (outboundDistancePopulated || returnDistancePopulated)
                            {
                                // check to see if the destination already has a value, ask to remove if it exists
                                existingDestination = $("#" + SEL.Addresses.Dom.Address.RecommendedDistances.Destinations + ">tr#dst" + destinationIdentifier);
                                
                                if (existingDestination.length === 1)
                                {
                                    if (!confirm("This destination already has distances specified with this address, if you continue they will be overwritten. Outbound = " + existingDestination.find(".addresses-outbound input").val() + " and Return = " + existingDestination.find(".addresses-return input").val() + ".\n\nDo you want to continue?"))
                                    {
                                        return;
                                    }
                                }

                                SEL.Addresses.Address.RecommendedDistances.Add(true);
                            }
                        }
                        
                        SEL.Addresses.Address.Save();
                    },
                    
                    Delete: function (deleteElement, addressDistanceIdentifier)
                    {
                        if (typeof addressDistanceIdentifier === "undefined" || addressDistanceIdentifier === null || isNaN(parseInt(addressDistanceIdentifier, 10)))
                        {
                            return;
                        }
                        
                        if (confirm(SEL.Addresses.Strings.RecommendedDistance.ConfirmDelete))
                        {
                            SEL.Data.Ajax({
                                data: { "addressDistanceIdentifier": addressDistanceIdentifier },
                                methodName: "DeleteAddressDistance",
                                serviceName: "svcAddresses",

                                success: function (data)
                                {
                                    if (data.hasOwnProperty("d") && typeof data.d !== "undefined" && data.d !== null && !isNaN(parseInt(data.d, 10)))
                                    {
                                        if (data.d === -999)
                                        {
                                            SEL.MasterPopup.ShowMasterPopup("You do not have permission to delete this recommended distance.", "Message from " + moduleNameHtml);
                                            return;
                                        }

                                        if (data.d === -1)
                                        {
                                            SEL.MasterPopup.ShowMasterPopup("The Address Distance does not exists.", "Message from " + moduleNameHtml);
                                            return;
                                        }

                                        if (data.d > 0)
                                        {
                                            SEL.Addresses.Address.RecommendedDistances.ClearAddressDistance(deleteElement);
                                        }
                                    }
                                }
                            });
                        }
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
}(moduleNameHTML, appPath));