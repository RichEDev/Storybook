<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aeEntityPopup.aspx.cs" Inherits="Spend_Management.shared.aeEntityPopup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
     <link id="favLink" runat="server" rel="shortcut icon"  type="image/x-icon" />
    <link id="jQueryCss" runat="server" type="text/css" href="/shared/css/jquery-ui-1.9.2.custom.css" rel="Stylesheet" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal runat="server" ID="litStyles" ClientIDMode="Static"></asp:Literal>
        <div class="sm_panel">
                <link rel="stylesheet" type="text/css" media="screen" href="<% = ResolveUrl("~/shared/css/styles.aspx") %>" />
                <cc1:ToolkitScriptManager ID="scriptman" runat="server" EnablePageMethods="True">
                    <Scripts>
                        <asp:ScriptReference Path="~/shared/javascript/sel.main.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/customEntities.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.grid.js" />
                        <asp:ScriptReference Path="~/shared/javascript/sel.common.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/minify/jsonParse.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.grid.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.autoComplete.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/sel.tooltips.js" />
                        <asp:ScriptReference Path="~/shared/javaScript/Attachments.js"/>
                    </Scripts>
                    <Services>
                        <asp:ServiceReference Path="~/shared/webServices/svcGrid.asmx" InlineScript="false" />
                        <asp:ServiceReference Path="~/shared/webServices/svcCustomEntities.asmx" />
                    </Services>
                </cc1:ToolkitScriptManager>
                <script language="javascript" type="text/javascript">
                    var hiddenCETabCntlID = '<%=hiddenCETab.ClientID %>';
                    var hiddenCETabIDCntlID = '<%=hiddenCETabID.ClientID %>';
                </script>
                <br />
                <asp:HiddenField runat="server" ID="hiddenCETab" />
                <asp:HiddenField runat="server" ID="hiddenCETabID" />

                <div class="sectiontitle" runat="server" id="divViewName"></div>

                <br />
                <div class="formbuttons">
                    <asp:LinkButton ID="lnkNew" runat="server"></asp:LinkButton>
                    <asp:Panel ID="panel" runat="server"></asp:Panel>
                    <asp:Literal ID="litgrid" runat="server"></asp:Literal>
                    <asp:Literal runat="server" ID="litButton"></asp:Literal>
                </div>
        </div>
    </form>
</body>
        <script language="javascript" type="text/javascript">
            $(document).ready(function ()
            {
                var link = $($('div[id*=lnkPopupWindow]'));
                if (link.length > 0) {
                    $.each(link, function () {
                        link.hide();
                    });
                }
            });
    </script>
</html>
