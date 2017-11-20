<%@ Page Language="c#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="import_statement.aspx.cs" Inherits="expenses.admin.import_statement" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%@ Register src="unallocatedcardnumbers.ascx" tagname="unallocatedcardnumbers" tagprefix="uc2" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
    <!--[if lt IE 9]>
    <style>
        INPUT[type=file]{
            border: 1px solid #bfbfbf!important;
        }  
    </style>     
<![endif]-->

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy ID="smpCreditCards"  runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/sel.CreditCards.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webServices/svcCreditCards.asmx" InlineScript="true" />
        </Services>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">

        var cardCompanyTableID = '<%=tblCardCompanies.ClientID %>';
    </script>

    <div class="inputpanel">
     <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                     <asp:Panel ID="pnlWizard" runat="server">
    <asp:Wizard ID="wizstatement" runat="server"
    CancelButtonImageUrl="/shared/images/buttons/btn_close.png" CancelButtonType="Image" 
        CancelDestinationPageUrl="~/admin/statements.aspx" DisplayCancelButton="True" 
        DisplaySideBar="False" 
        FinishCompleteButtonImageUrl="~/buttons/pagebutton_finish.gif" 
        FinishDestinationPageUrl="~/admin/statements.aspx"
        FinishCompleteButtonType="Image" 
        FinishPreviousButtonImageUrl="~/buttons/pagebutton_previous.gif" 
        StartNextButtonImageUrl="~/buttons/pagebutton_next.gif" 
        StartNextButtonType="Image"
        FinishPreviousButtonType="Image"  
        onactivestepchanged="wizstatement_ActiveStepChanged" 
        onnextbuttonclick="wizstatement_NextButtonClick" ActiveStepIndex="0" OnPreviousButtonClick="wizstatement_PreviousButtonClick">
        <StepNavigationTemplate>
            <asp:ImageButton ImageUrl="~/buttons/pagebutton_next.gif" runat="server" ID="cmdNext" CommandName="MoveNext" OnClientClick="SEL.CreditCards.SaveCardCompanies();" />
            <asp:ImageButton ImageUrl="~/buttons/pagebutton_previous.gif"  runat="server" ID="cmdPrevious" CommandName="MovePrevious" />
            <asp:ImageButton ImageUrl="/shared/images/buttons/btn_close.png" runat="server" ID="cmdCancel" PostBackUrl="~/admin/statements.aspx" CommandName="Cancel" />
        </StepNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1">

                <table>
                    <tr><td class="labeltd">Card Provider</td><td class="inputtd">
                        <asp:DropDownList ID="cmbprovider" runat="server">
                        </asp:DropDownList>
                        <asp:RangeValidator ID="rvProvider" runat="server" ErrorMessage="*" Text="*" ControlToValidate="cmbprovider" MaximumValue="999" MinimumValue="1" Type="Integer"></asp:RangeValidator>
                    </td></tr>
                    <tr><td class="labeltd">Statement File</td><td class="inputtd">
                        <asp:FileUpload ID="fuStatement" runat="server" Width="250" Height="25" /></td></tr>
                </table>

            </asp:WizardStep>
            <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                <div class="inputpaneltitle"><asp:Label id="lblCardCompanies" runat="server">Card Companies to Import</asp:Label></div>
                <asp:Table CssClass="datatbl" ID="tblCardCompanies" runat="server">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell Text="Card Company"></asp:TableHeaderCell>
                        <asp:TableHeaderCell Text="Use for import"></asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                
                </asp:Table>
                
            </asp:WizardStep>
            <asp:WizardStep ID="WizardStep3" runat="server" Title="Step 3">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" />
                <table>
                    <tr><td class="labeltd">Statement Name</td><td class="inputtd">
                        <asp:TextBox ID="txtname" runat="server"></asp:TextBox></td><td>
                            <asp:RequiredFieldValidator ID="reqname" runat="server" ErrorMessage="Please enter a Statement Name in the box provided" ControlToValidate="txtname" Text="*"></asp:RequiredFieldValidator></td></tr>
                    <tr><td class="labeltd">Statement Date</td><td class="inputtd">
                        <asp:TextBox ID="txtstatementdate" runat="server"></asp:TextBox></td><td>
                        <asp:CompareValidator ID="compdate" runat="server" ControlToValidate="txtstatementdate" Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="The statement date you have entered is not a valid date"></asp:CompareValidator></td></tr>
                </table>
            </asp:WizardStep>
            <asp:WizardStep runat="server" ID="stepmatch">
                
                <uc2:unallocatedcardnumbers ID="usrunallocatedcardnumbers" runat="server" />
                
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
                    </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="scriptsContent" runat="server" ContentPlaceHolderID="scripts">
     <script type="text/javascript">
         //Bootstrap td & th style override for buttons
         $(document).ready(function () {
             $('#<%=wizstatement.ClientID%> table').find('[id$="ImageButton"]').css('margin-right', '15px');
             $('#<%=wizstatement.ClientID%> table').find('[id$="ImageButton"]').css('margin-top', '10px');
             $('#<%=wizstatement.ClientID%> table').find('[id$="CancelImageButton"]').css('margin-right', '0px');
         });
     </script>
</asp:Content>

