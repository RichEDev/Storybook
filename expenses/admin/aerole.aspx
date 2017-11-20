<%@ Page language="c#" Inherits="expenses.aerole" MasterPageFile="~/expform.master" Codebehind="aerole.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content
        ID="Content2" runat="server" ContentPlaceHolderID="contentmain">


	<div class="valdiv">
		<asp:validationsummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:validationsummary>
		<asp:label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">lblmsg</asp:label>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Role Name:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtrolename" runat="server" MaxLength="50" meta:resourcekey="txtrolenameResource1"></asp:textbox></td>
				<td><asp:requiredfieldvalidator id="reqrolename" runat="server" ControlToValidate="txtrolename" ErrorMessage="Please enter a Role Name" meta:resourcekey="reqrolenameResource1">*</asp:requiredfieldvalidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top">
					<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:textbox></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblmasterrole" runat="server" meta:resourcekey="lblmasterroleResource1">Master Role:</asp:Label></td>
				<td class="inputtd"><asp:dropdownlist id="cmbmasters" runat="server" meta:resourcekey="cmbmastersResource1"></asp:dropdownlist></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Role Permissions</asp:Label></div>
		<asp:linkbutton id="cmdrole" runat="server" onclick="cmdrole_Click" meta:resourcekey="cmdroleResource1">Select All</asp:linkbutton>&nbsp;&nbsp;
		<asp:linkbutton id="cmdroledesel" runat="server" onclick="cmdroledesel_Click" meta:resourcekey="cmdroledeselResource1">De-select All</asp:linkbutton>
		<table>
			<tr>
				<th colspan="2">
					This role is allowed to</th></tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblsetup" runat="server" meta:resourcekey="lblsetupResource1">Modify the set-up of categories and system options:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chksetup" runat="server" meta:resourcekey="chksetupResource1"></asp:checkbox></td><td><img id="imgtooltip121" onclick="showTooltip(event, 'imgtooltip121','a6d982e9-80c5-48bc-9b34-6dadf644c11b');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblusersetup" runat="server" meta:resourcekey="lblusersetupResource1">Modify the set-up of employees, sign-off groups and roles:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkusersetup" runat="server" meta:resourcekey="chkusersetupResource1"></asp:checkbox></td><td><img id="imgtooltip122" onclick="showTooltip(event, 'imgtooltip122','35bcff96-4b05-4e99-a89b-5a228187548e');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblemplogon" runat="server" meta:resourcekey="lblemplogonResource1">Logon to other employee accounts:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkemplogon" runat="server" meta:resourcekey="chkemplogonResource1"></asp:checkbox></td><td><img id="imgtooltip123" onclick="showTooltip(event, 'imgtooltip123','3508e70a-802b-49d9-9c55-33fd25cca0f3');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblreports" runat="server" meta:resourcekey="lblreportsResource1">View administrator reports:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkreports" runat="server" meta:resourcekey="chkreportsResource1"></asp:checkbox></td><td><img id="imgtooltip124" onclick="showTooltip(event, 'imgtooltip124','f6ec8f83-7b7d-48a3-bc77-684360b1a30e');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblreportsreadonly" runat="server" meta:resourcekey="lblreportsreadonlyResource1">View read only reports:</asp:Label></td>
				<td class="inputtd">
					<asp:CheckBox id="chkreportsreadonly" runat="server" meta:resourcekey="chkreportsreadonlyResource1"></asp:CheckBox></td><td><img id="imgtooltip361" onclick="showTooltip(event, 'imgtooltip361','7750b5a2-e023-4502-be46-1ccc9e37c1d0');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcheckpay" runat="server" meta:resourcekey="lblcheckpayResource1">Check and pay expenses:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkcheckpay" runat="server" meta:resourcekey="chkcheckpayResource1"></asp:checkbox></td><td><img id="imgtooltip125" onclick="showTooltip(event, 'imgtooltip125','813e3571-f054-485e-8058-375ee16edd00');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblqedesign" runat="server" meta:resourcekey="lblqedesignResource1">Quick Edit Design:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkqedesign" runat="server" meta:resourcekey="chkqedesignResource1"></asp:checkbox></td><td><img id="imgtooltip126" onclick="showTooltip(event, 'imgtooltip126','c05d3d9a-d1bd-40c6-8296-d7c66d9650c3');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcreditcard" runat="server" meta:resourcekey="lblcreditcardResource1">Import Corporate Card statements:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkcreditcard" runat="server" meta:resourcekey="chkcreditcardResource1"></asp:checkbox></td><td><img id="imgtooltip127" onclick="showTooltip(event, 'imgtooltip127','6b68ed3c-be03-4b9c-ad70-0acebd998c6a');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			
			<tr>
				<td class="labeltd">
					<asp:Label id="lblapproval" runat="server" meta:resourcekey="lblapprovalResource1">Manage and approve advance requests:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkapproval" runat="server" meta:resourcekey="chkapprovalResource1"></asp:checkbox></td><td><img id="imgtooltip130" onclick="showTooltip(event, 'imgtooltip130','4dd78a33-3641-4f37-953d-283e5b55fe61');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblexports" runat="server" meta:resourcekey="lblexportsResource1">Export data and view export history:</asp:Label></td>
				<td class="inputtd"><asp:checkbox id="chkexports" runat="server" meta:resourcekey="chkexportsResource1"></asp:checkbox></td><td><img id="imgtooltip131" onclick="showTooltip(event, 'imgtooltip131','85d7e9fb-f4f2-423d-9870-1b18c97e57bc');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr><td class="labeltd">
                <asp:Label ID="lblauditlog" runat="server" Text="Search and view the Audit Log" meta:resourcekey="lblauditlogResource1"></asp:Label>:</td><td class="inputtd">
                <asp:CheckBox ID="chkauditlog" runat="server" meta:resourcekey="chkauditlogResource1" /></td><td><img id="imgtooltip362" onclick="showTooltip(event, 'imgtooltip362','0a64fd6a-3d63-4219-ac9b-9b9c96fb19a2');" src="../icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td></tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Access Permissions</asp:Label></div>
		<table>
			<tr>
				<td><asp:radiobutton id="optall" runat="server" Width="184px" GroupName="access" Checked="True" Text="This role can access all data" meta:resourcekey="optallResource1"></asp:radiobutton></td>
				<td><asp:RadioButton id="optgroups" runat="server" Text="This role can access data for any employees it is responsible for signing off"
						GroupName="access" meta:resourcekey="optgroupsResource1"></asp:RadioButton></td>
				<td><asp:radiobutton id="optselected" runat="server" Width="374px" GroupName="access" Text="This role can access data belonging to the roles selected below" meta:resourcekey="optselectedResource1"></asp:radiobutton></td>
			</tr>
		</table>
		<asp:checkboxlist id="chkroles" runat="server" RepeatDirection="Horizontal" RepeatColumns="3" meta:resourcekey="chkrolesResource1"></asp:checkboxlist>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label4" runat="server" meta:resourcekey="Label4Resource1">Claim Limits</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblminclaim" runat="server" meta:resourcekey="lblminclaimResource1">Claimants can not submit a claim if the grand total is below:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtminclaim" runat="server" meta:resourcekey="txtminclaimResource1"></asp:TextBox></td>
				<td>
					<asp:CompareValidator id="compminclaim" runat="server" Type="Currency" Operator="DataTypeCheck" ControlToValidate="txtminclaim"
						ErrorMessage="Please enter a valid value for the minimum claim total" meta:resourcekey="compminclaimResource1">*</asp:CompareValidator></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblmaxclaim" runat="server" meta:resourcekey="lblmaxclaimResource1">Claimants can not submit a claim if the grand total is above:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtmaxclaim" runat="server" meta:resourcekey="txtmaxclaimResource1"></asp:TextBox></td>
				<td>
					<asp:CompareValidator id="compmaxclaim" runat="server" Type="Currency" Operator="DataTypeCheck" ControlToValidate="txtmaxclaim"
						ErrorMessage="Please enter a valid value for the maximum claim total" meta:resourcekey="compmaxclaimResource1">*</asp:CompareValidator></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label5" runat="server" meta:resourcekey="Label5Resource1">Claimant Code Allocation</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcostcodesreadonly" runat="server" meta:resourcekey="lblcostcodesreadonlyResource1">Claimants cannot amend their designated cost code allocation:</asp:Label></td>
				<td class="inputtd">
					<asp:CheckBox id="chkcostcodesreadonly" runat="server" meta:resourcekey="chkcostcodesreadonlyResource1"></asp:CheckBox></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lbldepartmentsreadonly" runat="server" meta:resourcekey="lbldepartmentsreadonlyResource1">Claimants cannot amend their designated department allocation:</asp:Label></td>
				<td class="inputtd">
					<asp:CheckBox id="chkdepartmentsreadonly" runat="server" meta:resourcekey="chkdepartmentsreadonlyResource1"></asp:CheckBox></td>
			</tr>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblprojectcodesreadonly" runat="server" meta:resourcekey="lblprojectcodesreadonlyResource1">Claimants cannot amend their designated project code allocation:</asp:Label></td>
				<td class="inputtd">
					<asp:CheckBox id="chkprojectcodesreadonly" runat="server" meta:resourcekey="chkprojectcodesreadonlyResource1"></asp:CheckBox></td>
			</tr>
		</table>
	</div>
	
	<div class="inputpanel">
		<asp:imagebutton id="cmdok" runat="server" ImageUrl="../buttons/ok_up.gif" meta:resourcekey="cmdokResource1"></asp:imagebutton>&nbsp;&nbsp;
		<asp:imagebutton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:imagebutton></div>
	    <asp:textbox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:textbox><asp:textbox id="txtroleid" runat="server" Visible="False" meta:resourcekey="txtroleidResource1"></asp:textbox>
        </asp:Content>
