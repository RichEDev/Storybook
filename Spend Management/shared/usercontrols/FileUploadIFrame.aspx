<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUploadIFrame.aspx.cs" Inherits="Spend_Management.FileUploadIFrame" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="fileHead" runat="server">
	<title>Upload File</title>
    <link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
</head>
<body>
    <asp:Literal ID="litStyles" runat="server"></asp:Literal>
	<form id="fileUpload" runat="server">
        <cc1:ToolkitScriptManager ID="ScriptManager1" runat="server">
	    <Services>
	    <asp:ServiceReference Path="~/shared/webServices/svcAttachments.asmx" />
	    </Services>
	    <Scripts>
	    <asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />

	    </Scripts>
	    </cc1:ToolkitScriptManager >
        
	<script type="text/javascript">
	    //<![CDATA[
	    var modalCancelID;
	    var lblStatusID = 'lblStatus';

	    function doClose() {
	        
	        var attTitleCntl = document.getElementById('<%=attachTitleID %>');
	        if (attTitleCntl != null) {
	            attTitleCntl.value = '';
	        }
	        var attDescCntl = document.getElementById('<%=attachDescID %>');
	        if (attDescCntl != null) {
	            attDescCntl.value = '';
	        }
	        var attlblStatus = document.getElementById('<%=lblStatus.ClientID %>');
	        if (attlblStatus != null) {
	            attlblStatus.innerText = '';
	        }
	        var attreqFile = document.getElementById('<%=reqFile.ClientID %>');
	        if (attreqFile != null) {
	            attreqFile.innerText = '';
	        }
	        var attreqTitle = document.getElementById('reqTitle');
	        if (attreqTitle != null) {
	            attreqTitle.innerText = '';
	        }
	        var attfilePath = document.getElementById('<%=fileUploadBox.ClientID %>');
	        if (attfilePath != null) {
	            attfilePath.value = '';
	            var cloneFile = attfilePath.cloneNode(false);
	            attfilePath.parentNode.replaceChild(cloneFile, attfilePath);
	        }
	        var mdlcancelbtn = window.parent.document.getElementById(modalCancelID);
	        if (mdlcancelbtn != null) {
	            mdlcancelbtn.click();
	        } else {
	            var modalCloseBtn = window.parent.document.getElementById("modalBtnCancel");
	            modalCloseBtn.click();
	        }
	        return;
	    }


	    //]]>
	</script>
	
	<div class="formpanel formpanel_padding">
		<asp:Panel ID="pnlAttach" runat="server" style="overflow: auto;"></asp:Panel>
		<div class="onecolumnsmall"><asp:Label ID="lblAttachment" runat="server" CssClass="mandatory" meta:resourcekey="lblAttachmentResource1" Text="Attachment*" AssociatedControlID="fileUploadBox"></asp:Label><span class="inputs"><asp:FileUpload ID="fileUploadBox" runat="server" style="width:300px;" /></span> <span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqFile" runat="server" ErrorMessage="Please select an Attachment to upload." ControlToValidate="fileUploadBox" meta:resourcekey="reqFileResource1" ValidationGroup="validUpload">*</asp:RequiredFieldValidator></span></div>
		<div class=" formpanel_padding"><asp:ImageButton runat="server" ClientIDMode="Static" ID="btnUpload"  ImageUrl="~/shared/images/buttons/upload_up.gif" AlternateText="Upload" onclick="btnUpload_Click" />&nbsp;<img src="../images/buttons/cancel_up.gif" alt="Cancel" onclick="javascript:doClose();" id="cmdCancelAttach" runat="server" /></div>
		<asp:Label ID="lblStatus" runat="server">&nbsp;</asp:Label>
		<asp:HiddenField ID="GenIDVal" ClientIDMode="Static" runat="server" />
		<asp:HiddenField ID="DocType" runat ="server" />
        
	</div>
	</form>
</body>
</html>
