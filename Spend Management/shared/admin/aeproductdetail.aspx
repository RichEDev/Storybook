<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
	CodeBehind="aeproductdetail.aspx.cs" Inherits="Spend_Management.aeproductdetail" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register TagPrefix="cc1" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
	<asp:ScriptManagerProxy runat="server" ID="smProdDetails">
		<Scripts>
			<asp:ScriptReference Path="~/shared/javaScript/products.js" />
			<asp:ScriptReference Path="~/shared/javaScript/shared.js" />
		</Scripts>
	</asp:ScriptManagerProxy>
	<cc1:TabContainer runat="server" ID="productTabContainer" ActiveTabIndex="0">
		<cc1:TabPanel runat="server" HeaderText="Product Detail" ID="tabPDetail">
			<contenttemplate>
<div class="formpanel">
<div class="sectiontitle">Product Detail</div>
<div class="twocolumn">
<asp:Label runat="server" ID="lblProductName" AssociatedControlID="txtProductName" CssClass="mandatory">Product Name (*)</asp:Label>
<span class="inputs"><asp:TextBox runat="server" ValidationGroup="pdetail" TabIndex="1" ID="txtProductName" CssClass="fillspan" MaxLength="70"></asp:TextBox></span>
<span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator"><asp:RequiredFieldValidator runat="server" ID="reqProductName" ControlToValidate="txtProductName" Text="*" ErrorMessage="Product Name field is mandatory" ValidationGroup="pdetail"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender runat="server" ID="reqexProductName" TargetControlID="reqProductName"></cc1:ValidatorCalloutExtender>
</span></div>
<div class="onecolumnsmall">
<asp:Label runat="server" ID="lblProductCategory" AssociatedControlID="lstProductCategory">Product Category</asp:Label>
<span class="inputs"><asp:DropDownList runat="server" ValidationGroup="pdetail" ID="lstProductCategory" CssClass="fillspan" TabIndex="2"></asp:DropDownList></span>
<span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator">&nbsp;</span>
</div>
<div class="onecolumn">
<asp:Label runat="server" ID="lblDescription" AssociatedControlID="txtDescription">Product Description</asp:Label>
<span class="inputs"><asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="fillspan" ValidationGroup="pdetail" TabIndex="3"></asp:TextBox></span>
<span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator">&nbsp;</span>
</div>
<div class="twocolumn">
<asp:Label runat="server" ID="lblNotes" AssociatedControlID="txtNotes">Notes</asp:Label> 
<span class="inputs"><asp:TextBox runat="server" ID="txtNotes" CssClass="fillspan" ReadOnly="true">0 Notes</asp:TextBox></span>
<span class="inputicon"><asp:ImageButton runat="server" ID="imgNotes" ImageUrl="~/shared/images/icons/16/Plain/folder.png" /></span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator">&nbsp;</span>

<asp:Label runat="server" ID="lblProductCode" AssociatedControlID="txtProductCode">Product Code</asp:Label>
<span class="inputs"><asp:TextBox runat="server" ID="txtProductCode" CssClass="fillspan" TabIndex="4" ValidationGroup="pdetail" MaxLength="15"></asp:TextBox></span>
<span class="inputicon">&nbsp;</span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator">&nbsp;</span>

</div>

</div>
<div class="formpanel">
<div class="sectiontitle">Licence Information</div>
<div class="twocolumn">
<asp:Label runat="server" ID="lblNoLicences" AssociatedControlID="txtNoLicences">No. of Licences</asp:Label>
<span class="inputs"><asp:TextBox runat="server" ID="txtNoLicences" CssClass="fillspan" ReadOnly="true"></asp:TextBox></span>
<span class="inputicon"><asp:Image runat="server" ID="imgLicences" ImageUrl="~/shared/images/icons/16/Plain/funnel_add.png" /></span>
<span class="inputtooltipfield">&nbsp;</span>
<span class="inputvalidator">&nbsp;</span>
</div>
</div>
</contenttemplate>
		</cc1:TabPanel>
	</cc1:TabContainer>

	<div class="formpanel formbuttons">
		<asp:Image runat="server" ID="cmdSave" onclick="javascript:validateAndSubmit();"
			AlternateText="Save" ImageUrl="~/shared/images/buttons/btn_save.png" />
		<asp:ImageButton runat="server" ID="cmdExecSave" ImageUrl="~/shared/images/buttons/btn_save.png"
			AlternateText="Save" OnClick="cmdExecSave_Click" Style="display: none;" />&nbsp;
		<asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/shared/images/buttons/btn_close.png"
			AlternateText="Close" CausesValidation="false" OnClientClick="javascript:window.location.href='adminproducts.aspx';" />
	</div>
</asp:Content>
