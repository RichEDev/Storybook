<%@ Page language="c#" Inherits="expenses.aeclaim" MasterPageFile="~/expform.master" Codebehind="aeclaim.aspx.cs" EnableSessionState="ReadOnly" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/userdefined.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

     <div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	
    
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Claim Name:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtname" runat="server" MaxLength="50" meta:resourcekey="txtnameResource1"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqclaimname" runat="server" ErrorMessage="Please enter a value for the Claim Name field"
						ControlToValidate="txtname" meta:resourcekey="reqclaimnameResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top">
					<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></td>
			</tr>
			
			
		</table>
        <asp:Table ID="tbludf" runat="server" meta:resourcekey="tbludfResource1"></asp:Table>
	</div>
	
    <div class="inputpanel" >
        <asp:Label ID="lblduplicate" runat="server" Text="The claim cannot be saved as the name you have entered already exists." ForeColor="Red" Visible="false"></asp:Label></div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="ImageButton1" runat="server" ImageUrl="~/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="ImageButton1Resource1"></asp:ImageButton>
		<asp:TextBox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:TextBox>
		<asp:TextBox id="txtclaimid" runat="server" Visible="False" meta:resourcekey="txtclaimidResource1"></asp:TextBox>
	</div>
    
   
    </asp:Content>

