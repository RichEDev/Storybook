<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="policyCustomHelp.aspx.cs" Inherits="Spend_Management.policyCustomHelp" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/helpAndSupport.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div class="formpanel">
        <div class="sectiontitle">Custom Help and Support Advice</div>
        <div class="twocolumn">
            <asp:Label ID="lblMainContact" runat="server" AssociatedControlID="ddlMainContact">Main Contact</asp:Label><span class="inputs"><asp:DropDownList ID="ddlMainContact" runat="server"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltips"></span><span class="inputvalidationfield"></span>
        </div>
        <div class="onecolumn">
            <asp:Label ID="lblPolicyHelp" runat="server" AssociatedControlID="txtPolicyHelp">Help &amp; Support Text</asp:Label><span class="inputs"><asp:TextBox ID="txtPolicyHelp" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltips"></span><span class="inputvalidationfield"></span>
        </div>
        <div class="formbuttons">
            <asp:Image ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" />&nbsp;<asp:Image ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="cancelPolicyHelp();" />
        </div>
    </div>
</asp:Content>
