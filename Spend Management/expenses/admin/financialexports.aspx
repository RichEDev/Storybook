<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="financialexports.aspx.cs" Inherits="Spend_Management.financialexports" %>

<%@ Register Assembly="SpendManagementHelpers" Namespace="SpendManagementHelpers" TagPrefix="cc2" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/reports/ReportFunctions.asmx" />
            <asp:ServiceReference Path="~/expenses/webservices/svcFinancialExports.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/financialexports.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <a href="javascript:AddFinancialExport();" class="submenuitem">Add Export</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

    <script type="text/javascript">
        //<![CDATA[
        var ddlApplicationID = '<% = ddlApplication.ClientID %>';
        var ddlNHSTrustID = '<% = ddlNHSTrust.ClientID %>';
        var ddlReportID = '<% = ddlReport.ClientID %>';
        var ddlExportTypeID = '<% = ddlExportType.ClientID %>';
        var financialExportModalID = '<% = mdlFinancialExport.ClientID %>';
        var trustDivID = '<% = trustRow.ClientID %>';
        var exportTypeDivID = '<%= exportTypeRow.ClientID %>';
        var pnlFinancialExportGrid = '<% = pnlDynGrid.ClientID %>';
        var modESRAssignmentCheck = '<%= mdlESRAssignmentCheck.ClientID %>';
        var chkPreventNegativePayments = '<%= chkPreventNegativePayments.ClientID %>';
        var chkExpeditePayment = '<%=chkExpeditePayment.ClientID%>';
        //]]>
    </script>

    <div class="formpanel formpanel_padding">    
        <asp:Panel ID="pnlDynGrid" runat="server"><asp:Literal ID="litGrid" runat="server"></asp:Literal></asp:Panel>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
    
    <cc1:ModalPopupExtender runat="server" ID="mdlFinancialExport" OkControlID="btnSave" CancelControlID="btnCancel" OnOkScript="SaveFinancialExport" OnCancelScript="CancelFinancialExport" PopupControlID="pnlFinancialExport" TargetControlID="lnkFinancialExport" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:Panel CssClass="modalpanel" ID="pnlFinancialExport" runat="server" style="display:none;">
        <div class="formpanel">
            <div class="sectiontitle">Financial Export Details</div>
            <div class="onecolumnsmall">
                <asp:Label AssociatedControlID="ddlApplication" runat="server" ID="lblApplication">Application</asp:Label><span class="inputs"><asp:DropDownList ID="ddlApplication" runat="server" onchange="showTrust();" CssClass="fillspan"><asp:ListItem Value="1" Text="Custom Report"></asp:ListItem><asp:ListItem Value="2" Text="ESR"></asp:ListItem></asp:DropDownList></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="onecolumnsmall" id="trustRow" runat="server" style="display:none">
                <asp:Label AssociatedControlID="ddlNHSTrust" runat="server" ID="lblNHSTrust">NHS Trust</asp:Label><span class="inputs"><asp:DropDownList ID="ddlNHSTrust" runat="server" CssClass="fillspan"> </asp:DropDownList></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label AssociatedControlID="ddlReport" runat="server" ID="lblReport">Report</asp:Label><span class="inputs"><asp:DropDownList ID="ddlReport" runat="server"></asp:DropDownList></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
            </div>
            <div class="onecolumnsmall" id="exportTypeRow" runat="server">
                <asp:Label AssociatedControlID="ddlExportType" runat="server" ID="lblExportType">Export Type</asp:Label><span class="inputs"><asp:DropDownList ID="ddlExportType" runat="server" CssClass="fillspan"><asp:ListItem Value="2">Excel</asp:ListItem><asp:ListItem Value="3">CSV</asp:ListItem><asp:ListItem Value="4">Flat File</asp:ListItem></asp:DropDownList></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn" id="preventNegativeRow" runat="server">
                <asp:Label AssociatedControlID="chkPreventNegativePayments" runat="server" ID="Label1">Prevent negative payments</asp:Label><span class="inputs"><asp:CheckBox ID="chkPreventNegativePayments" runat="server"></asp:CheckBox></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip427" alt="" class="tooltipicon" onmouseover="SEL.Tooltip.Show('EC8A2622-0E94-41BC-AE93-F63CF9DE5C0E', 'sm', this);"  src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield"></span>
            </div>
               <div class="twocolumn" id="expeditePaymentRow" runat="server">
                <asp:Label AssociatedControlID="chkExpeditePayment" runat="server" ID="Label2">Expedite Payment Report</asp:Label><span class="inputs"><asp:CheckBox ID="chkExpeditePayment"  Checked="false" runat="server"></asp:CheckBox></span><span class="inputicons">&nbsp;</span><span class="inputtooltipfield"><img id="toolt" alt="" class="tooltipicon" onmouseover="SEL.Tooltip.Show('85332B98-2FFF-4A74-AC8E-591EE03C5EF9', 'sm', this);"  src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp; </span>
            </div>
            <div class="formbuttons">
                <asp:Image ID="btnSave" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" onclick="javascript:SaveFinancialExport();" AlternateText="Save" />&nbsp;<asp:Image runat="server" ID="btnCancel" ImageUrl="/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" onclick="javascript:CancelFinancialExport();" />
            </div>
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkFinancialExport" runat="server" style="display:none;">&nbsp;</asp:HyperLink>
    
    
    <asp:Panel CssClass="modalpanel" ID="pnlESRAssignmentCheck" runat="server" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                Expense Items missing ESR Assignment Numbers</div>
                <div id="ESRInfoDiv" style="height: 500px; width: 100%; overflow: scroll">
            </div>
                <div class="formbuttons">
                    <cc2:CSSButton ID="btnClose" runat="server" Text="cancel"  />&nbsp;<cc2:CSSButton ID="btnExport" runat="server" OnClientClick="javascript:runExport();" Text="export" />
                </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="mdlESRAssignmentCheck" OkControlID="btnExport" CancelControlID="btnClose" PopupControlID="pnlESRAssignmentCheck" TargetControlID="lnkESRAssignmentCheck" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:HyperLink ID="lnkESRAssignmentCheck" runat="server" Style="display: none;">&nbsp;</asp:HyperLink>    
    
</asp:Content>
