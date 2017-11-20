(function () {
    var scriptName = "LogonMessages";

    function execute() {
        SEL.registerNamespace("SEL.LogonMessages");

        SEL.LogonMessages =
        {
            /* Dom ID's */
            /* Logon AdminPanel Elements */
            ModalIcon: null,
            ModalBackgroundImage: null,
            ModalCategoryTitle: null,
            ModalCategoryColor: null,
            ModalHeader: null,
            ModalHeaderColor: false,
            ModalBody: null,
            ModalBodyColor: null,
            ModalButton: null,
            ModalButtonLink: false,
            ModalButtonColor: null,
            ModalButtonBackground: false,
            ModalHdBackground: null,
            ModalHdIcon: null,
            ModalModules: null,
            ModalPreviewButton: null,
            ModalSaveButton: null,
            InitialModulesListStore: null,
            IsValidIconCheck: null,
            IsValidImageCheck: null,
            ImageValidation: function () {
                $('#' + SEL.LogonMessages.ModalBackgroundImage.id).change(function () {
                    var filename = $(this).val();
                    var fileSize = $(this).prop('files')[0].size;
                    if (!(/\.(jpg|jpeg|JPG|JPEG)$/i).test(filename)) {
                        $(this).val('');
                        SEL.MasterPopup.ShowMasterPopup('Please select jpg/jpeg file.', 'Message from ' + moduleNameHTML);
                        var new_val = $(this).val();
                        if (new_val !== "") {
                            $(this).replaceWith($(this).clone(true));
                        }
                    } else if (fileSize > 150000) {
                        $(this).replaceWith($(this).clone(true));
                        SEL.MasterPopup.ShowMasterPopup('Maximum file size of 150kb is allowed.');
                    }
                    else {
                        filename = filename.replace(/.*(\/|\\)/, '');
                        if (filename.length > 25) {
                            $('.backroundNameHolder').html(filename.substring(0, 10) + "..." + filename.substring(filename.length - 8));
                        } else {
                            $('.backroundNameHolder').html(filename);
                        }
                    }
                });

                $('#' + SEL.LogonMessages.ModalIcon.id).change(function () {
                    var filename = $(this).val();
                    var fileSize = $(this).prop('files')[0].size;
                    if (!(/\.(jpg|jpeg|png|JPG|JPEG|PNG)$/i).test(filename)) {
                        $(this).val('');
                        SEL.MasterPopup.ShowMasterPopup('Please select jpg/jpeg/png file.', 'Message from ' + moduleNameHTML);
                        var new_val = $(this).val();
                        if (new_val !== "") {
                            $(this).replaceWith($(this).clone(true));
                        }
                    }
                    else if (fileSize > 150000) {
                        $(this).replaceWith($(this).clone(true));
                        SEL.MasterPopup.ShowMasterPopup('Maximum file size of 150kb is allowed.');
                    }
                    else {
                        $('.logonIconDetailsWrapper').removeClass('hideElement');
                        $('.removeIconHolder').show();
                        filename = filename.replace(/.*(\/|\\)/, '');
                        $('#' + SEL.LogonMessages.ModalHdIcon.id).val(filename);
                        if (filename.length > 25) {
                            $('.fileNameHolder').html(filename.substring(0, 10) + "..." + filename.substring(filename.length - 8));
                        } else {
                            $('.fileNameHolder').html(filename);
                        }
                    }
                });
            },
            PageLoadFunctions: function () {
                SEL.LogonMessages.ImageValidation();
                var backgroundImage = SEL.LogonMessages.ModalBackgroundImage;
                var iconImage = SEL.LogonMessages.ModalIcon;
                var header = SEL.LogonMessages.ModalHeader;
                var headerColor = SEL.LogonMessages.ModalHeaderColor;
                var title = SEL.LogonMessages.ModalCategoryTitle;
                var titleColor = SEL.LogonMessages.ModalCategoryColor;
                var body = SEL.LogonMessages.ModalBody;
                var bodyColor = SEL.LogonMessages.ModalBodyColor;
                var buttonText = SEL.LogonMessages.ModalButton;
                var buttonLink = SEL.LogonMessages.ModalButtonLink;
                var buttonColor = SEL.LogonMessages.ModalButtonColor;
                var buttonBackground = SEL.LogonMessages.ModalButtonBackground;
                var hdBackground = SEL.LogonMessages.ModalHdBackground;
                var hdIcon = SEL.LogonMessages.ModalHdIcon;
                var referenceModules = SEL.LogonMessages.ModalModules;
                var saveButton = SEL.LogonMessages.ModalSaveButton;
                $('.CmdPreview').click(function (e) {
                    e.preventDefault();
                    SEL.LogonMessages.ClearSession();

                    if (SEL.Common.ValidateForm('vgAddEditLogonMessages') === false) {
                        return false;
                    }
                    if ($('#' + backgroundImage.id).val() === null || $('#' + backgroundImage.id).val() === '') {
                        sessionStorage.setItem('initialBackGroundImage', $('#' + hdBackground.id).val());
                    }
                    if ($('#' + iconImage.id).val() === null || $('#' + iconImage.id).val() === '') {
                        sessionStorage.setItem('initialIconImage', $('#' + hdIcon.id).val());
                    }
                    SEL.LogonMessages.UploadImage($('#' + iconImage.id).prop('files'), 'iconForTitle');
                    SEL.LogonMessages.UploadImage($('#' + backgroundImage.id).prop('files'), 'backGroundImage');
                    sessionStorage.setItem('LogonCategoryTitle', $('#' + title.id).val());
                    sessionStorage.setItem('LogonCategoryTitleColor', $('#' + titleColor.id).val());
                    sessionStorage.setItem('LogonHeader', $('#' + header.id).val());
                    sessionStorage.setItem('LogonHeaderColor', $('#' + headerColor.id).val());
                    sessionStorage.setItem('LogonBody', $('#' + body.id).val().replace(/\r?\n/g, '<br />'));
                    sessionStorage.setItem('LogonBodyColor', $('#' + bodyColor.id).val());
                    sessionStorage.setItem('ButtonText', $('#' + buttonText.id).val());
                    sessionStorage.setItem('ButtonBackground', $('#' + buttonBackground.id).val());
                    sessionStorage.setItem('ButtonLink', $('#' + buttonLink.id).val());
                    sessionStorage.setItem('ButtonTextColor', $('#' + buttonColor.id).val());
                    var page = "../logon.aspx?previewId=" + sessionStorage.getItem('backGroundImage');
                    var $dialog = $('<div></div>')
                                   .html('<iframe style="border: 0px; " src="' + page + '" width="100%" height="100%"></iframe>')
                                   .dialog({
                                       autoOpen: false,
                                       modal: true,
                                       height: 800,
                                       width: 1300,
                                       title: "Logon Preview"
                                   });
                    setTimeout($dialog.dialog('open'), 1000);
                });

                var activeModule = $('#' + referenceModules.id).val().split(",");
                $('input[type="checkbox"]').each(function () {
                    if (activeModule.indexOf($(this).attr('data-ref')) > -1) {
                        $(this).attr('checked', true);
                    }
                });

                var failedModules = SEL.LogonMessages.InitialModulesListStore;
                if (failedModules !== "" && failedModules != null) {
                    SEL.MasterPopup.ShowMasterPopup('This message cannot be applied to ' + failedModules.replace(/,([^,]*)$/, ' and $1') + ' as there are 3 messages already active for this product.');
                }
                var isValiBackground = SEL.LogonMessages.IsValidImageCheck;          
                var isValidIcon = SEL.LogonMessages.IsValidIconCheck;
                var validationMessage = "";
                if (isValiBackground != 'True') {
                    validationMessage = "The background image must have dimensions of 840 x 1050px.";
                }
                
                if (isValidIcon != 'True') {
                    if (validationMessage !== "") {
                        validationMessage += "<br/>The icon must have dimensions of 37 x 37px.";
                    } else {
                        validationMessage += "The icon must have dimensions of 37 x 37px.";
                    }
                }
                if (validationMessage !== "") {
                    SEL.MasterPopup.ShowMasterPopup(validationMessage);
                }

                activeModule = '';
                $('#' + saveButton.id).click(function (e) {
                    $('#' + referenceModules.id).val('');
                    $('input:checkbox:checked').each(function () {
                        activeModule += $(this).attr('data-ref') + ',';
                    });
                    if (activeModule.length > 0) {
                        activeModule = activeModule.substring(0, activeModule.length - 1) + ',0';
                    }
                    $('#' + referenceModules.id).val(activeModule);
                });

                $('.removeIconHolder').click(function () {
                    $('.fileNameHolder').html('');
                    $(this).hide();
                    $('#' + iconImage.id).val("");
                    $('#' + hdIcon.id).val("");
                });
                if ($('#' + hdIcon.id).val() != null && $('#' + hdIcon.id).val() !== "") {
                    $('.logonIconDetailsWrapper').removeClass('hideElement');
                }
                $('.fileNameHolder').html($('#' + hdIcon.id).val());


            },
            ClearSession: function () {
                sessionStorage.removeItem('iconForTitle');
                sessionStorage.removeItem('LogonCategoryTitle');
                sessionStorage.removeItem('LogonCategoryTitleColor');
                sessionStorage.removeItem('LogonHeader');
                sessionStorage.removeItem('LogonHeaderColor');
                sessionStorage.removeItem('LogonBody');
                sessionStorage.removeItem('LogonBodyColor');
                sessionStorage.removeItem('backGroundImage');
                sessionStorage.removeItem('ButtonText');
                sessionStorage.removeItem('ButtonTextColor');
                sessionStorage.removeItem('ButtonBackground');
                sessionStorage.removeItem('ButtonLink');
                sessionStorage.removeItem('initialIconImage');
            },
            UploadImage: function (imageContent, imageID) {
                var url = window.URL || window.webkitURL;
                var file, img;
                if ((file = imageContent[0])) {
                    img = new Image();
                    img.onload = function () {
                        if (status !== 'error') {
                            SEL.LogonMessages.SendFile(file, imageID);
                        }
                    };
                    img.onerror = function () {
                        SEL.MasterPopup.ShowMasterPopup("Not a valid file:" + file.type);
                    };
                    img.src = url.createObjectURL(file);
                }
            },
            SendFile: function (file, imageID) {
                var formData = new FormData();
                formData.append('file', $('.' + imageID)[0].files[0]);
                $.ajax({
                    type: 'post',
                    url: '../logonBannerUploader.ashx',
                    data: formData,
                    success: function (status) {
                        sessionStorage.setItem(imageID, status);
                    },
                    processData: false,
                    contentType: false,
                    error: function () {
                        SEL.MasterPopup.ShowMasterPopup('Upload Failed');
                    }
                });
            },
            DeleteLogonMessage: function (id) {
                if (confirm('Are you sure you wish to delete the selected marketing information?')) {
                    Spend_Management.shared.webServices.svcLogonMessages.DeleteLogonMessage(id);
                    SEL.Grid.deleteGridRow('LogonMessages', id);
                }
            },

            ChangeStatus: function (id, status) {
                Spend_Management.shared.webServices.svcLogonMessages.ChangeStatus(id, status, SEL.LogonMessages.ChangeStatusComplete);
            },

            ChangeStatusComplete: function (data) {
                if (data !== null) {
                    SEL.MasterPopup.ShowMasterPopup("Message cannot be activated. There are already three active messages in " + data.replace(/,([^,]*)$/, ' and $1'));
                }

                SEL.Grid.refreshGrid('LogonMessages');
            }
        }
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}());