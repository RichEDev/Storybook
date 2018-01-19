<%@ Page language="c#" Inherits="expenses.admingroups" MasterPageFile="~/exptemplate.master" Codebehind="admingroups.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>



				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aegroup.aspx" class="submenuitem"><asp:Label id="Label1" runat="server">Add Signoff Group</asp:Label></a>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">				
<script language="javascript" type="text/javascript">
				
					
				function deleteGroup(groupid)
				{
					if (confirm('Are you sure you wish to delete the selected group?'))
					{
						PageMethods.DeleteGroup(accountid,groupid,deleteGroupComplete);
					}
				}
				
				function deleteGroupComplete(data)
				{
				    switch (data) {
				        case -1:
				            alert('The signoff group cannot be deleted as it is assigned to an employee(s).');
                            break;
                        case -2:
                            alert('The signoff group cannot be deleted as it is assigned to an employee(s) advance group');
                            break;
				        case -10:
				            alert('The signoff group cannot be deleted as it is assigned to one or more GreenLights or user defined field records');
				            break;
				        default:
				            SEL.Grid.deleteGridRow('groupsGrid', data);
				            break;
				    }
				    
				}
</script>

	
    <div class="inputpanel">
        <div class="formpanel formpanel_padding">
            <asp:Literal runat="server" ID="litSignOffGroups"></asp:Literal>
        </div>        
    </div>
    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
    </asp:Content>


