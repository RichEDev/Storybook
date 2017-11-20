<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="admin_aeitemrole" Codebehind="aeitemrole.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentleft" Runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript" language="javascript">

        

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmain" Runat="Server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" />
<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
    <table>
		<tr>
			<td class="labeltd"><asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Role Name</asp:Label></td>
			<td class="inputtd"><asp:textbox id="txtrolename" runat="server" MaxLength="50" meta:resourcekey="txtrolenameResource1"></asp:textbox></td>
			<td><asp:requiredfieldvalidator id="reqrolename" runat="server" ControlToValidate="txtrolename" ErrorMessage="Please enter a Role Name" meta:resourcekey="reqrolenameResource1">*</asp:requiredfieldvalidator></td>
		</tr>
		<tr>
			<td class="labeltd" vAlign="top"><asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description</asp:Label></td>
			<td class="inputtd"><asp:textbox id="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:textbox></td>
		</tr>
	</table>
</div>

<div class="inputpanel table-border2">
    <div class="inputpaneltitle">
        <asp:Label ID="lblallocateditems" runat="server" Text="Allocated Expense Items" meta:resourcekey="lblallocateditemsResource1"></asp:Label></div>
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Table ID="tblitems" runat="server" CssClass="datatbl" meta:resourcekey="tblitemsResource1">
        </asp:Table>
    </ContentTemplate>
    </asp:UpdatePanel>
    
    
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
    <asp:HyperLink ID="lnkadditem" CssClass="dropdownlink" runat="server" Font-Underline="True" meta:resourcekey="lnkadditemResource1">Add Expense Item</asp:HyperLink>
    <asp:Panel ID="pnlitems" runat="server" CssClass="dropdownholder" meta:resourcekey="pnlitemsResource1">
        <div id="dropdownblock">&nbsp;</div>
        
        
        
        </asp:Panel>
        <cc1:DropDownExtender ID="DropDownExtender1" runat="server" TargetControlID="lnkadditem" DropDownControlID="pnlitems" DynamicServicePath="" Enabled="True">
        </cc1:DropDownExtender>
    </ContentTemplate>
        
    </asp:UpdatePanel>
    
    
</div>

<div class="inputpanel">
    <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;<a href="adminitemroles.aspx"><img alt="Cancel" border="0" src="../buttons/cancel_up.gif" /></a>
</div>
</asp:Content>

