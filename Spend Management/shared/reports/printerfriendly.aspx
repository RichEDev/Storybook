<%@ Page Language="C#" AutoEventWireup="true" Codebehind="printerfriendly.aspx.cs" Inherits="Spend_Management.printerfriendly" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="igcalc" Namespace="Infragistics.WebUI.UltraWebCalcManager" Assembly="Infragistics4.WebUI.UltraWebCalcManager.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <style>
        table tbody tr th {
            color: black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <igcalc:UltraWebCalcManager ID="calcman" runat='server'>
        </igcalc:UltraWebCalcManager>
    <div align=center>
        <asp:Label ID="lbltitle" runat="server" Text="Label" meta:resourcekey="lbltitleResource1"></asp:Label>
    </div>
    <div>
        <asp:Literal ID="litreport" runat="server" meta:resourcekey="litreportResource1"></asp:Literal></div>
    </form>
</body>
</html>
