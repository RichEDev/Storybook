<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeflagrule.aspx.cs" Inherits="Spend_Management.aeflagrule" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script type="text/javascript">
        (function (columns) {
            columns.Drop = '<%= this.tcFilters.TreeDropClientID %>';
            columns.TreeContainer = '<%= this.tcFilters.ClientID %>';
            columns.Tree = '<%= this.tcFilters.TreeClientID %>';
        }(SEL.Reports.IDs.CriteriaSelector));
    </script>
    <asp:ScriptManagerProxy ID="smProxy" runat="server">
        <Services>
            <asp:ServiceReference Path="~/expenses/webservices/svcFlagRules.asmx" />
            <asp:ServiceReference Path="~/shared/webservices/svcTree.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/jquery-selui-dialog.js" />
            <asp:ScriptReference Path="~/expenses/javaScript/SEL.FlagRules.js" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.reports.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
 
    <div id="tabs">
        <ul>
            <li><a href="#tabsGeneral">General Details</a></li>
            <li><a href="#tabsRoles">Item Roles</a></li>
            <li><a href="#tabsExpenses">Expense Items</a></li>
        </ul>
        <div id="tabsGeneral">

            <div class="sm_panel">
                <div class="sectiontitle">
                    <asp:Label ID="Label1" runat="server" Text="General Details"></asp:Label></div>
                <div class="onecolumnsmall"><asp:Label CssClass="mandatory" ID="lblflagtype" runat="server" Text="Type*" AssociatedControlID="ddlstFlagType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstFlagType" runat="server">
                        <asp:ListItem Value="1" Text="Duplicate expense" />
                        <asp:ListItem Value="6" Text="Frequency of item (count)" />
                        <asp:ListItem Value="7" Text="Frequency of item (sum)" />
                        <asp:ListItem Value="9" Text="Group limit with a receipt" />
                        <asp:ListItem Value="8" Text="Group limit without a receipt" />
                        <asp:ListItem Value="15" Text="Home to location greater" />
                        <asp:ListItem Value="5" Text="Invalid date" />
                        <asp:ListItem Value="12" Text="Item not reimbursable" />
                        <asp:ListItem Value="17" Text="Item reimbursable" />
                        <asp:ListItem Value="4" Text="Item on a weekend" />
                        <asp:ListItem Value="3" Text="Limit with a receipt" />
                        <asp:ListItem Value="2" Text="Limit without a receipt" />
                        <asp:ListItem Value="20" Text="One item in a group" />
                        <asp:ListItem Value="21" Text="Only allow journeys which start and end at home or office" />
                        <asp:ListItem Value="19" Text="Passenger limit" />
                        <asp:ListItem Value="18" Text="Receipt not attached" />
                        <asp:ListItem Value="16" Text="Recommended distance exceeded" />
                        <asp:ListItem Value="22" Text="Restrict number of miles per day" />
                        <asp:ListItem Value="14" Text="Tip limit exceeded" />
                        <asp:ListItem Value="13" Text="Unused advance available" />
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('D1581A3A-57FE-4353-84E7-C1CDFA68F3D1', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                   </div>      
                 <div class="onecolumnsmall"><asp:Label ID="lblaction" runat="server" Text="Action*" AssociatedControlID="ddlstAction" CssClass="mandatory"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstAction" runat="server">
                        <asp:ListItem Value="1" Text="Flag Item"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Block Item"></asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('EE33B75A-020A-4DD2-85D0-02D1D3059947', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span></div>
                <div class="onecolumn">
                    <asp:Label ID="lbldescription" runat="server" Text="Description*" CssClass="mandatory" AssociatedControlID="txtdescription"></asp:Label><span class="inputs"><asp:TextBox ID="txtdescription" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqDescription" runat="server" ErrorMessage="Please enter a Description." Text="*" ControlToValidate="txtdescription" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator></span></div>
                <div class="onecolumn">
                    <asp:Label ID="lblflagtext" runat="server" Text="Custom explanation" AssociatedControlID="txtflagtext"></asp:Label><span class="inputs"><asp:TextBox ID="txtflagtext" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('BCA98905-C0A7-4859-8D5A-89AA4C3AF2B6', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span></div>
                <div class="twocolumn"><asp:Label ID="lblactive" runat="server" Text="Active" AssociatedControlID="chkactive"></asp:Label><span class="inputs"><asp:CheckBox ID="chkactive" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('84ABF8F4-8F5F-43EC-BBC0-3A3E86D18E59', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span><asp:Label ID="Label13" runat="server" Text="Level" AssociatedControlID="ddlstFlagLevel"></asp:Label><span class="inputs"><asp:DropDownList
                        ID="ddlstFlagLevel" runat="server">
                        <asp:ListItem Value="3" Text="Red"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Amber"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Information only"></asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('9FB0E74F-A9B3-4DB3-940A-07B884C4DEC6', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblclaimantJustificationRequired" runat="server" Text="Claimant justification required" AssociatedControlID="chkClaimantJustificationRequired"></asp:Label><span class="inputs"><asp:CheckBox ID="chkClaimantJustificationRequired" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('241E76A2-51A9-4336-9953-4014F2F58C12', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span><asp:Label ID="Label12" runat="server" Text="Approver justification required" AssociatedControlID="chkApproverJustificationRequired"></asp:Label><span class="inputs"><asp:CheckBox ID="chkApproverJustificationRequired" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('368D3A4F-C888-453C-A012-D97B9DC4E191', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span></div>
                <div class="twocolumn">
                    <asp:Label ID="lblDisplayFlagImmediately" runat="server" Text="Display flag immediately" AssociatedControlID="chkDisplayFlagImmediately"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayFlagImmediately" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('BF6397B3-4D4F-4850-8124-B5507EE8C1B1', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                <div class="onecolumn"><asp:Label ID="Label16" runat="server" Text="Notes for authoriser" AssociatedControlID="txtNotesForAuthoriser"></asp:Label><span class="inputs"><asp:TextBox ID="txtNotesForAuthoriser" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                <div id="divFields">
                    <div class="sectiontitle">
                        <asp:Label ID="lblDuplicateFieldsToCheck" runat="server" Text="Which fields should be checked to determine if an expense item is a duplicate?"></asp:Label></div>
                    <div class="sm_comment" style="padding-left: 3px;">Please note: When the claimant enters an expense, the fields selected below must be available on the add expense screen for the claimant to enter a value for, in order for a duplicate check to be performed.</div>
                    <div><p><a href="javascript:SEL.FlagsAndLimits.populateFields();SEL.FlagsAndLimits.popupFieldsModal(true);">Add Fields</a></p></div>
                    <div id="divSelectedFieldList">
                        <asp:Literal ID="litFields" runat="server"></asp:Literal></div>
                </div>
                <div id="divLimitRule">
                    <div class="twocolumn">
                        <asp:Label ID="lblnoflagtolerance" runat="server" Text="Do not flag if within X (%)" AssociatedControlID="txtNoFlagTolerance"></asp:Label><span class="inputs"><asp:TextBox ID="txtNoFlagTolerance" runat="server" MaxLength="5"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('619EFF00-19EC-4BF7-A21A-94519EF30082', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RangeValidator ID="rangeNoFlagTolerance" runat="server" ControlToValidate="txtNoFlagTolerance" MaximumValue="100" MinimumValue="0" Type="Double" Text="*" ErrorMessage="Please enter a value for Do not flag if within X (%) between 0 and 100." ValidationGroup="vgFlag" Display="Dynamic"></asp:RangeValidator></span><asp:Label ID="lblambertolerance" runat="server" Text="Amber tolerance (%)" AssociatedControlID="txtambertolerance"></asp:Label><span class="inputs"><asp:TextBox ID="txtambertolerance" runat="server" MaxLength="5"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('728104DF-0ECB-4542-9724-B73F50690525', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RangeValidator ID="rangeAmber" runat="server" ControlToValidate="txtambertolerance" MaximumValue="100" MinimumValue="0" Type="Double" Text="*" ErrorMessage="Please enter a value for Amber tolerance (%) between 0 and 100." ValidationGroup="vgFlag" Display="Dynamic"></asp:RangeValidator><asp:RegularExpressionValidator ID="regExpAmbertolerance" runat="server" ControlToValidate="txtambertolerance" ErrorMessage="Please enter an Amber tolerance (%) with a maximum of 2 decimal places." Text="*" ValidationExpression="[0-9]?[0-9]?[0-9]?\.?([0-9][0-9]?)?" ValidationGroup="vgFlag" Display="Dynamic"></asp:RegularExpressionValidator></span></div>
                </div>
                <div class="twocolumn" id="divReceiptLimitRule">
                    <asp:Label ID="lblIncreaseForNumOthers" runat="server" Text="Increase limit for meal expenses by the Number of Others field" AssociatedControlID="chkIncreaseByNumOthers"></asp:Label><span class="inputs"><asp:CheckBox ID="chkIncreaseByNumOthers" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('533D08E0-D28A-422C-81AA-98A620C7EED8', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span><span id="divShowLimitToClaimant"><asp:Label ID="Label15" runat="server" Text="Display a claimant's limit when they are adding an expense" AssociatedControlID="chkDisplayLimit"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayLimit" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('AA2B99A4-22DA-4D4F-9180-FC8CAF03D8AC', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span></span></div>
                <div class="twocolumn" id="divTipLimitRule">
                    <asp:Label ID="Label11" runat="server" Text="Limit (%)*" CssClass="mandatory" AssociatedControlID="txtTipLimit"></asp:Label><span class="inputs"><asp:TextBox ID="txtTipLimit" runat="server" MaxLength="5"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip43" onmouseover="SEL.Tooltip.Show('6FFF3D4D-36C5-439D-9858-B9E36A4267D2', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqTipLimit" runat="server" ErrorMessage="Please enter a Limit (%)." ControlToValidate="txtTipLimit" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:RangeValidator ID="rangeTipLimit" runat="server" ErrorMessage="Please enter a Limit between 1% and 100%." ControlToValidate="txtTipLimit" Type="Double" MinimumValue="1" MaximumValue="100" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RangeValidator>
                        <asp:RegularExpressionValidator ID="regExpTipLimit" runat="server" ControlToValidate="txtTipLimit" ErrorMessage="Please enter a Limit(%) with a maximum of 2 decimal places." Text="*" ValidationExpression="[0-9]?[0-9]?[0-9]?\.?([0-9][0-9]?)?" ValidationGroup="vgFlag" Display="Dynamic"></asp:RegularExpressionValidator></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                <div class="twocolumn" id="divGroupLimitRule">
                    <asp:Label AssociatedControlID="txtgrouplimit" runat="server" Text="Group limit*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtgrouplimit" runat="server" MaxLength="10"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqGroupLimit" runat="server" ErrorMessage="Please enter a Group Limit." Text="*" ControlToValidate="txtgrouplimit" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator ID="compGroupLimit" runat="server" Text="*" ErrorMessage="Please enter a valid Group Limit." ControlToValidate="txtgrouplimit" Operator="GreaterThanEqual" Type="Double" ValueToCompare="0" ValidationGroup="vgFlag" Display="Dynamic"></asp:CompareValidator></span></div>
                <div id="divCustomRule">
                    <!--<div class="formpanel" style="width: 882px;">-->
                    <div class="sectiontitle">Flag Criteria</div>
                    <div class="modalcontentssmall">
                        <div class="onecolumnpanel">
                            Use the editor below to choose the criteria for this report.
                        </div>
                        <helpers:TreeCombo ID="tcFilters" runat="server" ComboType="TreeAndFilters" ShowButtonMenu="true" Width="800" Height="310" LeftPanelWidth="270" LeftTitle="Available Fields" RightTitle="Flag Criteria" WebServicePath="~/shared/webservices/svcReports.asmx" FilterValidationGroup="vgViewFilter" RenderFilterModal="True" RenderReportOptions="True" ThemesPath="/static/js/jstree/themes/" />
                    </div>
                    <!--</div>-->
                </div>
                <div class="twocolumn" id="divInvalidDateRule">
                    <asp:Label ID="lbldatecomparisontype" runat="server" Text="Comparison type*" CssClass="mandatory" AssociatedControlID="ddlstDateComparisonType"></asp:Label><span class="inputs"><asp:DropDownList
                        ID="ddlstDateComparisonType" runat="server">
                        <asp:ListItem Value="1" Text="Set date" />
                        <asp:ListItem Value="2" Text="Last X months" />
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lbldatecomparisonvalue" runat="server" Text="Date*" AssociatedControlID="txtDate" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtDate" runat="server"></asp:TextBox><asp:TextBox ID="txtMonths" runat="server" MaxLength="2"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqMonths" runat="server" ErrorMessage="Please enter the Number of months." ControlToValidate="txtMonths" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:RequiredFieldValidator ID="reqDate" runat="server" ErrorMessage="Please enter a Date." ControlToValidate="txtDate" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator ID="compMonths" runat="server" ErrorMessage="Please enter a valid Number of months." ControlToValidate="txtMonths" Operator="GreaterThan" ValueToCompare="0" Type="Integer" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator ID="compDate" runat="server" ErrorMessage="Please enter a valid Date." ControlToValidate="txtDate" Operator="DataTypeCheck" Type="Date" Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:CompareValidator></span>
                </div>
                <div id="divFrequencyRule">
                    <div class="twocolumn">
                        <asp:Label ID="lblFrequency" runat="server" Text="Frequency*" AssociatedControlID="txtFrequency" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtFrequency" runat="server" MaxLength="3"></asp:TextBox><asp:TextBox ID="txtLimit" runat="server" MaxLength="10"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtFrequency" ID="reqFrequency" runat="server" ErrorMessage="Please enter a Frequency." Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:RequiredFieldValidator ControlToValidate="txtLimit" ID="reqLimit" runat="server" ErrorMessage="Please enter a Limit." ValidationGroup="vgFlag" Text="*" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator ID="compLimit" runat="server" ErrorMessage="Please enter a valid Limit." Text="*" ControlToValidate="txtLimit" Type="Currency" Operator="GreaterThan" ValueToCompare="0" ValidationGroup="vgFlag" Display="Dynamic"></asp:CompareValidator><asp:RangeValidator ID="rangeFrequency" runat="server" ErrorMessage="Please enter a Frequency between 1 and 100." Text="*" ControlToValidate="txtFrequency" MinimumValue="1" MaximumValue="100" Type="Integer" ValidationGroup="vgFlag" Display="Dynamic"></asp:RangeValidator><asp:RegularExpressionValidator ID="regExpNoFlagTolerance" runat="server" ControlToValidate="txtNoFlagTolerance" ErrorMessage="Please enter a Do not flag if within X (%) with a maximum of 2 decimal places." Text="*" ValidationExpression="[0-9]?[0-9]?[0-9]?\.?([0-9][0-9]?)?" ValidationGroup="vgFlag" Display="Dynamic"></asp:RegularExpressionValidator></span><asp:Label ID="Label7" runat="server" Text="Frequency type*" CssClass="mandatory" AssociatedControlID="ddlstFrequencyType"></asp:Label><span class="inputs">
                            <asp:DropDownList ID="ddlstFrequencyType" runat="server">
                                <asp:ListItem Value="1" Text="In the last"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Every"></asp:ListItem>
                            </asp:DropDownList>

                        </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label8" runat="server" Text="Period*" AssociatedControlID="txtPeriod" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtPeriod" runat="server" MaxLength="3"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtPeriod" ID="reqPeriod" runat="server" ErrorMessage="Please enter a Period." Text="*" ValidationGroup="vgFlag" Display="Dynamic"></asp:RequiredFieldValidator><asp:RangeValidator ID="rangePeriod" runat="server" ErrorMessage="Please enter a Period between 1 and 100." Text="*" ControlToValidate="txtPeriod" MinimumValue="1" MaximumValue="100" Type="Integer" ValidationGroup="vgFlag" Display="Dynamic"></asp:RangeValidator></span><asp:Label ID="Label9" runat="server" Text="Period type*" CssClass="mandatory" AssociatedControlID="ddlstPeriodType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstPeriodType" runat="server">
                            <asp:ListItem Value="1" Text="Days"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Weeks"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Months"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Years"></asp:ListItem>
                            <asp:ListItem Value="5" Text="Calendar weeks"></asp:ListItem>
                            <asp:ListItem Value="6" Text="Calendar months"></asp:ListItem>
                            <asp:ListItem Value="7" Text="Calendar years"></asp:ListItem>
                            <asp:ListItem Value="8" Text="Financial years"></asp:ListItem>
                        </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn" id="divFinancialYear">
                        <asp:Label ID="Label10" runat="server" Text="Financial year*" CssClass="mandatory" AssociatedControlID="ddlstFinancialYear"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstFinancialYear" runat="server"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                </div>
                <div id="divPassengerLimitRule">
                    <div class="twocolumn"><asp:Label ID="lblPassengerLimit" runat="server" Text="Passenger limit*" CssClass="mandatory" AssociatedControlID="txtPassengerLimit"></asp:Label><span class="inputs"><asp:TextBox ID="txtPassengerLimit" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqpassengerlimit" runat="server" ErrorMessage="Please enter a Passenger limit." Text="*" ValidationGroup="vgFlag" ControlToValidate="txtPassengerLimit" Display="Dynamic"></asp:RequiredFieldValidator><asp:RangeValidator ID="compPassengerLimit" runat="server" ErrorMessage="Please enter a Passenger limit between 1 and 20." Type="Integer" MinimumValue="1" MaximumValue="20" ValidationGroup="vgFlag" Text="*" ControlToValidate="txtPassengerLimit" Display="Dynamic"></asp:RangeValidator></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                </div>
                <div id="divRestrictDailyMileage">
                    <div class="twocolumn"><asp:Label ID="lblRestrictDailyMileage" runat="server" Text="Daily mileage limit*" CssClass="mandatory" AssociatedControlID="txtRestrictDailyMileage"></asp:Label><span class="inputs"><asp:TextBox ID="txtRestrictDailyMileage" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqRestrictDailyMileage" runat="server" ErrorMessage="Please enter a Daily mileage limit." Text="*" ValidationGroup="vgFlag" ControlToValidate="txtRestrictDailyMileage" Display="Dynamic"></asp:RequiredFieldValidator><asp:RangeValidator ID="compRestrictDailyMileage" runat="server" ErrorMessage="Please enter a Daily mileage limit greater than or equal to 0.01." Type="Double" MinimumValue="0.01" MaximumValue="99999999" ValidationGroup="vgFlag" Text="*" ControlToValidate="txtRestrictDailyMileage" Display="Dynamic"></asp:RangeValidator></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                </div>
            </div>
        </div>
        <div id="tabsRoles">
            <div class="sm_panel">
                <div class="sectiontitle">
                    <asp:Label ID="Label2" runat="server" Text="Associated Item Roles"></asp:Label></div>
                <div class="twocolumn"><asp:Label ID="Label17" runat="server" Text="Item roles to include" AssociatedControlID="ddlstItemRoleInclusionType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstItemRoleInclusionType" runat="server"><asp:ListItem Value="1" Text="All"></asp:ListItem><asp:ListItem Value="2" Text="Selected"></asp:ListItem></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                <div id="divItemRoles" style="display: none;">
                <div><p><a href="javascript:SEL.FlagsAndLimits.populateRoles();SEL.FlagsAndLimits.popupRolesModal(true);">Add Item Roles</a></p></div>
                <div id="gridSelectedRoles">
                    <asp:Literal ID="litroles" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
        <div id="tabsExpenses">
            <div class="sm_panel">
                <div class="sectiontitle">
                    <asp:Label ID="Label3" runat="server" Text="Associated Expense Items"></asp:Label></div>
                <div class="twocolumn"><asp:Label ID="Label18" runat="server" Text="Expense items to include" AssociatedControlID="ddlstExpenseItemInclusionType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstExpenseItemInclusionType" runat="server"><asp:ListItem Value="1" Text="All"></asp:ListItem><asp:ListItem Value="2" Text="Selected"></asp:ListItem></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                <div id="divExpenseItems" style="display: none;">
                    <div><p><a href="javascript:SEL.FlagsAndLimits.populateExpenseItems();SEL.FlagsAndLimits.popupExpenseItemsModal(true);">Add Expense Items</a></p></div>

                    <div id="divGridSelectedExpenseItems">
                        <asp:Literal ID="litexpenseitems" runat="server"></asp:Literal>
                    </div>
                                        </div>
            </div>
        </div>
    </div>

    <div class="formpanel" style="padding-left:0px;">
    <div class="formbuttons">
                    <helpers:CSSButton runat="server" ID="btnSave" Text="save" OnClientClick="SEL.FlagsAndLimits.currentAction='';SEL.FlagsAndLimits.saveFlagRule(true);return false;" UseSubmitBehavior="False" />&nbsp;&nbsp;<helpers:CSSButton runat="server" ID="CSSButton1" Text="cancel" OnClientClick="document.location = 'flags.aspx';return false;" UseSubmitBehavior="False" /></div>
    </div>

    
     
        
            <div id="divItemRolesModal" style="overflow: auto; max-height: 500px">
                    <div id="divGridRoles"></div>
        </div>


    <div id="divExpenseItemsModal" style="overflow: auto; max-height: 500px">
            
                <div id="divGridExpenseItems"></div>
            
        </div>



    <div id="divFieldsModal" style="overflow: auto; max-height: 500px">
            <div id="divFieldList"></div>            
    </div>
   
    <input type="hidden" value="D70D9E5F-37E2-4025-9492-3BCF6AA746A8" id="txtGuid" />
</asp:Content>

