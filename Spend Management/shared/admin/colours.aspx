<%@ Page language="c#" Inherits="Spend_Management.colours" MasterPageFile="~/masters/smForm.master" CodeBehind="colours.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<asp:Content runat="server" ID="CustomStyles" ContentPlaceHolderID="styles">
    <style type="text/css">
        .formpanel table tbody tr {
            height: auto;
        }
    </style>
    <!--[if IE 7]>
        <style>

            .formbuttons {
                width: 350px;
            }

            .inputs > SPAN {
                vertical-align: auto;
            }

        </style>
    <![endif]-->
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<asp:ScriptManagerProxy runat="server" ID="smp">
    <Services>
        <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/svcTooltip.asmx" />
        <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/svcColours.asmx" />
    </Services>
    <Scripts>
        <asp:ScriptReference Name="tooltips" />
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.colours.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
<link rel="stylesheet" type="text/css"  media="screen" href="../css/styles.aspx?style=logon" />

<style type="text/css">
        .formpanel .twocolumn .inputs input {
            width: 135px;
            float: left;
            margin-top: 0;
        }
</style>


    <div class="formpanel formpanel_padding">

        <div class="sectiontitle">
            <asp:Label ID="Label15" runat="server" Text="Header"></asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label7" runat="server" meta:resourcekey="Label7Resource1" AssociatedControlID="txtheaderbg">Background Colour</asp:Label>
            <span class="inputs">

                <asp:TextBox ID="txtheaderbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnlheaderbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">

                <cc1:ColorPickerExtender ID="colmenubg" runat="server" TargetControlID="txtheaderbg" PopupButtonID="imgheaderbg" SampleControlID="pnlheaderbg"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgheaderbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                
                <asp:Image ID="imgTooltipHeaderBGColour" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('936CC7A0-AF34-4A21-B4AD-351A06A0BD0B', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Header Background Colour." ControlToValidate="txtheaderbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="Label14" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="txtheadertxt">Breadcrumb Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtheadertxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnlheadertxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colheadertxt" runat="server" TargetControlID="txtheadertxt" PopupButtonID="imgheadertxt" SampleControlID="pnlheadertxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgheadertxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">

                <asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('F9A3FF21-DBFB-4CB2-9A91-3EBF266FBF4E', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Header Breadcrumb Text Colour." ControlToValidate="txtheadertxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">Page Title</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label8" runat="server" meta:resourcekey="Label8Resource1" AssociatedControlID="txtpagetitletxt">Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtpagetitletxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnlpagetitletxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colpagetitletxt" runat="server" TargetControlID="txtpagetitletxt" PopupButtonID="imgpagetitletxt" SampleControlID="pnlpagetitletxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgpagetitletxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('9E36D8EA-B2DD-451C-9101-286118431262', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Page Title Background Colour." ControlToValidate="txtpagetitletxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
         </div>

        <div class="sectiontitle">
            <asp:Label ID="Label13" runat="server" meta:resourcekey="Label13Resource1">Section Heading</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label16" runat="server" meta:resourcekey="Label16Resource1" AssociatedControlID="txtsectionheadingunderline">Underline Colour</asp:Label>
            <span class="inputs">

                <asp:TextBox ID="txtsectionheadingunderline" runat="server"></asp:TextBox>
                <asp:Label ID="pnlsectionheadingunderline" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">

                <cc1:ColorPickerExtender ID="colsectionheadingunderline" runat="server" TargetControlID="txtsectionheadingunderline" PopupButtonID="imgsectionheadingunderline" SampleControlID="pnlsectionheadingunderline"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgsectionheadingunderline" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                
                <asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D2160ACE-16DE-4BB4-BDE0-03FA18820F00', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Section Heading Underline Colour." ControlToValidate="txtsectionheadingunderline" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>

            <asp:Label ID="Label19" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="txtsectionheadingtxt">Text Colour</asp:Label>

            <span class="inputs">
                <asp:TextBox ID="txtsectionheadingtxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnlsectionheadingtxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colsectionheadingtxt" runat="server" TargetControlID="txtsectionheadingtxt" PopupButtonID="imgsectionheadingtxt" SampleControlID="pnlsectionheadingtxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgsectionheadingtxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">

                <asp:Image ID="Image4" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('17AFE39E-308B-4A6D-987F-320ED7C9E840', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator14" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Section Heading Text Colour." ControlToValidate="txtsectionheadingtxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label6" runat="server" meta:resourcekey="Label6Resource1">Menu Options</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label12" runat="server" meta:resourcekey="Label12Resource1" Text="Hover Text Colour" AssociatedControlID="txthoverbg"></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txthoverbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnlhoverbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colhoverbg" runat="server" TargetControlID="txthoverbg" PopupButtonID="imghoverbg" SampleControlID="pnlhoverbg"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imghoverbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image11" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('C03A3172-6545-4508-937A-F1D6DC0E475B', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Hover Text Colour for Menu Options." ControlToValidate="txthoverbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="lblPageOptionColour" runat="server" meta:resourcekey="Label12Resource1" AssociatedControlID="txtPageOptionColour">Standard Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtPageOptionColour" runat="server"></asp:TextBox>
                <asp:Label ID="pnlPageOptionColour" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colPageOptionColour" runat="server" TargetControlID="txtPageOptionColour" PopupButtonID="imgPageOptionColour" SampleControlID="pnlPageOptionColour"></cc1:ColorPickerExtender>
                    <asp:ImageButton ID="imgPageOptionColour" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield"><asp:Image ID="Image12" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('7B3180B2-1C1D-4DC5-97EC-6CE9CC731BBF', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="revPageOptionColour" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Menu Options Standard Text Colour." ControlToValidate="txtPageOptionColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label1" runat="server" meta:resourcekey="Label13Resource1">Page Options</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label21" runat="server" meta:resourcekey="Label16Resource1" AssociatedControlID="txtpageoptionsbg">Background Colour</asp:Label>
            <span class="inputs">

                <asp:TextBox ID="txtpageoptionsbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnlpageoptionsbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">

                <cc1:ColorPickerExtender ID="colpageoptionsbg" runat="server" TargetControlID="txtpageoptionsbg" PopupButtonID="imgpageoptionsbg" SampleControlID="pnlpageoptionsbg"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgpageoptionsbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                
                <asp:Image ID="Image5" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('04CDFE77-A62B-472A-A7C8-C971C179345E', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Page Options Background Colour." ControlToValidate="txtpageoptionsbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>

            <asp:Label ID="Label23" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="txtpageoptionstxt">Text Colour</asp:Label>

            <span class="inputs">
                <asp:TextBox ID="txtpageoptionstxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnlpageoptionstxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colpageoptiontxt" runat="server" TargetControlID="txtpageoptionstxt" PopupButtonID="imgpageoptionstxt" SampleControlID="pnlpageoptionstxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgpageoptionstxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">

                <asp:Image ID="Image17" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('431C2BF9-A613-4B3C-A56E-F56E43B8D7B2', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator15" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Page Options Text Colour." ControlToValidate="txtpageoptionstxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div> 

        <div class="sectiontitle">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Field Label</asp:Label>
        </div>
        <div class="twocolumn">
           
            <asp:Label ID="Label9" runat="server" meta:resourcekey="Label9Resource1" AssociatedControlID="txtfieldtext">Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtfieldtext" runat="server"></asp:TextBox>
                <asp:Label ID="pnlfieldtext" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colfieldtext" runat="server" TargetControlID="txtfieldtext" PopupButtonID="imgfieldtext" SampleControlID="pnlfieldtext"></cc1:ColorPickerExtender>
                        <asp:ImageButton ID="imgfieldtext" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image6" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('2F2DF9BC-4FB3-4743-BD6A-DE18D1B2BE46', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Field Label Text Colour." ControlToValidate="txtfieldtext" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>      

        <div class="sectiontitle">
            <asp:Label ID="Label30" runat="server" meta:resourcekey="Label13Resource1">Tab Option</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label31" runat="server" meta:resourcekey="Label16Resource1" AssociatedControlID="txttaboptionbg">Background Colour</asp:Label>
            <span class="inputs">

                <asp:TextBox ID="txttaboptionbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnltaboptionbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">

                <cc1:ColorPickerExtender ID="coltaboptionbg" runat="server" TargetControlID="txttaboptionbg" PopupButtonID="imgtaboptionbg" SampleControlID="pnltaboptionbg"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgtaboptionbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                
                <asp:Image ID="Image20" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('1471D670-788D-40F3-B37E-8452BAA2547A', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator18" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Tab Option Background Colour." ControlToValidate="txttaboptionbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>

            <asp:Label ID="Label33" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="txttaboptiontxt">Text Colour</asp:Label>

            <span class="inputs">
                <asp:TextBox ID="txttaboptiontxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnltaboptiontxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
               <cc1:ColorPickerExtender ID="coltaboptiontxt" runat="server" TargetControlID="txttaboptiontxt" PopupButtonID="imgtaboptiontxt" SampleControlID="pnltaboptiontxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgtaboptiontxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">

                <asp:Image ID="Image21" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('548E4BB8-9E94-4A5C-867E-82F265CA9C25', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator19" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Tab Option Text Colour." ControlToValidate="txttaboptiontxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label25" runat="server" meta:resourcekey="Label13Resource1">Table Header</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label26" runat="server" meta:resourcekey="Label16Resource1" AssociatedControlID="txttableheaderbg">Background Colour</asp:Label>
            <span class="inputs">

                <asp:TextBox ID="txttableheaderbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnltableheaderbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">

                <cc1:ColorPickerExtender ID="coltableheaderbg" runat="server" TargetControlID="txttableheaderbg" PopupButtonID="imgtableheaderbg" SampleControlID="pnltableheaderbg"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgtableheaderbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                
                <asp:Image ID="Image18" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('AAB4CB93-F92C-45C5-966C-5F01597D7B76', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator16" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Header Background Colour." ControlToValidate="txttableheaderbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>

            <asp:Label ID="Label28" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="txttableheadertxt">Text Colour</asp:Label>

            <span class="inputs">
                <asp:TextBox ID="txttableheadertxt" runat="server"></asp:TextBox>
                <asp:Label ID="pnltableheadertxt" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="coltableheadertxt" runat="server" TargetControlID="txttableheadertxt" PopupButtonID="imgtableheadertxt" SampleControlID="pnltableheadertxt"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgtableheadertxt" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">

                <asp:Image ID="Image19" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('2ABC08CE-7E76-4F71-A83B-12724E496019', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator17" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Header Text Colour." ControlToValidate="txttableheadertxt" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1">Table Row</asp:Label>
        </div>
    <div class="twocolumn">
            <asp:Label ID="Label10" runat="server" meta:resourcekey="Label10Resource1" Text="Background Colour" AssociatedControlID="txttablerow"></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txttablerow" runat="server"></asp:TextBox>
                <asp:Label ID="pnltablerow" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="coltablerow" runat="server" TargetControlID="txttablerow" PopupButtonID="imgtablerow" SampleControlID="pnltablerow"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgtablerow" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image8" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('24F0B656-8E01-40E9-AB38-2AFD786638D5', 'sm', this);" /></span>
            <span class="inputvalidatorfield">
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Row Background Colour." ControlToValidate="txttablerow" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="lblRowFGColour" runat="server" meta:resourcekey="Label10Resource1" AssociatedControlID="txtRowFGColour">Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtRowFGColour" runat="server"></asp:TextBox>
                <asp:Label ID="pnlRowFG" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colRowFG" runat="server" TargetControlID="txtRowFGColour" PopupButtonID="imgRowFG" SampleControlID="pnlRowFG"></cc1:ColorPickerExtender>
                        <asp:ImageButton ID="imgRowFG" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image7" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('01764E57-08A8-4685-9A63-82C5107C35B7', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="revRowFG" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Row Text Colour." ControlToValidate="txtRowFGColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        <div class="sectiontitle">
            <asp:Label ID="Label5" runat="server" meta:resourcekey="Label5Resource1">Table Alternate Row</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="Label11" runat="server" meta:resourcekey="Label11Resource1" Text="Background Colour" AssociatedControlID="txtalternatetablerow"></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtalternatetablerow" runat="server"></asp:TextBox>
                <asp:Label ID="pnlalternatetablerow" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colalternatetablerow" runat="server" TargetControlID="txtalternatetablerow" PopupButtonID="imgalternatetablerow" SampleControlID="pnlalternatetablerow"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgalternatetablerow" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image9" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('79F010B1-C630-4DB9-B303-52A16B5C2058', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Alternate Row Background Colour." ControlToValidate="txtalternatetablerow" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="lblAltRowFGColour" runat="server" meta:resourcekey="Label10Resource1" AssociatedControlID="txtAltRowFGColour">Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtAltRowFGColour" runat="server"></asp:TextBox><asp:Label ID="pnlAltRowFG" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colAltRowFG" runat="server" TargetControlID="txtAltRowFGColour" PopupButtonID="imgAltRowFG" SampleControlID="pnlAltRowFG"></cc1:ColorPickerExtender>
                        <asp:ImageButton ID="imgAltRowFG" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                            <asp:Image ID="Image10" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('21535B74-CBCD-403D-AE06-F50CD8715A97', 'sm', this);" Width="16px" />

            </span>
            <span class="inputvalidatorfield">
                                <asp:RegularExpressionValidator ID="revAltRowFG" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Table Alternate Row Text Colour." ControlToValidate="txtAltRowFGColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>

        

    <div class="sectiontitle">
            <asp:Label ID="Label17" runat="server" meta:resourcekey="Label7Resource1">Tooltip Colour</asp:Label>
        </div>
    <div class="twocolumn">
            <asp:Label ID="lblTooltipBackground" runat="server" meta:resourcekey="Label12Resource1" Text="Background Colour" AssociatedControlID="txtTooltipbg"></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtTooltipbg" runat="server"></asp:TextBox>
                <asp:Label ID="pnlTooltipbg" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colTooltipbg" runat="server" TargetControlID="txtTooltipbg" PopupButtonID="imgTooltipbg" SampleControlID="pnlTooltipbg"></cc1:ColorPickerExtender>
            <asp:ImageButton ID="imgTooltipbg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="imgTooltipbgIcon" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('C7F742FD-0BEC-4D83-825E-D24AF82B76C5', 'sm', this);" />
            
            </span>
            <span class="inputvalidatorfield">
                <asp:RegularExpressionValidator ID="RegExTooltipbg" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Tooltip Colour Background Colour." ControlToValidate="txtTooltipbg" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="lblTooltipTextColour" runat="server" meta:resourcekey="Label12Resource1" AssociatedControlID="txtTooltipTextColour">Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtTooltipTextColour" runat="server"></asp:TextBox>
                <asp:Label ID="pnlTooltipTextColour" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>
                
           </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colTooltipTextColour" runat="server" TargetControlID="txtTooltipTextColour" PopupButtonID="imgTooltipTextColour" SampleControlID="pnlTooltipTextColour"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgTooltipTextColour" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

             </span>
            <span class="inputtooltipfield">
                    <asp:Image ID="imgTooltipTextColourIcon" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D68893E9-1159-4C66-8577-E235CE25526D', 'sm', this);" />
          
            </span>
            <span class="inputvalidatorfield">
                <asp:RegularExpressionValidator ID="RegExTooltipTextColour" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Tooltip Colour Text Colour." ControlToValidate="txtTooltipTextColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>
            </span>
    </div>

        <div class="sectiontitle">
            <asp:Label ID="LabelGreenLightColours" runat="server" meta:resourcekey="LabelGreenLightColoursResource1">GreenLight Colours</asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="LabelGreenLightField" runat="server" meta:resourcekey="LabelGreenLightFieldResource1" Text="Label Text Colour" AssociatedControlID="txtGreenLightField"></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtGreenLightField" runat="server"></asp:TextBox>
                <asp:Label ID="pnlGreenLightField" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colGreenLightField" runat="server" TargetControlID="txtGreenLightField" PopupButtonID="imgGreenLightField" SampleControlID="pnlGreenLightField"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgGreenLightField" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image13" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('31C027D0-3AA6-48F0-88DC-572AC774BEEB', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for GreenLight Colours Label Text Colour." ControlToValidate="txtGreenLightField" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="LabelGreenLightSectionText" runat="server" meta:resourcekey="LabelGreenLightSectionTextResource1" AssociatedControlID="txtGreenLightSectionText">Section Text Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtGreenLightSectionText" runat="server"></asp:TextBox>
                <asp:Label ID="pnlGreenLightSectionText" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon">
                <cc1:ColorPickerExtender ID="colGreenLightSectionText" runat="server" TargetControlID="txtGreenLightSectionText" PopupButtonID="imgGreenLightSectionText" SampleControlID="pnlGreenLightSectionText"></cc1:ColorPickerExtender>
                        <asp:ImageButton ID="imgGreenLightSectionText" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image14" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('F113D0DD-F856-4404-B046-EB630B466F39', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for GreenLight Colours Section Text Colour." ControlToValidate="txtGreenLightSectionText" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="LabelGreenLightSectionBackground" runat="server" meta:resourcekey="LabelGreenLightSectionBackgroundResource1" AssociatedControlID="txtGreenLightSectionBackground">Section Background Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtGreenLightSectionBackground" runat="server"></asp:TextBox>
                <asp:Label ID="pnlGreenLightSectionBackground" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon"><cc1:ColorPickerExtender ID="colGreenLightSectionBackground" runat="server" TargetControlID="txtGreenLightSectionBackground" PopupButtonID="imgGreenLightSectionBackground" SampleControlID="pnlGreenLightSectionBackground"></cc1:ColorPickerExtender>
                <asp:ImageButton ID="imgGreenLightSectionBackground" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image15" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('5755F34C-F5EE-4615-905E-E2563B72909C', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator12" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for GreenLight Colours Section Background Colour." ControlToValidate="txtGreenLightSectionBackground" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
            <asp:Label ID="LabelGreenLightSectionUnderline" runat="server" meta:resourcekey="LabelGreenLightSectionUnderlineResource1" AssociatedControlID="txtGreenLightSectionUnderline">Section Underline Colour</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtGreenLightSectionUnderline" runat="server"></asp:TextBox>
                <asp:Label ID="pnlGreenLightSectionUnderline" runat="server" Style="width: 19px; height: 19px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>

            </span>
            <span class="inputicon"><cc1:ColorPickerExtender ID="colGreenLightSectionUnderline" runat="server" TargetControlID="txtGreenLightSectionUnderline" PopupButtonID="imgGreenLightSectionUnderline" SampleControlID="pnlGreenLightSectionUnderline"></cc1:ColorPickerExtender>
                        <asp:ImageButton ID="imgGreenLightSectionUnderline" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />

            </span>
            <span class="inputtooltipfield">
                <asp:Image ID="Image16" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('5F63028E-5D63-40D8-9294-37BCD778FD7A', 'sm', this);" />

            </span>
            <span class="inputvalidatorfield">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for GreenLight Colours Section Underline Colour." ControlToValidate="txtGreenLightSectionUnderline" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator>

            </span>
        </div>
    
    <div class="formbuttons">
         <helpers:CSSButton runat="server" ID="btnSave" meta:resourcekey="btnSaveResource1" CausesValidation="False" OnClientClick="validateform();" Text="save" />
         <helpers:CSSButton runat="server" ID="cmdrestore" OnClientClick="SEL.Colours.Restore(); return false;" Text="restore defaults" />
         <helpers:CSSButton runat="server" ID="btnCancel" meta:resourcekey="btnCancelResource1" CausesValidation="False" Text="cancel" OnClientClick="" />
    </div>
</div>

<tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
   </asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmenu">
</asp:Content>

<asp:Content runat="server" ID="customScripts" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
        $(document).ready(function () {
            var width = $(window).width(), height = $(window).height();
            if ((width <= 1024) && (height <= 768)) {
                $('.formpanel .twocolumn .inputs').attr('style', 'width: 170px !important;');
                $('.formpanel .twocolumn label').css('width', '40%');
            }
        });
    </script>
</asp:Content>

