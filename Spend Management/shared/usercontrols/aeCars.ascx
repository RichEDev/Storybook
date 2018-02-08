<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="aeCars.ascx.cs" Inherits="Spend_Management.aeCars" %>
<%@ Register Src="UploadAttachment.ascx" TagName="UploadCarDocAttachment" TagPrefix="uc1" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %><?xml version="1.0" encoding="iso-8859-1"?>

<script type="text/javascript" language="javascript">
    var txtmake = '<%=txtmake.ClientID %>';
    var txtmodel = '<%=txtmodel.ClientID %>';
    var txtregistration = '<%=txtregno.ClientID %>';
    var cmbUom = '<%=cmbUom.ClientID %>';
    var chkactive = '<%=chkactive.ClientID %>';
    var cmbcartype = '<%=cmbcartype.ClientID %>';
    var chkodometerreading = '<%=chkfuelcard.ClientID %>';
    var txtstartodo = '<%=txtodometer.ClientID %>';
    var txtendodo = '<%=txtendodometer.ClientID %>';
    var txtenginesize = '<%=txtEngineSize.ClientID %>';
   <%-- var txttaxexpiry = '<%=txttaxexpiry.ClientID %>';
    var txttaxlastchecked = '<%=txttaxlastchecked.ClientID %>';
    var txtmottestnumber = '<%=txtmottestnumber.ClientID %>';
    var txtmotexpiry = '<%=txtmotexpiry.ClientID %>';
    var txtmotlastchecked = '<%=txtmotlastchecked.ClientID %>';
    var txtinsurancenumber = '<%=txtinsurancenumber.ClientID %>';
    var txtinsuranceexpiry = '<%=txtinsuranceexpiry.ClientID %>';
    var txtinsurancelastchecked = '<%=txtinsurancelastchecked.ClientID %>';
    var txtserviceexpiry = '<%=txtserviceexpiry.ClientID %>';
    var txtservicelastchecked = '<%=txtservicelastchecked.ClientID %>';--%>
    var txtstart = '<%=dtstart.ClientID %>';
    var txtend = '<%=dtend.ClientID %>';
    var chkexemptfromhometooffice = '<%=chkexemptfromhometooffice.ClientID %>';
    var modAttachmentID = '<% = modCarDocAttachment.ClientID %>';
    var pnlodogrid = '<% =pnlOdoGrid.ClientID %>';
    var pnlesrdetail = '<% =this.pnlEsrDetail.ClientID %>';
    var modOdoReadingID = '<%=modOdoReading.ClientID %>';
    var txtReadingDateID = '<%=txtReadingDate.ClientID %>';
    var txtOldReadingID = '<%=txtOldReading.ClientID %>';
    var txtNewReadingID = '<%=txtNewReading.ClientID %>';
    var cmdSaveID = '<%=cmdSave.ClientID %>';
    var mileageGrid = '<%=litmileagegrid.ClientID %>';
    var carTabContainer = '<%=TabContainer1.ClientID %>';
    var carEsrTabPage = '<%=tabEsr.ClientID %>';
    var ddlFinancialYear = '<%=ddlFinancialYear.ClientID %>';
    var cmbvehicletype = '<%=cmbvehicletype.ClientID %>';

    var reqreg = '<%=reqreg.ClientID %>';
    var rfEngineSize = '<%=rfEngineSize.ClientID %>';
    var cvEngineSize = '<%=cvEngineSize.ClientID %>';
    var rvEngineType = '<%=rvEngineType.ClientID %>';

    var rvVehicleType = '<%=rvVehicleType.ClientID %>';
    var reqmake = '<%=reqmake.ClientID %>';
    var reqmodel = '<%=reqmodel.ClientID %>';



</script>


    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcCars.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/cars.js?date=20180206" />
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js" />
            <asp:ScriptReference Path="~/shared/javaScript/employees.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">

    function addReading()
    {
            var tbl;
            var newrow;
            var cell;
            tbl = document.getElementById('tblodo');

            newrow = tbl.insertRow();
            newrow.id = 'odo' + readingsused;
            cell = newrow.insertCell();
            cell.innerHTML = '<a href="javascript:deleteReading(' + readingsused + ');"><img src="../icons/delete2.gif"></a>';
            cell = newrow.insertCell();
            cell.innerHTML = '<input onblur="validateItem(\'readingdate' + readingsused + '\',4,\'Reading Date\');" id=readingdate' + readingsused + ' name="readingdate" type=textbox value="">';
            cell = newrow.insertCell();
            cell.innerHTML = '<input onblur="validateItem(\'oldreading' + readingsused + '\',2,\'Old Reading\');" id=oldreading' + readingsused + ' name="oldreading" type=textbox value="">';
            cell = newrow.insertCell();
            cell.innerHTML = '<input onblur="validateItem(\'newreading' + readingsused + '\',2,\'New Reading\');" id=newreading' + readingsused + ' name="newreading" type=textbox value="">';
            readingsused++;

        }
    function deleteReading(odometerid)
    {
	    if (confirm('Are you sure you wish to delete the selected reading?'))
	    {
                var i;
                var rowid;
                var tbl;

                tbl = document.getElementById('tblodo');

		    for (i = 0; i < tbl.rows.length; i++)
		    {
                    rowid = 'odo' + odometerid;

			    if (tbl.rows[i].id == rowid)
			    {
                        tbl.deleteRow(i);
                        break;
                    }
                }
            }
        }

        function showDocument(employeeid, carid, documenttype)
        {
            window.open("viewcardocument.aspx?employeeid=" + employeeid + "&carid=" + carid + "&documenttype=" + documenttype, null, 'menubar=no,toolbar=no');
        }

        function OnVehicleTypeChange(sender)
        {
            var ddl = document.getElementById(sender);
            var vehicletypeid = ddl.options[ddl.selectedIndex].value;

            

            if (vehicletypeid <= 1) {
                
                document.getElementById("<%=txtregno.ClientID%>").value = '';
                document.getElementById("<%=txtEngineSize.ClientID%>").value = '';
                if (vehicletypeid < 1) {
                    document.getElementById("<%=cmbcartype.ClientID%>").selectedIndex = 0;
                }
            }

            if (vehicletypeid == 1) {
                document.getElementById("<%=reqreg.ClientID%>").style.visibility = "hidden";
                document.getElementById("<%=rfEngineSize.ClientID%>").style.visibility = "hidden";
                document.getElementById("<%=cvEngineSize.ClientID%>").style.visibility = "hidden";
              <%--  document.getElementById("<%=rvEngineType.ClientID%>").style.visibility = "hidden";--%>

                document.getElementById("<%=reqreg.ClientID%>").enabled = false;
                document.getElementById("<%=rfEngineSize.ClientID%>").enabled = false;
                document.getElementById("<%=cvEngineSize.ClientID%>").enabled = false;
              <%--  document.getElementById("<%=rvEngineType.ClientID%>").enabled = false;--%>

                document.getElementById("<%=txtregno.ClientID%>").disabled = true;
                document.getElementById("<%=txtEngineSize.ClientID%>").disabled = true;
              <%--  document.getElementById("<%=cmbcartype.ClientID%>").disabled = true;--%>
            } else {

                document.getElementById("<%=reqreg.ClientID%>").enabled = true;
                document.getElementById("<%=rfEngineSize.ClientID%>").enabled = true;
                document.getElementById("<%=cvEngineSize.ClientID%>").enabled = true;
                document.getElementById("<%=rvEngineType.ClientID%>").enabled = true;

                document.getElementById("<%=txtregno.ClientID%>").disabled = false;
                document.getElementById("<%=txtEngineSize.ClientID%>").disabled = false;
                document.getElementById("<%=cmbcartype.ClientID%>").disabled = false;
            }
        }

    </script>
<style type="text/css">
    .formpanel .formbuttons IMG {
        float:none;
    }

</style>

<!--[if IE 7]>
<style>
    #<%=TabContainer1.ClientID%> {
        position: relative
    }
</style>
<![endif]-->
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    <cc1:TabContainer ID="TabContainer1" runat="server" ScrollBars="Vertical" Height="370px" Width="100%">
        <cc1:TabPanel ID="tabGenDets" runat="server" HeaderText="General Details" ScrollBars="None">
            <ContentTemplate>
                
                <asp:Panel class="formpanel Doesthisvehicle_padding" ID="replaceCar" runat="server">
                    
                    <div class="sectiontitle">
                        <asp:Label runat="server" Text="Previous Vehicle"></asp:Label>
                    </div>

                    <div class="twocolumn">
                        <asp:Label runat="server" AssociatedControlID="rdoReplace" CssClass="mandatory">Does this vehicle replace a previous one?</asp:Label>
                           <span class="inputs">
                            <asp:RadioButtonList ID="rdoReplace" CssClass="radio replaceCar" runat="server" RepeatLayout="UnorderedList">
                                <asp:ListItem Text="" Value="1"><span style="float: left; margin-right: 10px;">Yes</span></asp:ListItem>
                                <asp:ListItem Text="" Value="0"><span style=" float: left; margin-right: 10px;">No</span></asp:ListItem>
                            </asp:RadioButtonList>
                        </span><span class="inputicon"></span><span class="inputtooltipfield"><img class="tooltipicon" onclick="SEL.Tooltip.Show('960A504C-9F9B-4CE3-ABAF-CCD1E0D05595', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png"/></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="rdoReplace" ErrorMessage="Please specify whether or not this vehicle replaces a previous one" meta:resourcekey="reqmakeResource1" ValidationGroup="ValidationSummaryAeCar">*</asp:RequiredFieldValidator></span>
                    </div>

                    <div class="onecolumnsmall hidden previousCar">
                        <asp:Label runat="server" AssociatedControlID="cmbPreviousCar" CssClass="mandatory">Previous vehicle</asp:Label><span class="inputs">
                            <asp:DropDownList ID="cmbPreviousCar" runat="server" CssClass="fillspan" ValidationGroup="ValidationSummaryAeCar"></asp:DropDownList>
                        </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>

                </asp:Panel>
                    
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="Label7" runat="server" Text="Vehicle Details"></asp:Label>
                    </div>
                    
                    <div class="twocolumn">
                        <asp:Label ID="lblregno" runat="server" AssociatedControlID="txtregno" CssClass="mandatory" meta:resourcekey="lblregnoResource1">Registration Number*</asp:Label><span class="inputs"><asp:TextBox ID="txtregno" runat="server" CssClass="fillspan registrationNumber" meta:resourceKey="txtregnoResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox></span><span id="divLookupContainer" class="inputs" runat="server"><img src="/static/icons/16/new-icons/find.png" alt="Cancel" id="imgLookup" onclick="javascript:LookupVehicleDetails();"/></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqreg" runat="server" ControlToValidate="txtregno" ErrorMessage="Please enter the registration number of this vehicle" meta:resourceKey="reqregResource1" ValidationGroup="ValidationSummaryAeCar">*</asp:RequiredFieldValidator></span>
                        
                        <span id="divWait" style="display: none;">
                            <img alt="searching" src="/shared/images/ajax-loader.gif" style="margin-right: 8px;" height ="12"/>
                        </span>
                        <label id="lookupError" style="color:red" ></label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label8" runat="server" AssociatedControlID="txtmake" meta:resourcekey="lblmakeResource1" CssClass="mandatory">Make*</asp:Label><span class="inputs"><asp:TextBox ID="txtmake" runat="server" CssClass="fillspan" meta:resourcekey="txtmakeResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqmake" runat="server" ControlToValidate="txtmake" ErrorMessage="Please enter the make of this vehicle" meta:resourcekey="reqmakeResource1" ValidationGroup="ValidationSummaryAeCar">*</asp:RequiredFieldValidator></span>
                        <asp:Label ID="Label9" runat="server" AssociatedControlID="txtmodel" CssClass="mandatory" meta:resourcekey="lblmodelResource1">Model*</asp:Label><span class="inputs"><asp:TextBox ID="txtmodel" runat="server" CssClass="fillspan" meta:resourcekey="txtmodelResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqmodel" runat="server" ControlToValidate="txtmodel" ErrorMessage="Please enter the model of this vehicle" meta:resourcekey="reqmodelResource1" ValidationGroup="ValidationSummaryAeCar">*</asp:RequiredFieldValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label10" runat="server" meta:resourcekey="lblvehicletypeResource1" AssociatedControlID="cmbvehicletype" CssClass="mandatory">Vehicle Type*</asp:Label><span class="inputs"><asp:DropDownList ID="cmbvehicletype" runat="server" CssClass="fillspan" meta:resourceKey="cmbvehicletypeResource1" ValidationGroup="ValidationSummaryAeCar" onchange="OnVehicleTypeChange(this.id)" OnSelectedIndexChanged="cmbvehicletype_SelectedIndexChanged"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="rvVehicleType" runat="server" ControlToValidate="cmbvehicletype" ErrorMessage="Please select the vehicle type." Operator="GreaterThan" Type="Integer" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="0">*</asp:CompareValidator></span>
                        <asp:Label ID="lblcartype" runat="server" meta:resourcekey="lblcartypeResource1" AssociatedControlID="cmbcartype" CssClass="mandatory">Engine Type*</asp:Label><span class="inputs"><asp:DropDownList ID="cmbcartype" runat="server" CssClass="fillspan" onchange="javascript:filterMileageGrid();" meta:resourceKey="cmbcartypeResource1" ValidationGroup="ValidationSummaryAeCar" ></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="rvEngineType" runat="server" ControlToValidate="cmbcartype" ErrorMessage="Please select the engine type." Operator="GreaterThan" Type="Integer" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="0">*</asp:CompareValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label5" runat="server" AssociatedControlID="txtEngineSize" CssClass="mandatory" Text="Engine Size (cc)*"></asp:Label><span class="inputs"><asp:TextBox ID="txtEngineSize" runat="server" CssClass="fillspan" ValidationGroup="ValidationSummaryAeCar" MaxLength="5"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip383" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('7fd6e1ed-a9fc-48bc-a6c4-7e05456d8645', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfEngineSize" runat="server" ControlToValidate="txtEngineSize" ErrorMessage="Please enter the engine size (in cc's)" ValidationGroup="ValidationSummaryAeCar">*</asp:RequiredFieldValidator><asp:CompareValidator ID="cvEngineSize" runat="server" ControlToValidate="txtEngineSize" ErrorMessage="Engine size must contain the digits 0-9 only." Operator="DataTypeCheck" Text="*" Type="Integer" ValidationGroup="ValidationSummaryAeCar" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator ID="cvEngineSizeValue" runat="server" ControlToValidate="txtEngineSize" ErrorMessage="The engine size must be 0 or greater" ValidationGroup="ValidationSummaryAeCar" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0" Display="Dynamic">*</asp:CompareValidator></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label4" runat="server" Text="General Details"></asp:Label>
                    </div>
                    
                    <div class="twocolumn">
                        
                        <asp:Label ID="lblUom" runat="server" AssociatedControlID="cmbUom" meta:resourcekey="lblUomResource1">Unit of Measure</asp:Label><span class="inputs"><asp:DropDownList ID="cmbUom" runat="server" CssClass="fillspan" onchange="javascript:filterMileageGrid();" ValidationGroup="ValidationSummaryAeCar"><asp:ListItem Selected="True" Value="0">Miles</asp:ListItem><asp:ListItem Value="1">Kilometres</asp:ListItem></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    
                    <div class="twocolumn">
                        <asp:Panel ID="pnlActive" runat="server">
                            <asp:Label ID="lblactive" runat="server" AssociatedControlID="chkactive" meta:resourcekey="lblactiveResource1">Vehicle is active</asp:Label><span class="inputs"><asp:CheckBox ID="chkactive" runat="server" meta:resourcekey="chkactiveResource1"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltip391" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('636eb0ed-1b27-4cde-ad6e-d67b0c86b8f5', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" style="margin-top:-15px;" /></span><span class="inputvalidatorfield"></span>
                            <span id="exemptFromHomeToOffice" runat="server"><asp:Label ID="Label2" runat="server" AssociatedControlID="chkexemptfromhometooffice" Text="Exempt from 'Home to Location' Mileage"></asp:Label><span class="inputs"><asp:CheckBox ID="chkexemptfromhometooffice" runat="server"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></span>
                        </asp:Panel>
                    </div>
                    <asp:Panel id="pnlDates" runat="server">
                        <div class="sectiontitle" style="margin-top:20px; display: none;">
                            <asp:Label ID="lblStartEndDateTitle" runat="server" meta:resourcekey="Label3Resource1">Start / End Date of Vehicle Usage</asp:Label>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="lblstart" runat="server" AssociatedControlID="dtstart" meta:resourcekey="lblstartResource1">Start Date</asp:Label><span class="inputs"><asp:TextBox ID="dtstart" runat="server" CssClass="fillspan" meta:resourcekey="dtstartResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskstart" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="dtstart"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calstart" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgstart" TargetControlID="dtstart"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgstart" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgstartResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compstart" runat="server" ControlToValidate="dtstart" ErrorMessage="Please enter a valid Start Date" meta:resourcekey="compstartResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartmin" Operator="GreaterThanEqual" Type="Date" ControlToValidate="dtstart" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Start date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartmax" ControlToValidate="dtstart" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Start date must be on or before 31/12/3000"></asp:CompareValidator><asp:RequiredFieldValidator runat="server" ID="reqcarstartdate" ControlToValidate="dtstart" Enabled="False" ErrorMessage="Please enter the start date for this vehicle" ValidationGroup="ValidationSummaryAeCar" Text="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                            <asp:PlaceHolder runat="server" ID="phCarEndDate"><asp:Label ID="lblend" runat="server" AssociatedControlID="dtend" meta:resourcekey="lblendResource1">End Date</asp:Label><span class="inputs"><asp:TextBox ID="dtend" runat="server" CssClass="fillspan" meta:resourcekey="dtendResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskend" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="dtend"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calend" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgend" TargetControlID="dtend"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgend" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgendResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compend" runat="server" ControlToValidate="dtend" ErrorMessage="Please enter a valid end date" meta:resourcekey="compendResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpendmin" ControlToValidate="dtend" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="End date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpendmax" ControlToValidate="dtend" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="End date must on or before 31/12/3000"></asp:CompareValidator></span></asp:PlaceHolder>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlMileageCats" runat="server">                              
                        <div class="formpanel formpanel_left" >                      
                        <div class="sectiontitle">
                            <asp:Label ID="Label6" runat="server" Text="Vehicle Journey Rates"></asp:Label>
                        </div>
                        <div class="twocolumn"><asp:Label ID="Label1" runat="server" AssociatedControlID="ddlFinancialYear" >Financial Year</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlFinancialYear" onchange="javascript:filterMileageGrid();"></asp:DropDownList></span></div>
                        <div ID="litmileagegrid" runat="server"></div>
                        </div>
                            </asp:Panel>
                    <div class="formpanel formpanel_padding">
                    <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder>
                </div>
                </div>
</ContentTemplate>
</cc1:TabPanel>
        <%--<cc1:TabPanel ID="tabDutyOfCare" runat="server" HeaderText="Duty of Care">
            <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="lbltaxdetails" runat="server" meta:resourcekey="lbltaxdetailsResource1" Text="Tax Details"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lbltaxexpirydate" runat="server" AssociatedControlID="txttaxexpiry" meta:resourcekey="lbltaxexpirydateResource1" Text="Expiry Date"></asp:Label><span class="inputs"><asp:TextBox ID="txttaxexpiry" runat="server" CssClass="fillspan" meta:resourcekey="txttaxexpiryResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="msktaxexpiry" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txttaxexpiry"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="caltaxexpiry" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgtaxexpiry" TargetControlID="txttaxexpiry"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgtaxexpiry" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgtaxexpiryResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="comptaxexpiry" runat="server" ControlToValidate="txttaxexpiry" ErrorMessage="The tax expiry date you have entered is invalid" meta:resourcekey="comptaxexpiryResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmptaxexpirymin" ControlToValidate="txttaxexpiry" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Tax expiry date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmptaxexpirymax" ControlToValidate="txttaxexpiry" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Tax expiry date must be on or before 31/12/3000" Font-Bold="False"></asp:CompareValidator></span>
                        <asp:Label ID="lbltaxdocument" runat="server" AssociatedControlID="holdertax" meta:resourcekey="lbltaxdocumentResource1" Text="Document"></asp:Label><span class="inputs"><div id="taxDiv"><asp:PlaceHolder ID="holdertax" runat="server"></asp:PlaceHolder></div></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn" id="TaxCheckFields">
                        <asp:Label ID="lbltaxlastchecked" runat="server" AssociatedControlID="txttaxlastchecked" meta:resourcekey="lbltaxlastcheckedResource1" Text="Last Checked"></asp:Label><span class="inputs"><asp:TextBox ID="txttaxlastchecked" runat="server" CssClass="fillspan" meta:resourcekey="txttaxlastcheckedResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="msktaxlastchecked" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txttaxlastchecked"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="caltaxlastchecked" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgtaxlastchecked" TargetControlID="txttaxlastchecked"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgtaxlastchecked" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgtaxlastcheckedResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="comptaxlastchecked" runat="server" ControlToValidate="txttaxlastchecked" ErrorMessage="The Tax Last Checked date you have entered is invalid" meta:resourcekey="comptaxlastcheckedResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmptaxlastcheckedmin" ControlToValidate="txttaxlastchecked" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Tax last checked date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmptaxlastcheckedmax" ControlToValidate="txttaxlastchecked" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Tax last checked date must be on or before 31/12/3000"></asp:CompareValidator></span>
                        <asp:PlaceHolder ID="taxCheckedBy" runat="server"></asp:PlaceHolder><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="lblmotdetails" runat="server" meta:resourcekey="lblmotdetailsResource1" Text="MOT Details1"></asp:Label></div>
                    <div class="twocolumn">
                        <asp:Label ID="lblmottestnumber" runat="server" AssociatedControlID="txtmottestnumber" meta:resourcekey="lblmottestnumberResource1" Text="MOT Test Number"></asp:Label><span class="inputs"><asp:TextBox ID="txtmottestnumber" runat="server" CssClass="fillspan" meta:resourcekey="txtmottestnumberResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblmotexpiry" runat="server" AssociatedControlID="txtmotexpiry" meta:resourcekey="lblmotexpiryResource1" Text="Expiry Date"></asp:Label><span class="inputs"><asp:TextBox ID="txtmotexpiry" runat="server" CssClass="fillspan" meta:resourcekey="txtmotexpiryResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskmotexpiry" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtmotexpiry"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calmotexpiry" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgmotexpiry" TargetControlID="txtmotexpiry"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgmotexpiry" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgmotexpiryResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compmotexpiry" runat="server" ControlToValidate="txtmotexpiry" ErrorMessage="The MOT expiry date you have entered is invalid" meta:resourcekey="compmotexpiryResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmotexpirymin" ControlToValidate="txtmotexpiry" Operator="GreaterThanEqual" Type="Date" ValueToCompare="01/01/1900" Text="*" ErrorMessage="MOT expiry date must be on or after 01/01/1900" ValidationGroup="ValidationSummaryAeCar"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmotexpirymax" ControlToValidate="txtmotexpiry" Operator="LessThanEqual" Type="Date" Text="*" ValueToCompare="31/12/3000" ValidationGroup="ValidationSummaryAeCar" ErrorMessage="MOT expiry date must on or before 31/12/3000"></asp:CompareValidator></span>
                    </div>
                    <div class="twocolumn" id="MOTCheckFields">
                        <asp:Label ID="lblmotlastchecked" runat="server" AssociatedControlID="txtmotlastchecked" meta:resourcekey="lblmotlastcheckedResource1" Text="Last Checked"></asp:Label><span class="inputs"><asp:TextBox ID="txtmotlastchecked" runat="server" CssClass="fillspan" meta:resourcekey="txtmotlastcheckedResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskmotlastchecked" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtmotlastchecked"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calmotlastchecked" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgmotlastchecked" TargetControlID="txtmotlastchecked"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgmotlastchecked" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgmotlastcheckedResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compmotlastchecked" runat="server" ControlToValidate="txtmotlastchecked" ErrorMessage="The MOT Last Checked date you have entered is invalid" meta:resourcekey="compmotlastcheckedResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmotlastcheckedmin" ControlToValidate="txtmotlastchecked" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="MOT last checked date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmotlastcheckedmax" ControlToValidate="txtmotlastchecked" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="MOT last checked date must be on or before 31/12/3000"></asp:CompareValidator></span>
                        <asp:PlaceHolder ID="motCheckedBy" runat="server"></asp:PlaceHolder><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblmotdocument" runat="server" AssociatedControlID="holdermot" meta:resourcekey="lblmotdocumentResource1" Text="Document"></asp:Label><span class="inputs"><div id="MOTDiv"><asp:PlaceHolder ID="holdermot" runat="server"></asp:PlaceHolder></div></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="lblinsurancedetails" runat="server" meta:resourcekey="lblinsurancedetailsResource1" Text="Insurance Details"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblinsurancenumber" runat="server" AssociatedControlID="txtinsurancenumber" meta:resourcekey="lblinsurancenumberResource1" Text="Insurance Number"></asp:Label><span class="inputs"><asp:TextBox ID="txtinsurancenumber" runat="server" CssClass="fillspan" meta:resourcekey="txtinsurancenumberResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblinsuranceexpiry" runat="server" AssociatedControlID="txtinsuranceexpiry" meta:resourcekey="lblinsuranceexpiryResource1" Text="Expiry Date"></asp:Label><span class="inputs"><asp:TextBox ID="txtinsuranceexpiry" runat="server" CssClass="fillspan" meta:resourcekey="txtinsuranceexpiryResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskinsuranceexpiry" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtinsuranceexpiry"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calinsuranceexpiry" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imginsuranceexpiry" TargetControlID="txtinsuranceexpiry"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imginsuranceexpiry" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imginsuranceexpiryResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compinsuranceexpiry" runat="server" ControlToValidate="txtinsuranceexpiry" ErrorMessage="The insurance expiry date you have entered is invalid" meta:resourcekey="compinsuranceexpiryResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpinsuranceexpirymin" ControlToValidate="txtinsuranceexpiry" Type="Date" Operator="GreaterThanEqual" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Insurance expiry date must on or after 01/01/1900" ValidationGroup="ValidationSummaryAeCar"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpinsuranceexpirymax" Type="Date" Operator="LessThanEqual" ControlToValidate="txtinsuranceexpiry" ValueToCompare="31/12/3000" ValidationGroup="ValidationSummaryAeCar" Text="*" ErrorMessage="Insurance expiry date must be on or before 31/12/3000"></asp:CompareValidator></span>
                    </div>
                    <div class="twocolumn" id="InsuranceCheckFields">
                        <asp:Label ID="lblinsurancelastchecked" runat="server" AssociatedControlID="txtinsurancelastchecked" meta:resourcekey="lblinsurancelastcheckedResource1" Text="Last Checked"></asp:Label><span class="inputs"><asp:TextBox ID="txtinsurancelastchecked" runat="server" CssClass="fillspan" meta:resourcekey="txtinsurancelastcheckedResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskinsurancelastchecked" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtinsurancelastchecked"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calinsurancelastchecked" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imginsurancelastchecked" TargetControlID="txtinsurancelastchecked"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imginsurancelastchecked" runat="server" ImageUrl="~/shared/images/icons/cal.gif" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compinsurancelastchecked" runat="server" ControlToValidate="txtinsurancelastchecked" ErrorMessage="The Insurance Last Checked date you have entered is invalid" meta:resourcekey="compinsurancelastcheckedResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmplastcheckedmin" ControlToValidate="txtinsurancelastchecked" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Insurance last check date entered must be on or after 01/01/1900" ValidationGroup="ValidationSummaryAeCar"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmplastcheckedmax" ControlToValidate="txtinsurancelastchecked" ValueToCompare="31/12/3000" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="Insurance last checked date entered must be on or before 31/12/3000" ValidationGroup="ValidationSummaryAeCar"></asp:CompareValidator></span>
                        <asp:PlaceHolder ID="insuranceCheckedBy" runat="server"></asp:PlaceHolder><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        <%--<asp:Label 
                            ID="lblinsurancecheckedby" runat="server" 
                            AssociatedControlID="cmbinsurancecheckedby" 
                            meta:resourcekey="lblinsurancecheckedbyResource1" Text="Checked By"></asp:Label><span 
                            class="inputs"><asp:DropDownList ID="cmbinsurancecheckedby" runat="server" 
                            CssClass="fillspan" meta:resourcekey="cmbinsurancecheckedbyResource1" 
                            ValidationGroup="ValidationSummaryAeCar">
                                    </asp:DropDownList>
                                    </span><span class="inputicon"></span><span 
                            class="inputtooltipfield">
                        </span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblinsurancedocument" runat="server" AssociatedControlID="holderinsurance" meta:resourcekey="lblinsurancedocumentResource1" Text="Document"></asp:Label><span class="inputs"><div id="insuranceDiv"><asp:PlaceHolder ID="holderinsurance" runat="server"></asp:PlaceHolder></div></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="lblservicedetails" runat="server" meta:resourcekey="lblservicedetailsResource1" Text="Service Details"></asp:Label></div>
                    <div class="twocolumn">
                        <asp:Label ID="lblserviceexpiry" runat="server" AssociatedControlID="txtserviceexpiry" meta:resourcekey="lblserviceexpiryResource1" Text="Expiry Date"></asp:Label><span class="inputs"><asp:TextBox ID="txtserviceexpiry" runat="server" CssClass="fillspan" meta:resourcekey="txtserviceexpiryResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskserviceexpiry" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtserviceexpiry"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calserviceexpiry" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgserviceexpiry" TargetControlID="txtserviceexpiry"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgserviceexpiry" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgserviceexpiryResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compserviceexpiry" runat="server" ControlToValidate="txtserviceexpiry" ErrorMessage="The service expiry date you have entered is invalid" meta:resourcekey="compserviceexpiryResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpserviceexpirymin" ControlToValidate="txtserviceexpiry" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Service expiry date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpserviceexpirymax" ControlToValidate="txtserviceexpiry" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Service expiry date must be on or before 31/12/3000"></asp:CompareValidator></span>
                        <asp:Label ID="lblservicedocument" runat="server" AssociatedControlID="holderservice" meta:resourcekey="lblservicedocumentResource1" Text="Document"></asp:Label><span class="inputs"><div id="serviceDiv"><asp:PlaceHolder ID="holderservice" runat="server"></asp:PlaceHolder></div></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn" id="ServiceCheckFields">
                        <asp:Label ID="lblservicelastchecked" runat="server" AssociatedControlID="txtservicelastchecked" meta:resourcekey="lblservicelastcheckedResource1" Text="Last Checked"></asp:Label><span class="inputs"><asp:TextBox ID="txtservicelastchecked" runat="server" CssClass="fillspan" meta:resourcekey="txtservicelastcheckedResource1" ValidationGroup="ValidationSummaryAeCar"></asp:TextBox><cc1:MaskedEditExtender ID="mskservicelastchecked" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtservicelastchecked"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calservicelastchecked" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgservicelastchecked" TargetControlID="txtservicelastchecked"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image ID="imgservicelastchecked" runat="server" ImageUrl="~/shared/images/icons/cal.gif" meta:resourcekey="imgservicelastcheckedResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compservicelastchecked" runat="server" ControlToValidate="txtservicelastchecked" ErrorMessage="The Service Last Checked date you have entered is invalid" meta:resourcekey="compservicelastcheckedResource1" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpservicelastcheckedmin" ControlToValidate="txtservicelastchecked" Operator="GreaterThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Service last checked date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpservicelastcheckedmax" ControlToValidate="txtservicelastchecked" Operator="LessThanEqual" Type="Date" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Service last checked date must be on or before 31/12/3000"></asp:CompareValidator></span>
                        <asp:PlaceHolder ID="serviceCheckedBy" runat="server"></asp:PlaceHolder><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                </div>
        </ContentTemplate>
</cc1:TabPanel>--%>
        <cc1:TabPanel ID="tabOdomter" runat="server" HeaderText="Odometer Readings">
            <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="Label3" runat="server" Text="Odometer Readings"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblfuelcard" runat="server" AssociatedControlID="chkfuelcard" meta:resourcekey="lblfuelcardResource1"><p style="width:200px;margin-top: -5px;">Odometer Reading Required</p></asp:Label><span class="inputs"><asp:CheckBox ID="chkfuelcard" runat="server" meta:resourcekey="chkfuelcardResource1"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield" style="margin-top: -12px;"><img id="imgtooltip392"  alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('4e07edb8-66ba-41b3-a2d4-594853272c9f', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblodometer" runat="server" AssociatedControlID="txtodometer" meta:resourcekey="lblodometerResource1">Start Odometer Reading</asp:Label><span class="inputs"><asp:TextBox ID="txtodometer" runat="server" CssClass="fillspan" meta:resourcekey="txtodometerResource1" ValidationGroup="ValidationSummaryAeCar" MaxLength="7"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtodometer" ErrorMessage="The Start Odometer Reading you have entered is invalid." meta:resourcekey="CompareValidator1Resource1" Operator="DataTypeCheck" Type="Integer" ValidationGroup="ValidationSummaryAeCar">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartodopositive" Type="Integer" Operator="GreaterThanEqual" ControlToValidate="txtodometer" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="0" Text="*" ErrorMessage="Start odometer reading must be greater than or equal to zero"></asp:CompareValidator></span>
                        <asp:Label ID="lblendodometer" runat="server" AssociatedControlID="txtendodometer" meta:resourcekey="lblendodometerResource1">End Odometer Reading</asp:Label><span class="inputs"><asp:TextBox ID="txtendodometer" runat="server" CssClass="fillspan" meta:resourcekey="txtendodometerResource1" ValidationGroup="ValidationSummaryAeCar" MaxLength="7"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compstartodo" runat="server" ControlToValidate="txtendodometer" ErrorMessage="The End Odometer Reading you have entered is invalid." meta:resourcekey="compstartodoResource1" Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CompareValidator ID="compendodogreaterthanstart" runat="server" ErrorMessage="The end odometer reading must be greater than the start odometer reading" ControlToCompare="txtodometer" ControlToValidate="txtendodometer" Operator="GreaterThanEqual" Type="Integer" Text="*" ValidationGroup="ValidationSummaryAeCar"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpendodometerpositive" ControlToValidate="txtendodometer" Type="Integer" ValidationGroup="ValidationSummaryAeCar" ValueToCompare="0" Operator="GreaterThanEqual" Text="*" ErrorMessage="End odometer reading must be greater than or equal to zero"></asp:CompareValidator></span>
                </div>
                </div>
                <div class="formpanel">
                    <asp:Panel ID="pnlOdoGrid" runat="server">
                        <asp:Literal ID="litOdo" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
        </ContentTemplate>
</cc1:TabPanel>
        <cc1:TabPanel runat="server" ID="tabEsr" HeaderText="ESR Details" Visible="False">
            <ContentTemplate>
                <div class="sm_panel">
                    <asp:Panel ID="pnlEsrDetail" runat="server">
                        <asp:Literal ID="litEsr" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
    <div runat="server" ID="pnlBtns">
    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <a href="javascript:saveCar(true);"> 
                <img src="/shared/images/buttons/btn_save.png" alt="Save" id="cmdSave" runat="server" /></a>&nbsp;&nbsp;
                <img src="/shared/images/buttons/cancel_up.gif" alt="Cancel" id="cmdCancel" runat="server" onclick="javascript:navigateTo(false, true);" />
        </div>
    </div>
</div>
    
    <asp:Panel ID="pnlOdo" runat="server"  CssClass="modalpanel" style="display:none;">
        <div class="formpanel">
            <div class="sectiontitle">
                        <asp:Label ID="lblOdoTitle" runat="server" Text="Odometer Reading"></asp:Label></div>
            <div class="twocolumn">
                <asp:Label ID="lblReadingDate" CssClass="mandatory" runat="server" Text="Reading date*" meta:resourcekey="lblReadingDateResource1" AssociatedControlID="txtReadingDate" ></asp:Label><span class="inputs"><asp:TextBox ID="txtReadingDate" runat="server" CssClass="fillspan" meta:resourcekey="txtReadingDateResource1" MaxLength="7"></asp:TextBox><cc1:MaskedEditExtender ID="mskodoreadingdate" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="£" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureName="en-GB" CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="txtReadingDate"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calodoreadingdate" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgodoreadingdate" TargetControlID="txtReadingDate"></cc1:CalendarExtender></span><span class="inputicon"><asp:Image runat="server" ID="imgodoreadingdate" ImageUrl="~/shared/images/icons/cal.gif" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqReadingDate" runat="server" ErrorMessage="Please enter the 'Reading Date' in the box provided" Text="*" ControlToValidate="txtReadingDate" ValidationGroup="vgOdo"></asp:RequiredFieldValidator><asp:CompareValidator ID="compReadingDate" runat="server" ControlToValidate="txtReadingDate" ErrorMessage="The reading date you have entered is invalid" Operator="DataTypeCheck" Type="Date" ValidationGroup="vgOdo">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpodoreadingdatemin" ControlToValidate="txtReadingDate" Operator="GreaterThanEqual" Type="Date" ValidationGroup="vgOdo" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Odometer reading date must be on or after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpodoreadingdatemax" ControlToValidate="txtReadingDate" Operator="LessThanEqual" Type="Date" ValidationGroup="vgOdo" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Odometer reading date must be on or before 31/12/3000"></asp:CompareValidator></span><span class="inputtooltipfield">&nbsp</span>
    	        <asp:Label ID="lblOldReading" CssClass="mandatory" runat="server" Text="Old Reading*" meta:resourcekey="lblOldReadingResource1" AssociatedControlID="txtOldReading" ></asp:Label><span class="inputs"><asp:TextBox ID="txtOldReading" runat="server" CssClass="fillspan" meta:resourcekey="txtOldReadingResource1" MaxLength="7"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqOldReading" runat="server" ErrorMessage="Please enter the Old Odometer Reading in the box provided" Text="*" ControlToValidate="txtOldReading" ValidationGroup="vgOdo"></asp:RequiredFieldValidator><asp:CompareValidator ID="compoldreading" runat="server" ControlToValidate="txtOldReading" ErrorMessage="The value you have entered for Old Reading is invalid" Type="Integer" Operator="GreaterThanEqual" Text="*" ValueToCompare="0" ValidationGroup="vgOdo"></asp:CompareValidator></span><span class="inputtooltipfield">&nbsp</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblNewReading" runat="server" Text="New Reading" meta:resourcekey="lblNewReadingResource1" AssociatedControlID="txtNewReading" ></asp:Label><span class="inputs"><asp:TextBox ID="txtNewReading" runat="server" CssClass="fillspan" meta:resourcekey="txtNewReadingResource1" MaxLength="7"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter the New Odometer Reading in the box provided" Text="*" ControlToValidate="txtNewReading" ValidationGroup="vgOdo"></asp:RequiredFieldValidator><asp:CompareValidator ID="compnewreading" runat="server" ControlToValidate="txtNewReading" ErrorMessage="The value you have entered for New Reading is invalid" Type="Integer" Operator="GreaterThanEqual" Text="*" ValueToCompare="0" ValidationGroup="vgOdo"></asp:CompareValidator><asp:CompareValidator ID="compNewReadingGreaterThanStart" runat="server" ErrorMessage="The new reading must be greater than the old reading" Text="*" Type="Integer" ControlToCompare="txtOldReading" ValidationGroup="vgOdo" ControlToValidate="txtNewReading" Operator="GreaterThanEqual"></asp:CompareValidator></span><span class="inputtooltipfield">&nbsp</span>
            </div>
            <div class="formbuttons"><a href="javascript:saveOdometerReading();"><img src="/shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdOdoReadingCancel"
                runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>
    
        
        <asp:Panel ID="pnlCarDocAttachments" runat="server" CssClass="modalpanel" Style="display: none; width: 880px; height: 140px;">
        <div class="formpanel formpanel_padding">
        <uc1:UploadCarDocAttachment ID="usrCarDocAttachments" runat="server" />
            <div class="formpanel formbuttons">
               <asp:ImageButton ID="cmdCarDocAttachCancel" OnClientClick="javascript:refreshAttachDiv(divID, attachDocType, carid);" ImageUrl="~/shared/images/buttons/btn_close.png" runat="server" CausesValidation="False" style="padding-left: 5px; display:  none;"/>
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modCarDocAttachment" runat="server" TargetControlID="lnkCarDocAttachment" PopupControlID="pnlCarDocAttachments" BackgroundCssClass="modalBackground" CancelControlID="cmdCarDocAttachCancel" ></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkCarDocAttachment" runat="server" style="display: none;"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modOdoReading" runat="server" TargetControlID="lnkOdoReading" PopupControlID="pnlOdo" BackgroundCssClass="modalBackground" CancelControlID="cmdOdoReadingCancel" ></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkOdoReading" runat="server" style="display: none;"></asp:LinkButton>

