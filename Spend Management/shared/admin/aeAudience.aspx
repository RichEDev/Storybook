<%@ Page Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="aeAudience.aspx.cs" Inherits="Spend_Management.aeAudience" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');" id="lnkGeneral" class="selectedPage">Audience Details</a>
    <a href="javascript:changePage('Employees');" id="lnkEmployees">Employees</a>
    <div id="budgetHolderLnkContainer" runat="server"><a href="javascript:changePage('BudgetHolders');" id="lnkBudgetHolders">Budget Holders</a></div>
    <a href="javascript:changePage('Teams');" id="lnkTeams">Teams</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentoptions" runat="server">
    <div id="pgOptEmployees" style="display:none;">
        <a href="javascript:SEL.Audience.AddEmployeeToAudience();" title="New Employee" class="submenuitem">New Employee</a>
    </div>
    <div id="pgOptBudgetHolders" style="display:none;">
        <a href="javascript:SEL.Audience.AddBudgetHolderToAudience();" title="New Budget Holder" class="submenuitem">New Budget Holder</a>
    </div>
    <div id="pgOptTeams" style="display:none;">
        <a href="javascript:SEL.Audience.AddTeamToAudience();" title="New Team" class="submenuitem">New Team</a>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.audiences.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAudiences.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var audId;
        var entityIdentifier;
        var baseTableId;
        var parentRecordId;
        var pnlEmployees = '<% = pnlEmployeesGrid.ClientID %>';
        var pnlBudgetHolders = '<% = pnlBudgetHoldersGrid.ClientID %>';
        var pnlTeams = '<% = pnlTeamsGrid.ClientID %>';
        var audienceNameID = '<% = txtAudienceName.ClientID %>';
        var audienceDescriptionID = '<% = txtAudienceDescription.ClientID %>';

        var employeesModalGridID = '<% = pnlEmployeesModalGrid.ClientID %>';
        var employeesModalID = '<% = mdlEmployees.ClientID %>';
        var employeesFilter = '<% = txtEmployeeGridFilter.ClientID %>';

        var budgetHoldersModalGridID = '<% = pnlBudgetHoldersModalGrid.ClientID %>';
        var budgetHoldersModalID = '<% = mdlBudgetHolders.ClientID %>';
        var budgetHoldersFilter = '<% = txtBudgetHolderGridFilter.ClientID %>';

        var teamsModalGridID = '<% = pnlTeamsModalGrid.ClientID %>';
        var teamsModalID = '<% = mdlTeams.ClientID %>';
        var teamsFilter = '<% = txtTeamGridFilter.ClientID %>';
        //]]>
    </script>
    
    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Audience Details</div>
                <div class="twocolumn">
                    <asp:Label ID="lblAudienceName" runat="server" AssociatedControlID="txtAudienceName" CssClass="mandatory">Audience name *</asp:Label><span class="inputs"><asp:TextBox ID="txtAudienceName" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvAudienceName" runat="server" ControlToValidate="txtAudienceName" ErrorMessage="Audience name is mandatory" Text="*" ValidationGroup="generalDetails"></asp:RequiredFieldValidator><asp:CompareValidator ID="cvAudienceName" runat="server" ControlToValidate="txtAudienceName" ErrorMessage="Audience name should be a string value" Text="*" Operator="DataTypeCheck" Type="String" ValidationGroup="generalDetails"></asp:CompareValidator></span>
                </div>
                <div class="onecolumn">
                    <asp:Label ID="lblAudienceDescription" runat="server" AssociatedControlID="txtAudienceDescription" CssClass="mandatory"><p class="labeldescription">Audience description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtAudienceDescription" runat="server" CssClass="fillspan" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                </div>
            </div>
        </div>
        <div id="pgEmployees" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Employees</div>
                <asp:Panel ID="pnlEmployeesGrid" runat="server"><asp:Literal ID="litEmployees" runat="server"></asp:Literal></asp:Panel>
            </div>
        </div>
        <div id="pgBudgetHolders" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Budget Holders</div>
                <asp:Panel ID="pnlBudgetHoldersGrid" runat="server"><asp:Literal ID="litBudgetHolders" runat="server"></asp:Literal></asp:Panel>
            </div>
        </div>
        <div id="pgTeams" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Teams</div>
                <asp:Panel ID="pnlTeamsGrid" runat="server"><asp:Literal ID="litTeams" runat="server"></asp:Literal></asp:Panel>
            </div>
        </div>
        <div class="formpanel formpanel_padding">
            <div class="formbuttons">
                <asp:Image ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" runat="server" ID="btnSave" onclick="javascript:SEL.Audience.SaveAudience();" />&nbsp;<asp:Image ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" runat="server" ID="btnCancel" onclick="javascript:SEL.Audience.CancelAudience();" />
            </div>
        </div>
    </div>
    
    <div>
    <cc1:ModalPopupExtender ID="mdlEmployees" runat="server" TargetControlID="lnkDummyEmp" PopupControlID="pnlEmployeesModal" OnOkScript="SaveAudienceEmployees" OnCancelScript="CancelAudienceEmployees" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:Panel ID="pnlEmployeesModal" runat="server" CssClass="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">New Employees</div>
            <div class="twocolumn">
                <asp:Label ID="lblEmployeeGridFilter" AssociatedControlID="txtEmployeeGridFilter" runat="server">Username</asp:Label><span class="inputs"><asp:TextBox ID="txtEmployeeGridFilter" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"><asp:Image ID="btnEmployeeSearch" runat="server" ImageUrl="~/shared/images/icons/find.png" onclick="javascript:SEL.Audience.FilterEmployeesGrid();" AlternateText="Search" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <asp:Panel ID="pnlEmployeesModalGrid" runat="server" CssClass="twocolumn"><asp:Literal ID="litEmployeesModalGrid" runat="server"></asp:Literal></asp:Panel>
            <div class="formbuttons">
                <asp:Image ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" runat="server" ID="btnSaveEmployees" onclick="javascript:SEL.Audience.SaveAudienceEmployees();" />&nbsp;<asp:Image ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" runat="server" ID="btnCancelEmployees" onclick="javascript:SEL.Audience.CancelAudienceEmployees();" />
            </div>
        </div>
    </asp:Panel>
    
    <cc1:ModalPopupExtender ID="mdlBudgetHolders" runat="server" TargetControlID="lnkDummyBH" PopupControlID="pnlBudgetHoldersModal" OnOkScript="SaveAudienceBudgetHolders" OnCancelScript="CancelAudienceBudgetHolders" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:Panel ID="pnlBudgetHoldersModal" runat="server" CssClass="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">New Budget Holders</div>
            <div class="twocolumn">
                <asp:Label ID="lblBudgetHolderGridFilter" AssociatedControlID="txtBudgetHolderGridFilter" runat="server">Budget holder name</asp:Label><span class="inputs"><asp:TextBox ID="txtBudgetHolderGridFilter" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"><asp:Image ID="btnBudgetHolderSearch" runat="server" ImageUrl="~/shared/images/icons/find.png" onclick="javascript:SEL.Audience.FilterBudgetHoldersGrid();" AlternateText="Search" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <asp:Panel ID="pnlBudgetHoldersModalGrid" runat="server" CssClass="twocolumn"><asp:Literal ID="litBudgetHoldersModalGrid" runat="server"></asp:Literal></asp:Panel>
            <div class="formbuttons">
                <asp:Image ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" runat="server" ID="btnSaveBudgetHolders" onclick="javascript:SEL.Audience.SaveAudienceBudgetHolders();" />&nbsp;<asp:Image ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" runat="server" ID="btnCancelBudgetHolders" onclick="javascript:SEL.Audience.CancelAudienceBudgetHolders();" />
            </div>
        </div>
    </asp:Panel>
    
    <cc1:ModalPopupExtender ID="mdlTeams" runat="server" TargetControlID="lnkDummyTeam" PopupControlID="pnlTeamsModal" OnOkScript="SaveAudienceTeams" OnCancelScript="CancelAudienceTeams" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:Panel ID="pnlTeamsModal" runat="server" CssClass="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">New Teams</div>
            <div class="twocolumn">
                <asp:Label ID="lblTeamGridFilter" AssociatedControlID="txtTeamGridFilter" runat="server">Team name</asp:Label><span class="inputs"><asp:TextBox ID="txtTeamGridFilter" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"><asp:Image ID="btnTeamSearch" runat="server" ImageUrl="~/shared/images/icons/find.png" onclick="javascript:SEL.Audience.FilterTeamsGrid();" AlternateText="Search" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <asp:Panel ID="pnlTeamsModalGrid" runat="server" CssClass="twocolumn"><asp:Literal ID="litTeamsModalGrid" runat="server"></asp:Literal></asp:Panel>
            <div class="formbuttons">
                <asp:Image ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" runat="server" ID="btnSaveTeams" onclick="javascript:SEL.Audience.SaveAudienceTeams();" />&nbsp;<asp:Image ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" runat="server" ID="btnCancelTeams" onclick="javascript:SEL.Audience.CancelAudienceTeams();" />
            </div>
        </div>
    </asp:Panel>
    
    
    <asp:HyperLink runat="server" ID="lnkDummyEmp" style="display:none;"></asp:HyperLink>
    <asp:HyperLink runat="server" ID="lnkDummyBH" style="display:none;"></asp:HyperLink>
    <asp:HyperLink runat="server" ID="lnkDummyTeam" style="display:none;"></asp:HyperLink>
    </div>
</asp:Content>
