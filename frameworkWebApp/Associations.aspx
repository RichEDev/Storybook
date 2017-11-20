<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" 
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.Associations" Codebehind="Associations.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:UpdatePanel ID="UpdatePanel_Assoc" runat="server">
        <ContentTemplate>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
                <table>
                    <tr>
                        <td class="labeltd" align="center" style="width: 250px;">
                            <asp:Label ID="lblAvailable" runat="server">available</asp:Label></td>
                        <td>
                        </td>
                        <td class="labeltd" align="center" style="width: 250px;">
                            <asp:Label ID="lblSelected" runat="server">selected</asp:Label></td>
                    </tr>
                    <tr>
                        <td class="inputtd">
                            <asp:ListBox ID="lstAvailable" runat="server" SelectionMode="Multiple" Height="300px"
                                Rows="25" Width="250"></asp:ListBox></td>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:ImageButton runat="server" ID="cmdSelect" ImageUrl="./buttons/navigate_right2.png" OnClick="cmdSelect_Click" style="margin-bottom: 10px;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:ImageButton runat="server" ID="cmdDeselect" ImageUrl="./buttons/navigate_left2.png" OnClick="cmdDeselect_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="inputtd">
                            <asp:ListBox ID="lstSelected" runat="server" SelectionMode="Multiple" Height="300px"
                                Rows="25" Width="250"></asp:ListBox></td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:Label ID="lblHint" runat="server">HINT: Hold CTRL + mouse click for multiple selections</asp:Label></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/page_close.png" OnClick="cmdOK_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="updprg_Assoc" runat="server" AssociatedUpdatePanelID="UpdatePanel_Assoc">
        <ProgressTemplate>
            <div class="progresspanel">
                 <div>Updating ...</div>
                <br />           
                  <asp:Image runat="server" ID="imgPOptPanel" ImageUrl="~/shared/images/ajax-loader.gif"
                    AlternateText="Updating..." />
            </div>

        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>

