<%@ Page Language="VB" MasterPageFile="~/FWMaster.master" AutoEventWireup="false" CodeFile="Backup.aspx.vb" Inherits="Backup" title="Framework Backup" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
<a href="./help_text/default_csh.htm#0" target="_blank" class="submenuitem">Help</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<div class="inputpanel">
<div class="inputpaneltitle">File Attachment Backup</div><asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<table>
<tr>
<td class="labeltd"><asp:Label runat="server" ID="lblCopyStatus"></asp:Label></td>
<td>
    <asp:ImageButton ID="cmdBackup" runat="server" CausesValidation="false" ImageUrl="./buttons/ok.gif" OnClick="cmdBackup_Click" /></td>
</tr>
<tr>
<td colspan="2">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server"><ProgressTemplate><img src="images/wait.gif" alt="Copy in progress" /></ProgressTemplate>
    </asp:UpdateProgress>
</td>
</tr>
</table>
</ContentTemplate>
 </asp:UpdatePanel>
</div>
    
   
</asp:Content>

