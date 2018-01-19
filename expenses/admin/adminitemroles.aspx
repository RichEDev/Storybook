<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="admin_adminitemroles" Title="Untitled Page" Codebehind="adminitemroles.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
    
    <a href="aeitemrole.aspx" class="submenuitem">Add Item Role</a>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">

<script type="text/javascript" language="javascript">
    function deleteRole (itemroleid)
    {
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

        var grid = igtbl_getGridById(contentID + 'gridroles');

        grid.Rows.remove(grid.getActiveRow().getIndex());
    }
</script>

    <div class="inputpanel table-border">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <igtbl:UltraWebGrid ID="gridroles" runat="server" SkinID="gridskin" OnInitializeLayout="gridroles_InitializeLayout" OnInitializeRow="gridroles_InitializeRow" OnSortColumn="gridroles_SortColumn" meta:resourcekey="gridrolesResource1">
            <DisplayLayout>
                <ActivationObject BorderColor="" BorderWidth="">
                </ActivationObject>
            </DisplayLayout>
        </igtbl:UltraWebGrid>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
</asp:Content>

