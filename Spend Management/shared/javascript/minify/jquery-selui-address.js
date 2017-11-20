/*global, $:false */
/*
  Address Picker jQueryUI widget
 
  Usage: 

    A HTML text input and hidden input are needed, as follows:
    
        <input type="text" id="addressText" rel="addressId" />
        <input type="hidden" id="addressId" />

    Then usage is just like any other jQueryUI widget; you can pass options if you don't want the defaults e.g.

        $("#addressText").address({
            enableLabels: false,
            option1: value,
            option2: value
        });

  Options:

    appendTo - Selector of an element which will contain the widget DOM. (default = "body")
    
    backgroundElement - jQuery object of the text field's associated hidden field, this can be omitted by putting the 
    hidden field's ID in the "rel" attribute of the text field. The example above demonstrates this. (default = undefined)
    
    debugging - When true, debugging information will be output to the browser console. (default = false)
    
    delay - After a keystroke, the millisecond delay before a search is performed, subsequent keystrokes reset the delay. (default = 300)
    
    enableFavourites - Enable the "favourites" feature. (default = true)
    
    enableLabels - Enable the "labels" feature. (default = true)
    
    enableAddingAddresses - Allow users to add new addresses. (default = true)
    
    enableHomeAndOffice - Provide home and office address functionality. Adds home and office buttons which act shortcuts to typing the "home" or "office" keyword, also enhances home/office address search results with special icons

    enableManualAddresses - Allow users to see manually added address in the results. (default = true)

    esrAssignmentField - The associated ESR Assignment <select> if applicable, so that when users type their home/office keyword they will see the home/office address in the results (default = undefined)

    minLength - The minimum number of characters required before a search is performed. (default = 3)

  Events (all should be prefixed with "address" when subscribing): 

    matched(event, addressData) - fires when an address has been selected

    notfoundclick(event) - fires when the "Address Not Found?" link is clicked, the default behaviour can be cancelled by returning false

  Notes:

    There is a "global" array $.address.instances[] which holds each instance of this widget
*/
(function ($, undefined) {

    // extend the jQuery menu to prevent the textfield from being given focus as soon as an item is selected
    // this code is lifted from $.ui.menu, see comments with 4 slashes //// for changes.
    $.widget("selui.addressmenu", $.ui.menu, {
        widgetEventPrefix: "addressmenu",
        _create: function () {
            this.activeMenu = this.element;
            this.element
                .uniqueId()
                .addClass("ui-menu ui-widget ui-widget-content ui-corner-all")
                .toggleClass("ui-menu-icons", !!this.element.find(".ui-icon").length)
                .attr({
                    role: this.options.role,
                    tabIndex: 0
                })
                // need to catch all clicks on disabled menu
                // not possible through _on
                .bind("click" + this.eventNamespace, $.proxy(function (event) {
                    if (this.options.disabled) {
                        event.preventDefault();
                    }
                }, this));

            if (this.options.disabled) {
                this.element
                    .addClass("ui-state-disabled")
                    .attr("aria-disabled", "true");
            }

            this._on({
                // Prevent focus from sticking to links inside menu after clicking
                // them (focus should always stay on UL during navigation).
                "mousedown .ui-menu-item > a": function (event) {
                    event.preventDefault();
                },
                "click .ui-state-disabled > a": function (event) {
                    event.preventDefault();
                },
                "click .ui-menu-item:has(a)": function (event) {
                    var target = $(event.target).closest(".ui-menu-item");
                    if (!mouseHandled && target.not(".ui-state-disabled").length) {
                        mouseHandled = true;

                        this.select(event);
                        // Open submenu on click
                        if (target.has(".ui-menu").length) {
                            this.expand(event);
                        } else if (!this.element.is(":focus")) {
                            // Redirect focus to the menu
                            //// SEL - we don't want the text field element to receive focus when the items in the widget menus are clicked
                            //// this.element.trigger("focus", [true]); 

                            // If the active item is on the top level, let it stay active.
                            // Otherwise, blur the active item since it is no longer visible.
                            if (this.active && this.active.parents(".ui-menu").length === 1) {
                                clearTimeout(this.timer);
                            }
                        }
                    }
                },
                "mouseenter .ui-menu-item": function (event) {
                    var target = $(event.currentTarget);
                    // Remove ui-state-active class from siblings of the newly focused menu item
                    // to avoid a jump caused by adjacent elements both having a class with a border
                    target.siblings().children(".ui-state-active").removeClass("ui-state-active");
                    this.focus(event, target);
                },
                mouseleave: "collapseAll",
                "mouseleave .ui-menu": "collapseAll",
                focus: function (event, keepActiveItem) {
                    // If there's already an active item, keep it active
                    // If not, activate the first item
                    var item = this.active || this.element.children(".ui-menu-item").eq(0);

                    if (!keepActiveItem) {
                        this.focus(event, item);
                    }
                },
                blur: function (event) {
                    this._delay(function () {
                        if (!$.contains(this.element[0], this.document[0].activeElement)) {
                            this.collapseAll(event);
                        }
                    });
                }
                // todo disabled keyboard nav as it's interfering with the new label textbox
                //keydown: "_keydown"
            });

            this.refresh();

            // Clicks outside of a menu collapse any open menus
            this._on(this.document, {
                click: function (event) {
                    if (!$(event.target).closest(".ui-menu").length) {
                        this.collapseAll(event);
                    }

                    // Reset the mouseHandled flag
                    mouseHandled = false;
                }
            });
        }
    });

    // another menu with the same behaviour but a different event name is needed for countries
    $.widget("selui.countrymenu", $.selui.addressmenu, {
        widgetEventPrefix: "countrymenu" 
    });

    // "global" address object, keeps a reference to every address widget on the page
    $.address = {
        data: {
            accountFavourites: [],
            accountLabels: [],
            favourites: [],
            labels: [],
            countries: [],
            country: undefined,
            loaded: false,
            loading: false
        },
        getWidgetConfigurationAndData: function () {

            if (!$.address.data.loaded && !$.address.data.loading) {

                $.address.data.loading = true;

                SEL.Data.Ajax({
                    serviceName: "svcAddresses",
                    methodName: "GetWidgetConfigurationAndData",
                    context: this,
                    success: function(response) {
                        var data = response.d;
                        $.address.data = {
                            labels: data.EmployeeLabels,
                            favourites: data.EmployeeFavourites,
                            accountFavourites: data.AWFavourites,
                            accountLabels: data.AWLabels,
                            countries: data.Countries,
                            homeAddresses: data.HomeAddresses,
                            officeAddresses: data.OfficeAddresses,
                            loaded: true,
                            loading: false
                        };

                        // options shared by all widgets on a page
                        $.address.options = {
                            DefaultCountryAlpha3Code: data.DefaultCountryAlpha3Code,
                            PostcodeAnywhereKey: data.PostcodeAnywhereKey,
                            MandatoryPostcodeForAddresses: data.MandatoryPostcodeForAddresses,
                            AllowMultipleWorkAddresses: data.AllowMultipleWorkAddresses
                        };

                        // set every widget to the default country, any address widgets created later will pick up this value themselves
                        $.each($.address.instances, function(index, widget) {
                            widget.switchCountry(data.DefaultCountryAlpha3Code);
                        });

                        // standardise property names, todo improve this
                        for (var i = 0; i < $.address.data.accountLabels.length; i++) {
                            var accountLabel = $.address.data.accountLabels[i];
                            accountLabel.Id = accountLabel.GlobalIdentifier;
                            accountLabel.Identifier = accountLabel.AddressID;
                            accountLabel.IsRetrievable = true;
                        }

                        for (i = 0; i < $.address.data.labels.length; i += 1) {
                            var label = $.address.data.labels[i];
                            label.Id = label.GlobalIdentifier;
                            label.Identifier = label.AddressID;
                            label.IsRetrievable = true;
                        }

                        for (i = 0; i < $.address.data.favourites.length; i += 1) {
                            var favourite = $.address.data.favourites[i];
                            favourite.Id = favourite.GlobalIdentifier;
                            favourite.Identifier = favourite.AddressID;
                            favourite.IsRetrievable = true;
                        }

                        for (i = 0; i < $.address.data.accountFavourites.length; i += 1) {
                            var accountFavourite = $.address.data.accountFavourites[i];
                            accountFavourite.Id = accountFavourite.GlobalIdentifier;
                            accountFavourite.Identifier = accountFavourite.AddressID;
                            accountFavourite.IsRetrievable = true;
                        }
                    }
                });
            }
        },
        instances: [],
        options: {
            DefaultCountryAlpha3Code: undefined,
            PostcodeAnywhereKey: undefined,
            MandatoryPostcodeForAddresses: undefined,
            AllowMultipleWorkAddresses: undefined,
            SearchWithOfficeKeywordOrIcon: false
        },
        rebind: function () {
            var instance;
            for (var i = 0; i < this.instances.length; i += 1) {
                instance = this.instances[i];
                instance._rebind.call(instance);
            }
        }
    };

    var selWebServicePrefix = "/shared/webServices/svcAddresses.asmx/";
    var pcaWebServicePrefix = "//services.postcodeanywhere.co.uk/CapturePlus/Interactive/";

    // jQueryUI widget
    $.widget("selui.address", {
        widgetEventPrefix: "address",
        // default options, any of which can be overridden
        options: {
            // the element to add the widget to
            appendTo: "body",
            // a reference to the <input type="hidden"> element where a selected address' AddressID will be populated
            backgroundElement: undefined,
            // the default country, if undefined $.address.options.DefaultCountryAlpha3Code will be used
            country: $.address.options.DefaultCountryAlpha3Code,
            // output useful stuff to the console
            debugging: false,
            // number of milliseconds to wait after the last keystroke before performing a search
            delay: 500,
            // a reference to the element that this widget relates to
            elementId: undefined,
            // enable the creation of manually added addresses
            enableAddingAddresses: true,
            // enable manual addresses
            enableManualAddresses: true,
            // enable the labels feature
            enableLabels: true,
            // display account-wide labels
            enableAccountWideLabels: true,
            // enable the favourites feature
            enableFavourites: true,
            // display account-wide favourites
            enableAccountWideFavourites: true,
            // allow for quick selection of home / office addresses and enchancement of address search results which are home/office addresses
            enableHomeAndOffice: false,
            // the associated ESR Assignment <select> field, a jQuery object
            esrAssignmentField: undefined,
            // force users to provide an address name before they can use the address they select
            forceAddressNameEntry: false,
            // minimum number of characters required before a search will be performed
            minLength: 3,
            // the position of the widget relative to the target element, "of" can't be set yet, see _create()
            position: {
                my: "left top",
                at: "left bottom",
                of: null,
                collision: "none"
            },
            // web service URIs
            sources: {
                postcodeAnywhereAutocomplete: pcaWebServicePrefix + "AutoComplete/v2.00/json3.ws?",
                postcodeAnywhereAutocompleteFind: pcaWebServicePrefix + "AutoCompleteFind/v2.00/json3.ws?",
                postcodeAnywhereRetrieveById: pcaWebServicePrefix + "RetrieveById/v2.00/json3.ws?",
                selSaveNewAddress: selWebServicePrefix + "Save",
                selFetchCapturePlusAddress: selWebServicePrefix + "GetByCapturePlusId",
                selFetchSelAddress: selWebServicePrefix + "Get",
                selAutocomplete: selWebServicePrefix + "AutocompleteForWeb",
                selAutocompleteByAccount: selWebServicePrefix + "AutocompleteByAccount",
                selSaveAddressWithName: selWebServicePrefix + "GetByCapturePlusIdAndCreateManualAddress"
            },
            // localizable strings
            strings: {
                startMessage: "<p>Type the first {minLength} characters of your search term to see your results here.</p>",
                noResultsMessage: "<p>No addresses could be found.</p>",
                noFavouritesMessage: "<p>There are no address favourites to display.</p>",
                noLabelsMessage: "<p>There are no address labels to display.</p>",
                createLink: "Can't find what you're looking for?",
                createMessage: "Our address database should contain all addresses, but sometimes new addresses take a while to appear. You can use this form to create the address you need, <strong>the address will be created in the country you are searching</strong>.",
                createFailedDuplicate: "This address could not be saved because one already exists with the same information.",
                createFailedBadPostcode: "This address could not be saved because the postcode you entered is not valid.",
                createFailedValidation: "Mandatory fields (marked with a *) must be completed.",
                createFailedNoAccessRole: "You do not have permission to create new addresses.",
                formCancelButton: "", // button text is in the image
                formSaveButton: "", // button text is in the image
                labelCreateButton: "Label this adddress",
                labelCreateSaveButton: "Save",
                labelCreateCancelButton: "Cancel",
                labelDeleteButton: "Remove label",
                labelCreateFailedDuplicateText: "A label with this name already exists.",
                labelCreateFailedDuplicateAddress: "A label already exists for this address.",
                labelCreateFailedValidation: "A label must contain at least 3 characters.",
                labelCreateFailedKeyword: "This text cannot be used for a label.",
                favouriteSaveButton: "Favourite this address",
                favouriteDeleteButton: "Remove favourite",
                tabSearchButton: "Search",
                tabFavouritesButton: "Favourites",
                tabLabelsButton: "Labels",
                tabCountriesButton: "Switch country",
                homeButton: "Use your home address (if available)",
                officeButton: "Use your work address (if available)",
                listItemStandard: "Standard address",
                listItemFavourite: "Favourite address",
                listItemLabel: "Labelled address",
                listItemAccountFavourite: "Account favourite address",
                listItemManuallyAddedAddress: "Manually added address",
                addressNameEntryMessage: "You must specify a name for this address before you can use it, please take care to use a meaningful name that other claimants and approvers will recognise. Note that this address will be available to all users so avoid using a name containing any sensitive information which could be used to identify someone.",
                pcaSearchFailedMessage: "Unable to retrieve all addresses.",
                pcaSearchFailedUri: "http://knowledge.software-europe.com/articles/Frequently_Asked_Question/Why-am-I-unable-to-retrieve-all-addresses"
            }
        },

        /* private methods */
        // adds event handlers to the widget's associated <input> element
        _addElementEventBindings: function () {
            // field element events
            this._on(this.element, {
                "focus": function () {
                    this._display();
                },
                "click": function () {
                    this._display();
                },
                "keydown": function (event) {
                    this._display();

                    var keyCodes = $.ui.keyCode;

                    switch (event.keyCode) {
                        case keyCodes.ESCAPE:
                            this.close();
                            break;
                        case keyCodes.TAB:
                            // if the user has a search result highlighted and that item is either a retrievable PCA address or a SEL address
                            // then populate the field with that item, otherwise empty the field
                            if (this.panel.is(':visible') && this.searchMenu.active) {
                                var itemData = this.searchMenu.active.data("item.sel-address");
                                if (typeof(itemData.IsRetrievable) === "undefined" || itemData.IsRetrievable === true) {
                                    this.searchMenu.select(event);
                                }
                            }
                            break;
                        case keyCodes.DOWN:
                            this._move("next", event);
                            break;
                        case keyCodes.UP:
                            this._move("previous", event);
                            break;
                        case keyCodes.PAGE_DOWN:
                            this._move("nextPage", event);
                            break;
                        case keyCodes.PAGE_UP:
                            this._move("previousPage", event);
                            break;
                        case keyCodes.ENTER:
                        case keyCodes.NUMPAD_ENTER:
                            if (this.panel.is(':visible') && this.searchMenu.active) {
                                event.preventDefault();
                                this.searchMenu.select(event);
                            }
                            break;
                        default:
                            clearTimeout(this.searching);
                            this.searching = setTimeout($.proxy(function () {
                                this.search();
                            }, this), this.options.delay);
                            break;
                    }

                },
                "blur": function (event) {
                    this.debug("blur fired on element");

                    // clicks on the panel would cause the field's blur event to fire, causing the panel to close. 
                    // see the panel's mousedown/mouseup handlers 
                    if (this.cancelFieldBlur !== true) {
                        this.close();

                        if (this.options.enableHomeAndOffice) {
                            // attempt to translate home or office keywords
                            SEL.AddressesAndTravel.PopulateAddressPickerByKeyword(this.element);
                        }
                    }

                    // if the address isn't matched attempt to resolve the search text to a personal/account label
                    if (this.options.enableLabels && !this.addressMatched) {

                        // convenience function for when a label is matched to the search term 
                        var onLabelMatched = function (matchedLabel) {
                            
                            if (matchedLabel.Id) {

                                // lookup the label's address
                                this._getAddressByPcaId({
                                    pcaId: matchedLabel.Id,
                                    labelId: matchedLabel.AddresslabelID,
                                    success: function (data) {
                                        this.debug("label text matched", data);
                                        this._trigger("matched", event, data);

                                        this._value(data.d.FriendlyName, data.d.Identifier);
                                    }
                                });
                            } else {

                                // lookup the label's manual address
                                this._getAddressBySelId({
                                    selId: matchedLabel.Identifier,
                                    labelId: matchedLabel.AddresslabelID,
                                    success: function (data) {
                                        this.debug("label text matched", data);
                                        this._trigger("matched", event, data);

                                        this._value(data.d.FriendlyName, data.d.Identifier);
                                    }
                                });

                            }
                        };

                        for (var i = 0; i < $.address.data.labels.length; i += 1) {
                            var label = $.address.data.labels[i];
                            
                            if (this.element.val().toLowerCase() === label.Text.toLowerCase()) {

                                onLabelMatched.call(this, label);
                                return;
                            }
                        }

                        for (i = 0; i < $.address.data.accountLabels.length; i += 1) {
                            label = $.address.data.accountLabels[i];

                            if (this.element.val().toLowerCase() === label.Text.toLowerCase()) {

                                onLabelMatched.call(this, label);
                                return;

                            }
                        }
                    }
                    
                    if (this.cancelFieldBlur !== true && !this.addressMatched) {
                        if (this.backgroundElement.val() !== "-100") { // this is in a not -100 check to ensure we don't wipe the text box for addresses unable to be matched from mobile
                            this.backgroundElement.val("");
                            this.element.val("");
                        }
                    }
                }
            });
        },

        // adds event handlers to the window, and the widget's own DOM elements
        _addEventBindings: function () {

            // window events
            this._on($(document), {
                "click": function (event) {
                    // if the widget is stealing focus from the <input> element, and the click was outside of that element and outside of the widget
                    if (this.hasFocus === true && this.element.is(event.target) === false && this.panel.has(event.target).length === 0) {
                        this.close();
                    }
                }
            });

            // widget's panel events
            this._on(this.panel, {
                "mousedown": function () {
                    this.debug("panel mousedown", arguments);

                    // interrupts the timeout setup by the blur event of the field
                    this.cancelFieldBlur = true;
                },
                "mouseup": function () {
                    this.debug("panel mouseup", arguments);

                    // allows the field element to blur
                    this.cancelFieldBlur = false;

                    // set focus on the field element, unless focus is needed somewhere (an <input>) within the widget
                    if (this.panel.find("input:focus").length === 0) {
                        this.element.focus();
                    }
                }
            });

            // tab button events
            this._on(this.searchTab, { "click": function () { this._switchPanel(this.searchPanel); } });

            if (this.options.enableLabels || this.options.enableAccountWideLabels) {
                this._on(this.labelsTab, {
                    "click": function () {
                        this._switchPanel(this.labelsPanel);
                    }
                });
            }

            if (this.options.enableFavourites || this.options.enableAccountWideFavourites) {
                this._on(this.favouritesTab, { "click": function () { this._switchPanel(this.favouritesPanel); } });
            }

            this._on(this.countriesTab, { "click": function () { this._switchPanel(this.countriesPanel); } });

            if (this.options.enableAddingAddresses) {
                // "can't find what you're looking for" link events
                this._on(this.newAddressMessage, {
                    "click": function (event) {
                        if (this._trigger("notfoundclick", event) === false) return;
                        
                        this._switchPanel(this.newAddressPanel);
                    }
                });
            }
          
            // menu events
            this._on(this.panel.find(".ui-menu"), {
                "addressmenuselect": function (event, ui) {
                    this._searchMenuItemSelected(event, ui);
                },
                "addressmenufocus": function (event, data) {
                    var listElement = data.item;

                    // old IE seems to have an issue whereby clicking in an empty menu incorrectly fires this event;
                    if (!listElement.data() || $.isEmptyObject(listElement.data())) return;

                    clearTimeout(this.searchMenuBlurTimer);

                    var addressData = listElement.data()["item.selAddress"];
                    var showContextButtons = ((addressData.IsRetrievable || addressData.Identifier) && !listElement.hasClass("home-address") && !listElement.hasClass("office-address"));

                    var listElementIsLabel = listElement.hasClass("personal-label") || listElement.hasClass("account-label");
                    var favouritesPanelVisible = (this.panel.find(".ui-sel-favourites:visible").length > 0);

                    // show the appropriate label button (show neither if the new label input container is present, or we're inside the favourites list)
                    if (this.options.enableLabels) {
                        if (listElement.find(".sel-input-label-add").length) {
                            showContextButtons = false;
                        } else {
                            var showAddLabelButton = (!listElementIsLabel);
                            this.addLabelButton.toggle(showAddLabelButton && !favouritesPanelVisible);
                            this.deleteLabelButton.toggle(!showAddLabelButton && !favouritesPanelVisible && !listElement.hasClass("account-label"));
                        }
                    }

                    // show the appropriate favourite button, hide both if the list item is a label or account favourite
                    var listElementIsAccountFavourite = listElement.hasClass("account-favourite");
                    if (this.options.enableFavourites) {
                        var showAddFavouriteButton = (!listElement.hasClass("personal-favourite"));
                        this.addFavouriteButton.toggle(showAddFavouriteButton && !listElementIsAccountFavourite && !listElementIsLabel);
                        this.deleteFavouriteButton.toggle(!showAddFavouriteButton && !listElementIsAccountFavourite && !listElementIsLabel);
                    }

                    // hide or show the context buttons container 
                    if (showContextButtons) {
                        this.contextButtons
                            .show()
                            .zIndex(listElement.zIndex() + 1)
                            .position({
                                my: "right top",
                                at: "right top",
                                of: listElement,
                                collision: "none"
                            });
                    } else {
                        this.contextButtons.hide();
                    }
                    
                    this.contextButtons.currentElement = listElement;
                },
                "addressmenublur": function () {
                    this.searchMenuBlurTimer = setTimeout($.proxy(function () {
                        this.contextButtons.hide();
                    }, this), 13);
                },
                "click .sel-input-label-add": function (event) {
                    // we're inside the new label container element, so prevent the click from selecting the address
                    event.preventDefault();

                    if ($(event.target).hasClass("save")) {

                        if (this.contextButtons.currentNewLabelElement) {
                            var labelText = $.trim(this.contextButtons.currentNewLabelElement.find("input").val());
                            var strings = this.options.strings;
                            
                            var showError = function (message) {
                                this.addLabelValidationMarker.show();
                                this.addLabelMessage.html(message);
                            };

                            if (labelText.length < 3) {
                                showError.call(this, strings.labelCreateFailedValidation);
                                
                            } else {
                                var listElement = this.contextButtons.currentElement;
                                var addressData = listElement.data()["item.selAddress"];
                                var friendlyAddressText = addressData.FriendlyName || (addressData.Match || "") + addressData.Suggestion;

                                SEL.Data.Ajax({
                                    serviceName: "svcAddresses",
                                    methodName: "SaveLabel",
                                    data: {
                                        identifier: addressData.Identifier || addressData.Id,
                                        labelText: labelText
                                    },
                                    context: this,
                                    success: function (data) {
                                        var response = data.d;

                                        switch (response) {
                                            case -1: // duped label text
                                                showError.call(this, strings.labelCreateFailedDuplicateText);
                                                break;
                                            case -2: // dupe address, should never occur
                                                showError.call(this, strings.labelCreateFailedDuplicateAddress);
                                                break;
                                            case -3: // reserved word, should never occur
                                                showError.call(this, strings.labelCreateFailedKeyword);
                                                break;
                                            default:
                                                this._hideNewLabelForm();

                                                // success
                                                listElement.addClass("personal-label")
                                                    .children("a").html(labelText).attr("title", friendlyAddressText);

                                                // add a label id to the address data object
                                                addressData.AddressLabelID = response;

                                                // add the new label to the global collection
                                                $.address.data.labels.push({
                                                    GlobalIdentifier: addressData.Id,
                                                    Id: addressData.Id,
                                                    AddressID: addressData.Identifier,
                                                    Identifier: addressData.Identifier,
                                                    AddressLabelID: response,
                                                    Text: labelText,
                                                    AddressFriendlyName: friendlyAddressText,
                                                    IsRetrievable: true
                                                });

                                                // sort the labels alphabetically
                                                $.address.data.labels.sort(function(a, b) {
                                                    return a.Text.localeCompare(b.Text);
                                                });

                                                break;
                                        }

                                    }
                                });
                            }
                        }

                    }

                    if ($(event.target).hasClass("cancel")) {
                        this._hideNewLabelForm();
                        this.contextButtons.show();
                    }

                    return false;
                },
                "mousemove li>a": function (event) {
                    // display "tooltips" (title attr) for the icon to the left of each search result.
                    // the icon is a background image on the list item, so need to calculate if the mouse is over the icon or not
                    var listElement = $(event.target).parent();
                    // reset the title attr
                    listElement.attr("title", "");

                    // if the cursor is within 18 pixels of the left edge of the item, then that's considered to be hovering over the icon
                    var mouseIsOverIcon = (event.pageX - listElement.offset().left < 18);

                    if (mouseIsOverIcon) {
                        var strings = this.options.strings;
                        var tooltip = strings.listItemStandard;

                        if (listElement.hasClass("personal-favourite")) {
                            tooltip = strings.listItemFavourite;
                        } else if (listElement.hasClass("account-favourite")) {
                            tooltip = strings.listItemAccountFavourite;
                        } else if (listElement.hasClass("personal-label")) {
                            tooltip = strings.listItemLabel;
                        } else if (listElement.hasClass("manual-address")) {
                            tooltip = strings.listItemManuallyAddedAddress;
                        } else if (listElement.hasClass("not-retrievable")) {
                            tooltip = strings.listItemNotRetrievable;
                        } else if (listElement.hasClass("sel-country")) {
                            tooltip = "";
                        }
                        
                        // apply the tooltip
                        listElement.attr("title", tooltip);
                    }

                },
                "scroll": function () {
                    this.cancelFieldBlur = false;
                    this.element.focus();

                    this.contextButtons.hide();
                },
                "countrymenuselect": function (event, ui) {
                    var item = ui.item.data("item.selCountry");
                    this.switchCountry(item.Alpha3CountryCode);
                    
                    this._switchPanel(this.searchPanel);

                    // force the search to fire again
                    this.search(true);
                }
            });

            // home and office buttons
            if (this.options.enableHomeAndOffice) {
                this._on(this.homeButton, {
                    "click": function () {
                        this.value("home", "");
                        this.element.closest("td").next().children("input[type=text]").focus();
                    }
                });

                this._on(this.officeButton, {
                    "click": function () {
                        this.value("office", "");
                        this._search("office");
                        this.element.focus();
                    }
                });

                this._on(this.homeButtonLarge, {
                    "click": function () {
                        this.value("home", "");
                        this.element.closest("td").next().children("input[type=text]").focus();
                    }
                });

                this._on(this.officeButtonLarge, {
                    "click": function () {
                        this.value("office", "");
                        this._search("office");
                        this.element.focus();
                    }
                });
            }

            // contextual buttons
            this._on(this.contextButtons, {
                "mouseover": function (event) {
                    var listElement = this.contextButtons.currentElement;

                    listElement.trigger("mouseover");
                    var buttonsElement = $(event.currentTarget);
                }
            });

            // todo refactor into methods
            this._on(this.contextButtons.children(), {
                "mousedown": function(event) {
                    event.preventDefault();
                },
                "click": function (event) {
                    var button = $(event.currentTarget);
                    var listElement = this.contextButtons.currentElement;
                    var addressData = listElement.data()["item.selAddress"];
                    
                    // clicked the "add label" contextual button show the new label container
                    if (button.hasClass("sel-button-label-add")) {

                        this.contextButtons.hide();

                        var element = this.contextButtons.currentElement.children("a");

                        // prevent the address field from stealing focus back from the input element
                        this.cancelFieldBlur = true;
                        this.hasFocus = true;
                        
                        // append the new label container to the list item and give focus to the input element
                        this.addLabelForm.show().appendTo(element)
                                            .children("input").val("").focus();

                        this.addLabelMessage.empty();
                        this.addLabelValidationMarker.hide();
                        
                       this.contextButtons.currentNewLabelElement = element;
                    }

                    // clicked the "delete label" contextual button
                    if (button.hasClass("sel-button-label-delete")) {

                        SEL.Data.Ajax({
                            serviceName: "svcAddresses",
                            methodName: "DeleteLabel",
                            data: {
	                            addressLabelId: addressData.AddressLabelID
                            },
                            context: this,
                            success: function(data) {
                                var response = data.d;

                                if (response === 0) {
                                    this.debug("deleted label " + addressData.AddressLabelID, response);

                                    var labelData;
                                    // remove the label data from the global collection
                                    for (var i = 0; i < $.address.data.labels.length; i += 1) {
                                        labelData = $.address.data.labels[i];
                                        if (labelData.AddressLabelID === addressData.AddressLabelID) {
                                            $.address.data.labels.splice(i, 1);
                                            break;
                                        }
                                    }

                                    // the list element for the label needs restoring to a normal search result, or removing if it's been added the list
                                    if (typeof (addressData.Text) !== "undefined") {
                                        // list element is just a label, drop it
                                        listElement.remove();
                                        
                                        // there might be an orphaned label element in the search list, update or remove it 
                                        if (!this.panel.find(".ui-sel-search:visible").length) {
                                            $.each(this.panel.find(".ui-sel-search li.personal-label"), function (itemIndex, item) {
                                                var searchListElement = $(item);
                                                var searchListElementData = searchListElement.data()["item.selAddress"];
                                                
                                                if (searchListElementData.AddressLabelID === addressData.AddressLabelID) {
                                                    
                                                    if (typeof (searchListElementData.Text) !== "undefined") {
                                                        // list element is just a label, drop it
                                                        searchListElement.remove();
                                                    } else {
                                                        // enhanced PostcodeAnywhere result, restore it to a standard item
                                                        searchListElement
                                                            .removeClass("personal-label")
                                                            .children("a").text(searchListElementData.FriendlyName || (searchListElementData.Match || "") + searchListElementData.Suggestion);
                                                    }
                                                    // break $.each
                                                    return false;
                                                }
                                            });
                                        }
                                        
                                    } else {
                                        // enhanced PostcodeAnywhere result, restore it to a standard item
                                        listElement.removeClass("personal-label")
                                                   .children("a")
                                                   .text(addressData.FriendlyName || (addressData.Match || "") + addressData.Suggestion);
                                    }

                                    this.contextButtons.hide();

                                    // the last label might have just been removed so display the "no labels" message
                                    if (!$.address.data.labels.length && !$.address.data.accountLabels.length) {
                                        this.noLabelsPanel.show();
                                    }
                                } 
                                // todo handle other response codes
                            }
                        });
                    }

                    // clicked on one of the favourite buttons
                    if (button.attr("class").indexOf("sel-button-favourite-") !== 1) {
                        if (button.hasClass("sel-button-favourite-add")) {
                            SEL.Data.Ajax({
                                serviceName: "svcAddresses",
                                methodName: "saveFavourite",
                                data: {
                                    identifier: addressData.Identifier || addressData.Id
                                },
                                context: this,
                                success: function (data) {
                                    var response = data.d;

                                    if (response > 0) {
                                        listElement.addClass("personal-favourite");
                                        this.addFavouriteButton.hide();
                                        this.deleteFavouriteButton.show();

                                        // add a favourite id to the address data object
                                        addressData.FavouriteID = response;
                                        
                                        // update the global favourites list
                                        $.address.data.favourites.push({
                                            FavouriteID: response,
                                            IsRetrievable: true,
                                            GlobalIdentifier: addressData.Id,
                                            Id: addressData.Id,
                                            AddressID: addressData.Identifier,
                                            Identifier: addressData.Identifier,
                                            AddressFriendlyName: addressData.FriendlyName || (addressData.Match || "") + addressData.Suggestion
                                        });
                                        
                                        // this might have occured on the favourites panel, so we need to check for any current search results that need marking as favourite
                                        if (!this.panel.find(".ui-sel-search:visible").length) {
                                            $.each(this.panel.find(".ui-sel-search li.sel-suggestion"), function (itemIndex, item) {
                                                var searchListElement = $(item);
                                                var searchListElementData = searchListElement.data()["item.selAddress"];
                                                
                                                if (searchListElementData.Id === addressData.GlobalIdentifier) {
                                                    searchListElement.addClass("personal-favourite");
                                                    // break $.each
                                                    return false;
                                                }
                                            });
                                        }
                                    }
                                    // todo handle other response codes
                                }
                            });
                        }

                        if (button.hasClass("sel-button-favourite-delete")) {
                            SEL.Data.Ajax({
                                serviceName: "svcAddresses",
                                methodName: "deleteFavourite",
                                data: {
                                    addressFavouriteId: addressData.FavouriteID
                                },
                                context: this,
                                success: function (data) {
                                    var response = data.d;

                                    if (response === 1) {
                                        // remove the favourite from the global collection
                                        for (var i = 0; i < $.address.data.favourites.length; i += 1) {
                                            var favouriteData = $.address.data.favourites[i];
                                            if (favouriteData.FavouriteID === addressData.FavouriteID) {
                                                $.address.data.favourites.splice(i, 1);
                                                break;
                                            }
                                        }

                                        listElement.removeClass("personal-favourite");
                                        
                                        // there might be a search result that's marked as a favourite, if so restore it to a standard item
                                        if (!this.panel.find(".ui-sel-search:visible").length) {
                                            $.each(this.panel.find(".ui-sel-search li.personal-favourite"), function (itemIndex, item) {
                                                var searchListElement = $(item);
                                                var searchListElementData = searchListElement.data()["item.selAddress"];

                                                if (searchListElementData.FavouriteID === addressData.FavouriteID) {
                                                    searchListElement.removeClass("personal-favourite");
                                                    // break $.each
                                                    return false;
                                                }
                                            });
                                        }

                                        this.addFavouriteButton.show();
                                        this.deleteFavouriteButton.hide();
                                    }
                                    
                                    // todo handle other response codes
                                }
                            });

                        }

                    }

                    return false;
                }
            });

            // window resize events (handled differently to work around IE7s resize event weirdness)
            // The input element might move if the page is resized or zoomed (modals are fixed to the center of the viewport) 
            // We need to keep the widget positioned relative to the element.
            // todo: improve, not always required - wasteful
            var onWindowResize = function () {

                // IE7 / Compat mode fire multiple resize events, only handle the first via "one".
                $(window).one("resize", $.proxy(function () {
                    setTimeout($.proxy(function () {
                        // reposition the element
                        this._position();
                        // when complete, set this up again for next time
                        onWindowResize.call(this);
                    }, this), 6);
                }, this));

            };
            // set the listener up for the first resize event
            onWindowResize.call(this);

            // window scroll events
            // The input element might move if the page is scrolled (modals are fixed to the center of the viewport) 
            // We need to keep the widget positioned relative to the element.
            // todo: this sucks, improve it 
            $(window).on("scroll", $.proxy(function () {
                // reposition the element
                //this._position();
            }, this));
        },

        // create all necessary DOM elements for the widget's "new address" panel
        _buildNewAddressPanel: function () {
            var panel = this.newAddressPanel;
            if (panel.built !== true) {
                var strings = this.options.strings;

                panel.form = $("<form>");
                
                var instructions = $("<p>")
                    .addClass("ui-sel-instructions")
                    .html(strings.createMessage);

                var fields = [];
                fields.push(this._createField.call(this, "addressName", "Name", 250));
                fields.push(this._createField.call(this, "line1", "Line 1", 256, true));
                fields.push(this._createField.call(this, "line2", "Line 2", 256));
                fields.push(this._createField.call(this, "city", "City", 256));
                fields.push(this._createField.call(this, "county", "County", 256));
                fields.push(this._createField.call(this, "postcode", "Postcode", 32, $.address.options.MandatoryPostcodeForAddresses));

                var buttons = $("<div>")
                    .addClass("buttons");

                panel.form.cancelButton = $("<input>")
                    .attr("type", "button")
                    .attr("value", strings.createCancelButton)
                    .addClass("form-button cancel-button")
                    .appendTo(buttons);

                panel.form.saveButton = $("<input>")
                    .attr("type", "submit")
                    .attr("value", strings.createSaveButton)
                    .addClass("form-button save-button")
                    .appendTo(buttons);

                panel.form.append(instructions, fields, buttons);
                panel.append(panel.form);

                this._on(panel.form.cancelButton, {
                    "click": function () {
                        this._switchPanel(this.searchPanel);
                        this.hasFocus = true;
                        return false;
                    }
                });
                
                this._on(panel.form, {
                    "submit": function (event) {
                        event.preventDefault();
                        this._saveNewAddress();
                        return false;
                    }
                });

                this._on(panel.form.find("input[type=text]"), {
                    "focus": function (event) {
                        event.stopPropagation();
                        this.hasFocus = true;
                    }
                });

                panel.built = true;
            }
        },

        // create all necessary DOM elements for the widget and set up event handlers
        _buildWidgetDom: function () {
            // ensure the browser doesn't display its own autocomplete for the field
            this.element.attr("autocomplete", "off");

            var strings = this.options.strings;

            // outer elements
            this.panel = $("<div>")
                .addClass("ui-sel-address ui-sel-wrapper")
                .appendTo($(this.options.appendTo))
                .zIndex(this.element.zIndex() + 1)
                .hide();

            this.tabContainer = $("<div>")
                .addClass("ui-sel-tab-container ui-widget ui-widget-content ui-corner-bottom")
                .appendTo(this.panel);

            this.startPanel = $("<div>")
                .addClass("ui-sel-tab-panel ui-sel-start")
                .appendTo(this.tabContainer)
                .html(strings.startMessage);

            this.loadingIndicator = $("<div>")
                .addClass("ui-sel-loading")
                .css({ display: "none" })
                .appendTo(this.tabContainer);

            // search tab panel elements
            this.searchPanel = $("<div>")
                .addClass("ui-sel-tab-panel ui-sel-search")
                .css("display", "none")
                .appendTo(this.tabContainer);

            this.searchMenu = $("<ul>")
                .addressmenu()
                .appendTo(this.searchPanel)
                // .data means that this.searchMenu represents the JqueryUI menu's object, rather than just its DOM
                .data("addressmenu");

            this.noSearchResultsPanel = $("<div>")
                .addClass("ui-sel-tab-panel ui-sel-no-results")
                .css("display", "none")
                .html(strings.noResultsMessage)
                .appendTo(this.searchPanel);

            // tabs container
            this.tabs = $("<ul>")
                .addClass("ui-sel-tabs-nav")
                .appendTo(this.panel);

            // the search tabs
            this.searchTab = $("<li>")
                .addClass("ui-sel-tab-search")
                .attr("title", strings.tabSearchButton)
                .appendTo(this.tabs);

            // the row context buttons 
            this.contextButtons = $("<div>")
                .addClass("sel-context-buttons");

            // favourites tab panel elements
            if (this.options.enableFavourites || this.options.enableAccountWideFavourites) {
                this.favouritesPanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-favourites")
                    .css("display", "none")
                    .appendTo(this.tabContainer);

                this.favouritesMenu = $("<ul>")
                    .addressmenu()
                    .appendTo(this.favouritesPanel)
                    // .data means that this.searchMenu represents the JqueryUI menu's object, rather than just its DOM
                    .data("addressmenu");
                
                this.noFavouritesPanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-no-results")
                    .html(strings.noFavouritesMessage)
                    .appendTo(this.favouritesPanel);

                this.favouritesTab = $("<li>")
                    .addClass("ui-sel-tab-favourites")
                    .attr("title", strings.tabFavouritesButton)
                    .appendTo(this.tabs);

                if (this.options.enableFavourites) {
                    this.addFavouriteButton = $("<span>")
                        .addClass("sel-button-favourite-add")
                        .attr("title", strings.favouriteSaveButton)
                        .appendTo(this.contextButtons);

                    this.deleteFavouriteButton = $("<span>")
                        .addClass("sel-button-favourite-delete")
                        .attr("title", strings.favouriteDeleteButton)
                        .appendTo(this.contextButtons);
                }
            }

            // labels tab panel elements
            if (this.options.enableLabels || this.options.enableAccountWideLabels) {
                this.labelsPanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-labels")
                    .css("display", "none")
                    .appendTo(this.tabContainer);

                this.labelsMenu = $("<ul>")
                    .addressmenu()
                    .appendTo(this.labelsPanel)
                    // .data means that this.searchMenu represents the JqueryUI menu's object, rather than just its DOM
                    .data("addressmenu");
                
                this.noLabelsPanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-no-results")
                    .html(strings.noLabelsMessage)
                    .appendTo(this.labelsPanel);

                this.labelsTab = $("<li>")
                    .addClass("ui-sel-tab-labels")
                    .attr("title", strings.tabLabelsButton)
                    .appendTo(this.tabs);

                if (this.options.enableLabels) {
                    this.addLabelButton = $("<span>")
                        .addClass("sel-button-label-add")
                        .attr("title", strings.labelCreateButton)
                        .appendTo(this.contextButtons);

                    this.deleteLabelButton = $("<span>")
                        .addClass("sel-button-label-delete")
                        .attr("title", strings.labelDeleteButton)
                        .appendTo(this.contextButtons);

                    this.addLabelSaveButton = $("<div>")
                        .addClass("ui-sel-button save")
                        .attr("title", strings.labelCreateSaveButton);

                    this.addLabelCancelButton = $("<div>")
                        .attr("title", strings.labelCreateCancelButton)
                        .addClass("ui-sel-button cancel");

                    this.addLabelValidationMarker = $("<div>")
                        .addClass("ui-sel-invalid")
                        .html("*")
                        .hide();

                    this.addLabelMessage = $("<div>")
                        .addClass("ui-sel-error");

                    this.addLabelForm = $("<form>")
                        .addClass("sel-input-label-add")
                        .append($("<label>").html("Label*"),
                                $("<input>").attr("maxlength", "50"),
                                this.addLabelValidationMarker,
                                this.addLabelSaveButton,
                                this.addLabelCancelButton,
                                this.addLabelMessage)
                        .appendTo(this.searchPanel);

                    // handle the user pressing Enter
                    this._on(this.addLabelForm, {
                        "submit": function (event) {
                            event.preventDefault();
                            this.addLabelSaveButton.trigger("click", [this.addLabelSaveButton]);
                            return false;
                        } 
                    });
                }
            }

            // enforced address name entry / manual address form
            if (this.options.forceAddressNameEntry) {
                this.addressNamePanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-new-address")
                    .css("display", "none")
                    .appendTo(this.tabContainer);
                
                this.addressNamePanel.form = $("<form>")
                    .appendTo(this.addressNamePanel);

                var form = this.addressNamePanel.form;

                $("<p>")
                    .addClass("ui-sel-instructions")
                    .html(strings.addressNameEntryMessage)
                    .appendTo(form);

                this._createField.call(this, "addressName", "Name", 250, true)
                    .appendTo(form);

                $("<input>")
                    .attr("type", "hidden")
                    .attr("name", "capturePlusId")
                    .appendTo(form);
                
                $("<input>")
                    .attr("type", "hidden")
                    .attr("name", "labelId")
                    .appendTo(form);

                var buttons = $("<div>")
                    .addClass("buttons")
                    .appendTo(form);

                form.cancelButton = $("<input>")
                    .attr("type", "button")
                    .attr("value", strings.formCancelButton)
                    .addClass("form-button cancel-button")
                    .appendTo(buttons);

                form.saveButton = $("<input>")
                    .attr("type", "submit")
                    .attr("value", strings.formSaveButton)
                    .addClass("form-button save-button")
                    .appendTo(buttons);

                this._on(form.cancelButton, {
                    "click": function () {
                        this._switchPanel(this.searchPanel);
                        this.hasFocus = true;
                        return false;
                    }
                });

                this._on(form, {
                    "submit": function (event) {
                        event.preventDefault();
                        this._saveAddressWithName();
                        return false;
                    }
                });
                
                this._on(form.find("input[type=text]"), {
                    "focus": function (event) {
                        event.stopPropagation();
                        this.hasFocus = true;
                    }
                });
            }

            // coutries tab panel elements
            this.countriesPanel = $("<div>")
                .addClass("ui-sel-tab-panel ui-sel-countries")
                .css("display", "none")
                .appendTo(this.tabContainer);

            this.countriesMenu = $("<ul>")
                .countrymenu()
                .appendTo(this.countriesPanel)
                .data("countrymenu");
             
            this.countriesTab = $("<li>")
                .addClass("ui-sel-tab-countries")
                .attr("title", strings.tabCountriesButton)
                .appendTo(this.tabs);

            // new address panel elements
            if (this.options.enableAddingAddresses) {
                this.newAddressMessage = $("<div>")
                    .addClass("ui-sel-no-results-message")
                    .html(strings.createLink)
                    .appendTo(this.searchPanel);

                this.newAddressPanel = $("<div>")
                    .addClass("ui-sel-tab-panel ui-sel-new-address")
                    .css("display", "none")
                    .appendTo(this.tabContainer);
            }
            
            // home and office shortcut buttons
            if (this.options.enableHomeAndOffice) {
                this.buttonContainer = $("<div>")
                    .addClass("ui-sel-shortcut-buttons")
                    .appendTo(this.tabContainer);

                this.homeButton = $("<div>")
                    .addClass("ui-sel-home-button")
                    .attr("title", strings.homeButton)
                    .appendTo(this.buttonContainer);

                this.officeButton = $("<div>")
                    .addClass("ui-sel-office-button")
                    .attr("title", strings.officeButton)
                    .appendTo(this.buttonContainer);

                this.officeButtonLarge = $("<span>")
                    .addClass("ui-sel-office-button ui-sel-home-office-buttons")
                    .attr("title", strings.officeButton);

                this.startPanel.prepend(this.officeButtonLarge);

                this.homeButtonLarge = $("<span>")
                    .addClass("ui-sel-home-button ui-sel-home-office-buttons")
                    .attr("title", strings.homeButton);

                this.startPanel.prepend(this.homeButtonLarge);


            }

            this.contextButtons.appendTo(this.tabContainer);

            // disable selection of text within the widget
            this.searchPanel.disableSelection();
            this.tabs.disableSelection();

        },

        // widget constructor
        _create: function () {

            // add this address widget to the global address collection object
            $.address.instances.push(this);

            // load the labels, favourites and countries
            $.address.getWidgetConfigurationAndData();

            // setup the final position property
            this.options.position.of = this.element;

            // update the start message
            this.options.strings.startMessage = this.options.strings.startMessage.replace("{minLength}", this.options.minLength);

            // keep a reference to the associated <input>'s DOM Id
            this.elementId = this.element.attr("id");

            // setup a reference to the hidden form element that will hold the ID value of the selected address, 
            // if one isn't provided but this.element has a "rel" attribute then try to get the hidden field's Id from there.
            this.backgroundElement = this.options.backgroundElement;
            if (typeof (this.backgroundElement) === "undefined" && (typeof (this.element.attr("rel")) !== "undefined")) {
                this.backgroundElement = $("input[type=hidden][id*=" + this.element.attr("rel") + "]");
            }

            // if the fields contain any existing values (probably because we're editing an existing record) keep a reference to them
            this.currentElementValue = this.element.val();
            this.currentBackgroundElementValue = this.backgroundElement.val();
            if (+this.currentBackgroundElementValue > 0) {
                this.addressMatched = true;
            }

            // keep a reference to the date field which is used to determine the current home/office address, if this functionality is enabled
            if (this.options.enableHomeAndOffice) {
                this.homeAndOfficeDateField = $("input[id*=_txtdate]");
            }

            // some common options for SEL ajax calls
            this.ajaxOptions = {
                beforeSend: this.showLoadingIndicator,
                context: this
            };

            // some common options for the Postcode Anywhere ajax calls
            this.postcodeAnywhereAjaxOptions = {
                beforeSend: this.showLoadingIndicator,
                dataType: "jsonp",
                crossDomain: true,
                context: this,
                // without a timeout limit the error function will never fire when a cross-domain callback fails
                timeout: 20000
            };
                
            // Build the widget DOM and set up event handlers 
            this._buildWidgetDom();
            this._addElementEventBindings();
            this._addEventBindings();

            // set the default country
            this.switchCountry($.address.options.DefaultCountryAlpha3Code);

            this.reset();
        },

        // convenience function for adding a field to the new address form, returns a new "row" for the form
        _createField: function (name, label, maxLength, validate) {
            // create the "row" div for the form, the class "panel" is added so that the field labels inherit the correct style from the page
            var fieldRow = $("<div>").addClass("ui-sel-form-field formpanel");
                    
            // add the label, optionally with a mandatory indicator
            $("<label>")
                .html(label + ((validate) ? "*" : ""))
                .addClass((validate) ? "mandatory" : "")
                .appendTo(fieldRow);
                    
            // add the field
            var field = $("<input>")
                .attr("type", "text")
                .attr("name", name)
                .attr("maxlength", maxLength)
                .appendTo(fieldRow);

            // optionally add the validation marker element and validation behaviour
            if (validate) {
                $("<span>")
                    .html("*")
                    .addClass("ui-sel-invalid")
                    .appendTo(fieldRow)
                    .hide();

                $(field).on("change", $.proxy(function (event) {
                    this._validateNewAddressField($(event.currentTarget));
                }, this));
            }

            return fieldRow;
        },

        // makes the widget visible on the page
        _display: function () {
            this.debug("_display called");

            if (this.panel.is(':visible') === false) {
                this.panel
                    .zIndex(this.element.zIndex() + 1)
                    .show();
                this._position();
            }
        },

        // adds labels, favourites and manually added addresses to the widget
        _enhanceSuggestions: function () {
            this.debug("._enhanceSuggestion called", arguments);
            var i;

            // enhance the list or display the "no results" message
            if (this.searchMenu.element.children().length) {

                // order of precedence is Home/Office > Awabel > Label > Awavourite > Favourite
                // iterate each search result (but not the labels that might have already been added)
                $.each(this.searchMenu.element.children().not(".personal-label, .account-label"), $.proxy(function (itemIndex, item) {
                    item = $(item);
                    var itemData = item.data()["item.selAddress"];

                    if (!itemData) {
                        return;
                    }

                    // home / office addresses
                    if (this.options.enableHomeAndOffice) {
                        var claimDate = $.datepicker.parseDate("dd/mm/yy", this.homeAndOfficeDateField.val());
                        var endDate;

                        for (i = 0; i < $.address.data.homeAddresses.length; i += 1) {
                            var homeAddress = $.address.data.homeAddresses[i];

                            // if the item's SEL ID or PCA ID are a match for the home address
                            if ((itemData.GlobalIdentifier !== "" && (itemData.GlobalIdentifier === homeAddress.GlobalIdentifier || itemData.Id === homeAddress.GlobalIdentifier)) || (itemData.Identifier && itemData.Identifier === homeAddress.Identifier)) {
                                
                                // and the home address dates are valid for the claim
                                endDate = $.datepicker.parseDate("dd/mm/yy", homeAddress.EndDate);
                                if ($.datepicker.parseDate("dd/mm/yy", homeAddress.StartDate) <= claimDate && (endDate === null || endDate >= claimDate)) {
                                    item.children("a").html(SEL.Expenses.Settings.HomeAddressKeyword).attr("title", homeAddress.AddressFriendlyName);
                                    item.addClass("home-address");

                                    return;
                                }
                            }
                        }

                        for (i = 0; i < $.address.data.officeAddresses.length; i += 1) {
                            var officeAddress = $.address.data.officeAddresses[i];

                            // if the item's SEL ID or PCA ID are a match for the home address
                            if ((itemData.GlobalIdentifier !== "" && (itemData.GlobalIdentifier === officeAddress.GlobalIdentifier || itemData.Id === officeAddress.GlobalIdentifier)) || (itemData.Identifier && itemData.Identifier === officeAddress.Identifier)) {

                                // and the home address dates are valid for the claim
                                endDate = $.datepicker.parseDate("dd/mm/yy", officeAddress.EndDate);
                                if ($.datepicker.parseDate("dd/mm/yy", officeAddress.StartDate) <= claimDate && (endDate === null || endDate >= claimDate)) {
                                    
                                    item.children("a").html(SEL.Expenses.Settings.OfficeAddressKeyword).attr("title", officeAddress.AddressFriendlyName);
                                    if ($.address.options.SearchWithOfficeKeywordOrIcon) {
                                        item.addClass("office-address");
                                    }

                                    return;
                                }
                            }
                        }

                    }

                    // account wide labels
                    if (this.options.enableAccountWideLabels) {

                        for (i = 0; i < $.address.data.accountLabels.length; i += 1) {
                            var accountLabel = $.address.data.accountLabels[i];

                            //if the label is the primary Awabel and PCA IDs match mark the search result as a label
                            if (accountLabel.Primary && ((itemData.Id && itemData.Id === accountLabel.GlobalIdentifier) || (itemData.Identifier && itemData.Identifier === accountLabel.AddressID))) {
                                // add the address label id to the list item's data object
                                itemData.AddressLabelID = accountLabel.AddressLabelID;
                                // enhance the list item
                                item.children("a").html(accountLabel.Text).attr("title", accountLabel.AddressFriendlyName);
                                item.addClass("account-label");

                                return;
                            }
                        }
                    }
                    
                    // personal labels
                    if (this.options.enableLabels) {

                        for (i = 0; i < $.address.data.labels.length; i += 1) {
                            var label = $.address.data.labels[i];

                            //if the PCA IDs match mark the search result as a label
                            if ((itemData.Id && itemData.Id === label.GlobalIdentifier) || (itemData.Identifier && itemData.Identifier === label.AddressID)) {
                                // add the address label id to the list item's data object
                                itemData.AddressLabelID = label.AddressLabelID;
                                // enhance the list item
                                item.children("a").html(label.Text).attr("title", label.AddressFriendlyName);
                                item.addClass("personal-label");

                                return;
                            }
                        }
                    }

                    // account wide favourites
                    if (this.options.enableAccountWideFavourites) {

                        for (i = 0; i < $.address.data.accountFavourites.length; i += 1) {
                            var accountFavourite = $.address.data.accountFavourites[i];

                            //if the PCA IDs match mark the search result as a favourite
                            if ((itemData.Id && itemData.Id === accountFavourite.GlobalIdentifier) || (itemData.Identifier && itemData.Identifier === accountFavourite.AddressID)) {

                                // add the address favourite id to the list item's data object
                                itemData.FavouriteID = accountFavourite.FavouriteID;

                                // enhance the list item
                                item.addClass("account-favourite");

                                return;
                            }
                        }
                    }

                    // personal favourites
                    if (this.options.enableFavourites) {

                        for (i = 0; i < $.address.data.favourites.length; i += 1) {
                            var favourite = $.address.data.favourites[i];

                            // if the PCA IDs / SEL IDs match mark the search result as a favourite
                            if ((itemData.Id && itemData.Id === favourite.GlobalIdentifier) || (itemData.Identifier && itemData.Identifier === favourite.AddressID)) {
                                // add the address favourite id to the list item's data object
                                itemData.FavouriteID = favourite.FavouriteID;
                                // enhance the list item
                                item.addClass("personal-favourite");

                                return;
                            }
                        }
                    }
                }, this));

                this.searchMenu.refresh();
            }
            
        },

        // returns a friendly address string from a SEL address object
        _extractSelMatchedAddressValues: function (data, fields) {
            this.debug("._extractSelMatchedAddressValues called", data);

            var value = "";

            for (var i = 0; i < fields.length; i += 1) {
                var fieldData = data[fields[i]];
                if (fieldData !== "") {
                    if (value !== "") value += ", ";

                    value += fieldData;
                }
            }

            return value;
        },

        // gets a PostcodeAnywhere retrievable address
        _getAddressByPcaId: function (options) {
            this.xhr = SEL.Data.Ajax($.extend({
                url: this.options.sources.selFetchCapturePlusAddress,
                data: {
                    capturePlusId: options.pcaId,
                    labelId: options.labelId ? options.labelId : 0
                }
            }, this.ajaxOptions))
                .always(this._hideLoadingIndicator)
                .done(options.success);
        },

        // gets a SEL address
        _getAddressBySelId: function (options) {
            this.xhr = SEL.Data.Ajax($.extend({
                url: this.options.sources.selFetchSelAddress,
                data: {
                    addressIdentifier: options.selId,
                    labelId: options.labelId || 0
                }
            }, this.ajaxOptions))
                .always(this._hideLoadingIndicator)
                .done(options.success);
        },

        // gets a new list of suggestions from PostcodeAnywhere from a PostcodeAnywhereFindId
        _getAutocompleteByPcaFindId: function (options) {
            $.ajax($.extend({
                url: this.options.sources.postcodeAnywhereAutocompleteFind,
                data: {
                    Key: $.address.options.PostcodeAnywhereKey,
                    Id: options.pcaAutocompleteId
                }
            }, this.postcodeAnywhereAjaxOptions))
                .always(this._hideLoadingIndicator)
                .done(options.success);
        },

        // returns a CSS background-image rule for a given country's flag, "size" is actually the folder name
        _getFlagCssRule: function(countryName, size) {
            return "background-image: url(\"/static/icons/" + size + "/plain/flag_" + countryName.toLowerCase().replace(/ /g, "_") + ".png\") !important;";
        },

        // hides the loading spinner
        _hideLoadingIndicator: function () {
            this.loadingIndicator.hide();
        },

        // restores the <form> for creating new labels to its original DOM position and hides it
        _hideNewLabelForm: function () {
            this.addLabelForm.appendTo(this.searchPanel).hide();
        },

        // navigates the items the search menu according to a specified direction
        _move: function (direction, event) {
            event.preventDefault();

            if (this.searchMenu.element.is(":visible")) {
                // only continue if we're not at the top or bottom of the menu
                if (this.searchMenu.isFirstItem() && /^previous/.test(direction) ||
                    this.searchMenu.isLastItem() && /^next/.test(direction)) {
                    return;
                }
            }
            // JqueryUI Menu takes care of the rest
            this.searchMenu[direction](event);
        },

        // rebinds the widget to its element, useful if the element is removed and re-added (.Net update panel)
        _rebind: function () {
            this.debug("_rebind called");

            // unbind event handlers for the element
            this._off(this.element, "focus");
            this._off(this.element, "click");
            this._off(this.element, "keydown");
            this._off(this.element, "blur");

            // get the element again
            this.element = $("#" + this.elementId);
            this.options.position.of = this.element;
            this.backgroundElement = $("input[type=hidden][id*=" + this.element.attr("rel") + "]");

            // re-add event handlers to the element
            this._addElementEventBindings();
        },

        // positions the widget relative to the widget's field element
        _position: function () {
            this.panel.position(this.options.position);
        },

        // save a label
        _saveLabel: function() {
            
        },

        // save an address with
        _saveAddressWithName: function() {
            var strings = this.options.strings;
            var form = this.addressNamePanel.form;
            var messageElement = this.addressNamePanel.find(".ui-sel-instructions");
            messageElement.removeClass("ui-sel-error");

            // disable the save button and show the loading spinner
            form.saveButton.attr("disabled", "disabled");
            this._showLoadingIndicator();

            // convenience function to enable the save button and hide the loading spinner
            var onComplete = function () {
                form.saveButton.removeAttr("disabled");
                this._hideLoadingIndicator();
            };
            
            // validate the request data
            if (this._validateNewAddressField(form.find("[name=addressName]"))) {
                // construct the request data
                var data = {
                    addressName: form.find("[name=addressName]").val(),
                    capturePlusId: form.find("[name=capturePlusId]").val(),
                    labelId: +form.find("[name=labelId]").val()
                };

                SEL.Data.Ajax($.extend({
                    url: this.options.sources.selSaveAddressWithName,
                    data: data
                }, this.ajaxOptions), this)
                .done(function (responseData) {
                    responseData = responseData.d;
                    
                    var message = "";
                    if (responseData.ReturnCode < 1) {
                        messageElement.addClass("ui-sel-error");
                    }

                    switch (responseData.ReturnCode) {
                        case -1: // invalid, dupe
                            message = strings.createFailedDuplicate;
                            break;
                            // case -2 should never occur because validation has already occured client-side
                        case -3: // invalid, bad postcode
                            message = strings.createFailedBadPostcode;
                            break;
                        case -999: // no access role
                            message = strings.createFailedNoAccessRole;
                            break;
                        default:
                            // reset the form and the message, then return to the search panel
                            form.find("input[type=text], input[type=hidden]").val("");
                            message = strings.addressNameMessage;
                            this._switchPanel(this.searchPanel);

                            this._trigger("matched", undefined, responseData.Address);
                            this._value(responseData.Address.FriendlyName, responseData.Address.Identifier);

                            // force a search, which will show the new address
                            this.search(true);
                    }

                    // updates the form message, resetting then adding the extra message class
                    messageElement.html(message);
                    onComplete.call(this);
                });
            } else {
                // validation failed
                messageElement.html(strings.createFailedValidation).addClass("ui-sel-error");
                onComplete.call(this);
            }
        },

        // save a new address
        _saveNewAddress: function () {
            var strings = this.options.strings;
            var form = this.newAddressPanel.form;
            var messageElement = this.newAddressPanel.find(".ui-sel-instructions");
            messageElement.removeClass("ui-sel-error");

            // disable the save button and show the loading spinner
            form.saveButton.attr("disabled", "disabled");
            this._showLoadingIndicator();

            // convenience function to enable the save button and hide the loading spinner
            var onComplete = function () {
                form.saveButton.removeAttr("disabled");
                this._hideLoadingIndicator();
            };

            var line1Valid = this._validateNewAddressField(form.find("[name=line1]"));
            var postcodeValid = $.address.options.MandatoryPostcodeForAddresses ? this._validateNewAddressField(form.find("[name=postcode]")) : true;
            
            // validate the request data
            if (line1Valid && postcodeValid) {
                // construct the request data
                var data = {
                    addressIdentifier: 0,
                    addressName: form.find("[name=addressName]").val(),
                    line1: form.find("[name=line1]").val(),
                    line2: form.find("[name=line2]").val(),
                    line3: "",
                    city: form.find("[name=city]").val(),
                    county: form.find("[name=county]").val(),
                    alpha3CountryCode: this.options.country || $.address.options.DefaultCountryAlpha3Code,
                    postcode: form.find("[name=postcode]").val(),
                    archived: false,
                    latitude: "",
                    longitude: "",
                    capturePlusIdentifier: "",
                    esrLocationIdentifier: null,
                    esrAddressIdentifier: null,
                    accountWideFavourite: false,
                    recommendedDistances: null,
                    accessRoleCheck: false
                };

                SEL.Data.Ajax($.extend({
                    url: this.options.sources.selSaveNewAddress,
                    data: data
                }, this.ajaxOptions), this)
                .done(function (responseData) {
                    var message = "";
                    if (responseData.d < 1) {
                        messageElement.addClass("ui-sel-error");
                    }
                    
                    switch (responseData.d) {
                        case -1: // invalid, dupe
                            message = strings.createFailedDuplicate;
                            break;
                            // case -2 should never occur because validation has already occured client-side
                        case -3: // invalid, bad postcode
                            message = strings.createFailedBadPostcode;
                            break;
                        case -999: // no access role
                            message = strings.createFailedNoAccessRole;
                            break;
                        default:
                            // reset the form and the message, then return to the search panel
                            form.find("input[type=text]").val("");
                            message = strings.createMessage;
                            this._switchPanel(this.searchPanel);

                            // update the address field elements as if an address had been selected
                            var textValue = this._extractSelMatchedAddressValues(data, ["addressName", "line1", "city", "postcode"]);
                            var idValue = responseData.d;
                            this._value(textValue, idValue);

                            // force a search, which will show (at least) the new address
                            this.search(true);
                    }

                    // updates the form message, resetting then adding the extra message class
                    messageElement.html(message);
                    onComplete.call(this);
                });
            } else {
                // validation failed
                messageElement.html(strings.createFailedValidation).addClass("ui-sel-error");
                onComplete.call(this);
            }

        },

        // performs a search, fetching results from SEL Web Services and PostcodeAnywhere's API
        _search: function (value) {

            var pcaData = [], selData = [];

            // abort any pending search requests
            SEL.Data.AbortAjax(this.selXhr);
            SEL.Data.AbortAjax(this.pcaXhr);

            this._showLoadingIndicator();
            
            // clear the results list
            this.searchMenu.element.empty().scrollTop(0);
            this.contextButtons.hide();
            this.noSearchResultsPanel.hide();
            
            // any personal/account labels that match the search term should be displayed at the top of the search results
            if (this.options.enableLabels || this.options.enableAccountWideLabels) {

                // convenience function for making new a label <li>
                var createLabel = function (data, css) {
                    return $("<li>")
                        .data("item.selAddress", data)
                        .addClass("sel-suggestion " + css)
                        .append($("<a>").text(data.Text).attr("title", data.AddressFriendlyName));
                };

                var labelMenuItems = [];

                if (this.options.enableLabels) {

                    for (var i = 0; i < $.address.data.labels.length; i++) {
                        var label = $.address.data.labels[i];
                        if (label.Text.toLowerCase().lastIndexOf(value.toLowerCase(), 0) === 0) {
                            labelMenuItems.push(createLabel(label, "personal-label"));
                        }
                    }
                }

                if (this.options.enableAccountWideLabels) {
                    
                    for (i = 0; i < $.address.data.accountLabels.length; i++) {
                        var accountLabel = $.address.data.accountLabels[i];
                        if (accountLabel.Text.toLowerCase().lastIndexOf(value.toLowerCase(), 0) === 0) {
                            labelMenuItems.push(createLabel(accountLabel, "account-label"));
                        }
                    }
                }

                // append the list items to the list element
                if (labelMenuItems.length) {
                    this.searchMenu.element.append(labelMenuItems);
                }
            }

            // build the ajax options object for SEL searches, optionally tack on the accountId if the option is present (Self reg)
            var esrAssignmentId = (this.options.esrAssignmentField && this.options.esrAssignmentField.length) ? this.options.esrAssignmentField.val() : 0;
            esrAssignmentId = esrAssignmentId === null ? 0 : esrAssignmentId;
            var date = (SEL.AddressesAndTravel) ? SEL.AddressesAndTravel.GetDateField(this.element).val() : "";
            if (date === undefined) {
                date = "";
            }

            var selAjaxOptions = {
                url: this.options.accountId ? this.options.sources.selAutocompleteByAccount : this.options.sources.selAutocomplete,
                data: {
                    alpha3CountryCode: this.options.country || $.address.options.DefaultCountryAlpha3Code,
                    searchTerm: value,
                    date: date,
                    esrAssignmentId: esrAssignmentId
                }
            };
            if (this.options.accountId) {
                selAjaxOptions.data.accountId = this.options.accountId;
            }

            // two web services are called simultaneously, we use a jQuery Promise so that a method is called when both callbacks are complete
            $.when(
                // SEL ajax call
                this.selXhr = SEL.Data.Ajax($.extend(selAjaxOptions, this.ajaxOptions))
                    .done(function (data) {
                        selData = data.d;
                        this.debug("SEL Data", selData);
                    var workAddressKeyword = SEL.Expenses === null || SEL.Expenses === undefined
                        ? ""
                        : SEL.Expenses.Settings.WorkAddressKeyword.toLowerCase();
                    $.address.options.SearchWithOfficeKeywordOrIcon = value.toLowerCase() === "office" || value.toLowerCase() === workAddressKeyword;
                    var hideMultiple = $.address.options.SearchWithOfficeKeywordOrIcon && !$.address.options.AllowMultipleWorkAddresses;
                    this._suggestSel(selData, hideMultiple);
                    }),

                // PCA ajax call
                this.pcaXhr = $.ajax($.extend({
                    url: this.options.sources.postcodeAnywhereAutocomplete,
                    data: {
                        Key: $.address.options.PostcodeAnywhereKey,
                        Country: this.options.country || $.address.options.DefaultCountryAlpha3Code,
                        searchTerm: value
                    }
                }, this.postcodeAnywhereAjaxOptions))
                    .done(function (data) {
                        pcaData = data.Items;

                        // check the response isn't a PCA error message
                        if (!(pcaData.length === 1 && !pcaData[0].Match && pcaData[0].Suggestion.toLowerCase().replace(/\s+/g, "") !== this.element.val().toLowerCase().replace(/\s+/g, "")))
                        {
                            // display the search results straight away
                            this._suggestPca(pcaData);
                        }

                        this.debug("PCA Data", pcaData);
                    })
                    .fail(function (xmlHttpRequest, textStatus, errorThrown) {
                        this.debug("PCA call failed", xmlHttpRequest, textStatus, errorThrown);

                        // append the list items to the list element, then reset the scroll position
                        this.searchMenu.element.prepend($("<li>")
    		                .data("item.sel-address", undefined)
                            .addClass("sel-suggestion sel-error")
                            .append($("<a>").text("Not all addresses could be retrieved")));
                        this.searchMenu.refresh();
                    })

            ).always($.proxy(function() {

                // both callbacks are complete, hide the spinner
                this._hideLoadingIndicator();

                // if there are no results from either source display the "no results" message
                if (!this.searchMenu.element.children().length) {
                    this.noSearchResultsPanel.show();
                }

            }, this));
        },

        // this method is called when a search menu item is clicked
        _searchMenuItemSelected: function (event, ui) {

            if (ui.item.hasClass("sel-error")) {
                window.open(this.options.strings.pcaSearchFailedUri);
                return;
            }

            this._showLoadingIndicator();

            // get the data object associated with the clicked element
            var item = ui.item.data("item.sel-address");

            // the selected search result relates to a Postcode Anywhere search result
            if ($.type(item.Id) !== "undefined" && item.Id !== "") {

                // the selected search result is "retrievable", i.e. relates to a single address record
                if (item.IsRetrievable === true) {
                    
                    // if the option is set, collect a name for the address before retrieving it
                    if (this.options.forceAddressNameEntry) {

                        // populate the ID fields in the form before switching to it 
                        this.addressNamePanel.form.find("input[name=capturePlusId]").val(item.Id);
                        this.addressNamePanel.form.find("input[name=labelId]").val(item.AddressLabelID || 0);
                        
                        this._switchPanel(this.addressNamePanel);

                    } else {
                        // get the address record
                        this._getAddressByPcaId({
                            pcaId: item.Id,
                            labelId: item.AddresslabelID,
                            success: function (data) {
                                data = data.d;
                                this.debug("address matched", data);
                                this._trigger("matched", event, data);

                                var textValue = data.FriendlyName;
                                if (ui.item.hasClass("home-address")) {
                                    textValue = SEL.Expenses.Settings.HomeAddressKeyword;
                                }

                                this.element.attr("data-office", ui.item.hasClass("office-address"));

                                this._value(textValue, data.Identifier);
                            }
                        });

                    }

                }
                    // the search result is relates to a list of further suggestions
                else {
                    // get the list of suggestions and display them
                    this._getAutocompleteByPcaFindId({
                        pcaAutocompleteId: item.Id,
                        success: function (autocompleteData) {
                            this.debug("address not matched", autocompleteData);

                            this._hideLoadingIndicator();

                            // clear the results list
                            this.searchMenu.element.empty();

                            this._suggestPca(autocompleteData.Items);

                            // scroll the list to the top
                            this.searchMenu.element.scrollTop(0);
                            
                        }
                    });
                }
            }
            // the search result relates to a manually added SEL address
            else {
                this.debug("manual address matched", item);
                this._trigger("matched", event, item);

                var textValue = item.FriendlyName || item.AddressFriendlyName;
                if (ui.item.hasClass("home-address")) {
                    textValue = SEL.Expenses.Settings.HomeAddressKeyword;
                }

                this.element.attr("data-office", ui.item.hasClass("office-address"));

                setTimeout($.proxy(function() {
                    this._value(textValue, item.Identifier);
                    if (this.element.attr("data-office") === "true") {
                        if (SEL.AddressesAndTravel !== null && SEL.AddressesAndTravel !== undefined) {
                            SEL.AddressesAndTravel.UpdateOfficeAddressesWithNewValue(textValue, item.Identifier);    
                        }
                        
                        $('#' + hdnWorkAddressID).val(item.Identifier);
                    }
                    this._hideLoadingIndicator();
                }, this), 100);
            }
        },

        // shows the loading spinner
        _showLoadingIndicator: function () {
            this.loadingIndicator.show();
        },

        // display the PCA search results
        _suggestPca: function (items) {
            var item;

            this.startPanelHidden = true;

            // show the search results panel
            this._switchPanel(this.searchPanel);

            if (items.length > 0) {

                // create the list
                var menuItems = [];
                for (var i = 0; i < items.length; i += 1) {
                    item = items[i];

                    // build a list item element and add it to the list
                    menuItems.push($("<li>")
		                .data("item.sel-address", item)
		                .addClass("sel-suggestion" + (item.IsRetrievable ? "" : " not-retrievable"))
		                .append($("<a>").text((item.Match || "") + item.Suggestion)));
                }
                // append the list items to the list element, then reset the scroll position
                this.searchMenu.element.append(menuItems);

                this._enhanceSuggestions();
            }
        },

        // display the SEL search results
        _suggestSel: function(items, hideMultiple) {

            // show the search results panel
            this.startPanelHidden = true;
            this._switchPanel(this.searchPanel);

            // manually added addresses
            if (items && this.options.enableManualAddresses) {
                var itemsToAdd = [];
                // add each manual address to the beginning of the search list
                if ((items.ManualAddresses.length === 1 && hideMultiple) && (SEL.AddressesAndTravel !== null && SEL.AddressesAndTravel !== undefined)) {
                    SEL.AddressesAndTravel.PopulateAddressPickerByKeyword(this.element);    
                } else
                {
                    for (var i = 0; i < items.ManualAddresses.length; i += 1) {
                        var item = items.ManualAddresses[i];

                        itemsToAdd.push($("<li>")
                            .data("item.sel-address", item)
                            .addClass("sel-suggestion manual-address")
                            .append($("<a>").text(item.FriendlyName)));
                    }
                    this.searchMenu.element.prepend(itemsToAdd);
                }
            }
            this._enhanceSuggestions();
        },

        // makes the passed tab panel visible, hiding all other tab panels
        _switchPanel: function (panel) {
            this.debug("_switchPanel called", arguments);
            this.tabContainer.children(":not(.ui-sel-loading)").hide();
            this.panel.find(".ui-sel-tabs-nav li").removeClass("ui-sel-tab-active");

            if (panel.is(this.startPanel)) {
                // visually set the active tab
                this.panel.find("li.ui-sel-tab-search").addClass("ui-sel-tab-active");
                this.tabContainer.children(".ui-sel-shortcut-buttons").show();
            }

            // the search panel might need to be substituted for the "start" panel
            if (panel.is(this.searchPanel)) {
                // visually set the active tab
                this.panel.find("li.ui-sel-tab-search").addClass("ui-sel-tab-active");
                this.tabContainer.children(".ui-sel-shortcut-buttons").show();

                if (!this.startPanelHidden) {
                    panel = this.startPanel;
                }
            }

            // the "new address" panel needs building if it hasn't already been shown 
            if (panel.is(this.newAddressPanel) && this.newAddressPanel.built !== true) {
                this._buildNewAddressPanel();
            }

            var i;
            
            // (re)populate the labels panel
            if (panel.is(this.labelsPanel) && (this.options.enableLabels || this.options.enableAccountWideLabels)) {
                
                // visually set the active tab
                this.panel.find("li.ui-sel-tab-labels").addClass("ui-sel-tab-active");

                // clear the label list
                this.labelsMenu.element.empty().scrollTop(0);
                this.noLabelsPanel.show();
                var labelsMenuItems = [];

                var createLabel = function(data, css) {
                    if (data.GlobalIdentifier === "") {
                        css = "manual-address " + css;
                    }

                    var label = $("<li>")
                        .data("item.selAddress", data)
                        .addClass("sel-suggestion " + css)
                        .append($("<a>").text(data.Text).attr("title", data.AddressFriendlyName));

                    return label;
                };

                if (this.options.enableLabels) {
                    for (i = 0; i < $.address.data.labels.length; i += 1) {
                        labelsMenuItems.push(createLabel($.address.data.labels[i], "personal-label"));
                    }
                }

                if (this.options.enableAccountWideLabels) {
                    for (i = 0; i < $.address.data.accountLabels.length; i += 1) {
                        labelsMenuItems.push(createLabel($.address.data.accountLabels[i], "account-label"));
                    }
                }

                if (labelsMenuItems.length) {
                    this.noLabelsPanel.hide();
                    
                    // append the list items to the list element, then reset the scroll position
                    this.labelsMenu.element.append(labelsMenuItems);
                    this.labelsMenu.refresh();
                }
            }
            
            // (re)populate the favourites panel
            if (panel.is(this.favouritesPanel) && (this.options.enableFavourites || this.options.enableAccountWideFavourites)) {
                // visually set the active tab
                this.panel.find("li.ui-sel-tab-favourites").addClass("ui-sel-tab-active");

                // clear the favourite list
                this.favouritesMenu.element.empty().scrollTop(0);
                this.noFavouritesPanel.show();
                var favouritesMenuItems = [];
                
                var createFavourite = function (data, css) {
                    if (data.GlobalIdentifier === "") {
                        css = "manual-address " + css;
                    }

                    var favourite = $("<li>")
                        .data("item.selAddress", data)
                        .addClass("sel-suggestion " + css)
                        .append($("<a>").text(data.AddressFriendlyName));

                    return favourite;
                };
                
                // personal favourites
                if (this.options.enableFavourites) {
                    for (i = 0; i < $.address.data.favourites.length; i++) {
                        favouritesMenuItems.push(createFavourite($.address.data.favourites[i], "personal-favourite"));
                    }
                }
                
                // account wide favourites
                if (this.options.enableAccountWideFavourites) {
                    for (i = 0; i < $.address.data.accountFavourites.length; i++) {
                        favouritesMenuItems.push(createFavourite($.address.data.accountFavourites[i], "account-favourite"));
                    }
                }
                
                if (favouritesMenuItems.length) {
                    this.noFavouritesPanel.hide();

                    // append the list items to the list element, then reset the scroll position
                    this.favouritesMenu.element.append(favouritesMenuItems);
                    this.favouritesMenu.refresh();
                }

            }

            if (panel.is(this.countriesPanel)) {
                // visually set the active tab
                this.panel.find("li.ui-sel-tab-countries").addClass("ui-sel-tab-active");
                // populate the countries menu, just once
                if (!this.countriesMenu.element.children().length) {

                    // convenience function for creating a country list item
                    var createCountry = function(data) {
                        return $("<li>")
                            .data("item.selCountry", data)
                            .addClass("sel-country")
                            .append($("<a>").text(data.Country).attr("style", this._getFlagCssRule(data.Country, "16")));
                    };

                    var countriesMenuItems = [];
                    for (i = 0; i < $.address.data.countries.length; i += 1) {
                        countriesMenuItems.push(createCountry.call(this, $.address.data.countries[i]));
                    }

                    this.countriesMenu.element.append(countriesMenuItems);
                    this.countriesMenu.refresh();
                }
            }
            
            // show the specified panel
            panel.show();
            
            // this causes the widget to disappear because of a timing issue with the address field blurring, 
            // investigate when sorting out the IE7-widget-getting-stuck-open problem
            /*
            if (panel.is(this.newAddressPanel)) {
                this.newAddressPanel.form.find("input[type=text]").first().focus();
            }
            
            if (panel.is(this.addressNamePanel)) {
                this.addressNamePanel.form.find("input[type=text]").first().focus();
            }
            */
        },

        // checks that a given field (in the new address form) isn't blank, adds or removes the invalid indicator accordingly
        _validateNewAddressField: function (fieldElement) {
            var valid = ($.trim(fieldElement.val()) !== "");
            fieldElement.siblings("span.ui-sel-invalid").toggle(!valid);
            return valid;
        },

        // updates the widget's field with the passed value, then closes the widget
        _value: function (textValue, backgroundValue) {
            this.element.val(textValue);
            this.currentElementValue = textValue;
            this.backgroundElement.val(backgroundValue);
            this.currentBackgroundElementValue = backgroundValue;
            this.addressMatched = true;
            // force the "change" event to fire on the hidden field
            this.backgroundElement.change();
            this.close();
        },
        
        /* public methods */
        value: function (textValue, backgroundValue) {
            this._value(textValue, backgroundValue);
        },
        
        // returns the current background (addressId) value
        getBackgroundValue: function() {
            return this.backgroundElement.val();
        },

        // closes the widget
        close: function () {
            this.debug(".close called");

                this.panel.hide();
            
            if (this.options.enableLabels) {
                this._hideNewLabelForm();
            }
            
            this.hasFocus = false;
        },

        // outputs debug information to the console (if debugging is enabled and the console is available)
        debug: function () {
            if (this.options.debugging && "console" in window) {
                console.info(this, arguments);
            }
        },

        // resets the widget (re-gets the values of associated fields and restores the start panel)
        reset: function () {
            this.currentBackgroundElementValue = this.backgroundElement.val();
            this.currentElementValue = this.element.val();

            this._switchPanel(this.startPanel);
        },

        // initialises a search, using whatever value is in the widget's field
        // use preserveFieldValues to bypass resetting the associated textbox and hidden ID field
        search: function (preserveFieldValues) {
            var value = this.element.val().trim();

            if (!preserveFieldValues) {
                // only continue if the text field's value has actually changed
                if (value === this.currentElementValue) return;
                this.currentElementValue = value;
                this.backgroundElement.val("");
                this.currentBackgroundElementValue = "";
                this.addressMatched = false;
            }

            if (value.length >= this.options.minLength) {
                this._search(value);
            } else {
                // clear the results list and display the start panel
                this.searchMenu.element.empty().scrollTop(0);

                this.startPanelHidden = false;
                this._switchPanel(this.startPanel);
            }
        },

        // changes the country code that is used for search and creating manually added addresses,
        // then forces a new search (with the current text) in that country
        switchCountry: function (alpha3CountryCode) {
            if (alpha3CountryCode && alpha3CountryCode != "") {
                this.options.country = alpha3CountryCode;
                var countryName = "";

                for (var i = 0; i < $.address.data.countries.length; i += 1) {
                    var country = $.address.data.countries[i];
                    if (country.Alpha3CountryCode === alpha3CountryCode) {
                        countryName = country.Country;
                    }
                }
                this.countriesTab.attr("style", this._getFlagCssRule(countryName, "32"));
            }
        }
    });

})(jQuery);
