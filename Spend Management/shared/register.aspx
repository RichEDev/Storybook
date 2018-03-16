<%@ Page Language="C#" MasterPageFile="~/masters/AnonymousUser.Master" AutoEventWireup="true" Inherits="register" CodeBehind="register.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/AnonymousUser.Master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="pageContents">
    
    <div id="maincontentdiv">
        <div class="inputpanel">
            <div id="timeline" style="width: 950px;">
                <div id="timelineleft">
                    <img src="images/buttons/timeline_left.gif" /></div>
                <asp:Literal ID="littimeline" runat="server" meta:resourcekey="littimelineResource1"></asp:Literal>
                <div id="timelineright">
                    <img src="images/buttons/timeline_right.gif" /></div>
            </div>
        </div>
    
        <script type="text/javascript">
            function changeStep (index)
            {
                document.getElementById("requiredStep").value = index;
                theForm.submit();
                document.getElementById("requiredStep").value = '';
            }
    
            function txtretypeemail_onblur()
            {
                var email = new String();
                var username = new String();
                username = "";
                email = document.getElementById(contentID + "wizregister_txtretypeemail").value
                if (email.indexOf('@') != -1)
                {
                    username = email.substring(0, email.indexOf('@',0));
                }
                document.getElementById(contentID + "wizregister_txtusername").value = username;
            }
    
            function setAddressFieldsReadOnly(val) {
                //we don't want to set them as readonly in asp.net as it won't accept the values we set on postback
                var controls = $(
                    "#" + SEL.Addresses.Dom.Address.AddressName + ", " +
                    "#" + SEL.Addresses.Dom.Address.Line1 + ", " +
                    "#" + SEL.Addresses.Dom.Address.Line2 + ", " +
                    "#" + SEL.Addresses.Dom.Address.City + ", " +
                    "#" + SEL.Addresses.Dom.Address.County + ", " +
                    "#" + SEL.Addresses.Dom.Address.CountryName + ", " +
                    "#" + SEL.Addresses.Dom.Address.Postcode
                );
                if (val === true) {
                    controls.attr("readonly", "readonly");
                } else if (val === false) {
                    controls.removeAttr("readonly");
                }
            }
    
            function setAddressFieldsReadWrite() {
                setAddressFieldsReadOnly(false);
            }
        
            $(function () {
                $(".datepicker").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: "dd/mm/yy",
                    maxDate: "0",
                    firstDay: "1",
                    showOn: "both",
                    buttonImage: "images/icons/cal.gif",
                    buttonImageOnly: true,
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    buttonText: ""
                });
        
                window.Sys.Application.add_load(function () {
                    if (Number(SEL.Addresses.anonymousLookupUsages) >= SEL.Addresses.Identifiers.MaximumAnonymousLookupUsages) {
                        //they've used up their quota
                        $("#addresssearchrow").hide();
                    } else {
                        $(".ui-sel-address-picker").address({
                                enableFavourites: false,
                                enableAccountWideFavourites: false,
                                enableLabels: false,
                                enableAccountWideLabels: false,
                                enableManualAddresses: true,
                                accountId : <% = ViewState["accountid"] ?? "null" %>
                            })
                            .on("addressnotfoundclick", function () {
                                $(this).address("close").address("reset").val("");
                                setAddressFieldsReadWrite();
                                $("#" + SEL.Addresses.Dom.Address.Line1).focus();
                                return false;
                            })
                            .on("addressmatched", function (event, addressData) {
                                $(["AddressName", "Line1", "Line2", "City", "County", "CountryName", "Postcode", "GlobalIdentifier", "Udprn", "Latitude", "Longitude"]).each(function () {
                                    var field = this;
                                    $("#" + SEL.Addresses.Dom.Address[field]).val(addressData[field] || addressData[field.toUpperCase()]);
                                });
                                $(this).address("close").address("reset").val("");
                                setAddressFieldsReadOnly(true);
                                if (!addressData.Postcode) $("#" + SEL.Addresses.Dom.Address.Postcode).removeAttr("readonly");
                                SEL.Addresses.anonymousLookupUsages = addressData.AnonymousLookupUsages;
                            });

                        setAddressFieldsReadOnly(true);
                    }
                });

                SEL.Addresses = SEL.Addresses || {};
                SEL.Addresses.Dom = SEL.Addresses.Dom || {};
                SEL.Addresses.Dom.Address = SEL.Addresses.Dom.Address || {};
                SEL.Addresses.Identifiers = SEL.Addresses.Identifiers || {};
                SEL.Addresses.Identifiers.MaximumAnonymousLookupUsages = <% = string.IsNullOrEmpty(ConfigurationManager.AppSettings["maximumAnonymousLookupUsages"]) ? "5" : ConfigurationManager.AppSettings["maximumAnonymousLookupUsages"] %> ;
                (function (a) {
                    a.Search = "<% = this.txtaddresssearch.ClientID %>";
                    a.AddressName = "<% = this.txtAddressName.ClientID %>";
                    a.Line1 = "<% = this.txtaddressline1.ClientID %>";
                    a.Line2 = "<% = this.txtaddressline2.ClientID %>";
                    a.City = "<% = this.txtcity.ClientID %>";
                    a.County = "<% = this.txtcounty.ClientID %>";
                    a.CountryName = "<% = this.txtcountry.ClientID %>";
                    a.Postcode = "<% = this.txtpostcode.ClientID %>";
                    a.GlobalIdentifier = "<% = this.hdnGlobalIdentifier.ClientID %>";
                    a.Udprn = "<% = this.hdnUdprn.ClientID %>";
                    a.Latitude = "<% = this.hdnLatitude.ClientID %>";
                    a.Longitude = "<% = this.hdnLongitude.ClientID %>";
                    a.anonymousLookupUsages = "<% = Session["anonymousLookupUsages"] %>";
                    a.AccountID = "<% = ViewState["accountid"] %>";

                    $("#" + a.Search).keyup(function () {
                        if (SEL.Addresses.anonymousLookupUsages > SEL.Addresses.Identifiers.MaximumAnonymousLookupUsages) {

                            alert('You have used the maximum number of lookups. Please manually enter the address.');

                            $("#addresssearchrow").hide();
                            setAddressFieldsReadWrite();
                        }

                    });

                }(SEL.Addresses.Dom.Address));

            });


            function OnVehicleTypeChange(sender) {
                var ddl = document.getElementById(sender);
                var vehicletypeid = parseInt(ddl.options[ddl.selectedIndex].value);

                if (vehicletypeid == 1)
                {
                    document.getElementById("<%=reqreg.ClientID%>").style.visibility = "hidden";
                    document.getElementById("<%=reqenginetype.ClientID%>").style.visibility = "hidden";
                    document.getElementById("<%=cvEngineSize.ClientID%>").style.visibility = "hidden";

                    document.getElementById("<%=reqreg.ClientID%>").enabled = false;
                    document.getElementById("<%=reqenginetype.ClientID%>").enabled = false;
                    document.getElementById("<%=cvEngineSize.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtaccountreference.ClientID%>").enabled = false;

                    document.getElementById("<%=txtregno.ClientID%>").disabled = true;
                    document.getElementById("<%=txtEngineSize.ClientID%>").disabled = true;
                    document.getElementById("<%=cmbcartype.ClientID%>").disabled = true;
                }
                else if(vehicletypeid>1) {
                    document.getElementById("<%=reqreg.ClientID%>").enabled = true;
                    document.getElementById("<%=reqenginetype.ClientID%>").enabled = true;
                    document.getElementById("<%=cvEngineSize.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtaccountreference.ClientID%>").enabled = true;

                    document.getElementById("<%=txtregno.ClientID%>").disabled = false;
                    document.getElementById("<%=txtEngineSize.ClientID%>").disabled = false;
                    document.getElementById("<%=cmbcartype.ClientID%>").disabled = false;
                }
            }


            function OnBankDetailsTextBoxChanged() {
                var accountName = document.getElementById("<%=txtaccountholdername.ClientID%>");
                var accountNumber = document.getElementById("<%=txtaccountholdernumber.ClientID%>");
                var accounttype = document.getElementById("<%=cmbAccounttype.ClientID%>").selectedIndex;
                var currency = document.getElementById("<%=ddlCurrency.ClientID%>").selectedIndex;
                var sortcode = document.getElementById("<%=txtsortcode.ClientID%>");
                var accountreference = document.getElementById("<%=txtaccountreference.ClientID%>");
                var country = document.getElementById("<%=ddlBankCountry.ClientID%>").selectedIndex;

                var selectedCountry = document.getElementById("ddlBankCountry");
                var countryValue = selectedCountry.options[selectedCountry.selectedIndex].text;
                if(countryValue!=null && countryValue == 'United Kingdom')
                {
                    document.getElementById("<%=retxtaccountholernumberlength.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtSortCodeNumberCheck.ClientID%>").enabled = true; 
                    document.getElementById("<%=retxtSortCodeEmptyCheck.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtSortCodeCharactersCheck.ClientID%>").enabled = true;
                }
                else{
                    document.getElementById("<%=retxtaccountholernumberlength.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtSortCodeNumberCheck.ClientID%>").enabled = false; 
                    document.getElementById("<%=retxtSortCodeEmptyCheck.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtSortCodeCharactersCheck.ClientID%>").enabled = false;
                }

                if (accountName.value != '' || accountNumber.value != '' || accounttype > 0 || currency > 0 || sortcode.value != '' || accountreference.value != ''||country>0) {
                    document.getElementById("<%=rftxtaccountholdername.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtAccountName.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtaccountholdernumber.ClientID%>").enabled = true;
                    document.getElementById("<%=rfAccountNumber.ClientID%>").enabled = true;
                    document.getElementById("<%=rvAccountType.ClientID%>").enabled = true;
                    document.getElementById("<%=retxtSortCode.ClientID%>").enabled = true;
                    document.getElementById("<%=rvCurrency.ClientID%>").enabled = true;
                    document.getElementById("<%=rvBankCountry.ClientID%>").enabled = true;
                } 
                else if (accountName.value == '' && accountNumber.value == '' && accounttype == 0 && currency == 0  && sortcode.value == '' && accountreference.value == ''&&country==0) {
                    document.getElementById("<%=rftxtaccountholdername.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtAccountName.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtaccountholdernumber.ClientID%>").enabled = false;
                    document.getElementById("<%=rfAccountNumber.ClientID%>").enabled = false;
                    document.getElementById("<%=rvAccountType.ClientID%>").enabled = false;
                    document.getElementById("<%=retxtSortCode.ClientID%>").enabled = false;
                    document.getElementById("<%=rvCurrency.ClientID%>").enabled = false;
                    document.getElementById("<%=rvBankCountry.ClientID%>").enabled = false;
                }


            }

        </script>
        

        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
            <Scripts>
                <asp:ScriptReference Name="tooltips" />

                <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js" />
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.data.js" />
                <asp:ScriptReference Path="~/shared/javaScript/minify/jquery-selui-address.js" />

            </Scripts>
        </asp:ScriptManagerProxy>
    <tooltip:tooltip id="usrTooltip" runat="server"></tooltip:tooltip>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" />
        &nbsp;<asp:Wizard ID="wizregister" runat="server" ActiveStepIndex="0"
            OnFinishButtonClick="WizregisterFinishButtonClick" DisplaySideBar="False"
            OnNextButtonClick="WizregisterNextButtonClick"
            OnActiveStepChanged="WizregisterActiveStepChanged"
            CancelButtonImageUrl="~/shared/images/buttons/cancel_up.gif" CancelButtonType="Image"
            DisplayCancelButton="True" OnCancelButtonClick="WizregisterCancelButtonClick"
            FinishCompleteButtonImageUrl="~/shared/images/buttons/pagebutton_finish.gif"
            FinishCompleteButtonType="Image"
            FinishPreviousButtonImageUrl="~/shared/images/buttons/pagebutton_previous.gif"
            FinishPreviousButtonType="Image"
            StartNextButtonImageUrl="~/shared/images/buttons/pagebutton_next.gif"
            StartNextButtonType="Image"
            StepNextButtonImageUrl="~/shared/images/buttons/pagebutton_next111.gif"
            StepNextButtonType="Image"
            StepPreviousButtonImageUrl="~/shared/images/buttons/pagebutton_previous.gif"
            StepPreviousButtonType="Image" Width="800"
            meta:resourcekey="wizregisterResource1">
    
            <WizardSteps>
                <asp:WizardStep runat="server" Title="Step 1 - Employee Name" StepType="Start" meta:resourcekey="WizardStepResource1">
                    
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            <asp:Label ID="lblnamelbl" runat="server" Text="Employee Name &amp;amp; Logon Details" meta:resourcekey="lblnamelblResource1"></asp:Label></div>
                            <table>
                            <tr><td class="labeltd"><asp:Label ID="lbltitle" runat="server" Text="Title:" meta:resourcekey="lbltitleResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txttitle" runat="server" meta:resourcekey="txttitleResource1"></asp:TextBox></td><td><asp:RequiredFieldValidator ID="reqtitle" runat="server" ErrorMessage="Please enter your title in the box provided" ControlToValidate="txttitle" SetFocusOnError="True" meta:resourcekey="reqtitleResource1">*</asp:RequiredFieldValidator></td></tr>
                            <tr><td class="labeltd" style="height: 27px"><asp:Label ID="lblfirstname" runat="server" Text="First Name:" meta:resourcekey="lblfirstnameResource1"></asp:Label></td><td class="inputtd" style="height: 27px"><asp:TextBox ID="txtfirstname" runat="server" meta:resourcekey="txtfirstnameResource1"></asp:TextBox></td><td style="height: 27px"><asp:RequiredFieldValidator ID="reqfirstname" runat="server" ErrorMessage="Please enter your firstname in the box provided" ControlToValidate="txtfirstname" SetFocusOnError="True" meta:resourcekey="reqfirstnameResource1">*</asp:RequiredFieldValidator></td></tr>
                            <tr><td class="labeltd"><asp:Label ID="lblsurname" runat="server" Text="Surname:" meta:resourcekey="lblsurnameResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtsurname" runat="server" AutoCompleteType="LastName" meta:resourcekey="txtsurnameResource1"></asp:TextBox></td><td><asp:RequiredFieldValidator ID="reqsurname" runat="server" ErrorMessage="Please enter your surname in the box provided" ControlToValidate="txtsurname" SetFocusOnError="True" meta:resourcekey="reqsurnameResource1">*</asp:RequiredFieldValidator></td></tr>
                            <tr><td class="labeltd"><asp:Label ID="lblemaillbl" runat="server" Text="E-mail Address:" meta:resourcekey="lblemaillblResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtemail" runat="server" meta:resourcekey="txtemailResource1"></asp:TextBox></td><td><asp:RegularExpressionValidator ID="regemail" runat="server" ErrorMessage="The e-mail address you have entered is not valid" ControlToValidate="txtemail" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" meta:resourcekey="regemailResource1">*</asp:RegularExpressionValidator><asp:RequiredFieldValidator ID="reqemail" runat="server" ErrorMessage="Please enter your e-mail address in the box provided" ControlToValidate="txtemail" SetFocusOnError="True" meta:resourcekey="reqemailResource1">*</asp:RequiredFieldValidator></td></tr>
                            <tr><td class="labeltd"><asp:Label ID="lblretypeemail" runat="server" Text="Re-type E-mail Address:" meta:resourcekey="lblretypeemailResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtretypeemail" runat="server" meta:resourcekey="txtretypeemailResource1"></asp:TextBox></td><td><asp:CompareValidator ID="compemail" runat="server" ErrorMessage="The e-mail address and re-typed e-mail address must match" ControlToCompare="txtemail" ControlToValidate="txtretypeemail" SetFocusOnError="True" meta:resourcekey="compemailResource1">*</asp:CompareValidator><asp:RequiredFieldValidator ID="reqretypeemail" runat="server" ErrorMessage="Please re-type your e-mail address in the box provided" ControlToValidate="txtretypeemail" SetFocusOnError="True" meta:resourcekey="reqretypeemailResource1">*</asp:RequiredFieldValidator></td></tr>    
                                   <tr><td class="labeltd"><asp:Label ID="lblusernamelbl" runat="server" Text="Username:" meta:resourcekey="lblusernamelblResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtusername" runat="server" meta:resourcekey="txtusernameResource1"></asp:TextBox></td><td><asp:RequiredFieldValidator ID="requsername" runat="server" ErrorMessage="Please enter a username you would like to logon to expenses with" ControlToValidate="txtusername" SetFocusOnError="True" meta:resourcekey="requsernameResource1">*</asp:RequiredFieldValidator><img id="imgtooltip390" onclick="SEL.Tooltip.Show('29f00dee-1934-479f-a302-c8282c32e4de', 'sm', this);" src="images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td></tr>
                            </table>
                        <asp:Label ID="lblmsggeneral" runat="server" Text="Label" Visible="False" ForeColor="Red" meta:resourcekey="lblmsggeneralResource1"></asp:Label>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Password" meta:resourcekey="WizardStepResource2">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                    <asp:Label ID="lblenterpw" runat="server" Text="Enter a password" meta:resourcekey="lblenterpwResource1"></asp:Label></div>
                    <table>
			        <tr><td class="labeltd"><asp:Label id="lblnew" runat="server" meta:resourcekey="lblnewResource1">Password</asp:Label></td><td class="inputtd"><asp:TextBox id="txtnew" runat="server" TextMode="Password" meta:resourcekey="txtnewResource1"></asp:TextBox></td><td><asp:RequiredFieldValidator id="reqpassword" runat="server" ErrorMessage="Please enter the new password" ControlToValidate="txtnew" meta:resourcekey="reqpasswordResource1">*</asp:RequiredFieldValidator></td></tr>
			        <tr><td class="labeltd"><asp:Label id="lblrenew" runat="server" meta:resourcekey="lblrenewResource1">Re-type Password</asp:Label></td><td class="inputtd"><asp:TextBox id="txtrenew" runat="server" TextMode="Password" meta:resourcekey="txtrenewResource1"></asp:TextBox></td><td><asp:CompareValidator id="compnew" runat="server" ControlToValidate="txtrenew" ErrorMessage="The new password and re-typed new password values must be identical" ControlToCompare="txtnew" meta:resourcekey="compnewResource1">*</asp:CompareValidator><asp:RequiredFieldValidator id="reqrenew" runat="server" ErrorMessage="Please re-type the new password" ControlToValidate="txtrenew" meta:resourcekey="reqrenewResource1">*</asp:RequiredFieldValidator></td></tr>
                        </table>
                        <asp:Label ID="lblpassword" runat="server" Text="Label" ForeColor="Red" Visible="False" meta:resourcekey="lblpasswordResource1"></asp:Label>
                    </div>
                    <asp:Literal ID="litpolicy" runat="server" meta:resourcekey="litpolicyResource1"></asp:Literal>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Employee Contact Details" meta:resourcekey="WizardStepResource3">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                    <asp:Label ID="lblcontactdetails" runat="server" Text="Employee Contact Details" meta:resourcekey="lblcontactdetailsResource1"></asp:Label></div>
                        <table>
                        <tr><td class="labeltd"><asp:Label ID="lblextension" runat="server" Text="Extension Number:" meta:resourcekey="lblextensionResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtextension" runat="server" meta:resourcekey="txtextensionResource1"></asp:TextBox></td></tr>
                        <tr><td class="labeltd"><asp:Label ID="lblmobilenumber" runat="server" Text="Mobile Number:" meta:resourcekey="lblmobilenumberResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtmobile" runat="server" meta:resourcekey="txtmobileResource1"></asp:TextBox></td></tr>
                        <tr><td class="labeltd"><asp:Label ID="lblpager" runat="server" Text="Pager Number:" meta:resourcekey="lblpagerResource1"></asp:Label></td><td class="inputtd"><asp:TextBox ID="txtpager" runat="server" meta:resourcekey="txtpagerResource1"></asp:TextBox></td></tr>
                        </table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Step 2 - Employee Address" meta:resourcekey="WizardStepResource4">
                    <div class="inputpanel">
                <div class="inputpaneltitle"><asp:Label ID="lbladdress" runat="server" Text="Employee Contact Details" meta:resourcekey="lbladdressResource1"></asp:Label></div>
                        <table>
                            <tr id="addresssearchrow">
                                <td class="labeltd">
                                    <asp:Label ID="lbladdresssearch" AssociatedControlID="txtaddresssearch" runat="server">Address</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtaddresssearch" runat="server" MaxLength="250" CssClass="ui-sel-address-picker"></asp:TextBox>
                                    <asp:HiddenField runat="server" ID="addressidhidden"/>
                                    <asp:HiddenField runat="server" ID="hdnGlobalIdentifier"/>
                                    <asp:HiddenField runat="server" ID="hdnUdprn"/>
                                    <asp:HiddenField runat="server" ID="hdnLatitude"/>
                                    <asp:HiddenField runat="server" ID="hdnLongitude"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd"><asp:Label id="lblAddressName" AssociatedControlID="txtAddressName" runat="server">Address Name</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtAddressName" runat="server" MaxLength="250"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lbladdressline1" AssociatedControlID="txtaddressline1" runat="server">Address Line 1</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtaddressline1" runat="server" MaxLength="250"></asp:textbox></td>
                        <td><asp:RequiredFieldValidator ID="rfvaddressline1" runat="server" ErrorMessage="Please enter your address in the box provided" ControlToValidate="txtaddressline1" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lbladdressline2" AssociatedControlID="txtaddressline2" runat="server">Address Line 2</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtaddressline2" runat="server" MaxLength="250"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lblcity" AssociatedControlID="txtcity" runat="server">City/Town</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtcity" runat="server" MaxLength="250"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lblcounty" AssociatedControlID="txtcounty" runat="server">County/State</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtcounty" runat="server" MaxLength="250"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lblcountry" AssociatedControlID="txtcountry" runat="server">Country</asp:Label></td>
                        <td class="inputtd">
                            <asp:textbox id="txtcountry" runat="server" MaxLength="250"></asp:textbox>
                            <cc1:AutoCompleteExtender ID="autocompcountry" runat="server" TargetControlID="txtcountry"
                                ServicePath="~/shared/register.aspx" ServiceMethod="getCountryList"
                                MinimumPrefixLength="1" CompletionInterval="100" DelimiterCharacters="" Enabled="True">
                            </cc1:AutoCompleteExtender>
                        </td>
                        <td><asp:RequiredFieldValidator ID="rfvtxtcountry" runat="server" ErrorMessage="Please enter your country in the box provided" ControlToValidate="txtcountry" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lblpostcode" AssociatedControlID="txtpostcode" runat="server">Postcode/Zip</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtpostcode" runat="server" MaxLength="250"></asp:textbox></td>
                        <td><asp:RequiredFieldValidator ID="rfvpostcode" runat="server" ErrorMessage="Please enter your postcode/zip code in the box provided" ControlToValidate="txtpostcode" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="lbldateataddress" AssociatedControlID="txtdateataddress" runat="server">When did you move to this Address?</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txtdateataddress" runat="server" MaxLength="250" CssClass="datepicker"></asp:textbox></td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvdateataddress" runat="server" ErrorMessage="When did you move to your current Address?" ControlToValidate="txtdateataddress" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revdateataddress" runat="server" ErrorMessage="Please enter your date in the following format day/month/year" ControlToValidate="txtdateataddress" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" SetFocusOnError="True"></asp:RegularExpressionValidator>
                        </td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Home Telephone</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txthomephone" runat="server" MaxLength="50" meta:resourcekey="txthomephoneResource1"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Home Fax</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txthomefax" runat="server" MaxLength="50" meta:resourcekey="txthomefaxResource1"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			        <tr>
			            <td class="labeltd"><asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Home E-mail Address</asp:Label></td>
                        <td class="inputtd"><asp:textbox id="txthomeemail" runat="server" MaxLength="50" meta:resourcekey="txthomeemailResource1"></asp:textbox></td>
                        <td class="inputtd">&nbsp;</td>
			        </tr>
			    </table>
			</div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Employment Details" meta:resourcekey="WizardStepResource5">
                <div class="inputpanel">
                <div class="inputpaneltitle">
                    <asp:Label ID="lblemploymentdetails" runat="server" Text="Employment Details" meta:resourcekey="lblemploymentdetailsResource1"></asp:Label></div>
                <table>
                <tr><td class="labeltd">
                    <asp:Label ID="lblcreditaccnt" runat="server" Text="Credit Account / Purchase Ledger Number" meta:resourcekey="lblcreditaccntResource1"></asp:Label></td><td class="inputtd">
                        <asp:TextBox ID="txtpurchaseledger" runat="server" meta:resourcekey="txtpurchaseledgerResource1"></asp:TextBox></td></tr>
                    <tr><td class="labeltd">
                        <asp:Label ID="lblposition" runat="server" Text="Position" meta:resourcekey="lblpositionResource1"></asp:Label></td><td class="inputtd">
                        <asp:TextBox ID="txtposition" runat="server" meta:resourcekey="txtpositionResource1"></asp:TextBox></td></tr>
                    <tr><td class="labeltd">
                        <asp:Label ID="lblpayroll" runat="server" Text="Payroll Number" meta:resourcekey="lblpayrollResource1"></asp:Label></td><td class="inputtd">
                        <asp:TextBox ID="txtpayrollnumber" runat="server" meta:resourcekey="txtpayrollnumberResource1"></asp:TextBox></td></tr>
                      
                            <tr><td class="labeltd">
                                <asp:Label ID="lbllinemanger" runat="server" Text="Line Manager" meta:resourcekey="lbllinemangerResource1"></asp:Label></td><td class="inputtd">
                                <asp:DropDownList ID="cmblinemanager" runat="server" meta:resourcekey="cmblinemanagerResource1">
                                </asp:DropDownList></td></tr>
                                <tr><td class="labeltd">
                                    <asp:Label ID="lblprimarycountry" runat="server" Text="Primary Country" meta:resourcekey="lblprimarycountryResource1"></asp:Label></td><td class="inputtd">
                                <asp:DropDownList ID="cmbcountry" runat="server" meta:resourcekey="cmbcountryResource1">
                                </asp:DropDownList></td></tr>
                                <tr><td class="labeltd">
                                    <asp:Label ID="lblprimarycurrency" runat="server" Text="Primary Currency" meta:resourcekey="lblprimarycurrencyResource1"></asp:Label></td><td class="inputtd">
                                <asp:DropDownList ID="cmbcurrency" runat="server" meta:resourcekey="cmbcurrencyResource1">
                                </asp:DropDownList></td></tr>
                </table>
                </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Role" meta:resourcekey="WizardStepResource6">
                    <asp:Literal ID="litroles" runat="server" meta:resourcekey="litrolesResource1"></asp:Literal>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Signoff Group" meta:resourcekey="WizardStepResource7">
                    <asp:Literal ID="litsignoffs" runat="server" meta:resourcekey="litsignoffsResource1"></asp:Literal>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Advances Signoff Group" meta:resourcekey="WizardStepResource8">
                    <asp:Literal ID="litadvancessignoffs" runat="server" meta:resourcekey="litadvancessignoffsResource1"></asp:Literal>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Department / Costcode Breakdown" meta:resourcekey="WizardStepResource9">
                <div class="inputpanel">
                <div class="inputpaneltitle">
                    <asp:Label ID="lblcoding" runat="server" Text="Coding Breakdown" meta:resourcekey="lblcodingResource1"></asp:Label></div>
                <table>
                    <tr><td class="labeltd">
                        <asp:Label ID="lbldepartment" runat="server" Text="Department" meta:resourcekey="lbldepartmentResource1"></asp:Label></td><td class="inputtd">
                        <asp:DropDownList ID="cmbdepartment" runat="server" meta:resourcekey="cmbdepartmentResource1">
                        </asp:DropDownList></td></tr>
                        <tr><td class="labeltd">
                            <asp:Label ID="lblcostcode" runat="server" Text="Cost Code" meta:resourcekey="lblcostcodeResource1"></asp:Label></td><td class="inputtd">
                        <asp:DropDownList ID="cmbcostcode" runat="server" meta:resourcekey="cmbcostcodeResource1">
                        </asp:DropDownList></td></tr>
                        <tr><td class="labeltd">
                            <asp:Label ID="lblprojectcode" runat="server" Text="Project Code" meta:resourcekey="lblprojectcodeResource1"></asp:Label></td><td class="inputtd">
                        <asp:DropDownList ID="cmbprojectcode" runat="server" meta:resourcekey="cmbprojectcodeResource1">
                        </asp:DropDownList></td></tr>
                </table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Bank Details" meta:resourcekey="WizardStepResource10">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            <asp:Label ID="lblbankdetails" runat="server" Text="Bank Details" meta:resourcekey="lblbankdetailsResource1"></asp:Label>
                        </div>
                        <table>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblaccountholdername" runat="server" Text="Account Name:" meta:resourcekey="lblaccountholdernameResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtaccountholdername" runat="server" meta:resourcekey="txtaccountholdernameResource1" onchange="OnBankDetailsTextBoxChanged()" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rftxtaccountholdername" runat="server" ErrorMessage="Please enter an Account Name." ControlToValidate="txtaccountholdername" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="retxtAccountName" ErrorMessage="Account Name should not contain special characters." ValidationExpression="^[a-zA-Z0-9\-.\s]+$" ControlToValidate="txtaccountholdername">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblaccountholdernumber" runat="server" Text="Account Number:" meta:resourcekey="lblaccountholdernumberResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtaccountholdernumber" runat="server" meta:resourcekey="txtaccountholdernumberResource1"  onchange="OnBankDetailsTextBoxChanged()" MaxLength="20"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfAccountNumber" runat="server" ErrorMessage="Please enter an Account Number." ControlToValidate="txtaccountholdernumber" InitialValue=""  Enabled="False">*</asp:RequiredFieldValidator>
                                     <asp:RegularExpressionValidator runat="server" ID="retxtaccountholdernumber" ErrorMessage="The value you have entered for Account Number is invalid. Valid characters are the numbers 0-9."  ValidationExpression="^\d+" ControlToValidate="txtaccountholdernumber" Display="None"></asp:RegularExpressionValidator>
                                     <asp:RegularExpressionValidator runat="server" ID="retxtaccountholernumberlength" Enabled="false" ErrorMessage="The Account Number must consist of 7 or 8 characters." ValidationExpression="^.{7,8}$" ControlToValidate="txtaccountholdernumber" Display="None"></asp:RegularExpressionValidator>
                                  </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblaccounttype" runat="server" Text="Account Type:" meta:resourcekey="lblaccounttypeResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:DropDownList ID="cmbAccounttype" runat="server" CssClass="fillspan" ValidationGroup="vgAddEditAccount" meta:resourcekey="cmbAccounttypeResource"  onchange="OnBankDetailsTextBoxChanged()"></asp:DropDownList>
                                       <asp:CompareValidator ID="rvAccountType" runat="server" ControlToValidate="cmbAccounttype" ErrorMessage="Please select an Account Type." Operator="GreaterThan" Type="Integer" ValueToCompare="0"  Enabled="False">*</asp:CompareValidator></td>
                                 
                                   
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblsortcode" runat="server" Text="Sort Code:" meta:resourcekey="lblsortcodeResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtsortcode" runat="server" meta:resourcekey="txtsortcodeResource1" onchange="OnBankDetailsTextBoxChanged()" MaxLength="100"></asp:TextBox>
                                     <asp:RegularExpressionValidator runat="server" ID="retxtSortCode" ErrorMessage="Sort Code should not contain special characters." ValidationExpression="^[a-zA-Z0-9\-.\s]+$" ControlToValidate="txtSortCode" Enabled="False" Display="None">*</asp:RegularExpressionValidator>
                                   <asp:RequiredFieldValidator ID="retxtSortCodeEmptyCheck" runat="server" ErrorMessage="Please enter an Sort Code." ControlToValidate="txtsortcode" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="retxtSortCodeNumberCheck" ErrorMessage="The value you have entered for Sort Code is invalid.Valid characters are the numbers 0-9." ValidationExpression="^[0-9]+$" ControlToValidate="txtSortCode" Enabled="False" Display="None"></asp:RegularExpressionValidator>
                                     <asp:RegularExpressionValidator runat="server" ID="retxtSortCodeCharactersCheck" ErrorMessage="The Sort Code must consist of 6 characters." ValidationExpression="^[a-zA-Z0-9]{6}$" ControlToValidate="txtSortCode" Enabled="False" Display="None"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblaccountref" runat="server" Text="Reference:" meta:resourcekey="lblaccountrefResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtaccountreference" runat="server" meta:resourcekey="txtaccountreferenceResource1" onchange="OnBankDetailsTextBoxChanged()" MaxLength="20"></asp:TextBox><asp:RegularExpressionValidator runat="server" ID="retxtaccountreference" ErrorMessage="The value you have entered for Reference is invalid. Valid characters are alphanumerics and special characters '-.,/:\_'" ValidationExpression="^[-.,/\\:\w\s]*$" ControlToValidate="txtaccountreference" Display="None"></asp:RegularExpressionValidator></td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblcurrency" runat="server" Text="Account Currency:" meta:resourcekey="lblcurrencyResource"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:DropDownList ID="ddlCurrency" runat="server" meta:resourcekey="ddlcurrencyResource"  onchange="OnBankDetailsTextBoxChanged()"></asp:DropDownList>
                                    <asp:CompareValidator ID="rvCurrency" runat="server" ControlToValidate="ddlCurrency" ErrorMessage="Please select an Account Currency." Operator="GreaterThan" Type="Integer" ValueToCompare="0"  Enabled="False">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblBankCountry" runat="server" Text="Country:" meta:resourcekey="lblbankcountryResource"></asp:Label></td>
                                <td class="inputtd">
                                        <asp:DropDownList ClientIDMode="Static" ID="ddlBankCountry" runat="server" meta:resourcekey="ddlbankcountryResource" onchange="OnBankDetailsTextBoxChanged()">
                                        </asp:DropDownList>
                                       <asp:CompareValidator ID="rvBankCountry" runat="server" ControlToValidate="ddlBankCountry" ErrorMessage="Please select an Account Country." Operator="GreaterThan" Type="Integer" ValueToCompare="0"  Enabled="False">*</asp:CompareValidator>
                                </td>

                            </tr>
                            
                            <tr>
                                <td class="labeltd">
                                    <asp:Label AssociatedControlID="txtIban" ID="lblIban" runat="server">IBAN</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtIban" runat="server" MaxLength="34" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ErrorMessage="The IBAN specified must be between 5 and 34 characters." ValidationExpression="^.{5,34}$" ControlToValidate="txtIban" Display="None"></asp:RegularExpressionValidator><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" ErrorMessage="The value you have entered for IBAN is invalid. Only letters and numbers are allowed." ValidationExpression="[a-zA-Z0-9]*$" ControlToValidate="txtIban">*</asp:RegularExpressionValidator>
                                </td>

                            </tr>
                            
                            <tr>
                                <td class="labeltd">
                                    <asp:Label AssociatedControlID="txtSwiftCode" ID="lblSwiftCode" runat="server">SWIFT Code</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtSwiftCode" runat="server" MaxLength="11" onkeydown="SEL.Forms.RunOnEnter(event, SEL.BankAccounts.SaveBankAccountOnEnter);"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ErrorMessage="The SWIFT Code specified must be between 8 and 11 characters." ValidationExpression="^.{8,11}$" ControlToValidate="txtSwiftCode" Display="None"></asp:RegularExpressionValidator><asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ErrorMessage="The value you have entered for SWIFT Code is invalid. Only letters and numbers are allowed." ValidationExpression="[a-zA-Z0-9]*$" ControlToValidate="txtSwiftCode">*</asp:RegularExpressionValidator>
                                </td>

                            </tr>
                        </table>
                        <asp:Label ID="lblmsgBankDetails" runat="server" Text="Label" Visible="False" ForeColor="Red" ></asp:Label>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Step 3 - Vehicle Details" meta:resourcekey="WizardStepResource11">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            <asp:Label ID="lblcardetails" runat="server" Text="Vehicle Details" meta:resourcekey="lblcardetailsResource1"></asp:Label>
                        </div>
                        <table>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblcarwork" runat="server" Text="Do you use a vehicle in the course of your work?" meta:resourcekey="lblcarworkResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:CheckBox ID="chkusecar" runat="server" AutoPostBack="True" OnCheckedChanged="ChkusecarCheckedChanged" meta:resourcekey="chkusecarResource1" />
                                </td>
                            </tr>
                            
                             <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblVehicleType" runat="server" meta:resourcekey="lblvehicletypeResource1">Vehicle Type</asp:Label></td>
                                <td class="inputtd">
                                    
                                   <%-- <asp:DropDownList ID="cmbvehicletype" runat="server" AutoPostBack="false" onchange="OnVehicleTypeChange(this.id)" meta:resourcekey="cmbvehicletypeResource1">
                                    </asp:DropDownList>--%>

                                    <asp:DropDownList ID="cmbvehicletype" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CmbvehicletypeSelectedIndexChanged"   meta:resourcekey="cmbvehicletypeResource1">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                   <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please select the vehicle type" ControlToValidate="cmbvehicletype" SetFocusOnError="True" meta:resourcekey="reqenginetypeResource1">*</asp:RequiredFieldValidator>--%>
                                    <asp:CompareValidator ID="rvVehicleType" runat="server" ControlToValidate="cmbvehicletype" ErrorMessage="Please select the vehicle type." Operator="GreaterThan" Type="Integer" ValueToCompare="0">*</asp:CompareValidator>

                                </td>
                            </tr>
                            
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblmake" runat="server" meta:resourcekey="lblmakeResource1">Make</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtmake" runat="server" meta:resourcekey="txtmakeResource1"></asp:TextBox></td>
                                <td>
                                    <asp:RequiredFieldValidator Enabled="False" ID="reqmake" runat="server" ControlToValidate="txtmake" ErrorMessage="Please enter the make of this vehicle" meta:resourcekey="reqmakeResource1">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblmodel" runat="server" meta:resourcekey="lblmodelResource1">Model</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtmodel" runat="server" meta:resourcekey="txtmodelResource1"></asp:TextBox></td>
                                <td>
                                    <asp:RequiredFieldValidator Enabled="False" ID="reqmodel" runat="server" ControlToValidate="txtmodel" ErrorMessage="Please enter the model of this vehilce" meta:resourcekey="reqmodelResource1">*</asp:RequiredFieldValidator></td>
                            </tr>

                           

                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblregno" runat="server" meta:resourcekey="lblregnoResource1">Registration No</asp:Label></td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtregno" runat="server" meta:resourcekey="txtregnoResource1"></asp:TextBox></td>
                                <td>
                                    <asp:RequiredFieldValidator Enabled="False" ID="reqreg" runat="server" ControlToValidate="txtregno" ErrorMessage="Please enter the registration number of this vehicle" meta:resourcekey="reqregResource1">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="labeltd">Engine Size (cc)</td>
                                <td class="inputtd">
                                    <asp:TextBox ID="txtEngineSize" runat="server" Enabled="False"></asp:TextBox></td>
                                <td>
                                    <asp:CompareValidator ID="cvEngineSize" runat="server" Enabled="False" ControlToValidate="txtEngineSize" Type="Integer" Operator="DataTypeCheck" ErrorMessage="Engine Size must be a number e.g. 1600">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblUom" runat="server" meta:resourcekey="lblUomResource1">Mileage Unit of Measure</asp:Label>
                                </td>
                                <td class="inputtd">
                                    <asp:DropDownList ID="cmbUom" runat="server">
                                        <asp:ListItem Selected="True" Text="Miles" Value="0">Miles</asp:ListItem>
                                        <asp:ListItem Text="Kilometres" Value="1">Kilometres</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblcartype" runat="server" meta:resourcekey="lblcartypeResource1">Engine Type</asp:Label></td>
                                <td class="inputtd">
                                    <asp:DropDownList ID="cmbcartype" runat="server" meta:resourcekey="cmbcartypeResource1">
                                    </asp:DropDownList></td>
                                <td>
                                    <asp:RequiredFieldValidator ID="reqenginetype" runat="server" ErrorMessage="Please select the engine type of this vehicle" ControlToValidate="cmbcartype" SetFocusOnError="True" meta:resourcekey="reqenginetypeResource1">*</asp:RequiredFieldValidator></td>
                            </tr>
                        </table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Mileage Details" meta:resourcekey="WizardStepResource12">
                    <asp:Literal ID="litmileage" runat="server" meta:resourcekey="litmileageResource1"></asp:Literal>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="User Defined Fields" meta:resourcekey="WizardStepResource13">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            <asp:Label ID="lblotherinfo" runat="server" Text="Other Information" meta:resourcekey="lblotherinfoResource1"></asp:Label>
                        </div>
                        <asp:Table ID="tbludf" runat="server" meta:resourcekey="tbludfResource1">
                        </asp:Table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" StepType="Finish" meta:resourcekey="WizardStepResource14">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            <asp:Label ID="lblsummary" runat="server" Text="Summary" meta:resourcekey="lblsummaryResource1"></asp:Label>
                        </div>
                        <table>

                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblsumname" runat="server" Text="Name:" meta:resourcekey="lblsumnameResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:Label ID="lblname" runat="server" Text="Label" meta:resourcekey="lblnameResource1"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblsumemail" runat="server" Text="E-mail Address:" meta:resourcekey="lblsumemailResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:Label ID="lblemail" runat="server" Text="Label" meta:resourcekey="lblemailResource1"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    <asp:Label ID="lblsumusername" runat="server" Text="Username:" meta:resourcekey="lblsumusernameResource1"></asp:Label></td>
                                <td class="inputtd">
                                    <asp:Label ID="lblusername" runat="server" Text="Label" meta:resourcekey="lblusernameResource1"></asp:Label></td>
                            </tr>
                        </table>
                    </div>
                    <asp:Literal ID="litempcontact" runat="server" meta:resourcekey="litempcontactResource1"></asp:Literal>
                    <asp:Literal ID="lithomeaddr" runat="server" meta:resourcekey="lithomeaddrResource1"></asp:Literal>
                    <asp:Literal ID="litempinfo" runat="server" meta:resourcekey="litempinfoResource1"></asp:Literal>
                    <asp:Literal ID="litrole" runat="server" meta:resourcekey="litroleResource1"></asp:Literal>
                    <asp:Literal ID="litsignoff" runat="server" meta:resourcekey="litsignoffResource1"></asp:Literal>
                    <asp:Literal ID="litadvancessignoff" runat="server" meta:resourcekey="litadvancessignoffResource1"></asp:Literal>
                    <asp:Literal ID="litcardetails" runat="server" meta:resourcekey="litcardetailsResource1"></asp:Literal>
                    <asp:Literal ID="litudfs" runat="server" meta:resourcekey="litudfsResource1"></asp:Literal>

                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>

        <input type="hidden" name="requiredStep" id="requiredStep" />
    </div>
</asp:Content>

<asp:Content ID="scriptsContent" runat="server" ContentPlaceHolderID="scripts">
     <script type="text/javascript">
         //Bootstrap td & th style override for buttons
         $(document).ready(function() {
             $('#<%=wizregister.ClientID%> table').find('[id$="ImageButton"]').css('margin-right', '15px');
             $('#<%=wizregister.ClientID%> table').find('[id$="ImageButton"]').css('margin-top', '10px');
             $('#<%=wizregister.ClientID%> table').find('[id$="CancelImageButton"]').css('padding-right', '0px');
             $('#<%=txtdateataddress.ClientID%>').css('margin-right', '5px');

             var width = $(window).width(), height = $(window).height();
             if ((width <= 1024) && (height <= 768)) {
                 $('#maincontentdiv').css('width','600px');
                 $('#timeline').css('width','800px');
                 $('#ctl00_pageContents_wizregister').css('width','600px');
             }
         });
     </script>
</asp:Content>
