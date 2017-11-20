<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttachmentTypes.aspx.cs" MasterPageFile="~/masters/smTemplate.master" Inherits="Spend_Management.AttachmentTypes" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:HyperLink CssClass="submenuitem" ID="lnkAddAttachmentType" runat="server" NavigateUrl="javascript:SEL.Common.ShowModal(modAttachmentTypesID); SEL.AttachmentTypes.GetAttachmentTypeData();">Add Attachment Type</asp:HyperLink>
</asp:Content>
				
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
    <asp:ScriptManagerProxy ID="smpAttachmentTypes"  runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.AttachmentTypes.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAttachmentTypes.asmx" InlineScript="true" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        var modAttachmentTypesID = '<%=modAttachmentTypes.ClientID %>';
        var cmbAttachmentTypeID = '<%=cmbAttachmentType.ClientID %>';
    </script>
    <div class="formpanel formpanel_padding">
        <div id="divGrid">
            <asp:Literal runat="server" ID="litGrid"></asp:Literal>
        </div>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False" AlternateText="Close"></asp:ImageButton>
        </div> 
    </div>

    <asp:Panel ID="pnlAttachmentTypes" runat="server" CssClass="modalpanel" style="display:none;">
	    <div class="formpanel">
            <div class="sectiontitle">Attachment Type Details</div>
            <div class="onecolumnsmall">
                <asp:Label CssClass="mandatory" ID="lblAttachmentType" runat="server" meta:resourcekey="lblAttachmentTypeResource1" AssociatedControlID="cmbAttachmentType">Attachment File Type</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbAttachmentType" runat="server" meta:resourcekey="cmbAttachmentTypeResource1"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="formbuttons"><asp:Image  runat="server" ID="cmdSaveAttachmentType" OnClick="SEL.AttachmentTypes.SaveAttachmentType();" ImageUrl="~/shared/images/buttons/btn_save.png"></asp:Image>&nbsp;&nbsp;<asp:ImageButton id="cmdCancelAttachmentType" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdCancelAttachmentTypeResource1"></asp:ImageButton></div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modAttachmentTypes" runat="server" TargetControlID="lnklistitem" PopupControlID="pnlAttachmentTypes" BackgroundCssClass="modalBackground" CancelControlID="cmdCancelAttachmentType"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnklistitem" runat="server" style="display:none;">LinkButton</asp:LinkButton>

</asp:Content>
