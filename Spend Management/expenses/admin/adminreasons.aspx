<%@ Page language="c#" Inherits="Spend_Management.adminreasons" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminreasons.aspx.cs" EnableViewState="false" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
       <asp:HyperLink runat="server" ID="lnkNewReason" NavigateUrl="aereason.aspx"  CssClass="submenuitem">New Reason</asp:HyperLink>
      <a class="submenuitem" href="/admin/filterrules.aspx?FilterType=5"><asp:Label id="lblFilterRules" runat="server" meta:resourcekey="lblFilterRulesResource1">Filter Rules</asp:Label></a>
    </asp:Content>
				
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">				
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/reasons.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
    <asp:Literal ID="litgrid" runat="server"></asp:Literal>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
</div>

    </asp:Content>


