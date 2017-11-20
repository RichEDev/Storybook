<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.ContractSavings" CodeFile="ContractSavings.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Contract Savings<asp:Label ID="lblTitle" runat="server"></asp:Label></div>
        <asp:Panel runat="server" ID="panelDataTable">
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblFrom" runat="server">Display for dates</asp:Label></td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="dateFrom" runat="server">
                        </igtxt:WebDateTimeEdit>
                        &nbsp;</td>
                    <td class="labeltd" align="center">
                        <asp:Label ID="lblTo" runat="server">to</asp:Label></td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="dateTo" runat="server">
                        </igtxt:WebDateTimeEdit>
                        &nbsp;</td>
                    <td>
                        <asp:ImageButton ID="cmdRefresh" runat="server" ImageUrl="buttons/refresh.gif"></asp:ImageButton>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div class="inputpanel">
        <asp:Literal runat="server" ID="litSavingsTable"></asp:Literal>
    </div>
    <asp:Panel runat="server" ID="panelEditFields" Visible="false">
        <div class="inputpanel"><asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </div>
        <div class="inputpanel">
            <table>
                <tr>
                    <td class="labeltd">
                        Saving Date</td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="dateSaving" runat="server" TabIndex="1">
                        </igtxt:WebDateTimeEdit>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqSavingDate" runat="server" ErrorMessage="A date of saving must be specified" ControlToValidate="dateSaving">***</asp:RequiredFieldValidator></td>
                    <td class="labeltd">
                        Reference</td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtReference" TabIndex="2"></asp:TextBox></td>
                    <td class="inputtd">
                        <asp:Label ID="hiddenSavingId" runat="server" Visible="False">0</asp:Label></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Amount Saved</td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtAmount" runat="server" TabIndex="3"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqAmount" runat="server" ErrorMessage="An amount saved must be specified" ControlToValidate="txtAmount">***</asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cmpAmount" runat="server"
                            ErrorMessage="Amount field must be numeric" Operator="DataTypeCheck" Type="Double" ControlToValidate="txtAmount">***</asp:CompareValidator></td>
                    <td class="labeltd">
                        Comment</td>
                    <td class="inputtd" colspan="2">
                        <asp:TextBox ID="txtComment" runat="server" Rows="2" TabIndex="4" TextMode="MultiLine" Width="100%"></asp:TextBox></td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" />&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
        </div>
    </asp:Panel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
<asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkAdd">Add Saving</asp:LinkButton>
<a href="./help_text/default_csh.htm#1190" target="_blank" class="submenuitem">Help</a>
</asp:Content>
