<%@ Page language="c#" Inherits="expenses.aehotel" StylesheetTheme="ExpensesTheme" Codebehind="aehotel.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>aehotel</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript">
				function changeHeader()
					{
						window.parent.parent.header.heading.innerHTML = "Add / Edit Signoff Group";
						
						window.parent.parent.header.help.href = "javascript:show_help('help/AD_UM_groups.htm'); ";
						
					}
		</script>
		<LINK type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="valdiv">
				<asp:ValidationSummary id="ValidationSummary1" runat="server"></asp:ValidationSummary>
				<asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False">Label</asp:Label>
			</div>
			<div class="inputpanel">
				<asp:Literal id="litstyles" runat="server"></asp:Literal>
				<table>
					<tr>
						<td class="labeltd">Hotel Name:</td>
						<td class="inputtd">
							<asp:TextBox id="txtname" runat="server"></asp:TextBox></td>
						<td>
							<asp:RequiredFieldValidator id="reqhotel" runat="server" ErrorMessage="Please enter the name of this hotel"
								ControlToValidate="txtname">*</asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="labeltd">Address Line 1:</td>
						<td class="inputtd">
							<asp:TextBox id="txtaddress1" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">Address Line 2:</td>
						<td class="inputtd">
							<asp:TextBox id="txtaddress2" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">City:</td>
						<td class="inputtd">
							<asp:TextBox id="txtcity" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">County:</td>
						<td class="inputtd">
							<asp:TextBox id="txtcounty" runat="server"></asp:TextBox></td>
						<td>
							<asp:RequiredFieldValidator id="reqcounty" runat="server" ErrorMessage="County is required. Please enter a value"
								ControlToValidate="txtcounty">*</asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="labeltd">Postcode:</td>
						<td class="inputtd">
							<asp:TextBox id="txtpostcode" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">Country:</td>
						<td class="inputtd">
							<asp:TextBox id="txtcountry" runat="server"></asp:TextBox></td>
						<td>
							<asp:RequiredFieldValidator id="reqcountry" runat="server" ControlToValidate="txtcountry" ErrorMessage="Country is requried, please enter a value">*</asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="labeltd">Tel Num:</td>
						<td class="inputtd">
							<asp:TextBox id="txttelno" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">Email Address:</td>
						<td class="inputtd">
							<asp:TextBox id="txtemail" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td class="labeltd">Rating:</td>
						<td class="inputtd">
							<asp:DropDownList id="cmbrating" runat="server">
								<asp:ListItem></asp:ListItem>
								<asp:ListItem Value="1">1 Star</asp:ListItem>
								<asp:ListItem Value="2">2 Stars</asp:ListItem>
								<asp:ListItem Value="3">3 Stars</asp:ListItem>
								<asp:ListItem Value="4">4 Stars</asp:ListItem>
								<asp:ListItem Value="5">5 Stars</asp:ListItem>
							</asp:DropDownList></td>
					</tr>
				</table>
			</div>
			<div class="inputpanel">
				<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png"></asp:ImageButton>&nbsp;&nbsp;<a href="hotelsearch.aspx"><img border="0" src="buttons/cancel_up.gif"></a>
			</div>
		</form>
	</body>
</HTML>
