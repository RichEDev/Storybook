<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aeexchangerate.aspx.cs" MasterPageFile="~/masters/smForm.master" Inherits="Spend_Management.aeexchangerate" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <script src="../javascript/currencies.js" type="text/javascript" language="javascript"></script>
    <script src="../javascript/validate.js" type="text/javascript" language="javascript"></script>
    
    <script type="text/javascript" language="javascript">
        var cmbmonthid = '<%= cmbMonth.ClientID %>';
        var txtyearid = '<%= txtYear.ClientID %>';
        var txtstartdateid = '<%= txtStartDate.ClientID %>';
        var txtenddateid = '<%= txtEndDate.ClientID %>';
        var currID = '<% = currID %>';
        var currencyid = '<% = currencyid %>';
    </script>
    
    <asp:ValidationSummary ID="vgMain" runat="server" ShowSummary="False"/>
    <div id="monthlyCurrencies" runat="server">
        <div class="formpanel">
            <div class="sectiontitle">General Details</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" id="lblMonth" runat="server" meta:resourcekey="lblMonthResource1" AssociatedControlID="cmbMonth">Month*</asp:Label>
                <span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbMonth" runat="server" meta:resourcekey="cmbMonthResource1">
                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource1">January</asp:ListItem>
						    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">February</asp:ListItem>
						    <asp:ListItem Value="3" meta:resourcekey="ListItemResource3">March</asp:ListItem>
						    <asp:ListItem Value="4" meta:resourcekey="ListItemResource4">April</asp:ListItem>
						    <asp:ListItem Value="5" meta:resourcekey="ListItemResource5">May</asp:ListItem>
						    <asp:ListItem Value="6" meta:resourcekey="ListItemResource6">June</asp:ListItem>
						    <asp:ListItem Value="7" meta:resourcekey="ListItemResource7">July</asp:ListItem>
						    <asp:ListItem Value="8" meta:resourcekey="ListItemResource8">August</asp:ListItem>
						    <asp:ListItem Value="9" meta:resourcekey="ListItemResource9">September</asp:ListItem>
						    <asp:ListItem Value="10" meta:resourcekey="ListItemResource10">October</asp:ListItem>
						    <asp:ListItem Value="11" meta:resourcekey="ListItemResource11">November</asp:ListItem>
						    <asp:ListItem Value="12" meta:resourcekey="ListItemResource12">December</asp:ListItem>
                </asp:DropDownList></span>
			    <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label CssClass="mandatory" id="lblYear" runat="server" meta:resourcekey="lblYearResource1" AssociatedControlID="txtYear">Year*</asp:Label>
                <span class="inputs"><asp:TextBox ID="txtYear" runat="server" CssClass="fillspan" meta:resourcekey="txtYearResource1" MaxLength="4"></asp:TextBox></span>
		        <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator id="reqYear" runat="server" ErrorMessage="Please enter a value for the year field" ControlToValidate="txtYear" meta:resourcekey="reqYearResource1">*</asp:RequiredFieldValidator>
				    <asp:CompareValidator id="compYear" runat="server" ErrorMessage="Please enter a valid value for the year field" ControlToValidate="txtYear" Type="Integer" ValueToCompare="1998" Operator="GreaterThan" meta:resourcekey="compYearResource1">*</asp:CompareValidator>
				    <asp:CompareValidator id="compYearLess" runat="server" ErrorMessage="Please enter a valid value for the year field" ControlToValidate="txtYear" Type="Integer" ValueToCompare="2500" Operator="LessThan" meta:resourcekey="compYearResource1">*</asp:CompareValidator>
                </span>
            </div>
        </div>
    </div>
    
    <div id="rangeCurrencies" runat="server">
        <div class="formpanel">
            <div class="sectiontitle">General Details</div>
            <div class="twocolumn">
				<asp:Label id="lblStartDate"  CssClass="mandatory" runat="server" meta:resourcekey="lblstartdateResource1" AssociatedControlID="txtStartDate">Start Date*</asp:Label>

				<span class="inputs"><asp:TextBox id="txtStartDate" CssClass="fillspan" runat="server" meta:resourcekey="txtstartdateResource1"></asp:TextBox>
				    
                </span>
                <span class="inputicon"><asp:Image ID="imgstart" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imgstartResource1" />
                <cc1:MaskedEditExtender CultureName="en-GB" ID="mskstartdate" MaskType="Date" Mask="99/99/9999" TargetControlID="txtstartdate"
                        runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True">
                    </cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calstart" runat="server" TargetControlID="txtstartdate" PopupButtonID="imgstart" Format="dd/MM/yyyy" Enabled="True">
                    </cc1:CalendarExtender>
                </span>
				<span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqstartdate" runat="server" ErrorMessage="Please enter a start date" ControlToValidate="txtStartDate" meta:resourcekey="reqstartdateResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compstartdate" runat="server" ErrorMessage="Please enter a valid start date"
						ControlToValidate="txtStartDate" Operator="DataTypeCheck" Type="Date" meta:resourcekey="compstartdateResource1">*</asp:CompareValidator>
				</span>
			
                <asp:Label id="lblEndDate"  CssClass="mandatory" runat="server" meta:resourcekey="lblEndDateResource1" AssociatedControlID="txtEndDate">End Date*</asp:Label>

				<span class="inputs"><asp:TextBox id="txtEndDate" runat="server" CssClass="fillspan" meta:resourcekey="txtEndDateResource1"></asp:TextBox>
				    <cc1:MaskedEditExtender CultureName="en-GB" ID="mskenddate" MaskType="Date" Mask="99/99/9999" TargetControlID="txtEndDate"
                        runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True">
                    </cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calEnd" runat="server" TargetControlID="txtEndDate" PopupButtonID="imgEnd" Format="dd/MM/yyyy" Enabled="True">
                    </cc1:CalendarExtender>
                </span>
                <span class="inputicon"><asp:Image ID="imgEnd" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imgEndResource1" /></span>
				<span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqEndDate" runat="server" ErrorMessage="Please enter an end date" ControlToValidate="txtEndDate" meta:resourcekey="reqstartdateResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compEndDate" runat="server" ErrorMessage="Please enter a valid end date"
						ControlToValidate="txtEndDate" Operator="DataTypeCheck" Type="Date" meta:resourcekey="compEndDateResource1">*</asp:CompareValidator><asp:CompareValidator ID="compEndDateGTStartDate" runat="server" ControlToCompare="txtStartDate" ControlToValidate="txtEndDate" Operator="GreaterThanEqual" Type="Date" ErrorMessage="The end date must be after the start date">*</asp:CompareValidator>
				</span>
            </div>
        </div>
    </div>
    
    <div class="formpanel">
	    <div class="sectiontitle">Exchange Rates</div>
        <asp:Literal ID="litexchangerates" runat="server" meta:resourcekey="litexchangeratesResource1"></asp:Literal>
	</div>
    
    <div class="formbuttons">
        <asp:Image ID="imgok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="saveCurrencyExchanges();" style="cursor:pointer" />
            &nbsp;&nbsp;
        <asp:ImageButton id="cmdcancel" runat="server" 
            ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" 
            meta:resourcekey="cmdcancelResource1" onclick="cmdcancel_Click"></asp:ImageButton>
    </div>


</asp:Content>
