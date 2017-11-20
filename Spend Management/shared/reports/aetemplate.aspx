<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_aetemplate" Codebehind="aetemplate.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Templates</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1" />
    <div>
        <table>
            <tr><td class="labeltd">
                <asp:Label ID="lbltemplatename" runat="server" Text="Template Name:" meta:resourcekey="lbltemplatenameResource1"></asp:Label></td><td class="inputtd" style="width: 155px">
                <asp:TextBox ID="txtname" runat="server" meta:resourcekey="txtnameResource1"></asp:TextBox></td><td>
                    <asp:RequiredFieldValidator ID="reqname" runat="server" ErrorMessage="Please enter a name for this template in the box provided" ControlToValidate="txtname" meta:resourcekey="reqnameResource1" Text="*"></asp:RequiredFieldValidator></td></tr>
            <tr><td class="labeltd">
                <asp:Label ID="lbldescription" runat="server" Text="Description:" meta:resourcekey="lbldescriptionResource1"></asp:Label></td><td class="inputtd" style="width: 155px">
                <asp:TextBox ID="txtdescription" runat="server" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></td></tr>
        </table>
    </div>
    <div>
    <a target="main" href="javascript:getTemplateName();"><img src="/shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<a href="javascript:window.close();"><img src="../images/buttons/cancel_up.gif" /></a>
    </div>
    </form>
</body>
</html>
