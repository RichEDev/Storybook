<%@ Page language="c#" Inherits="expenses.adminp11d" MasterPageFile="~/exptemplate.master" Codebehind="adminp11d.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>



				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aep11d.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add P11D Category</asp:Label></a>
				
				</asp:Content>
			
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<script language="javascript" type="text/javascript">
				
				function deleteP11d(pdcatid)
				{
					if (confirm('Are you sure you wish to delete the selected P11d category?'))
					{
					    PageMethods.deleteP11d(accountid,pdcatid, deleteP11dComplete, deleteP11dFailed);
					}
}

function deleteP11dComplete(data) {
    switch (data) {
        case 0:
            var grid = igtbl_getGridById(contentID + 'gridp11d');

            grid.Rows.remove(grid.getActiveRow().getIndex());
            break;
        case -10:
            alert('The category cannot be deleted as it is currently assigned to one or more GreenLights or user defined fields.');
            break;
        default:
            break;
    }
}
function deleteP11dFailed(error) {
    alert('An error occurred attempting to delete the P11D category');
}
</script>

<div class="inputpanel table-border">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <igtbl:UltraWebGrid ID="gridp11d" runat="server" SkinID="gridskin" OnInitializeLayout="gridp11d_InitializeLayout" OnInitializeRow="gridp11d_InitializeRow" OnSortColumn="gridp11d_SortColumn" meta:resourcekey="gridp11dResource1">
        <DisplayLayout>
            <ActivationObject BorderColor="" BorderWidth="">
            </ActivationObject>
        </DisplayLayout>
    </igtbl:UltraWebGrid>
    </ContentTemplate>
    </asp:UpdatePanel>

</div>

    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>

    </asp:Content>


