<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Spend_Management.shared.WebForm1" %>
<%@ Register Src="~/shared/usercontrols/Map.ascx" TagName="Popup" TagPrefix="Map" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"><title></title></head>
<body>
    <script language="javascript" type="text/javascript">
        var CurrentUserInfo = {
            'AccountID': 350,
            'SubAccountID': 1,
            'EmployeeID': 21362,
            'IsDelegate': false,
            'DelegateEmployeeID': 0,
            'Module': { 'ID': 2, 'Name': 'expenses' }
        };
    </script>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="sm">
            <Services>
                <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/AddressesAndTravel.asmx" />
            </Services>
        </asp:ScriptManager>
        <Map:Popup ID="mapPopup" runat="server"></Map:Popup>
    </form>
</body>
</html>
