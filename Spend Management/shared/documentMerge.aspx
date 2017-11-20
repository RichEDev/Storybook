<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="documentMerge.aspx.cs" Inherits="Spend_Management.documentMerge" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Merging Document...</title>
    <link id="jQueryCss" runat="server" type="text/css" href="/shared/css/jquery-ui-1.9.2.custom.css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <cc1:ToolkitScriptManager ID="toolkitScrMgr" runat="server">
            <Services>
                <asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcDocumentMerge.asmx" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.main.js" />
                <asp:ScriptReference Name="common" />
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.docMerge.js" />
                <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
            </Scripts>
        </cc1:ToolkitScriptManager>
        <div>
            <table height="100%" width="100%" id="tblReportStatus">
                <tr>
                    <td valign="middle" align="center">
                        <div id="reportStatus">
                            Processing Request
                        </div>
                        <div id="reportProgress" style="width: 250px; background-image: url('/shared/images/exportBackground250px.png'); height: 20px;">
                            <div id="reportDone" style="background-image: url('/shared/images/export250px.png'); height: 20px; text-align: left; float: left;">
                                &nbsp;
                            </div>
                        </div>
                        <div id="reportPercentDone">
                            0%
                        </div>
                    </td>
                </tr>
                <tr>
                    <td valign="middle" align="center">
                        <span id="divMergeProgress">
                            <asp:Literal ID="litMergeProgress" runat="server"></asp:Literal>
                        </span>
                    </td>
                </tr>
                <tr id="documentHyperLinkRow" runat="server" style="display: none;">
                    <td valign="middle" align="center">
                        <span id="divDocumentUrl">
                            <asp:HyperLink ID="hyperLinkDocumentUrl" runat="server"></asp:HyperLink>
                        </span>
                    </td>
                </tr>
            </table>
            <div style="display:none">
                HiddenDiv
                <asp:TextBox runat="server" ID="divProject" CssClass="projectId"></asp:TextBox>
                <asp:TextBox runat="server" ID="divMerge" CssClass="mergeId"></asp:TextBox>
                
            </div>
        </div>
    </form>
</body>

</html>
