<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CostCentreBreakdown.ascx.cs" Inherits="Spend_Management.CostCentreBreakdown" %>
<asp:ScriptManagerProxy ID="smpCostCentreBreakdown" runat="server">
	<Scripts>
		<asp:ScriptReference Path="~/shared/javaScript/costCentreBreakdown.js" />
	</Scripts>
</asp:ScriptManagerProxy>
<div>
    <script type="text/javascript">
    //<![CDATA[
        // ids
        var ccbItemId;
        var ccbModalId = '<% = ModalExtenderId %>';
        var ccbTableId = '<% = tblCostCentreBreakdown.ClientID %>';
        var ccbddlDepartmentsId = '<% = ddlDepartmentsId %>';
        var ccbddlCostCodesId = '<% = ddlCostCodesId %>';
        var ccbddlProjectCodesId = '<% = ddlProjectCodesId %>';
    
        // Booleans
        // Convert.ToString().ToLower() as .Net bool strings are True/False and JS bool is true/false
        var ccbUseCostCentres = <%= Convert.ToString(costCentresEnabled).ToLower() %>;
        var ccbUseDepartments = <%= Convert.ToString(departmentCodeEnabled).ToLower() %>;
        var ccbUseCostCodes = <%= Convert.ToString(costCodeEnabled).ToLower() %>;
        var ccbUseProjectCodes = <%= Convert.ToString(projectCodeEnabled).ToLower() %>;
        var ccbReadOnly = <%= Convert.ToString(ReadOnly).ToLower() %>;
    
        var ccbRowClass = "row1";
        var ccbRowId = 0;
    //]]>
    </script>

    <asp:Panel ID="pnlCostCentreBreakdown" runat="server">
        <span id="ccodecontrol" class="formpanel">
            <div class="sectiontitle" style="width: auto;">Cost Centre Breakdown</div>
            <div>
                <asp:Table ID="tblCostCentreBreakdown" runat="server" CssClass="cGrid"><asp:TableHeaderRow ID="trCostCentreBreakdown" runat="server"></asp:TableHeaderRow></asp:Table>
            </div>
            <asp:Panel ID="pnlButtons" runat="server" CssClass="formbuttons"></asp:Panel>

            <div style="display: none;">
                <asp:Panel runat="server" ID="costCentreDepartmentsHolder" data-modal="<%# departmentsModal.ClientID %>" data-panel="<%# departmentsPanel.ClientID %>" data-grid="<%# departmentsGrid.ClientID %>"></asp:Panel>
                <asp:Panel runat="server" ID="costCentreCostCodesHolder" data-modal="<%# costCodesModal.ClientID %>" data-panel="<%# costCodesPanel.ClientID %>" data-grid="<%# costCodesGrid.ClientID %>"></asp:Panel>
                <asp:Panel runat="server" ID="costCentreProjectCodesHolder" data-modal="<%# projectCodesModal.ClientID %>" data-panel="<%# projectCodesPanel.ClientID %>" data-grid="<%# projectCodesGrid.ClientID %>"></asp:Panel>
            </div>
        </span>
    </asp:Panel>

    <asp:PlaceHolder runat="server" ID="comboModals"></asp:PlaceHolder>
</div>