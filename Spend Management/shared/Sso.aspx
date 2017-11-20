<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sso.aspx.cs" Inherits="Spend_Management.shared.Sso" EnableTheming="false" Theme="" StylesheetTheme=""%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%=this.ModuleName%> logon</title>
	<style type="text/css">
		#errorContainer 
		{
		 margin-left:auto;
		 margin-right:auto;
		 width: 800px;
		 text-align: center;
		 margin-top: 10%;
		}

		#errorContainer #errorImage {
			float: left;
		}

		#errorContainer #errorMessage{
			color: #5e6e66;
			font-family: arial;
			font-size: small;
			text-align: left;
			width: 480px;
			float: right;
			margin-top: 15px;
		}
		#errorContainer #errorCompanyLogo
		{
			float: right;
			margin-top: 80px;
		}

		#errorContainer #errorHighlight {
			font-family: georgia;
			color: #ea0f6b;
			font-size: x-large;
		}
 
	</style>
</head>
<body>
	<div id="errorContainer">
		<div id="errorImage"><img id="errorImageContent" src="/static/images/errors/logon.jpg" style="border-width:0px;" alt="<%=this.ModuleName%> logon" title="<%=this.ModuleName%> logon" /></div>
		<div id="errorCompanyLogo"><img id="CompanyLogo" src="/static/images/branding/company/selenity.png" style="border-width:0px;" alt="Selenity" title="Selenity" /></div>
	    <div id="errorMessage"><p><span id="errorHighlight"><%=this.ModuleName%> logon</span><br /><asp:Literal runat="server" ID="litMessage"></asp:Literal><br /><asp:Literal runat="server" ID="litDetail"></asp:Literal></p></div>
	</div>
</body>

</html>
