<%@ Page language="c#" Inherits="Spend_Management.aeteam" MasterPageFile="~/masters/smForm.master" Codebehind="aeteam.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="javascript:AddTeamMembers();" class="submenuitem">Add Team Member</a>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/teams.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcTeams.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var TeamID = <% = TeamID %>;
        var txtTeamNameID = '<% = txtTeamName.ClientID %>';
        var txtDescriptionID = '<% = txtDescription.ClientID %>';
        var ddlTeamLeaderID = '<% = ddlTeamLeader.ClientID %>';
        var pnlTeamMembersGridID = '<% = pnlTeamMembersGrid.ClientID %>';
        var mdlEmployeesGridID = '<% = mdlEmployeesGrid.ClientID %>';
        var pnlEmployeesGridID = '<% = pnlEmployeesGrid.ClientID %>';
        //]]>
    </script>
    	<div class="formpanel formpanel_padding">
		<div class="sectiontitle"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<div class="twocolumn">
			<asp:Label ID="lblTeamName" runat="server" AssociatedControlID="txtTeamName" meta:resourcekey="lblteamnameResource1" CssClass="mandatory">Team name *</asp:Label><span class="inputs"><asp:TextBox id="txtTeamName" runat="server" MaxLength="50" meta:resourcekey="txtteamnameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="rfvTeam" runat="server" ValidationGroup="generalDetails" ErrorMessage="Please enter a team name" ControlToValidate="txtTeamName" meta:resourcekey="reqteamResource1">*</asp:RequiredFieldValidator></span>
	    </div>
	    <div class="onecolumn">
	        <asp:Label id="lblDescription" runat="server" AssociatedControlID="txtDescription" meta:resourcekey="lbldescriptionResource1"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox id="txtDescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
		</div>
		<div class="onecolumnsmall">
			<asp:Label ID="lblTeamLeader" runat="server" AssociatedControlID="ddlTeamLeader" meta:resourcekey="lblteamnameResource1">Team leader</asp:Label><span class="inputs"><asp:DropDownList ID="ddlTeamLeader" runat="server" CssClass="fillspan"><asp:ListItem Value="0">[None]</asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
		</div>
	</div>
	<div class="formpanel formpanel_padding">
		<div class="sectiontitle"><asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">Team Members</asp:Label></div>
		<asp:Panel ID="pnlTeamMembersGrid" runat="server"><asp:Literal ID="litTeamMembersGrid" runat="server"></asp:Literal></asp:Panel>

	    <div class="formbuttons">
		    <asp:Image ID="btnSave" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" onclick="javascript:SaveTeam();" meta:resourcekey="cmdokResource1"></asp:Image>&nbsp;<asp:Image id="btnCancel" runat="server" ImageUrl="/shared/images/buttons/cancel_up.gif" onclick="javascript:CancelTeam();" meta:resourcekey="cmdcancelResource1"></asp:Image>
	    </div>
	</div>
	
	<cc1:ModalPopupExtender ID="mdlEmployeesGrid" runat="server" PopupControlID="pnlEmployees" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
	<asp:Panel ID="pnlEmployees" runat="server" CssClass="formpanel modalpanel" style="width:auto; display:none; height: auto; overflow: auto;">
	    <div class="sectiontitle">Add Team Members</div>
	    <asp:Panel ID="pnlEmployeesGrid" runat="server"><asp:Literal ID="litEmployeesGrid" runat="server"></asp:Literal></asp:Panel>
	    <div class="formbuttons">
	        <asp:Image ID="btnSaveModal" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" onclick="javascript:SaveEmployeesModal();"></asp:Image>&nbsp;<asp:Image id="btnCancelModal" runat="server" ImageUrl="/shared/images/buttons/cancel_up.gif" onclick="javascript:CancelEmployeesModal();"></asp:Image>
	    </div>
	</asp:Panel>
	<asp:HyperLink ID="lnkDummy" runat="server" style="display:none;"></asp:HyperLink>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="scripts">
    <script>
        $(document).ready(function() {
            $('a:contains("Add Team Member")').click(function() {
                $("body").css("overflow", "hidden");
            });

            $("#ctl00_contentmain_btnSaveModal").click(function() {
                $("body").css("overflow", "auto");
            });

            $("#ctl00_contentmain_btnCancelModal").click(function() {
                $("body").css("overflow", "auto");
            });        
        })
       
    </script>
</asp:Content>

