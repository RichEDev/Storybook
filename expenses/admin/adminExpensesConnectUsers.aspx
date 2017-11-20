<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" CodeBehind="adminExpensesConnectUsers.aspx.cs" Inherits="expenses.admin.adminExpensesConnectUsers" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>



<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">

    <script type="text/javascript" language="javascript">

        var licenseExists = false;
        
        function deleteUser(employeeid) 
        {
            if (confirm('Are you sure you wish to delete the selected employee expensesConnect license?')) {
                PageMethods.deleteEmployeeLicense(accountid, employeeid);
                var grid = igtbl_getGridById(contentID + 'gridExpensesConnectUsers');

                grid.Rows.remove(grid.getActiveRow().getIndex());

                noOfUsedLicenses--;
                var lbl = $get('<%= lblNoOfLicenses.ClientID %>');
                lbl.innerHTML = noOfUsedLicenses;
            }
        }

        function showAddUserPanel() 
        {
            $find('<%= modadduser.ClientID %>').show();
        }

        function hideAddUserPanel() 
        {
            $find('<%= modadduser.ClientID %>').hide();
        }

        function userHasLicense() 
        {
            var surnameVal = $get('<%= txtsurname.ClientID %>').value;
            if (surnameVal != "") 
            {
                PageMethods.checkLicenseExists(accountid, surnameVal, userHasLicenseComplete);
            }
        }

        function userHasLicenseComplete(boolResult) 
        {
            licenseExists = boolResult;
        }

        function validateIfUserHasLicense(sender, args) 
        {
            if (licenseExists) 
            {
                args.IsValid = false;
                licenseExists = false;
            }
            else 
            {
                args.IsValid = true
            }
            
            licenseExists = false;
        }
        
        function validateNoOfUsers(sender, args) 
        {
            var count = noOfUsedLicenses + 1;
            
            if (count > noOfLicenses)
            {
                args.IsValid = false;
            }
            else 
            {
                args.IsValid = true;
            }
        }

        function setNoOfUsers() 
        {
            if (Page_ClientValidate('vgUserLicense') == false) {
                return false;
            }

            noOfUsedLicenses++;

            hideAddUserPanel();
            
        }

        function clearSurname() 
        {
            $get('<%= txtsurname.ClientID %>').value = "";
        }
    
    </script>
    
    <a href="javascript:showAddUserPanel()" class="submenuitem" ><asp:Label id="lblAddUser" runat="server" meta:resourcekey="lblAddUserResource1">Add User</asp:Label></a>

</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <asp:Label ID="lblNoOfLicensesDesc1" runat="server"></asp:Label>
        <asp:Label ID="lblNoOfLicenses" runat="server"></asp:Label>
        <asp:Label ID="lblNoOfLicensesDesc2" runat="server"></asp:Label>
        
        <igtbl:UltraWebGrid ID="gridExpensesConnectUsers" runat="server" SkinID="gridskin" 
                OnInitializeLayout="gridExpensesConnectUsers_InitializeLayout" 
                OnInitializeRow="gridExpensesConnectUsers_InitializeRow"> 
        </igtbl:UltraWebGrid>
    </div>
    <asp:Panel ID="pnlAddUser" runat="server" CssClass="modalpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblAddEmployee" runat="server" Text="Add Employee" meta:resourcekey="lblAddEmployeeResource1"></asp:Label></div>
                
                <div class="valdiv" id="valdiv"><asp:validationsummary ValidationGroup="vgUserLicense" id="vgAeExpenses" runat="server" Width="100%" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="vtrsummaryResource1"></asp:validationsummary><br /><asp:label id="lblmsg" runat="server" Visible="False" Font-Size="Small" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:label></div>
                
                <div class="inputpanel">
                    
                    <asp:CustomValidator ClientValidationFunction="validateNoOfUsers" ID="custNoOfUsers" runat="server" ErrorMessage="You cannot allocate any more expensesConnect licenses as you have reached your maximum limit." ValidationGroup="vgUserLicense"></asp:CustomValidator><br />
                    <asp:CustomValidator ClientValidationFunction="validateIfUserHasLicense" ID="custUserHasLicense" runat="server" ErrorMessage="This user already has an expensesConnect license allocated." ValidationGroup="vgUserLicense"></asp:CustomValidator>
                    
                    <table>
                        <tr>
		                    <td class="labeltd">
			                    <asp:Label id="lblsurname" runat="server" meta:resourcekey="lblsurnameResource1">Enter surname of employee (or lead characters)</asp:Label>
		                    </td>
		                    <td class="inputtd">
			                    <asp:TextBox id="txtsurname" runat="server" Width=250 onblur="userHasLicense()" meta:resourcekey="txtsurnameResource1"></asp:TextBox>
                                <cc1:AutoCompleteExtender ID="autoSurname" runat="server" TargetControlID="txtsurname" ServicePath="~/svcAutocomplete.asmx" ServiceMethod="getEmployeeNameAndUsername" CompletionSetCount="1" MinimumPrefixLength="1" EnableCaching=true></cc1:AutoCompleteExtender>
		                    </td>
		                    <td>
		                        <asp:RequiredFieldValidator ID="reqSurname" runat="server" ErrorMessage="Please enter an Employee in the box provided" Text="*" ControlToValidate="txtsurname" ValidationGroup="vgUserLicense"></asp:RequiredFieldValidator>
		                    </td>
		                </tr>
	                </table>
	            </div>
           
                <div class="inputpanel">
                   <asp:ImageButton OnClientClick="javascript:setNoOfUsers()" ID="cmdadduser" runat="server" onclick="cmdadduser_Click" CausesValidation="False" onblur="clearSurname()" ImageUrl="~/shared/images/buttons/btn_save.png"/>
                   &nbsp;&nbsp;
                   <asp:ImageButton ID="cmdaddcancel" ImageUrl="~/buttons/cancel_up.gif"
                       runat="server" CausesValidation="False" />
                </div>
        </asp:Panel>
      
      <cc1:ModalPopupExtender ID="modadduser" runat="server" TargetControlID="lnkaddemp" PopupControlID="pnlAddUser" BackgroundCssClass="modalBackground" CancelControlID="cmdaddcancel"></cc1:ModalPopupExtender>      
            <asp:LinkButton ID="lnkaddemp" runat="server" style="display:none;">LinkButton</asp:LinkButton>
</asp:Content>
