<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
	CodeBehind="admindocmergeprojects.aspx.cs" Inherits="Spend_Management.admindocmergeprojects"
%>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
	<asp:LinkButton runat="server" CssClass="submenuitem" Text="New Configuration" ID="lnkAddProject"
		OnClick="lnkAddProject_Click"></asp:LinkButton>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

	<script language="javascript" type="text/javascript">
		var mdlMergeExec = '<%=mdlMergeExec.ClientID %>';
		var txtemailto = '<%=txtEmailTo.ClientID %>';
		var txtemailcc = '<%=txtEmailCC.ClientID %>';
		var lstdocumentformat = '<%=lstDocumentFormat.ClientID %>';
		var txtsubject = '<%=txtSubject.ClientID %>';
		var txtemailbody = '<%=txtEmailBody.ClientID %>';
		var divConfigurations = '<%=divConfigurations.ClientID %>';
	</script>

	<asp:ScriptManagerProxy runat="server" ID="SMgrProxy">
		<Services>
			<asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcDocumentMerge.asmx" />
		</Services>
		<Scripts>
		    <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.docmerge.js" />
		</Scripts>
	</asp:ScriptManagerProxy>
	<div id="divMergeProjects">
	<div class="formpanel formpanel_padding">
		<div class="sectiontitle">
			Current Document Configurations</div>
		<asp:Panel runat="server" ID="projectPanel">
            <div id="divConfigurations" runat="server"></div>
		</asp:Panel>
	    <div class="formbuttons" style="padding:0px;">
	        <asp:ImageButton runat="server" ID="btnClose" 
                ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" 
                CausesValidation="false" onclick="BtnCloseClick" />
	    </div>
	</div>

	</div>
	<asp:Panel runat="server" CssClass="modalpanel" ID="pnlMergeExec" Style="display: none;">
		<div class="formpanel formpanel_padding">
			<div class="sectiontitle">
				Torch Issue Options</div>
			<div class="twocolumn">
				<asp:Label runat="server" ID="lblDocumentFormat" AssociatedControlID="lstDocumentFormat">Document format</asp:Label>
				<span class="inputs">
					<asp:DropDownList ID="lstDocumentFormat" runat="server">
						<asp:ListItem Text=".DOC (Word 97-2003)" Value="1"></asp:ListItem>
						<asp:ListItem Text=".DOCX (Word 2007)" Value="2"></asp:ListItem>
					</asp:DropDownList>
				</span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
			</div>
			<div class="twocolumn">
				<asp:Label runat="server" ID="lblEmailTo" AssociatedControlID="txtEmailTo" CssClass="mandatory">To*</asp:Label>
				<span class="inputs">
					<asp:TextBox runat="server" ID="txtEmailTo" TabIndex="2" ValidationGroup="valMergeOptions"
						CssClass="fillspan"></asp:TextBox></span> <span class="inputicon">&nbsp;</span>
						<span class="inputtooltipfield"></span>
				<span class="inputvalidatorfield">
					<asp:RegularExpressionValidator runat="server" ID="regEmailTo" ControlToValidate="txtEmailTo"
						ErrorMessage="Invalid email address supplied" ValidationGroup="valMergeOptions"
						ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator>
					<asp:RequiredFieldValidator runat="server" ID="reqEmailTo" ControlToValidate="txtEmailTo"
						Text="*" ErrorMessage="Email address is mandatory" ValidationGroup="valMergeOptions"></asp:RequiredFieldValidator>
				</span>
			</div>
			<div class="twocolumn">
				<asp:Label runat="server" ID="lblEmailCC" AssociatedControlID="txtEmailCC">Cc</asp:Label>
				<span class="inputs">
					<asp:TextBox runat="server" ID="txtEmailCC" CssClass="fillspan" ValidationGroup="valMergeOptions"
						TabIndex="3"></asp:TextBox></span> <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span>
				<span class="inputvalidatorfield">
					<asp:RegularExpressionValidator runat="server" ID="regEmailCC" ControlToValidate="txtEmailCC"
						ValidationGroup="valMergeOptions" Text="*" ErrorMessage="Invalid email address supplied"
						ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></span>
			</div>
			<div class="twocolumn">
				<asp:Label runat="server" ID="lblSubject" CssClass="mandatory" AssociatedControlID="txtSubject">Subject*</asp:Label>
				<span class="inputs">
					<asp:TextBox runat="server" ID="txtSubject" CssClass="fillspan" ValidationGroup="valMergeOptions"
						TabIndex="4"></asp:TextBox></span> <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span>
				<span class="inputvalidatorfield">
					<asp:RequiredFieldValidator runat="server" ID="reqSubject" ControlToValidate="txtSubject"
						ValidationGroup="valMergeOptions" Text="*" ErrorMessage="Email subject line is mandatory"></asp:RequiredFieldValidator></span>
			</div>
			<div class="onecolumn">
				<asp:Label runat="server" ID="lblEmailBody" AssociatedControlID="txtEmailBody">Message body</asp:Label>
				<span class="inputs">
					<asp:TextBox runat="server" ID="txtEmailBody" CssClass="fillspan" TextMode="MultiLine"
						ValidationGroup="valMergeOptions" TabIndex="5"></asp:TextBox></span> <span class="inputicon">
							&nbsp;</span> <span class="inputtooltipfield"></span><span class="inputvalidatorfield">&nbsp;</span>
			</div>
		    <div class="formbuttons">
			    <asp:ImageButton runat="server" ID="btnOKMerge" ImageUrl="~/shared/images/buttons/btn_save.png"
				    OnClientClick="javascript:SEL.DocMerge.DoMerge();" Style="cursor: hand;" AlternateText="Merge"
				    ValidationGroup="valMergeOptions" />&nbsp;
			    <asp:ImageButton runat="server" ID="btnCancelMerge" ImageUrl="~/shared/images/buttons/cancel_up.gif"
				    AlternateText="Cancel" OnClientClick="javascript:SEL.DocMerge.CancelMerge();" Style="cursor: hand;" />
                <div id="ajaxLoaderMaster" style="width:32px;display:none;position:relative;margin-left:8px;">
                    <img src="../images/ajax-loader.gif" alt="" />
                </div>
		    </div>
		</div>
	</asp:Panel>
	<cc1:ModalPopupExtender runat="server" OnOkScript="DoMerge" OnCancelScript="SEL.DocMerge.CancelMerge"
		TargetControlID="lnkLaunchModal" PopupControlID="pnlMergeExec" BackgroundCssClass="modalBackground"
		ID="mdlMergeExec" CancelControlID="btnCancelMerge" OkControlID="btnOKMerge">
	</cc1:ModalPopupExtender>
	<asp:LinkButton runat="server" ID="lnkLaunchModal" Style="display: none;">&nbsp;</asp:LinkButton>


    

    <asp:Panel ID="pnlMessage" runat="server" CssClass="errorModal" style="display:none;">
        <div id="divMessage" runat="server">
        <asp:Label id="lblMessage" runat="server" Text="" />
        </div>
        <div style="padding: 0px 5px 5px 5px;">
            <img src="/shared/images/buttons/btn_close.png" id="btnMessageClose" alt="OK" onclick="" style="cursor: pointer;" />
        </div>
    </asp:Panel>

    <asp:Label runat="server" Text="" id="lblDummy" />
    <cc1:ModalPopupExtender 
        ID="mdlMessage" 
        TargetControlID="lblDummy"
        runat="server" 
        OkControlID="btnMessageClose"
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnlMessage" 
        DropShadow="False">
    </cc1:ModalPopupExtender>

</asp:Content>
