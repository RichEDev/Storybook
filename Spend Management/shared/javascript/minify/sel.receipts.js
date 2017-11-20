/*global SEL:false, $g:false, $:false, Spend_Management: false */
(function (SEL, $g) {
    var scriptName = "Receipts";

    function execute() {
        SEL.registerNamespace("SEL.Receipts");
        // Provides a lightbox preview of receipts for expense items and claims
        SEL.Receipts = {
            IDs: {
                CurrentElement : null,
                CurrentClaimId : null,
                CurrentExpenseId : null,
                CurrentReceiptId : null,
                DeleteReasonModalId : null,
                DeleteReasonTextId: null,
                DeleteValidationId: null,
                DeleteModal: null,
                MustGiveDeleteReason: true,
                DeleteReasonTextBox: null,
                DeleteValidation: null
            },
            // Setup page for viewing the receipts for a single expense item, there must be a list of receipt elements (receipts.ascx) in the DOM 
            // for colorbox to consume
            Bind: function (expenseId, claimId) {
                $(document).ready($.proxy(function () {
                    this.SetupColorbox();

                    SEL.Receipts.IDs.CurrentClaimId = claimId;
                    SEL.Receipts.IDs.CurrentExpenseId = expenseId;
                    SEL.Receipts.IDs.DeleteModal = $("#" +SEL.Receipts.IDs.DeleteReasonModalId);
                    SEL.Receipts.IDs.DeleteReasonTextBox = $("#" +SEL.Receipts.IDs.DeleteReasonTextId);
                    SEL.Receipts.IDs.DeleteValidation = $("#" + SEL.Receipts.IDs.DeleteValidationId);

                    var highestZIndex = 1 + SEL.Common.GetHighestZIndexInt();
                    var modal = SEL.Receipts.IDs.DeleteModal;
                    modal.dialog({
                        modal: true,
                        autoOpen: false,
                        zIndex: highestZIndex,
                        width: 900,
                        height: 200,
                        closeOnEscape: true,
                        draggable: false,
                        resizable: false,
                        open: function () {
                            //replace button classes
                            $(".ui-button").unbind();//prevent the jQuery standard focus effects
                            $(".ui-button").click(function () {
                                modal.dialog('destroy');
                            }); // replace the click event
                            $(".ui-button").keyup(function (event) {
                                if (event.keyCode == 13) {
                                    modal.dialog('destroy');
                                } // also allow ENTER to close modal
                    });
                            $('.ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix').removeClass('ui-dialog-buttonpane ui-widget-content ui-helper-clearfix').addClass('formbuttons').addClass('formpanel');
                            $('.ui-dialog-buttonset').removeClass('ui-dialog-buttonset').addClass('buttonContainer');
                            $('.ui-button.ui-widget.ui-corner-all.ui-button-text-only').removeClass('ui-button ui-widget ui-corner-all ui-button-text-only').addClass('buttonInner');

                            $('.ui-button-text').removeClass('ui-button-text');
                            $('.ui-state-default').removeClass('ui-state-default');
                            $('.ui-state-hover').removeClass('ui-state-hover');
                            $('.ui-state-focus').removeClass('ui-state-focus');
                        }
                    });
                    modal.dialog("option", "dialogClass", "modalpanel");
                    modal.dialog("option", "dialogClass", "formpanel");
                    $(".ui-dialog-titlebar").hide();


                    // show the empty message if there are no items in the list
                    if ($("#attachedReceipts li").length === 0)
                    {
                        $("#attachedReceipts p.emptyMessage").show();
                    }

                    // attach a listener for clicks on each delete anchor
                    $("#attachedReceipts a.deleteReceipt").on("click", $.proxy(function (event) {
                        var element = $(event.currentTarget);
                        element = element.parent("li");
                        SEL.Receipts.IDs.CurrentElement = element;
                        SEL.Receipts.IDs.CurrentReceiptId = parseFloat(element.data('id'));

                        if (SEL.Receipts.IDs.MustGiveDeleteReason === 'False') {
                            SEL.Receipts.DeleteReceipt();
                            event.preventDefault();
                        } else {
                            SEL.Receipts.IDs.DeleteValidation.hide();
                            SEL.Receipts.IDs.DeleteModal.dialog('open');
                            SEL.Receipts.IDs.DeleteReasonTextBox.focus();
                        }
                        return false;
                    }, this));
                    return false;
                }, this));
            },
            // Setup page for viewing the receipts for a claim and all of its expense items, jQuery selectors must be passed for the claim and expense 
            // item receipt anchors. There must be a receipt list element template (receipts.ascx) in the DOM for colorbox to consume
            //SEL.Receipts.BindAjax(".receiptPreview", ".claimReceipts a");
            BindAjax: function (expenseButtonDomSelector, claimButtonDomSelector) {
                this.ajaxMode = true;

                // attach handlers to receipt buttons
                $(document).ready($.proxy(function () {

                    $("#gridExpenses, #gridReturned, #gridApproved").on("click", "a.receiptPreview img", $.proxy(function(event) {
                        var element = $(event.target).parent("a");
                        var expenseId = $(element).attr("href");
                        expenseId = +expenseId.substring(expenseId.lastIndexOf("#") + 1, expenseId.length);

                        this.SetupColorbox("expense", expenseId, "GetReceiptsForExpenseItem", '{ "expenseId": ' + expenseId + '}');
                        return false;

                    }, this));

                    $(claimButtonDomSelector).on("click", $.proxy(function (event) {
                        var claimId = +$(event.currentTarget).attr("href");
                        this.SetupColorbox("claim", claimId, "GetReceiptsForClaim", '{ "claimId": ' + claimId + '}');
                        return false;
                    }, this));

                }, this));

            },
            // Creates the list DOM structure and appends it to the page
            BuildListElement: function (data, successFunction) {
                
                // clear the current list
                var listElement = $(".receiptsBackgroundList")
                    .empty()
                    .append("<ul>");

                var itemElements = [];

                // rebuild the list
                for (var i = 0; i < data.length; i += 1) {

                    var item = data[i];

                    var itemElement, firstImage, firstImageSrc;

                    var fileExtension = item.extension.toLowerCase();
                    if (item.validImageForBrowser) {

                        // keep a reference to the first image
                        if (typeof (firstImageSrc) === "undefined") {
                            firstImageSrc = '../' + item.filename;
                        }

                        // copy the template <li>
                        itemElement = $("a.receiptImage").parent().clone();
                        // update the new <li> with the receipts properties
                        itemElement.children("a")
                                   .attr("href", '../' + item.filename)
                                   .children("img")
                                        .attr("src", '../' + item.filename);
                    } else {
                        // copy the template <li>
                        itemElement = $("a.receiptOther").parent().clone();
                        // update the new <li> with the receipts properties
                        itemElement.children("a")
                                   .attr("href", '../' + item.filename)
                                   .children(".noReceiptImage")
                                        .addClass(fileExtension);
                    }

                    // remove the delete button
                    itemElement.children(".deleteReceipt").remove();

                    // add the receipt to the list
                    itemElements.push(itemElement);
                }

                listElement.children("ul").append(itemElements);

                // if there are no images just open colorbox immediately, otherwise preload the first image (otherwise colorbox might
                // open too quickly, causing layout issues)
                if (typeof (firstImageSrc) === "undefined") {
                    successFunction.call(this);
                } else {
                    $(firstImage = new Image()).on("load", $.proxy(function () {
                        successFunction.call(this);
                    }, this));
                    firstImage.src = firstImageSrc;
                }
            },
            // hides the modal
            HideDeleteModal: function () {
                SEL.Receipts.IDs.DeleteModal.dialog('close');
            },
            // Submits a receipt delete request to the server, a function can be passed which will be executed when the callback succeeds
            DeleteReceipt: function () {

                if (SEL.Receipts.IDs.MustGiveDeleteReason === 'True') {
                // validate
                    var reason = SEL.Receipts.IDs.DeleteReasonTextBox.val();
                    if (String.isNullOrWhitespace(reason)) {
                        SEL.Receipts.IDs.DeleteValidation.show();
                        return;
                    }

                    // trim reason
                    if (reason.length > 4000) {
                        reason = reason.substring(0, 4000);
                    }

                    // hide modal
                    SEL.Receipts.IDs.DeleteModal.dialog('close');
                }

                var sendData = "{\"receiptId\": " + SEL.Receipts.IDs.CurrentReceiptId + ", \"expenseId\": " + SEL.Receipts.IDs.CurrentExpenseId + ", \"claimId\": " + SEL.Receipts.IDs.CurrentClaimId + ", \"reason\": '" + reason + "' }";

                // make call
                $.ajax({
                    url: appPath + 'webServices/receipts.asmx/DeleteReceipt',
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: sendData,
                    success: function (data) {

                        // hide + remove the clicked element.
                        var element = SEL.Receipts.IDs.CurrentElement;
                        element.fadeOut({
                            duration: 750,
                            complete: function () {
                                element.remove();
                                // show the empty message if there are no longer any items in the list
                                if ($("#attachedReceipts li").length === 0) {
                                    $("#attachedReceipts p.emptyMessage").show();
                                }
                            }
                        });
                    },
                    error: function (xmlHttpRequest, textStatus, errorThrown) {
                        var reasonForFailure = JSON.parse(xmlHttpRequest.responseText).Message;
                        // show the text box.
                        SEL.MasterPopup.ShowMasterPopup('There was an issue deleting this receipt:<br/>' + reasonForFailure + '.<span style="display:none;">' + errorThrown + '</span>',
                            'Message from ' + moduleNameHTML);
                    }
                });
            },
            // Sets up colorbox (http://www.jacklmoore.com/colorbox/) for receipts
            SetupColorbox: function (mode, id, webServiceMethod, webServiceData) {
                // Older browsers need the body and/or html elements to have 100% height for an overlay to fill the viewport, 
                // done here because adding a rule to the theme CSS would potentially impact other pages
                if ($.support.leadingWhitespace === false) // IE6-8
                {
                    $("body, html").css("height", "100%");
                }

                // initialises colorbox, function is either called immediately or, if in ajax mode, the receipt data
                // is retrieved and DOM elements are created first
                var setupColorbox = function () {

                    // helper method which determines if a receipt list item is an image
                    var isImage = function (element) {
                        return ($(element).hasClass("receiptImage"));
                    };

                    var listElements = $(".colorbox a.receiptImage, .colorbox a.receiptOther");

                    // initialise colorbox
                    $(listElements).colorbox({
                        current: "Receipt {current} of {total}",
                        html: function () {
                            // custom renderer function
                            var html = '<div class="noPreview"><a href="#" target="_blank"><p>This receipt can\'t be viewed here, click to download it instead.</p></a></div>';

                            // if the item is an image create the default colorbox image element
                            if (isImage(this)) {
                                html = $("<img>").addClass("cboxPhoto").attr("src", $(this).attr("href"));
                            }
                            return html;
                        },
                        maxWidth: "95%",
                        maxHeight: "95%",
                        opacity: 0.7,
                        open: this.ajaxMode,
                        onComplete: function () {
                            if (!isImage(this)) {
                                //update the anchor (from the "html" property above) to point to the corresponding file.
                                var anchor = $("#cboxLoadedContent div.noPreview a")[0];
                                anchor.href = this.href;

                                // copy the icon that was clicked into the anchor
                                $(this).find(".noReceiptImage").clone().prependTo(anchor);
                            }
                            
                            // ensure focus is on the colorbox element, otherwise colorbox shortcut keys won't work
                            $("#colorbox").focus();
                        },
                        rel: 'receiptsGroup',
                        scalePhotos: false,
                        title: function () {
                            return '<a class="title" href="' + $(this).attr("href") + '" target="_blank">Download file (opens a new window)</a>';
                        }
                    });

                };

                if (this.ajaxMode === true) {

                    $.ajax({
                        type: "POST",
                        url: appPath + "/webServices/Receipts.asmx/" + webServiceMethod,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: webServiceData,
                        context: this,
                        success: function (data) {
                            data = data.d;
                                this.BuildListElement(data, setupColorbox);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert('An error occurred with service call.\n\n' + errorThrown);
                        }
                    });

                } else {
                    setupColorbox.call(this);

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
}(SEL, $g));
