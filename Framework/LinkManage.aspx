<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.LinkManage" CodeFile="LinkManage.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript">
			function setFocus()
			{
				Form1.txtNewDefinition.focus();
			}
    </script>

    <asp:UpdatePanel runat="server" ID="ajaxUpdatePanel">
        <ContentTemplate>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    Link Management
                    <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblLinkDef" runat="server">Link Definition</asp:Label></td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstLinkDefs" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstLinkDefs_SelectedIndexChanged">
                            </asp:DropDownList></td>
                        <td>
                            <asp:ImageButton runat="server" ID="cmdDeleteDef" ImageUrl="./icons/delete2.gif" /></td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblNewDefinition" runat="server"></asp:Label></td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtNewDefinition" runat="server" TabIndex="1" MaxLength="50"></asp:TextBox>&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" OnClick="cmdOK_Click" /></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:Literal ID="litLinkData" runat="server"></asp:Literal></div>
            <div class="inputpanel">
                <asp:Label ID="lblErrorStatus" runat="server"></asp:Label><asp:Label ID="hiddenContractId"
                    runat="server" Visible="False"></asp:Label></div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" /></div>
        <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelMain" DisplayAfter="50" AssociatedUpdatePanelID="ajaxUpdatePanel">
        <ProgressTemplate>
            <div class="inputpanel">
                <div align="center">
                    <img src="images/loading.gif" alt="Loading..." /></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="cmdPrint" CssClass="submenuitem" Visible="False">Print</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1163" target="_blank" class="submenuitem">Help</a>
    </asp:Content>
