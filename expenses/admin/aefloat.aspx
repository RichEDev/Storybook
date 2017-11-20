<%@ Page language="c#" Inherits="expenses.aefloat" MasterPageFile="~/expform.master" Codebehind="aefloat.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAutocomplete.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/employees.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lbladvancename" runat="server" meta:resourcekey="lbladvancenameResource1">Advance Name:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtname" runat="server" MaxLength="50" meta:resourcekey="txtnameResource1"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqname" runat="server" ErrorMessage="Please enter a name for this float" ControlToValidate="txtname" meta:resourcekey="reqnameResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" vAlign="top">
					<asp:Label id="lblreason" runat="server" meta:resourcekey="lblreasonResource1">Reason for Advance:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtreason" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtreasonResource1"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblemployee" runat="server" meta:resourcekey="lblemployeeResource1">Employee Name:</asp:Label></td>
				<td class="inputtd">
					<asp:PlaceHolder ID="placeEmp" runat="server"></asp:PlaceHolder><%--<asp:dropdownlist id="cmbemployees" runat="server" meta:resourcekey="cmbemployeesResource1"></asp:dropdownlist>--%></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcurrency" runat="server" meta:resourcekey="lblcurrencyResource1">Currency:</asp:Label></td>
				<td class="inputtd">
					<asp:dropdownlist id="cmbcurrencies" runat="server" meta:resourcekey="cmbcurrenciesResource1"></asp:dropdownlist></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblamount" runat="server" meta:resourcekey="lblamountResource1">Amount:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtamount" runat="server" Width="50px" meta:resourcekey="txtamountResource1"></asp:textbox></td>
				<td><asp:requiredfieldvalidator id="reqamount" runat="server" ControlToValidate="txtamount" ErrorMessage="Please enter the amount required for this float" meta:resourcekey="reqamountResource1">*</asp:requiredfieldvalidator><asp:comparevalidator id="compamount" runat="server" ControlToValidate="txtamount" ErrorMessage="Please enter a valid amount"
						Operator="DataTypeCheck" Type="Currency" meta:resourcekey="compamountResource1">*</asp:comparevalidator></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:imagebutton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:imagebutton>&nbsp;&nbsp;<asp:imagebutton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:imagebutton></div>
	<asp:TextBox id="txtfloatid" runat="server" Visible="False" meta:resourcekey="txtfloatidResource1"></asp:TextBox>
	<asp:TextBox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:TextBox>
        </asp:Content>

