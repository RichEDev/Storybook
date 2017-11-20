<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalMatrices.aspx.cs" Inherits="Spend_Management.shared.admin.ApprovalMatrices" MasterPageFile="~/masters/smForm.master" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:content id="Content2" runat="server" contentplaceholderid="contentmenu">
    <asp:HyperLink runat="server" ID="lnkNewMatrix" NavigateUrl="aeApprovalMatrix.aspx" CssClass="submenuitem">New Approval Matrix</asp:HyperLink>
</asp:content>
<asp:content id="Content1" runat="server" contentplaceholderid="contentmain">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.approvalMatrices.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div id="gridMatrixDiv">
            <asp:Literal runat="server" ID="litgrid"></asp:Literal>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClick="btnCloseClick" CausesValidation="false" />
        </div>
    </div>

</asp:content>