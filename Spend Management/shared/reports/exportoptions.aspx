<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_exportoptions" EnableTheming="true" Codebehind="exportoptions.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export Options</title>
    <link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
    <link rel="stylesheet" type="text/css"  media="screen" href="../css/styles.aspx?style=logon" />
    <link href="/static/syncfusion/css/web/default-theme/ej.web.all.css" rel="stylesheet" />
    <style>
        th.e-headercell {
            color: #282827;
        }
    </style>
    <link id="favLink" runat="server" rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
</head>
<body>
<script type="text/javascript">
    function initialiseGrid(data) {
        var gridData = JSON.parse(data);
        $("#gridFlatFile").ejGrid({
            dataSource: gridData,
            columns: [
                { field: "reportcolumnid", visible:false},
                { field: "columnname", headerText: "Column", isPrimaryKey: true },
                { field: "fieldlength", headerText: "Length", editType: "numericedit" }
            ],
            editSettings: {
                allowEditing: true
            }
        });
    }

    function Save() {
        var fieldLengths = {};
        $(".e-row, .e-alt_row").each(function() {
            var fieldid = $(this).find(".e-rowcell:nth-child(1)").text();
            var length = $(this).find(".e-rowcell:nth-child(3)").text();
            fieldLengths[fieldid.toString()] = length;
        });
        $(".e-editedrow").each(function () {
            var length = $(this).find(".e-numerictextbox").val();
            var fieldid = $(this).find(".e-rowcell:nth-child(1) input").val();
            fieldLengths[fieldid.toString()] = length;
        });
        PageMethods.SaveExportOptions($("#optdelimitertab").is(':checked'), $("#txtdelimiter").val(), $("#cmbfooter").val(), new URL(window.location.href).searchParams.get("reportid"),
            $("#chkshowheadersexcel").is(':checked'), $("#chkshowheaderscsv").is(':checked'), $("#chkshowheadersflat").is(':checked'), $("#chkremovecarriagereturns").is(':checked'), $("#chkencloseinspeechmarks").is(':checked'), JSON.stringify(fieldLengths), CloseWindow
        );
       
    }

    function CloseWindow(result) {
        if (result) {
            window.close();
        }
    }
</script>
<asp:Literal ID="litStyles" runat="server"></asp:Literal>
    <form id="form1" runat="server">

        <cc1:ToolkitScriptManager ID="tsm" runat="server" EnablePageMethods="True">
            <Services>
                <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="/static/js/jQuery/jquery-1.9.1.min.js"/>
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.main.js" />
                <asp:ScriptReference Name="common" />
                <asp:ScriptReference Name="SyncFusion" />
                <asp:ScriptReference Name="tooltips" />
            </Scripts>
        </cc1:ToolkitScriptManager>
        
    <div class=inputpanel>
        <div class="inputpaneltitle">
            <asp:Label ID="lblexcel" runat="server" Text="Excel Options" meta:resourcekey="lblexcelResource1"></asp:Label></div>
        <table>
            <tr><td class="labeltd">
                <asp:Label ID="lblexcelheader" runat="server" Text="Show Header:" meta:resourcekey="lblexcelheaderResource1"></asp:Label></td><td class="inputtd">
                <asp:CheckBox ID="chkshowheadersexcel" runat="server" meta:resourcekey="chkshowheadersexcelResource1" /></td>
                <td><span class="logontooltip"><asp:Image ID="Image4" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6e0c7a28-7ba1-4d31-9856-5b41dff570f8', 'sm', this);" /></span></td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblcsv" runat="server" Text="CSV Options" meta:resourcekey="lblcsvResource1"></asp:Label></div>
        <table>
            <tr><td class="labeltd">
                <asp:Label ID="lblcsvheader" runat="server" Text="Show Header:" meta:resourcekey="lblcsvheaderResource1"></asp:Label></td><td class="inputtd">
                <asp:CheckBox ID="chkshowheaderscsv" runat="server" meta:resourcekey="chkshowheaderscsvResource1" /></td>
                <td><span class="logontooltip"><asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6e0c7a28-7ba1-4d31-9856-5b41dff570f8', 'sm', this);" /></span></td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle"><asp:Label ID="lbldelimiter" runat="server" Text="Delimiter"></asp:Label></div>
        <table>
            <tr><td class="labeltd">Tab Delimited</td><td class="inputtd">
                <asp:radiobutton runat="server" ID="optdelimitertab" 
                    GroupName="delimiter"></asp:radiobutton>
                </td><td></td></tr>
            <tr><td class="labeltd">Other, please specify</td><td class="inputtd">
                <asp:radiobutton runat="server" ID="optdelimiterother" Name="optdelimiter" 
                    GroupName="delimiter"></asp:radiobutton>
                <asp:textbox runat="server" ID="txtdelimiter" style="width: 30px;"></asp:textbox>
            </td><td></td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle"><asp:label runat="server" text="Carriage Returns"></asp:label></div>
        <table>
            <tr><td class="labeltd">Remove carriage returns</td><td class="inputtd"><asp:checkbox runat="server" id="chkremovecarriagereturns"></asp:checkbox></td><td></td></tr>
            
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle"><asp:label runat="server" text="Speech Marks"></asp:label></div>
        <table>
            <tr><td class="labeltd">Enclose in speech marks</td><td class="inputtd"><asp:checkbox runat="server" id="chkencloseinspeechmarks"></asp:checkbox></td><td></td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblflatfile" runat="server" Text="Flat File Options" meta:resourcekey="lblflatfileResource1"></asp:Label></div>
        <table>
            <tr><td class="labeltd">
                <asp:Label ID="lblflatfileheader" runat="server" Text="Show Header:" meta:resourcekey="lblflatfileheaderResource1"></asp:Label></td><td class="inputtd">
                <asp:CheckBox ID="chkshowheadersflat" runat="server" meta:resourcekey="chkshowheadersflatResource1" /></td>
                                <td><span class="logontooltip"><asp:Image ID="Image6" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6e0c7a28-7ba1-4d31-9856-5b41dff570f8', 'sm', this);" /></span></td></tr>
        </table>
  <div id="gridFlatFile"></div>
    </div>
    <div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblfooter" runat="server" Text="Footer Report" meta:resourcekey="lblfooterResource1"></asp:Label>:</div>
    <table>
        <tr><td class="labeltd">
            <asp:Label ID="lblfooterrpt" runat="server" Text="Footer Report" meta:resourcekey="lblfooterrptResource1"></asp:Label></td><td class="inputtd">
                                <asp:DropDownList ID="cmbfooter" runat="server" meta:resourcekey="cmbfooterResource1">
                                </asp:DropDownList></td>
                                <td><span class="logontooltip"><asp:Image ID="Image7" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('52160640-6c8c-457e-9492-3e62cde5f3dc', 'sm', this);" /></span></td></tr></table>
    </div>
    <div class="inputpanel">
        <img ID="cmdok" runat="server" src="/shared/images/buttons/btn_save.png" onclick="javascript:Save(); return false;" /> &nbsp;&nbsp;<a href="javascript:window.close();"><img style="border-width: 0px;" src="../images/buttons/cancel_up.gif" /></a>
    </div>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    </form>
</body>
</html>
