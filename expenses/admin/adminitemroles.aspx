<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="admin_adminitemroles" Title="Untitled Page" Codebehind="adminitemroles.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
    
    <a href="aeitemrole.aspx" class="submenuitem">Add Item Role</a>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">

<script type="text/javascript" language="javascript">
    var id = 0;
    function deleteRole (itemroleid) {
        id = itemroleid;
        if (confirm('Are you sure you would like to delete the selected role?'))
        {
            PageMethods.deleteRole(accountid, itemroleid, deleteRoleComplete);
        }
    }
    
    function deleteRoleComplete(returnvalue) {
        if (returnvalue == -1) {
            SEL.MasterPopup.ShowMasterPopup('The item role cannot be deleted as it is associated with one or more flag rules.');
            return;
        }

        SEL.Grid.deleteGridRow('gridItemRoles', id);
    }
</script>
    
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>

</asp:Content>

