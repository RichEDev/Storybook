var childWin;

function OpenRechargeItem(conId,conProdId)
{
    document.location = 'ContractRechargeBreakdown.aspx?action=edit&id=' + conId + '&raid=' + conProdId;
}

function OpenRechargeBreakdown(conProdId)
{
    childWin = window.open('ContractRechargeBreakdown.aspx?action=showtemplate&cpId=' + conProdId,'rechargebreakdown','locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');
}