<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeAccessRole.aspx.cs" Inherits="Spend_Management.aeAccessRole" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
    <Services>
    <asp:ServiceReference Path="~/shared/webServices/svcAccessRoles.asmx" />
    </Services>
    <Scripts>
    <asp:ScriptReference Path="~/shared/javaScript/accessRoles.js" />
    </Scripts>
    <Scripts>
    <asp:ScriptReference Path="~/shared/javaScript/sel.AccessRoles.js" />
   </Scripts> 
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="twocolumn"><asp:Label ID="lblRoleName" runat="server" AssociatedControlID="txtRoleName" CssClass="mandatory">Role Name*</asp:Label><span class="inputs"><asp:TextBox ID="txtRoleName" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"> </span><span class="tooltipicon"> </span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfRoleName" ControlToValidate="txtRoleName" ErrorMessage="Role Name must be specified" Text="*" runat="server"></asp:RequiredFieldValidator></span></div>
        <div class="onecolumn"><asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server"></asp:TextBox></span></div>
        <div class="onecolumn" id="reportAccessLevelDiv"><asp:Label ID="lblReportAccessLevel" runat="server" AssociatedControlID="radReportAccessLevel_3">Reports Access</asp:Label><span class="inputs">Select what data can be reported on by this access role<br /><asp:RadioButton ID="radReportAccessLevel_3" GroupName="accessLevel" runat="server" /> All data<br /><asp:RadioButton ID="radReportAccessLevel_1" GroupName="accessLevel" runat="server" /><asp:Literal runat="server" ID="litclaimantreportopt"></asp:Literal><br /><asp:RadioButton ID="radReportAccessLevel_2" GroupName="accessLevel" runat="server" /> Data from the following access roles: 
            
            <asp:HyperLink ID="lnkAccessRoles" runat="server" Text="Access Roles" CssClass="hyperlink">Set Access Roles</asp:HyperLink></span></div>
        <asp:Panel runat="server" ID="accessSpan"><div class="sectiontitle">Product Access</div>
            <div class="twocolumn" runat="server" ID="websiteDiv"><asp:Label ID="lblWebsite" runat="server" AssociatedControlID="chkWebsite" CssClass="productAccess mandatory">Website*</asp:Label>
                <span class="inputs"><asp:CheckBox ID="chkWebsite" GroupName="productAccess" runat="server"  onClick="ValidateProductAccess();"/></span><asp:CustomValidator runat="server" CssClass="cvProductAccess" ErrorMessage="You must select at least one Product to Access." Text="*" ClientValidationFunction="ValidateProductAccess" ></asp:CustomValidator></div>
            <div class="twocolumn" runat="server" ID="mobileDiv" ><asp:Label ID="lblMobile" runat="server" AssociatedControlID="chkMobile" CssClass="productAccess mandatory">Mobile*</asp:Label>
                <span class="inputs"><asp:CheckBox ID="chkMobile" GroupName="productAccess" runat="server" onClick="ValidateProductAccess();"/></span><asp:CustomValidator runat="server" CssClass="cvProductAccess" Text="*" ClientValidationFunction="ValidateProductAccess" ></asp:CustomValidator></div>
            <div class="twocolumn" runat="server" ID="ApiDiv"><asp:Label ID="lblAPI" runat="server" AssociatedControlID="chkAPI" CssClass="productAccess mandatory">API*</asp:Label>
                <span class="inputs"><asp:CheckBox ID="chkAPI" GroupName="productAccess" runat="server" onClick="ValidateProductAccess();"/></span><asp:CustomValidator runat="server" CssClass="cvProductAccess" Text="*" ClientValidationFunction="ValidateProductAccess" ></asp:CustomValidator></div></asp:Panel>
        <span id="tableHolder"><asp:PlaceHolder ID="phElementAccessTabs" runat="server"></asp:PlaceHolder></span>
        <div class="formbuttons">
            <img src="/shared/images/buttons/btn_save.png" alt="Save" title="Save" onclick="saveAccessRoleElementAccess(accessRoleID, accessRoleNameID, accessRoleDescriptionID);" /> 
            <a href="accessRoles.aspx" title="Access Roles">
                <img src="/shared/images/buttons/cancel_up.gif" alt="Cancel" title="Cancel" /></a>

        </div>
    </div>
    <div id="ReportableFields"></div>
    <asp:Panel ID="pnlAccessRoles" runat="server" CssClass="modalpanel" >
        <asp:Literal ID="litLinkedAccessRoles" runat="server"></asp:Literal>
    </asp:Panel>
        <script language="javascript" type="text/javascript">
            var accessRoleID = '<% = accessRoleID %>';
            var accessRoleNameID = '<% = txtRoleName.ClientID %>';
            var accessRoleDescriptionID = '<% = txtDescription.ClientID %>';
            var linkedAccessRolesDivID = '<% = pnlAccessRoles.ClientID %>';
            var claimMinimumAmount = document.getElementById("ctl00_contentmain_tcElementAccess_tb2_<% = ClaimMinimumAmount %>");
            var claimMaximumAmount = document.getElementById("ctl00_contentmain_tcElementAccess_tb2_<% = ClaimMaximumAmount %>");
            var chkCanEditDepartmentsObj = '<% = chkCanEditDepartmentsObj %>';
            var chkCanEditCostCodesObj = '<% = chkCanEditCostCodesObj %>';
            var chkCanEditProjectCodesObj = '<% = chkCanEditProjectCodesObj  %>';
            var chkMustHaveBankAccountObj = '<% = chkMustHaveBankAccountObj  %>';
            SEL.AccessRole.ExclusionType = '<% = this.ExclusionType  %>';
            $(document).ready(function () {
                SEL.AccessRole.LoadReportableFieldsLogic(accessRoleID);
            });
    </script>
    <cc1:PopupControlExtender ID="popupAccessRoles" runat="server" PopupControlID="pnlAccessRoles" TargetControlID="lnkAccessRoles"></cc1:PopupControlExtender>
</asp:Content>