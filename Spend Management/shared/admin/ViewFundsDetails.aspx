<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smForm.master" CodeBehind="ViewFundsDetails.aspx.cs" Inherits="Spend_Management.shared.admin.ViewFundsDetails" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content runat="server" ID="customStyles" ContentPlaceHolderID="styles">
    <!--[if IE 7]>
        <style> 
            .formpanel {
                display: inline-block;
            }
        </style>
   <![endif]-->
    <style type="text/css">
        #ctl00_contentmain_imgTransStartDate, #ctl00_contentmain_imgTransEndDate {
            margin-top: 3px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    
  <div id="div">
        <div class="formpanel">
            <div class="sectiontitle"><asp:Label ID="lblFundDetails" runat="server" Text="Fund Details" ></asp:Label></div>
          <div class="twocolumn">
                <asp:Label ID="lblFund" runat="server" Text="Available Funds"  AssociatedControlID="lblAvailableFund"></asp:Label>
                <span class="inputs"><asp:Label runat="server" ID="lblAvailableFund" CssClass="fillspan"></asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>

            <div class="sectiontitle">Search Option</div>
            <div class="twocolumn">
                <asp:Label ID="lblDateRangeType" runat="server" Text="Transaction Type" AssociatedControlID="ddlTranscationType"></asp:Label>
                <span class="inputs"><asp:DropDownList ID="ddlTranscationType" runat="server" CssClass="fillspan">
                    <asp:ListItem Text="All"></asp:ListItem>
                    <asp:ListItem Text="Credit"></asp:ListItem>
                    <asp:ListItem Text="Debit"></asp:ListItem>
                </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>

            <div class="twocolumn">
                <span id="spanDateThreshold1">
                    <asp:Label ID="lblDateval1" runat="server" Text="Start Date" AssociatedControlID="txtTransactionStartDate"></asp:Label><span class="inputs"><asp:TextBox ID="txtTransactionStartDate" runat="server" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="maskedDateVal1" runat="server"
                        CultureName="en-GB" Mask="99/99/9999" MaskType="Date"
                        TargetControlID="txtTransactionStartDate">
                    </cc1:MaskedEditExtender>
                        <cc1:CalendarExtender ID="calendarExtenderDateVal1" runat="server" Format="dd/MM/yyyy"
                            PopupButtonID="imgTransStartDate" TargetControlID="txtTransactionStartDate" PopupPosition="BottomLeft">
                        </cc1:CalendarExtender>
                    </span><span class="inputicon">
                        <asp:Image ID="imgTransStartDate" runat="server" ImageUrl="~/shared/images/icons/cal.gif" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compdate1" runat="server" ErrorMessage="The value you have entered for Start Date is invalid" ControlToValidate="txtTransactionStartDate" ValidationGroup="vgDateRange" Type="Date" Operator="DataTypeCheck" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate1min" ControlToValidate="txtTransactionStartDate" Type="Date" Operator="GreaterThan" ValueToCompare="01/01/1900" ValidationGroup="vgDateRange" Text="*" ErrorMessage="Please enter Start Date must after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpDate1max" ControlToValidate="txtTransactionStartDate" Display="Dynamic" Type="Date" Operator="LessThan" ValidationGroup="vgDateRange" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Please enter Start Date before 31/12/3000"></asp:CompareValidator></span></span>
            </div>
            
            <div class="twocolumn">
                <span id="span1">
                    <asp:Label ID="lblEndDate" runat="server" Text="End Date" AssociatedControlID="txtTransactionEndDate"></asp:Label><span class="inputs"><asp:TextBox ID="txtTransactionEndDate" runat="server" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                        CultureName="en-GB" Mask="99/99/9999" MaskType="Date"
                        TargetControlID="txtTransactionEndDate">
                    </cc1:MaskedEditExtender>
                        <cc1:CalendarExtender ID="calendarExtenderEndDate" runat="server" Format="dd/MM/yyyy"
                            PopupButtonID="imgTransEndDate" TargetControlID="txtTransactionEndDate" PopupPosition="BottomLeft">
                        </cc1:CalendarExtender>
                    </span><span class="inputicon">
                        <asp:Image ID="imgTransEndDate" runat="server" ImageUrl="~/shared/images/icons/cal.gif" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator
                                ID="cvtxtTransactionEndDateIsValid" runat="server" ErrorMessage="The value you have entered for End Date is invalid" ControlToValidate="txtTransactionEndDate" ValidationGroup="vgDateRange" Type="Date" Operator="DataTypeCheck" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cvtxtTransactionEndDateIsAfter" ControlToValidate="txtTransactionEndDate" Type="Date" Operator="GreaterThan" ValueToCompare="01/01/1900" ValidationGroup="vgDateRange" Text="*" ErrorMessage="Please enter End Date must after 01/01/1900"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cvtxtTransactionEndDateIsBefore" ControlToValidate="txtTransactionEndDate" Type="Date" Operator="LessThan" ValidationGroup="vgDateRange" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Please enter End Date before 31/12/3000"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cvCompareStartandEndDate" ControlToValidate="txtTransactionEndDate" ControlToCompare="txtTransactionStartDate" Type="Date" Operator="GreaterThanEqual" ValidationGroup="vgDateRange" ValueToCompare="31/12/3000" Text="*" ErrorMessage="Please enter End Date on or after Start Date"></asp:CompareValidator></span></span>
            </div>

             <br />
            <div class="sectionitle-botton">
                <%-- <helpers:CSSButton id="btnAccept1" runat="server" text="search" UseSubmitBehavior="False" OnClientClick="javascript:SEL.FundDetails.RefreshGrid(); return false;"/>--%>
                <asp:ValidationSummary runat="server" ValidationGroup="vgDateRange" ShowMessageBox="true" ShowSummary="false"/>
                 <helpers:CSSButton id="cmdSearch" runat="server" text="search" UseSubmitBehavior="False" OnClick="cmdSearch_OnClick" ValidationGroup="vgDateRange" />
                   <helpers:CSSButton id="cmdCancel" runat="server" text="cancel" OnClick="cmdCancel_OnClick" UseSubmitBehavior="False"/>
            </div>
            <br/>
              <asp:Panel runat="server" ID="pnlGrid">
                <asp:Literal ID="litFundDetails" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>
  
</asp:Content>
