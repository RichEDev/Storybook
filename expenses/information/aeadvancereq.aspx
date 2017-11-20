<%@ Page language="c#" Inherits="expenses.information.aeadvancereq" MasterPageFile="~/expform.master" Codebehind="aeadvancereq.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Advance Name:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtname" runat="server" meta:resourcekey="txtnameResource1" MaxLength="50"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqname" runat="server" ErrorMessage="Please enter a name for this advance"
						ControlToValidate="txtname" meta:resourcekey="reqnameResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" height="20">
					<asp:Label id="lblamount" runat="server" meta:resourcekey="lblamountResource1">Amount:</asp:Label></td>
				<td class="inputtd" height="20">
					<asp:TextBox id="txtamount" runat="server" Width="50px" meta:resourcekey="txtamountResource1"></asp:TextBox>
					<asp:DropDownList id="cmbcurrencies" runat="server" meta:resourcekey="cmbcurrenciesResource1"></asp:DropDownList></td>
				<td height="20">
					<asp:RequiredFieldValidator id="reqadvance" runat="server" ErrorMessage="Please enter the amount required for this float"
						ControlToValidate="txtamount" meta:resourcekey="reqadvanceResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compamount" runat="server" ErrorMessage="Please enter a valid amount" ControlToValidate="txtamount"
						Type="Currency" Operator="DataTypeCheck" meta:resourcekey="compamountResource1">*</asp:CompareValidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top">
					<asp:Label id="lblreason" runat="server" meta:resourcekey="lblreasonResource1">Reason for Request:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtreason" runat="server" TextMode="MultiLine" meta:resourcekey="txtreasonResource1" MaxLength="4000"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblrequiredby" runat="server" meta:resourcekey="lblrequiredbyResource1">Required By:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="dtrequiredby" runat="server" meta:resourcekey="dtrequiredbyResource1"></asp:TextBox>&nbsp;<asp:Image ID="imgrequiredby" ImageUrl="../icons/cal.gif" runat="server" meta:resourcekey="imgrequiredbyResource1" /><cc1:MaskedEditExtender ID="mskrequiredby" MaskType="Date" Mask="99/99/9999" TargetControlID="dtrequiredby" CultureName="en-GB"
                        runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True">
                    </cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calrequiredby" runat="server" TargetControlID="dtrequiredby" PopupButtonID="imgrequiredby" Format="dd/MM/yyyy" Enabled="True">
                    </cc1:CalendarExtender></td>
				<td>
					<asp:CompareValidator id="comprequiredby" runat="server" ControlToValidate="dtrequiredby" ErrorMessage="A required by date cannot be in the past"
						Operator="GreaterThanEqual" Type="Date" meta:resourcekey="comprequiredbyResource1">*</asp:CompareValidator></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

  </asp:Content>

