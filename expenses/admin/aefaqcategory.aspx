<%@ Page language="c#" Inherits="expenses.admin.aefaqcategory" MasterPageFile="~/expform.master" Codebehind="aefaqcategory.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcategory" runat="server" meta:resourcekey="lblcategoryResource1">Category</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtcategory" runat="server" meta:resourcekey="txtcategoryResource1" MaxLength="50"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqcategory" runat="server" ErrorMessage="Please enter the category name into the box provided"
						ControlToValidate="txtcategory" meta:resourcekey="reqcategoryResource1">*</asp:RequiredFieldValidator>
						
						</td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" CausesValidation="False" ImageUrl="../buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

    </asp:Content>

