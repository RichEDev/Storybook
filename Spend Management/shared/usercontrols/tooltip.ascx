<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="tooltip.ascx.cs" Inherits="Spend_Management.tooltip" %>
<asp:ScriptManagerProxy ID="smp" runat="server">
<Services>
<asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
</Services>
</asp:ScriptManagerProxy>
<asp:Panel ID="pnlTooltipContainer" runat="server" CssClass="tooltipcontainer" style="display: none">
   <asp:Panel ID="pnlTooltipContent" runat="server" CssClass="tooltipcontent"></asp:Panel>
</asp:Panel>
<cc1:PopupControlExtender ID="pceTooltip" runat="server" TargetControlID="imgTooltipTarget" PopupControlID="pnlTooltipContainer" Position="Right" OffsetX="15"></cc1:PopupControlExtender>
<asp:Image ID="imgTooltipTarget" runat="server" style="display:none;" ImageUrl="" AlternateText="" />