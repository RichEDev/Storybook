<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="aedocumenttemplate.aspx.cs" Inherits="Spend_Management.aedocumenttemplate" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:content id="Content1" runat="server" contentplaceholderid="contentmain">
    <link rel="stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/shared/css/layout.css") %>" />
    <link rel="stylesheet" type="text/css"  media="screen" href="<% = ResolveUrl("~/shared/css/styles.aspx") %>" />
	<asp:ScriptManagerProxy runat="server" ID="SMgrProxy">
		<Services>
			<asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcDocumentMerge.asmx" />
		</Services>
		<Scripts>                                                              
            <asp:ScriptReference Name="tooltips" />
		</Scripts>
	</asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
    <div class="sectiontitle">Add / Edit Document Template</div>
    <div class="twocolumn">
        <asp:Label runat="server" ID="lblDocName" AssociatedControlID="txtDocName" CssClass="mandatory">Template name*</asp:Label>
        <span class="inputs"><asp:TextBox runat="server" ID="txtDocName" CssClass="fillspan" MaxLength="150"></asp:TextBox></span>
        <span class="inputicon"></span>
        <span class="inputtooltipfield"></span>
        <span class="inputvalidatorfield">
        <asp:RequiredFieldValidator runat="server" ID="reqDocName" ControlToValidate="txtDocName" ErrorMessage="A template name is mandatory" Text="*" ValidationGroup="vgMain"></asp:RequiredFieldValidator>

        </span>
    </div>
    <div class="onecolumn">
        <asp:Label runat="server" ID="lblDocDesc" AssociatedControlID="txtDocDesc"><p class="labeldescription">Template description</p></asp:Label>
        <span class="inputs"><asp:TextBox runat="server" ID="txtDocDesc" Rows="10" TextMode="MultiLine" CssClass="fillspan" maxlength="500"></asp:TextBox></span>
        <span class="inputicon"></span>
        <span class="inputtooltipfield"></span>
        <span class="inputvalidatorfield"></span>
    </div>
    <div class="onecolumnsmall">
        <asp:Label runat="server" id="lblMergeProject" AssociatedControlID="cmbMergeProject" CssClass="mandatory">Document configuration*</asp:Label>
        <span class="inputs input-box-document">
            <asp:DropDownList id="cmbMergeProject" runat="server">
            </asp:DropDownList>        
        </span>
        <span class="inputicon"></span>
        <span class="inputtooltipfield">
            <asp:Image ID="imgTPMergeProject" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('5076546b-36a7-4975-a332-1c87adad9013', 'sm', this);" />
        </span>
        <span class="inputvalidatorfield">
        <asp:RequiredFieldValidator runat="server" ID="reqMergeProject" ControlToValidate="cmbMergeProject" ErrorMessage="A document configuration is mandatory" Text="*" ValidationGroup="vgMain"></asp:RequiredFieldValidator>

        </span>
    </div>
    <div class="twocolumn">
        <asp:Label runat="server" ID="lblUpload" Text="File to upload*" AssociatedControlID="fuDocTemplate" CssClass="mandatory"></asp:Label>
        <span class="inputs">
            <asp:FileUpload ID="fuDocTemplate" runat="server" /></span>
            <span class="inputicon"></span>
            <span class="inputtooltipfield">
                <asp:Image ID="imgTPFileUpload" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('e9e9626a-2b2a-4a61-8ef8-e721e97f8e80', 'sm', this);" />
            </span>
            <span class="inputvalidatorfield">
                <asp:RequiredFieldValidator runat="server" ID="reqUpload" ErrorMessage="File not specified for upload" Text="*" ControlToValidate="fuDocTemplate" ValidationGroup="vgMain"></asp:RequiredFieldValidator>
        </span>
    </div>
    <div>
        <asp:Literal runat="server" ID="litMessage"></asp:Literal>
    </div>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdOK" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
                onclick="cmdOK_Click" />&nbsp;
        <asp:ImageButton ID="cmdCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" 
                CausesValidation="false" onclick="cmdCancel_Click" />
                <asp:HiddenField runat="server" ID="hiddenDocId" />
    </div>
</div>

    <asp:Panel runat="server" ID="pnlMessageHolder"></asp:Panel>
</asp:content>
<asp:content id="Content2" runat="server" contentplaceholderid="contentmenu">

<tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
                
</asp:content>
<asp:content id="Content3" runat="server" contentplaceholderid="contentleft">

                
</asp:content>
