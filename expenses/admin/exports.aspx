<%@ Page language="c#" Inherits="expenses.admin.exports" MasterPageFile="~/exptemplate.master" Codebehind="exports.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics35.WebUI.UltraWebGrid.v8.3, Version=8.3.20083.1009, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript">
function exportReport (reportid, exporttype)
        {
            window.open("../reports/exportreport.aspx?financialexport=1&exporttype=" + exporttype + "&reportid=" + reportid,'export','width=300,height=150,status=no,menubar=no');
        }
		
</script>

	<div class="valdiv">
		<asp:Label id="lblmsg" runat="server" ForeColor="Red"></asp:Label>
	</div>
	<div class="inputpanel">
        <igtbl:UltraWebGrid ID="gridreports" runat="server" OnInitializeLayout="gridreports_InitializeLayout" OnInitializeRow="gridreports_InitializeRow">
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowSortingDefault="OnClient"
                BorderCollapseDefault="Separate" HeaderClickActionDefault="SortSingle" Name="UltraWebGrid1"
                RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                Version="4.00" ViewType="OutlookGroupBy">
                <GroupByBox>
                    <Style BackColor="ActiveBorder" BorderColor="Window"></Style>
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
                </HeaderStyleDefault>
                <RowAlternateStyleDefault CssClass="row2">
                </RowAlternateStyleDefault>
                <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                </EditCellStyleDefault>
                <FrameStyle CssClass="datatbl">
                </FrameStyle>
                <Pager AllowPaging="True" MinimumPagesForDisplay="2" PageSize="30">
                    <Style BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</Style>
                </Pager>
                <AddNewBox Hidden="False">
                    <Style BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</Style>
                </AddNewBox>
            </DisplayLayout>
        </igtbl:UltraWebGrid>
		
	</div>
	

    </asp:Content>

