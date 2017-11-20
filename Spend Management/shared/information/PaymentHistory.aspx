<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="PaymentHistory.aspx.cs" Inherits="Spend_Management.shared.information.PaymentHistory" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="paymentHistoryContent" ContentPlaceHolderID="contentmain" runat="server">
    <div class="formpanel">
        <div class="sectiontitle">Payment History</div>
        <asp:Panel ID="pnlDynGrid" runat="server">
            <asp:Literal ID="litGrid" runat="server"></asp:Literal></asp:Panel>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
</asp:Content>

