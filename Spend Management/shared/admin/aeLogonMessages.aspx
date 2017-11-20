<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smForm.master" CodeBehind="aeLogonMessages.aspx.cs" Inherits="Spend_Management.shared.admin.aeLogonMessages" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>


<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="contentmain" runat="server">

    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.logonMessages.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcLogonMessages.asmx" InlineScript="false" />
            <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        $(document).ready(function () {
            //Pass Dom elements to javascript variables
            SEL.LogonMessages.ModalBackgroundImage = <%=this.backGroundImage.ClientID %>;
            SEL.LogonMessages.ModalIcon = <%=this.iconForTitle.ClientID %>;
            SEL.LogonMessages.ModalCategoryTitle = <%=this.txtCategoryTitle.ClientID %>;
            SEL.LogonMessages.ModalCategoryColor = <%=this.txtCategoryColorCode.ClientID %>;
            SEL.LogonMessages.ModalBody = <%=this.txtBody.ClientID %>;
            SEL.LogonMessages.ModalHeader = <%=this.txtHeader.ClientID %>;
            SEL.LogonMessages.ModalHeaderColor = <%=this.txtHeaderColorCode.ClientID %>;
            SEL.LogonMessages.ModalBodyColor = <%=this.txtBodyColorCode.ClientID %>;
            SEL.LogonMessages.ModalButton = <%=this.txtButtonText.ClientID %>;
            SEL.LogonMessages.ModalButtonLink = <%=this.txtLink.ClientID %>;
            SEL.LogonMessages.ModalButtonColor = <%=this.txtButtonTextColor.ClientID %>;
            SEL.LogonMessages.ModalButtonBackground = <%=this.txtButtonBackGroundColor.ClientID %>;
            SEL.LogonMessages.ModalHdBackground = <%=this.hdInitialBackgroundImage.ClientID%>;
            SEL.LogonMessages.ModalHdIcon = <%=this.lblFileNameHolder.ClientID%>;
            SEL.LogonMessages.ModalModules = <%=this.referenceModuleList.ClientID %>;
            SEL.LogonMessages.ModalSaveButton = <%=this.cmdSave.ClientID %>;

            //Assign variable values for reference
            SEL.LogonMessages.InitialModulesListStore = '<%=this.ListOfModules%>';
            SEL.LogonMessages.IsValidIconCheck = '<%=this.ValidIconCheck%>';
            SEL.LogonMessages.IsValidImageCheck = '<%=this.ValidBackgroundCheck%>';
            //Load Javascript functions
            SEL.LogonMessages.PageLoadFunctions();
        });

         function SetMaxLength(Event, Object, MaxLen) {
                return (Object.value.length <= MaxLen) || (Event.keyCode == 13 || Event.keyCode == 32)
            }
            function validateButtonLink(s, args) {
            if ($('#<%=txtLink.ClientID %>').val().length == 0)
             {
               args.IsValid = false;
             }
             else
             {
              args.IsValid = true;
             }             
             }
            function validateButtonText(s, args) {
            if ($('#<%=this.txtButtonText.ClientID %>').val().length == 0)
             {
               args.IsValid = false;
             }
             else
             {
              args.IsValid = true;
             }            
    }
    </script>
    <div class="formpanel formpanel_padding logonform">

        <div class="sectiontitle">
            General Details
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblCategoryTitle" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtCategoryTitle">Category Title</asp:Label>
            <span class="inputs"><asp:TextBox ID="txtCategoryTitle" runat="server" MaxLength="40"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgTooltipHeaderBGColour" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('46BA2A58-F7D7-40F9-8015-65AE578EFE9A', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
            <asp:Label ID="Label1" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtCategoryTitle">Text Colour</asp:Label><span class="inputs"><asp:TextBox ID="txtCategoryColorCode" runat="server" CssClass="colorTextBox" Text="5e6e66"></asp:TextBox><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></span>
            <asp:Label ID="pnlcattitle" runat="server" CssClass="colorHolder"></asp:Label><span class="inputicon"><cc1:colorpickerextender id="cpCategoryTitle" runat="server" targetcontrolid="txtCategoryColorCode" popupbuttonid="imgCatTitle" samplecontrolid="pnlcattitle"></cc1:colorpickerextender>
            <asp:ImageButton ID="imgCatTitle" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" Width="15px" /></span><asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtCategoryColorCode" ID="RegularExpressionValidator3" ValidationExpression="^[0-9a-fA-F]{6}$" runat="server" ErrorMessage="Please enter a valid value for Category Title Text Colour." ValidationGroup="vgAddEditLogonMessages" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblHeader" CssClass="mandatory" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtHeader">Header*</asp:Label>
            <span class="inputs"><asp:TextBox ID="txtHeader" runat="server" MaxLength="50"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D0CDDE0F-BF2E-42E9-A557-124D5306E73A', 'sm', this);" /></span><span class="inputvalidatorfield">
            <asp:RequiredFieldValidator ValidationGroup="vgAddEditLogonMessages" ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter header text." ControlToValidate="txtHeader" Text="*"></asp:RequiredFieldValidator></span>
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtHeader">Text Colour</asp:Label><span class="inputs"><asp:TextBox ID="txtHeaderColorCode" runat="server" CssClass="colorTextBox" Text="5e6e66"></asp:TextBox></span>
            <asp:Label ID="pnlHeader" runat="server" CssClass="colorHolder"></asp:Label><span class="inputicon"><cc1:colorpickerextender id="cpHeader" runat="server" targetcontrolid="txtHeaderColorCode" popupbuttonid="imgHeader" samplecontrolid="pnlHeader"></cc1:colorpickerextender>
            <asp:ImageButton ID="imgHeader" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" Width="15px" /></span><asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtHeaderColorCode" ID="RegularExpressionValidator1" ValidationExpression="^[0-9a-fA-F]{6}$" runat="server" ErrorMessage="Please enter a valid value for Header Text Colour." ValidationGroup="vgAddEditLogonMessages" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div class="twocolumn" style="margin-bottom: 20px;">
            <span class="labelaligntop"><asp:Label CssClass="mandatory" ID="Label3" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtBody">Body*</asp:Label></span>
            <span class="inputs"><asp:TextBox ID="txtBody" runat="server" onkeypress="return SetMaxLength(event, this, 200);" Width="160Px" Columns="24" Rows="5" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield topalign"><asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon " onmouseover="SEL.Tooltip.Show('EE3DE71C-E9CB-48EA-81FB-219D52447D58', 'sm', this);" /></span><span class="inputvalidatorfield topalign">
            <asp:RequiredFieldValidator ValidationGroup="vgAddEditLogonMessages" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter body text." ControlToValidate="txtBody">*</asp:RequiredFieldValidator></span>
            <asp:Label ID="Label5" CssClass="topalign" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtBody">Text Colour</asp:Label><span class="inputs topalign"><asp:TextBox ID="txtBodyColorCode" runat="server" CssClass="colorTextBox" Text="5e6e66"></asp:TextBox></span>
            <asp:Label ID="pnlBody" runat="server" CssClass="colorHolder topalign"></asp:Label><span class="inputicon topalign"><cc1:colorpickerextender id="Colorpickerextender2" runat="server" targetcontrolid="txtBodyColorCode" popupbuttonid="imgBody" samplecontrolid="pnlBody"></cc1:colorpickerextender>
            <asp:ImageButton ID="imgBody" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" Width="15px" /></span><span class="inputvalidatorfield topalign"><asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtBodyColorCode" ID="RegularExpressionValidator2" ValidationExpression="^[0-9a-fA-F]{6}$" Text="*" runat="server" ErrorMessage="Please enter a valid value for Body Text Colour." ValidationGroup="vgAddEditLogonMessages"></asp:RegularExpressionValidator></span>
        </div>

        <asp:Repeater ID="moduleRepeater" runat="server">
            <HeaderTemplate>
                <div class="twocolumn">
            </HeaderTemplate>
            <ItemTemplate>
                <%# (Container.ItemIndex != 0 && Container.ItemIndex % 2 == 0) ? "</div><div class='twocolumn'>" : string.Empty %>
                <label for='module'><%# Eval("value") %></label>
                <span class="inputs">
                    <input id='module' runat="server" type="checkbox" data-ref='<%# Eval("key") %>' name="moduleCheckBoxes" /></span>
                <span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>

        <input type="hidden" id="referenceModuleList" runat="server" name="referenceModuleList" />

        <div class="twocolumn">
            <label class="mandatory" id="lblBackGroundImage">Background Image*</label>
                      <input type="file" id="backGroundImage" class="backGroundImage" runat="server" name="backGroundImage" style="display: none"/><span class="inputs"><input type = "button" value = "Choose image" onclick ="javascript:document.getElementById('ctl00_contentmain_backGroundImage').click();"></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6798EEE3-CFE8-4A17-8331-9E042D6F6197', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgAddEditLogonMessages" ID="BackgroundImageValidator" runat="server" ErrorMessage="Please upload background image." ControlToValidate="backGroundImage">*</asp:RequiredFieldValidator></span>
            <div class="logonBackgroundDetailsWrapper"><label id="logonBackgroundDetailsWrapper" class="backroundNameHolder" runat="server"></label></div>
       <input type="hidden" runat="server" id="hdInitialBackgroundImage"/>
             </div>
        <div class="twocolumn">
            <label id="lblIconForTitle">Icon</label>
            <input type="file" id="iconForTitle" class="iconForTitle" runat="server" name="iconForTitle" style="display: none"/> <span class="inputs"><input type = "button" value = "Choose image" onclick ="javascript:document.getElementById('ctl00_contentmain_iconForTitle').click();"></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image4" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('AFE880A4-ED8D-4887-80F2-9FA7FE81CE12', 'sm', this);" /></span>
           <span class="inputvalidatorfield"></span><div id="logonIconDetailsWrapper" class="logonIconDetailsWrapper hideElement" runat="server"> <label class="fileNameHolder" id="fileNameHolder" runat="server"></label> <span id="removeIconHolder" class="removeIconHolder">Remove Icon</span><input runat="server" type="hidden" id="lblFileNameHolder"/></div>
        </div>
        <div class="sectiontitle">
            Button
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblButtonText" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtButtonText">Text</asp:Label>
            <span class="inputs" style="display: inline-block !important;">
                <asp:TextBox ID="txtButtonText" MaxLength="20" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield">
                <asp:Image ID="imgTooltipButtonText" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('BAD8F231-947B-44AD-B856-D74FA489D2ED', 'sm', this);" /></span><span class="inputvalidatorfield">
               <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Please enter button text." ValidationGroup="vgAddEditLogonMessages"
                ControlToValidate="txtLink" Text="*" ClientValidationFunction="validateButtonText" /></span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="Label8" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtLink">Link</asp:Label> <span class="inputs" style="display: inline-block !important;"><asp:TextBox ID="txtLink" runat="server" MaxLength="2000"></asp:TextBox></span><span class="inputicon"></span><span class="inputvalidatorfield">
               <asp:CustomValidator ID="cvLink" runat="server" ErrorMessage="Please add a link to the button." ValidationGroup="vgAddEditLogonMessages"
                ControlToValidate="txtButtonText" Text="*" ClientValidationFunction="validateButtonLink" /></span>  
            <span class="inputtooltipfield"><asp:Image ID="imgTooltipButtonLink" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('7938179D-4BF8-4AFA-9587-CC36B9650E2A', 'sm', this);" /></span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblButtonTextColor" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtButtonTextColor">Text colour</asp:Label>
            <span class="inputs inputForColorCode" style="display: inline-block !important;">
                <asp:TextBox ID="txtButtonTextColor" runat="server" CssClass="colorTextBox" Text="5e6e66"></asp:TextBox>
            </span>

            <asp:Label ID="lblButtonBackColor" runat="server" CssClass="colorHolder"></asp:Label>
            <span class="inputicon">
                <cc1:colorpickerextender id="cdButtonTextColor" runat="server" targetcontrolid="txtButtonTextColor" popupbuttonid="imgButtonTextColor" samplecontrolid="lblButtonBackColor"></cc1:colorpickerextender>
                <asp:ImageButton ID="imgButtonTextColor" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" Width="15px" />
            </span>
            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtButtonTextColor" ID="RegularExpressionValidator5" ValidationExpression="^[0-9a-fA-F]{6}$" Text="*" runat="server" ErrorMessage="Please enter a valid value for Button Text Colour." ValidationGroup="vgAddEditLogonMessages"></asp:RegularExpressionValidator>

        </div>
        <div class="twocolumn">
            <asp:Label ID="lblBackGroundColor" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="lblButtonBgColor">Background Colour</asp:Label>

            <span class="inputs">
                <asp:TextBox ID="txtButtonBackGroundColor" runat="server" CssClass="colorTextBox" Text="ffffff"></asp:TextBox>
            </span>
            <asp:Label ID="lblButtonBgColor" runat="server" CssClass="colorHolder"></asp:Label>
            <span class="inputicon">
                <cc1:colorpickerextender id="cdButtonBackGroundColor" runat="server" targetcontrolid="txtButtonBackGroundColor" popupbuttonid="imgButtonBgColor" samplecontrolid="lblButtonBgColor"></cc1:colorpickerextender>
                <asp:ImageButton ID="imgButtonBgColor" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" Width="15px" />
            </span>
            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtButtonBackGroundColor" ID="RegularExpressionValidator6" ValidationExpression="^[0-9a-fA-F]{6}$" Text="*" runat="server" ErrorMessage="Please enter a valid value for Button Background Colour." ValidationGroup="vgAddEditLogonMessages"></asp:RegularExpressionValidator>

        </div>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdSave" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" ValidationGroup="vgAddEditLogonMessages" />&nbsp;&nbsp;
            <asp:ImageButton ID="CmdPreview" CssClass="CmdPreview" runat="server" ImageUrl="/shared/images/buttons/Preview.png" ValidationGroup="vgAddEditLogonMessages" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>&nbsp;&nbsp;
            <asp:ImageButton ID="cmdCancel" runat="server" ImageUrl="/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>

        </div>

    </div>

</asp:Content>
