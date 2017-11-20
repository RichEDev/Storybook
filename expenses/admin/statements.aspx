<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="statements.aspx.cs" Inherits="expenses.admin.statements" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="import_statement.aspx" class="submenuitem">Import Statement</a>
    <a href="transaction_viewer.aspx" class="submenuitem">Transaction Viewer</a>
    <a href="corporate_cards.aspx" class="submenuitem">Corporate Card Providers</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

<script type="text/javascript" language="javascript">
    function deleteStatement (statementid)
    {
        if (confirm('Are you sure you wish to delete the selected statement?'))
        {
            PageMethods.deleteStatement(accountid, statementid, deleteStatementComplete);
        }
    }
    
    function deleteStatementComplete (response)
    {
        if (response == false)
        {
            alert('The selected statement cannot be deleted as one or more transactions have already been reconciled');
        }
        else
        {
            var grid = igtbl_getGridById(contentID + 'gridstatements');
						
				grid.Rows.remove(grid.getActiveRow().getIndex());
        }
    }
</script>

<div class="inputpanel table-border">
    <igtbl:UltraWebGrid ID="gridstatements" runat="server" SkinID="gridskin" 
        oninitializelayout="gridstatements_InitializeLayout" 
        oninitializerow="gridstatements_InitializeRow">
    </igtbl:UltraWebGrid></div>
    <div class="inputpanel"><a href="../exportmenu.aspx"><img src="../shared/images/buttons/btn_close.png" alt="Close" /></a></div>
</asp:Content>
