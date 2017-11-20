<%@ Page Title="" Language="C#" MasterPageFile="~/masters/SMForm.master" AutoEventWireup="true" Inherits="Spend_Management.TaskSummary" Codebehind="TaskSummary.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/SMForm.master" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
    <asp:LinkButton runat="server" ID="lnkViewActive" CssClass="submenuitem" Visible="false"
        OnClick="lnkViewActive_Click">View Active</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkViewClosed" CssClass="submenuitem" OnClick="lnkViewClosed_Click">View Closed</asp:LinkButton>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:HiddenField runat="server" ID="returnURL" />
    <div class="formpanel formpanel_padding">
    <div class="sectiontitle">Task Summary</div>
    <div class="twocolumn">
<asp:Label runat="server" ID="lblAppArea" AssociatedControlID="txtAppArea">Applies To</asp:Label>
<span class="inputs"><asp:TextBox runat="server" ID="txtAppArea" CssClass="fillspan" Enabled="false"></asp:TextBox></span>
    <span class="inputicon">&nbsp;</span>
    <span class="inputtooltipfield">&nbsp;</span>
    <span class="inputvalidatorfield">&nbsp;</span>
<asp:Label runat="server" ID="lblRegarding" AssociatedControlID="txtRegarding">Regarding</asp:Label>
    <span class="inputs"><asp:TextBox runat="server" ID="txtRegarding" Enabled="false" CssClass="fillspan"></asp:TextBox></span>
    <span class="inputicon">&nbsp;</span>
    <span class="inputtooltipfield">&nbsp;</span>
    <span class="inputvalidatorfield">&nbsp;</span>
</div>
    </div>
    <div class="formpanel formpanel_padding">
        <asp:PlaceHolder runat="server" ID="phTaskSummary"></asp:PlaceHolder>
   
    <div class="formbuttons">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="~/shared/images/buttons/btn_close.png"
            CausesValidation="false" OnClick="cmdClose_Click" />
    </div> </div>
</asp:Content>
