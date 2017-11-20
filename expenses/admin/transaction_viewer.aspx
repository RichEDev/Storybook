<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="transaction_viewer.aspx.cs" Inherits="expenses.admin.transaction_viewer" MasterPageFile="~/exptemplate.master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register tagprefix="igtblexp" namespace="Infragistics.WebUI.UltraWebGrid.ExcelExport" Assembly="Infragistics4.WebUI.UltraWebGrid.ExcelExport.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>



<asp:Content ID="Content1" runat="server" contentplaceholderid="contentmain">



    <div class="inputpanel">
        <div class="inputpaneltitle">Filter Options</div>
        <table>
            <tr><td class="labeltd">Statement</td><td class="inputtd">
                <asp:DropDownList ID="cmbstatement" runat="server" AutoPostBack="True" 
                                        onselectedindexchanged="cmbstatement_SelectedIndexChanged">
                </asp:DropDownList>
            </td></tr>
            <tr><td class="labeltd">Transaction Type</td><td class="inputtd">
                <asp:DropDownList ID="cmbtransactiontype" runat="server" AutoPostBack="True" 
                                        onselectedindexchanged="cmbtransactiontype_SelectedIndexChanged">
                    <asp:ListItem Value="0">All Transactions</asp:ListItem>
                    <asp:ListItem Value="1">Reconciled Transactions</asp:ListItem>
                    <asp:ListItem Value="2">Un-Reconciled Transactions</asp:ListItem>
                </asp:DropDownList>
            </td></tr>
            <tr><td class="labeltd">Employee</td><td class="inputtd">
                <asp:TextBox ID="txtemployee" runat="server"></asp:TextBox><cc1:AutoCompleteExtender
                    ID="autocompemployee" runat="server" TargetControlID="txtemployee" ServicePath="../shared/webServices/svcAutocomplete.asmx" ServiceMethod="getEmployeeUsername" CompletionSetCount="10" EnableCaching="true" MinimumPrefixLength="1" >
                </cc1:AutoCompleteExtender>
            </td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
            onclick="cmdok_Click" />
         <a href="statements.aspx"><img alt="Close" src="/shared/images/buttons/btn_close.png" border="0" /></a>

    </div>
    <div class="inputpanel">
    <asp:Literal runat="server" ID="litMessage"></asp:Literal>
        <igtbl:UltraWebGrid ID="gridtransactions" runat="server" SkinID="gridskin" 
            oninitializelayout="gridtransactions_InitializeLayout" 
            oninitializerow="gridtransactions_InitializeRow">
            
        </igtbl:UltraWebGrid>
        
        <igtblexp:UltraWebGridExcelExporter ID="export" runat="server">
        </igtblexp:UltraWebGridExcelExporter>    
    </div>
   

</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="contentleft">

                
</asp:Content>
<asp:Content ID="Content3" runat="server" contentplaceholderid="contentmenu">
    <asp:LinkButton ID="lnkexporttoexcell" runat="server" 
        onclick="lnkexporttoexcell_Click" CssClass="submenuitem">Export To Excel</asp:LinkButton>
                
</asp:Content>

