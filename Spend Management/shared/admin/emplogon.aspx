<%@ Page language="c#" Trace="false" Inherits="Spend_Management.emplogon" MasterPageFile="~/masters/smTemplate.master" Codebehind="emplogon.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <script type="text/javascript" language="javascript">
        
        function changeEmployee() {
            var cmbEmployee = '<%=cmbemployee.ClientID %>';
            var employeeID;
            if (document.getElementById(cmbEmployee)== null) {
                employeeID = SEL.Grid.getSelectedItemFromGrid('gridEmployees');
            }
            else {
                employeeID = document.getElementById(cmbEmployee).options[document.getElementById(cmbEmployee).selectedIndex].value;
            }
            if (employeeID == 0) {
                alert('Please select an employee account to logon to');
                return;
            }
            PageMethods.logon(employeeID, delegateType, changeEmployeeComplete);
                        
        }

        function changeEmployeeComplete(data) {
            if (data === true) {
                document.location = '../../home.aspx?emplogon=1';
            } 
            else 
            {
                alert('You are currently logged in as another user already.');
            }
        }
    </script>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Selecting an employee below will allow you, as an administrator or delegate, to perform actions on their behalf. When you have finished, click "Logoff employee account" under the header bar to return here.</asp:Label></div>
        <asp:MultiView ID="mview" runat="server">
            <asp:View ID="viewsearch" runat="server">
            
                <div class="sectiontitle"><asp:Label id="Label2" runat="server" meta:resourcekey="Label1Resource1">Search Options</asp:Label></div>
		    <div class="twocolumn"><asp:Label id="lblsurname" AssociatedControlID="txtsurname" runat="server" meta:resourcekey="lblsurnameResource1">Enter surname of employee (or lead characters)</asp:Label><span class="inputs"><asp:TextBox id="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label id="lblusername" runat="server" meta:resourcekey="lblusernameResource1" AssociatedControlID="txtusername">Username</asp:Label><span class="inputs"><asp:TextBox id="txtusername" runat="server" meta:resourcekey="txtusernameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
		    <div class="onecolumnsmall" id="signoffDiv" runat="server"><asp:Label id="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1" AssociatedControlID="cmbgroups">Signoff Group</asp:Label><span class="inputs"><asp:DropDownList id="cmbgroups" runat="server" meta:resourcekey="cmbgroupsResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
		    <div class="onecolumnsmall"><asp:Label id="lblrole" runat="server" meta:resourcekey="lblroleResource1" AssociatedControlID="cmbroles">Role</asp:Label><span class="inputs"><asp:DropDownList id="cmbroles" runat="server" meta:resourcekey="cmbrolesResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
		    <div class="onecolumnsmall"><asp:Label id="lbldepartment" runat="server" meta:resourcekey="lbldepartmentResource1" AssociatedControlID="cmbdepartments">Default Department</asp:Label><span class="inputs"><asp:DropDownList id="cmbdepartments" runat="server" meta:resourcekey="cmbdepartmentsResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
		    <div class="onecolumnsmall"><asp:Label id="lblcostcode" runat="server" meta:resourcekey="lblcostcodeResource1" AssociatedControlID="cmbcostcodes">Default Costcode</asp:Label><span class="inputs"><asp:DropDownList id="cmbcostcodes" runat="server" meta:resourcekey="cmbcostcodesResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
		    <div class="formbuttons"><asp:ImageButton id="cmdsearch" runat="server" 
                    ImageUrl="~/shared/images/buttons/pagebutton_search.gif" 
                    meta:resourcekey="cmdokResource1" onclick="cmdsearch_Click"></asp:ImageButton></div>
            <div class="sectiontitle"><asp:Label ID="Label3" runat="server" Text="Search Results"></asp:Label></div>                    
                
                <asp:Literal ID="litresults" runat="server"></asp:Literal>
                
            </asp:View>
            <asp:View runat="server" ID="viewdropdown">
                <div class="twocolumn"><asp:Label id="lblemployee" runat="server" meta:resourcekey="lblemployeeResource1" AssociatedControlID="cmbemployee">Please select an employee</asp:Label><span class="inputs"><asp:DropDownList
                        ID="cmbemployee" runat="server">
                    </asp:DropDownList>
                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
            </asp:View>
        </asp:MultiView>
        <div class="formbuttons">
		<a href="javascript:changeEmployee();"><img src="/shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" 
            ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" 
            meta:resourcekey="cmdcancelResource1" onclick="cmdcancel_Click"></asp:ImageButton>
	</div>
    </div>    </asp:Content>

