<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
	CodeBehind="adminproducts.aspx.cs" Inherits="Spend_Management.adminproducts" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
	<asp:LinkButton runat="server" ID="lnkAdd" CssClass="submenuitem" OnClientClick="javascript:window.location.href='aeproductdetail.aspx?id=0';">Add</asp:LinkButton>
	<asp:LinkButton runat="server" ID="lnkOpenModal" CssClass="submenuitem" style="display: none;">Open Modal</asp:LinkButton>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<script language="javascript" type="text/javascript">
	var modalpanelID = '<%=sModalPanel %>';
</script>
	<asp:ScriptManagerProxy runat="server" ID="smgrProxy">
		<Scripts>
			<asp:ScriptReference Path="~/shared/javaScript/shared.js" />
			<asp:ScriptReference Path="~/shared/javaScript/products.js" />
		</Scripts>
	</asp:ScriptManagerProxy>
	<div class="formpanel">
		<div class="sectiontitle">
			Product Definitions</div>
		<asp:Literal runat="server" ID="litProductGrid"></asp:Literal>
	</div>
	<asp:Panel ID="pnlProductDetail" runat="server" CssClass="modalpanel" Style="display: none;">
		<div class="formpanel">
			<div class="sectiontitle">
				Product Definition
			</div>
			<asp:PlaceHolder runat="server" ID="phProductDetail"></asp:PlaceHolder>
		</div>
		<div class="formbuttons">
			<asp:ImageButton runat="server" ID="cmdProductUpdate" ImageUrl="~/shared/images/buttons/btn_save.png" />
			<asp:ImageButton ID="cmdProductCancel" ImageUrl="~/shared/images/buttons/cancel_up.gif"
				runat="server" CausesValidation="False" />
		</div>
		<asp:ImageButton runat="server" ID="modalCancel" Style="display: none;" AlternateText="cancel"
			CausesValidation="false" />
	</asp:Panel>
	<%--<cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="lnkOpenModal"
		PopupControlID="pnlProductDetail" BackgroundCssClass="modalBackground" CancelControlID="modalCancel">
	</cc1:ModalPopupExtender>--%>
</asp:Content>
