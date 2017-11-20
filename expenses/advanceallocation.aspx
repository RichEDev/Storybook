<%@ Page language="c#" Inherits="expenses.advanceallocation" Codebehind="~/advanceallocation.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head runat="server">
    <title>Advance Allocation</title>
</head>
<body>

<form id="form1" runat="server">

<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
    <table>
        <tr><td class="labeltd">Advance Name:</td><td class="inputtd">
            <asp:Label ID="lblname" runat="server" Text="Label" meta:resourcekey="lblnameResource1"></asp:Label></td><td class="labeltd">Advance Total:</td><td class="inputtd">
                <asp:Label ID="lbltotal" runat="server" Text="Label" meta:resourcekey="lbltotalResource1"></asp:Label></td></tr>
        <tr><td class="labeltd">Allocated Amount:</td><td class="inputtd">
            <asp:Label ID="lblallocated" runat="server" Text="Label" meta:resourcekey="lblallocatedResource1"></asp:Label></td><td class="labeltd">Available Amount:</td><td class="inputtd">
                <asp:Label ID="lblavailable" runat="server" Text="Label" meta:resourcekey="lblavailableResource1"></asp:Label></td></tr>
    </table>
</div>
<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblallocateditems" runat="server" Text="Allocated Items" meta:resourcekey="lblallocateditemsResource1"></asp:Label></div>

    <igtbl:UltraWebGrid ID="gridexpenses" runat="server" SkinID="gridskin" OnInitializeLayout="gridexpenses_InitializeLayout" OnInitializeRow="gridexpenses_InitializeRow" meta:resourcekey="gridexpensesResource1">
       
    </igtbl:UltraWebGrid> </div>      </form> 
                </body>
</html>
