<%@ Page language="c#" Inherits="Spend_Management.adminbudget" MasterPageFile="~/masters/smTemplate.master"  Codebehind="adminbudget.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aebudget.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">New Budget Holder</asp:Label></a>
				
				</asp:Content>
				
		<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">		

	<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="../javaScript/budgetHolders.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcBudgetHolders.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

<div class="formpanel formpanel_padding">
    <asp:Literal runat="server" ID="litgrid"></asp:Literal>
    <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
</div>

       
    </asp:Content>


