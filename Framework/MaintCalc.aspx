<%@ Page Language="vb" AutoEventWireup="false" Inherits="Framework2006.MaintCalc" CodeFile="MaintCalc.aspx.vb" %>

<html>
<head runat="server">
    <title>Next Period Cost Calculation</title>
</head>
<body>
    <form id="MaintCalcForm" method="post" runat="server">
        <asp:Literal runat="server" ID="litStyles"></asp:Literal>
        <div class="inputpanel">
            <asp:Literal runat="server" ID="litScript"></asp:Literal>
            <asp:Literal runat="server" ID="litCalculation"></asp:Literal>
        </div>
        <div class="inputpanel">
            <a onclick="window.close();">
                <img src="./buttons/page_close.gif" /></a></div>
    </form>
</body>
</html>
