<%@ Page language="c#" Inherits="Spend_Management.adminallowances" MasterPageFile="~/masters/smtemplate.master"  Codebehind="adminallowances.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smtemplate.master" %>



				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
                    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                        <Scripts>
                            <asp:ScriptReference Path="~/expenses/javaScript/allowances.js" />
                        </Scripts>
                    </asp:ScriptManagerProxy>

				<a href="aeallowance.aspx" class="submenuitem">
					<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">New Allowance</asp:Label></a>
					
					</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>        
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>        
    </div>
    </asp:Content>

