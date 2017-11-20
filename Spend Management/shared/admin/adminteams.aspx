<%@ Page language="c#" Inherits="Spend_Management.adminteams" MasterPageFile="~/masters/smForm.master" Codebehind="adminteams.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="aeteam.aspx" class="submenuitem">
    <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Team</asp:Label></a>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy runat="server" ID="smproxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/teams.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcTeams.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var pnlTeamsGridID = '<% = pnlTeamsGrid.ClientID %>';
        var pnlTeamMembersID = '<% = pnlTeamMembers.ClientID %>';
        var popTeamMembersGridID = '<% = popTeamMembersGrid.ClientID %>';
        var pnlTeamMembersGridID = '<% = pnlTeamMembersGrid.ClientID %>';
        //]]>
    </script>
    
    <div class="formpanel formpanel_padding">
        <asp:Panel ID="pnlTeamsGrid" runat="server">
	        <asp:Literal id="litTeamsGrid" runat="server" EnableViewState="False" meta:resourcekey="litgridResource1"></asp:Literal>
	    </asp:Panel>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div> 
	</div>
    
    <cc1:PopupControlExtender ID="popTeamMembersGrid" runat="server" OffsetX="22" OffsetY="22" TargetControlID="lnkDummy" PopupControlID="pnlTeamMembers"></cc1:PopupControlExtender>
    <asp:Panel ID="pnlTeamMembers" runat="server" CssClass="formpanel modalpanel" style="width:auto; display:none;">
        <div class="sectiontitle">Team Members</div>
        <asp:Panel ID="pnlTeamMembersGrid" runat="server"></asp:Panel>
    </asp:Panel>
    
    <asp:HyperLink ID="lnkDummy" runat="server" style="display:none;" />


</asp:Content>


