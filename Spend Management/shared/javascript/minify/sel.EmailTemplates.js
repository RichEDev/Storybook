(function (moduleNameHtml, appPath)
{
    var scriptName = "emailTemplates";

    function execute()
    {
        SEL.registerNamespace("SEL.EmailTemplates");
        SEL.EmailTemplates = {
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
                SEL.EmailTemplates.Elements.RecipientModal = $("#" + SEL.EmailTemplates.Elements.recipientModalForm).dialog({
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
                            click: SEL.EmailTemplates.GetRecipient
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
                SEL.EmailTemplates.RecipientModal.type = recType;
                SEL.EmailTemplates.ClearRecipientModal();
                SEL.EmailTemplates.Elements.RecipientModal.dialog("open");
            },

            HideAddRecipientModal: function ()
            {
                SEL.EmailTemplates.Elements.RecipientModal.dialog("close");
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
                    prePopulate: SEL.EmailTemplates.GetTokenInputPrePopulate("#" + SEL.EmailTemplates.Elements.hdntoid),
                    onAdd: function (item, textSearched) {
                        SEL.EmailTemplates.AddInputToken('txtTo', SEL.EmailTemplates.Elements.hdntoid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.EmailTemplates.Elements.hdntoid).val($("#" + SEL.EmailTemplates.Elements.hdntoid).val().replace(item.id, ""));
                    }
                }));

                $('#txtCC').tokenInput(SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "SearchEmployeeEmailByNameAndUsername" }), $.extend(options, {
                    prePopulate: SEL.EmailTemplates.GetTokenInputPrePopulate("#" + SEL.EmailTemplates.Elements.hdnccid),
                    onAdd: function (item, textSearched)
                    {
                        SEL.EmailTemplates.AddInputToken('txtCC', SEL.EmailTemplates.Elements.hdnccid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.EmailTemplates.Elements.hdnccid).val($("#" + SEL.EmailTemplates.Elements.hdnccid).val().replace(item.id, ""));
                    }
                }));

                $('#txtBCC').tokenInput(SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "SearchEmployeeEmailByNameAndUsername" }), $.extend(options, {
                    prePopulate: SEL.EmailTemplates.GetTokenInputPrePopulate("#" + SEL.EmailTemplates.Elements.hdnbccid),
                    onAdd: function (item, textSearched)
                    {
                        SEL.EmailTemplates.AddInputToken('txtBCC', SEL.EmailTemplates.Elements.hdnbccid, item, textSearched);
                    },
                    onDelete: function (item) {
                        $("#" + SEL.EmailTemplates.Elements.hdnbccid).val($("#" + SEL.EmailTemplates.Elements.hdnbccid).val().replace(item.id, ""));
                    }
                }));
            },

            GetRecipient: function ()
            {
                var username;
                var splitval1;
                var teamddl = $get(SEL.EmailTemplates.Elements.cmbteamid);
                var teamid = teamddl.options[teamddl.selectedIndex].value;
                var budgetddl = $get(SEL.EmailTemplates.Elements.cmbbudgetid);
                var budgetid = budgetddl.options[budgetddl.selectedIndex].value;
                var otherddl = $get(SEL.EmailTemplates.Elements.cmbotherid);
                var senderType = otherddl.options[otherddl.selectedIndex].value;
                var sender = otherddl.options[otherddl.selectedIndex].text;
                var greenlightddl = $get(SEL.EmailTemplates.Elements.cmbGreenLightAttribute);
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
                    Spend_Management.svcEmailTemplates.getRecipientInfo(username, teamid, budgetid, senderType, sender, greenLightId, function(data) {

                        var recipientFieldSelector;
                        switch (SEL.EmailTemplates.RecipientModal.type) {
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

                        SEL.EmailTemplates.HideAddRecipientModal();

                    }, SEL.EmailTemplates.CommandFail);
                }
            },

            ValidateBodyTextLength: function (source, args) {
                var txtVal;
                for (var i in CKEDITOR.instances) {
                    if (i == SEL.EmailTemplates.Elements.editorid) {
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
                var txtVal = $('#' + SEL.EmailTemplates.Elements.txtsubjectid).val();
                
                if (txtVal === "")
                {
                    args.IsValid = false;
                }

                return;
            },

            ValidateBroadcastMessageLength: function (source, args)
            {
                if ($get(SEL.EmailTemplates.Elements.chksendNotes)) {
                    isSendNote = $get(SEL.EmailTemplates.Elements.chksendNotes).checked;
                    if (isSendNote) {
                        var txtVal = $('#' + SEL.EmailTemplates.Elements.emailNotes).val();

                        if (txtVal === "") {
                            args.IsValid = false;
                        }
                    }
                }
                return;
            },

            ValidateMobileNotificationMessageIsRequired: function (source, args) {
                if ($get(SEL.EmailTemplates.Elements.chkCanSendMobileNotification)) {
                    canSendMobileNotification = $get(SEL.EmailTemplates.Elements.chkCanSendMobileNotification).checked;
                    if (canSendMobileNotification) {
                        var txtVal = $('#' + SEL.EmailTemplates.Elements.mobileNotificationMessage).val();

                        if (txtVal === "") {
                            args.IsValid = false;
                        }
                    }
                }
                return;
            },

            ValidateMobileNotificationMessageLength: function (source, args) {

                var txtVal = $('#' + SEL.EmailTemplates.Elements.mobileNotificationMessage).val();
                if (txtVal !== "" && txtVal.length > 400) {
                    args.IsValid = false;
                }
                return;
            },

            ValidateToRecipient: function (source, args) {

                var toVal = $get(SEL.EmailTemplates.Elements.hdntoid).value;
                if (toVal === "" || toVal === null) {
                    args.IsValid = false;
                }
                if ($get(SEL.EmailTemplates.Elements.chksystemtempateid)) {
                    isSystemTemplate = $get(SEL.EmailTemplates.Elements.chksystemtempateid).checked;
                    if (isSystemTemplate) {
                        args.IsValid = true;
                    }
                }
                return;

            },

            SetBodyHeight: function () {
                if ($('#' + SEL.EmailTemplates.Elements.chksystemtempateid).is(":visible") === false) {
                    $('#cke_' + SEL.EmailTemplates.Elements.rtBodyText).find('div.cke_contents').css('height', '560px');
                } 
            },

            OnSystemTemplateCheckChanged: function (sender)
            {
                var checkbox = document.getElementById(sender).checked;
                if (SEL.EmailTemplates.Elements.chksystemtempateidinitialval == null) SEL.EmailTemplates.Elements.chksystemtempateidinitialval = !checkbox;
                var selectedProductArea = $('#' + SEL.EmailTemplates.Elements.cmbareaid +' :selected').closest('optgroup').attr('label');
                if (checkbox && selectedProductArea !== 'GreenLights')
                {
                    $('#' + SEL.EmailTemplates.Elements.reqTo).css('visibility', 'hidden');
                    $('#' + SEL.EmailTemplates.Elements.reqTo).prop('disabled', true);
                    // if it's system template we need to show all the relevant notes fields
                    SEL.EmailTemplates.ShowNoteRelatedFields();
                }
                else {
                    $('#' + SEL.EmailTemplates.Elements.reqTo).prop('disabled', false);
                    // if it isn't system template we need to hide all the relevant notes fields
                    SEL.EmailTemplates.HideNoteRelatedFields();
                }
            },

            OnProductAreaChange : function() {
                $('#' + SEL.EmailTemplates.Elements.cmbareaid).change(function () {
                    if ($('#' + SEL.EmailTemplates.Elements.chksystemtempateid).is(':checked')) {
                        var selected = $(':selected', this);
                        if (selected.closest('optgroup').attr('label') === 'GreenLights') {
                            SEL.EmailTemplates.HideNoteRelatedFields();
                        } else {
                            SEL.EmailTemplates.ShowNoteRelatedFields();
                        }
                    }
                });
            },

            ShowNoteRelatedFields: function () {
                $('#' + SEL.EmailTemplates.Elements.lblSendNote).show();
                $('#' + SEL.EmailTemplates.Elements.chksendNotes).parent().show();
                $('#cke_' + SEL.EmailTemplates.Elements.rtBodyText).find('.cke_contents').css('height', '200px');
                $('#' + SEL.EmailTemplates.Elements.notesHeader).show();
                $('#' + SEL.EmailTemplates.Elements.noteswrapper).show();
            },

            HideNoteRelatedFields: function() {
                $('#' + SEL.EmailTemplates.Elements.lblSendNote).hide();
                $('#' + SEL.EmailTemplates.Elements.chksendNotes).parent().hide();
                $('#' + SEL.EmailTemplates.Elements.chksendNotes).attr('checked', false);
                for (var instances in CKEDITOR.instances) {
                    if (instances == SEL.EmailTemplates.Elements.emailNotes) {
                        CKEDITOR.instances[instances].setData('');
                    }
                }
                $('#cke_' + SEL.EmailTemplates.Elements.rtBodyText).find('.cke_contents').css('height', '560px');
                $('#' + SEL.EmailTemplates.Elements.notesHeader).hide();
                $('#' + SEL.EmailTemplates.Elements.noteswrapper).hide();
            },

            GetEmailTemplate: function (event)
            {
                SEL.Grid.filterGridCmb('gridSysEmailTemplates', event);
                SEL.Grid.filterGridCmb('gridEmailTemplates', event);
            },

            ValidateAndShowAttachmentModal: function ()
            {
                if (emailtemplateid == 0)
                {
                    if (validateform('vgMain') == false)
                    {
                        return;
                    }

                    SEL.EmailTemplates.SaveTemplate(true, function () { SEL.EmailTemplates.ShowFileAttachmentModal();});
                }
                else
                {
                    SEL.EmailTemplates.ShowFileAttachmentModal();
                }
            },

            HideAttachmentModal: function ()
            {
                $find(SEL.EmailTemplates.Elements.modattachmentid).hide();
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
                    var hdnToElement = $("#" + SEL.EmailTemplates.Elements.hdntoid);
                    if (hdnToElement.val() === '' || hdnToElement.val().length === 0) {
                        $("#" + SEL.EmailTemplates.Elements.hdntoid).removeAttr("value");
                    }
                });
            },
           
            EnableDisableEditingToField: function () {

                if ($get(SEL.EmailTemplates.Elements.chksystemtempateid).checked) {
                    var listElements = $("#ctl00_contentmain_hdnTo").next("ul").children("li");
                    for (var k = 0; k < listElements.length; k++) {
                        var listElement = listElements[k];
                        if (!$(listElement).hasClass("notToDelete")) {
                            $(listElement).children("span").click();
                        }
                    }
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).val(SEL.EmailTemplates.hdnToInitialValues);
                    $("#butTo").css("pointer-events","none");
                    $("#butTo").attr("src", "/shared/images/icons/16/plain/add2_grey.png");
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).parent().css("pointer-events", "none");
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).parent().css("opacity", "0.8");
                    $("#token-input-txtTo").attr("disabled", "disabled").val("Determined by the system").css("width", "250px");
                }
                else {
                    $('#butTo').css("pointer-events", "auto");
                    $('#butTo').attr("src", "/shared/images/icons/16/plain/add2.png");
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).parent().css("pointer-events", "auto");
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).parent().css("opacity", "");
                    $("#token-input-txtTo").removeAttr("disabled").val("");
                }
            },
            ValidateToRecipientForSystemTemplate: function () {

                SEL.EmailTemplates.hdnToInitialValues = $("#" + SEL.EmailTemplates.Elements.hdntoid).val();
                var listElements = $("#" + SEL.EmailTemplates.Elements.hdntoid).next("ul").children("li");
                for (var j = 0; j < listElements.length - 1; j++) {
                    $(listElements[j]).addClass("notToDelete");
                }
                SEL.EmailTemplates.EnableDisableEditingToField();
                $("#" + SEL.EmailTemplates.Elements.chksystemtempateid).change(function () {
                    SEL.EmailTemplates.EnableDisableEditingToField();
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
                    if (instances === SEL.EmailTemplates.Elements.editorid) {
                        body = CKEDITOR.instances[instances].getData();
                    }
                    if (instances === SEL.EmailTemplates.Elements.emailNotes) {
                        notes = CKEDITOR.instances[instances].getData();
                    }
                }

                subject = $('#' + SEL.EmailTemplates.Elements.txtsubjectid).val();
                $('.subjectFields').children().each(function() {
                    subject = subject.replace($(this).text(), $(this)[0].outerHTML);
                });

                var templatename = $get(SEL.EmailTemplates.Elements.txttemplatenameid).value;
                var to = $get(SEL.EmailTemplates.Elements.hdntoid).value;
                var cc = $get(SEL.EmailTemplates.Elements.hdnccid).value;
                var bcc = $get(SEL.EmailTemplates.Elements.hdnbccid).value;
                var priorityddl = $get(SEL.EmailTemplates.Elements.cmbpriorityid);
                var priority = priorityddl.options[priorityddl.selectedIndex].value;
                var ddl;
                var isSystemTemp = null;
                if ($get(SEL.EmailTemplates.Elements.chksystemtempateid)) {
                    isSystemTemp = $get(SEL.EmailTemplates.Elements.chksystemtempateid).checked;
                }

                var isSendNote = false;
                if ($get(SEL.EmailTemplates.Elements.chksendNotes)) {
                    isSendNote = $get(SEL.EmailTemplates.Elements.chksendNotes).checked;
                }

                var isSendEmail = false;
                if ($get(SEL.EmailTemplates.Elements.chkSendEmail)) {
                    isSendEmail = $get(SEL.EmailTemplates.Elements.chkSendEmail).checked;
                }

                var canSendMobileNotification = false;
                if ($get(SEL.EmailTemplates.Elements.chkCanSendMobileNotification)) {
                    canSendMobileNotification = $get(SEL.EmailTemplates.Elements.chkCanSendMobileNotification).checked;
                }

                var mobileMessage = $get(SEL.EmailTemplates.Elements.mobileNotificationMessage).value;

                if (!update)
                {
                    ddl = $get(SEL.EmailTemplates.Elements.cmbareaid);
                }

                if (ddl != null)
                {
                    tableid = ddl.options[ddl.selectedIndex].value;
                }

                Spend_Management.svcEmailTemplates.saveEmailTemplate(emailtemplateid, templatename, to, cc, bcc, subject, priority, body, tableid, isSystemTemp, isSendNote, notes, update, isSendEmail, canSendMobileNotification, mobileMessage, function (value)
                {
                    emailtemplateid = value;
                    if (emailtemplateid === -1) {
                        SEL.MasterPopup.ShowMasterPopup('The Template name you have provided already exists.', "Message from Expenses");
                        emailtemplateid = 0;
                        return;
                    }

                    if (emailtemplateid > 0) {
                        var grid = SEL.Grid.getGridById('gridAttachments');
                        if (grid !== null && grid !== undefined && grid.filters.length > 0) {
                            grid.filters[0].values1[0] = emailtemplateid;
                        }
                        
                        SEL.Grid.updateGrid(grid);
                        var ifrFile = document.getElementById('iFrEmailAttach');
                        if (ifrFile) {
                            document.getElementById('hdnEmailtemplateId').value = emailtemplateid;
                            ifrFile.contentWindow.document.getElementById('GenIDVal').value = emailtemplateid;
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

                }, SEL.EmailTemplates.CommandFail);
            },

            Cancel: function ()
            {
                var temp = "";
                if (window.location.href.contains("?")){
                    var isSystemTemp = null;
                    if (SEL.EmailTemplates.Elements.chksystemtempateidinitialval == null) {
                        if ($get(SEL.EmailTemplates.Elements.chksystemtempateid)) {
                            isSystemTemp = $get(SEL.EmailTemplates.Elements.chksystemtempateid).checked;
                        }
                        temp = isSystemTemp === true ? "?template=system" : "?template=custom";
                    }
                    else temp = SEL.EmailTemplates.Elements.chksystemtempateidinitialval === true ? "?template=system" : "?template=custom";   
                }
                window.location = "/shared/admin/adminnotificationtemplates.aspx" + temp;
            },

            DeleteEmailTemplate: function (emailtemplateid)
            {
                currentRowID = emailtemplateid;
                if (confirm('Are you sure you wish to delete the selected Notification Template?'))
                {
                    PageMethods.deleteEmailTemplate(emailtemplateid, SEL.EmailTemplates.DeleteEmailTemplateComplete, SEL.EmailTemplates.CommandFail);
                }
            },

            DeleteEmailTemplateComplete: function (val)
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
                $get(SEL.EmailTemplates.Elements.cmbteamid).selectedIndex = 0;
                $get(SEL.EmailTemplates.Elements.cmbbudgetid).selectedIndex = 0;
                $get(SEL.EmailTemplates.Elements.cmbotherid).selectedIndex = 0;
                $get(SEL.EmailTemplates.Elements.cmbGreenLightAttribute).selectedIndex = 0;
                if ($('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute + ' option').length < 2) {
                    $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).parent().css('display', 'none');
                    $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).parent().prev().css('display', 'none');
                } else
                {
                    $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).parent().css('display', '');
                    $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).parent().prev().css('display', '');
                }
            },

            ChangeBase: function ()
            {
                if (SEL.EmailTemplates.ConfirmChangeBase())
                {
                    if (!confirm('Are you sure you wish to change the Product area?\n\nChanging the Product area will remove any fields associated with the current selection.')) {
                        SEL.EmailTemplates.RestoreLastSelected();
                        return;
                    }
                    SEL.EmailTemplates.RemoveBaseFields();
                }

                var params = new SEL.EmailTemplates.GetInitialNodes();
                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetEasyTreeNodes', params, SEL.EmailTemplates.TreeRefreshComplete, SEL.EmailTemplates.CommandFail);
                
                var newtext = $('#' + SEL.EmailTemplates.Elements.cmbareaid).find(":selected").text();
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
                var selector = '#' + SEL.EmailTemplates.Elements.cmbareaid + ' option[value="' + tableid + '"]';
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
                var tableId = $g(SEL.EmailTemplates.Elements.cmbareaid).value;
                this.baseTableString = tableId;
            },

            TreeRefreshComplete: function (data)
            {
                $('#' + SEL.EmailTemplates.Elements.baseTreeData).val(JSON.stringify(data.d));
                SEL.EmailTemplates.Tree.CreateEasyTree('baseTree', SEL.EmailTemplates.Elements.baseTreeData);
                var params = new SEL.EmailTemplates.GetInitialNodes();
                SEL.Ajax.Service('/shared/webServices/svcEmailTemplates.asmx/', 'GetGreenLightAttributes', params, SEL.EmailTemplates.GreenLightAttributeRefreshComplete, SEL.EmailTemplates.CommandFail);
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
                                $find(SEL.EmailTemplates.Elements.moduploadid).hide();
                                return;
                            }
                        }

                        if ($f('ctl00_contentmain_emailTemp_pnlAttachments')) {
                            if ($g('ctl00_contentmain_emailTemp_pnlAttachments').style.display != 'none') {
                                $find(SEL.EmailTemplates.Elements.modattachmentid).hide();
                                return;
                            }
                        }

                        SEL.EmailTemplates.HideAddRecipientModal();
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
                    SEL.EmailTemplates.Tree.ResetDragNotesWrapper();
                    SEL.EmailTemplates.Tree.ResetDragWrapper();
                    SEL.EmailTemplates.Tree.DisableControls(false);
                },
            
                DisableControls: function (disabled) {
                    $("#" + SEL.EmailTemplates.Elements.hdnccid).prop('disabled', disabled);
                    $("#" + SEL.EmailTemplates.Elements.hdnbccid).prop('disabled', disabled);
                    $("#" + SEL.EmailTemplates.Elements.hdntoid).prop('disabled', disabled);
                    $("#" + SEL.EmailTemplates.Elements.txttemplatenameid).prop('disabled', disabled);
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
                        var valueToUpdate = SEL.EmailTemplates.Tree.GetActiveTab();
                        var html = '<span contenteditable="false" class="merge" node="' + source.internalId + '" title="' + source.crumbs + '">[' + valueToUpdate + valueToDrop + ']</span>&nbsp;';
                        if (target === SEL.EmailTemplates.Elements.txtsubjectid) {
                            insertAtCaret($('#' + SEL.EmailTemplates.Elements.txtsubjectid)[0], '[' + valueToUpdate + valueToDrop + ']');
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
                    SEL.EmailTemplates.Tree.ResetWrappers();
                },
                UpdateDroppedValueInBody: function (valueToDrop, source) {
                    SEL.EmailTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.EmailTemplates.Elements.editorid);
                },

                UpdateDroppedValueInNotes: function (valueToDrop, source) {
                    SEL.EmailTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.EmailTemplates.Elements.emailNotes);
                },

                UpdateDroppedValueInSubject: function (valueToDrop, source) {
                    SEL.EmailTemplates.Tree.UpdateDroppedValue(valueToDrop, source, SEL.EmailTemplates.Elements.txtsubjectid);
                }, 
            
                CreateEasyTree: function (easyTreeDiv, dataDiv) {
                    SEL.EmailTemplates.Tree.Variables.EasyTrees.push($('#' + easyTreeDiv).easytree({
                        data: $('#' + dataDiv).val(),
                        disableIcons: true,
                        enableDnd: true,
                        openLazyNode: function(event, nodes, node, hasChildren) {
                            node.lazyUrl = "/shared/webServices/svcReports.asmx/GetBranchEasyTreeNodes";
                            node.lazyUrlJson = JSON.stringify(new SEL.EmailTemplates.Tree.GetBranchNodes(node.internalId, node.fieldid, node.crumbs));

                            if (!hasChildren) {
                                SEL.EmailTemplates.Tree.Variables.ScrollOffset = $('#' + easyTreeDiv + '>ul').scrollTop();

                                $('#' + easyTreeDiv + '>ul').hide(1000);
                                $('#' + easyTreeDiv + ' #' + node.id).removeClass('isLazy');
                                return true;
                            }

                            return false;
                        },
                        opening: function() {
                            SEL.EmailTemplates.Tree.Variables.ScrollOffset = $('#' + easyTreeDiv + '>ul').scrollTop();
                        },
                        built: function(nodes) {
                            if (SEL.EmailTemplates.Tree.Variables.ScrollOffset !== null) {
                                $('#' + easyTreeDiv + '>ul').scrollTop(SEL.EmailTemplates.Tree.Variables.ScrollOffset);
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
                                    node = SEL.EmailTemplates.Tree.Variables.EasyTrees[idx].getNode(id);
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
                            
                            if ($(target).attr('id') === SEL.EmailTemplates.Elements.txtsubjectid) {
                                SEL.EmailTemplates.Tree.UpdateDroppedValueInSubject(valueToDrop, source);
                            }

                            if ($(target).parent().parent().parent().hasClass("cke_editor_" + SEL.EmailTemplates.Elements.editorid))
                            {
                                SEL.EmailTemplates.Tree.UpdateDroppedValueInBody(valueToDrop, source);
                            }

                            if ($(target).parent().parent().parent().hasClass("cke_editor_" + SEL.EmailTemplates.Elements.emailNotes))
                            {
                                SEL.EmailTemplates.Tree.UpdateDroppedValueInNotes(valueToDrop, source);
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
                    SEL.EmailTemplates.Tree.CreateEasyTree('baseTree', SEL.EmailTemplates.Elements.baseTreeData, SEL.EmailTemplates.Tree.Variables.BaseEasyTree);
                    SEL.EmailTemplates.Tree.CreateEasyTree('receiverTree', SEL.EmailTemplates.Elements.employeeTreeData, SEL.EmailTemplates.Tree.Variables.ReceiverEasyTree);
                    SEL.EmailTemplates.Tree.CreateEasyTree('senderTree', SEL.EmailTemplates.Elements.employeeTreeData, SEL.EmailTemplates.Tree.Variables.SenderEasyTree);
                    var fnHandler = function() {
                        $('.cke_editable').addClass('easytree-droppable');
                    };
                    CKEDITOR.on('instanceReady', fnHandler);
                }
            },
            GreenLightAttributeRefreshComplete: function(data) {
                $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute + ' option').remove();
                $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute + ' optgroup').remove();
                $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val("00000000-0000-0000-0000-000000000000").html('[None]'));
                $(data.d).each(function ()
                {
                    if (this.Owner > '')
                    {
                        $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val(this.Id).html(this.Name).attr('data-category', this.Owner));
                    } else {
                        $('#' + SEL.EmailTemplates.Elements.cmbGreenLightAttribute).append($('<option></option>').val(this.Id).html(this.Name));
                    }
                    
                });

                SEL.EmailTemplates.GroupComboBoxItem(SEL.EmailTemplates.Elements.cmbGreenLightAttribute);
            },
            GetFieldComments: function () {
                setTimeout(function() {
                    $("body").on("mouseover", ".easytree-title", function () {

                        if (SEL.EmailTemplates.Elements.TooltipTimer !== null) {
                            clearTimeout(SEL.EmailTemplates.Elements.TooltipTimer);
                            SEL.EmailTemplates.Elements.TooltipTimer = null;
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
                        SEL.EmailTemplates.Elements.TooltipTimer = setTimeout(function () {

                            var parentDivId = $this.closest("div").attr("id");
                            var fieldId;
                            if (parentDivId === "baseTree") {
                                fieldId = SEL.EmailTemplates.Tree.Variables.EasyTrees[0].getNode($this.parent().attr("id")).fieldid;
                            } else if (parentDivId === "senderTree") {
                                fieldId = SEL.EmailTemplates.Tree.Variables.EasyTrees[2].getNode($this.parent().attr("id")).fieldid;
                            } else {
                                fieldId = SEL.EmailTemplates.Tree.Variables.EasyTrees[1].getNode($this.parent().attr("id")).fieldid;
                            }
                            var params = { fieldGuid: fieldId }
                            SEL.Ajax.Service("/shared/webServices/svcReports.asmx/", "GetFieldComment", params, getFieldCommentComplete, SEL.EmailTemplates.CommandFail);
                        }, 1000);
                       
                    });
                    $('.easytree-title').unbind('mouseout').mouseout(function () {
                        if (SEL.EmailTemplates.Elements.TooltipTimer !== null) {
                            clearTimeout(SEL.EmailTemplates.Elements.TooltipTimer);
                            SEL.EmailTemplates.Elements.TooltipTimer = null;
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