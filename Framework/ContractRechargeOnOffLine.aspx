<%@ Page Language="C#" MasterPageFile="~/FWMaster.master" AutoEventWireup="true"
    CodeFile="ContractRechargeOnOffLine.aspx.cs" Inherits="ContractRechargeOnOffLine"
    Title="On-Off line dates" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
    <asp:LinkButton runat="server" ID="lnkAddDates" CssClass="submenuitem" OnClick="lnkAddDates_Click">Add Dates</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1114" target="_blank" class="submenuitem">Help</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <asp:Panel runat="server" ID="curDatesPanel">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Display filter</div>
            <table>
                <tr>
                    <td class="labeltd">
                        Display start</td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtDisplayStart" runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="calexDisplayStart" runat="server" Format="dd/MM/yyyy" TargetControlID="txtDisplayStart">
                        </cc1:CalendarExtender>
                        <cc1:ValidatorCalloutExtender ID="cmpexStartDate" runat="server" TargetControlID="cmpStartDate">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="reqexDisplayStart" runat="server" TargetControlID="reqDisplayStart">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td class="labeltd">
                        Display finish</td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtDisplayFinish" runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="calexDisplayFinish" runat="server" Format="dd/MM/yyyy"
                            TargetControlID="txtDisplayFinish">
                        </cc1:CalendarExtender>
                        <cc1:ValidatorCalloutExtender ID="cmpexSEDates" runat="server" TargetControlID="cmpSEDates">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="cmpexFinishDate" runat="server" TargetControlID="cmpFinishDate">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="reqexDisplayFinish" runat="server" TargetControlID="reqDisplayFinish">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td>
                        <asp:ImageButton runat="server" ID="cmdRefresh" ImageUrl="~/Buttons/refresh.gif"
                            OnClick="cmdRefresh_Click" ValidationGroup="SEValidate" />
                        <td>
                            <asp:RequiredFieldValidator ID="reqDisplayStart" runat="server" ControlToValidate="txtDisplayStart"
                                ErrorMessage="Display start is a mandatory field" SetFocusOnError="True" ValidationGroup="SEValidate">***</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cmpSEDates" runat="server" ErrorMessage="Start date must preceed the finish date"
                                ControlToCompare="txtDisplayStart" ControlToValidate="txtDisplayFinish" Operator="GreaterThan"
                                SetFocusOnError="True" Type="Date" ValidationGroup="SEValidate">***</asp:CompareValidator>
                            <asp:CompareValidator ID="cmpStartDate" runat="server" ControlToValidate="txtDisplayStart"
                                ErrorMessage="Start date specified is invalid" Operator="DataTypeCheck" SetFocusOnError="True"
                                Type="Date" ValidationGroup="SEValidate">***</asp:CompareValidator>
                            <asp:CompareValidator ID="cmpFinishDate" runat="server" ControlToValidate="txtDisplayFinish"
                                ErrorMessage="The Finish date specified is invalid" Operator="DataTypeCheck"
                                SetFocusOnError="True" Type="Date" ValidationGroup="SEValidate">***</asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="reqDisplayFinish" runat="server" ControlToValidate="txtDisplayFinish"
                                ErrorMessage="Display Finish is a mandatory field" SetFocusOnError="True" ValidationGroup="SEValidate">***</asp:RequiredFieldValidator></td>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <div class="inputpaneltitle">
                On-Off Line Dates for Contract Products</div>
            <asp:Panel runat="server" ID="oodData">
            </asp:Panel>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdClose" CausesValidation="false" ImageUrl="~/buttons/page_close.gif"
                OnClick="cmdClose_Click" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="dateEntryPanel" Visible="false">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Enter new date range</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label runat="server" ID="lblOfflineDate"></asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtOfflineFrom" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:CompareValidator ID="cmpOfflineFrom" runat="server" ControlToValidate="txtOfflineFrom"
                            ErrorMessage="Offline from date specified is invalid" Operator="DataTypeCheck"
                            SetFocusOnError="True" Type="Date" ValidationGroup="OODateEntry">***</asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="reqOfflineFrom" runat="server" ControlToValidate="txtOfflineFrom"
                            ErrorMessage="Offline date field is mandatory" SetFocusOnError="True" ValidationGroup="OODateEntry">***</asp:RequiredFieldValidator>
                        <cc1:ValidatorCalloutExtender ID="cmpexOfflineFrom" runat="server" TargetControlID="cmpOfflineFrom">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="reqexOfflineFrom" runat="server" TargetControlID="reqOfflineFrom">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label runat="server" ID="lblOnlineDate"></asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtOnlineFrom" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:CompareValidator ID="cmpOnlineFrom" runat="server" ControlToValidate="txtOnlineFrom"
                            ErrorMessage="The Online date specified is invalid" Operator="DataTypeCheck"
                            SetFocusOnError="True" Type="Date" ValidationGroup="OODateEntry">***</asp:CompareValidator>
                        <asp:CompareValidator ID="cmpOODate" runat="server" ControlToCompare="txtOfflineFrom"
                            ControlToValidate="txtOnlineFrom" ErrorMessage="The offline date must preceed the online date"
                            Operator="GreaterThan" SetFocusOnError="True" ValidationGroup="OODateEntry">***</asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="reqOnlineFrom" runat="server" ControlToValidate="txtOnlineFrom"
                            ErrorMessage="The Online date is mandatory" SetFocusOnError="True" ValidationGroup="OODateEntry">***</asp:RequiredFieldValidator>
                        <cc1:ValidatorCalloutExtender ID="cmpexOODates" runat="server" TargetControlID="cmpOODate">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="cmpexOnlineFrom" runat="server" TargetControlID="cmpOnlineFrom">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender ID="reqexOnlineFrom" runat="server" TargetControlID="reqOnlineFrom">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/buttons/update.gif" OnClick="cmdUpdate_Click" />
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/buttons/cancel.gif" CausesValidation="false"
                OnClick="cmdCancel_Click" />
        </div>
    </asp:Panel>
</asp:Content>
