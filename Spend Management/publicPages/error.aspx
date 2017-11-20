<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="Spend_Management.publicPages.ErrorPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #errorContainer {
            margin-left:auto;
            margin-right:auto;
            width: 800px;
            text-align: center;
            margin-top: 60px;
        }

        #errorContainer #errorHeading {
	        color: #5e6e66;
            font-size: xx-large;
	        font-family: georgia;
        }

        #errorContainer #errorImage {
	        float: left;
        }

        #errorContainer #errorMessage {
	        color: #5e6e66;
	        font-family: arial;
	        font-size: small;
	        text-align: left;
	        width: 480px;
	        float: right;
	        margin-top: 15px;
        }

        #errorContainer #errorCompanyLogo {
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
    <form id="form1" runat="server">
        <div id="errorContainer">
            <div id="errorHeading"><asp:Literal runat="server" ID="errorTitle"></asp:Literal></div>
	        <div id="errorImage"><asp:Image runat="server" ID="errorImageContent"/></div>
	        <div id="errorCompanyLogo"><asp:Image runat="server" ID="companyLogo" /></div>
	        <div id="errorMessage"><asp:Literal runat="server" ID="errorText"></asp:Literal></div>
        </div>
    </form>
</body>
</html>
