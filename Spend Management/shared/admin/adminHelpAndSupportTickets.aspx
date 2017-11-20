<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="adminHelpAndSupportTickets.aspx.cs" Inherits="Spend_Management.shared.admin.adminHelpAndSupportTickets" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
	<div class="formpanel formpanel_padding">
	    
        <div class="twocolumn">
    	    <asp:Label runat="server" AssociatedControlID="cmbStatus">Status filter</asp:Label>
		    <span class="inputs"><asp:DropDownList runat="server" ID="cmbStatus" CssClass="cmbfilter" onchange="javascript:SEL.Grid.filterGridCombo('gridInternalTickets','Status', true);"/></span>
	    </div>

        <asp:Panel ID="pnlGrid" runat="server" CssClass="formpanel formpanel_padding">
            <asp:Literal ID="litInternalTickets" runat="server"></asp:Literal>
        </asp:Panel>

        
            <asp:ImageButton ID="cmdClose" OnClick="CmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
       

    </div>

</asp:Content>
