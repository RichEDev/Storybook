<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    Inherits="Framework2006.LinkedContracts" CodeFile="LinkedContracts.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <script type="text/javascript" language="javascript">
			function doPrint()
			{
				alert('Please ensure correct printer an orientation selected');
				window.print();
			}
    </script>

    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblTitle" runat="server">title</asp:Label></div>
        <asp:Literal ID="litLinkData" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
        <asp:LinkButton ID="lnkPrint" runat="server" CssClass="submenuitem">Print</asp:LinkButton>
        <a href="./help_text/default_csh.htm#1191" target="_blank" class="submenuitem">Help</a>
</asp:Content>
