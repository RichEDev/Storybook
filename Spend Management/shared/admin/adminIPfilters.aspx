<%@ Page language="c#" Inherits="Spend_Management.adminIPfilters" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminIPfilters.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">        
    <asp:Panel ID="pnlAddNewIPFilter" runat="server">
	    <a href="javascript:SEL.IPFilters.ShowIPFiltersModal();" class="submenuitem">New IP Filter</a>
    </asp:Panel>				
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	

    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Name="tooltips" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.ipfilters.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcIPFilters.asmx" InlineScript="false" />
            <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>

    <link rel="stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/shared/css/layout.css") %>" />
    <link rel="stylesheet" type="text/css"  media="screen" href="<% = ResolveUrl("~/shared/css/styles.aspx") %>" />   

	<div class="formpanel formpanel_padding">
	    
        <div ID="divComments" runat="server" class="comment" style="padding: 10px" ><asp:Literal ID="litHelpGuide" runat="server"></asp:Literal></div><br />

            <asp:Panel runat="server" ID="pnlGrid">
                <asp:Literal ID="litgrid" runat="server"></asp:Literal>
            </asp:Panel>

        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>        
        
    </div>	  

    <cc1:ModalPopupExtender ID="modIPFilter" runat="server" BackgroundCssClass="modalBackground" OkControlID="cmdSaveIPFilter"
    TargetControlID="cmdSaveIPFilter" CancelControlID="cmdCancelIPFilter" OnCancelScript="" OnOkScript="" PopupControlID="panel_Popup" /> 

    <asp:Panel ID="panel_Popup" runat="server" CssClass="modalpanel formpanel" style="display: none"> 
        
        <div class="sectiontitle">IP Filter Details</div>
            
        <div class="twocolumn">            
            <asp:Label CssClass="mandatory" id="lblIpAddress" runat="server" meta:resourcekey="lblIPFilterResource1" Text="IP Address*" AssociatedControlID="txtipaddress"></asp:Label>
            <span class="inputs"><asp:TextBox CssClass="fillspan" id="txtipaddress" runat="server" MaxLength="50" meta:resourcekey="txtIPFilterResource1"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield"><asp:Image ID="imgTooltipIPAddress" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4ae8f309-f1d9-4f22-bbb1-89ca97023690', 'sm', this);" /></span>
            <span class="inputvalidatorfield">
            <asp:RequiredFieldValidator id="reqipaddress" runat="server" ErrorMessage="Please enter an IP Address in the box provided" ControlToValidate="txtipaddress" meta:resourcekey="reqipaddressResource1">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="regexpIPAddress" runat="server" ErrorMessage="The IP Address you have entered is not valid" ControlToValidate="txtipaddress" ValidationExpression="^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$">*</asp:RegularExpressionValidator>
            </span>
            <asp:Label ID="lblActive" runat="server" AssociatedControlID="chkactive">Active</asp:Label>
            <span class="inputs"><asp:CheckBox ID="chkactive" runat="server"></asp:CheckBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield"><asp:Image ID="imgTooltipIPActive" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('b621c1fa-7aae-492a-a6dc-fc06969189da', 'sm', this);" /></span>
        </div>
           
        <div class="onecolumn">
            <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" Text="" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label>
            <span class="inputs"><asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span>
        </div>                 

        <div class="formbuttons">
                       
            <asp:ImageButton ID="cmdSaveIPFilter" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClientClick="SEL.IPFilters.SaveIPFilter();"></asp:ImageButton>&nbsp;&nbsp;
            <asp:ImageButton ID="cmdCancelIPFilter" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdCancelAttachmentTypeResource1"></asp:ImageButton>
        </div>

        <asp:Button ID="btn_Ok" runat="server" Text="OK" CssClass="buttons" Visible="false" />
        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CssClass="buttons" Visible="false" />

    </asp:Panel>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    </asp:UpdatePanel>    

    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>

</asp:Content>


