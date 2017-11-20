<%@ Page Language="C#" MasterPageFile="~/masters/SMForm.master" AutoEventWireup="true" Inherits="Spend_Management.MyTasks" Title="My Tasks" Codebehind="MyTasks.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/SMForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">

    <asp:LinkButton runat="server" ID="lnkViewCompleted" CssClass="submenuitem" onclick="lnkViewCompleted_Click">View Closed</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkViewActive" Visible="false" CssClass="submenuitem" onclick="lnkViewActive_Click">View Active</asp:LinkButton>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">

    <div class="formpanel formpanel_padding">
        <asp:PlaceHolder runat="server" ID="phMyTasks"></asp:PlaceHolder>
        <asp:HiddenField runat="server" ID="returnURL" />

        <div class="formbuttons">
            <asp:ImageButton runat="server" ID="cmdClose" AlternateText="Close" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="false" onclick="cmdClose_Click" />
        </div>
    </div>
    
</asp:Content>

