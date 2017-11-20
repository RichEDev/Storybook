<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="admindoctemplates.aspx.cs" Inherits="Spend_Management.admindoctemplates" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="contentmain">
<asp:ScriptManagerProxy ID="smDocTemplates" runat="server">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcDocumentMerge.asmx" />
        <asp:ServiceReference Path="~/shared/webServices/svcDocumentTemplates.asmx" />
    </Services>
    <Scripts>
        <asp:ScriptReference Path="~/shared/javascript/documentTemplate.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<script language="javascript" type="text/javascript">
    var copynewmodalcntl = '<%=modcopynew.ClientID %>';
    var copynewtitletxt = '<%=txtCopyNewTitle.ClientID %>';
    var copynewdesctxt = '<%=txtCopyNewDesc.ClientID %>';
</script>
<div class="formpanel formpanel_padding">
<div class="sectiontitle">Document Templates</div>
<asp:Literal runat="server" ID="litMsg"></asp:Literal>
<div id="divDocTemplates" runat="server">
    <asp:Literal runat="server" ID="litDocTemplates"></asp:Literal>
</div>
<div class="formbuttons">
<asp:ImageButton runat="server" ID="btnClose" 
        ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" 
        onclick="btnClose_Click" />
</div></div>      
</asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" onclick="lnkNew_Click" CssClass="submenuitem">New Template</asp:LinkButton>
                
<asp:Panel ID="pnlcopynew" runat="server" CssClass="modalpanel" Style="display: none;">
<div class="formpanel">
    <div class="sectiontitle">Copy Template</div>
    <div class="twocolumn">
        <asp:Label runat="server" id="lblCopyNewTitle" AssociatedControlID="txtCopyNewTitle" CssClass="mandatory">Template name*</asp:Label>
        <span class="inputs"><asp:TextBox runat="server" ID="txtCopyNewTitle" CssClass="fillspan" MaxLength="150"></asp:TextBox></span>
        <span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqCopyNewTitle" Text="*" ValidationGroup="vgCopyNewTitle" ErrorMessage="A new template name must be specified" ControlToValidate="txtCopyNewTitle"></asp:RequiredFieldValidator></span>
    </div>
    <div class="onecolumn">
        <asp:Label runat="server" ID="lblCopyNewDesc" AssociatedControlID="txtCopyNewDesc">Template description</asp:Label>
        <span class="inputs"><asp:TextBox runat="server" ID="txtCopyNewDesc" CssClass="fillspan" TextMode="MultiLine"></asp:TextBox></span>
        <span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
    </div>
    <div class="formbuttons">
        <img id="cmdcopynewok" alt="OK" src="../images/buttons/btn_save.png" onclick="javascript:if (validateform('vgCopyNewTitle') == false) { return false; } else { SaveTemplateCopy(); }" />&nbsp;&nbsp;<asp:ImageButton runat="server" ID="cmdcopynewcancel" AlternateText="Cancel" ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" />
    </div>
</div>
</asp:Panel>
<asp:LinkButton runat="server" ID="lnkdummy"></asp:LinkButton>
<cc1:ModalPopupExtender ID="modcopynew" runat="server" TargetControlID="lnkdummy"
        PopupControlID="pnlcopynew" BackgroundCssClass="modalBackground" CancelControlID="cmdcopynewcancel">
    </cc1:ModalPopupExtender>


    <asp:Label runat="server" Text="" id="lblDummy" />

    <asp:Panel ID="pnlMessage" runat="server" CssClass="errorModal" style="display:none;">
        <div id="divMessage" runat="server">
            <asp:Label id="lblMessage" runat="server" Text="" />
        </div>
        <div style="padding: 0px 5px 5px 5px;">
            <img src="/shared/images/buttons/btn_close.png" id="btnMessageClose" alt="OK" onclick="" style="cursor: pointer;" />
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender 
        ID="mdlMessage" 
        TargetControlID="lblDummy"
        runat="server" 
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnlMessage" 
        DropShadow="False"
        CancelControlID="btnMessageClose">
    </cc1:ModalPopupExtender>

</asp:Content>
