<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="aeDocTemplatesAssociation.aspx.cs" Inherits="Spend_Management.aeDocTemplatesAssociation" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:content id="Content1" contentplaceholderid="contentmenu" runat="server">
    <asp:HyperLink runat="server" ID="lnkNew" Text="New Association" NavigateUrl="javascript:launchAddAssocsModal();"
		        CssClass="submenuitem"></asp:HyperLink>
</asp:content>
<asp:content id="Content2" contentplaceholderid="contentleft" runat="server">
    
</asp:content>
<asp:content id="Content3" contentplaceholderid="contentmain" runat="server">
    <script language="javascript" type="text/javascript">
        docID = <%= ViewState["docid"] %>;
    </script>
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/documentTemplate.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcDocumentTemplates.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">
        var mdlAddAssocs = '<%= mdlAddAssocs.ClientID %>';
        var mdlMessage = '<%= mdlMessage.ClientID %>';
    </script>

    <div class="formpanel formpanel_padding">
       <div class="sectiontitle">Document Template Associations</div>
            <div class="twocolumn">
            <div id="divGrid" name="divGrid" style="width:500px;">
                <asp:Literal runat="server" id="litGrid"></asp:Literal>
            </div>
        </div>
			<div class="formbuttons">
                <asp:ImageButton runat="server" ID="btnClose" 
        ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" CausesValidation="False"
        onclick="btnClose_Click" />
	        </div>
    </div>

    <asp:Panel ID="pnlAddAssocs" runat="server" CssClass="errorModal">
        <div id="pnlAddAssociation" class="formpanel" style="overflow:auto;">
		    <div class="sectiontitle">Add New Document Template Association</div>
            <div class="twocolumn">
                <div id="divGridAvailable" name="divGridAvailable">
    		        <asp:Literal id="ltGridAssociationChoices" runat="server" />
                </div>
		    </div>
		    <div class="formbuttons">
		        <asp:ImageButton id="cmdok" runat="server" 
                    ImageUrl="../images/buttons/btn_save.png" ValidationGroup="vgMain" 
                    onclick="cmdok_Click"></asp:ImageButton>&nbsp;&nbsp;
		        <asp:ImageButton id="cmdcancel" runat="server" 
                    ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False"></asp:ImageButton>
	        </div>
        </div>
    </asp:Panel>

    <asp:Label runat="server" Text="" id="lblDummy" />
    <cc1:ModalPopupExtender 
        ID="mdlAddAssocs" 
        TargetControlID="lblDummy"
        runat="server" 
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnlAddAssocs" 
        DropShadow="False"
        CancelControlID="cmdcancel">
    </cc1:ModalPopupExtender>


    <asp:Panel ID="pnlMessage" runat="server" CssClass="errorModal" style="display:none;">
        <div id="divMessage" class="formpanel">
		    <div class="sectiontitle">New Template Association</div>
            <div class="twocolumn">
    		    <asp:Label id="lblMessage" runat="server" />
		    </div>
		    <div class="formbuttons">
		        <asp:ImageButton id="cmdMessageok" runat="server" 
                    ImageUrl="../images/buttons/btn_close.png" CausesValidation="false"></asp:ImageButton>
	        </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender 
        ID="mdlMessage" 
        TargetControlID="lblDummy"
        runat="server" 
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnlMessage" 
        DropShadow="False"
        CancelControlID="cmdMessageok">
    </cc1:ModalPopupExtender>

</asp:content>
