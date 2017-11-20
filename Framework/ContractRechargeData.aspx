<%@ Page Language="vb" AutoEventWireup="false" Inherits="Framework2006.ContractRechargeData"
    CodeFile="ContractRechargeData.aspx.vb" MasterPageFile="~/fWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:Panel runat="server" ID="RPEditPanel" Visible="false">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Recharge Payment Details</div>
            <table>
                <tr>
                    <td class="labeltd">
                        Recharge Period</td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="dateRechargePeriod" TabIndex="1" runat="server">
                        </igtxt:WebDateTimeEdit>
                    </td>
                    <td class="labeltd">
                        <asp:Label ID="lblCustomer" TabIndex="3" runat="server"></asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstCustomer" TabIndex="3" runat="server">
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Recharge Amount</td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtRechargeAmount" TabIndex="3" runat="server"></asp:TextBox>
                        <asp:CompareValidator ID="cmpAmount" runat="server" Operator="DataTypeCheck" ControlToValidate="txtRechargeAmount"
                            Type="Double" ErrorMessage="Value must be numeric">***</asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender ID="cmpexAmount" runat="server" TargetControlID="cmpAmount">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" />
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="RPFilterPanel">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Recharge Payment Filter</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblDisplayRange" runat="server">Display Data for</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtFromDate"></asp:TextBox>
                        <asp:CompareValidator runat="server" ID="cmpFD" Operator="DataTypeCheck" Type="Date"
                            ControlToValidate="txtFromDate" Text="**" ErrorMessage="Invalid Date Format specified"></asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender ID="cmpexFD" runat="server" TargetControlID="cmpFD">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td class="labeltd">
                        <asp:Label ID="lblTo" runat="server">to</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtToDate"></asp:TextBox>
                        <asp:CompareValidator ID="cmpDate" runat="server" Operator="GreaterThan" ControlToValidate="txtToDate"
                            Type="Date" ErrorMessage="'To' date must be later than the 'From' date" ControlToCompare="txtFromDate">**</asp:CompareValidator>
                        <asp:CompareValidator ID="cmpTD" runat="server" ControlToValidate="txtToDate" ErrorMessage="Invalid Date Format specified"
                            Operator="DataTypeCheck" Type="Date">**</asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender ID="cmpexTD" runat="server" TargetControlID="cmpTD">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="cmpexFDTD" runat="server" TargetControlID="cmpDate">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td>
                        <asp:ImageButton runat="server" ID="cmdRefresh" ImageUrl="./buttons/refresh.gif" /></td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div class="inputpanel">
        <asp:Literal runat="server" ID="litReturnCount"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:Panel runat="server" ID="RPData">
        </asp:Panel>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif"
            CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentleft">
    <div class="panel">
        <div class="paneltitle">
            Navigation</div>
        <asp:LinkButton ID="lnkCDnav" runat="server" CausesValidation="False" CssClass="submenuitem">Contract Details</asp:LinkButton>
        <asp:LinkButton ID="lnkCAnav" runat="server" CausesValidation="False" CssClass="submenuitem">Additional Details</asp:LinkButton>
        <asp:LinkButton ID="lnkCPnav" runat="server" CausesValidation="False" CssClass="submenuitem">Contract Products</asp:LinkButton>
        <asp:LinkButton ID="lnkIDnav" runat="server" CausesValidation="False" CssClass="submenuitem">Invoice Details</asp:LinkButton>
        <asp:LinkButton ID="lnkIFnav" runat="server" CausesValidation="False" CssClass="submenuitem">Invoice Forecasts</asp:LinkButton>
        <asp:LinkButton ID="lnkNSnav" runat="server" CausesValidation="False" CssClass="submenuitem">Note Summary</asp:LinkButton>
        <asp:LinkButton ID="lnkLCnav" runat="server" CausesValidation="False" CssClass="submenuitem"
            Visible="false">Linked Contracts</asp:LinkButton>
        <asp:LinkButton ID="lnkCHnav" runat="server" CausesValidation="False" CssClass="submenuitem">Contract History</asp:LinkButton>
        <asp:LinkButton ID="lnkRTnav" runat="server" CausesValidation="False" CssClass="submenuitem">Recharge Template</asp:LinkButton>
        <asp:LinkButton ID="lnkRPnav" runat="server" CausesValidation="False" CssClass="submenuitem">Recharge Payments</asp:LinkButton>
        
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="./help_text/default_csh.htm#1118" target="_blank" class="submenuitem">Help</a>
</asp:Content>
