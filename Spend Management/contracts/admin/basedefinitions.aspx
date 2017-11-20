<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="basedefinitions.aspx.cs" Inherits="Spend_Management.basedefinitions" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
<asp:ScriptManagerProxy runat="server" ID="smProxy">
<Scripts>
    <asp:ScriptReference Path="~/contracts/javascript/basedefinitions.js" />
    <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
</Scripts>
<Services>
    <asp:ServiceReference Path="~/contracts/webservices/svcBaseDefinitions.asmx" />
</Services>
</asp:ScriptManagerProxy>

<script language="javascript" type="text/javascript">
    var modalDefEdit = '<%= modDefEdit.ClientID %>';
    var validationGroup = '<%= valGroup %>';
    var baseDefObject = new baseDefinitions();
    baseDefObject.bdTableId = '<%=baseTableId %>';
    baseDefObject.element = '<%=currentElement %>';
    
</script>

<asp:Literal runat="server" ID="litAddDef"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<div class="formpanel formpanel_padding">
    <div class="sectiontitle">Current Definitions</div>
        <div id="bdefDivPanel">
            <img src="../../shared/images/ajax-loader.gif" alt="Loading..." />
        </div>
        <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div> 
    </div>
     
    <asp:Panel ID="pnlDefEdit" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">Definition Details</div>
            <asp:PlaceHolder runat="server" ID="phBDefFields"></asp:PlaceHolder>
            <div class="formbuttons">
                <img src="/shared/images/buttons/btn_save.png" alt="Save" id="cmdSave" onclick="javascript:baseDefObject.saveDefinition();" />
                <asp:ImageButton ID="cmdDefCancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" runat="server" CausesValidation="False" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modDefEdit" runat="server" TargetControlID="lnkBaseDef"
        PopupControlID="pnlDefEdit" BackgroundCssClass="modalBackground"
        CancelControlID="cmdDefCancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkBaseDef" style="display: none;"></asp:LinkButton>
</asp:Content>
