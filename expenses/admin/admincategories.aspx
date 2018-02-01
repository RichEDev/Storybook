<%@ Page language="c#" Inherits="expenses.admincategories" MasterPageFile="~/exptemplate.master" Codebehind="admincategories.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aecategory.aspx" class="submenuitem">
					<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Expense Category</asp:Label></a>
					</asp:Content>
				
		<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
		<script language="javascript" type="text/javascript">
		    var id = 0;
            function deleteCategory(categoryid) {
                id = categoryid;
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
				            SEL.Grid.deleteGridRow('gridCats', id);
				            break;
				    }
				}			
		
</script>

            <div class="formpanel formpanel_padding">
                <asp:Literal ID="litgrid" runat="server"></asp:Literal>
                <div class="formbuttons">
                    <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
                </div>
            </div>




        </asp:Content>


