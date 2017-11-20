<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="viewentities.aspx.cs" Inherits="Spend_Management.viewentities" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Import Namespace="SpendManagementLibrary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:Literal ID="litadd" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

<asp:ScriptManagerProxy runat="server" ID="smProxy">
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/customEntities.js" />
<asp:ScriptReference Path="~/shared/javaScript/shared.js" />
<asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />
</Scripts>
<Services>
<asp:ServiceReference Path="~/shared/webServices/svcCustomEntities.asmx" />
</Services>
</asp:ScriptManagerProxy>
<script type="text/javascript" language="javascript">
    var sGridID = '<% = sGridID %>';
    var mdlRejectID = '<% = mdlRejectRecord.ClientID %>';
    var txtReasonForRejectionID = '<% = txtReasonForRejection.ClientID %>';
    function runWorkflow (recordid, entityID)
    {
        PageMethods.runWorkflow (accountid, recordid, entityID)
    }
    function approveRecord(recordid, entityID)
    {
        PageMethods.approve(accountid, recordid, entityID);
        SEL.Grid.deleteGridRow(sGridID, recordid);
    }
    function updateDecisionTrue(recordid, entityID)
    {
        PageMethods.updateDecision(accountid, recordid, entityID, true);
    }

    (function (a)
    {
        a.Modal = '<%= mdlFormSelectionAttributeValue.ClientID %>';
        a.Panel = '<%= pnlFormSelectionAttributeValue.ClientID %>';
        a.PanelTitle = '<%= pnlFormSelectionAttributeValue.ClientID %>';
        a.PanelBody = '<%= mdlFormSelectionAttributeValue.ClientID %>';
    }(SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd));
    
</script>
<div class="formpanel formpanel_padding">
<div class="sectiontitle" id="divviewname" runat="server">
View Name</div>
    <asp:Literal ID="litgrid" runat="server"></asp:Literal>
    <div class="formbuttons">
    <asp:ImageButton runat="server" ID="cmdClose" 
            ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" 
            onclick="cmdClose_Click"></asp:ImageButton>
    </div>
</div>

<cc1:ModalPopupExtender ID="mdlRejectRecord" runat="server" PopupControlID="pnlRejectRecord" BackgroundCssClass="modalBackground" TargetControlID="hlRejectRecord" OnOkScript="RejectRecord" OnCancelScript="CloseRejectModal"></cc1:ModalPopupExtender>
<asp:Panel ID="pnlRejectRecord" runat="server" CssClass="modalpanel formpanel" style="display:none;">
    <div class="sectiontitle">Reason for Rejection</div>
    <div class="onecolumn"><label for="txtReasonForRejection" class="mandatory">Reason *</label><span class="inputs"><asp:TextBox ID="txtReasonForRejection" runat="server" Columns="20" Rows="4" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqRejectReason" runat="server" ControlToValidate="txtReasonForRejection" Text="*" ErrorMessage="Please provide a reason for the rejection of this request, as it is required information" ValidationGroup="rejectModal"></asp:RequiredFieldValidator></span></div>
    <div class="formbuttons"><img src="/shared/images/buttons/btn_reject.png" alt="Reject" id="btnRejectRecord" /> <img src="/shared/images/buttons/cancel_up.gif" alt="Cancel" onclick="CloseRejectModal();" /></div>
</asp:Panel>
<asp:HyperLink ID="hlRejectRecord" runat="server" style="display: none;">&nbsp;</asp:HyperLink>

    <asp:Panel ID="pnlFormSelectionAttributeValue" runat="server" CssClass="modalpanel formpanel" style="display:none;">
        <div class="sectiontitle">Form Selection</div>
        <div class="onecolumnsmall" id="textFormSelectionAttributeValue" runat="server">
            <asp:Label ID="lblFormSelectionAttributeTextValue" runat="server" AssociatedControlID="txtFormSelectionAttributeTextValue">Text Attribute</asp:Label><span class="inputs"><asp:TextBox ID="txtFormSelectionAttributeTextValue" runat="server" MaxLength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvFormSelectionAttributeTextValue" runat="server" ControlToValidate="txtFormSelectionAttributeTextValue" Text="*" ErrorMessage="Please enter a value for Attribute Name." ValidationGroup="vgFormSelectionAttributeTextValue"></asp:RequiredFieldValidator></span>
        </div>
        <div class="onecolumnsmall" id="listFormSelectionAttributeValue" runat="server">
            <asp:Label ID="lblFormSelectionAttributeListValue" runat="server" AssociatedControlID="ddlFormSelectionAttributeListValue">List Attribute</asp:Label><span class="inputs"><asp:DropDownList ID="ddlFormSelectionAttributeListValue" runat="server"/></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvFormSelectionAttributeListValue" runat="server" ControlToValidate="ddlFormSelectionAttributeListValue" Text="*" ErrorMessage="Please enter a value for Attribute Name." ValidationGroup="vgFormSelectionAttributeValue"></asp:RequiredFieldValidator></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" Text="save" OnClientClick="SEL.CustomEntities.FormSelection.Attribute.ViewAddSave();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton runat="server" Text="cancel" OnClientClick="SEL.CustomEntities.FormSelection.Attribute.ViewAddCancel();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="mdlFormSelectionAttributeValue" runat="server" PopupControlID="pnlFormSelectionAttributeValue" TargetControlID="lnkFormSelectionAttributeValue" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:HyperLink ID="lnkFormSelectionAttributeValue" runat="server" style="display: none;">&nbsp;</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" src="<%=GlobalVariables.StaticContentLibrary%>/js/expense/jquery.smoothscroll.js"></script>
     <script type="text/javascript">
         $(window).load(function () {
// Initialize the scroll after the page contents are loaded
             $('#maindiv div.formpanel > div:eq(1)').customScroll({ cursorwidth: '7px', cursorcolor: "#19a2e6", autohidemode: false });
// Resizing the scroll area dynamically
             $("#menu-toggle").click(function () {
                // $('#maindiv div.formpanel > div:eq(1)').getNiceScroll().hide();
                 setTimeout(function () {
                     $('#maindiv div.formpanel > div:eq(1)').getNiceScroll().resize();
                   //  $('#maindiv div.formpanel > div:eq(1)').getNiceScroll().show();
                 }, 500);

             });
         });
    </script>
</asp:Content>
