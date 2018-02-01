<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="statements.aspx.cs" Inherits="expenses.admin.statements" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="import_statement.aspx" class="submenuitem">Import Statement</a>
    <a href="transaction_viewer.aspx" class="submenuitem">Transaction Viewer</a>
    <a href="corporate_cards.aspx" class="submenuitem">Corporate Card Providers</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

<script type="text/javascript" language="javascript">
    var id = 0;
    function deleteStatement (statementid) {
        id = statementid;
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
            SEL.Grid.deleteGridRow('gridCardStatements', id);
        }
    }
</script>

   <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" href="../exportmenu.aspx" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
</asp:Content>
