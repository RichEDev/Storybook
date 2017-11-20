
<%@ Page language="c#" Inherits="expenses.qelst" MasterPageFile="~/exptemplate.master" Codebehind="qelst.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="styles">
    <style type="text/css">
        nobr a{
            color: #003768;
            text-decoration:underline;
        }
    </style>

  </asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel table-border">
	
    <igtbl:UltraWebGrid ID="gridforms" runat="server" SkinID="gridskin" OnInitializeLayout="gridforms_InitializeLayout" OnInitializeRow="gridforms_InitializeRow" meta:resourcekey="gridformsResource1">
        
    </igtbl:UltraWebGrid>
</div>

    	<div class="formpanel formpanel_padding">
          
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
          
        </div>

  </asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="qeform.aspx" class="submenuitem">
        <asp:Label ID="lblupload" runat="server" Text="Upload<br />Spreadsheet" meta:resourcekey="lbluploadResource1"></asp:Label></a></asp:Content>

