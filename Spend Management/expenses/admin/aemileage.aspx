<%@ Page language="c#" Inherits="Spend_Management.aemileage" MasterPageFile="~/masters/smForm.master" Codebehind="aemileage.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
     .sectiontitle {
         margin-top: 30px;
     }
  </style>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <script type="text/javascript" language="javascript">
        var cmbDateRangeType = '<%=cmbDateRangeType.ClientID %>';
        var txtcarsize = '<%=txtcarsize.ClientID %>';
        var txtdescription = '<%=txtdescription.ClientID %>';
        var txtuserratetable = '<%=txtnhscode.ClientID %>';
        var txtuserratefromengine = '<%=txtnhsstart.ClientID%>';
        var txtuserratetoengine = '<%=txtnhsend.ClientID%>';
        var cmbThreshold = '<%=cmbThreshold.ClientID %>';
        var cmbCurrency = '<%=cmbCurrency.ClientID %>';
        var cmbUom = '<%=cmbUom.ClientID %>';
        var chkcalcmilestotal = '<%=chkcalcmilestotal.ClientID %>';
        var txtDateVal1 = '<%=txtDateVal1.ClientID %>';
        var txtDateVal2 = '<%=txtDateVal2.ClientID %>';
        var cmbRangeType = '<%=cmbRangeType.ClientID %>';
        var txtPassenger = '<%=txtPassenger.ClientID %>';
        var txtPassengerx = '<%=txtPassengerx.ClientID %>';
        var txtThresholdVal1 = '<%=txtThresholdVal1.ClientID %>';
        var txtThresholdVal2 = '<%=txtThresholdVal2.ClientID %>';
        var txtHeavyBulky = '<%=txtHeavyBulky.ClientID %>';
        var reqDateVal1 = '<%=reqDateVal1.ClientID %>';
        var reqDateVal2 = '<%=reqDateVal2.ClientID %>';
        var reqThreshold1 = '<%=reqThreshold1.ClientID %>';
        var reqThreshold2 = '<%=reqThreshold2.ClientID %>';
        var cmpThresholdVal1 = '<%=compThresholdVal1.ClientID %>';
        var cmpThresholdVal2 = '<%=compThresholdVal2.ClientID %>';
        var cmpDateVal1min = '<%=cmpDate1min.ClientID %>';
        var cmpDateVal2min = '<%=cmpDate2min.ClientID %>';
        var cmpDateVal1max = '<%=cmpDate1max.ClientID %>';
        var cmpDateVal2max = '<%=cmpDate2max.ClientID %>';
        var dateRangeGrid = '<%=pnlDateRanges.ClientID %>';
        var thresholdGrid = '<%=pnlThresholdGrid.ClientID %>';
        var ddlFinancialYear = '<%= ddlFinancialYear.ClientID %>';
    </script>
        
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
            <asp:ScriptReference Path="~/expenses/javaScript/vehicleJourneyRates.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>
	<div class="formpanel formpanel_padding">
	    <div class="sectiontitle"><asp:Label id="lblDetails" runat="server" meta:resourcekey="lblDetailsResource1">General Details</asp:Label></div>
	    <div class="twocolumn">
	        <asp:Label id="lblmcategory" runat="server" meta:resourcekey="lblmcategoryResource1" Text="Vehicle Journey Rate Category" AssociatedControlID="txtcarsize"></asp:Label><span class="inputs"><asp:TextBox id="txtcarsize" runat="server" MaxLength="50" meta:resourcekey="txtcarsizeResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqmileagename" runat="server" ErrorMessage="Please enter a name for this Vehicle Journey Rate Category" ControlToValidate="txtcarsize" meta:resourcekey="reqmileagenameResource1" 
                ValidationGroup="vgMain">*</asp:RequiredFieldValidator></span><asp:Label id="lblThreshold" runat="server" meta:resourcekey="lblOptAnnualResource1" Text="Threshold Type" AssociatedControlID="cmbThreshold"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbThreshold" runat="server">
                        <asp:ListItem Value="0">Annual</asp:ListItem>
                        <asp:ListItem Value="1">Per Journey</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip359" onclick="SEL.Tooltip.Show('de2f21c4-abf0-4d19-91f7-41170ba3b71a', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></span><span class="inputvalidatorfield"></span></div>
        <div class="onecolumn">
            <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>        						
        <div class="onecolumnsmall">
            <asp:Label id="lblCurrency" runat="server" meta:resourcekey="lblCurrencyResource1" AssociatedControlID="cmbCurrency" Text="Currency"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbCurrency" runat="server">
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn"><asp:Label id="lblUom" runat="server" meta:resourcekey="lblUomResource1" AssociatedControlID="cmbUom" Text="Unit of measure"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbUom" runat="server">
                        <asp:ListItem Value="0">Miles</asp:ListItem>
                        <asp:ListItem Value="1">Kilometres</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label id="lblcalcmilestotal" runat="server" meta:resourcekey="lblcalcmilestotalResource1" Text="Calculate new journey total" AssociatedControlID="chkcalcmilestotal"></asp:Label><span class="inputs"><asp:CheckBox id="chkcalcmilestotal" runat="server" meta:resourcekey="chkcalcmilestotalResource1"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip360" onclick="SEL.Tooltip.Show('986e06f3-1de9-4077-82e9-d6ae9de160be', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></span><span class="inputvalidatorfield"></span></div> 
        <asp:Panel ID="pannhs" runat="server">
        <div class="sectiontitle">NHS Mileage</div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblnhscode" Text="NHS Mileage Code" AssociatedControlID="txtnhscode"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtnhscode"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqnhscode" ValidationGroup="vgMain" Display="None" ErrorMessage="Please enter a value for NHS mileage code." ControlToValidate="txtnhscode"/></span>
        </div>             
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblnhsstart" Text="Start Engine Size" AssociatedControlID="txtnhsstart"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtnhsstart"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><cc1:FilteredTextBoxExtender runat="server" ID="ftenhsstart" FilterMode="ValidChars" ValidChars="0123456789" TargetControlID="txtnhsstart" /><asp:RequiredFieldValidator runat="server" ID="reqnhsstart" ValidationGroup="vgMain" Display="None" ErrorMessage="Please enter a value for start engine size." ControlToValidate="txtnhsstart"></asp:RequiredFieldValidator></span><asp:Label runat="server" ID="lblnhsend" Text="End Engine Size" AssociatedControlID="txtnhsend"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtnhsend"></asp:TextBox><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpenginesize" Type="Integer" ControlToValidate="txtnhsend" ControlToCompare="txtnhsstart" Operator="GreaterThanEqual" ValidationGroup="vgMain" ErrorMessage="Start engine size must be less than end engine size." Display="None"></asp:CompareValidator><asp:RequiredFieldValidator runat="server" ID="reqnhsend" ValidationGroup="vgMain" Display="None" ErrorMessage="Please enter a value for end engine size." ControlToValidate="txtnhsend"/><cc1:FilteredTextBoxExtender runat="server" ID="ftenhsend" FilterMode="ValidChars" ValidChars="0123456789" TargetControlID="txtnhsend" /></span></span>
            </div></asp:Panel>
        <div class="sectiontitle"><asp:Label id="Label7" runat="server" >Financial Year</asp:Label></div>                    
        <div class="onecolumnsmall">
            <asp:DropDownList runat="server" ID="ddlFinancialYear"></asp:DropDownList>
        </div>
        <div class="sectiontitle"><asp:Label id="lblRange" runat="server" meta:resourcekey="lblRangeResource1">Date Ranges</asp:Label></div>                    
        <asp:Literal runat="server" ID="litAddDateRange"></asp:Literal>
        
        <asp:Panel ID="pnlDateRanges" runat="server">
        <asp:Literal ID="litDateRangeGrid" runat="server"></asp:Literal>
        </asp:Panel>
<div class="formbuttons"><a href="javascript:SEL.VehicleJourneyRate.CurrentAction = 'OK'; SEL.VehicleJourneyRate.Save();"><img border="0" src="../../shared/images/buttons/btn_save.png" alt="Save" /></a>&nbsp;&nbsp;<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
        
        <asp:Panel ID="pnlDateRange" runat="server" class="normal-font">
        <div class="formpanel">
            <div class="sectiontitle"><asp:Label id="Label5" runat="server" meta:resourcekey="Label1Resource1">Date Range Details</asp:Label></div>
            <div class="twocolumn"><asp:Label id="lblDateRangeType" runat="server" meta:resourcekey="lblDateRangeTypeResource1" Text="Date Range Type" AssociatedControlID="cmbDateRangeType"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbDateRangeType" runat="server" CssClass="fillspan">
            <asp:ListItem Value="0" Text="Before"></asp:ListItem>
            <asp:ListItem Value="1" Text="After or Equal To"></asp:ListItem>
            <asp:ListItem Value="2" Text="Between"></asp:ListItem>
            <asp:ListItem Value="3" Text="Any"></asp:ListItem>
            </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
            <div class="twocolumn"><span id="spanDateThreshold1"><asp:Label id="lbldateval1" runat="server" meta:resourcekey="lbldate1Resource1" Text="Date value 1" AssociatedControlID="txtDateVal1"></asp:Label><span class="inputs"><asp:TextBox id="txtDateVal1" runat="server" CssClass="fillspan dateField"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                            ID="reqDateVal1" runat="server" ErrorMessage="Please enter a value for Date 1 in the box provided" Text="*" ControlToValidate="txtDateVal1" ValidationGroup="vgDateRange"></asp:RequiredFieldValidator><asp:CompareValidator
                                ID="compdate1" runat="server" ErrorMessage="The value you have entered for Date 1 is invalid" ControlToValidate="txtDateVal1" ValidationGroup="vgDateRange" Type="Date" Operator="DataTypeCheck" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate1min" ControlToValidate="txtDateVal1" Type="Date" Operator="GreaterThanEqual" ValueToCompare="01/01/1900" ValidationGroup="vgDateRange" Text="*" ErrorMessage="Date Value 1 must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate1max" ControlToValidate="txtDateVal1" Type="Date" Operator="LessThanEqual" ValidationGroup="vgDateRange" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Date Value 1 must be on or before 31/12/3000"></asp:CompareValidator></span></span><span id="spanDateThreshold2"><asp:Label id="lbldateval2" runat="server" meta:resourcekey="lbldateval2Resource1" Text="Date value 2" AssociatedControlID="txtDateVal2"></asp:Label><span class="inputs"><asp:TextBox id="txtDateVal2" runat="server" CssClass="dateField"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                            ID="reqDateVal2" runat="server" ErrorMessage="Please enter a value for Date 2 in the box provided" Text="*" ControlToValidate="txtDateVal2" ValidationGroup="vgDateRange"></asp:RequiredFieldValidator><asp:CompareValidator
                                ID="CompareValidator2" runat="server" ErrorMessage="The value you have entered for Date 2 is invalid" ControlToValidate="txtDateVal2" ValidationGroup="vgDateRange" Type="Date" Operator="DataTypeCheck" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate2min" Operator="GreaterThanEqual" Type="Date" ControlToValidate="txtDateVal2" ValidationGroup="vgDateRange" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Date Value 2 must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate2max" Type="Date" Operator="LessThanEqual" ControlToValidate="txtDateVal2" ValidationGroup="vgDateRange" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Date Value 2 must be on or before 31/12/3000"></asp:CompareValidator></span></span></div>
            <div class="sectiontitle"><asp:Label ID="Label1" runat="server" Text="Thresholds"></asp:Label></div>    
            <a id="lnkAddThreshold" href="javascript:SEL.VehicleJourneyRate.CurrentThresholdId = 0; SEL.VehicleJourneyRate.DateRange.Threshold.ShowModal(true);">Add Threshold</a>          
            <asp:Panel ID="pnlThresholdGrid" runat="server">
                <asp:Literal ID="litThresholds" runat="server"></asp:Literal>		
            </asp:Panel>
            <div class="formbuttons"><a href="javascript:SEL.VehicleJourneyRate.CurrentAction = 'DateRangeOK'; SEL.VehicleJourneyRate.DateRange.Save();"><img src="../../shared/images/buttons/btn_save.png" alt="Save" /></a>&nbsp;&nbsp;<asp:ImageButton
                    ID="cmddaterangecancel" OnClientClick="SEL.VehicleJourneyRate.DateRange.HideModal();" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" /></div>                    
        </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlthreshold" runat="server" class="normal-font">
        <div class="formpanel">
            <div class="sectiontitle"><asp:Label id="Label2" runat="server" meta:resourcekey="Label1Resource1">Threshold Details</asp:Label></div>     
            <div class="twocolumn"><asp:Label id="lblThresholdType" runat="server" meta:resourcekey="lblThresholdTypeResource1" Text="Threshold Range Type" AssociatedControlID="cmbRangeType"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbRangeType" runat="server">
                        </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
            <div class="twocolumn"><span id="spanThreshold1"><asp:Label id="lblThresholdVal1" runat="server" meta:resourcekey="lblThresholdVal1Resource1" Text="Threshold value 1" AssociatedControlID="txtThresholdVal1"></asp:Label><span class="inputs"><asp:TextBox id="txtThresholdVal1" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                    ID="reqThreshold1" runat="server"  ErrorMessage="Please enter a value for threshold 1 in the box provided" Text="*" ControlToValidate="txtThresholdVal1" ValidationGroup="vgThreshold"></asp:RequiredFieldValidator><asp:CompareValidator id="compThresholdVal1" runat="server"
						ControlToValidate="txtThresholdVal1" Type="Currency" Operator="GreaterThan" ValueToCompare="0" ValidationGroup="vgThreshold" Text="*" ErrorMessage="Please enter a valid value for threshold 1 in the box provided">*</asp:CompareValidator></span></span><span id="spanThreshold2"><asp:Label id="lblThresholdVal2" runat="server" meta:resourcekey="lblThresholdVal2Resource1" Text="Threshold value 2" AssociatedControlID="txtThresholdVal2"></asp:Label><span class="inputs"><asp:TextBox id="txtThresholdVal2" runat="server" MaxLength="15"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                        ID="reqThreshold2" runat="server" ErrorMessage="Please enter a value for threshold 2 in the box provided" Text="*" ControlToValidate="txtThresholdVal2" ValidationGroup="vgThreshold"></asp:RequiredFieldValidator><asp:CompareValidator id="compThresholdVal2" runat="server"
						ControlToValidate="txtThresholdVal2" Type="Currency" Operator="GreaterThan" ValueToCompare="0" ValidationGroup="vgThreshold" Text="*" ErrorMessage="Please enter a valid value for threshold 2 in the box provided">*</asp:CompareValidator></span></span></div>
            
            <div class="sectiontitle">Fuel Rates</div>
            <a href="javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Rate.Edit(null);">Add Fuel rate</a>
            <asp:Literal ID="litRatesGrid" runat="server" />
            
            <div class="sectiontitle"><asp:Label ID="Label4" runat="server" Text="Additional Rates"></asp:Label></div>		
            <div class="twocolumn"><asp:Label id="lblPassenger" runat="server" AssociatedControlID="txtPassenger" Text="Rate for Passenger 1"></asp:Label><span class="inputs"><asp:TextBox id="txtPassenger" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator id="compPass" runat="server" ErrorMessage="Please enter a valid value for Rate for Passenger 1"
						ControlToValidate="txtPassenger" Type="Double" Operator="DataTypeCheck" ValidationGroup="vgThreshold">*</asp:CompareValidator></span><asp:Label id="lblPassengerx" runat="server" AssociatedControlID="txtPassengerx" Text="Rate for other passengers"></asp:Label><span class="inputs"><asp:TextBox id="txtPassengerx" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator id="compPassx" runat="server" ErrorMessage="Please enter a valid value for Rate for other Passengers"
						ControlToValidate="txtPassengerx" Type="Double" Operator="DataTypeCheck" ValidationGroup="vgThreshold">*</asp:CompareValidator></span></div>				
            <div class="twocolumn"><asp:Label id="Label6" runat="server" AssociatedControlID="txtHeavyBulky" Text="Rate for Heavy and Bulky Goods"></asp:Label><span class="inputs"><asp:TextBox id="txtHeavyBulky" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator id="CompareValidator1" runat="server" ErrorMessage="Please enter a valid value for Rate for Heavy and Bulky Goods"
						ControlToValidate="txtHeavyBulky" Type="Double" Operator="DataTypeCheck" ValidationGroup="vgThreshold">*</asp:CompareValidator></span><span class="inputs"></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">
						</span></div>										
            
            
            <div class="formbuttons">
                <asp:Image ID="Image1"
                    onclick="SEL.VehicleJourneyRate.DateRange.Threshold.Save();" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" />&nbsp;&nbsp;<asp:Image
                    ID="cmdthresholdcancel" onclick="SEL.VehicleJourneyRate.DateRange.Threshold.HideModal();" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" />
            </div>						
        </div>
    

    </asp:Panel>
    
        <asp:Panel ID="pnlThresholdRate" runat="server" class="normal-font">
            <div class="formpanel">
                
                <div class="sectiontitle"><asp:Label ID="Label3" runat="server">Fuel Rate</asp:Label></div>     

                <div class="twocolumn">
                    <asp:Label ID="Label8" runat="server" AssociatedControlID="ddlVehicleEngineType" CssClass="mandatory">
                        Vehicle engine type*
                    </asp:Label><span class="inputs">
                        <asp:DropDownList ID="ddlVehicleEngineType" runat="server"/>
                    </span><span  class="inputicon">
                    </span><span class="inputtooltipfield">
                        <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.VehicleJourneyRate.DateRange.Threshold.Rate.HELP.VEHICLE_ENGINE_TYPE, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" style="border: none;" />
                    </span><span class="inputvalidatorfield">
                    </span>
                </div>

                <div class="twocolumn">
                    <asp:Label ID="Label9" runat="server" AssociatedControlID="txtRatePerUnit" CssClass="mandatory">
                        Rate per mile/KM*
                    </asp:Label><span class="inputs">
                        <asp:TextBox ID="txtRatePerUnit" runat="server" CssClass="4dp" />
                    </span><span  class="inputicon">
                    </span><span class="inputtooltipfield">
                        <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.VehicleJourneyRate.DateRange.Threshold.Rate.HELP.RATE_PER_UNIT, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" style="border: none;" />
                    </span><span class="inputvalidatorfield">
                    </span>
                </div>
            
                <div class="twocolumn">
                    <asp:Label ID="Label10" runat="server" AssociatedControlID="txtAmountForVat" CssClass="mandatory">
                        Amount for VAT*
                    </asp:Label><span class="inputs">
                        <asp:TextBox ID="txtAmountForVat" runat="server" CssClass="4dp" />
                    </span><span  class="inputicon">
                    </span><span class="inputtooltipfield">
                        <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.VehicleJourneyRate.DateRange.Threshold.Rate.HELP.AMOUNT_FOR_VAT, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" style="border: none;" />
                    </span><span class="inputvalidatorfield">
                    </span>
                </div>
            
                <div class="formbuttons">
                    <a href="javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Rate.Save();"><img
                        border="0" src="../../shared/images/buttons/btn_save.png" alt="Save" /></a>&nbsp;
                    <asp:Image ID="btnThresholdRateCancel" onclick="javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Rate.HideModal();" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
                </div>						
            </div>
    
        </asp:Panel>
    
	</div>
    
    <script type="text/javascript">
        SEL.VehicleJourneyRate.CONTENT_MAIN = '<%=this.cmdcancel.Parent.ClientID%>_';
    </script>

    <tooltip:tooltip runat="server" />
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".formpanel .sectiontitle:first").css("margin-top", 0);
        });
    </script>
    
</asp:Content>