<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="settledadvances.aspx.cs" Inherits="expenses.admin.settledadvances" Title="Untitled Page" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="inputpanel">
    <igtbl:UltraWebGrid ID="gridadvances" runat="server" SkinID="gridskin" OnInitializeLayout="gridadvances_InitializeLayout" meta:resourcekey="gridadvancesResource1">
       
    </igtbl:UltraWebGrid>
    </div>

    	<div class="formpanel formpanel_padding">
            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>

</asp:Content>
