<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.About" CodeFile="About.aspx.vb" Title="About" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">General Details</div>
        <table>
            <tr><td class="labeltd" width="250"><b>Supplied By :</b></td><td class="inputtd" width="250">Software (Europe) Ltd.</td></tr>
            <tr><td class="labeltd"><b>Company URL:</b></td><td class="inputtd"><a href="http://www.software-europe.co.uk" target="_blank">www.software-europe.co.uk</a></td></tr>
            <tr><td class="labeltd"><b>Version installed:</b></td><td class="inputtd"><asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></td></tr>
            <tr><td class="labeltd">Concurrent users active</td><td class="inputtd"><asp:Label ID="lblActiveUsers" runat="server"></asp:Label></td></tr>
        </table>
    </div>
    <div class="inputpanel">
    <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
<a href="./help_text/default_csh.htm#1048" target="_blank" class="submenuitem">Help</a>
                
</asp:Content>

