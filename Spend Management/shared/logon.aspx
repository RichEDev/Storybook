<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logon.aspx.cs" Inherits="Spend_Management.logonPage" EnableTheming="false" Theme="" StylesheetTheme="" %>
<%@ Import Namespace="SpendManagementLibrary" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %><?xml version="1.0" encoding="iso-8859-1"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<!--[if lt IE 7]> <html class="lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>    <html class="lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>    <html class="lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!--> <html> <!--<![endif]-->
<head id="Head1" runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title>expenses logon</title>
    <link id="jQueryUiCss" rel="stylesheet" type="text/css" media="screen" href="/" />
      <link id="BannerSlider" rel="stylesheet" type="text/css" media="screen" href="/" />
    <link rel="stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/shared/css/logon.css?v=2") %>" />
    <link id="favLink" runat="server" rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
    
  <!--[if IE 8]>
        <style>
            #modal-panel {
                background: #252525;
      }
             
        </style>
<![endif]-->


</head>
    
<body id="logonpage" class="logon-background">
    
    <form id="logonForm" runat="server">
    <cc1:ToolkitScriptManager  ID="tsm" runat="server">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
        <asp:ServiceReference Path="~/shared/webServices/svcLogon.asmx" InlineScript="false" />
    </Services>
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.main.js" />
        <asp:ScriptReference Name="common" />
        <asp:ScriptReference Name="tooltips" />
        <asp:ScriptReference Name="logon" />
    </Scripts>
    </cc1:ToolkitScriptManager>
          <script type="text/javascript">
            SEL.Logon.FlashForgottenDetailsButton = <%= this.ShowForgottenDetails.ToString().ToLower() %>;
       
            $(document).ready(function () {
                // make logon links and buttons into jqueryui buttons
                $("input.jqueryui-button, a.jqueryui-button").button();
                
                // prompt the user to use the forgotten details feature
                if (SEL.Logon.FlashForgottenDetailsButton === true) {
                    SEL.Logon.PromptForgottenDetails();
                }

                // auto display the forgotten details panel, (link from self-reg)
                if (window.location.hash === "#forgottenDetails") {
                    SEL.Logon.ShowForgottenDetails();
                }
                
                $("#lnkForgottenDetails").on("click", function () {
                    SEL.Logon.ShowForgottenDetails();
                    return false;
                });

                //Load Slider 
                SEL.Logon.PageLoadfunctionsForSliders();
            });
        </script>

    <cc1:ModalPopupExtender ID="mdlMasterPopup" runat="server" OnCancelScript="SEL.MasterPopup.HideMasterPopup();" OnOkScript="SEL.MasterPopup.HideMasterPopup();" BackgroundCssClass="modalMasterBackground" PopupControlID="pnlMasterPopup" TargetControlID="lnkMasterPopup" DropShadow="False">
    </cc1:ModalPopupExtender>

    <asp:Panel ID="pnlMasterPopup" runat="server" CssClass="errorModal" Style="display: none; z-index: 16000099;">
            <div id="divMasterPopup"></div>
            <div>
            <img src="/shared/images/buttons/btn_close.png" id="btnMasterPopup" alt="OK" onclick="SEL.MasterPopup.HideMasterPopup();" style="cursor: pointer;" />
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkMasterPopup" runat="server" Style="display: none;" Text="&nbsp;" NavigateUrl="javascript:void(0);"></asp:HyperLink>

        <div class="page-wrapper">
            
            <div id="titlebar-wrapper">
                <div id="titlebar">
                    <h1><asp:Literal runat="server" ID="litPageTitle"></asp:Literal></h1>
                    <a href="http://www.selenity.com" target="_blank" title="Innovative technology by Selenity">Innovative Technology by Selenity</a>
                </div>
            </div>
            
            <div class="page-wrapper">
                <div id="left" class="left-wrapper">
                <div id="front-wrapper">
                                        
                    <div id="front-panel">
                       
  <asp:Literal ID="userIPAddress" runat="server" />
                        
                        <div id="modal-panel">
                           
                            <div id="logonTopPanel">
                                
      <div id="logonleftpanel">
                                    
                                    <div id="logonPanel">

                                        <div class="formrow">
                                            <div class="logontooltip"><asp:Image ID="imgTooltipCompany" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('dfb6903f-1b5c-4607-b0eb-82bf15726d34', 'sm', this);" /></div>
                                            <asp:Label ID="lblCompanyID" AssociatedControlID="txtCompanyID" runat="server" Text="Company ID"></asp:Label>
                                            <asp:TextBox ID="txtCompanyID" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator CssClass="required" ID="rfCompanyID" runat="server" ControlToValidate="txtCompanyID" Text="*" Display="Dynamic" ValidationGroup="Logon"></asp:RequiredFieldValidator>
                                        </div>

                                        <div class="formrow">
                                            <div class="logontooltip"><asp:Image ID="imgTooltipUsername" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('8c5475d9-b6e4-48f6-9d17-7d804312fd58', 'sm', this);" /></div>
                                            <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server" Text="Username"></asp:Label>
                                            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfUsername" runat="server" ControlToValidate="txtUsername" Text="*" Display="Dynamic" ValidationGroup="Logon"></asp:RequiredFieldValidator>
                                        </div>

                                        <div class="formrow">
                                            <div class="logontooltip"><asp:Image ID="imgTooltipPassword" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('67e44c3a-eae3-46d7-ad3c-67b4c9470b40', 'sm', this);" /></div>
                                            <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server" Text="Password"></asp:Label>
                                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        </div>

	                                    <div class="messageContainer">
	                                        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
      </div>
        
                                        <div class="formrow">
                                            <div class="logontooltip"><asp:Image ID="imgTooltipRememberDetails" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('667d54eb-b756-479d-9f01-8daef71a7ee1', 'sm', this);" /></div>
                                            <asp:Label ID="lblRememberDetails" runat="server" AssociatedControlID="chkRememberDetails" Text="Remember Details"></asp:Label>
                                            <asp:CheckBox ID="chkRememberDetails" runat="server" CssClass="checkbox" />
      </div>
                                
                                        <div id="rememberdetailsdisclaimer"><p>We advise you not to select remember details if you are using a public or shared computer.</p></div>
                                       <div id="logonButtons">
                                            <div id="logonbutton"><asp:Button ID="btnLogon" CssClass="jqueryui-button" runat="server" ToolTip="Logon" Text="logon" onclick="btnLogon_Click" ValidationGroup="Logon" /></div>
    </div>
                                        
	</div>

                                    <asp:Panel id="forgottenDetailsPanel" runat="server">
        <div id="forgottenDetailsIntro">If you have forgotten your logon details please enter your email address below</div>
                                
                                        <div class="formrow">
                                            <div class="logontooltip"><asp:Image ID="imgTooltipEmailAddress" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('031da2ae-7055-42fd-ad16-a858d0b1f1f0', 'sm', this);" /></div>
                                            <asp:Label ID="lblEmailAddress" AssociatedControlID="txtEmailAddress" runat="server" Text="Email Address"></asp:Label>
                                            <asp:TextBox ID="txtEmailAddress" runat="server" onkeypress="return SEL.Forms.RunOnEnter(event, SEL.Logon.SubmitForgottenDetails);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfEmail" runat="server" ControlToValidate="txtEmailAddress" Text="*" Display="Dynamic" ValidationGroup="ForgottenDetails" ErrorMessage="Please enter an email address"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regEmail" runat="server" ValidationExpression="\w+([-+.'!#$%&*/=?^_`~{}|]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="txtEmailAddress" Display="Dynamic" ValidationGroup="ForgottenDetails" ErrorMessage="Please enter a valid email address">*</asp:RegularExpressionValidator>
                                        </div>

                                        <div id="forgottenDetailsMessage" class="messageContainer"></div>

                                        <div id="forgottenDetailsButtons">
                                            <!-- todo these spans can probably go [1]-->
                                            <span id="spanForgottenDetailsSubmit">
                                                <a id="imgForgottenDetailsSubmit" class="jqueryui-button" href="javascript:SEL.Logon.SubmitForgottenDetails()">submit</a>
                                            </span>
                                        </div>
    </asp:Panel>

                                </div>

                                <div id="logonrightpanel">
                                    <asp:Image ID="imgBrandingLogo" runat="server" AlternateText="" ToolTip="" CssClass="productlogo" ImageUrl="~/shared/images/branding/expenses76w_77h.png" />
                                    <asp:HyperLink ID="lnkForgottenDetails" CssClass="jqueryui-button" runat="server" Text="forgotten details" ToolTip="Forgotten Details" NavigateUrl="#forgottenDetails"  />
                                    <asp:HyperLink ID="lnkSelfRegistration" CssClass="jqueryui-button" runat="server" Text="register" NavigateUrl="~/shared/register.aspx" />
                                </div>

                            </div>
                            
                            <!-- #information messages -->
	                        <div class="logonBottomPanel" id="informationcontainer" runat="server">
                                <div><asp:Literal ID="litInformationMessages" runat="server"></asp:Literal></div>
	                        </div>

                        </div>
  </div>
  
</div>
                    </div>
                <div id="right">
                    <div class="preLoaderDiv">
                        <img class="preLoaderImage" src="../shared/images/easytree_loading.gif" />
                    </div>
                    <div id="bannerHolder" style="visibility: hidden;">
                        <!--[if IE]>
                                <div class='slider-container-ie'>
                        <![endif]-->
                        <asp:Repeater ID="BannerGenerator" runat="server">
                            <HeaderTemplate>
                                <ul id="bxSlider">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <div class="contentHolder">
                                        <span>
                                            <img id="icon" class='<%#CheckForNullOrEmpty(Eval("Icon")) ? "hideElement": "iconHolderForBanner"%>' src='../Logos/MarketingInformation/icons/<%# Eval("Icon")%>' />
                                        </span>
                                        <span class='<%#CheckForNullOrEmpty(Eval("CategoryTitle")) ? "hideElement": "content-title"%>' style='color: #<%# Eval("CategoryTitleColourCode") %>;'><%# Eval("CategoryTitle") %></span>
                                        <div class="content-description">
                                            <p class="title" style='color: #<%# Eval("HeaderTextColourCode") %>;'><%# Eval("HeaderText") %></p>
                                            <p class="copy" style='color: #<%# Eval("BodyTextColourCode") %>;'><%# Eval("BodyText") %></p>

                                            <a class='<%#CheckForNullOrEmpty(Eval("ButtonLink")) ? "hideElement": "banner-button"%>' href='<%# Eval("ButtonLink") %>' style='color: #<%# Eval("ButtonForeColour") %>; background-color: #<%# Eval("ButtonbackGroundColour")%>;' onclick="target='_blank';"><%# Eval("ButtonText") %></a>
                                        </div>
                                    </div>
                                    <img class="sliderImage" src='../Logos/MarketingInformation/<%# Eval("BackgroundImage")%>' />
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>

                        </asp:Repeater>


                        <div id="previewRequest"></div>

                        <!--[if IE]></div><![endif]-->
                    </div>

                </div>

                <asp:Panel id="pnlLocked" runat="server" Visible="False">
                    <div id="divlocked" class="speech">Your account is currently locked. Please check your email for instructions.</div>
                </asp:Panel>

            </div>

        </div>


    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    </form>
</body>
</html>
 