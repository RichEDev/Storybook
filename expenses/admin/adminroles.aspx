<%@ Page language="c#" Inherits="expenses.adminroles" MasterPageFile="~/exptemplate.master" Codebehind="adminroles.aspx.cs" StylesheetTheme="ExpensesThemeNew" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics35.WebUI.UltraWebGrid.v9.1, Version=9.1.20091.1015, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aerole.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Role</asp:Label></a>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript" type="text/javascript">
		
					
				function deleteRole(roleid)
				{
					var details
					if (confirm('Are you sure you wish to delete the selected role?'))
					{
						
						PageMethods.deleteRole(accountid,roleid,deleteRoleComplete);
						
					}
				}
				
				function deleteRoleComplete(data)
				{
				    if (data == false)
				    {
				        alert('This role cannot be deleted as it is still assigned to an employee(s)');
				        return;
				    }
				    else
				    {
				        var grid = igtbl_getGridById(contentID + 'gridroles');
						
        				grid.Rows.remove(grid.getActiveRow().getIndex());
				    }
				}
</script>

	
	
    <div class="inputpanel">
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
    </asp:Content>


