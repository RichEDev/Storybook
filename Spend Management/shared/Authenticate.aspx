<%@ Page Language="C#" MasterPageFile="~/masters/AnonymousUser.Master"  AutoEventWireup="true" CodeBehind="Authenticate.aspx.cs" Inherits="Spend_Management.AuthenticatePage" %>
<%@ MasterType VirtualPath="~/masters/AnonymousUser.Master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %> 

<asp:Content ID="Content7" ContentPlaceHolderID="pageContents" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    <div id="logonform">
        <div id="titlegradient"></div>
        <table border="0" style="width: 360px; text-align: left;">
            <tr><td colspan="3">Please enter your Reset Key in the box provided below.</td></tr>
            <tr><td style="width: 100px;"><asp:Label ID="lblTxtKey" runat="server" Text="Reset Key" AssociatedControlID="txtKey"></asp:Label></td><td><asp:TextBox ID="txtKey" runat="server" CssClass="fillspan"></asp:TextBox><asp:RequiredFieldValidator ID="rfKey" runat="server" ControlToValidate="txtKey" ErrorMessage="Key is a required field" Text="*"></asp:RequiredFieldValidator></td><td><img id="imgtooltip393" onclick="SEL.Tooltip.Show('ce85290a-c643-4866-9d81-c795c17ea9f0', 'sm', this);" src="images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td></tr>
            <tr><td colspan="3" class="errortext"><asp:Literal ID="litMessage" runat="server"></asp:Literal></td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3"><asp:ImageButton ID="btnOk" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="btnOk_Click" AlternateText="Ok" /></td></tr>
        </table>
    </div>

</asp:Content>