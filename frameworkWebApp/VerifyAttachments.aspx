<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.VerifyAttachments" Codebehind="VerifyAttachments.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Attachment Verification
        </div>
        <asp:Panel ID="panelUpload" runat="server" Visible="false" HorizontalAlign="Center">
            <table>
                <tr>
                    <td class="labeltd">File to Upload</td>
                    <td class="inputtd" colspan=2><asp:Label ID="lblFilename" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Upload file</td>
                    <td class="inputtd">
                        <input id="attachment" type="file" size="50" name="attachment" runat="server" style="width: 400px;"></td>
                    <td>
                        <asp:Label ID="hiddenAttID" runat="server" Visible="False"></asp:Label></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Attachment Type</td>
                    <td class="inputradio">
                        <asp:RadioButtonList ID="rdoAttachmentType" runat="server" RepeatDirection="Horizontal" CssClass="inputradio">
                            <asp:ListItem Value="0" Selected="True">Open</asp:ListItem>
                            <asp:ListItem Value="1">Secure</asp:ListItem>
                        </asp:RadioButtonList></td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div class="inputpanel">
        <asp:Literal ID="litAttachments" runat="server"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:PlaceHolder ID="holderFileDialog" runat="server"></asp:PlaceHolder>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/ok.gif" />&nbsp;
        <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/page_close.png" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">           
</asp:Content>

