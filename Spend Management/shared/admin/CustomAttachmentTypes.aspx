<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomAttachmentTypes.aspx.cs" MasterPageFile="~/masters/smTemplate.master" Inherits="Spend_Management.CustomAttachmentTypes" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:HyperLink CssClass="submenuitem" ID="lnkAddCustAttachmentType" runat="server" NavigateUrl="javascript:SEL.AttachmentTypes.PopulateCustomAttachmentTypeData(null);">Add Custom Attachment</asp:HyperLink>
</asp:Content>
				
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
    <asp:ScriptManagerProxy ID="smpCustAttachmentTypes"  runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.AttachmentTypes.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAttachmentTypes.asmx" InlineScript="true" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        var modCustAttachmentTypesID = '<%=modCustAttachmentTypes.ClientID %>';
        var txtExtensionID = '<%=txtExtension.ClientID %>';
        var txtMimeHeaderID = '<%=txtMimeHeader.ClientID %>';
        var txtDescriptionID = '<%=txtDescription.ClientID %>';
        var GlobalMimeID = '';
    </script>
    <div class="formpanel formpanel_padding">
        <div id="divCustGrid">
            <asp:Literal runat="server" ID="litCustGrid"></asp:Literal>
        </div>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>

    <asp:Panel ID="pnlCustAttachmentTypes" runat="server" CssClass="modalpanel" style="display:none;">
	    <div class="formpanel">
            <div class="sectiontitle">Custom Attachment Details</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblExtension" runat="server" meta:resourcekey="lblExtensionResource1" AssociatedControlID="txtExtension">File Extension*</asp:Label><span class="inputs"><asp:TextBox ID="txtExtension" MaxLength="15" runat="server" meta:resourceKey="txtExtensionResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqExtension" ControlToValidate="txtExtension" ErrorMessage="File Extension is a mandatory field, please enter a value" Text="*" ValidationGroup="vgCustomAttach"></asp:RequiredFieldValidator></span><asp:Label CssClass="mandatory" ID="lblMimeHeader" runat="server" meta:resourcekey="lblMimeHeaderResource1" AssociatedControlID="txtMimeHeader">Mime Header*</asp:Label><span class="inputs"><asp:TextBox ID="txtMimeHeader" MaxLength="150" runat="server" meta:resourceKey="txtMimeHeaderResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqMimeHeader" ControlToValidate="txtMimeHeader" ErrorMessage="Mime Header is a mandatory field, please enter a value" Text="*" ValidationGroup="vgCustomAttach"></asp:RequiredFieldValidator></span>
            </div>
            <div class="onecolumn">
                <asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource1" AssociatedControlID="txtDescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" meta:resourceKey="txtDescriptionResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="formbuttons"><asp:Image  runat="server" ID="cmdSaveCustAttachmentType" OnClick="SEL.AttachmentTypes.SaveCustomAttachmentType();" ImageUrl="~/shared/images/buttons/btn_save.png"></asp:Image>&nbsp;&nbsp;<asp:ImageButton id="cmdCustCancelAttachmentType" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdCustCancelAttachmentTypeResource1"></asp:ImageButton></div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modCustAttachmentTypes" runat="server" TargetControlID="lnklistitem" PopupControlID="pnlCustAttachmentTypes" BackgroundCssClass="modalBackground" CancelControlID="cmdCustCancelAttachmentType"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnklistitem" runat="server" style="display:none;">LinkButton</asp:LinkButton>
</asp:Content>
