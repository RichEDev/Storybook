<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.ViewSingleNote" CodeFile="ViewSingleNote.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript" type="text/javascript">
    function OpenAttachment()
    {
        var path;
        var att_list = document.getElementById('attachments');
        
        if(att_list != null)
        {
            path = att_list.options[att_list.selectedIndex].value;
            if(path != '0')
            {
                var att_type = path.substring(0,1);
                var att_path = path.substring(1);
                if(att_type == '1')
                {
                    if(confirm('\tThis attachment is in the secure area.\nDo you wish to issue an email request for this attachment to the Contract Owner?'))
                    {
                        window.location.href='ContractSummary.aspx?action=email&attid=' + att_path;
                    }
                }
                else if(att_type == '3')
                {
                    /* URL link attachment, so just open */
                    window.open(att_path);
                }
                else
                {
                    window.open('ViewAttachment.aspx?id=' + att_path);
                }
            }
        }
    }
    </script>

    <div class="inputpanel">
        <div class="inputpaneltitle">
            Single Note View
            <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
        <asp:Literal runat="server" ID="litNoteData"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
<a href="./help_text/default_csh.htm#1189" target="_blank" class="submenuitem">Help</a>
                
</asp:Content>

