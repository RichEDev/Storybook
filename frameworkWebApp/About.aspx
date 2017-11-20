<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.About" Title="About" Codebehind="About.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">General Details</div>
        <table>
            <tr><td class="labeltd" width="250"><b>Supplied By :</b></td><td class="inputtd" width="250">Selenity Ltd.</td></tr>
            <tr><td class="labeltd"><b>Company URL:</b></td><td class="inputtd"><a href="http://www.selenity.com" target="_blank">www.selenity.com</a></td></tr>
            <!--<tr><td class="labeltd"><b>Version installed:</b></td><td class="inputtd"><asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></td></tr>-->
            <tr><td class="labeltd">Concurrent users active</td><td class="inputtd"><asp:Label ID="lblActiveUsers" runat="server"></asp:Label></td></tr>
        </table>
    </div>
    <div class="inputpanel">
    <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>

