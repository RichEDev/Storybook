<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeImportTemplate.aspx.cs" Inherits="Spend_Management.aeImportTemplate" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.importTemplates.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcImportTemplates.asmx" InlineScript="false" />
            <asp:ServiceReference Path="~/shared/webServices/svcWorkflows.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    <script type="text/javascript" language="javascript">
        (function(it)
        {
            it.TabsDomID = '<%=pnlMappings.ClientID %>';
            it.TemplateNameDomID = '<%=txtTemplateName.ClientID %>';
            it.ApplicationTypeDomID = '<%=ddlApplicationType.ClientID %>';
            it.TrustDomID = '<%=ddlTrustID.ClientID %>';
            it.CurrentTemplateID = <%=jsTemplateID %>;
        }(SEL.ImportTemplates));
    </script>
    <asp:Panel ID="pnlFormpanel" CssClass="formpanel formpanel_padding" runat="server">
        <div class="sectiontitle">General Details</div>
        <div class="twocolumn">
            <asp:Label runat="server" AssociatedControlID="txtTemplateName" ID="lblTemplateName" Text="Template Name*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtTemplateName" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvTemplateName" runat="server" ErrorMessage="Please enter a Template Name." Text="*" ControlToValidate="txtTemplateName"></asp:RequiredFieldValidator><cc1:FilteredTextBoxExtender runat="server" ID="ftbeTemplateName" TargetControlID="txtTemplateName" InvalidChars="<>" FilterType="Custom" FilterMode="InvalidChars"/></span><asp:Label runat="server" AssociatedControlID="ddlApplicationType" ID="lblApplicationType" Text="Application Type*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:DropDownList runat="server" CssClass="fillspan" ID="ddlApplicationType" OnSelectedIndexChanged="ddlApplicationType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpApplicationType" ControlToValidate="ddlApplicationType" Operator="GreaterThan" ValueToCompare="-1" Text="*" Type="Integer" ErrorMessage="Please select an Application Type."></asp:CompareValidator></span>
        </div>
        <div class="onecolumnsmall">
            <%--<asp:Label runat="server" AssociatedControlID="chkAutomated" ID="lblAutomated" Text="Automated Import"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAutomated" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>--%>
            <asp:Label runat="server" AssociatedControlID="ddlTrustID" ID="lblTrustID" Text="Trust*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:DropDownList runat="server" CssClass="fillspan" ID="ddlTrustID" OnSelectedIndexChanged="ddlTrustID_SelectedIndexChanged" AutoPostBack="true"><asp:ListItem Text="[None]" Value="0"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator
                ID="cvTrust" runat="server" ErrorMessage="Please select a Trust." Text="*" ControlToValidate="ddlTrustID" ValueToCompare="0" Operator="GreaterThan" Type="Integer"></asp:CompareValidator></span>
        </div>
        <asp:UpdatePanel ID="upnlMappings" runat="server" UpdateMode="Conditional">
            <Triggers><asp:AsyncPostBackTrigger EventName="selectedindexchanged" ControlID="ddlApplicationType" /><asp:AsyncPostBackTrigger EventName="selectedindexchanged" ControlID="ddlTrustID"/></Triggers>
            <ContentTemplate>
        <asp:Panel ID="pnlMappings" runat="server">
            </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        <div class="formbuttons">
            <a onclick="javascript:SEL.ImportTemplates.SaveImportTemplateMappings();"><asp:Image runat="server" ID="btnSave" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" /></a>&nbsp;<a href="#" onclick="javascript:SEL.ImportTemplates.CancelImportTemplateMapping();"><asp:Image runat="server" ID="btnCancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" /></a>
        </div>
        <cc1:TabContainer ID="blankTabContainer" runat="server" Visible="true">
        </cc1:TabContainer>
    </asp:Panel>
</asp:Content>
