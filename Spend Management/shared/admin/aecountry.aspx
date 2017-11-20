<%@ Page language="c#" Inherits="Spend_Management.shared.admin.aecountry" MasterPageFile="~/masters/smForm.master" Codebehind="aecountry.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
                <a class="submenuitem" href="javascript:showSubcatModal();" id="lnkNewVatRate">New VAT Rate</a>
				

</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <script type="text/javascript" language="javascript">
        var modsubcatid = '<%=modAddSubcat.ClientID %>';
        var cmbcountryid = '<%=cmbcountry.ClientID %>';
        var txtcountrycodeid = '<%=txtCountryCode.ClientID %>';
        var txtAlpha3Country = '<% = txtAlpha3.ClientID %>';
        var txtNumeric3 = '<% = txtNumeric3.ClientID %>';
        var chkPostcodeAnywhereEnabled = '<% = chkPostcodeAnywhereEnabled.ClientID %>';
        var cmbsubcatid = '<%=cmbSubcat.ClientID %>';
        var txtvatrateid = '<%=txtVatRate.ClientID %>';
        var txtclaimablepercentageid = '<%=txtClaimablePercentage.ClientID %>';
        var ratesGrid = '<%=pnlrates.ClientID %>';
    </script>
    <script src="../javascript/countries.js" type="text/javascript" language="javascript"></script>
    <div class="formpanel formpanel_padding">

		<div class="sectiontitle">General Details</div>
        <div class="onecolumnsmall">
			<asp:Label CssClass="mandatory" id="lblcountry" runat="server" meta:resourcekey="lblcountryResource1" AssociatedControlID="cmbCountry">Country*</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbcountry" runat="server" meta:resourcekey="cmbcountryResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
		</div>
        <div class="twocolumn">
            <asp:Label CssClass="mandatory" ID="lblCountryCode" runat="server" AssociatedControlID="txtCountryCode">Alpha 2*</asp:Label><span class="inputs"><asp:TextBox ID="txtCountryCode" runat="server" CssClass="fillspan"  Enabled="False" meta:resourcekey="txtcountrycodeResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label CssClass="mandatory" ID="lblAlpha3" runat="server" AssociatedControlID="txtCountryCode">Alpha 3*</asp:Label><span class="inputs"><asp:TextBox ID="txtAlpha3" runat="server" CssClass="fillspan"  Enabled="False" meta:resourcekey="txtcountrycodeResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
		</div>

        <div class="twocolumn">
            <asp:Label CssClass="mandatory" ID="lblNumeric3" runat="server" AssociatedControlID="txtCountryCode">Numeric 3*</asp:Label><span class="inputs"><asp:TextBox ID="txtNumeric3" runat="server" CssClass="fillspan"  Enabled="False" meta:resourcekey="txtcountrycodeResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label ID="lblPostcodeAnywhereEnabled" runat="server" AssociatedControlID="chkPostcodeAnywhereEnabled">Postcode Anywhere Support</asp:Label><span class="inputs"><asp:CheckBox ID="chkPostcodeAnywhereEnabled" runat="server" CssClass="fillspan" Enabled="False" meta:resourcekey="txtcountrycodeResource1"></asp:CheckBox></span>
		</div>
			
        <asp:Panel ID="pnlrates" runat="server">
        <div class="sectiontitle">VAT Rates</div>
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        </asp:Panel>
			<div class="formbuttons">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="../images/buttons/btn_save.png" meta:resourcekey="cmdokResource1" ValidationGroup="vgMain"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>
	</div>
	
	<asp:Panel ID="pnlAddSubcat" runat="server" CssClass="modalpanel">
	    <div class="formpanel">
            <div class="sectiontitle">Expense Item VAT Rate Details</div>
            <div class="onecolumnsmall"><asp:Label CssClass="mandatory" id="lblSubcat" runat="server" meta:resourcekey="lblSubcatResource1" AssociatedControlID="cmbSubcat">Expense Item*</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbSubcat" runat="server" meta:resourcekey="cmbSubcatResource1"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" id="lblVatRate" runat="server" meta:resourcekey="lblVatRateResource1" AssociatedControlID="txtVatRate">VAT Rate*</asp:Label><span class="inputs"><asp:TextBox ID="txtVatRate" runat="server" CssClass="fillspan" meta:resourcekey="txtVatRateResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqVatRate" runat="server" ErrorMessage="Please enter a VAT rate in the box provided" Text="*" ControlToValidate="txtVatRate" ValidationGroup="vgSubcat"></asp:RequiredFieldValidator><asp:CompareValidator ID="compVatRate" runat="server" ErrorMessage="Please enter a number for the VAT Rate" Text="*" ControlToValidate="txtVatRate" ValidationGroup="vgSubcat" Type="Double" Operator="DataTypeCheck"></asp:CompareValidator></span><asp:Label CssClass="mandatory" id="lblClaimablePercentage" runat="server" meta:resourcekey="lblClaimablePercentageResource1" AssociatedControlID="txtClaimablePercentage">Claimable Percentage*</asp:Label><span class="inputs"><asp:TextBox ID="txtClaimablePercentage" runat="server" CssClass="fillspan" meta:resourcekey="txtClaimablePercentageResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqClaimablePercentage" runat="server" ErrorMessage="Please enter the Claimable Percentage in the box provided" Text="*" ControlToValidate="txtClaimablePercentage" ValidationGroup="vgSubcat"></asp:RequiredFieldValidator><asp:CompareValidator ID="compClaimablePercentage" runat="server" ErrorMessage="Please enter a number for the Claimable Percentage" Text="*" ControlToValidate="txtClaimablePercentage" ValidationGroup="vgSubcat" Type="Double" Operator="DataTypeCheck"></asp:CompareValidator></span>
            </div>
            <div class="formbuttons"><a href="javascript:saveCountrySubcat();"><img alt="Save" src="../images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<asp:ImageButton id="cmdCancelSubcat" runat="server" ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdCancelSubcatResource1"></asp:ImageButton>
        </div>
        
	    </div>  
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modAddSubcat" runat="server" TargetControlID="lnklistitem" PopupControlID="pnlAddSubcat" BackgroundCssClass="modalBackground" CancelControlID="cmdCancelSubcat"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnklistitem" runat="server" style="display:none;">LinkButton</asp:LinkButton></asp:Content>