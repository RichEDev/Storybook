<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aenotificationtemplate.aspx.cs" Inherits="Spend_Management.aenotificationtemplate" %>
<%@ Register Src="~/shared/usercontrols/attachmentList.ascx" TagName="UploadAttachment" TagPrefix="uc1" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="cke" %>
<%@ Register TagPrefix="asp" Namespace="SpendManagementHelpers" Assembly="SpendManagementHelpers" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="styles">

     <style type="text/css">
        #ctl00_contentmain_usrAttachments_pnlUpload{
            width: 46%;
            height: 25%;
        }

        #iFrEmailAttach{
            width: 100%;
            height: 100%;
        }

        #ctl00_contentmain_usrAttachments_usrUpload_divFileUpload{
            height: 265px!important;
        }

        .formpanel a{
            color: #003768;
            text-decoration:underline;
        }
        .merge {
            background-color: #d3d3d3;
        }
    </style>
    <link id="TokenInputCss" runat="server" type="text/css" rel="stylesheet" href="/shared/css/token-input.css"/>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smProxy" runat="server">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/shared/webservices/svcEmailTemplates.asmx" />
            <asp:ServiceReference InlineScript="true" Path="~/shared/webservices/svcWorkflows.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.emailtemplates.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript" src="../javaScript/minify/jquery.min.js"></script>
    <script type="text/javascript" src="/static/js/jQuery/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/static/js/jQuery/jquery-ui-1.9.2.custom.js"></script>
    <script type="text/javascript" src="/static/js/jQuery/jquery.idletimer.js"></script>
    <script type="text/javascript" src="/static/js/jQuery/easytree/jquery.easytree.js"></script>
    <script type="text/javascript" src="/static/js/expense/jquery.qtip.min.js"></script>
    <script type="text/javascript" src="../javascript/minify/jquery.tokeninput.js"></script>
    <script type="text/javascript" src="../javaScript/minify/jstree.min.js"></script>
    <script type="text/javascript" src="../javaScript/minify/sel.idletimeout.js"></script>
    <script type="text/javascript" src="../javaScript/sel.ajax.js"></script>
    <script type="text/javascript" src="/shared/reports/formula.js"></script>
    <script type="text/javascript" language="javascript">

        $(document).ready(function () {

             (function (r) {
                r.recipientModalForm = '<%= recipientModalForm.ClientID %>';

                r.hdntoid = '<%= hdnTo.ClientID %>';
                r.hdnccid = '<%= hdnCC.ClientID %>';
                r.hdnbccid = '<%= hdnBCC.ClientID %>';
                r.txtsubjectid = '<%= rtSubject.ClientID %>';
                r.cmbteamid = '<%= cmbTeam.ClientID %>';
                r.cmbbudgetid = '<%= cmbBudget.ClientID %>';
                r.cmbotherid = '<%= cmbOther.ClientID %>';
                r.cmbGreenLightAttribute = '<%=cmbGreenLightAttribute.ClientID%>';
                r.reqTo = '<%=reqTo.ClientID %>';

                r.editorid = '<%= rtBodyText.ClientID %>';
                r.txttemplatenameid = '<%= txtTemplateName.ClientID %>';
                r.cmbpriorityid = '<%= cmbPriority.ClientID %>';
                r.cmbareaid = '<%= cmbArea.ClientID %>';
                r.chksystemtempateid = '<%= chkSystemTemplate.ClientID %>';

                r.chksendNotes = '<%= ChkSendNote.ClientID %>';
                r.chkSendEmail = '<%= ChkSendEmail.ClientID%>';
                r.chkCanSendMobileNotification = '<%= chkCanEmailNotification.ClientID%>';
                r.emailNotes = '<%= txtNotes.ClientID %>';
                r.notesHeader = '<%= notesHeader.ClientID %>';
                r.mobileNotificationMessage = '<%= this.txtMobileNotificationMessage.ClientID%>';
                r.lblSendNote = '<%= lblSendNote.ClientID %>';
                r.noteswrapper = '<%= noteswrapper.ClientID %>';
                r.rtBodyText = '<%= rtBodyText.ClientID %>';

                r.baseTreeData = '<%= this.baseTreeData.ClientID %>';
                r.employeeTreeData = '<%= this.employeeTreeData.ClientID %>';
                r.subjectHtml = '<%=this.subjectHtml.ClientID %>';
                r.bodyHtml = '<%=this.bodyHtml.ClientID %>';
                r.noteHtml = '<%=this.noteHtml.ClientID %>';
            }(SEL.EmailTemplates.Elements));
            $('#tabs').tabs();

            CKEDITOR.config.allowedContent = true;
            CKEDITOR.config.title = false;
            CKEDITOR.config.font_names = ("Calibri/Calibri, Verdana, Geneva, sans-serif;" + CKEDITOR.config.font_names).split(";").sort().join(";");
            CKEDITOR.config.font_defaultLabel = 'Arial';
            CKEDITOR.config.fontSize_defaultLabel = '12px';

            SEL.EmailTemplates.RecipientModalInitialise();
            SEL.EmailTemplates.InitialiseTokenInputPlugin();
            SEL.EmailTemplates.BindOnKeyDownHandler();
            SEL.EmailTemplates.Tree.PageLoad();
            SEL.EmailTemplates.ClearHdnToField();
            SEL.EmailTemplates.GroupComboBoxItem(SEL.EmailTemplates.Elements.cmbareaid);
            SEL.EmailTemplates.GroupComboBoxItem(SEL.EmailTemplates.Elements.cmbGreenLightAttribute);
            SEL.EmailTemplates.ValidateToRecipientForSystemTemplate();
            SEL.EmailTemplates.OnProductAreaChange();
            SEL.EmailTemplates.GetFieldComments();
        });
    </script>
    <style>
        #listSurname.AutoCompleteExtenderCompletionList
        {
            z-index: 2000 !important;
            border: 1px solid buttonshadow;
            cursor: pointer;
        }

        #listSurname .AutoCompleteExtenderCompletionListItem  {
            padding: 2px;
        }

        ul.token-input-list {
            margin-left: -2px;
            width: 571px;
            max-height: 100px;
            overflow-y: auto;
        }

        li.token-input-token {
            display: inline-block;
            width: auto;
        }
        </style>

    <div class="formpanel formpanel_padding">
        <asp:ValidationSummary ID="valMain" runat="server" ShowSummary="False" ValidationGroup="vgMain" />
        <div class="sectiontitle">
            <asp:Label ID="lblTitle" runat="server" meta:resourcekey="lblTitleResource1">General Details</asp:Label>
        </div>
        <div class="onecolumnsmall" id="divArea" runat="server">
            <asp:Label ID="lblAreaTable" runat="server" Text="Product area" meta:resourcekey="lblAreaTableResource1" AssociatedControlID="cmbArea">

            </asp:Label><span class="inputs"><asp:DropDownList ID="cmbArea" runat="server" CssClass="fillspan" AutoPostBack="False" meta:resourcekey="cmbAreaResource1"
               onchange="javascript:SEL.EmailTemplates.ChangeBase();">
            </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span> <span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblTemplateName" CssClass="mandatory" runat="server" Text="Template name*" meta:resourcekey="lblTemplateNameResource1" AssociatedControlID="txtTemplateName"></asp:Label><span class="inputs"><asp:TextBox ID="txtTemplateName" runat="server" MaxLength="250" meta:resourcekey="txtTemplateNameResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqTempName" runat="server" ErrorMessage="Please enter a Template name." Text="*" ControlToValidate="txtTemplateName" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblSendEmail" runat="server" Text="Send email"  AssociatedControlID="ChkSendEmail"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="ChkSendEmail" /></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
            <asp:Label ID="lblSendNote" runat="server" Text="Send broadcast message" meta:resourcekey="lblBCCResource1" AssociatedControlID="ChkSendNote"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="ChkSendNote" /></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="twocolumn" id="sendMobileNotifcationCheckboxDiv" runat="server">
            <asp:Label ID="lblSendMobileNotification" runat="server" Text="Send mobile notification"  AssociatedControlID="chkCanEmailNotification"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkCanEmailNotification" /></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="sectiontitle">
            <asp:Label ID="Label1" runat="server" meta:resourcekey="lblTitleResource1">Email</asp:Label>
        </div>
       
        <div class="onecolumnsmall">
            <asp:Label ID="lblTo" CssClass="mandatory" runat="server" Text="To*" AssociatedControlID="hdnTo"></asp:Label>
            <span class="inputs"><asp:HiddenField ID="hdnTo" runat="server" /><input id="txtTo" type="text" class="fillspan" /></span>
            <span class="inputicon"><img src="/shared/images/icons/16/plain/add2.png" alt="" id="butTo" onclick="javascript:SEL.EmailTemplates.ShowAddRecipientModal('to');" /></span>
            <span class="inputvalidatorfield"><asp:CustomValidator ID="reqTo" runat="server" ErrorMessage="Please enter a To recipient." Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateToRecipient" ValidationGroup="vgMain"></asp:CustomValidator></span>
            <span class="inputtooltipfield"><img id="imgSystemTemplate" onmouseover="SEL.Tooltip.Show('DA129D5F-B356-43EB-8C86-AD0E05EE8AA1', 'ex', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblCC" runat="server" Text="CC" AssociatedControlID="hdnCC"></asp:Label>
            <span class="inputs"><asp:HiddenField ID="hdnCC" runat="server" /><input id="txtCC" type="text" class="fillspan" /></span>
            <span class="inputicon"><img src="/shared/images/icons/16/plain/add2.png" alt="" id="butCC" onclick="javascript:SEL.EmailTemplates.ShowAddRecipientModal('cc');" /></span>
            <span class="inputvalidatorfield">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
        </div>
         <div class="onecolumnsmall">
            <asp:Label ID="lblBCC" runat="server" Text="BCC" AssociatedControlID="hdnBCC"></asp:Label>
            <span class="inputs"><asp:HiddenField ID="hdnBCC" runat="server" /><input id="txtBCC" type="text" class="fillspan" /></span>
            <span class="inputicon"><img src="/shared/images/icons/16/plain/add2.png" alt="" id="butBCC" onclick="javascript:SEL.EmailTemplates.ShowAddRecipientModal('bcc');" /></span>
            <span class="inputvalidatorfield">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="Label2" runat="server" CssClass="mandatory" Text="Subject*" meta:resourcekey="lblSubjectResource1" AssociatedControlID="rtSubject"></asp:Label><span class="inputs"><asp:TextBox ID="rtSubject"  runat="server" Height="21px" TextMode="SingleLine" ToolTip="" CssClass="subject easytree-droppable" MaxLength="250" /></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Please enter a Subject."                                                                                                                                                                                                                                                                                                                                                                                                                                       Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateSubjectTextLength" ValidationGroup="vgMain"></asp:CustomValidator></span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div ID="subjectFields" class="subjectFields" runat="server" style="display: none;"></div>
        <div class="twocolumn">
            <asp:Label ID="lblPriority" runat="server" Text="Priority" meta:resourcekey="lblPriorityResource1" AssociatedControlID="cmbPriority"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbPriority" runat="server" CssClass="fillspan" meta:resourcekey="cmbPriorityResource1">
                <asp:ListItem Value="0" Text="Normal"></asp:ListItem>
                <asp:ListItem Value="2" Text="High"></asp:ListItem>
                <asp:ListItem Value="1" Text="Low"></asp:ListItem>
            </asp:DropDownList></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="twocolumn" id="sendMobileNotifcationCheckboxDiv" runat="server">
            <asp:Label ID="lblSendMobileNotification" runat="server" Text="Send mobile notification"  AssociatedControlID="chkCanEmailNotification"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkCanEmailNotification" /></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblAttachment" runat="server" Text="Attachments" meta:resourcekey="lblAttachmentsResource1" AssociatedControlID="txtAttachments"></asp:Label><span class="inputs"><asp:TextBox ID="txtAttachments" Visible="false" runat="server" CssClass="fillspan" meta:resourcekey="txtAttachmentsResource1"></asp:TextBox></span><span class="inputicon"><a href="javascript:SEL.EmailTemplates.ValidateAndShowAttachmentModal();"><img src="/shared/images/icons/16/plain/add2.png" alt="" id="butAttach" /></a></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
            <asp:Label ID="lblSystemTemplate" runat="server" Text="System template" meta:resourcekey="lblBCCResource1" AssociatedControlID="chkSystemTemplate" ></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSystemTemplate" onclick="SEL.EmailTemplates.OnSystemTemplateCheckChanged(this.id)"/></span><span class="inputicon"></span><span class="inputvalidatorfield">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
        </div>
       
        
    </div>
    <div class="formpanel formpanel_padding" style="max-width: 850px;">
        <div style="float:left; width: 60%;">
        
                <div style="background-color: rgba(206, 194, 194, 0); display: none; margin-top:110px;" id="dragTarget"></div>
                <div style="width: 100%; height: 320px; padding: 1px; margin-right:12px;" id="wrapper" runat="server">
                    <cke:CKEditorControl ID="rtBodyText" AutoPostBack="true" runat="server" Width="100%" RemovePlugins="elementspath" ToolTip="Email body."  />
                    <asp:CustomValidator ID="custxtValidator" runat="server" ErrorMessage="Please enter Email body."
                    Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateBodyTextLength" ValidationGroup="vgMain"></asp:CustomValidator>
                </div>
                <div class="sectiontitle" style="margin-top: 10px;"  ID="notesHeader" runat="server">Broadcast Message
                    <span class="inputtooltipfield"><img style="margin-right: 0px" id="imgNote" onmouseover="SEL.Tooltip.Show('FC198784-08C7-4A3E-A060-F7CDF1AC418B', 'ex', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon pull-right" /></span>
                </div>
                <div style="background-color: rgba(206, 194, 194, 0); display: none; float:left; margin-top:110px;" id="dragNotes"></div>
                <div id="noteswrapper" style="width: 100%; height: 320px;" runat="server">
                    <cke:CKEditorControl ID="txtNotes" AutoPostBack="true" runat="server" Height="290px" Width="100%" RemovePlugins="elementspath" ToolTip="Note" />
                    <asp:CustomValidator ID="custxtNoteValidator" runat="server" ErrorMessage="Please enter Broadcast Message."
                    Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateBroadcastMessageLength" ValidationGroup="vgMain"></asp:CustomValidator>
                </div>            
            <div class="sectiontitle" style="margin-top: 10px;"  ID="mobileNotificationHeader" runat="server">Mobile Notification Message
                <span class="inputtooltipfield"><img style="margin-right: 0px" id="imgNote" onmouseover="SEL.Tooltip.Show('7e52f777-fee9-495f-8046-f3a565a0dc69', 'ex', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon pull-right" /></span>
            </div>       
            <div id="mobileNotificationWrapper" style="width: 100%" runat="server">
                <asp:TextBox runat="server" Height="100px" Width="99%" ID="txtMobileNotificationMessage" TextMode="MultiLine"></asp:TextBox>
                
                <asp:CustomValidator ID="reqMobileNotificationMessage" runat="server" ErrorMessage="Please enter a mobile notification message."
                                     Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateMobileNotificationMessageIsRequired" ValidationGroup="vgMain"></asp:CustomValidator>
                <asp:CustomValidator ID="checkMessageLength" runat="server" ErrorMessage="Mobile notification message cannot be more than 400 characters in length."
                                     Text="*"  ClientValidationFunction="SEL.EmailTemplates.ValidateMobileNotificationMessageLength" ValidationGroup="vgMain"></asp:CustomValidator>
            </div>
        </div>
        
        <div id="tabs" style="max-height: 690px; height: auto; width: 35%; margin: 0 0 10px 10px; float: right; " >
            <ul>
            <li><a runat="server" ID="baseTreeHeader" class="baseTreeHeader" href="#tabFields">Base</a></li>
            <li><a href="#tabSender">From</a></li>
            <li><a href="#tabReceiver">To</a></li>
            </ul>
            <div id="tabSender">
                <div id="senderTree" style="height: 630px;"></div>
            </div> 
            <div id="tabReceiver">
                <div id="receiverTree" style="height: 630px;"></div>
            </div>

            <div id="tabFields">
                <div id="baseTree" style="height: 630px;"></div>
            </div>
         </div>

    </div>
   
    <div id="listSurname"></div>
    <div id="recipientModalForm" class="formpanel" style="display: none;" runat="server">
            <div class="twocolumn">
                <asp:Label ID="lblTeam" runat="server" AssociatedControlID="cmbTeam" Text="Team"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="cmbTeam" CssClass="fillspan" runat="server"></asp:DropDownList></span>
                <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblBudgetHolder" runat="server" AssociatedControlID="cmbBudget" Text="Budget holder"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList CssClass="fillspan" ID="cmbBudget" runat="server"></asp:DropDownList></span>
                <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblOther" runat="server" AssociatedControlID="cmbOther" Text="Other elements"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="cmbOther" CssClass="fillspan" runat="server">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="Approver" Value="5"></asp:ListItem>
                        <asp:ListItem Text="Item Owner" Value="4"></asp:ListItem>
                        <asp:ListItem Text="Line Manager" Value="0"></asp:ListItem>
                    </asp:DropDownList></span>
                <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblGreenLightAttribute" runat="server" AssociatedControlID="cmbGreenLightAttribute" Text="GreenLight attributes"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList CssClass="fillspan" ID="cmbGreenLightAttribute" runat="server"></asp:DropDownList></span>
                <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">&nbsp;</span>
            </div>
        </div>

   <div id="attachmentForm" style="display:none">
        <div class="formpanel">
            <uc1:UploadAttachment ID="usrAttachments" runat="server" />
            </div>
        </div>

   <div class="formbuttons" style="margin-top: 76px; clear: both;">
        <a href="javascript:SEL.EmailTemplates.SaveTemplate(false);" runat="server">
            <img src="/shared/images/buttons/btn_save.png" alt="" id="cmdSave" /></a>&nbsp;&nbsp;
        <a href="javascript:SEL.EmailTemplates.Cancel();">
            <img src="/shared/images/buttons/cancel_up.gif" alt="" id="cmdCancel" /></a>
    </div>
    <input type="hidden" id="hdnEmailtemplateId" />
    <asp:HiddenField runat="server" ID="baseTreeData"/>
    <asp:HiddenField runat="server" ID="employeeTreeData"/>
    <asp:HiddenField runat="server" ID="subjectHtml"/>
    <asp:HiddenField runat="server" ID="bodyHtml"/>
    <asp:HiddenField runat="server" ID="noteHtml"/>
</asp:Content>
        
        
