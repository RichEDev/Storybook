<%@ Page language="c#" Inherits="expenses.information.aeholiday" MasterPageFile="~/expform.master" Codebehind="aeholiday.aspx.cs" %>

<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content
        ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle table-border">
            <asp:Label ID="lblholidaydates" runat="server" Text="Holiday Dates" meta:resourcekey="lblholidaydatesResource1"></asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblstartdate" runat="server" meta:resourcekey="lblstartdateResource1">Start Date:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="dtstart" runat="server" meta:resourcekey="dtstartResource1"></asp:TextBox>&nbsp; &nbsp; &nbsp;<asp:Image ID="imgstart" ImageUrl="/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgstartResource1" /><cc1:MaskedEditExtender ID="mskstartdate" MaskType="Date" Mask="99/99/9999" TargetControlID="dtstart"
                        runat="server" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True">
                    </cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calstart" runat="server" TargetControlID="dtstart" PopupButtonID="imgstart" Format="dd/MM/yyyy" Enabled="True">
                    </cc1:CalendarExtender>
                </td>
				<td>
					<asp:RequiredFieldValidator id="reqstart" runat="server" ErrorMessage="Please enter the start date of the holiday"
						ControlToValidate="dtstart" meta:resourcekey="reqstartResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compstart" runat="server" ErrorMessage="Please enter a valid Start Date" ControlToValidate="dtstart"
						Operator="DataTypeCheck" Type="Date" meta:resourcekey="compstartResource1">*</asp:CompareValidator>
						<asp:CompareValidator runat="server" ID="cmpminstart" ControlToValidate="dtstart" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Start Date must be greater than or equal to 01/01/1900"></asp:CompareValidator>
						<asp:CompareValidator runat="server" ID="cmpmaxstart" ControlToValidate="dtstart" ValueToCompare="31/12/3000" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="Start Date must be less than or equal to 31/12/3000"></asp:CompareValidator>
						</td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblenddate" runat="server" meta:resourcekey="lblenddateResource1">End Date:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="dtend" runat="server" meta:resourcekey="dtendResource1"></asp:TextBox>&nbsp; &nbsp; &nbsp;<asp:Image ID="imgend" ImageUrl="/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgendResource1" /><cc1:MaskedEditExtender ID="mskenddate" MaskType="Date" Mask="99/99/9999" TargetControlID="dtend"
                        runat="server" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True">
                    </cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calenddate" runat="server" TargetControlID="dtend" PopupButtonID="imgend" Format="dd/MM/yyyy" Enabled="True">
                    </cc1:CalendarExtender></td>
				<td>
					<asp:RequiredFieldValidator id="reqend" runat="server" ErrorMessage="Please enter the end date of the holiday"
						ControlToValidate="dtend" meta:resourcekey="reqendResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compend" runat="server" ErrorMessage="Please enter a valid end date" ControlToValidate="dtend"
						Operator="DataTypeCheck" Type="Date" meta:resourcekey="compendResource1">*</asp:CompareValidator><asp:CompareValidator
                            ID="compendafter" runat="server" ErrorMessage="Please enter an end date that is after the start date" ControlToValidate="dtend" ControlToCompare="dtstart" Operator="GreaterThanEqual" Type="Date" Text="*"></asp:CompareValidator>
                    <asp:CompareValidator runat="server" ID="cmpminend" ControlToValidate="dtend" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="End Date must be greater than or equal to 01/01/1900"></asp:CompareValidator>
                    <asp:CompareValidator runat="server" ID="cmpmaxend" ControlToValidate="dtend" ValueToCompare="31/12/3000" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="End Date must be less than or equal to 31/12/3000"></asp:CompareValidator>
                            </td>
			</tr>
		</table>
	</div>
	<div class="inputpanel table-border2">
		<div class="inputpaneltitle">
            <asp:Label ID="lblsignoffdetails" runat="server" Text="While you are on holiday, your claims will be approved &#13;&#10;&#9;&#9;&#9;by the following:" meta:resourcekey="lblsignoffdetailsResource1"></asp:Label></div>
		<asp:Literal id="litholiday" runat="server" meta:resourcekey="litholidayResource1"></asp:Literal>
	</div>
	<div class="inputpanel">
		<asp:imagebutton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:imagebutton>&nbsp;&nbsp;
		<asp:imagebutton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:imagebutton></div>
	<asp:textbox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:textbox><asp:textbox id="txtholidayid" runat="server" Visible="False" meta:resourcekey="txtholidayidResource1"></asp:textbox>
        </asp:Content>

