(function (moduleNameHtml, appPath)
{
    var scriptName = "notificationTemplates";

    function execute()
    {
        SEL.registerNamespace("SEL.NotificationTemplates");
        SEL.NotificationTemplates = {
            Elements:
            {
                recipientModalForm: null,
                modattachmentid: null,
                moduploadid: null,

                hdntoid: null,
                hdnccid: null,
                hdnbccid: null,
                txtsubjectid: null,
                cmbteamid: null,
                cmbbudgetid: null,
                cmbotherid: null,
                cmbGreenLightAttribute: null,
                reqTo: null,

                editorid: null,
                txttemplatenameid: null,
                cmbpriorityid: null,
                cmbareaid: null,
                chksystemtempateid: null,
                chksystemtempateidinitialval: null,

                chksendNotes: null,
                chkSendEmail: null,
                chkCanSendMobileNotification: null,
                mobileNotificationMessage: null,
                emailNotes: null,
                notesHeader: null,
                lblSendNote: null,
                noteswrapper: null,
                rtBodyText: null,
                hdnToInitialValues: null,

                baseTreeData: null,
                employeeTreeData: null,
                RecipientModal: undefined,
                TooltipTimer:null
            },

            RecipientModal: {
                type: null
            },

            RecipientModalInitialise: function ()
            {
                SEL.NotificationTemplates.Elements.RecipientModal = $("#" + SEL.NotificationTemplates.Elements.recipientModalForm).dialog({
                    modal: true,
                    title: "Add Recipient",
                    width: 870,
                    height: 226,
                    resizable: false,
                    closeOnEscape: true,
                    autoOpen: false,
                    buttons: [
                        {
                            text: "ok",
                            click: SEL.NotificationTemplates.GetRecipient
                        }, {
                            text: "cancel",
                            click: function() {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            },

            ShowAddRecipientModal : function (recType)
            {
                SEL.NotificationTemplates.RecipientModal.type = recType;
                SEL.NotificationTemplates.ClearRecipientModal();
                SEL.NotificationTemplates.Elements.RecipientModal.dialog("open");
            },

            HideAddRecipientModal: function ()
            {
                SEL.NotificationTemplates.Elements.RecipientModal.dialog("close");
            },

            GetTokenInputPrePopulate: function (hiddenFieldElementSelector)
            {
                var tokenInputPrePopulate = [];
                if ($(hiddenFieldElementSelector).val()) {
                    tokenInputPrePopulate = $.map($(hiddenFieldElementSelector).val().split("; "), function (item) {
                        return {
                            id: item + "; ",
                            name: item.substring(0, 1) === "{" ? item.substring(1, item.length - 1) : item
                    };
                    });
                }
                tokenInputPrePopulate.pop();

                return tokenInputPrePopulate;
            },

            InitialiseTokenInputPlugin: function ()
            {
                var options = {
                    method: "POST",
                    contentType: "json",
                    jsonContainer: "d",
                    searchDelay: 500,
                    minChars: 3,
                    preventDuplicates: true,
                    hintText: "Begin typing to search for an employee or click the plus icon for more options",
                    tokenFormatter: function (item) { return "<li><p>" + item.name + "</p></li>" },
                    resultsFormatter: function (item) { return "<li>" + item.searchDisplay + "</li>" },
                    onSend: function (params) {
                        params.contentType = "application/json";
                        // convert the data to a string if necessary
                        if ($.type(params.data) === 'object') {
                            params.data = JSON.stringify(params.data);
                        }
                    }
                };
                
                $('#txtTo').tokenInput(SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "SearchEmployeeEmailByNameAndUsername" }), $.extend(options, {
                    prePopulate: SEL.NotificationTemplates.GetTokenInputPrePopulate("#" + SEL.NotificationTemplates.Elements.hdntoid),
                    onAdd: function (item, textSearched) {
                        SEL.NotificationTemplates.AddInputToken('txtTo', SEL.NotificationTemplates.Elements.hdntoid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.NotificationTemplates.Elements.hdntoid).val($("#" + SEL.NotificationTemplates.Elements.hdntoid).val().replace(item.id, ""));
                    }
                }));

                $('#txtCC').tokenInput(SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "SearchEmployeeEmailByNameAndUsername" }), $.extend(options, {
                    prePopulate: SEL.NotificationTemplates.GetTokenInputPrePopulate("#" + SEL.NotificationTemplates.Elements.hdnccid),
                    onAdd: function (item, textSearched)
                    {
                        SEL.NotificationTemplates.AddInputToken('txtCC', SEL.NotificationTemplates.Elements.hdnccid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.NotificationTemplates.Elements.hdnccid).val($("#" + SEL.NotificationTemplates.Elements.hdnccid).val().replace(item.id, ""));
                    }
                }));

                $('#txtBCC').tokenInput(SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "SearchEmployeeEmailByNameAndUsername" }), $.extend(options, {
                    prePopulate: SEL.NotificationTemplates.GetTokenInputPrePopulate("#" + SEL.NotificationTemplates.Elements.hdnbccid),
                    onAdd: function (item, textSearched)
                    {
                        SEL.NotificationTemplates.AddInputToken('txtBCC', SEL.NotificationTemplates.Elements.hdnbccid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.NotificationTemplates.Elements.hdnbccid).val($("#" + SEL.NotificationTemplates.Elements.hdnbccid).val().replace(item.id, ""));
                    }
                }));
            },

            GetRecipient: function ()
            {
                var username;
                var splitval1;
                var teamddl = $get(SEL.NotificationTemplates.Elements.cmbteamid);
                var teamid = teamddl.options[teamddl.selectedIndex].value;
                var budgetddl = $get(SEL.NotificationTemplates.Elements.cmbbudgetid);
                var budgetid = budgetddl.options[budgetddl.selectedIndex].value;
                var otherddl = $get(SEL.NotificationTemplates.Elements.cmbotherid);
                var senderType = otherddl.options[otherddl.selectedIndex].value;
                var sender = otherddl.options[otherddl.selectedIndex].text;
                var greenlightddl = $get(SEL.NotificationTemplates.Elements.cmbGreenLightAttribute);
                var greenLightId = greenlightddl.options[greenlightddl.selectedIndex].value;
                username = '';
                if (teamid === '')
                {
                    teamid = 0;
                }

                if (budgetid === '')
                {
                    budgetid = 0;
                }

                else {
                    Spend_Management.svcNotificationTemplates.getRecipientInfo(username, teamid, budgetid, senderType, sender, greenLightId, function(data) {

                        var recipientFieldSelector;
                        switch (SEL.NotificationTemplates.RecipientModal.type) {
                            case "to":
                                recipientFieldSelector = "#txtTo";
                                break;
                            case "cc":
                                recipientFieldSelector = "#txtCC";
                                break;
                            case "bcc":
                                recipientFieldSelector = "#txtBCC";
                                break;
                        }

                        $.each(data, function(index, item) {
                            $(recipientFieldSelector).tokenInput("add", item);
                        });

                        SEL.NotificationTemplates.HideAddRecipientModal();

                    }, SEL.NotificationTemplates.CommandFail);
                }
            },

            ValidateBodyTextLength: function (source, args) {
                var txtVal;
                for (var i in CKEDITOR.instances) {
                    if (i == SEL.NotificationTemplates.Elements.editorid) {
                        txtVal = CKEDITOR.instances[i].getData();
                    }
                }
                if (txtVal == "") {
                    args.IsValid = false;
                }

                return;
            },

            ValidateSubjectTextLength: function (source, args)
            {
                var txtVal = $('#' + SEL.NotificationTemplates.Elements.txtsubjectid).val();
                
                if (txtVal === "")
                {
                    args.IsValid = false;
                }

                return;
            },

            ValidateBroadcastMessageLength: function (source, args)
            {
                if ($get(SEL.NotificationTemplates.Elements.chksendNotes)) {
                    isSendNote = $get(SEL.NotificationTemplates.Elements.chksendNotes).checked;
                    if (isSendNote) {
                        var txtVal = $('#' + SEL.NotificationTemplates.Elements.emailNotes).val();

                        if (txtVal === "") {
                            args.IsValid = false;
                        }
                    }
                }
                return;
            },

            ValidateMobileNotificationMessageIsRequired: function (source, args) {
                if ($get(SEL.NotificationTemplates.Elements.chkCanSendMobileNotification)) {
                    canSendMobileNotification = $get(SEL.NotificationTemplates.Elements.chkCanSendMobileNotification).checked;
                    if (canSendMobileNotification) {
                        var txtVal = $('#' + SEL.NotificationTemplates.Elements.mobileNotificationMessage).val();

                        if (txtVal === "") {
                            args.IsValid = false;
                        }
                    }
                }
                return;
            },

            ValidateMobileNotificationMessageLength: function (source, args) {

                var txtVal = $('#' + SEL.NotificationTemplates.Elements.mobileNotificationMessage).val();
                if (txtVal !== "" && txtVal.length > 400) {
                    args.IsValid = false;
                }
                return;
            },

            ValidateToRecipient: function (source, args) {

                var toVal = $get(SEL.NotificationTemplates.Elements.hdntoid).value;
                if (toVal === "" || toVal === null) {
                    args.IsValid = false;
                }
                if ($get(SEL.NotificationTemplates.Elements.chksystemtempateid)) {
                    isSystemTemplate = $get(SEL.NotificationTemplates.Elements.chksystemtempateid).checked;
                    if (isSystemTemplate) {
                        args.IsValid = true;
                    }
                }
                return;

            },

            SetBodyHeight: function () {
                if ($('#' + SEL.NotificationTemplates.Elements.chksystemtempateid).is(":visible") === false) {
                    $('#cke_' + SEL.NotificationTemplates.Elements.rtBodyText).find('div.cke_contents').css('height', '560px');
                } 
            },

            OnSystemTemplateCheckChanged: function (sender)
            {
                var checkbox = document.getElementById(sender).checked;
                if (SEL.NotificationTemplates.Elements.chksystemtempateidinitialval == null) SEL.NotificationTemplates.Elements.chksystemtempateidinitialval = !checkbox;
                var selectedProductArea = $('#' + SEL.NotificationTemplates.Elements.cmbareaid +' :selected').closest('optgroup').attr('label');
                if (checkbox && selectedProductArea !== 'GreenLights')
                {
                    $('#' + SEL.NotificationTemplates.Elements.reqTo).css('visibility', 'hidden');
                    $('#' + SEL.NotificationTemplates.Elements.reqTo).prop('disabled', true);
                    // if it's system template we need to show all the relevant notes fields
                    SEL.NotificationTemplates.ShowNoteRelatedFields();
                }
                else {
                    $('#' + SEL.NotificationTemplates.Elements.reqTo).prop('disabled', false);
                    // if it isn't system template we need to hide all the relevant notes fields
                    SEL.NotificationTemplates.HideNoteRelatedFields();
                }
            },

            OnProductAreaChange : function() {
                $('#' + SEL.NotificationTemplates.Elements.cmbareaid).change(function () {
                    if ($('#' + SEL.NotificationTemplates.Elements.chksystemtempateid).is(':checked')) {
                        var selected = $(':selected', this);
                        if (selected.closest('optgroup').attr('label') === 'GreenLights') {
                            SEL.NotificationTemplates.HideNoteRelatedFields();
                        } else {
                            SEL.NotificationTemplates.ShowNoteRelatedFields();
                        }
                    }
                });
            },

            ShowNoteRelatedFields: function () {
                $('#' + SEL.NotificationTemplates.Elements.lblSendNote).show();
                $('#' + SEL.NotificationTemplates.Elements.chksendNotes).parent().show();
                $('#cke_' + SEL.NotificationTemplates.Elements.rtBodyText).find('.cke_contents').css('height', '200px');
                $('#' + SEL.NotificationTemplates.Elements.notesHeader).show();
                $('#' + SEL.NotificationTemplates.Elements.noteswrapper).show();
            },

            HideNoteRelatedFields: function() {
                $('#' + SEL.NotificationTemplates.Elements.lblSendNote).hide();
                $('#' + SEL.NotificationTemplates.Elements.chksendNotes).parent().hide();
                $('#' + SEL.NotificationTemplates.Elements.chksendNotes).attr('checked', false);
                for (var instances in CKEDITOR.instances) {
                    if (instances == SEL.NotificationTemplates.Elements.emailNotes) {
                        CKEDITOR.instances[instances].setData('');
                    }
                }
                $('#cke_' + SEL.NotificationTemplates.Elements.rtBodyText).find('.cke_contents').css('height', '560px');
                $('#' + SEL.NotificationTemplates.Elements.notesHeader).hide();
                $('#' + SEL.NotificationTemplates.Elements.noteswrapper).hide();
            },

            GetEmailTemplate: function (event)
            {
                SEL.Grid.filterGridCmb('gridSysEmailTemplates', event);
                SEL.Grid.filterGridCmb('gridEmailTemplates', event);
            },

            ValidateAndShowAttachmentModal: function ()
            {
                if (notificationtemplateid == 0)
                {
                    if (validateform('vgMain') == false)
                    {
                        return;
                    }

                    SEL.NotificationTemplates.SaveTemplate(true, function () { SEL.NotificationTemplates.ShowFileAttachmentModal();});
                }
                else
                {
                    SEL.NotificationTemplates.ShowFileAttachmentModal();
                }
            },

            HideAttachmentModal: function ()
            {
                $find(SEL.NotificationTemplates.Elements.modattachmentid).hide();
            },

            ShowFileAttachmentModal: function () {
                $("#attachmentForm").dialog({
                    modal: true,
                    autoOpen: true,
                    resizable: false,
                    title: "Add Attachment",
                    width: 920,

                    buttons: [
                         {
                             text: 'close',
                             id: 'btnCancel',
                             click: function () {
                                 $(this).dialog('close');
                             }
                         }
                    ]
                   
                });
            },
            ClearHdnToField: function() {
                $("#txtTo").change(function () {
                    var hdnToElement = $("#" + SEL.NotificationTemplates.Elements.hdntoid);
                    if (hdnToElement.val() === '' || hdnToElement.val().length === 0) {
                        $("#" + SEL.NotificationTemplates.Elements.hdntoid).removeAttr("value");
                    }
                });
            },
           
            EnableDisableEditingToField: function () {

                if ($get(SEL.NotificationTemplates.Elements.chksystemtempateid).checked) {
                    var listElements = $("#ctl00_contentmain_hdnTo").next("ul").children("li");
                    for (var k = 0; k < listElements.length; k++) {
                        var listElement = listElements[k];
                        if (!$(listElement).hasClass("notToDelete")) {
                            $(listElement).children("span").click();
                        }
                    }
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).val(SEL.NotificationTemplates.hdnToInitialValues);
                    $("#butTo").css("pointer-events","none");
                    $("#butTo").attr("src", "/shared/images/icons/16/plain/add2_grey.png");
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).parent().css("pointer-events", "none");
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).parent().css("opacity", "0.8");
                    $("#token-input-txtTo").attr("disabled", "disabled").val("Determined by the system").css("width", "250px");
                }
                else {
                    $('#butTo').css("pointer-events", "auto");
                    $('#butTo').attr("src", "/shared/images/icons/16/plain/add2.png");
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).parent().css("pointer-events", "auto");
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).parent().css("opacity", "");
                    $("#token-input-txtTo").removeAttr("disabled").val("");
                }
            },
            ValidateToRecipientForSystemTemplate: function () {

                SEL.NotificationTemplates.hdnToInitialValues = $("#" + SEL.NotificationTemplates.Elements.hdntoid).val();
                var listElements = $("#" + SEL.NotificationTemplates.Elements.hdntoid).next("ul").children("li");
                for (var j = 0; j < listElements.length - 1; j++) {
                    $(listElements[j]).addClass("notToDelete");
                }
                SEL.NotificationTemplates.EnableDisableEditingToField();
                $("#" + SEL.NotificationTemplates.Elements.chksystemtempateid).change(function () {
                    SEL.NotificationTemplates.EnableDisableEditingToField();
                });
            },

            GroupComboBoxItem: function (selectId)
            {
                var groups = {};
                var selector = "#" + selectId;
                var value = $(selector).val();

                $(selector + " option[data-category]").each(function () {
                    groups[$.trim($(this).attr("data-category"))] = true;
                });
                $.each(groups, function (c) {
                    $(selector + " option[data-category='" + c + "']").wrapAll('<optgroup label="' + c + '">');
                });

                $(selector).val(value);
            },

            SaveTemplate: function (cancelRedirect, cancelRedirectFunction)
            {
                if (validateform('vgMain') == false)
                {
                    return;
                }

                var body='';
                var notes = '';
                var subject = '';
                for (var instances in CKEDITOR.instances) {
                    if (instances === SEL.NotificationTemplates.Elements.editorid) {
                        body = CKEDITOR.instances[instances].getData();
                    }
                    if (instances === SEL.NotificationTemplates.Elements.emailNotes) {
                        notes = CKEDITOR.instances[instances].getData();
                    }
                }

                subject = $('#' + SEL.NotificationTemplates.Elements.txtsubjectid).val();
                $('.subjectFields').children().each(function() {
                    subject = subject.replace($(this).text(), $(this)[0].outerHTML);
                });

                var templatename = $get(SEL.NotificationTemplates.Elements.txttemplatenameid).value;
                var to = $get(SEL.NotificationTemplates.Elements.hdntoid).value;
                var cc = $get(SEL.NotificationTemplates.Elements.hdnccid).value;
                var bcc = $get(SEL.NotificationTemplates.Elements.hdnbccid).value;
                var priorityddl = $get(SEL.NotificationTemplates.Elements.cmbpriorityid);
                var priority = priorityddl.options[priorityddl.selectedIndex].value;
                var ddl;
                var isSystemTemp = null;
                if ($get(SEL.NotificationTemplates.Elements.chksystemtempateid)) {
                    isSystemTemp = $get(SEL.NotificationTemplates.Elements.chksystemtempateid).checked;
                }

                var isSendNote = false;
                if ($get(SEL.NotificationTemplates.Elements.chksendNotes)) {
                    isSendNote = $get(SEL.NotificationTemplates.Elements.chksendNotes).checked;
                }

                var isSendEmail = false;
                if ($get(SEL.NotificationTemplates.Elements.chkSendEmail)) {
                    isSendEmail = $get(SEL.NotificationTemplates.Elements.chkSendEmail).checked;
                }

                var canSendMobileNotification = false;
                if ($get(SEL.NotificationTemplates.Elements.chkCanSendMobileNotification)) {
                    canSendMobileNotification = $get(SEL.NotificationTemplates.Elements.chkCanSendMobileNotification).checked;
                }

                var mobileMessage = $get(SEL.NotificationTemplates.Elements.mobileNotificationMessage).value;

                if (!update)
                {
                    ddl = $get(SEL.NotificationTemplates.Elements.cmbareaid);
                }

                if (ddl != null)
                {
                    tableid = ddl.options[ddl.selectedIndex].value;
                    tableid = ddl.options[ddl.selectedIndex].value;
                }

                Spend_Management.svcNotificationTemplates.SaveNotificationTemplate(notificationtemplateid, templatename, to, cc, bcc, subject, priority, body, tableid, isSystemTemp, isSendNote, notes, update, isSendEmail, canSendMobileNotification, mobileMessage, function (value)
                {
                    notificationtemplateid = value;
                    if (notificationtemplateid === -1) {
                        SEL.MasterPopup.ShowMasterPopup('The Template name you have provided already exists.', "Message from Expenses");
                        notificationtemplateid = 0;
                        return;
                    }

                    if (notificationtemplateid > 0) {
                        var grid = SEL.Grid.getGridById('gridAttachments');
                        if (grid !== null && grid !== undefined && grid.filters.length > 0) {
                            notificationtemplateid
                            grid.filters[0].values1[0] = notificationtemplateid;
                        }
                        
                        SEL.Grid.updateGrid(grid);
                        var ifrFile = document.getElementById('iFrEmailAttach');
                        if (ifrFile) {
                            document.getElementById('hdnEmailtemplateId').value = notificationtemplateid;
                            ifrFile.contentWindow.document.getElementById('GenIDVal').value = notificationtemplateid;
                        }

                    }

                    if (cancelRedirect) {
                        if (cancelRedirectFunction) {
                            cancelRedirectFunction();
                        }
                    } else {
                        var temp = isSystemTemp === true ? "system" : "custom";
                        window.location = "/shared/admin/adminnotificationtemplates.aspx?template=" + temp;
                    }

                }, SEL.NotificationTemplates.CommandFail);
            },

            Cancel: function ()
            {
                var temp = "";
                if (window.location.href.contains("?")){
                    var isSystemTemp = null;
                    if (SEL.NotificationTemplates.Elements.chksystemtempateidinitialval == null) {
                        if ($get(SEL.NotificationTemplates.Elements.chksystemtempateid)) {
                            isSystemTemp = $get(SEL.NotificationTemplates.Elements.chksystemtempateid).checked;
                        }
                        temp = isSystemTemp === true ? "?template=system" : "?template=custom";
                    }
                    else temp = SEL.NotificationTemplates.Elements.chksystemtempateidinitialval === true ? "?template=system" : "?template=custom";   
                }
                window.location = "/shared/admin/adminnotificationtemplates.aspx" + temp;
            },

            DeleteNotificationTemplate: function (notificationtemplateid)
            {
                currentRowID = notificationtemplateid;
                if (confirm('Are you sure you wish to delete the selected Notification Template?'))
                {
                    PageMethods.DeleteNotificationTemplate(notificationtemplateid, SEL.NotificationTemplates.DeleteNotificationTemplateComplete, SEL.NotificationTemplates.CommandFail);
                }
            },

            DeleteNotificationTemplateComplete: function (val)
            {
                SEL.Grid.deleteGridRow('gridEmailTemplates', currentRowID);
            },

            CommandFail: function (error)
            {
                if (error["_message"] != null)
                    {
                        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
                    }
                else
                {
                    SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
                }
                return;
            },

            ClearRecipientModal: function ()
            {
                $get(SEL.NotificationTemplates.Elements.cmbteamid).selectedIndex = 0;
                $get(SEL.NotificationTemplates.Elements.cmbbudgetid).selectedIndex = 0;
                $get(SEL.NotificationTemplates.Elements.cmbotherid).selectedIndex = 0;
                $get(SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).selectedIndex = 0;
                if ($('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute + ' option').length < 2) {
                    $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).parent().css('display', 'none');
                    $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).parent().prev().css('display', 'none');
                } else
                {
                    $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).parent().css('display', '');
                    $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).parent().prev().css('display', '');
                }
            },

            ChangeBase: function ()
            {
                if (SEL.NotificationTemplates.ConfirmChangeBase())
                {
                    if (!confirm('Are you sure you wish to change the Product area?\n\nChanging the Product area will remove any fields associated with the current selection.')) {
                        SEL.NotificationTemplates.RestoreLastSelected();
                        return;
                    }
                    SEL.NotificationTemplates.RemoveBaseFields();
                }

                var params = new SEL.NotificationTemplates.GetInitialNodes();
                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetEasyTreeNodes', params, SEL.NotificationTemplates.TreeRefreshComplete, SEL.NotificationTemplates.CommandFail);
                
                var newtext = $('#' + SEL.NotificationTemplates.Elements.cmbareaid).find(":selected").text();
                if (newtext.length > 17) {
                    $('.baseTreeHeader').attr('title', newtext);
                    newtext = newtext.substring(0, 17) + "...";
                } else {
                    $('.baseTreeHeader').attr('title', '');
                }

                $('.baseTreeHeader').text(newtext);
            },

            ConfirmChangeBase: function () {
                var contentText = '';
                for (var i in CKEDITOR.instances) {
                    contentText = contentText + CKEDITOR.instances[i].getData();
                }

                var re = /\[.*?\]/g;
                var m;
                var foundBaseEntries = false;
                do {
                    m = re.exec(contentText);
                    if (m) {
                        if (m[0].indexOf('[To:') === -1 && m[0].indexOf('[From:') === -1 ) {
                            foundBaseEntries = true;
                        }
            
                    }
                } while (m);

                return foundBaseEntries;
            },

            RestoreLastSelected: function ()
            {
                var selector = '#' + SEL.NotificationTemplates.Elements.cmbareaid + ' option[value="' + tableid + '"]';
                $(selector).prop('selected', true);
            },

            RemoveBaseFields: function ()
            {
                var contentText, newContent, currentEditor, re = /<span.*?<\/span>/g, match;

                for(var i in CKEDITOR.instances) {
                    currentEditor = CKEDITOR.instances[i];
                    contentText = currentEditor.getData();
                    newContent = contentText;

                    do
                    {
                        match = re.exec(contentText);
                        if (match)
                        {
                            if (match[0].indexOf('[To:') === -1 && match[0].indexOf('[From:') === -1)
                            {
                                newContent = newContent.replace(match[0].toString(), '');
                            }
                        }
                    } while (match);
                    
                    currentEditor.setData(newContent);
                }
            },
            
            GetInitialNodes: function ()
            {
                var tableId = $g(SEL.NotificationTemplates.Elements.cmbareaid).value;
                this.baseTableString = tableId;
            },

            TreeRefreshComplete: function (data)
            {
                $('#' + SEL.NotificationTemplates.Elements.baseTreeData).val(JSON.stringify(data.d));
                SEL.NotificationTemplates.Tree.CreateEasyTree('baseTree', SEL.NotificationTemplates.Elements.baseTreeData);
                var params = new SEL.NotificationTemplates.GetInitialNodes();
                SEL.Ajax.Service('/shared/webServices/svcNotificationTemplates.asmx/', 'GetGreenLightAttributes', params, SEL.NotificationTemplates.GreenLightAttributeRefreshComplete, SEL.NotificationTemplates.CommandFail);
            },

            BindOnKeyDownHandler: function ()
            {
                document.onkeydown = function(e) {
                    e = e || window.event;
                    if (e.keyCode === $.ui.keyCode.ESCAPE) {
                        if ($g('ctl00_pnlMasterPopup').style.display === '') {
                            SEL.MasterPopup.HideMasterPopup();
                            return;
                        }

                        if ($f('ctl00_contentmain_emailTemp_usrAttachments_pnlUpload')) {
                            if ($g('ctl00_contentmain_emailTemp_usrAttachments_pnlUpload').style.display != 'none') {
                                $find(SEL.NotificationTemplates.Elements.moduploadid).hide();
                                return;
                            }
                        }

                        if ($f('ctl00_contentmain_emailTemp_pnlAttachments')) {
                            if ($g('ctl00_contentmain_emailTemp_pnlAttachments').style.display != 'none') {
                                $find(SEL.NotificationTemplates.Elements.modattachmentid).hide();
                                return;
                            }
                        }

                        SEL.NotificationTemplates.HideAddRecipientModal();
                    }
                }
            },
            AddInputToken: function (tokenFieldId, hiddenFieldId, item, textSearched) {
                if (item.id === "-1") {
                    if ($('#' + tokenFieldId).prev().find('.token-input-token:contains("' + textSearched + '")').length > 1) {
                        $('#' + tokenFieldId).tokenInput("remove", textSearched);

                    } else {
                        $('#' + tokenFieldId).tokenInput("remove", item);
                        $("#" + hiddenFieldId).val($("#" + hiddenFieldId).val() + textSearched + ';');
                        var newitem = { id: "0", name: textSearched.replace(/[,&\=]/, ""), searchDisplay: textSearched.replace(/[,&\=]/, "") };
                        $('#' + tokenFieldId).tokenInput("add", newitem);
                    }
                }
                else {
                    var id = item.id.trim();
                    if ($('#' + tokenFieldId).prev().find('.token-input-token:contains("' + item.name.trim() + '")').length > 1) {
                        $('#' + tokenFieldId).tokenInput("remove", item);
                    }
                    else if (id.indexOf('@') > 0 && $('#' + tokenFieldId).prev().find('.token-input-token:contains("' + id.substring(0, id.length - 1) + '")').length >= 1) {
                        $('#' + tokenFieldId).prev().find('.token-input-token:contains("' + id.substring(0, id.length - 1) + '")').remove();
                        $("#" + hiddenFieldId).val($("#" + hiddenFieldId).val().replace(id + ';', ""));
                        $("#" + hiddenFieldId).val($("#" + hiddenFieldId).val() + item.name + ';');
                    }
                    else {
                        if (id.indexOf('@') > 0) {
                            $("#" + hiddenFieldId).val($("#" + hiddenFieldId).val() + item.name + ';');
                        }
                        else {
                            $("#" + hiddenFieldId).val($("#" + hiddenFieldId).val() + item.id);
                        }
                    }
                }
            },

            Tree : {
                Variables: {
                    ScrollOffset: null,
                    EasyTrees: []
                },

                ResetDragNotesWrapper: function () {
                    $("#noteswrapper").css("z-index", "0");
                    $("#noteswrapper").css("position", "static");
                    $("#dragNotes").css("height", "0px");
                    $("#dragNotes").css("width", "0px");
                    $("#dragNotes").hide();
                },

                ResetDragWrapper: function () {
                    $("#wrapper").css("z-index", "0");
                    $("#wrapper").css("position", "static");
                    $("#dragTarget").css("height", "0px");
                    $("#dragTarget").css("width", "0px");
                    $("#dragTarget").hide();
                },

                ResetWrappers: function ()
                {
                    SEL.NotificationTemplates.Tree.ResetDragNotesWrapper();
                    SEL.NotificationTemplates.Tree.ResetDragWrapper();
                    SEL.NotificationTemplates.Tree.DisableControls(false);
                },
            
                DisableControls: function (disabled) {
                    $("#" + SEL.NotificationTemplates.Elements.hdnccid).prop('disabled', disabled);
                    $("#" + SEL.NotificationTemplates.Elements.hdnbccid).prop('disabled', disabled);
                    $("#" + SEL.NotificationTemplates.Elements.hdntoid).prop('disabled', disabled);
                    $("#" + SEL.NotificationTemplates.Elements.txttemplatenameid).prop('disabled', disabled);
                },

                GetActiveTab: function () {
                    var result="";
                    var activeTreeTab = $("#tabs").tabs("option", "active");
                    switch (activeTreeTab) {
                        case 1:
                            result = "From:";
                            break;
                        case 2:
                            result = "To:";
                            break;
                    }
    
                    return result;
                },

                UpdateDroppedValue: function (valueToDrop, source, target)
                {
                    if (valueToDrop !== "")
                    {
                        var valueToUpdate = SEL.NotificationTemplates.Tree.GetActiveTab();
                        var html = '<span contenteditable="false" class="merge" node="' + source.internalId + '" title="' + source.crumbs + '">[' + valueToUpdate + valueToDrop + ']</span>&nbsp;';
                        if (target === SEL.NotificationTemplates.Elements.txtsubjectid) {
                            insertAtCaret($('#' + SEL.NotificationTemplates.Elements.txtsubjectid)[0], '[' + valueToUpdate + valueToDrop + ']');
                            var insertNode = $(html).attr('node');
                            if ($('.subjectFields').find('[node="'+insertNode+'"]').length === 0) {
                                $('.subjectFields').append($(html));
                            }
                            
                        } else {
                            for (var i in CKEDITOR.instances) {
                                if (i === target) {
                                    
                                    var oEditor = CKEDITOR.instances[i];
                                    var newElement = CKEDITOR.dom.element.createFromHtml(html, oEditor.document);
                                    oEditor.insertElement(newElement);
                                }
                            }
                        }
                    } 
                    SEL.NotificationTemplates.Tree.ResetWrappers();
                },
                UpdateDroppedValueInBody: function (valueToDrop, source) {
                    SEL.NotificationTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.NotificationTemplates.Elements.editorid);
                },

                UpdateDroppedValueInNotes: function (valueToDrop, source) {
                    SEL.NotificationTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.NotificationTemplates.Elements.emailNotes);
                },

                UpdateDroppedValueInSubject: function (valueToDrop, source) {
                    SEL.NotificationTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.NotificationTemplates.Elements.txtsubjectid);
                }, 
            
                CreateEasyTree: function (easyTreeDiv, dataDiv) {
                    SEL.NotificationTemplates.Tree.Variables.EasyTrees.push($('#' + easyTreeDiv).easytree({
                        data: $('#' + dataDiv).val(),
                        disableIcons: true,
                        enableDnd: true,
                        openLazyNode: function(event, nodes, node, hasChildren) {
                            node.lazyUrl = "/shared/webServices/svcReports.asmx/GetBranchEasyTreeNodes";
                            node.lazyUrlJson = JSON.stringify(new SEL.NotificationTemplates.Tree.GetBranchNodes(node.internalId, node.fieldid, node.crumbs));

                            if (!hasChildren) {
                                SEL.NotificationTemplates.Tree.Variables.ScrollOffset = $('#' + easyTreeDiv + '>ul').scrollTop();

                                $('#' + easyTreeDiv + '>ul').hide(1000);
                                $('#' + easyTreeDiv + ' #' + node.id).removeClass('isLazy');
                                return true;
                            }

                            return false;
                        },
                        opening: function() {
                            SEL.NotificationTemplates.Tree.Variables.ScrollOffset = $('#' + easyTreeDiv + '>ul').scrollTop();
                        },
                        built: function(nodes) {
                            if (SEL.NotificationTemplates.Tree.Variables.ScrollOffset !== null) {
                                $('#' + easyTreeDiv + '>ul').scrollTop(SEL.NotificationTemplates.Tree.Variables.ScrollOffset);
                                $('#' + easyTreeDiv + '>ul').show();
                            }
                        },
                        canDrop: function (event, nodes, isSourceNode, source, isTargetNode, target) {
                            var node;
                            if (isSourceNode) {
                                node = source;
                            } else {
                                var id = $(source).attr('id');
                                var idx = 0;
                                node = null;
                                while (node === null && idx < 2) {
                                    node = SEL.NotificationTemplates.Tree.Variables.EasyTrees[idx].getNode(id);
                                    idx++;
                                }
                            }
                            if (node === null || node === undefined)
                            {
                                return false;
                            }

                            return !isTargetNode && node.children === null && !node.isLazy;
                        },
                        dropping: function(event, nodes, isSourceNode, source, isTargetNode, target, canDrop) {
                            if (isTargetNode || !isSourceNode || !canDrop) {
                                return false;
                            }
                            var valueToDrop = source.text;
                            
                            if ($(target).attr('id') === SEL.NotificationTemplates.Elements.txtsubjectid) {
                                SEL.NotificationTemplates.Tree.UpdateDroppedValueInSubject(valueToDrop, source);
                            }

                            if ($(target).parent().parent().parent().hasClass("cke_editor_" + SEL.NotificationTemplates.Elements.editorid))
                            {
                                SEL.NotificationTemplates.Tree.UpdateDroppedValueInBody(valueToDrop, source);
                            }

                            if ($(target).parent().parent().parent().hasClass("cke_editor_" + SEL.NotificationTemplates.Elements.emailNotes))
                            {
                                SEL.NotificationTemplates.Tree.UpdateDroppedValueInNotes(valueToDrop, source);
                            }

                            return true;
                        }
                    }));
                },

                GetBranchNodes: function (id, fieldId, crumbs, idPrefix) {
                    this.fieldID = fieldId;
                    this.crumbs = crumbs;
                    this.nodeID = id;
                    this.idPrefix = idPrefix;
                },

                PageLoad: function () {
                    SEL.NotificationTemplates.Tree.CreateEasyTree('baseTree', SEL.NotificationTemplates.Elements.baseTreeData, SEL.NotificationTemplates.Tree.Variables.BaseEasyTree);
                    SEL.NotificationTemplates.Tree.CreateEasyTree('receiverTree', SEL.NotificationTemplates.Elements.employeeTreeData, SEL.NotificationTemplates.Tree.Variables.ReceiverEasyTree);
                    SEL.NotificationTemplates.Tree.CreateEasyTree('senderTree', SEL.NotificationTemplates.Elements.employeeTreeData, SEL.NotificationTemplates.Tree.Variables.SenderEasyTree);
                    var fnHandler = function() {
                        $('.cke_editable').addClass('easytree-droppable');
                    };
                    CKEDITOR.on('instanceReady', fnHandler);
                }
            },
            GreenLightAttributeRefreshComplete: function(data) {
                $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute + ' option').remove();
                $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute + ' optgroup').remove();
                $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val("00000000-0000-0000-0000-000000000000").html('[None]'));
                $(data.d).each(function ()
                {
                    if (this.Owner > '')
                    {
                        $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val(this.Id).html(this.Name).attr('data-category', this.Owner));
                    } else {
                        $('#' + SEL.NotificationTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val(this.Id).html(this.Name));
                    }
                    
                });

                SEL.NotificationTemplates.GroupComboBoxItem(SEL.NotificationTemplates.Elements.cmbGreenLightAttribute);
            },
            GetFieldComments: function () {
                setTimeout(function() {
                    $("body").on("mouseover", ".easytree-title", function () {

                        if (SEL.NotificationTemplates.Elements.TooltipTimer !== null) {
                            clearTimeout(SEL.NotificationTemplates.Elements.TooltipTimer);
                            SEL.NotificationTemplates.Elements.TooltipTimer = null;
                        }

                        var $this = $(this);
                        var getFieldCommentComplete = function(data) {
                            if (data.d !== undefined && data.d !== null && data.d > "") {
                                SEL.Tooltip.Show("<div>" + data.d + "</div>", "sm", $("#" + $this.parent().attr("id"))[0]);
                                var left;
                                if (Number($(window).width()) > 1230) {
                                    left = $("#tabs").position().left + $("#tabs").width() / 2;
                                }
                                else {
                                    left = $("#tabs").position().left + $("#tabs").width() / 6;
                                }
                                
                                $(".tooltipcontainer").css("left", left + "px");
                                var top = $(".tooltipcontainer").position().top + 18;
                                $(".tooltipcontainer").css("top", top + "px");
                            }
                            else {
                                $(".tooltipcontainer").hide();
                            }
                        };
                        SEL.NotificationTemplates.Elements.TooltipTimer = setTimeout(function () {

                            var parentDivId = $this.closest("div").attr("id");
                            var fieldId;
                            if (parentDivId === "baseTree") {
                                fieldId = SEL.NotificationTemplates.Tree.Variables.EasyTrees[0].getNode($this.parent().attr("id")).fieldid;
                            } else if (parentDivId === "senderTree") {
                                fieldId = SEL.NotificationTemplates.Tree.Variables.EasyTrees[2].getNode($this.parent().attr("id")).fieldid;
                            } else {
                                fieldId = SEL.NotificationTemplates.Tree.Variables.EasyTrees[1].getNode($this.parent().attr("id")).fieldid;
                            }
                            var params = { fieldGuid: fieldId }
                            SEL.Ajax.Service("/shared/webServices/svcReports.asmx/", "GetFieldComment", params, getFieldCommentComplete, SEL.NotificationTemplates.CommandFail);
                        }, 1000);
                       
                    });
                    $('.easytree-title').unbind('mouseout').mouseout(function () {
                        if (SEL.NotificationTemplates.Elements.TooltipTimer !== null) {
                            clearTimeout(SEL.NotificationTemplates.Elements.TooltipTimer);
                            SEL.NotificationTemplates.Elements.TooltipTimer = null;
                        }
                    });
                }, 1000);
            }
        }
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(moduleNameHTML));