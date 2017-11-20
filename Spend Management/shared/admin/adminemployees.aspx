<%@ Page language="c#" Inherits="Spend_Management.adminemployees" EnableSessionState="True" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminemployees.aspx.cs" EnableViewState="false" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


	
	
					<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
                <asp:Panel ID="pnlAddNewEmployee" runat="server">
					<a href="aeemployee.aspx" class="submenuitem"><asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">New Employee</asp:Label></a>
					</asp:Panel>
					</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/employees.js?date=20161112" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcEmployees.asmx" InlineScript="false"  />
        </Services>
    </asp:ScriptManagerProxy>
				
	<script type="text/javascript" language="javascript">
        var cmbFilterID = '<%=cmbfilter.ClientID %>';
        var pnlGridID = '<%=pnlGrid.ClientID %>';
       
    </script>
    
		<div class="formpanel formpanel_padding">
		    <div class="twocolumn">
			    <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="cmbfilter">Display Filter</asp:Label>
			    <span class="inputs"><asp:DropDownList id="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCmb('gridEmployees',event);" meta:resourcekey="cmbfilterResource1">
				    <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">All Employees</asp:ListItem>
				    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Archived Employees</asp:ListItem>
				    <asp:ListItem Value="0" meta:resourcekey="ListItemResource3">Un-archived Employees</asp:ListItem>
			    </asp:DropDownList></span>                    	       
		    </div>
		    
            <asp:Panel runat="server" ID="pnlGrid">
                <asp:Literal ID="litgrid" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div> 
        </div>
    
        
    </asp:Content>
	

