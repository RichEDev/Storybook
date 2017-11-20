<%@ Page Language="C#" MasterPageFile="~/masters/SMForm.master" AutoEventWireup="true" Inherits="Spend_Management.TaskHistory" Title="Untitled Page" CodeBehind="TaskHistory.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/SMForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">Task History -
            <asp:Label runat="server" ID="lblTaskTitle"></asp:Label></div>
        <asp:Panel runat="server" ID="historyPanel"></asp:Panel>

        <div class="formbuttons">
            <asp:ImageButton runat="server" ID="cmdClose" CausesValidation="false" ImageUrl="~/shared/images/buttons/btn_close.png" OnClick="cmdClose_Click" />
        </div>
    </div>
</asp:Content>
