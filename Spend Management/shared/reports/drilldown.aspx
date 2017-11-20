<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_drilldown" Codebehind="drilldown.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Drilldown Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="position: absolute; height: 400px; width: 100%; overflow: auto;">
        <asp:RadioButtonList ID="optlist" runat="server" Height="420px" meta:resourcekey="optlistResource1">
        </asp:RadioButtonList></div>
        
        <div style="position:absolute; top: 420px;">
                                                      
            <a target="drilldowns" href="javascript:getDrillDownReport();"><img style="border:0px;" src="../images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<a href="javascript:window.close();"><img style="border:none;" src="../images/buttons/cancel_up.gif" /></a>
        </div>
    </form>
</body>
</html>
