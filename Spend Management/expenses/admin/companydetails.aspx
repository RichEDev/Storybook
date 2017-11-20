<%@ Page language="c#" Inherits="expenses.admin.companydetails" MasterPageFile="~/masters/smForm.master" Codebehind="companydetails.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="contentmenu">
    <a href="javascript:SEL.CompanyDetails.Year.New();"  id="lnkGeneral" class="submenuitem"><asp:Label id="Label4" runat="server">New Financial Year</asp:Label></a> 
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<style>
.ui-datepicker-year{
    display:none;
}
</style>

    <script language="javascript" type="text/javascript">
        (function (r)
        {
            r.FinancialYearModal = '<%= modFinancialYear.ClientID %>';
            r.FinancialYearDescription = '<%= txtYearDescription.ClientID %>';
            r.FinancialYearStart = '<%= txtYearStart.ClientID %>';
            r.FinancialYearEnd = '<%= txtYearEnd.ClientID %>';
            r.FinancialYearActive = '<%= chkActive.ClientID %>';
            r.FinancialYearPrimary = '<%= chkPrimary.ClientID %>';
        }
        (SEL.CompanyDetails.DOMIDs));
        $(document).ready(function () {
            SEL.CompanyDetails.SetupDateFields();
        });
</script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/sel.companyDetails.js" />
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
        </Scripts>
    </asp:ScriptManagerProxy>
	<div class="valdiv">
		<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" Font-Size="Small" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
	<div class="formpanel formpanel_padding">
		<div class="sectiontitle">
			<asp:Label id="Label1" runat="server" CssClass="sectiontitle">Name and Address</asp:Label></div>
            <div class="onecolumnsmall">
		        <asp:Label id="lblcompanyname" runat="server" meta:resourcekey="lblcompanynameResource1" AssociatedControlID="txtcompanyname">Company Name</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtcompanyname" runat="server" MaxLength="50" meta:resourcekey="txtcompanynameResource1"></asp:TextBox></span>
                </div>
        <div class="twocolumn">
			    <asp:Label id="lbladdress1" runat="server" meta:resourcekey="lbladdress1Resource1" AssociatedControlID="txtaddress1">Address 1</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtaddress1" runat="server" MaxLength="50" meta:resourcekey="txtaddress1Resource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span><asp:Label id="lbladdress2" runat="server" meta:resourcekey="lbladdress2Resource1" AssociatedControlID="txtaddress2">Address 2</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtaddress2" runat="server" MaxLength="50" meta:resourcekey="txtaddress2Resource1"></asp:TextBox></span>
			</div>
        <div class="twocolumn">
				
					<asp:Label id="lblcity" runat="server" meta:resourcekey="lblcityResource1" AssociatedControlID="txtcity">City</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtcity" runat="server" MaxLength="50" meta:resourcekey="txtcityResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span><asp:Label id="lblcounty" runat="server" meta:resourcekey="lblcountyResource1" AssociatedControlID="txtcounty">County</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtcounty" runat="server" MaxLength="50" meta:resourcekey="txtcountyResource1"></asp:TextBox></span>
			</div>
        <div class="twocolumn">
					<asp:Label id="lblpostcode" runat="server" meta:resourcekey="lblpostcodeResource1" AssociatedControlID="txtpostcode">Postcode</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtpostcode" runat="server" MaxLength="50" meta:resourcekey="txtpostcodeResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span>
			</div>
	
		<div class="sectiontitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Contact Details</asp:Label></div>
        <div class="twocolumn">
					<asp:Label id="lbltelno" runat="server" meta:resourcekey="lbltelnoResource1" AssociatedControlID="txttelno">Tel No</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txttelno" runat="server" MaxLength="50" meta:resourcekey="txttelnoResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span><asp:Label id="lblfaxno" runat="server" meta:resourcekey="lblfaxnoResource1" AssociatedControlID="txtfaxno">Fax No</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtfaxno" runat="server" MaxLength="50" meta:resourcekey="txtfaxnoResource1"></asp:TextBox></span>
			</div>
        <div class="onecolumnsmall">
					<asp:Label id="lblemail" runat="server" meta:resourcekey="lblemailResource1" AssociatedControlID="txtemail">E-Mail Address</asp:Label>
				<span class="inputs">
					<asp:TextBox id="txtemail" runat="server" MaxLength="50" meta:resourcekey="txtemailResource1"></asp:TextBox></span>
			</div>
		<div class="sectiontitle">
			<asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Company Bank Details</asp:Label></div>
		<div class="onecolumnsmall">
            <asp:Label id="lblreference" runat="server" meta:resourcekey="lblreferenceResource1" AssociatedControlID="txtbankref">Bank Reference No</asp:Label>
			<span class="inputs">
					<asp:TextBox id="txtbankref" runat="server" MaxLength="50" ></asp:TextBox>
			</span>
            </div>
        <div class="twocolumn">
			<asp:Label id="lblaccountnumber" runat="server" meta:resourcekey="lblaccountnumberResource1" AssociatedControlID="txtaccountno">Account Number</asp:Label>
	        <span class="inputs">
					<asp:TextBox id="txtaccountno" runat="server" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span><asp:Label id="lblaccounttype" runat="server" meta:resourcekey="lblaccounttypeResource1" AssociatedControlID="txtaccounttype">Account Type</asp:Label>
			<span class="inputs">
					<asp:TextBox id="txtaccounttype" runat="server" MaxLength="50" ></asp:TextBox></span>
            </div>
        <div class="twocolumn">
					
            </div>
        <div class="twocolumn">
					<asp:Label id="lblsortcode" runat="server" meta:resourcekey="lblsortcodeResource1" AssociatedControlID="txtsortcode">Sort Code</asp:Label>
			<span class="inputs">
					<asp:TextBox id="txtsortcode" runat="server" MaxLength="50" ></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span>
            </div>
		
	
	    <div class="sectiontitle">
            <asp:Label ID="lblfinancialyear" runat="server" Text="Financial Year" meta:resourcekey="lblfinancialyearResource1"></asp:Label></div>
        <asp:Literal ID="litFinancialYearGrid" runat="server"></asp:Literal>
	
	    <div class="sectiontitle">Other Information</div>

        <div class="twocolumn">
	        <asp:Label ID="lblcompanynumber" AssociatedControlID="txtcompanynumber" runat="server">Company Number</asp:Label><span class="inputs">
                <asp:TextBox ID="txtcompanynumber" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"></span>
	    </div>
	</div>
        
	<div class="formpanel formpanel_padding">
        <helpers:CSSButton ID="btnok" UseSubmitBehavior="true" Text="save" runat="server" />
        <helpers:CSSButton ID="btncancel" UseSubmitBehavior="true" Text="cancel" runat="server" />
	</div>
    
    <asp:Panel ID="pnlFinancialYear" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle" id="divApprovalMatrixLevelTitle">Financial Year</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblYearDescription" runat="server" Text="Description*" AssociatedControlID="txtYearDescription"></asp:Label><span class="inputs"><asp:TextBox ID="txtYearDescription" runat="server" CssClass="fillspan" maxlength="15" ></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqYearDescription" Text="*" ValidationGroup="vgYear" ControlToValidate="txtYearDescription" ErrorMessage="Please enter a Description." Display="Dynamic" ></asp:RequiredFieldValidator></span>
            </div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblYearStart" runat="server" Text="Start Date*" AssociatedControlID="txtYearStart"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtYearStart" MaxLength="5" CssClass="dateField"/></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvYearStart" runat="server" ErrorMessage="Please enter a Start Date." Text="*" ControlToValidate="txtYearStart" ValidationGroup="vgYear" Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblYearEnd" runat="server" Text="End Date*" AssociatedControlID="txtYearEnd"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtYearEnd" MaxLength="5" CssClass="dateField"/></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter an End Date." Text="*" ControlToValidate="txtYearEnd" ValidationGroup="vgYear" Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblActive" runat="server" Text="Active" AssociatedControlID="chkActive"></asp:Label><span class="inputs"><asp:CheckBox runat="server" id="chkActive" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblPrimary" runat="server" Text="Primary" AssociatedControlID="chkPrimary"></asp:Label><span class="inputs"><asp:CheckBox runat="server" id="chkPrimary" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveLevel" runat="server" Text="save" onclientclick="SEL.CompanyDetails.Year.Save();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelLevel" runat="server" Text="cancel" onclientclick="SEL.CompanyDetails.Year.Cancel();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <asp:LinkButton ID="lnkFinancialYear" runat="server" style="display:none;">LinkButton</asp:LinkButton>

	<cc1:ModalPopupExtender ID="modFinancialYear" runat="server" TargetControlID="lnkFinancialYear" PopupControlID="pnlFinancialYear" BackgroundCssClass="modalBackground" CancelControlID="btnCancelLevel">
	</cc1:ModalPopupExtender>


    </asp:Content>


