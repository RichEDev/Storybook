<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="~/shared/Templates/Template.aspx.cs" Inherits="Spend_Management.shared.Templates.TemplatePage" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');$g(SEL.Template.DomIDs.General.TemplateItemName).focus();" id="lnkGeneral" class="selectedPage">Template Page Details</a> <a href="javascript:SEL.Template.TemplateItem.Focus();" id="lnkTemplateItems">Template Items</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentoptions" runat="server">
    <div id="pgOptLevels" style="display: none;">
        <a href="javascript:SEL.Template.TemplateItem.New();" class="submenuitem" ID="lnkNewTemplateItem" runat="server">New Template Item</a> 
    </div>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="sel.template.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

<script type="text/javascript" language="javascript">
    (function (r) {
        r.General.TemplateItemName = '<%= txtTemplateItemName.ClientID %>';
        r.General.TemplateItemDescription = '<%= txtTemplateItemDescription.ClientID %>';
        r.General.TemplateItemNameRequiredValidator = '<%= reqTemplateItemName.ClientID %>';
        r.TemplateItemSubItem.TemplateItemSubItemName = '<%= txtTemplateItemSubItemName.ClientID %>';
        r.TemplateItemSubItem.TemplateItemSubItemDescription = '<%= txtTemplateItemDescription.ClientID %>';
        r.TemplateItemSubItem.TemplateItemSubItemNameRequiredValidator = '<%= reqTemplateItemSubItemName.ClientID %>';        
        r.TemplateItemSubItem.Modal = '<%= modItemTemplateSubItem.ClientID %>';
        r.TemplateItemSubItem.Panel = '<%= pnlTemplateItemSubItem.ClientID %>';
        r.TemplateItemSubItem.Grid = '<%= pnlTemplateItemSubItemGrid.ClientID %>';
    }(SEL.ApprovalMatrices.DomIDs));

    $(document).ready(function () {
        SEL.Common.SetTextAreaMaxLength();
        SEL.Template.SetupEnterKeyBindings();
    });

</script>
    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <div class="formpanel">
                <div class="sectiontitle">General Details</div>
                <div class="twocolumn">
                    <asp:Label CssClass="mandatory" ID="lblTemplateItemName" runat="server" Text="Template item name*" AssociatedControlID="txtTemplateItemName"></asp:Label><span class="inputs"><asp:TextBox ID="txtTemplateItemName" runat="server" CssClass="fillspan" MaxLength="250" ></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftbeTemplateItemName" runat="server" TargetControlID="txtTemplateItemName" FilterMode="InvalidChars" InvalidChars="<>" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqTemplateItemName" runat="server" ErrorMessage="Please enter a Template item name." Text="*" ControlToValidate="txtTemplateItemName" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator></span>
                </div>
                <div class="onecolumn">
                    <asp:Label ID="lblTemplateItemDescription" runat="server" Text="Description" AssociatedControlID="txtTemplateItemDescription" ></asp:Label><span class="inputs"><asp:TextBox ID="txtTemplateItemDescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
                </div>
            </div>
        </div>
        <div id="pgLevels" class="subPage" style="display: none;">
            <div class="formpanel">
                <asp:Panel ID="pnlTemplateItemSubItemGrid" runat="server">
                    <asp:Literal runat="server" ID="litgrid"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div class="formpanel formbuttons">
            <helpers:CSSButton id="btnTemplateItemSave" runat="server" text="save" onclientclick="SEL.Template.TemplateItem.Save('templateitem');return false;" UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnTemplateItemCancel" runat="server" text="cancel" onclientclick="SEL.Template.TemplateItem.Cancel();return false;" UseSubmitBehavior="False"/>
        </div>
    </div>
        <asp:Panel ID="pnlTemplateItemSubItem" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle" id="divTemplateItemSubItems">Template Item Sub Items</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblTemplateItemSubItemName" runat="server" Text="Template item sub item name*" AssociatedControlID="txtTemplateItemSubItemName"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtTemplateItemSubItemName" MaxLength="250" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqTemplateItemSubItemName" runat="server" ErrorMessage="Please enter a Template item sub item name." Text="*" ControlToValidate="txtTemplateItemSubItemName" ValidationGroup="vgLevel" Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnTemplateItemSubItemSave" runat="server" Text="save" onclientclick="SEL.Template.TemplateItemSubItem.Save();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnTemplateItemSubItemCancel" runat="server" Text="cancel" onclientclick="SEL.Template.TemplateItemSubItem.Cancel();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modItemTemplateSubItem" runat="server" TargetControlID="lnkTemplateItemSubItem" PopupControlID="pnlTemplateItemSubItem" BackgroundCssClass="modalBackground" CancelControlID="btnTemplateItemSubItemCancel" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkTemplateItemSubItem" runat="server" Style="display: none;"></asp:LinkButton>
    
    
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
</asp:Content>
