<%@ Page Language="C#" MasterPageFile="~/masters/smPagedForm.master"AutoEventWireup="true" CodeBehind="adminemailtemplates.aspx.cs" Inherits="Spend_Management.adminemailtemplates" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('System');" id="lnkSystem" class="selectedPage">System Notification Templates</a>
    <a href="javascript:changePage('Custom');" id="lnkCustom" >Custom Notification Templates</a>
    <asp:Panel runat="server" ID="pnlAddTemplate"><a href="aeemailtemplate.aspx" class="submenuitem">New Notification Template</a></asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmain" runat="server">

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/shared/webservices/svcEmailTemplates.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.emailtemplates.js" />
        </Scripts>
        
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript" language="javascript">
        var pnlGridID = '<%=pnlGrid.ClientID %>';
    </script>
    
    <div id="divPages">
        <div id="pgSystem" class="primaryPage formpanel formpanel_padding">
                <div class="twocolumn">
            <div class="sectiontitle">
                <asp:Label ID="lblsystemtemps" runat="server" Text="System Notification Templates" meta:resourcekey="lblsystemtempsResource1"></asp:Label>
                </div>
                    
            </div>
            <asp:Literal ID="litsystemgrid" runat="server"></asp:Literal>
        </div>
    
        <div id="pgCustom" class="formpanel formpanel_padding"  style="display: none">
            <div class="twocolumn">
                <div class="sectiontitle">
                    <asp:Label ID="lblTemplates" runat="server" Text=" Custom Notification Templates" meta:resourcekey="lblTemplatesResource1"></asp:Label>
                </div>
	     
	        </div>

            <asp:Panel runat="server" ID="pnlGrid">
                <asp:Literal ID="litgrid" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>
    </div>
</asp:Content>
