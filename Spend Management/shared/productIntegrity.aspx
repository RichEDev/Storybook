<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="productIntegrity.aspx.cs" Inherits="Spend_Management.shared.ProductIntegrity" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color: #272727; color: #FFFFFF;">
    <style type="text/css">
        h3 { color: #ffffff; }
    </style>
    <form id="form1" runat="server">
    <div style="padding: 20px">
    <asp:Literal runat="server" ID="serverInformation"></asp:Literal>
    <asp:Literal runat="server" ID="sqlDependencies"></asp:Literal>
    <asp:Literal runat="server" ID="assembliesSummary"></asp:Literal>
    </div>
    </form>
</body>
</html>
