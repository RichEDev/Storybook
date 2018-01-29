/// <reference path="knockout-3.1.0.debug.js" />
/*global SEL:false, $g:false, $:false, Spend_Management: false */
(function (sel) {
    var scriptName = "ReceiptManagement";

    function execute() {
        sel.registerNamespace("SEL.ReceiptManagement");

        sel.ReceiptManagement = {

            // common ids
            Ids: {
                CurrentElement: null,
                CurrentId: null,
                CurrentClaimId: null,
                CurrentExpenseId: null,
                CurrentReceiptId: null,
                FromClaimSelector: null,

                DeleteReasonModalId: null,
                DeleteReasonTextId: null,
                DeleteValidationId: null,
                MustGiveDeleteReason: true,

                IsDeclaration: false,
                DeclarationModalId: null,
                DeclarationAcceptButtonId: null,

                SaveButtonId: null,
                CancelButtonId: null
            },


            // elements in the page
            Elements: {
                DeleteModal: null,
                DeclarationModal: null,
                DeleteReasonTextBox: null,
                DeleteValidation: null,
                ReceiptTreeControl: null,
                ReceiptUploadFileInput: null,
                ReceiptUploadError: null,
                SaveButton: null,
                CancelButton: null,
                DeclareButton: null,
                ReceiptPreview: null
            },


            // general functions
            General: {

                DeleteCallback: null,

                Init: function () {

                    sel.ReceiptManagement.Elements.DeleteValidation = $("#" + sel.ReceiptManagement.Ids.DeleteValidationId);
                    sel.ReceiptManagement.Elements.DeleteReasonTextBox = $("#" + sel.ReceiptManagement.Ids.DeleteReasonTextId);
                    sel.ReceiptManagement.Elements.DeleteModal = sel.Common.BuildJqueryDialogue("#" + sel.ReceiptManagement.Ids.DeleteReasonModalId, 900, 200);
                    sel.ReceiptManagement.Elements.SaveButton = $("#" + sel.ReceiptManagement.Ids.SaveButtonId);
                    sel.ReceiptManagement.Elements.CancelButton = $("#" + sel.ReceiptManagement.Ids.CancelButtonId);
                    sel.ReceiptManagement.Elements.DeclareButton = $("#" + sel.ReceiptManagement.Ids.DeclarationAcceptButtonId);
                    sel.ReceiptManagement.Elements.ReceiptPreview = $("#ReceiptManagementPreview");
                    sel.ReceiptManagement.Elements.DeclarationModal = sel.Common.BuildJqueryDialogue("#" + sel.ReceiptManagement.Ids.DeclarationModalId, 310, 155);

                    sel.ReceiptManagement.Elements.DeclareButton.click(function () {
                        sel.ReceiptManagement.General.Save();
                        return true;
                    });

                    // handle the scroll positioning of the receipt preview
                    $(window).scroll(function () {
                        sel.ReceiptManagement.Elements.ReceiptPreview.toggleClass('scrolled', sel.ReceiptManagement.Elements.ReceiptTreeControl.height() > sel.ReceiptManagement.Elements.ReceiptPreview.height() && $(this).scrollTop() > 200);
                    });

                    if (sel.ReceiptManagement.Ids.FromClaimSelector) {

                        sel.ReceiptManagement.Elements.SaveButton.hide();
                        $("#instructions").hide();

                    } else {

                        sel.ReceiptManagement.Elements.SaveButton.bind("click", function () {

                            if (sel.ReceiptManagement.Ids.IsDeclaration == "True") {
                                sel.ReceiptManagement.Elements.DeclarationModal.dialog('open');
                            } else {
                                sel.ReceiptManagement.General.Save();
                            }

                            return false;
                        });

                        var prompt = $("#instructions #instruction-prompt");
                        var detail = $("#instructions #instruction-detail").hide();

                        prompt.click(function (event) {
                            prompt.text(detail.is(":visible") ? "Show help" : "Hide help");
                            detail.toggle(100);
                            event.preventDefault();
                        });
                    }

                    // remove the upload validation warning when a new file is selected, or a new upload is attempted
                    $("input[type=file]").on("change", function () {
                        $(".uploadValidationMessageContainer").hide();
                    });
                    $("input.saveButton").on("click", function () {
                        $(".uploadValidationMessageContainer").hide();
                    });

                    var expenseId = isNaN(sel.ReceiptManagement.Ids.CurrentExpenseId) ? null : parseInt(sel.ReceiptManagement.Ids.CurrentExpenseId);

                    // kick off
                    sel.ReceiptManagement.Elements.ReceiptTreeControl = $('#ReceiptTree');
                    sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree({
                        claimId: sel.ReceiptManagement.Ids.CurrentClaimId,
                        expenseId: expenseId,
                        showDeleteModalCallback: sel.ReceiptManagement.General.ShowDeleteModal,
                        previewCallback: sel.ReceiptManagement.General.Preview,
                        fromClaimSelector: sel.ReceiptManagement.Ids.FromClaimSelector,
                        declarationMode: sel.ReceiptManagement.Ids.IsDeclaration == "True"
                    });

                    sel.ReceiptManagement.Uploader.Init();

                    return false;
                },

                // hides the modal
                ShowDeleteModal: function (deleteCallback) {

                    if (sel.ReceiptManagement.Ids.MustGiveDeleteReason === 'True') {
                        sel.ReceiptManagement.General.DeleteCallback = deleteCallback;
                        sel.ReceiptManagement.Elements.DeleteModal.dialog('open');
                    } else {
                        deleteCallback();
                    }
                },

                // hides the modal
                HideDeleteModal: function () {
                    sel.ReceiptManagement.General.DeleteCallback = null;
                    sel.ReceiptManagement.Elements.DeleteModal.dialog('close');
                },

                // Submits a receipt delete request to the server
                DeleteReceipt: function () {
                    var reason = null;

                    if (sel.ReceiptManagement.Ids.MustGiveDeleteReason === 'True') {
                        // validate
                        reason = sel.ReceiptManagement.Elements.DeleteReasonTextBox.val();
                        if (String.isNullOrWhitespace(reason)) {
                            sel.ReceiptManagement.Elements.DeleteValidation.show();
                            return;
                        }

                        // trim reason
                        if (reason.length > 4000) {
                            reason = reason.substring(0, 4000);
                        }

                    }

                    // call the callback, passing the reason
                    sel.ReceiptManagement.General.DeleteCallback(reason);

                    // hide modal
                    sel.ReceiptManagement.General.HideDeleteModal();
                },

                // Preview a receipt if possible
                Preview: function (url, isImage, icon) {
                    if (isImage) {
                        $('#ReceiptManagementPreview').html('<img alt="" src="' + url + '">');
                    } else {
                        $('#ReceiptManagementPreview').html('<a href="' + url + '" target="_blank"><img src="' + icon + '"></div><p>This receipt cannot be viewed here, click to download it instead.</p></a></div>');
                    }

                    return false;
                },

                // shows the confirmation modal (if it exists);
                ShowConfirmModal: function () {

                    if (sel.ReceiptManagement.Ids.IsDeclaration) {
                        sel.ReceiptManagement.Elements.DeclarationModal.dialog('open');
                    }

                    return false;
                },

                // hides the confirmation modal (if it exists);
                HideConfirmModal: function () {

                    if (sel.ReceiptManagement.Ids.IsDeclaration) {
                        sel.ReceiptManagement.Elements.DeclarationModal.dialog('close');
                    }

                    sel.ReceiptManagement.Elements.SaveButton.removeAttr('disabled');
                    sel.ReceiptManagement.Elements.CancelButton.removeAttr('disabled');

                    return false;
                },

                // hides the confirmation modal (if it exists);
                Confirm: function () {

                    if (sel.ReceiptManagement.Ids.IsDeclaration) {
                        sel.ReceiptManagement.Elements.DeclarationModal.dialog('close');
                    }

                    sel.ReceiptManagement.Elements.SaveButton.removeAttr('disabled');
                    sel.ReceiptManagement.Elements.CancelButton.removeAttr('disabled');

                    return false;
                },

                // Saves the tree
                Save: function () {

                    sel.ReceiptManagement.Elements.SaveButton.attr('disabled', 'disabled');
                    sel.ReceiptManagement.Elements.CancelButton.attr('disabled', 'disabled');

                    var data = { tree: ko.mapping.toJS(sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree.viewModel()) };

                    // Call to fetch the receipt from the cloud
                    $.ajax({
                        url: "expenses/webServices/claims.asmx/UpdateClaimReceiptTree",
                        method: "POST",
                        data: JSON.stringify(data),
                        contentType: 'application/json; charset=utf-8',
                        dataType: "json",
                        success: function () {

                            sel.ReceiptManagement.Elements.SaveButton.removeAttr('disabled');
                            sel.ReceiptManagement.Elements.SaveButton.removeAttr('onclick');
                            sel.ReceiptManagement.Elements.CancelButton.removeAttr('disabled');
                            sel.ReceiptManagement.Elements.SaveButton.unbind("click");
                            sel.ReceiptManagement.Elements.SaveButton.click();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                            try {

                                var json = JSON.parse(xhr.responseText);

                                if (json.Message.indexOf("Authentication failed") > -1) {
                                    window.document.location = "/logon.aspx?returnUrl=" + window.document.location.href;
                                } else {
                                    sel.MasterPopup.ShowMasterPopup(json.Message + '<br/><span style="display:none;">' + thrownError + '</span>', 'Message from ' + window.moduleNameHTML);
                                }

                            } catch (error) {

                                sel.MasterPopup.ShowMasterPopup('An error has occurred.<br/><span style="display:none;">' + thrownError + '</span>', 'Message from ' + window.moduleNameHTML);
                            }
                            return false;
                        }
                    });

                    return false;
                }

            }, // end SEL.General


            // Functions for the uploader control in the page
            Uploader: {

                // Initialise upload form.
                Init: function () {

                    $("body").on("change", "#ReceiptFile", sel.ReceiptManagement.Uploader.Upload);
                    sel.ReceiptManagement.Elements.ReceiptUploadError = $("#ReceiptUploadError").hide();
                    return false;
                },

                // Uploads the selected receipt.
                Upload: function () {

                    var inputTag = $("#ReceiptFile");
                    
                    if (inputTag.val() != "") {

                        $('<input>').attr({ "class": "upload-clone-main", type: inputTag.attr('type'), name: "name-clone" }).insertAfter(inputTag);
                        var dynamicForm = $("<form>").attr({ method: "post", id: "dynamicImageForm", action: "/webServices/receipts.asmx/UploadReceiptToCloud" }).appendTo("body").hide();
                        dynamicForm.attr((this.encoding ? 'encoding' : 'enctype'), 'multipart/form-data').append(inputTag).hide();

                        dynamicForm.ajaxSubmit({
                            url: $("#dynamicImageForm").attr("action"),
                            type: "POST",
                            timeout: 50000,
                            beforeSend: function () {
                                sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree.beginUpload();
                                sel.ReceiptManagement.Uploader.UpdateUploadErrorText(false, "Uploading...", false);
                                return true;
                            },
                            success: function (data) {
                                // expect a node back here with the tempUrl populated
                                var xml = $(data);
                                var result = JSON.parse(xml.text());
                                if ($.isArray(result)) {
                                    var currentOutput;
                                    for (var i = 0 ; i < result.length; i++) {
                                        currentOutput = result[i];
                                        sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree.addNew(currentOutput.id, currentOutput.url, currentOutput.icon, currentOutput.isCurrentReceiptImage);
                                    }
                                }
                                else {
                                    sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree.addNew(result.id, result.url, result.icon, result.isImage);
                                }
                                sel.ReceiptManagement.Elements.ReceiptUploadError.hide();
                                var element = document.getElementById("dynamicImageForm");
                                element.parentNode.removeChild(element);
                                return false;
                            },
                            error: function (xhr) {                              
                                var message = xhr.responseText.replace("System.Web.HttpException: ", "").replace("System.ArgumentException: ", "").split("\r")[0];
                                sel.ReceiptManagement.Uploader.UpdateUploadErrorText(true, "An error has occurred during upload. " + message, true);
                                return false;
                            },
                            complete: function () {
                                $("body").remove("#dynamicImageForm");
                                sel.ReceiptManagement.Elements.ReceiptTreeControl.ReceiptTree.resetFileInput();
                                sel.ReceiptManagement.Elements.ReceiptTreeControl.removeAttr('disabled');
                                sel.ReceiptManagement.Elements.SaveButton.removeAttr('disabled');
                            }
                        });
                    }

                    return false;

                }, // end SEL.ReceiptManagement.Uploader.Upload

                UpdateUploadErrorText: function (isError, message) {
                    sel.ReceiptManagement.Elements.ReceiptUploadError.html(message).css('color', isError ? 'red' : 'black').show();
                }


            } // end SEL.ReceiptManagement.Uploader


        }; // end SEL.ReceiptManagement


    } // end execute()

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL));

// ReSharper disable DuplicatingLocalDeclaration
// ReSharper disable InconsistentNaming
// JQuery Plugin for managing receipts
; (function ($, window, document) {

    // Create the defaults once
    var pluginName = 'ReceiptTree';
    var defaults = {
        claimId: 0,
        expenseId: 0,
        getUrl: "expenses/webServices/claims.asmx/GetClaimReceiptTree",
        uploadUrl: "/webServices/receipts.asmx/UploadReceiptToCloud",
        showDeleteModalCallback: function () { return ""; },
        previewCallback: function () { },
        declarationMode: false
    };

    // The actual plugin constructor
    function ReceiptTree(element, options) {

        // PROPERTIES
        this.element = element;
        this.window = window;
        this.$window = $(window);
        this.document = document;
        this.viewModel = {}
        this.moveCopyButtons = null;
        this.moveButton = null;
        this.copyButton = null;
        this.uploadPlaceHolder = null;
        this.lastUploadTarget = null;
        this.uploading = false;
        this.rootMapping = {};
        this.currentItem = null;
        this.copyWasClicked = null;
        this.allowCopyOnly = null;
        this.lastTimeoutId = null;
        this.fromClaimSelector = null;
        this.declarationMode = false;
        this.receiptFileInputMarkup = '<input type="file" id="ReceiptFile" name="receipt" title="Click to upload a receipt"></input>';

        this.activeTimeouts = [];

        // set up the options
        this.options = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        // start a fight
        this.init();
    }

    // initialisation
    ReceiptTree.prototype.init = function () {

        var base = this;

        base.rootMapping = {
            "__type": "SpendManagementLibrary.Expedite.DTO.ReceiptManagementClaim",
            'Header': {
                create: function (options) {
                    return new ReceiptManagementHeader(options.data);
                },
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            },
            "Children": {
                create: function (options) {
                    return new ReceiptManagementExpense(options.data);
                },
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            },
            "DeletedReceipts": {
                create: function (options) {
                    return new ReceiptManagementDeletedReceiptInfo(options.data);
                },
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            }
        };

        function ReceiptManagementHeader(data) {

            var self = this;

            var mapping = {
                "Children": {
                    create: function (options) {
                        return new ReceiptManagementReceipt(options.data);
                    },
                    key: function (data) {
                        return ko.utils.unwrapObservable(data.Id);
                    }
                }
            }

            return ko.mapping.fromJS(data, mapping, self);
        };

        function ReceiptManagementExpense(data) {
            var self = this;

            var mapping = {
                "Children": {
                    create: function (options) {
                        return new ReceiptManagementReceipt(options.data);
                    },
                    key: function (data) {
                        return ko.utils.unwrapObservable(data.Id);
                    }
                }
            }

            return ko.mapping.fromJS(data, mapping, self);
        };

        function ReceiptManagementReceipt(data) {
            var self = this;
            return ko.mapping.fromJS(data, self);
        };

        function ReceiptManagementDeletedReceiptInfo(data) {
            var self = this;
            return ko.mapping.fromJS(data, self);
        };

        // ReSharper restore DuplicatingLocalDeclaration
        // ReSharper restore InconsistentNaming

        base.uploadPlaceHolder = $('<a id="UploadPlaceHolder" title="Click to upload a receipt" class="prevent">' + base.receiptFileInputMarkup + '</a>').click(function () {
            $("#ReceiptFile").change();
        }).hide();

        base.moveCopyButtons = $('<div id="ReceiptCopyMove"><a id="ReceiptCopyMoveButtonMove">move</a><a id="ReceiptCopyMoveButtonCopy">copy</a></div>').hide();
        base.moveButton = base.moveCopyButtons.find('#ReceiptCopyMoveButtonMove');
        base.copyButton = base.moveCopyButtons.find('#ReceiptCopyMoveButtonCopy');

        $(base.document.body).append(base.moveCopyButtons);
        $(base.document.body).append(base.uploadPlaceHolder);

        // METHODS

        // handles the toggling of a receipt container, where you can force open or closed using the optional param
        ReceiptTree.prototype.toggleReceiptContainer = function (target, forceOpenOrClose) {
            var infoBox = $(target);
            var receiptHolder = infoBox.next();

            (forceOpenOrClose === true || forceOpenOrClose === false)
                ? (forceOpenOrClose ? open() : close())
                : (receiptHolder.is(':visible') ? close() : open());

            function open() {
                infoBox.css("background-image", "url(/shared/images/icons/24/plain/notebook3.png)").css("font-weight", "bold");
                receiptHolder.stop().show(300);
            }

            function close() {
                infoBox.css("background-image", "url(/shared/images/icons/24/plain/notebook.png)").css("font-weight", "normal");
                receiptHolder.stop().hide(150);
            }
        };

        // clears a timeout
        base.clearTimeouts = function () {

            if (!base.activeTimeouts.length) return;
            while (base.activeTimeouts.length) clearTimeout(base.activeTimeouts.pop());
        };

        $.fn.classList = function() {return this[0].className.split(/\s+/); }

        ReceiptTree.prototype.showUpload = function (data, event) {

            if (base.uploading) return;

            base.clearTimeouts();

            if (!base.options.fromClaimSelector) {

                var node = event.toElement ? $(event.toElement) : $(event.target);

                if (node.is('#ReceiptFile')) return;
                if (!node.is(".ko_container")) node = $(node.parents(".ko_container")[0]);

                if(node && node.length == 1 && node.hasClass("ko_container")) {
                    base.lastUploadTarget = ko.dataFor(node[0]);
                    node.append(base.uploadPlaceHolder.show());
                    base.activeTimeouts.push(base.lastTimeoutId);
                }
            }
        }

        ReceiptTree.prototype.hideUpload = function (data, event) {

            if (base.uploading) return;

            base.clearTimeouts();

            var toElement = event.toElement ? $(event.toElement) : $(event.relatedTarget).parent();
            var isUploader = toElement.is("#UploadPlaceHolder") || toElement.is("#ReceiptFile");
            var targetIsContainer = toElement.is(".ko_container");
            
            if (!isUploader && !targetIsContainer) {

                base.lastTimeoutId = setTimeout(function () {
                    if (navigator.userAgent.indexOf("Firefox") < 0) {
                        $(document.body).append(base.uploadPlaceHolder.hide());
                    }
                    base.clearTimeouts();
                }, 800);

                base.activeTimeouts.push(base.lastTimeoutId);
            }
        }

        // deletes an item from the viewModel, updating a list to add the reason why.
        ReceiptTree.prototype.deleteItem = function (item, parent, context) {

            if (base.uploading) return;

            // clear the preview image if it's the one we're deleting
            var previewImage = $("#ReceiptManagementPreview img");
            if (item.Url() === previewImage.attr("src")) {
                previewImage.hide();
            }

            // call to show the modal, passing in a callback 
            base.options.showDeleteModalCallback(function (reason) {

                var parentId = parent == context ? null : parent.Id();
                var unlinkData = { Id: item.Id(), ParentId: parentId, Reason: reason };

                // add the deleted receipt to the list
                context.DeletedReceipts.push(new ReceiptManagementDeletedReceiptInfo(unlinkData));

                // remove from VM
                parentId ? parent.Children.remove(item) : context.Header.Children.remove(item);
            });
        };

        // deletes an item from the viewModel, updating a list to add the reason why.
        ReceiptTree.prototype.preview = function (item) {
            var url = item.Url();
            var isImage = item.IsImage();
            var icon = item.Icon();
            if (typeof (url) === "string") {
                base.options.previewCallback(url, isImage, icon);
            }
        };

        // adds an src binder to knockout
        ko.bindingHandlers.src = {
            update: function (element, valueAccessor) {
                ko.bindingHandlers.attr.update(element, function () {
                    return { src: valueAccessor() }
                });
            }
        };

        // set up the drop handler
        ko.bindingHandlers.sortable.afterMove = function (arg, event) {

            var posx = 0, posy = 0;

            if (event.pageX || event.pageY) {

                posx = event.pageX - base.$window.scrollLeft();
                posy = event.pageY - base.$window.scrollTop();

            } else if (event.clientX || event.clientY) {

                posx = event.clientX - base.$window.scrollLeft();
                posy = event.clientY - base.$window.scrollTop();

            }

            var node = $(event.target);

            if (node.hasClass('ko_container')) {

                // save the target
                base.currentItem = { arguments: arg, targetContext: ko.dataFor(event.target) };
                base.copyWasClicked = null;

                if (arg.sourceParentNode != event.target) {

                    // determine if the source was allowed to be deleted from (will affect whether to allow 'move')
                    var sourceData = ko.dataFor(arg.sourceParentNode[0]);
                    base.allowCopyOnly = sourceData.PreventDelete && sourceData.PreventDelete();

                    if (base.allowCopyOnly) {
                        base.moveButton.hide();
                    }

                    // offset so under mouse
                    posx -= base.moveCopyButtons.width() * 0.25;
                    posy -= base.moveCopyButtons.height() * 0.5;

                    base.moveCopyButtons.css("top", posy + "px");
                    base.moveCopyButtons.css("left", posx + "px");
                    base.moveCopyButtons.show();
                }
            }
        };

        // copies the current item using the info in base.currentItem
        function duplicateCurrentItem() {

            if (base.currentItem != null) {
                var original = base.currentItem.arguments.item;
                var options = { Id: original.Id(), Url: original.Url(), IsImage: original.IsImage(), Icon: original.Icon, PreventDelete: original.PreventDelete(), EditMessage: original.EditMessage() };
                var clone = base.viewModel.clone(options);
                base.currentItem.arguments.sourceParent.splice(base.currentItem.arguments.sourceIndex, 0, clone);
                resetMoveCopyInterface();
            }
        }

        // resets the move / copy ui
        function resetMoveCopyInterface() {
            base.allowCopyOnly = null;
            base.moveButton.show();
            base.moveCopyButtons.hide();
            base.currentItem = null;
            base.copyWasClicked = null;
        }

        // handle the copy
        base.copyButton.click(function () { duplicateCurrentItem(); });

        // handle the move
        $(document).click(function (event) {

            if (base.uploading) return;

            if (!$(event.target).closest('#ReceiptCopyMoveButtons').length && base.moveCopyButtons.is(":visible")) {

                (event.preventDefault) ? event.preventDefault() : event.returnValue = false;

                if (base.allowCopyOnly || (base.currentItem != null && base.copyWasClicked != true)) {

                    if (base.allowCopyOnly) {

                        // override the move to be a copy by default
                        duplicateCurrentItem();

                    } else {

                        // standard move
                        if (base.currentItem.arguments.targetParent != base.currentItem.arguments.sourceParent) {

                            var original = base.currentItem.arguments.item;
                            var dpData = ko.dataFor(base.currentItem.arguments.sourceParentNode[0]);
                            var parentId = base.currentItem.arguments.sourceParent === base.viewModel.Header.Children ? null : dpData.Id();
                            var unlinkData = { Id: original.Id(), ParentId: parentId, Reason: null };

                            // add the deleted receipt to the list
                            base.viewModel.DeletedReceipts.push(new ReceiptManagementDeletedReceiptInfo(unlinkData));
                        }

                        resetMoveCopyInterface();
                    }
                }
            }
        });
        
        // get the data from the url
        $.ajax({
            url: base.options.getUrl,
            method: "POST",
            data: '{ "claimId": ' + this.options.claimId + ', "fromClaimSelector": ' + this.options.fromClaimSelector + ' }',
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            success: function (data) {

                // grab the data and convert to knockout
                base.viewModel = ko.mapping.fromJS(data.d, base.rootMapping);
                base.viewModel.deleteItem = base.deleteItem;
                base.viewModel.preview = base.preview;
                base.viewModel.showUpload = base.showUpload;
                base.viewModel.hideUpload = base.hideUpload;
                base.viewModel.declarationMode = base.options.declarationMode;
                base.viewModel.clone = function(options) {
                    return new ReceiptManagementReceipt(options);
                };
                base.viewModel.beginUpload = function() {
                    base.uploading = true;
                };
                base.viewModel.resetFileInput = function() {
                    base.lastUploadTarget = null;
                    base.uploading = false;
                    base.uploadPlaceHolder.html($(base.receiptFileInputMarkup));
                };
                base.viewModel.addNew = function(id, url, icon, isImage) {
                    if (base.lastUploadTarget) {
                        base.lastUploadTarget.Children.push(new ReceiptManagementReceipt({
                            Id: id,
                            Url: url,
                            IsImage: isImage,
                            Icon: icon,
                            PreventDelete: base.lastUploadTarget.PreventDelete(),
                            EditMessage: base.lastUploadTarget.EditMessage()
                        }));
                    }
                };

                // bind
                ko.applyBindings(base.viewModel);

                // create new mothods now we have the viewmodel
                $.fn[pluginName].clone = base.viewModel.clone;
                $.fn[pluginName].addNew = base.viewModel.addNew;
                $.fn[pluginName].viewModel = function () { return base.viewModel; }
                $.fn[pluginName].beginUpload = base.viewModel.beginUpload;
                $.fn[pluginName].resetFileInput = base.viewModel.resetFileInput;

                // toggle them all open or just the one, depending on if an ExpenseId is passed
                if (base.options.expenseId) {
                    $('.receipt-management-expense-info-id:not(:contains(' + base.options.expenseId + "))").parent().each(function (index, element) {
                        base.toggleReceiptContainer(element, false);
                    });
                    base.toggleReceiptContainer($('.receipt-management-expense-info-id:contains(' + base.options.expenseId + ")").parent(), true);
                }

                $('.receipt-management-expense-info').click(function () { base.toggleReceiptContainer(this); });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                // ReSharper disable once UseOfImplicitGlobalInFunctionScope
                SEL.MasterPopup.ShowMasterPopup('The claim could not be found.<br/><span style="display:none;">' + thrownError + '</span>', 'Message from ' + window.moduleNameHTML);
                return false;
            }
        });
    };

    // A really lightweight plugin wrapper around the constructor, 
    // preventing against multiple instantiations
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, 'plugin_' + pluginName)) {
                $.data(this, 'plugin_' + pluginName,
                new ReceiptTree(this, options));
            }
        });
    }

})(jQuery, window, document);

//receipts chrome

$(document).ready(function () {

    var isChromium = window.chrome,
    vendorName = window.navigator.vendor,
    isOpera = window.navigator.userAgent.indexOf("OPR") > -1,
    isIEedge = window.navigator.userAgent.indexOf("Edge") > -1;
    if (isChromium !== null && isChromium !== undefined && vendorName === "Google Inc." && isOpera == false && isIEedge == false) {

        $(document).on("mousedown", "#ReceiptTree .receipt-management-receipt", function () {

            $(this).closest(".receipt-management-receipt").css("opacity", "0");

            var yPosition = window.pageYOffset;
            setTimeout(function () {
                $(".ui-sortable-helper").css("margin-top", yPosition + "px");
                $(".receipt-management-receipt").css("opacity", "1");
            }, 150);


        });

        $(document).on("mouseup", "#ReceiptTree .receipt-management-receipt", function () {
            $(".receipt-management-receipt").css("margin-top", "0px");
        });

    };
});