<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="migratecardocuments.aspx.cs" Inherits="expenses.migratecardocuments" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtaccountid" runat="server"></asp:TextBox>
    </div>
    <asp:Button ID="cmdmigrate" runat="server" Text="Button" 
        onclick="cmdmigrate_Click" />
    </form>
</body>
</html>
