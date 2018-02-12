<%@ Page language="c#" Inherits="Spend_Management.aeSignoffGroup" MasterPageFile="~/masters/smForm.master" Codebehind="aeSignoffGroup.aspx.cs" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <script type="text/javascript" src="/expenses/javaScript/sel.SignoffGroups.js?date=20180212"></script>
    <script type="text/javascript" language="javascript">
        (function (r) {
            r.Validators.cmblist = '<%= this.cmblistValidator.ClientID %>';
            r.Validators.cmbholidaylist = '<%= this.cmbholidaylistValidator.ClientID %>';
            r.Validators.DropDownLineManagerAssignmentSupervisor = '<%= this.CostCodeDivValidator.ClientID %>';
            r.Validators.txtExtraLevels = '<%= this.txtExtraLevelsValidator.ClientID %>';
            r.Validators.txtamountRequired = '<%= this.txtamountRequired.ClientID %>';
            r.Validators.txtamountCompare = '<%= this.txtamountCompare.ClientID %>';

            r.SignoffGroup.GroupId = '<%= this.Request.QueryString["groupid"]%>';
            r.SignoffGroup.GroupName = '#<%= this.txtgroupname.ClientID %>';
            r.SignoffGroup.GroupDescription = '#<%= this.txtdescription.ClientID %>';
            r.SignoffGroup.OneClickAuth = '#<%= this.chkAllowOneStepAuthorisation.ClientID %>';
            r.SignoffGroup.ClaimIsInProgress = '#<%= this.lblclaimsinprocess.ClientID %>';

            r.SignoffStage.SignoffDropdown = '#<%= this.cmbsignofftype.ClientID %>';
            r.SignoffStage.SignoffValuesDropdown = '#<%= this.cmblist.ClientID %>';
            r.SignoffStage.SignoffValuesLabel = '#<%= this.SignoffValuesLabel.ClientID %>';

            r.SignoffStage.HolidayDropdown = '#<%= this.cmbonholiday.ClientID%>';
            r.SignoffStage.HolidayListDropdown = '#<%= this.cmbholidaylist.ClientID%>';
            r.SignoffStage.HolidayListLabel = '#<%= this.Label8.ClientID%>';
            r.SignoffStage.HolidayTypeDropdown = '#<%= this.cmbholidaytype.ClientID%>';
            r.SignoffStage.HoliayTypeLabel = '#<%= this.Label7.ClientID%>';

            r.SignoffStage.CostCodeOwnerLabel = '#<%= this.LabelLineManagerAssignmentSupervisor.ClientID%>';
            r.SignoffStage.CostCodeOwnerDropDown = '#<%= this.DropDownLineManagerAssignmentSupervisor.ClientID%>';

            r.SignoffStage.EnvelopeReceivedLabel = '#<%= this.lblNotifyWhenEnvelopeReceived.ClientID%>';
            r.SignoffStage.EnvelopeReceivedCheckBox = '#<%= this.chkNotifyWhenEnvelopeReceived.ClientID%>';
            r.SignoffStage.EnvelopeNotReceivedLabel = '#<%= this.lblNotifyWhenEnvelopeNotReceived.ClientID%>';
            r.SignoffStage.EnvelopeNotReceivedCheckBox = '#<%= this.chkNotifyWhenEnvelopeNotReceived.ClientID%>';

            r.SignoffStage.ExtraLevelsLabel = '#<%= this.lblExtraLevels.ClientID %>';
            r.SignoffStage.ExtraLevelsTextBox = '#<%= this.txtExtraLevels.ClientID %>';
            r.SignoffStage.FromMyLevelLabel = '#<%= this.lblFromMyLevel.ClientID %>';
            r.SignoffStage.FromMyLevelCheckbox = '#<%= this.chkFromMyLevel.ClientID %>';

            r.SignoffStage.IncludeDropDown = '#<%= this.cmbinclude.ClientID %>';
            r.SignoffStage.AmountLabel = '#<%= this.Label6.ClientID %>';
            r.SignoffStage.AmountTextBox = '#<%= this.txtamount.ClientID %>';
            r.SignoffStage.AmountDropDown = '#<%= this.cmbincludelst.ClientID %>';

            r.SignoffStage.InvlolvementDropDown = '#<%= this.cmbinvolvement.ClientID %>';

            r.SignoffStage.ApproverEmailCheckbox = '#<%= this.chksendmail.ClientID %>';
            r.SignoffStage.ClaimantEmailCheckbox = '#<%= this.chkclaimantmail.ClientID %>';
            r.SignoffStage.SingleSignoffCheckbox = '#<%= this.chksinglesignoff.ClientID %>';
            r.SignoffStage.DeclarationCheckbox = '#<%= this.chkdisplaydeclaration.ClientID %>';
            r.SignoffStage.ApproverJustificationCheckbox = '#<%= this.chkApproverJustificationsRequired.ClientID %>';
        }(SEL.SignoffGroups.DomIDs));

        $(document).ready(function () {
            SEL.SignoffGroups.SetupDialogsStage();
        });
    </script>
    
    <div id="divAddStage">
        <a href="javascript:SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal = true;SEL.SignoffGroups.DomIDs.SignoffStage.StageId = 0;SEL.SignoffGroups.SignoffGroup.Save();" class="submenuitem">New Stage</a>
    </div>
</asp:Content>
    
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    

    <div class="inputpanel">
        <br/>
        <asp:Label Visible="false" ID="lblclaimsinprocess" font-size="12pt" runat="server" Text="This signoff group cannot currently be amended as there are one or more claims in the approval process relating to this signoff group." ForeColor="Red"></asp:Label>
    </div>

	<div class="sm_panel">
	    <div class="sectiontitle">
	        <asp:Label id="Label3" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label>
	    </div>
	    <div class="twocolumn">
	        <asp:Label id="lblgroupname" runat="server" CssClass="mandatory" meta:resourcekey="lblgroupnameResource1" AssociatedControlID="txtgroupname">Name*</asp:Label>
	        <span class="inputs"><asp:textbox id="txtgroupname" runat="server" MaxLength="50" meta:resourcekey="txtgroupnameResource1"></asp:textbox></span>
	        <span class="inputicon"></span>
	        <span class="inputtooltipfield"></span>
	        <span class="inputvalidatorfield">
	            <asp:RequiredFieldValidator id="reqgroupname" runat="server" ErrorMessage="Please enter a valid Name." ControlToValidate="txtgroupname" meta:resourcekey="reqgroupnameResource1" ValidationGroup="vgMain">*</asp:RequiredFieldValidator>
	        </span>
	        <asp:Label runat="server" ID="lblAllowOneStepAuthorisation" AssociatedControlID="chkAllowOneStepAuthorisation">Allow one step authorisation</asp:Label>
	        <span class="inputs">
	            <asp:CheckBox runat="server" ID="chkAllowOneStepAuthorisation" />
	        </span>
	        <span class="inputicon"></span>
	        <span class="inputtooltipfield">
	            <img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('3dcba94d-4a5a-4b89-bb49-a9613eaa14df', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
	        </span>
	        <span class="inputvalidatorfield"></span>
	    </div>
        
	    <div class="onecolumn">
	        <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription">Description</asp:Label>
	        <span class="inputs">
	            <asp:textbox id="txtdescription" runat="server" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:textbox>
	        </span>
	        <span class="inputicon"></span>
	        <span class="inputtooltipfield"></span>
	        <span class="inputvalidatorfield"></span>
	    </div>
        <div class="formpanel formpanel_padding">
	        <div class="sectiontitle">
                <asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Signoff Stages</asp:Label>
	        </div>
            <asp:Literal runat="server" ID="litGrid"></asp:Literal>
            <helpers:CSSButton runat="server" ID="btnSave" Text="save" OnClientClick="SEL.SignoffGroups.SignoffGroup.Save(); return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton runat="server" ID="btnCancel" Text="cancel" OnClientClick="SEL.SignoffGroups.SignoffGroup.Cancel(); return false;" UseSubmitBehavior="False" />
        </div>
    </div>
                    
	<div id="AddEdiStageModal" style="display: none; overflow: auto; max-height: 600px;">
	    <div class="sm_panel">
	        <div class="sectiontitle">
	            <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label>
	        </div>
	        <div class="twocolumn">
	            <asp:Label id="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1" AssociatedControlID="cmbsignofftype">Type</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbsignofftype" runat="server" AutoPostBack="False" meta:resourcekey="cmbsignofftypeResource1" onchange="javascript:SEL.SignoffGroups.SignoffStage.SignoffTypeDropdownOnChange();">
	                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource1">Budget Holder</asp:ListItem>
	                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Employee</asp:ListItem>
	                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource3">Team</asp:ListItem>
	                    <asp:ListItem Value="4" meta:resourcekey="ListItemResource4">Line Manager</asp:ListItem>
	                    <asp:ListItem Value="8" meta:resourcekey="ListItemResource21">Cost Code Owner</asp:ListItem>
	                </asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip366" onclick="SEL.Tooltip.Show('ac662c39-a0f1-4395-baa7-4a4d3f7b9cac', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	                
	            <asp:Label id="SignoffValuesLabel" runat="server" meta:resourcekey="lblsignoffResource1" AssociatedControlID="cmblist">Approver</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmblist" runat="server" meta:resourcekey="cmblistResource1"></asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield">
	                <asp:RequiredFieldValidator id="cmblistValidator" runat="server" ControlToValidate="cmblist" ErrorMessage="Please select a Signoff Person for this stage." Text="*" meta:resourcekey="reqsignoffResource1" ValidationGroup="vgStage"></asp:RequiredFieldValidator>
	            </span>
	        </div>

	        <div class="twocolumn">
	            <asp:Label id="lblholiday" runat="server" meta:resourcekey="lblholidayResource1" AssociatedControlID="cmbonholiday">If approver is on holiday</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbonholiday" runat="server" AutoPostBack="False" meta:resourcekey="cmbonholidayResource1" onchange="javascript:SEL.SignoffGroups.SignoffStage.OnHolidayOnChange();">
	                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource14">Take no action</asp:ListItem>
	                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource15">Skip stage</asp:ListItem>
	                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource16">Assign claim to someone else</asp:ListItem>
	                </asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	            <span class="inputs"></span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	        </div>
            
	        <div class="twocolumn" id="divOnHoliday" style="display: none;">
	            <asp:Label id="Label7" runat="server" meta:resourcekey="lblholidayResource1" AssociatedControlID="cmbholidaytype">Holiday approver type</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbholidaytype" runat="server" Visible="True" AutoPostBack="False" meta:resourcekey="cmbholidaytypeResource1" onchange="javascript:SEL.SignoffGroups.SignoffStage.HolidayApproverOnChange();">
	                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource17">Budget Holder Responsible</asp:ListItem>
	                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource18">Employee</asp:ListItem>
	                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource19">Team</asp:ListItem>
	                    <asp:ListItem Value="4" meta:resourcekey="ListItemResource20">Line Manager</asp:ListItem>
	                </asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	            <asp:Label id="Label8" runat="server" meta:resourcekey="lblholidayResource1" AssociatedControlID="cmbholidaylist">Holiday approver</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbholidaylist" runat="server" meta:resourcekey="cmbholidaylistResource1"></asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield">
	                <asp:RequiredFieldValidator id="cmbholidaylistValidator" runat="server" ErrorMessage="Please select a user responsible whilst the signoff person is on holiday." Enabled="False" ControlToValidate="cmbholidaylist" meta:resourcekey="reqholidaylstResource1"  ValidationGroup="vgStage">*</asp:RequiredFieldValidator>
	            </span>
	        </div>

	        <div class="twocolumn" id="divCostCodeOwner" style="display: none;">
	            <asp:Label id="LabelLineManagerAssignmentSupervisor" runat="server" AssociatedControlID="DropDownLineManagerAssignmentSupervisor" CssClass="mandatory">Action if no cost code owner and no default cost code owner*</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="DropDownLineManagerAssignmentSupervisor" runat="server">
	                    <asp:ListItem Value="">[None]</asp:ListItem>
	                    <asp:ListItem Value="LineManager">Assign claim to Line Manager</asp:ListItem>
	                </asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield">
	                <asp:RequiredFieldValidator id="CostCodeDivValidator" runat="server" ErrorMessage="Please select an Action if no cost code owner and no default cost code owner." ControlToValidate="DropDownLineManagerAssignmentSupervisor" Enabled="False"  ValidationGroup="vgStage">*</asp:RequiredFieldValidator>
	            </span>
	        </div>
            
	        <div class="twocolumn" id="divScanAttach" style="display: none;">
	            <asp:Label ID="lblNotifyWhenEnvelopeReceived" runat="server" AssociatedControlID="chkNotifyWhenEnvelopeReceived">Notify claimant when envelopes are received for Scan & Attach</asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chkNotifyWhenEnvelopeReceived" runat="server" ClientIDMode="Static"></asp:CheckBox>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip3230" onclick="SEL.Tooltip.Show('F7E5A916-241A-4FBF-A3C2-501010C226D5', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
	            </span>
	            <span class="inputvalidatorfield"></span>
	            <asp:Label ID="lblNotifyWhenEnvelopeNotReceived" runat="server" AssociatedControlID="chkNotifyWhenEnvelopeNotReceived">Notify claimant when envelopes are <strong>not</strong> received for Scan & Attach</asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chkNotifyWhenEnvelopeNotReceived" runat="server" ClientIDMode="Static"></asp:CheckBox>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip3231" onclick="SEL.Tooltip.Show('C7BAAC76-BB65-4A6F-B0EF-196A66F966E8', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	        </div>

	        <div class="twocolumn" id="divApprovalMatrix" style="display: none;">
	            <asp:Label id="lblExtraLevels" runat="server" meta:resourcekey="lblExtraLevelsResource1" CssClass="mandatory" AssociatedControlID="txtExtraLevels" >Include additional approval matrix levels*</asp:Label>
	            <span class="inputs">
	                <asp:TextBox id="txtExtraLevels" runat="server" style="margin-left: 2px;"></asp:TextBox>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip589" onclick="SEL.Tooltip.Show('6de3682d-0d5a-4d25-a4e0-6d2942c1f770', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
	            </span>
	            <span class="inputvalidatorfield">
	                <cc1:FilteredTextBoxExtender runat="server" ID="fteExtraLevels" FilterMode="ValidChars" ValidChars="0123456789" TargetControlID="txtExtraLevels"/>
	                <asp:RequiredFieldValidator id="txtExtraLevelsValidator" runat="server" Enabled="False" ControlToValidate="txtExtraLevels" ErrorMessage="Please enter a number of approval matrix levels above the actual claim amount level that approvers may be chosen from by the claimant." ValidationGroup="vgStage">*</asp:RequiredFieldValidator>
	            </span>
	            <asp:Label id="lblFromMyLevel" runat="server" meta:resourcekey="lblFromMyLevelResource1" AssociatedControlID="chkFromMyLevel">Only allow approvers above my level</asp:Label>
	            <span class="inputs">
	                <asp:CheckBox id="chkFromMyLevel" runat="server" style="margin-left: 2px;"></asp:CheckBox>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	        </div>

	        <div class="sectiontitle">
	            <asp:Label id="Label5" runat="server" meta:resourcekey="Label1Resource1">Involvement</asp:Label>
	        </div>
	        <div class="twocolumn">
	            <asp:Label id="lblinclude" runat="server" meta:resourcekey="lblincludeResource1" AssociatedControlID="cmbinclude">When to include</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbinclude" runat="server" AutoPostBack="False" meta:resourcekey="cmbincludeResource1"  onchange="javascript:SEL.SignoffGroups.SignoffStage.WhenToIncludeOnChange();">
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
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip367" onclick="SEL.Tooltip.Show('ef816c1c-9e8e-427b-a8b9-7701c801b97b', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	            <asp:Label id="Label6" runat="server" meta:resourcekey="lblincludeResource1" AssociatedControlID="txtamount" style="display: none;" ></asp:Label>
	            <span class="inputs">
	                <asp:TextBox id="txtamount" runat="server" meta:resourcekey="txtamountResource1" style="display: none;" ></asp:TextBox>
	                <asp:DropDownList id="cmbincludelst" runat="server" meta:resourcekey="cmbincludelstResource1" style="display: none;" ></asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield">
	                <asp:RequiredFieldValidator id="txtamountRequired" runat="server" ErrorMessage="Please enter an Amount." ControlToValidate="txtamount" meta:resourcekey="reqamountResource1" ValidationGroup="vgStage" text="*"></asp:RequiredFieldValidator>
	                <asp:CompareValidator id="txtamountCompare" runat="server" ErrorMessage="Please enter a valid Amount." ControlToValidate="txtamount" Operator="DataTypeCheck" Type="Currency" meta:resourcekey="compamountResource1" ValidationGroup="vgStage" text="*"></asp:CompareValidator>
	            </span>
	        </div>

	        <div class="twocolumn">
	            <asp:Label id="lblinvolvement" runat="server" meta:resourcekey="lblinvolvementResource1" AssociatedControlID="cmbinvolvement">Action</asp:Label>
	            <span class="inputs">
	                <asp:DropDownList id="cmbinvolvement" runat="server" meta:resourcekey="cmbinvolvementResource1">
	                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource13">User is to check claim</asp:ListItem>
	                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource12">Just notify user of claim</asp:ListItem>
	                </asp:DropDownList>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
                <span class="inputs"></span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	        </div>

	        <div class="sectiontitle">
	            <asp:Label id="Label9" runat="server" meta:resourcekey="Label1Resource1">Notifications</asp:Label>
	        </div>
	        <div class="twocolumn">
	            <asp:Label ID="lblsendemailapprover" runat="server" Text="Notify approver when claim reaches this stage" meta:resourcekey="lblsendemailapproverResource1" AssociatedControlID="chksendmail"></asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chksendmail" runat="server" meta:resourcekey="chksendmailResource1" />
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	            <asp:Label id="lblclaimantmail" runat="server" meta:resourcekey="lblclaimantmailResource1" Text="Notify claimant when claim reaches this stage" AssociatedControlID="chkclaimantmail"></asp:Label>
	            <span class="inputs">
	                <asp:CheckBox id="chkclaimantmail" runat="server" meta:resourcekey="chkclaimantmailResource1"></asp:CheckBox>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	        </div>

	        <div class="sectiontitle">
	            <asp:Label id="Label10" runat="server" meta:resourcekey="Label1Resource1">Other Options</asp:Label>
	        </div>
	        <div class="twocolumn">
	            <asp:Label ID="lbloneclick" runat="server" Text='Enable "One Click" Signoff' meta:resourcekey="lbloneclickResource1" AssociatedControlID="chksinglesignoff"></asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chksinglesignoff" runat="server" meta:resourcekey="chksinglesignoffResource1" AutoPostBack="False"  onchange="javascript:SEL.SignoffGroups.SignoffStage.OneClickSignOffOnChange();"/>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip369" onclick="SEL.Tooltip.Show('8b496e8f-e22f-4dfa-8e98-bb0bbe0d51d0', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	            <asp:Label ID="Label11" runat="server" Text='Approvers must sign declaration before approving a claim' meta:resourcekey="lbloneclickResource1" AssociatedControlID="chkdisplaydeclaration"></asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chkdisplaydeclaration" runat="server" />
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip368" onclick="SEL.Tooltip.Show('94228db9-89ed-445b-9dad-b3b47c07c373', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	        </div>
	        <div class="twocolumn">
	            <asp:Label ID="Label12" runat="server" Text='Approver justifications required' meta:resourcekey="lbloneclickResource1" AssociatedControlID="chkApproverJustificationsRequired"></asp:Label>
	            <span class="inputs">
	                <asp:CheckBox ID="chkApproverJustificationsRequired" runat="server" AutoPostBack="False" onchange="javascript:SEL.SignoffGroups.SignoffStage.ApproverJustificationOnChange();"/>
	            </span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield">
	                <img id="imgtooltip368" onclick="SEL.Tooltip.Show('6889C27E-CD62-4695-8FE8-BC39E2DEC3F4', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/>
	            </span>
	            <span class="inputvalidatorfield"></span>
	            <span class="inputs"></span>
	            <span class="inputicon"></span>
	            <span class="inputtooltipfield"></span>
	            <span class="inputvalidatorfield"></span>
	        </div>
        </div>
	</div>
	

</asp:Content>


