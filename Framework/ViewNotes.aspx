<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.ViewNotes"
    CodeFile="ViewNotes.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript" type="text/javascript">
    function DisableDelete()
    {
        var cntl = document.getElementById('lnkDelete');
        if(cntl != null)
        {
            cntl.style.display = 'none';
        }
    }
    
    function ExitNotes()
    {
        var enReq = PageMethods.ExitNotes(fnExitNotesComplete);
        
    }
    
    function fnExitNotesComplete(strResults)
    {
        window.location.href = strResults;
        return true;
    }
    </script>

    <asp:UpdatePanel ID="UpdatePanelNotes" runat="server">
        <ContentTemplate>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    <asp:Label ID="lblSubTitle" runat="server">subtitle</asp:Label></div>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblNoteCategory" runat="server">note category</asp:Label></td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstNoteCategory" TabIndex="3" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="lstNoteCategory_SelectedIndexChanged">
                            </asp:DropDownList></td>
                        <td class="labeltd">
                            <asp:Label ID="lblNoteType" runat="server">note type</asp:Label>
                        </td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstNoteType" TabIndex="4" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton runat="server" ID="cmdReset" ImageUrl="~/icons/24/plain/document_refresh.gif"
                                AlternateText="Reset" CausesValidation="false" OnClick="cmdReset_Click" /></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:Label ID="lblErrorString" runat="server"></asp:Label>
            </div>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    Note List</div>
                <asp:ListBox ID="lstNoteList" TabIndex="5" runat="server" AutoPostBack="True" Rows="4"
                    Width="350" OnSelectedIndexChanged="lstNoteList_SelectedIndexChanged"></asp:ListBox>
            </div>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    Note Detail</div>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblNoteDate" runat="server">date</asp:Label></td>
                        <td class="inputtd">
                            <igtxt:WebDateTimeEdit ID="txtCreatedDate" runat="server" Nullable="false" MinValue="01/01/1900">
                            </igtxt:WebDateTimeEdit>
                        </td>
                        <td class="inputtd">
                            <asp:Literal ID="litAttach" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td class="inputtd" valign="top" colspan="3">
                            <asp:TextBox ID="txtNote" TabIndex="1" runat="server" Width="350" Height="200" TextMode="MultiLine"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblCreatedBy" runat="server">created by</asp:Label></td>
                        <td class="inputtd" colspan="2">
                            <asp:TextBox ID="txtCreatedBy" runat="server" Wrap="False" ReadOnly="True"></asp:TextBox></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" OnClick="cmdUpdate_Click" />
                <a onclick="javascript:ExitNotes();" style="cursor: hand;">
                    <asp:Image runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" /></a>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:Label ID="hiddenAttArea" runat="server" Visible="False"></asp:Label>
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem" CausesValidation="false">New Note</asp:LinkButton>
    <asp:UpdatePanel runat="server" ID="menu_UpdatePanel">
        <ContentTemplate>
            <asp:Literal runat="server" ID="litDelete"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>
    <a href="./help_text/default_csh.htm#1005" target="_blank" class="submenuitem">Help</a>
</asp:Content>
