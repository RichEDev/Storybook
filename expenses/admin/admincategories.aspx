<%@ Page language="c#" Inherits="expenses.admincategories" MasterPageFile="~/exptemplate.master" Codebehind="admincategories.aspx.cs" %>


<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aecategory.aspx" class="submenuitem">
					<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Expense Category</asp:Label></a>
					</asp:Content>
				
		<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
		<script language="javascript" type="text/javascript">

		    function deleteCategory(categoryid) {
		        if (confirm('Are you sure you wish to delete the selected expense category?')) {

		            PageMethods.deleteCategory(accountid, categoryid, deleteCategoryComplete);

		        }
		    }

				function deleteCategoryComplete(data) {
				    switch (data) {
				        case 1:
				            alert('This Expense Category cannot be deleted as there are still Sub Categories assigned to it.');
				            break;
				        case -10:
				            alert('This Expense Category cannot be deleted as it is currently assigned to one or more GreenLights or user defined fields.');
				            break;
				        default:
				            var grid = igtbl_getGridById(contentID + 'gridcategories');

				            grid.Rows.remove(grid.getActiveRow().getIndex());
				            break;
				    }
				}			
		
</script>

	
<div class="inputpanel table-border">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <igtbl:UltraWebGrid ID="gridcategories" runat="server" SkinID="gridskin" OnInitializeLayout="gridcategories_InitializeLayout" OnInitializeRow="gridcategories_InitializeRow" OnSortColumn="gridcategories_SortColumn" meta:resourcekey="gridcategoriesResource1">
        <DisplayLayout>
            <ActivationObject BorderColor="" BorderWidth="">
            </ActivationObject>
        </DisplayLayout>
    </igtbl:UltraWebGrid>
    </ContentTemplate>
    </asp:UpdatePanel>
</div>


<div>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
</div>

    </asp:Content>


