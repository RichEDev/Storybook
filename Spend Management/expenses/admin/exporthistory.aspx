<%@ Page language="c#" Inherits="Spend_Management.exporthistory" MasterPageFile="~/masters/smForm.master" Codebehind="exporthistory.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/reports/ReportFunctions.asmx" />
            <asp:ServiceReference Path="~/expenses/webservices/svcFinancialExports.asmx" InlineScript="false" />
            <asp:ServiceReference Path="~/expenses/webservices/svcAutomatedTransfers.asmx" InlineScript="false" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="../javaScript/exporthistory.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var dynGrid = '<% = pnlGrid.ClientID %>';
        //]]>
    </script>
    
	<div class="formpanel formpanel_padding">
        <asp:Panel ID="pnlGrid" runat="server"><asp:Literal ID="litGrid" runat="server"></asp:Literal></asp:Panel>
    </div>
</asp:Content>

