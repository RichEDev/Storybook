<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false"
    Inherits="frameworkWebApp.Framework2006.UserSetupDetail" Codebehind="UserSetupDetail.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
  <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        </asp:ScriptManagerProxy>
    <script language="javascript">
            function CheckPwdLength(objsrc, args)
            {
                var pwdval = new String(args.Value);
                //alert('length = ' + pwdval.length);
                if(pwdval.length < 6 || pwdval.length > 10)
                {
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
            
            function PageSetup()
            {
				var cntl;
				
				cntl = document.getElementById('txtFullName');
				if(cntl != null)
				{
					cntl.focus();
				}
            }
    </script>

    <div class="inputpanel">
        <asp:Label ID="lblErrorString" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            User Details</div>
                    <div>
            <table class="datatbl">
                <tr>
                    <td class="row2">
                        <img src="images/information.gif" />&nbsp;
                        <asp:Label runat="server" ID="lblPasswordPolicy"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div>&nbsp;</div>
        <table>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblFullName" runat="server">full name</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox ID="txtFullName" runat="server" TabIndex="1" MaxLength="30"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="reqfullname" runat="server" ControlToValidate="txtFullName"
                        ErrorMessage="Full Name definition must be specified" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    <cc1:ValidatorCalloutExtender ID="reqexFullName" runat="server" TargetControlID="reqfullname">
                    </cc1:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblUserName" runat="server">logon id</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox ID="txtUserName" runat="server" MaxLength="50" TabIndex="2"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="requsername" runat="server" ControlToValidate="txtUserName"
                        ErrorMessage="A logon id must be specified" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    <cc1:ValidatorCalloutExtender ID="reqexusername" runat="server" TargetControlID="requsername">
                    </cc1:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td class="labeltd">
                    Position
                </td>
                <td class="inputtd">
                    <asp:TextBox ID="txtPosition" runat="server" TabIndex="3"></asp:TextBox>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="3" class="inputtd">
                    <asp:Literal ID="litTip" runat="server"></asp:Literal>
                </td>
            </tr>
            <asp:UpdatePanel runat="server" ID="PwdActivationPanel">
                <ContentTemplate>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblPassword" runat="server">password</asp:Label>
                        </td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtPassword" runat="server" MaxLength="25" TextMode="Password" Wrap="False"
                                TabIndex="4"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword"
                                ErrorMessage="A password must be specified" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            <cc1:ValidatorCalloutExtender ID="reqexPassword" runat="server" TargetControlID="reqPassword">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblConfirmPwd" runat="server">confirm pwd</asp:Label>
                        </td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtConfirmPwd" runat="server" TextMode="Password" TabIndex="5" MaxLength="25"></asp:TextBox>
                        </td>
                        <td>
                            <asp:CompareValidator ID="validatePwdEq" runat="server" ControlToCompare="txtPassword"
                                ControlToValidate="txtConfirmPwd" ErrorMessage="Password and confirmation password do not match"
                                SetFocusOnError="True">*</asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="reqConfirmPwd" runat="server" ControlToValidate="txtConfirmPwd"
                                ErrorMessage="Confirmation Password entry required" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            <cc1:ValidatorCalloutExtender ID="valexPwdEq" runat="server" TargetControlID="validatePwdEq">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexConfirmPwd" runat="server" TargetControlID="reqConfirmPwd">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                </ContentTemplate>
            </asp:UpdatePanel>
            <tr>
                <td colspan="3" class="inputtd">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblEmail" runat="server">email</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox ID="txtEmail" TabIndex="6" runat="server" MaxLength="100"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="regEmail" runat="server" 
                        ControlToValidate="txtEmail" ErrorMessage="Invalid email address entered" 
                        SetFocusOnError="True" 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator>
                    <cc1:ValidatorCalloutExtender ID="regexEmail" runat="server" 
                        TargetControlID="regEmail">
                    </cc1:ValidatorCalloutExtender>
                    <asp:RequiredFieldValidator ID="reqEmail" runat="server" 
                        ControlToValidate="txtEmail" ErrorMessage="Email address is mandatory" 
                        SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    <cc1:ValidatorCalloutExtender ID="reqexEmail" 
                        runat="server" TargetControlID="reqEmail">
                    </cc1:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblHint" runat="server">hint</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox ID="txtHint" runat="server" TabIndex="7" MaxLength="50"></asp:TextBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblIconSize" runat="server">icon size</asp:Label>
                </td>
                <td class="inputtd">
                    <asp:DropDownList runat="server" ID="lstIconSize">
                        <asp:ListItem Value="48">Large</asp:ListItem>
                        <asp:ListItem Value="24">Medium</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Label ID="hiddenuserid" runat="server" Visible="False"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Location Access Permission</div>
        <asp:Literal ID="litLocations" runat="server"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="~/buttons/update.gif" />&nbsp;<asp:ImageButton
            runat="server" ID="cmdCancel" ImageUrl="~/buttons/cancel.gif" 
            CausesValidation="False" /></div>
</asp:Content>
<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="contentmenu">>
</asp:Content>
