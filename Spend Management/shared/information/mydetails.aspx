<%@ Page Language="c#" Inherits="Spend_Management.mydetails" MasterPageFile="~/masters/smForm.master"
    CodeBehind="mydetails.aspx.cs" %>

<%@ Register Src="~/shared/usercontrols/CostCentreBreakdown.ascx" TagName="ccb" TagPrefix="ccb" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
     .sectiontitle {
         margin-top: 30px;
     }
    
      #ctl00_contentmain_ccb_tblCostCentreBreakdown td{
          text-align:left!important;
      } 

      #ctl00_contentmain_ccb_tblCostCentreBreakdown td select {
              width: 99%!important;
              height: 25px;
      }

      #ctl00_contentmain_ccb_tblCostCentreBreakdown td input{
          width:99%!important;
          height:19px;
      }

      #ctl00_contentmain_txtTitle{
          margin-left:-2px;
      }

      #ctl00_contentmain_lblfirstname{
          margin-left:-10px;
      }
  </style>
</asp:Content>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
     
    <script language="javascript" type="text/javascript">
        var employeewnd;
        function employee_onclick(from) {
            window.name = 'main';
            employeewnd = window.open('../employeesearch.aspx', 'search', 'width=600, height=600,scrollbars=yes');
        }

        var divEsrDetailsID = '<% = divEsrDetails.ClientID %>';
        var lblEsrDetailsTitleID = '<% = lblEsrDetailsTitle.ClientID %>';
        var modEsrDetailsID = '<% = modEsrDetail.ClientID %>';
        var pnlEsrDetailsID = '<% = pnlEsrDetails.ClientID %>';
    </script>

        <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/employees.js?date=20161112" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div class="valdiv">
        <asp:Label ID="lblmsg" runat="server" Visible="False" ForeColor="Red" Font-Size="Small" meta:resourcekey="lblmsgResource1">Label</asp:Label>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="false" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
    </div>
    
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">
            <asp:Label ID="lbllogondetails" runat="server" Text="Logon Details" meta:resourcekey="lbllogondetailsResource1"></asp:Label></div>
        <div class="twocolumn">
            <asp:Label ID="lblusernamel" runat="server" meta:resourcekey="lblusernamelResource1" AssociatedControlID="lblusername">Username</asp:Label><span class="inputs">
                  <asp:TextBox runat="server" ID="lblusername" ReadOnly="true" CssClass="fillspan" MaxLength="50"></asp:TextBox>
                  </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            <asp:Label ID="lblpassword" runat="server" meta:resourcekey="lblpasswordResource1" AssociatedControlID="cmbchangep">Password</asp:Label><span class="inputs">
            <asp:LinkButton ID="cmbchangep" runat="server" OnClick="cmbchangep_Click" meta:resourcekey="cmbchangepResource1" CausesValidation="false">Change Password</asp:LinkButton>
            </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
        
        <div class="sectiontitle">
            <asp:Label ID="lblname" runat="server" Text="Employee Name" meta:resourcekey="lblnameResource1"></asp:Label></div>
        <div class="twocolumn">
            <asp:Label ID="lblnamemsg" runat="server" Text="Your name and employment contact details can be updated by amending the details here and clicking save." meta:resourcekey="lblnamemsgResource1"></asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lbltitle" runat="server" meta:resourcekey="lbltitleResource1" AssociatedControlID="txtTitle" CssClass="mandatory">Title *</asp:Label>
            <span class="inputs">
            <asp:TextBox ID="txtTitle" runat="server" CssClass="fillspan" MaxLength="20"></asp:TextBox>
            </span>
            
            <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span>&nbsp;<span class="inputvalidatorfield">
             <asp:RequiredFieldValidator ID="reqtitle" runat="server" ErrorMessage="Please enter your title in the box provided" ControlToValidate="txtTitle" meta:resourcekey="reqtitleResource1" Display="Dynamic">*</asp:RequiredFieldValidator>&nbsp;
            </span>

            <asp:Label ID="lblfirstname" runat="server" meta:resourcekey="lblfirstnameResource1" AssociatedControlID="txtfirstname" CssClass="mandatory">Firstname *</asp:Label>
            <span class="inputs"><asp:TextBox ID="txtfirstname" runat="server" meta:resourcekey="txtfirstnameResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">
<asp:RequiredFieldValidator ID="reqfirstname" runat="server" ErrorMessage="Please enter your Firstname in the box provided" ControlToValidate="txtfirstname" meta:resourcekey="reqfirstnameResource1" Display="Dynamic">*</asp:RequiredFieldValidator>
</span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblsurname" runat="server" meta:resourcekey="lblsurnameResource1" AssociatedControlID="txtsurname" CssClass="mandatory">Surname *</asp:Label><span class="inputs">
                <asp:TextBox ID="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqsurname" runat="server" ErrorMessage="Please enter your Surname in the box provided" ControlToValidate="txtsurname" meta:resourcekey="reqsurnameResource1" Display="Dynamic">*</asp:RequiredFieldValidator></span>
        </div>
        
        <div class="sectiontitle">
            <asp:Label ID="lblcontactdetails" runat="server" Text="Employment Contact Details" meta:resourcekey="lblcontactdetailsResource1"></asp:Label></div>
        <div class="twocolumn">
            <asp:Label ID="lblextno" runat="server" meta:resourcekey="lblextnoResource1" AssociatedControlID="txtextension">Extension Number</asp:Label><span class="inputs"><asp:TextBox ID="txtextension" runat="server" meta:resourcekey="txtextensionResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblmobileno" runat="server" meta:resourcekey="lblmobilenoResource1" AssociatedControlID="txtmobileno">Mobile Number</asp:Label><span class="inputs"><asp:TextBox ID="txtmobileno" runat="server" meta:resourcekey="txtmobilenoResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblpagerno" runat="server" meta:resourcekey="lblpagernoResource1" AssociatedControlID="txtpagerno">Pager Number</asp:Label><span class="inputs"><asp:TextBox ID="txtpagerno" runat="server" meta:resourcekey="txtpagernoResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblemail" runat="server" meta:resourcekey="lblemailResource1" AssociatedControlID="txtemail">E-mail Address</asp:Label><span class="inputs"><asp:TextBox ID="txtemail" runat="server" MaxLength="200" meta:resourcekey="txtemailResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="regexemail" ControlToValidate="txtemail" Text="*" ErrorMessage="Invalid email address specified in employment contact details" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator></span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lbltelno" runat="server" meta:resourcekey="lbltelnoResource1" AssociatedControlID="txttelno">Tel Number</asp:Label><span class="inputs"><asp:TextBox ID="txttelno" runat="server" meta:resourcekey="txttelnoResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblemailhome" AssociatedControlID="txtemailhome">Personal E-mail Address</asp:Label><span class="inputs"><asp:TextBox ID="txtemailhome" runat="server" meta:resourcekey="txtemailhomeResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="regexemailhome" ControlToValidate="txtemailhome" ErrorMessage="Invalid email address specified for personal email" Text="*" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator></span>
        </div>
        
        <div class="twocolumn">
            <asp:Literal ID="litnotifyadmin" runat="server" meta:resourcekey="litnotifyadminResource1"></asp:Literal>
        </div>
        <div class="sectiontitle">
            <asp:Label ID="lblemployeedetails" runat="server" Text="Employee Details" meta:resourcekey="lblemployeedetailsResource1"></asp:Label></div>
        <div id="divEmpDetails">
        <div class="twocolumn">
                <asp:Label ID="lblpayroll" runat="server" meta:resourcekey="lblpayrollResource1" AssociatedControlID="txtpayroll">Payroll No</asp:Label><span class="inputs"><input id="txtpayroll" disabled="disabled" type="text" runat="server" class="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblpersonalmiles" runat="server" Text="Number Personal Miles (Current Tax Year)" meta:resourcekey="lblpersonalmilesResource1" AssociatedControlID="txtpersonalmiles"></asp:Label><span class="inputs"><asp:TextBox ID="txtpersonalmiles" runat="server" meta:resourcekey="txtpersonalmilesResource1" CssClass="fillspan" MaxLength="50" disabled="disabled"></asp:TextBox></span>
        </div>
        <div class="twocolumn">
                <asp:Label ID="lblcreditor" runat="server" meta:resourcekey="lblcreditorResource1" AssociatedControlID="txtcreditor">Creditor/ Purchase Ledger Number</asp:Label><span class="inputs"><input id="txtcreditor" disabled="disabled" type="text" runat="server" class="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblmileage" runat="server" meta:resourcekey="lblmileageResource1" AssociatedControlID="txtmileage">Current Mileage</asp:Label><span class="inputs"><asp:TextBox ID="txtmileage" runat="server" disabled="disabled" meta:resourcekey="txtmileageResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblposition" runat="server" meta:resourcekey="lblpositionResource1" AssociatedControlID="txtposition">Position</asp:Label><span class="inputs"><input id="txtposition" disabled="disabled" type="text" runat="server" class="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
         <div id="divEmailNotification">
        <div class="sectiontitle">
            <asp:Label ID="Label3" runat="server" Text="Email Notifications" meta:resourcekey="lbllogondetailsResource1"></asp:Label></div>
        <div class="twocolumn">
          <asp:Label ID="lblNotifyUnsubmission" runat="server" meta:resourcekey="lblNotifyUnsubmissionResource1" AssociatedControlID="chkNotifyUnsubmission">Notify me if a claim I'm approving is unsubmitted</asp:Label>
          <span class="inputs"><asp:CheckBox ID="chkNotifyUnsubmission" ClientIDMode="Static" runat="server" /></span>
          <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span></div>
        <asp:Literal runat="server" ID="litESRAssignments"></asp:Literal>  
        </div>
    <div id="divCCBreakdown">
        <asp:Panel ID="pnlCCB" runat="server">
            <ccb:ccb ID="ccb" runat="server" UserControlDisplayType="Inline" HideButtons="true" />
        </asp:Panel>
    </div>
    <br />
    <div class="formpanel formpanel_padding">
    <div id="divExpensesDetails">
            <div class="sectiontitle">
                <asp:Label ID="lblcardeetails" runat="server" Text="Vehicle Details" meta:resourcekey="lblcardeetailsResource1"></asp:Label></div>
        
            <asp:Literal ID="litcars" runat="server" meta:resourcekey="litcarsResource1"></asp:Literal>
            <asp:Literal ID="litCars2" runat="server"></asp:Literal>
                <div id="divBankDeatils">
            <div class="sectiontitle" >
                <asp:Label ID="lblBankDetails" runat="server" Text="Bank Details"></asp:Label></div>
        <div class="twocolumn" style="display: inline;">
            <asp:Literal ID="litbankdetails" runat="server" meta:resourcekey="litbankdetailsResource1"></asp:Literal>
        </div>
             </div>
        <div id="divClaimApproval">
                <div class="sectiontitle">
                    <asp:Label ID="lblclaimapproval" runat="server" Text="Claim Approval Stages" meta:resourcekey="lblclaimapprovalResource1"></asp:Label></div>
        <div class="twocolumn">
            <asp:Label AssociatedControlID="lblsignoff" ID="lblsignoffgroup" runat="server" Text="Signoff Group" meta:resourcekey="lblsignoffgroupResource1"></asp:Label><span class="inputs"><asp:Label ID="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1">Label</asp:Label></span>
        </div>
        <div class="twocolumn">
            <asp:Literal ID="litstages" runat="server" meta:resourcekey="litstagesResource1"></asp:Literal>
        </div>
        </div>
            <div class="sectiontitle" runat="server" id="divHomeAddress">
                <asp:Label ID="Label1" runat="server" Text="Home Addresses"></asp:Label></div>
        <asp:Literal ID="lithomeaddresses" runat="server"></asp:Literal>
            <div class="sectiontitle">
                <asp:Label ID="Label2" runat="server" Text="Work Addresses"></asp:Label></div>
        <asp:Literal ID="litworkaddresses" runat="server"></asp:Literal>
        </div>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1" OnClientClick="saveButtonClick();" ></asp:ImageButton>&nbsp;
            <asp:ImageButton ID="cmdcancel" runat="server" ImageUrl="~/shared/images/new-buttons/btn_close.png" meta:resourcekey="cmdcancelResource1" CausesValidation="False"></asp:ImageButton>
        </div>    
    </div>
    <asp:Panel ID="pnlEsrDetails" runat="server" CssClass="modalpanel" Style="display: none; width: 890px; height: 600px;">
        <div class="sm_panel">
            <div class="sectiontitle">
                <asp:Label ID="lblEsrDetailsTitle" runat="server" Text="Esr Details"></asp:Label>
            </div>
            <div id="divEsrDetails" runat="server" class="sm_panel" style="overflow: auto; height: 450px">
            </div>
            <div class="formbuttons">
                <asp:ImageButton ID="btnCancelEsr" OnClientClick="javascript:hideEsrDetailsModal();"
                    ImageUrl="~/shared/images/buttons/btn_close.png" runat="server" CausesValidation="False" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="modEsrDetail" TargetControlID="lnkEsrDetails" PopupControlID="pnlEsrDetails" BackgroundCssClass="modalBackground" CancelControlID="btnCancelEsr" />
    <asp:LinkButton ID="lnkEsrDetails" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".sectiontitle:first").css("margin-top", 0);
        });
    </script>
    
</asp:Content>
