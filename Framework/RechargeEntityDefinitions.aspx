<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.RechargeEntityDefinitions" CodeFile="RechargeEntityDefinitions.aspx.vb" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<asp:Label id="lblError" runat="server" ForeColor="Red"></asp:Label>
</div>
<div class="inputpanel">
<div class="inputpaneltitle"><asp:label id="lblTitle" runat="server">title</asp:label></div>
		<table>
				<tr>
					<td class="labeltd">
						<asp:label id="lblCustomer" runat="server">Customer</asp:label></td>
					<td class="inputtd"><asp:textbox id="txtCustomer" runat="server" MaxLength="50" tabIndex="1"></asp:textbox></td>
					<td><asp:RequiredFieldValidator id="reqName" runat="server" ErrorMessage="Required field (Entity Definition) not specified."
							ControlToValidate="txtCustomer">*</asp:RequiredFieldValidator>
                        <cc1:ValidatorCalloutExtender ID="reqexName" runat="server" TargetControlID="reqName">
                        </cc1:ValidatorCalloutExtender>
                    </td>
					<td class="labeltd"><asp:label id="lblShared" runat="server">Shared</asp:label></td>
					<td class="inputtd"><asp:checkbox id="chkShared" runat="server" tabIndex="2"></asp:checkbox></td>
					<td></td>
				</tr>
				<tr>
					<td class="labeltd">
						<asp:Label id="lblCode" runat="server">Code</asp:Label></td>
					<td class="inputtd">
						<asp:TextBox id="txtCode" runat="server" MaxLength="20" tabIndex="3"></asp:TextBox></td>
						<td></td>
					<td class="labeltd">
						<asp:Label id="lblSector" runat="server">Sector</asp:Label></td>
					<td class="inputtd">
						<asp:TextBox id="txtSector" runat="server" MaxLength="50" tabIndex="4"></asp:TextBox></td>
						<td></td>
				</tr>
				<tr>
					<td class="labeltd"><asp:label id="lblRepresentative" runat="server">Representative</asp:label></td>
					<td class="inputtd"><asp:dropdownlist id="lstRepresentative" runat="server" tabIndex="5"></asp:dropdownlist></td>
					<td></td>
					<td class="labeltd"><asp:label id="lblDeputyRep" runat="server">Deputy Rep.</asp:label></td>
					<td class="inputtd"><asp:dropdownlist id="lstDeputyRep" runat="server" tabIndex="6"></asp:dropdownlist></td>
					<td></td>
				</tr>
				<tr>
					<td class="labeltd"><asp:label id="lblAccountMgr" runat="server">Account Manager</asp:label></td>
					<td class="inputtd"><asp:dropdownlist id="lstAccountMgr" runat="server" tabIndex="7"></asp:dropdownlist></td>
					<td></td>
					<td class="labeltd"><asp:label id="lblServiceMgr" runat="server" >Service Manager</asp:label></td>
					<td class="inputtd"><asp:dropdownlist id="lstServiceMgr" runat="server" tabIndex="8"></asp:dropdownlist></td>
					<td></td>
				</tr>
				<tr>
					<td class="labeltd">
						<asp:Label id="lblServiceLine" runat="server">Service Line</asp:Label></td>
					<td class="inputtd">
						<asp:TextBox id="txtServiceLine" tabIndex="9" runat="server" MaxLength="20"></asp:TextBox></td>
					<td></td>
					<td class="labeltd">Closed</td>
					<td class="inputtd"><asp:checkbox id="chkClosed" runat="server"
							tabIndex="10"></asp:checkbox></td>
							<td></td>
				</tr>
				<tr>
					<td class="labeltd"><asp:label id="lblNotes" runat="server">Notes</asp:label></td>
					<td class="inputtd" colSpan="3"><asp:textbox id="txtNotes" runat="server" Width="300px" TextMode="MultiLine"
							MaxLength="250" tabIndex="11"></asp:textbox></td>
							<td colspan="2"></td>
				</tr>
				<tr>
					<td class="labeltd"><asp:label id="lblDateClosed" runat="server">Date Closed</asp:label></td>
					<td class="inputtd"><igtxt:webdatetimeedit id="meDateClosed" runat="server" tabIndex="12"></igtxt:webdatetimeedit></td>
					<td>
                        <cc1:ValidatorCalloutExtender ID="cmpexClosedDate" runat="server" TargetControlID="cmpClosed">
                        </cc1:ValidatorCalloutExtender>
                        <asp:CompareValidator ID="cmpClosed" runat="server" ControlToValidate="meDateClosed"
                            ErrorMessage="Closed Date entered is invalid" Operator="DataTypeCheck" Type="Date">*</asp:CompareValidator></td>
					<td class="labeltd"><asp:label id="lblCeaseDate" runat="server">Cease Date</asp:label></td>
					<td class="inputtd"><igtxt:webdatetimeedit id="meDateCeased" runat="server" tabIndex="13"></igtxt:webdatetimeedit></td>
					<td>
                        <cc1:ValidatorCalloutExtender ID="cmpexCeasedDate" runat="server" TargetControlID="cmpCeasedDate">
                        </cc1:ValidatorCalloutExtender>
                        <asp:CompareValidator ID="cmpCeasedDate" runat="server" ControlToValidate="meDateCeased"
                            ErrorMessage="The Ceased date entered is invalid" Operator="DataTypeCheck" Type="Date">*</asp:CompareValidator></td>
				</tr>
			</table>
</div>
<div class="inputpanel"><asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />&nbsp;<asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="False" /></div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
<a href="./help_text/default_csh.htm#1116" target="_blank" class="submenuitem">Help</a>
                
</asp:Content>

