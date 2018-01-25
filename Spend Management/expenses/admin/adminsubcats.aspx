<%@ Page language="c#" Inherits="Spend_Management.adminsubcats" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminsubcats.aspx.cs" EnableViewState="false"%>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aesubcat.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">New Expense Item</asp:Label></a>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/subcats.js?date=20180125" />
        </Scripts>
    </asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
    <asp:Literal ID="litgrid" runat="server"></asp:Literal>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
</div>


    </asp:Content>


