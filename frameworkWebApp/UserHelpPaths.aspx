<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.UserHelpPaths" Codebehind="UserHelpPaths.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <asp:Label ID="lblStatus" runat="server"></asp:Label></div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Contract Details Fields</div>
        <table>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblContractHelpDir" runat="server">Contract Help Dir</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtContractHelpDir" TabIndex="1" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblSupercedes" runat="server">Supercedes Contract</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtSupercedes" TabIndex="2" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblConType" runat="server">Contract Type</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtConType" TabIndex="3" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblCategory" runat="server">Contract Category</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtCategory" TabIndex="4" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblConStatus" runat="server">Contract Status</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtConStatus" TabIndex="5" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblOwner" runat="server">Contract Owner</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtOwner" TabIndex="6" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblContractValue" runat="server">Contract Value</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtContractValue" TabIndex="7" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblAnnualValue" runat="server">Annual Contract Value</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtAnnualValue" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblTermType" runat="server">Term Type</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtTermType" TabIndex="8" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblInvFreq" runat="server">Invoice Frequency</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtInvFreq" TabIndex="9" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblContractAdditional" runat="server">Additional Contract Detail</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtContractAdditional" TabIndex="10" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Vendor Fields</div>
        <table>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblVendorHelpDir" runat="server">Vendor Help Dir</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtVendorHelpDir" TabIndex="11" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblVendorName" runat="server">Vendor Name</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtVendorName" TabIndex="12" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblVendorCategory" runat="server">Vendor Category</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtVendorCategory" TabIndex="13" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblVendorStatus" runat="server">Vendor Status</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtVendorStatus" TabIndex="14" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblCustNumber" runat="server">Customer Number</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtCustNumber" TabIndex="15" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblFinancialYE" runat="server">Financial Year End</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtFinancialYE" TabIndex="16" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblFinStatus" runat="server">Financial Status</asp:Label></td>
                <td class="labeltd">
                    <asp:TextBox ID="txtFinStatus" TabIndex="17" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
        </table>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />&nbsp;<asp:ImageButton
            runat="server" ID="cmdCancel" CausesValidation="false" ImageUrl="./buttons/cancel.gif" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>

