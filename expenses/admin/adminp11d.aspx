<%@ Page language="c#" Inherits="expenses.adminp11d" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminp11d.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>



				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aep11d.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1"></asp:Label></a>
				
				</asp:Content>
			
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<script language="javascript" type="text/javascript">
				
function deleteP11d(pdcatid)
{
    if (confirm('This P11D Category will no longer be assigned to any expense items once deleted. Are you sure that you want to continue?'))
	{
        SEL.PublicApi.Call("DELETE", "P11DCategories/" + pdcatid, "", deleteP11dComplete, deleteP11dFailed);
	}
}

function deleteP11dComplete(data) {
    switch (data) {
        case 0:
            SEL.Grid.refreshGrid('gridP11D', 1);
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

    <div class="formpanel">
        <asp:Literal runat="server" id="litgrid"></asp:Literal>
    </div>

    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>

</asp:Content>


