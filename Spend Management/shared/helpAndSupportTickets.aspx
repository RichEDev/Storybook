<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="helpAndSupportTickets.aspx.cs" Inherits="Spend_Management.helpAndSupportTickets" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ContentPlaceHolderID="contentmenu" runat="server">
    <div id="divPageMenu">
        <a href="/shared/helpAndSupport.aspx">Online Help</a>
        <a id="ticketsLink" class="selectedPage" href="/shared/helpAndSupportTickets.aspx">My Tickets</a>
        <a id="circleLink" runat="server" href="http://circle.software-europe.com" target="_blank" title="Access our product support portal Circle">Visit Circle</a>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
       
    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {

        });
    </script>
    
    <div class="formpanel formpanel_padding" runat="server" ID="divSalesForceTickets" Visible="False">
        <div class="sectiontitle">Selenity Tickets</div>
        <div class="comment" runat="server">Support tickets you have raised with Selenity.</div>
        <asp:Literal runat="server" ID="TicketsSalesForce"></asp:Literal>
    </div>

    <div class="formpanel formpanel_padding" runat="server" ID="divInternalTickets" Visible="False">
        <div class="sectiontitle">Administrator Tickets</div>
        <div class="comment" runat="server">Support tickets you have raised with your Administrator.</div>
        <asp:Literal runat="server" ID="TicketsInternal"></asp:Literal>
    </div>
    
    <div class="formpanel formpanel_padding">
        <div class="formbuttons" style="padding:0px;">
            <helpers:CSSButton ID="cmdClose" OnClick="CmdClose_Click" runat="server" Text="close" UseSubmitBehavior="true" CausesValidation="False"></helpers:CSSButton>
        </div>   
    </div>
    
</asp:Content>
