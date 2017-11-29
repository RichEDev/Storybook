<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="bankAccounts.ascx.cs" Inherits="Spend_Management.shared.usercontrols.bankAccounts" %>

<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<%@ Register assembly="SpendManagementHelpers" namespace="SpendManagementHelpers" tagprefix="cc2" %>

<asp:ScriptManagerProxy ID="usemobiledevice_smp" runat="server">
    <Services>
        <asp:ServiceReference Path="../webServices/svcBankAccounts.asmx" InlineScript="false" />
        <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
    </Services>
    <Scripts>
        <asp:ScriptReference Name="tooltips" />
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.bankAccounts.js" />
    </Scripts>
</asp:ScriptManagerProxy>

    <asp:Literal ID="litBankAccounts" runat="server"></asp:Literal>
    
    <asp:Panel ID="AddEditAccounts" runat="server" class="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle" id="divBankAccountHeader">&nbsp;</div>
            <div class="twocolumn">
                <asp:Label AssociatedControlID="txtAccountName" ID="lblAccountName" runat="server" CssClass="mandatory">Account Name*</asp:Label><span class="inputs"><asp:TextBox ID="txtAccountName" runat="server" MaxLength="100" onchange ="OnBankDetailsTextBoxChanged()" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip533" onmouseover="SEL.Tooltip.Show('B4EAEA4B-2141-456A-886F-22EEFF1B22AD', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgAddEditAccount" ID="rfAccountName" runat="server" ErrorMessage="Please enter an Account Name." ControlToValidate="txtAccountName">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="retxtAccountName" ErrorMessage="Account Name should not contain special characters." ValidationGroup="vgAddEditAccount" ValidationExpression="^[a-zA-Z0-9\-.\s]+$" ControlToValidate="txtAccountName" Display="None"></asp:RegularExpressionValidator></span>

                <asp:Label AssociatedControlID="txtAccountNumber" ID="lblAccountNumber" runat="server" CssClass="mandatory">Account Number*</asp:Label><span class="inputs"><asp:TextBox ID="txtAccountNumber" runat="server" MaxLength="20" onchange ="OnBankDetailsTextBoxChanged()" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip532" onmouseover="SEL.Tooltip.Show('6A8A56DC-9866-46D0-B937-884DDD59FEEA', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgAddEditAccount" ID="rfAccountNumber" runat="server" ErrorMessage="Please enter an Account Number." ControlToValidate="txtAccountNumber" InitialValue="">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="retxtAccountNumber" ErrorMessage="The value you have entered for Account Number is invalid. Valid characters are the numbers 0-9." ValidationGroup="vgAddEditAccount"  ValidationExpression="^\d+" ControlToValidate="txtAccountNumber" Display="None"></asp:RegularExpressionValidator>
                    <asp:RegularExpressionValidator runat="server" ClientIDMode="Static"  ID="retxtaccountnumberlength" Enabled="false" ErrorMessage="The Account Number must consist of 7 or 8 characters." ValidationExpression="^.{7,8}$" ValidationGroup="vgAddEditAccount" ControlToValidate="txtAccountNumber" Display="None"></asp:RegularExpressionValidator></span>
            </div>
            
            <div class="twocolumn">
                <asp:Label AssociatedControlID="cmbAccounttype" ID="lblAccountType" runat="server" CssClass="mandatory">Account Type*</asp:Label><span class="inputs"><asp:DropDownList ID="cmbAccounttype" onchange ="OnBankDetailsTextBoxChanged()" runat="server" ValidationGroup="vgAddEditAccount"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="img1" onmouseover="SEL.Tooltip.Show('af2e35eb-1c19-43fd-9954-629866bed9ca', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                                                                                                                                                                                                                              <asp:CompareValidator ID="rvAccountType" runat="server" ControlToValidate="cmbAccounttype" ErrorMessage="Please select an Account Type." Operator="GreaterThan" Type="Integer" ValueToCompare="0"  ValidationGroup="vgAddEditAccount">*</asp:CompareValidator></span>

                <asp:Label AssociatedControlID="txtSortCode" ClientIDMode="Static" ID="lblSortCode" runat="server">Sort Code</asp:Label><span class="inputs"><asp:TextBox ID="txtSortCode" runat="server" MaxLength="100" onchange ="OnBankDetailsTextBoxChanged()" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img2" onmouseover="SEL.Tooltip.Show('34C7AAD7-F58A-40FA-B7AD-140FD5A61910', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
               <span class="inputvalidatorfield"> <asp:RegularExpressionValidator runat="server" ID="retxtSortCode" ErrorMessage="Sort Code should not contain special characters." ValidationGroup="vgAddEditAccount" ValidationExpression="^[a-zA-Z0-9\-.\s]+$" ControlToValidate="txtSortCode" Display="None"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ValidationGroup="vgAddEditAccount" ClientIDMode="Static"  ID="retxtSortCodeEmptyCheck" runat="server" ErrorMessage="Please enter a Sort Code." ControlToValidate="txtSortCode" Enabled="false">*</asp:RequiredFieldValidator>
                  <asp:RegularExpressionValidator runat="server" ClientIDMode="Static"  ID="retxtSortCodeNumberCheck" ErrorMessage="The value you have entered for Sort Code is invalid.Valid characters are the numbers 0-9." ValidationGroup="vgAddEditAccount" ValidationExpression="^\d+$" ControlToValidate="txtSortCode" Enabled="False" Display="None"></asp:RegularExpressionValidator>
                 <asp:RegularExpressionValidator runat="server" ClientIDMode="Static"  ID="retxtSortCodeCharactersCheck" ErrorMessage="The Sort Code must consist of 6 characters." ValidationGroup="vgAddEditAccount" ValidationExpression="^[a-zA-Z0-9]{6}$" ControlToValidate="txtSortCode" Enabled="False" Display="None"></asp:RegularExpressionValidator></span>
            </div>
            
            <div class="twocolumn">
                <asp:Label AssociatedControlID="txtReference" ID="lblReference" runat="server">Reference</asp:Label><span class="inputs"><asp:TextBox ID="txtReference" runat="server" MaxLength="20" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img3" onmouseover="SEL.Tooltip.Show('B648AFAC-3841-4956-B369-69585A3791AF', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                  <asp:RegularExpressionValidator runat="server" ID="retxtReference" ErrorMessage="The value you have entered for Reference is invalid. Valid characters are alphanumerics and special characters '-.,/:\_'" ValidationGroup="vgAddEditAccount" ValidationExpression="^[-.,/\\:\w\s]*$" ControlToValidate="txtReference" Display="None"></asp:RegularExpressionValidator>


                <asp:Label AssociatedControlID="ddlCurrency" ID="lblCurrency" runat="server" CssClass="mandatory">Account Currency*</asp:Label><span class="inputs"><asp:DropDownList ID="ddlCurrency" runat="server" onchange ="OnBankDetailsTextBoxChanged()" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img4" onmouseover="SEL.Tooltip.Show('83E19A12-689A-4BDF-B70F-797C9BF88440', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                <asp:CompareValidator ID="rvCurrency" runat="server" ControlToValidate="ddlCurrency" ErrorMessage="Please select an Account Currency." Operator="GreaterThan" Type="Integer" ValidationGroup="vgAddEditAccount" ValueToCompare="0">*</asp:CompareValidator>                                                                                                                                                                                                                                                                                                                                                            </span>
            </div>

             <div class="twocolumn">
            
            <asp:Label AssociatedControlID="ddlCountry" ID="lblCountry" runat="server" CssClass="mandatory">Country*</asp:Label><span class="inputs" ><asp:DropDownList ClientIDMode="Static" ID="ddlCountry" onchange ="OnBankDetailsTextBoxChanged()" runat="server"  onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img5" onmouseover="SEL.Tooltip.Show('B6AF95ED-C112-4F3F-921B-0318A95F93D4', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                <asp:CompareValidator ID="rvCountry" runat="server" ControlToValidate="ddlCountry" ErrorMessage="Please select a Country." Operator="GreaterThan" Type="Integer" ValidationGroup="vgAddEditAccount" ValueToCompare="0">*</asp:CompareValidator> </span>
                
                     <asp:Label AssociatedControlID="txtIban" ID="lblIban" runat="server">IBAN</asp:Label><span class="inputs"><asp:TextBox ID="txtIban" runat="server" MaxLength="34" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip532" onmouseover="SEL.Tooltip.Show('5C36049A-E73A-41EA-B331-AE1BFF52D8EE', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ErrorMessage="The IBAN specified must be between 5 and 34 characters." ValidationGroup="vgAddEditAccount"  ValidationExpression="^.{5,34}$" ControlToValidate="txtIban" Display="None"></asp:RegularExpressionValidator><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" ErrorMessage="The value you have entered for IBAN is invalid. Only letters and numbers are allowed." ValidationGroup="vgAddEditAccount"  ValidationExpression="^[\w\d_.-]+$" ControlToValidate="txtIban">*</asp:RegularExpressionValidator>
                         </span>
           </div>
        <div class="twocolumn">
            <asp:Label AssociatedControlID="txtSwiftCode" ID="lblSwiftCode" runat="server">SWIFT Code</asp:Label><span class="inputs"><asp:TextBox ID="txtSwiftCode" runat="server" MaxLength="11" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip532" onmouseover="SEL.Tooltip.Show('6B50F6C0-5334-4DA7-B768-AE07174DC043', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ErrorMessage="The SWIFT Code specified must be between 8 and 11 characters." ValidationGroup="vgAddEditAccount"  ValidationExpression="^.{8,11}$" ControlToValidate="txtSwiftCode" Display="None"></asp:RegularExpressionValidator><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ErrorMessage="The value you have entered for SWIFT Code is invalid. Only letters and numbers are allowed." ValidationGroup="vgAddEditAccount"  ValidationExpression="^[\w\d_.-]+$" ControlToValidate="txtSwiftCode">*</asp:RegularExpressionValidator>
            </span>
            </div>

            <asp:Panel ID="pnlAddEditAccountButtons" runat="server" CssClass="formbuttons"></asp:Panel>

        </div>

        <script type="text/javascript">
     function OnBankDetailsTextBoxChanged() {

         var selectedCountry = document.getElementById("ddlCountry");
         var countryValue = selectedCountry.options[selectedCountry.selectedIndex].text;
         if (countryValue != null && countryValue == 'United Kingdom')
         {
             document.getElementById("lblSortCode").className = "mandatory"
             document.getElementById("lblSortCode").innerText = "Sort Code*"
             document.getElementById("<%=retxtaccountnumberlength.ClientID%>").enabled = true;
             document.getElementById("<%=retxtSortCodeNumberCheck.ClientID%>").enabled = true;
             document.getElementById("<%=retxtSortCodeEmptyCheck.ClientID%>").enabled = true;
            document.getElementById("<%=retxtSortCodeCharactersCheck.ClientID%>").enabled = true;
         }
         else {
             document.getElementById("lblSortCode").className = ""
             document.getElementById("lblSortCode").innerText = "Sort Code"
               document.getElementById("<%=retxtaccountnumberlength.ClientID%>").enabled = false;
             document.getElementById("<%=retxtSortCodeNumberCheck.ClientID%>").enabled = false;
             document.getElementById("<%=retxtSortCodeEmptyCheck.ClientID%>").enabled = false;
             document.getElementById("<%=retxtSortCodeCharactersCheck.ClientID%>").enabled = false;
         }

     }

    </script>
    </asp:Panel>

    <asp:HyperLink ID="lnkAddEditAccount" runat="server" style="display:none">&nbsp;</asp:HyperLink>
    <cc1:ModalPopupExtender ID="modalAddEditAccount" runat="server" PopupControlID="AddEditAccounts" TargetControlID="lnkAddEditAccount"  BackgroundCssClass="modalBackground" ></cc1:ModalPopupExtender>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    
   
