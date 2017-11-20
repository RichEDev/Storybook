(function (SEL)
{
    var scriptName = "expenses";

    function execute()
    {
        SEL.registerNamespace("SEL.Expenses");
        SEL.Expenses =
        {
            Dom:
            {
                OrganisationSearch:
                {
                    Panel: null,
                    Modal: null
                }
            },
            
            Settings:
            {
                AllowClaimantAddOrganisations: null,
                OrganisationLabel: null
            },
            ExpenseItems: {
                Filter: function () {
                    if (SEL.Expenses.ExpenseItems.SubCats === null) {
                        SEL.Expenses.ExpenseItems.SubCats = JSON.parse($('#hdnSubCatDates').val());
                    }
                    var dateValue = $('#' + window.contentID + 'txtdate').val();
                    var date = dateValue.substr(6, 4) +
                        "-" +
                        dateValue.substr(3, 2) +
                        "-" +
                        dateValue.substr(0, 2);
                    $('.ellide>br').hide();
                    
                    $('.subcatlistitem').hide().each(function() {
                        var id = $(this).attr('subcat');
                        if (SEL.Expenses.ExpenseItems.SubCats !== null && id !== null && id !== undefined){
                            var subcat = $(this).attr('subcat');
                            var dates = SEL.Expenses.ExpenseItems.SubCats[subcat];
                            for (var i = 0; i < dates.length; i++) {
                                if (dates[i].StartDate <= date && dates[i].EndDate >= date) {
                                    $(this).show();
                                    $(this).next('br').show();
                                    break;
                                }
                                
                            }
                        }
                    });

                    $('#ctl00_contentmain_cmbsubcats span').each(function() {
                            var opt = $(this).find('option').show();
                            $(this).replaceWith(opt);
                        
                        });


                    $('#ctl00_contentmain_cmbsubcats option').each(function () {
                        var subCatId = $(this).val();
                        if (SEL.Expenses.ExpenseItems.SubCats !== null) {
                            var dates = SEL.Expenses.ExpenseItems.SubCats[subCatId];
                            if (dates !== undefined) {
                                var showItem = false;
                                for (var i = 0; i < dates.length; i++) {
                                    if (dates[i].StartDate <= date && dates[i].EndDate >= date) {
                                        showItem = true;
                                        break;
                                    }
                                }

                                if (showItem || $(this).is(':selected')) {
                                    $(this).show();
                                } else {
                                    $(this).wrap('<span>').hide();
                                }    
                            }
                        }
                    });

                },
                SubCats: null
            },
            Validate:
            {
                ItemIndex: function(containerPrefix, validatorIdentifier, lengthOfControlIdentifier)
                {
                    if (typeof containerPrefix !== "string" || typeof validatorIdentifier !== "string")
                    {
                        return 0;
                    }

                    return validatorIdentifier.substring(containerPrefix.length + lengthOfControlIdentifier, containerPrefix.length + validatorIdentifier.length - lengthOfControlIdentifier);
                },
                
                ItemValue: function (containerPrefix, itemIndex)
                {
                    var itemValueField = document.getElementById(containerPrefix + 'txttotal' + itemIndex);

                    if (typeof itemValueField === "undefined" || itemValueField === null)
                    {
                        itemValueField = document.getElementById(containerPrefix + 'txtmileage' + itemIndex);
                    }

                    if (typeof itemValueField === "undefined" || itemValueField === null)
                    {
                        itemValueField = document.getElementById(containerPrefix + 'txtmileage' + itemIndex + '_0');
                    }

                    return (typeof itemValueField !== "undefined" && itemValueField !== null && "value" in itemValueField) ? itemValueField.value : "";
                },
                
                Organisation:
                {
                    SaveIfNotExists: function (elementIdentifier)
                    {
                        var namespace = SEL.Expenses;
                        
                        if (namespace.Settings.AllowClaimantAddOrganisations && !namespace.Validate.Organisation.AutocompleteValid(elementIdentifier) && $("#" + elementIdentifier).length === 1 && $("#" + elementIdentifier).val().length > 0)
                        {
                            if (confirm("The " + namespace.Settings.OrganisationLabel + " you have entered does not exist, would you like to save it?"))
                            {
                                SEL.Data.Ajax({
                                    data: { "name": $("#" + elementIdentifier).val() },
                                    methodName: "SaveForClaimant",
                                    serviceName: "svcOrganisations",
                                    async: false,
                                    success: function (r) {
                                        var value = "-1";
                                        if ("d" in r) {
                                            if (r.d === -1) {
                                                alert("The organisation you are trying to add was previously archived, please contact an administrator if you would like to use it.");
                                                $("#" + elementIdentifier).val("");
                                            }

                                            if (r.d > 0) {
                                                value = r.d;
                                            }
                                        }

                                        $("#" + elementIdentifier + "_ID").val(value);
                                        $("#" + elementIdentifier).trigger("change"); // trigger the validation to occur
                                    },

                                    error: function (jqueryXmlHttpRequest, textStatus, errorThrown) { $("#" + elementIdentifier + "_ID").val("-1"); }
                                });
                            }
                        }
                    },

                    AutocompleteValid: function (elementIdentifier)
                    {
                        var identifier = $("#" + elementIdentifier + "_ID"),
                            text = $("#" + elementIdentifier);

                        return (identifier.length === 1 && text.length === 1 && ((identifier.val() === "" && text.val() === "") || (!isNaN(parseInt(identifier.val(), 10)) && parseInt(identifier.val(), 10) > 0 && text.val().length > 0)));
                    },

                    ItemLevelMandatory: function (sender, args) { SEL.Expenses.Validate.Organisation.ItemLevel(sender, args, true); },
                    
                    ItemLevelNotMandatory: function (sender, args) { SEL.Expenses.Validate.Organisation.ItemLevel(sender, args, false); },

                    ItemLevel: function(sender, args, mandatory)
                    {
                        var namespace = SEL.Expenses.Validate,
                            prefix = window.contentID,
                            index = namespace.ItemIndex(prefix, sender.id, 16),
                            elementIdentifier = prefix + index + "_txtOrganisation",
                            value = namespace.ItemValue(prefix, index);

                        if (value === "")
                        {
                            args.IsValid = true;
                            return;
                        }
                        
                        value = parseFloat(value);

                        if (!isNaN(value) && value > 0)
                        {
                            args.IsValid = namespace.Organisation.AutocompleteValid(elementIdentifier) && (!mandatory || $("#" + elementIdentifier).val().length > 0);
                            return;
                        }
                        else
                        {
                            args.IsValid = true;
                            return;
                        }
                    },

                    GeneralDetailsMandatory: function (sender, args) { SEL.Expenses.Validate.Organisation.GeneralDetails(sender, args, true); },

                    GeneralDetailsNotMandatory: function (sender, args) { SEL.Expenses.Validate.Organisation.GeneralDetails(sender, args, false); },

                    GeneralDetails: function (sender, args, mandatory)
                    {
                        var prefix = window.contentID,
                            elementIdentifier = prefix + "txtOrganisation";
                        
                        args.IsValid = SEL.Expenses.Validate.Organisation.AutocompleteValid(elementIdentifier) && (!mandatory || $("#" + elementIdentifier).val().length > 0);
                        return;
                    }
                }
            },
            
            OrganisationSearch:
            {
                Hide: function ()
                {
                    SEL.Common.HideModal(SEL.Expenses.Dom.OrganisationSearch.Modal);
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
}(SEL));