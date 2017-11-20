<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false"
    Inherits="Framework2006.TieredDefinitions" SmartNavigation="True" CodeFile="TieredDefinitions.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript">
			function PageSetup()
			{
				var ctrl = document.getElementById('txtDefInput');
				
				if(ctrl != null)
				{
					ctrl.focus();
				}
			}
    </script>

    <asp:Label ID="hiddenID" runat="server" Visible="False"></asp:Label>
    <asp:Label ID="hiddenParentID" runat="server" Visible="False"></asp:Label>
    <div class="inputpanel">
        <asp:Panel runat="server" ID="panelNewInput" Visible="false">
            <div class="inputpaneltitle">
                New Definition</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblDefTitle" runat="server">title</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtDefInput" runat="server" MaxLength="50" TabIndex="1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblFullDesc" runat="server">full description</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtFullDesc" runat="server" MaxLength="50" TabIndex="2"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />&nbsp;<asp:ImageButton
                    runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
            </div>
        </asp:Panel>
    </div>
    <div class="inputpanel">
        <div>
            <asp:Label ID="lblErrorString" runat="server"></asp:Label></div>
        <asp:Literal runat="server" ID="litDefinitions"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" CausesValidation="false" ImageUrl="./buttons/page_close.gif" />
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem">New Parent</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1146" target="_blank" class="submenuitem">Help</a>
</asp:Content>
