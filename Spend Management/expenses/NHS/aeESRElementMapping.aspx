<%@ Page Title="ESR Element Mapping" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeESRElementMapping.aspx.cs" Inherits="Spend_Management.aeESRElementMapping" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/expenses/webservices/ESR.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/ESRElementMappings.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        //<![CDATA[
        var ElementID = '<% = ElementID %>';
        var ElementDDLID = '<% = ddlESRElements.ClientID %>';
        var FieldsPanelID = '<% = pnlESRElementFields.ClientID %>';
        var SubCatsPanelID = '<% = pnlSubCats.ClientID %>';
        var TrustID = '<% = TrustID %>';
        //]]>
    </script>
    <div class="formpanel">
        <cc1:TabContainer ID="TabContainer1" runat="server">
            <cc1:TabPanel ID="tabESRElements" runat="server">
                <HeaderTemplate>ESR Elements</HeaderTemplate>
                <ContentTemplate>
                    <div>
                        <div class="sectiontitle">General Options</div>
                        <asp:Panel ID="pnlESRElements" runat="server" CssClass="onecolumnsmall">
                            <asp:Label ID="lblElement" runat="server" Text="ESR Element" AssociatedControlID="ddlESRElements"></asp:Label><span class="inputs"><asp:DropDownList
                                ID="ddlESRElements" runat="server" 
                                onselectedindexchanged="ddlESRElements_SelectedIndexChanged" 
                                AutoPostBack="True">
                            </asp:DropDownList></span><span class="inputvalidatorfield"><asp:CompareValidator ID="cvESRElements" Text="*" ControlToValidate="ddlESRElements" ValueToCompare="0" ValidationGroup="general" Operator="GreaterThan" runat="server" ErrorMessage="CompareValidator"></asp:CompareValidator></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span></asp:Panel>
                        <div class="sectiontitle">ESR Element Field Mappings</div>
                        <asp:UpdatePanel ID="upnlESRElementFields" runat="server" UpdateMode="Conditional">
                            <Triggers><asp:AsyncPostBackTrigger ControlID="ddlESRElements" EventName="selectedindexchanged" /></Triggers>
                            <ContentTemplate>
                                <asp:Panel ID="pnlESRElementFields" runat="server">
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabSubCats" runat="server">
                <HeaderTemplate>Expense Item Types</HeaderTemplate>
                <ContentTemplate>
                    <div>
                        <div class="sectiontitle">Associated Expense Item Types</div>
                        <asp:Panel ID="pnlSubCats" runat="server"></asp:Panel>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
        <div class="formbuttons"><asp:Image ID="btnSubmit" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="javascript:SaveESRElementMapping();" /> <asp:Image ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="javascript:CancelESRElementMapping();" /></div>
    </div>
</asp:Content>
