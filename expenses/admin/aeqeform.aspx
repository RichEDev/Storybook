<%@ Page language="c#" Inherits="expenses.aeqeform" MasterPageFile="~/expform.master" Codebehind="aeqeform.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton id="cmdformdesign" runat="server" CssClass="submenuitem" onclick="cmdformdesign_Click" meta:resourcekey="cmdformdesignResource1">Change Form Design</asp:LinkButton>
    <asp:LinkButton id="cmdprintout" runat="server" CssClass="submenuitem" onclick="cmdprintout_Click" meta:resourcekey="cmdprintoutResource1">Change Print-Out</asp:LinkButton>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">				
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblformname" runat="server" meta:resourcekey="lblformnameResource1">Form Name:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtname" runat="server" meta:resourcekey="txtnameResource1"  MaxLength="100"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqname" runat="server" ErrorMessage="Please enter a name for this form" ControlToValidate="txtname" meta:resourcekey="reqnameResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description</asp:Label>:</td>
				<td class="inputtd">
					<asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Quick Entry Form Design</asp:Label></div>
		<table>
			<tr>
				<td>
					<asp:RadioButton id="optgenmonth" runat="server" GroupName="rows" Checked="True" meta:resourcekey="optgenmonthResource1"></asp:RadioButton></td>
				<td>
					<asp:Label id="lblgenmonth" runat="server" meta:resourcekey="lblgenmonthResource1">Generate a row for each day of the month</asp:Label></td>
			</tr>
			<tr>
				<td>
					<asp:RadioButton id="optnumrows" runat="server" GroupName="rows" meta:resourcekey="optnumrowsResource1"></asp:RadioButton></td>
				<td class="Generate_input">
                    Generate
					<asp:TextBox id="txtnumrows" runat="server" Width="30px" MaxLength="2" meta:resourcekey="txtnumrowsResource1"></asp:TextBox>rows
					<asp:CompareValidator id="comprows" runat="server" ErrorMessage="Please enter a valid value in the Generate Numbers of rows box"
						Operator="DataTypeCheck" Type="Integer" ControlToValidate="txtnumrows" meta:resourcekey="comprowsResource1">*</asp:CompareValidator><asp:RequiredFieldValidator ID="rfvRows" runat="server" ErrorMessage="The Generate X rows field should not be blank" Display="Dynamic" ControlToValidate="txtnumrows">*</asp:RequiredFieldValidator></td>
			</tr>
		</table>
        <br/><br/><br/>
		<div class="aeqeform_table"><asp:Literal id="litform" runat="server" meta:resourcekey="litformResource1"></asp:Literal></div>
	</div>
	<div class="inputpanel ">
		<div style="margin-right:10px;float: left;"><asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton></div>
        <a href="adminqeforms.aspx"><img src="../buttons/cancel_up.gif"></a>
        

	</div>

   </asp:Content>


