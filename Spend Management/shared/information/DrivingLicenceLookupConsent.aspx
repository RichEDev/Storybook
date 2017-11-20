<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrivingLicenceLookupConsent.aspx.cs" MasterPageFile="~/masters/smForm.master" Inherits="Spend_Management.shared.information.DrivingLicenceLookupConsent" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">  
    
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">    
        <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
        <Scripts>
              <asp:ScriptReference Path="~/shared/javaScript/minify/jquery-selui-dialog.js" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.licencecheck.js" />
  </Scripts>

    </asp:ScriptManagerProxy>
   <script type="text/javascript">
       $(document).ready(function () {
           //Pass DOM elements to javascript variables
           SEL.DVLAConsent.DomIdentifiers.Firstname = '<%= this.txtFirstName.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.Surname = '<%= this.txtSurName.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.Email = '<%= this.txtEmail.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.DrivingLicenceNumber = '<%= this.txtDrivingLicenceNumber.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.DateOfBirth = '<%= this.txtDateOfBirth.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.Sex = '<%= this.ddlsex.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.Middlename = '<%= this.txtMiddleName.ClientID %>';

           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Firstname = '<%= this.txtFirstNameConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Surname = '<%= this.txtSurnameConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Email = '<%= this.txtEmailConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.DrivingLicenceNumber = '<%= this.txtDrivingLicenceNumberConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.DateOfBirth = '<%= this.txtDateOfBirthConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Sex = '<%= this.txtSexConfirmation.ClientID %>';
           SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Middlename = '<%= this.txtMiddleNameConfirmation.ClientID %>';

           SEL.DVLAConsent.LoaderImagePath = '<% = this.GetStaticLibraryPath() %>/icons/Custom/128/ajax-loader-grey-128.gif';
           SEL.DVLAConsent.InitializeDatePicker();

           SEL.DVLAConsent.SetupDialogs();
       });
      </script>
     <div class="formpanel formpanel_padding">
         <div class="sectiontitle">
             <asp:Label ID="lblconsentdetails" runat="server" Text="General Information" meta:resourcekey="lblcontactdetailsResource1"></asp:Label>
         </div>
         In order to validate your details held by DVLA, you will need to give consent every 3 years. This will provide us with permission to perform a check on your behalf.
         <br /><br />
         If you do not wish to provide consent or you are a non-DVLA licence holder you may opt out below, but please note we will not be able to automatically check your driving licence information.
         <div class="comment" id="instructions">
           <ol class="messagecomment">
             <li>
                 <p>Please enter your driving licence details in the fields below and then click submit.</p>
             </li>
             <li>
                 <p>You will be directed to the consent portal to re-verify the information that you have provided. Once you have verified this information, you will be able to provide consent for future checks.</p>
                 <p>Your organisation currently performs checks <asp:Literal runat="server" id="dvlaLookUpFrequencyValue"></asp:Literal>, but this is subject to change.</p>
             </li>
             <li>
                 <p>After successfully submitting your consent, you will receive a link to the consent portal and a secure key which is unique to you. This information will also be sent to the email address that you have provided.</p>
                 <p>You can revoke your consent at any time by returning to the consent portal, via the link provided, and then click Opt-Out. You will be required to enter your secure key to confirm.</p>
             </li>
           </ol>
         </div>
         <div id="consentDetailsHolder" runat="server" Visible="False">
            <div class="twocolumn">
             <asp:Label ID="lblConsentProvided" runat="server" meta:resourcekey="lblConsentProvidedResource" AssociatedControlID="txtDateOfBirth">Consent provided</asp:Label><span class="inputs"><asp:TextBox ID="txtConsentProvided" disabled="disabled" runat="server" meta:resourcekey="lblConsentProvidedResource" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskConsentProvided" MaskType="Date" Mask="99/99/9999" TargetControlID="txtConsentProvided" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calConsentDate" runat="server" TargetControlID="txtConsentProvided" Format="dd/MM/yyyy"></cc1:CalendarExtender></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgTooltipHeaderBGColour" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('83280464-277E-4360-B7F0-C673595AB8F8', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
            <asp:Label ID="lblConsentExpiryDate" runat="server" meta:resourcekey="lblConsentExpiryResource" AssociatedControlID="txtConsentExpiryDate">Consent expiry date</asp:Label><span class="inputs"><asp:TextBox ID="txtConsentExpiryDate" disabled="disabled" runat="server" meta:resourcekey="lblConsentExpiryResource" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskConsentExpiry" MaskType="Date" Mask="99/99/9999" TargetControlID="txtConsentExpiryDate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender><cc1:CalendarExtender ID="calConsentExpiry" runat="server" TargetControlID="txtConsentProvided" Format="dd/MM/yyyy"></cc1:CalendarExtender></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('141D75A9-7C87-4A1B-8126-7BA89E4157BB', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
         </div>
           <div class="twocolumn">
             <asp:Label ID="lblLookUpDate" runat="server" meta:resourcekey="lblLookUpDateResource" AssociatedControlID="txtLookUpDate">Last check date</asp:Label><span class="inputs"><asp:TextBox ID="txtLookUpDate" runat="server" disabled="disabled" meta:resourcekey="txtLookupDateResource1" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('40E1C9B4-3C7A-4B92-95D6-C74AB33964FF', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
         </div>
          <div class="onecolumnsmall">
             <asp:Label ID="lblSecurityCode" runat="server" meta:resourcekey="lblSecuritycodeResource" AssociatedControlID="txtSecurityCode">Secure key</asp:Label><span class="inputs"><asp:TextBox ID="txtSecurityCode" runat="server" disabled="disabled" meta:resourcekey="txtSecurityCodeResource1" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('A07E4D27-2EF6-4E81-B281-5370C645293E', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
         </div></div>
         </div>
    <div class="formpanel formpanel_padding">
         <div class="sectiontitle">
             <asp:Label ID="lblDriverDetails" runat="server" Text="Driver Details" meta:resourcekey="lnlDriverDetails"></asp:Label>
         </div>
         <div class="twocolumn">
             <asp:Label ID="lblFirstName" runat="server" CssClass="mandatory" meta:resourcekey="lblfirstnameResource1" AssociatedControlID="txtFirstName">First name*</asp:Label><span class="inputs"><asp:TextBox ID="txtFirstName" runat="server" meta:resourcekey="txtfirstnameResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">
                 <asp:RequiredFieldValidator ID="reqfirstname" ValidationGroup="vgDvlaLookupConsentValidation" runat="server" ErrorMessage="Please enter First name." ControlToValidate="txtFirstName" meta:resourcekey="reqfirstnameResource1" Display="Dynamic">*</asp:RequiredFieldValidator>
             </span>
            <asp:Label ID="lblMiddleName" runat="server" meta:resourcekey="lblmiddlenameResource1" AssociatedControlID="txtMiddleName">Middle name</asp:Label><span class="inputs"><asp:TextBox ID="txtMiddleName" runat="server" meta:resourcekey="txtmiddlenameResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
         </div>
        <div class="twocolumn">
             <asp:Label ID="lblSurName" runat="server" CssClass="mandatory" meta:resourcekey="lblsurnameResource1" AssociatedControlID="txtSurName">Surname*</asp:Label><span class="inputs"><asp:TextBox ID="txtSurName" runat="server" meta:resourcekey="txtsirnameResource1" CssClass="fillspan" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqsurname" ValidationGroup="vgDvlaLookupConsentValidation" runat="server" ErrorMessage="Please enter Surname." ControlToValidate="txtsurname" meta:resourcekey="reqsurnameResource1" Display="Dynamic">*</asp:RequiredFieldValidator></span>
            <asp:Label ID="lblDateOfBirth" runat="server" CssClass="mandatory" meta:resourcekey="lbldateofbirthResource1" AssociatedControlID="txtDateOfBirth">Date of birth*</asp:Label><span class="inputs"><asp:TextBox ID="txtDateOfBirth" runat="server" meta:resourcekey="txtdateofbirthResource1" CssClass="fillspan hasCalControl dateField"></asp:TextBox></span><span class="inputicon"><asp:Image ID="imgDateOfBirth" ImageUrl="~/shared/images/icons/cal.gif" runat="server" CssClass="dateCalImg"/></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">
                 <asp:RequiredFieldValidator ID="reqDateOfBirth" ValidationGroup="vgDvlaLookupConsentValidation" runat="server" ErrorMessage="Please enter date of birth." ControlToValidate="txtDateOfBirth" Display="Dynamic">*</asp:RequiredFieldValidator>
             </span>
             </div>
                  <div class="twocolumn">
             <asp:Label ID="lblSex" runat="server" CssClass="mandatory" meta:resourcekey="lblsexResource1" AssociatedControlID="ddlsex">Sex*</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="ddlsex" runat="server">
                  <asp:ListItem Text="[None]" Value="0"></asp:ListItem>
                  <asp:ListItem Text="Male" Value="1"></asp:ListItem>
                  <asp:ListItem Text="Female" Value="2"></asp:ListItem>
              </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="reqSex" runat="server" ControlToValidate="ddlsex" ErrorMessage="Please select your sex." Operator="GreaterThan" Type="Integer" ValidationGroup="vgDvlaLookupConsentValidation" ValueToCompare="0">*</asp:CompareValidator></span>
        <asp:Label ID="lblEmail" runat="server" CssClass="mandatory" meta:resourcekey="lblemailResource1" AssociatedControlID="txtEmail">Email address*</asp:Label><span class="inputs"><asp:TextBox ID="txtEmail" runat="server" MaxLength="200" meta:resourcekey="txtemailResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">
           <asp:RequiredFieldValidator ID="RequiredFieldValidator2"  ValidationGroup="vgDvlaLookupConsentValidation" runat="server" ErrorMessage="Please enter Email address." ControlToValidate="txtEmail" meta:resourcekey="reqtxtEmailResource1" Display="Dynamic">*</asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator runat="server" ID="regexemail" ControlToValidate="txtEmail" Text="*" ErrorMessage="Please enter a valid Email address." ValidationGroup="vgDvlaLookupConsentValidation" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator></span>
         </div>
        <div class="twocolumn">
           <asp:Label ID="lblDrivingLicenceNumber" CssClass="mandatory" runat="server" meta:resourcekey="lbldrivinglicencenumberResource1" AssociatedControlID="txtDrivingLicenceNumber">Driving licence number*</asp:Label><span class="inputs"><asp:TextBox ID="txtDrivingLicenceNumber" style="text-transform:uppercase" runat="server" meta:resourcekey="txtdrivinglicencenumberResource1" CssClass="fillspan" MaxLength="16"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgDvlaLookupConsentValidation" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter driving licence number." ControlToValidate="txtDrivingLicenceNumber" meta:resourcekey="reqsurnameResource1" Display="Dynamic">*</asp:RequiredFieldValidator>
<asp:RegularExpressionValidator runat="server" id="regexLicenceNumber" controltovalidate="txtDrivingLicenceNumber" ValidationGroup="vgDvlaLookupConsentValidation"  validationexpression="^(?=.{16}$)[A-Za-z]{1,5}9{0,4}[0-9](?:[05][1-9]|[16][0-2])(?:[0][1-9]|[12][0-9]|3[01])[0-9](?:99|[A-Za-z][A-Za-z9])(?![IOQYZioqyz01_])\w[A-Za-z]{2}" ErrorMessage="Please enter a valid licence number." Text="*" /></span>
        </div>

        <div class="formbuttons">
            <helpers:CSSButton ID="cmdSubmit" CausesValidation="False" runat="server" Text="I wish to provide consent" OnClientClick="SEL.DVLAConsent.OpenConfirmationDialog(); return false;"/>
            <helpers:CSSButton runat="server" ID="cmdDenyConsent" OnClick="cmdDenyConsent_OnClick" CausesValidation="False" Text="I do not provide consent"/>
            <helpers:CSSButton runat="server" ID="cmdCancel" OnClick="CmdCancelClick" CausesValidation="False" runat="server" Text="cancel"/>

        </div>

        <div id="ValidationWarningsModal"></div>   
        </div>
    
        <div id="DetailConfirmationModal">
        <div class="sm_panel">
            <div class="comment">Please note that the details you have entered will be used to look up your driving licence. It is very important that these details are identical to what is shown on your driving licence.</div>
            <div class="sectiontitle">Licence Details</div>
            <div class="twocolumn">
                <asp:Label ID="lblFirstNameConfirmation" runat="server" CssClass="mandatory" Text="First name*" AssociatedControlID="txtFirstNameConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtFirstNameConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label ID="lblMiddleNameConfirmation" runat="server" Text="Middle name" AssociatedControlID="txtMiddleNameConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtMiddleNameConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>
             <div class="twocolumn">
                <asp:Label ID="lblSurnameConfirmation" runat="server" CssClass="mandatory" Text="Surname*" AssociatedControlID="txtSurnameConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtSurnameConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label ID="lblDateOfBirthConfirmation" runat="server" Text="Date of birth*" CssClass="mandatory" AssociatedControlID="txtDateOfBirthConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtDateOfBirthConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon">
                </span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>

             <div class="twocolumn">
                <asp:Label ID="lblSexConfirmation" runat="server" CssClass="mandatory" Text="Sex*" AssociatedControlID="txtSexConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtSexConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label ID="Label6" runat="server" Text="Email address*" CssClass="mandatory" AssociatedControlID="txtEmailConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtEmailConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon">
                </span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>

             <div class="twocolumn">
                <asp:Label ID="lblDrivingLicenceNumberConfirmation" runat="server" CssClass="mandatory" Text="Driving licence number*" AssociatedControlID="txtDrivingLicenceNumberConfirmation"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtDrivingLicenceNumberConfirmation" runat="server" MaxLength="50" ReadOnly="True"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>


        </div>
    </div>

    <div id="ConsentPortalInformationModal">
        <div class="sm_panel">
            <div id="consentInfoPlaceholder" class="centered">
            </div>
        </div>
    
    </div>

</asp:Content>