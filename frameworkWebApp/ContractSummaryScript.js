// JScript File
var curContractId;
var CP_Count;
var bIsVariation;
var nActiveCP;

function GoHome()
{
    document.location = 'Home.aspx';
}

function DelInvForecastRec(InvForecastId,contractId)
{
    var sPrompt = 'Click OK to confirm deletion of the selected forecast';
    
    if(confirm(sPrompt))
    {
        SetContractId(contractId);
        PageMethods.DeleteForecast(InvForecastId, contractId, fnDelIFComplete);
    }
}

function fnDelIFComplete(strResult)
{
    if(strResult == "OK")
    {
        document.location = "ContractSummary.aspx?tab=4&id=" + curContractId;
    }
    else
    {
        alert(strResult);
    }
    return true;
}

function DeleteInvoice(InvId, contractId)
{
    var sPrompt = 'Click OK to confirm deletion of the selected invoice';
    
    if(confirm(sPrompt))
    {
        SetContractId(contractId);
        PageMethods.DeleteInvRec(InvId, contractId, fnDelInvComplete);
    }
}

function fnDelInvComplete(strResult)
{
    if(strResult == "OK")
    {
        document.location.href = "ContractSummary.aspx?tab=3&id=" + curContractId;
    }
    else
    {
        alert(strResult);
    }
    return true;
}

function DeleteConProd(conProdId)
{
    var sPrompt = 'Click OK to confirm deletion of the contract product';
    
    if(confirm(sPrompt))
    {
        PageMethods.DeleteContractProduct(conProdId,fnDeleteCPComplete, commandFail);
    }
}

function commandFail(error)
{
    if (error["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}

function fnDeleteCPComplete(strResult)
{
    if(strResult == "OK")
    {
        document.location.href = "ContractSummary.aspx?tab=2&id=" + curContractId;
    }
    else
    {
        alert(strResult);
    }
    return true;
}
        
function DeleteContract(contractId)
{
    var sPrompt = 'Click OK to confirm deletion of this contract';
    
    if(bIsVariation == true)
    {
        sPrompt = 'Click OK to confirm deletion of this variation';
    }
    
    //alert('curContractId = ' + curContractId);
    if(confirm(sPrompt))
    {
        //alert('redirecting...');
        PageMethods.DeleteContract(contractId,fnDeleteComplete);
    }
}

function fnDeleteComplete(strResult)
{
    //alert(strResult);
    if(strResult.substring(0,5) != "ERROR")
    {
        document.location = 'Home.aspx';
    }
    else {
        alert(strResult);
    }
    return true;
}

function SetContractId(conId)
{
    curContractId = conId;
}

function SetIsVariation(bVal)
{
    bIsVariation = bVal;
}

function GetCPDetail(CPId, activeContractId) {
    nActiveCP = CPId;
    if (activeContractId === undefined || activeContractId === null || activeContractId.length <= 0) {
        activeContractId = CPId;
    }
    maintParams = document.getElementById("MaintParams")
    PageMethods.GetCPDetail(CPId, fnGetCPDetailComplete, activeContractId, maintParams);
}

function fnGetCPDetailComplete(strResult)
{
    var CPpanel = document.getElementById('broadcastmsg');
    if(CPpanel != null)
    {
        CPpanel.style.display = 'block';
        CPpanel.innerHTML = strResult;
    }
    return true;
}

function SetCPArchiveStatus(contractId, CPId, CPStatus)
{
    SetContractId(contractId);
    PageMethods.SetCPStatus(CPId, CPStatus, fnCPStatusComplete);
}

function fnCPStatusComplete(strResult)
{
    if(strResult == "OK")
    {
        document.location = 'ContractSummary.aspx?tab=2&id=' + curContractId;
    }
    else
    {
        alert(strResult);
    }
    return true;
}




