<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_exportoptions" EnableTheming="true" Codebehind="exportoptions.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export Options</title>
    <link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
    <link rel="stylesheet" type="text/css"  media="screen" href="../css/styles.aspx?style=logon" />
    <link id="favLink" runat="server" rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
</head>
<body>
<asp:Literal ID="litStyles" runat="server"></asp:Literal>
    <form id="form1" runat="server">

        <cc1:ToolkitScriptManager ID="tsm" runat="server">
            <Services>
                <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="/static/js/jQuery/jquery-1.9.1.min.js"/>
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.main.js" />
                <asp:ScriptReference Name="common" />
                
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
        <igtbl:UltraWebGrid ID="gridflatfile" runat="server" OnInitializeLayout="gridflatfile_InitializeLayout" meta:resourcekey="gridflatfileResource1">
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout AllowColSizingDefault="Free" BorderCollapseDefault="Separate" CellClickActionDefault="Edit"
                HeaderClickActionDefault="NotSet" Name="UltraWebGrid1" RowHeightDefault="20px"
                RowSelectorsDefault="No" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00">
                <GroupByBox>
                    <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                    </BoxStyle>
                </GroupByBox>
                <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                </GroupByRowStyleDefault>
                <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </FooterStyleDefault>
                <RowStyleDefault CssClass="row1">
                </RowStyleDefault>
                <FilterOptionsDefault>
                    <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                        Font-Size="11px" Height="300px" Width="200px">
                        <Padding Left="2px" />
                    </FilterDropDownStyle>
                    <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                    </FilterHighlightRowStyle>
                </FilterOptionsDefault>
                <HeaderStyleDefault CssClass="th">
                    <BorderDetails WidthLeft="1px" WidthTop="1px" />
                </HeaderStyleDefault>
                <RowAlternateStyleDefault CssClass="row2">
                </RowAlternateStyleDefault>
                <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                </EditCellStyleDefault>
                <FrameStyle CssClass="datatbl" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                </FrameStyle>
                <Pager MinimumPagesForDisplay="2">
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </PagerStyle>
                </Pager>
                <AddNewBox Hidden="False">
                    <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </BoxStyle>
                </AddNewBox>
                <ActivationObject BorderColor="" BorderWidth="">
                </ActivationObject>
            </DisplayLayout>
        </igtbl:UltraWebGrid>
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
        <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" /> &nbsp;&nbsp;<a href="javascript:window.close();"><img style="border-width: 0px;" src="../images/buttons/cancel_up.gif" /></a>
    </div>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    </form>
</body>
</html>
