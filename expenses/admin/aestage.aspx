<%@ Page language="c#" Inherits="expenses.aestage" MasterPageFile="~/expform.master" Codebehind="aestage.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content runat="server" ContentPlaceHolderID="head" ID="customStyles">
    <style type="text/css">
            td input[type="checkbox"] {
                margin-top:-10px;       
          }
            #imgtooltip368, #imgtooltip369, #imgtooltip3232{
                margin-top:-12px;
            }
          #imgtooltip366 {
              margin-right: 50px;
          } 
          .labeltd {
              width: 29%;
              padding-bottom: 12px;
          }
    </style>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.signoff.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
        <div runat="server" id="divIsPostValidationStageComment" Visible="false" class="inputpanel comment" style="margin-bottom: 10px;">
            <p>
                This stage is the <b>post-validation verification stage</b>, which in most circumstances will be skipped.<br />
                For expense items that have their values edited after payment by more than the allowed percentage threshold, this stage will be used to ensure validity.
            </p>
        </div>

		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label>
		</div>
		<table>
			<tr>
				<td class="labeltd"><asp:Label id="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1">Stage Type</asp:Label></td>
				<td class="inputtd">
					<asp:DropDownList id="cmbsignofftype" runat="server" AutoPostBack="True" onselectedindexchanged="cmbsignofftype_SelectedIndexChanged" meta:resourcekey="cmbsignofftypeResource1">
						<asp:ListItem Value="1" meta:resourcekey="ListItemResource1">Budget Holder Responsible</asp:ListItem>
						<asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Employee</asp:ListItem>
						<asp:ListItem Value="3" meta:resourcekey="ListItemResource3">Team</asp:ListItem>
						<asp:ListItem Value="4" meta:resourcekey="ListItemResource4">Line Manager</asp:ListItem>
                        <asp:ListItem Value="8" meta:resourcekey="ListItemResource21">Cost Code Owner</asp:ListItem>
					</asp:DropDownList>
				</td>
                <td><img id="imgtooltip366" onclick="SEL.Tooltip.Show('ac662c39-a0f1-4395-baa7-4a4d3f7b9cac', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
				<td>
					<asp:DropDownList id="cmblist" runat="server" meta:resourcekey="cmblistResource1"></asp:DropDownList>
					<asp:RequiredFieldValidator id="reqsignoff" runat="server" ControlToValidate="cmblist" ErrorMessage="Please select a Signoff Person for this stage" meta:resourcekey="reqsignoffResource1">*</asp:RequiredFieldValidator>
					<asp:RangeValidator runat="server" ControlToValidate="cmbsignofftype" Type="Integer" MinimumValue="1" MaximumValue="2147483647" Text="Please select a Signoff person for this stage"></asp:RangeValidator>
				</td>
			</tr>
			<tr id="trExtraLevels" runat="server" Visible="False">
				<td class="labeltd"><asp:Label id="lblExtraLevels" runat="server" meta:resourcekey="lblExtraLevelsResource1" CssClass="mandatory">Include additional approval matrix levels*</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtExtraLevels" runat="server" style="margin-left: 2px;"></asp:TextBox>
                    <cc1:FilteredTextBoxExtender runat="server" ID="fteExtraLevels" FilterMode="ValidChars" ValidChars="0123456789" TargetControlID="txtExtraLevels" />
					<asp:RequiredFieldValidator id="rfvExtraLevels" runat="server" Enabled="False" ControlToValidate="txtExtraLevels" ErrorMessage="Please enter a number of approval matrix levels above the actual claim amount level that approvers may be chosen from by the claimant.">*</asp:RequiredFieldValidator>
				</td>
                <td><img id="imgtooltip589" onclick="SEL.Tooltip.Show('6de3682d-0d5a-4d25-a4e0-6d2942c1f770', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
            <tr id="trFromMyLevel" runat="server" Visible="False">
				<td class="labeltd">
				    <asp:Label id="lblFromMyLevel" runat="server" meta:resourcekey="lblFromMyLevelResource1" AssociatedControlID="chkFromMyLevel">Only allow approvers above my level</asp:Label>
				</td>
				<td class="inputtd">
					<asp:CheckBox id="chkFromMyLevel" runat="server" style="margin-left: 2px;"></asp:CheckBox>
				</td>
                <td></td>
			</tr>
			<tr>
				<td class="labeltd"><asp:Label id="lblinclude" runat="server" meta:resourcekey="lblincludeResource1">When to Include</asp:Label></td>
				<td class="inputtd">
					<asp:DropDownList id="cmbinclude" runat="server" AutoPostBack="True" onselectedindexchanged="cmbinclude_SelectedIndexChanged" meta:resourcekey="cmbincludeResource1">
						<asp:ListItem Value="1" meta:resourcekey="ListItemResource5">Always</asp:ListItem>
						<asp:ListItem Value="2" meta:resourcekey="ListItemResource6">Only if claim total exceeds</asp:ListItem>
						<asp:ListItem Value="5" meta:resourcekey="ListItemResource7">Only if claim total is below</asp:ListItem>
						<asp:ListItem Value="3" meta:resourcekey="ListItemResource8">Only if an expense item exceeds allowed amount</asp:ListItem>
						<asp:ListItem Value="9" >Only if an expense item fails validation twice</asp:ListItem>
						<asp:ListItem Value="4" meta:resourcekey="ListItemResource9">Claim includes the following cost code</asp:ListItem>
						<asp:ListItem Value="6" meta:resourcekey="ListItemResource10">Claim includes the following expense item</asp:ListItem>
						<asp:ListItem Value="8" meta:resourcekey="resListItemClaimIncludesDepartment">Claim includes the following department</asp:ListItem>
                        <asp:ListItem Value="7" meta:resourcekey="ListItemResource11">An expense item is older than a given number of days</asp:ListItem>
					</asp:DropDownList>
				</td>
                <td><img id="imgtooltip367" onclick="SEL.Tooltip.Show('ef816c1c-9e8e-427b-a8b9-7701c801b97b', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr>
				<td></td>
				<td class="inputtd"><asp:TextBox id="txtamount" runat="server" Visible="False" Width="50px" meta:resourcekey="txtamountResource1"></asp:TextBox><asp:DropDownList id="cmbincludelst" runat="server" Visible="False" meta:resourcekey="cmbincludelstResource1"></asp:DropDownList></td>
				<td>
					<asp:RequiredFieldValidator id="reqamount" runat="server" ErrorMessage="Please enter an amount" ControlToValidate="txtamount" meta:resourcekey="reqamountResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compamount" runat="server" ErrorMessage="Please enter a valid amount" ControlToValidate="txtamount" Operator="DataTypeCheck" Type="Currency" meta:resourcekey="compamountResource1">*</asp:CompareValidator>
				</td>
			</tr>
			<tr>
				<td class="labeltd"><asp:Label id="lblinvolvement" runat="server" meta:resourcekey="lblinvolvementResource1">Involvement</asp:Label></td>
				<td class="inputtd">
					<asp:DropDownList id="cmbinvolvement" runat="server" meta:resourcekey="cmbinvolvementResource1">
						<asp:ListItem Value="1" meta:resourcekey="ListItemResource12">Just notify user of claim</asp:ListItem>
						<asp:ListItem Value="2" meta:resourcekey="ListItemResource13">User is to check claim</asp:ListItem>
					</asp:DropDownList>
				</td>
			</tr>
            <tr id="rowLineManagerAssignmentSupervisor" clientidmode="Static" runat="server">
				<td class="labeltd"><asp:Label id="LabelLineManagerAssignmentSupervisor" runat="server" AssociatedControlID="DropDownLineManagerAssignmentSupervisor" CssClass="mandatory">Action if no cost code owner and no default cost code owner*</asp:Label></td>
				<td class="inputtd">
					<asp:DropDownList id="DropDownLineManagerAssignmentSupervisor" runat="server">
					    <asp:ListItem Value="">[None]</asp:ListItem>
						<asp:ListItem Value="LineManager">Assign claim to Line Manager</asp:ListItem>
						<asp:ListItem Value="AssignmentSupervisor">Assign claim to Assignment Supervisor</asp:ListItem>
					</asp:DropDownList>
				</td>
                <td>
                    <asp:RequiredFieldValidator id="ReqLineManagerAssignmentSupervisor" runat="server" ErrorMessage="Please select an Action if no cost code owner and no default cost code owner." ControlToValidate="DropDownLineManagerAssignmentSupervisor" Enabled="False">*</asp:RequiredFieldValidator>
                </td>
			</tr>
			<tr>
				<td class="labeltd" height="27"><asp:Label id="lblholiday" runat="server" meta:resourcekey="lblholidayResource1">If user is on holiday</asp:Label></td>
				<td class="inputtd" height="27">
					<asp:DropDownList id="cmbonholiday" runat="server" AutoPostBack="True" onselectedindexchanged="cmbonholiday_SelectedIndexChanged" meta:resourcekey="cmbonholidayResource1">
						<asp:ListItem Value="1" meta:resourcekey="ListItemResource14">Take no action</asp:ListItem>
						<asp:ListItem Value="2" meta:resourcekey="ListItemResource15" id="cmbonholidaySkipStage">Skip stage</asp:ListItem>
						<asp:ListItem Value="3" meta:resourcekey="ListItemResource16">Assign claim to someone else</asp:ListItem>
					</asp:DropDownList>
				</td>
			</tr>
			<tr>
				<td></td>
				<td>
					<asp:DropDownList id="cmbholidaytype" runat="server" Visible="False" AutoPostBack="True" onselectedindexchanged="cmbholidaytype_SelectedIndexChanged" meta:resourcekey="cmbholidaytypeResource1">
						<asp:ListItem Value="1" meta:resourcekey="ListItemResource17">Budget Holder Responsible</asp:ListItem>
						<asp:ListItem Value="2" meta:resourcekey="ListItemResource18">Employee</asp:ListItem>
						<asp:ListItem Value="3" meta:resourcekey="ListItemResource19">Team</asp:ListItem>
						<asp:ListItem Value="4" meta:resourcekey="ListItemResource20">Line Manager</asp:ListItem>
					</asp:DropDownList>
				</td>
				<td>
					<asp:DropDownList id="cmbholidaylist" runat="server" Visible="False" meta:resourcekey="cmbholidaylistResource1"></asp:DropDownList>
					<asp:RequiredFieldValidator id="reqholidaylst" runat="server" ErrorMessage="Please select a user responsible whilst the signoff person is on holiday" Enabled="False" ControlToValidate="cmbholidaylist" meta:resourcekey="reqholidaylstResource1">*</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
			    <td class="labeltd"><asp:Label ID="lblsendemailapprover" runat="server" Text="Send an e-mail notification to the approver when the claim reaches this stage" meta:resourcekey="lblsendemailapproverResource1"></asp:Label></td>
                <td class="inputtd"><asp:CheckBox ID="chksendmail" runat="server" meta:resourcekey="chksendmailResource1" /></td>
			</tr>
            <tr runat="server" id="ContainerNotifyWhenEnvelopeReceived">
				<td class="labeltd"><asp:Label ID="lblNotifyWhenEnvelopeReceived" runat="server" AssociatedControlID="chkNotifyWhenEnvelopeReceived">Notify claimant when Envelopes are received for Scan & Attach</asp:Label></td>
				<td class="inputtd"><asp:CheckBox ID="chkNotifyWhenEnvelopeReceived" runat="server" ClientIDMode="Static"></asp:CheckBox></td>
                <td><img id="imgtooltip3230" onclick="SEL.Tooltip.Show('F7E5A916-241A-4FBF-A3C2-501010C226D5', 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></td>
			</tr>   
            <tr runat="server" id="ContainerNotifyWhenEnvelopeNotReceived">
				<td class="labeltd"><asp:Label ID="lblNotifyWhenEnvelopeNotReceived" runat="server" AssociatedControlID="chkNotifyWhenEnvelopeNotReceived">Notify claimant when Envelopes are <strong>not</strong> received for Scan & Attach</asp:Label></td>
				<td class="inputtd"><asp:CheckBox ID="chkNotifyWhenEnvelopeNotReceived" runat="server" ClientIDMode="Static"></asp:CheckBox></td>
                <td><img id="imgtooltip3231" onclick="SEL.Tooltip.Show('C7BAAC76-BB65-4A6F-B0EF-196A66F966E8', 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></td>
			</tr>                                 
			<tr>
				<td class="labeltd"><asp:Label id="lblclaimantmail" runat="server" meta:resourcekey="lblclaimantmailResource1">Send an e-mail notification to the claimant when the claim reaches this stage</asp:Label></td>
				<td class="inputtd"><asp:CheckBox id="chkclaimantmail" runat="server" meta:resourcekey="chkclaimantmailResource1"></asp:CheckBox></td>
			</tr>
			<tr>
			    <td class="labeltd">
                    <asp:Label ID="lbloneclick" runat="server" Text='Enable "One Click" Signoff' meta:resourcekey="lbloneclickResource1"></asp:Label></td><td class="inputtd">
                    <asp:CheckBox ID="chksinglesignoff" runat="server" meta:resourcekey="chksinglesignoffResource1" AutoPostBack="True" OnCheckedChanged="chksinglesignoff_CheckedChanged" />
                </td>
                <td><img id="imgtooltip369" onclick="SEL.Tooltip.Show('8b496e8f-e22f-4dfa-8e98-bb0bbe0d51d0', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
            <tr>
                <td class="labeltd">Approvers at this stage must sign an electronic declaration before they can approve a claim</td>
                <td class="inputtd"><asp:CheckBox ID="chkdisplaydeclaration" runat="server" /></td>
                <td><img id="imgtooltip368" onclick="SEL.Tooltip.Show('94228db9-89ed-445b-9dad-b3b47c07c373', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
            </tr>
            <tr>
                <td class="labeltd">Approver justifications required</td>
                <td class="inputtd"><asp:CheckBox ID="chkApproverJustificationsRequired" runat="server" AutoPostBack="True" OnCheckedChanged="chkApproverJustificationsRequired_CheckedChanged" /></td>
                <td><img id="imgtooltip368" onclick="SEL.Tooltip.Show('6889C27E-CD62-4695-8FE8-BC39E2DEC3F4', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
            </tr>
        </table>
    </div>

    <asp:HiddenField runat="server" ID="hdnAllowPayBeforeValidate" />
    <div id="SectionPayBeforeValidate" runat="server" class="inputpanel" visible="False">
        <div class="inputpaneltitle">
            <asp:Label ID="lblPayBeforeValidateHeading" runat="server">Pay Before Validate</asp:Label>
        </div>
        <div class="inputpanel comment">
            <p>
                When Pay Before Validate (PBV) is enabled, a claimants' expenses can be allocated for payment at an earlier stage within the Signoff Group, and then corrected later following any validation failures.<br />
                In order for this to be achieved, a final 'Verify' stage must be included at the end of the Signoff Group. In most circumstances this stage will be skipped.<br />
                For expense claims which fail validation twice, or are corrected following validation, an approver should be defined who can check the expense to manually ensure its validity.<br />
                Expenses that fall into this category require extra work by the system to allocate the modified amounts for payment, so there are rules that apply to this group when PBV is enabled:
            </p>
            <ul>
                <li>There can only be one PBV stage in any signoff group.</li>
                <li>The PBV stage cannot be the first stage.</li>
                <li>The Validate stage must be the penultimate stage.</li>
                <li>The 'Verify' stage must be the final stage.</li>
            </ul>
            <p>
                Saving the Signoff Group will not be possible without the rules above being followed.
            </p>
        </div>
        
        <table>
            <tr runat="server" id="RowAllowPayBeforeValidate">
                <td class="labeltd">
                    <asp:Label ID="lblPayBeforeValidate" runat="server" AssociatedControlID="chkPayBeforeValidate">Allow "Pay Before Validate" at this stage</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:CheckBox ID="chkPayBeforeValidate" runat="server" AutoPostBack="True"></asp:CheckBox>
                </td>
                <td>
                    <img id="imgtooltip3232" onclick="SEL.Tooltip.Show(SEL.Signoff.HELP.PAY_BEFORE_VALIDATE, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                </td>
            </tr>
            <tr ID="trThresholdPayBeforeValidate" runat="server" Visible="False">
                <td class="labeltd">
                    <asp:Label runat="server" AssociatedControlID="txtThreshold">Correction verification threshold percentage</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox runat="server" ID="txtThreshold" style="margin-left: 2px;"  />
                    <cc1:FilteredTextBoxExtender runat="server" ID="txtThreshholdExtender1" FilterMode="ValidChars" ValidChars="0123456789" TargetControlID="txtThreshold" />
                    <asp:RangeValidator runat="server" ID="rangeThreshold" ControlToValidate="txtThreshold" ErrorMessage="Please enter a Correction verification threshold percentage between 0 and 100." MinimumValue="0" MaximumValue="100" Type="Integer">*</asp:RangeValidator>
                    <asp:RequiredFieldValidator runat="server" ID="reqThreshold" ControlToValidate="txtThreshold" ErrorMessage="Please enter a Correction verification threshold percentage between 0 and 100.">*</asp:RequiredFieldValidator>
                </td>
                <td>
                    <img id="imgtooltip3233" onclick="SEL.Tooltip.Show(SEL.Signoff.HELP.THRESHOLD, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                </td>
            </tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>

	<asp:TextBox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:TextBox>
	<asp:TextBox id="txtsignoffid" runat="server" Visible="False" meta:resourcekey="txtsignoffidResource1"></asp:TextBox>
	<asp:TextBox id="txtgroupid" runat="server" Visible="False" meta:resourcekey="txtgroupidResource1"></asp:TextBox>
    
    <script type="text/javascript">
        SEL.Signoff.CONTENT_MAIN = '<%=this.cmdcancel.Parent.ClientID%>_';
        if (<asp:Literal runat="server" ID="litScrollToBottom" EnableViewState="false">false</asp:Literal>)
        {
            setTimeout("$(window).scrollTop($(document).height());", 10);
        }
    </script>
    
    <div runat="server" id="divTooltipPadder" Visible="false" style="height: 135px;"><!-- This div is needed to keep the 'Correction verification threshold' tooltip on-screen --></div>

</asp:Content>

