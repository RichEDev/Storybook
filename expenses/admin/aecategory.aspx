<%@ Page language="c#" Inherits="expenses.aecategory" MasterPageFile="~/expform.master" Codebehind="aecategory.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" Font-Size="Small" meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcategory" runat="server" meta:resourcekey="lblcategoryResource1">Category Name:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtcategory" runat="server" MaxLength="50" meta:resourcekey="txtcategoryResource1" style="width: 300px;"></asp:textbox></td>
				<td><asp:requiredfieldvalidator id="reqcategory" runat="server" ErrorMessage="Please enter a category name" ControlToValidate="txtcategory" meta:resourcekey="reqcategoryResource1">*</asp:requiredfieldvalidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top">
					<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1" style="width: 500px;height:70px;"></asp:textbox></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel"><asp:imagebutton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:imagebutton>&nbsp;&nbsp;<asp:imagebutton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:imagebutton></div>
	<CENTER><asp:textbox id="txtcategoryid" runat="server" Visible="False" meta:resourcekey="txtcategoryidResource1"></asp:textbox>
		<asp:TextBox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:TextBox></CENTER>

    </asp:Content>

