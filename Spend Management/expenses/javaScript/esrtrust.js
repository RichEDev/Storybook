function showModal() {
    $find(modalClientID).show();
}

function hideModal() {

    $find(modalClientID).hide();
    activeTrustID = null;
}

function Trust()
{
    this.trustID = null;
    this.trustName = null;
    this.trustVPD = null;
    this.periodRun = null;
    this.periodType = null;
    this.FTPAddress = null;
    this.FTPUsername = null;
    this.FTPPassword = null;
    this.RunSequenceNumber = null;
    this.DelimiterCharacter = null;
    this.EsrInterfaceVersionNumber = null;
}

function NewTrust() 
{
    trustName.value = "";
    trustVPD.value = "";
    periodType.options[0].selected = true;
    periodRun.options[0].selected = true;
    FTPAddress.value = "";
    FTPUsername.value = "";
    FTPPassword.value = "";
    RunSequenceNumber.value = 1;
    DelimiterCharacter.value = "";
    ESRInterfaceVersion.options[0].selected = true;
    ESRInterfaceVersion.disabled = false;
    showModal();
}

function EditTrust(trustID) {
    activeTrustID = trustID;
    Spend_Management.ESR.GetESRTrustByID(trustID, EditTrustCallBack, AjaxError);
}

function EditTrustCallBack(data) {
    trustName.value = data.TrustName;
    trustVPD.value = data.TrustVPD;
    SetDropDownListValue(periodRun, data.PeriodRun);
    SetDropDownListValue(periodType, data.PeriodType);
    FTPAddress.value = data.FTPAddress;
    FTPUsername.value = data.FTPUsername;
    FTPPassword.value = data.FTPPassword;
    RunSequenceNumber.value = data.RunSequenceNumber;
    DelimiterCharacter.value = data.DelimiterCharacter;
    SetDropDownListValue(ESRInterfaceVersion, data.EsrInterfaceVersionNumber);
    Spend_Management.ESR.CheckForImportMappings(activeTrustID, CheckForImportMappingsComplete, AjaxError);
}

function CheckForImportMappingsComplete(status)
{
    ESRInterfaceVersion.disabled = status;
    showModal();
}

function SaveTrust() {
    if (validateform(null) === false) { return; }

    var trust = new Trust();
    if (activeTrustID !== null) {
        trust.trustID = activeTrustID;
    }
    else {
        trust.trustID = 0;
    }
    trust.trustName = trustName.value;
    trust.trustVPD = trustVPD.value;
    trust.periodRun = periodRun.value;
    trust.periodType = periodType.value;
    trust.FTPAddress = FTPAddress.value;
    trust.FTPUsername = FTPUsername.value;
    trust.FTPPassword = FTPPassword.value;
    trust.RunSequenceNumber = RunSequenceNumber.value;
    trust.DelimiterCharacter = DelimiterCharacter.value;
    trust.EsrInterfaceVersionNumber = ESRInterfaceVersion.value;
    Spend_Management.ESR.SaveNHSTrust(trust, SaveTrustCallBack, AjaxError);
}

function SaveTrustCallBack(data)
{
    if (data[0] === -1)
    {
        SEL.MasterPopup.ShowMasterPopup("The Trust Name you have entered already exists.");
    }
    else
        if (data[0] === -2) {
            SEL.MasterPopup.ShowMasterPopup("The Trust VPD you have entered already exists.");
        }
        else
        {
            hideModal();
            SEL.Grid.refreshGrid('trusts', SEL.Grid.currentPageNum['trusts']);

            //if (data[1] === 1)
            //{
            //    SEL.MasterPopup.ShowMasterPopup("The ESR Trust Version used by the trust has changed. Import Template Mappings require editing and updating for imports to succeed.");
            //}
        }
}

function AjaxError(data) {
    SEL.MasterPopup.ShowMasterPopup("An error has occurred");
}

function DeleteTrust(trustID)
{
    if (confirm("Are you sure you want to delete this trust?"))
    {
        Spend_Management.ESR.DeleteNHSTrust(trustID, DeleteTrustComplete);
    }
}

function DeleteTrustComplete(retVal)
{
    switch (retVal)
    {
        case 0:
            SEL.Grid.refreshGrid('trusts', SEL.Grid.currentPageNum['trusts']);
            break;
        case 1:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete the ESR Trust as it is associated to a financial export.");
            break;
        default:
            break;
    }
}

function ArchiveTrust(trustID) {
    Spend_Management.ESR.ArchiveTrust(trustID);
    currentRowID = trustID;
    
    var cell = SEL.Grid.getCellById('trusts', currentRowID, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') !== -1)
    {
        cell.innerHTML = "<a href='javascript:ArchiveTrust(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.gif'></a>";
    }
    else
    {
        cell.innerHTML = "<a href='javascript:ArchiveTrust(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}

function goToMappings(trustID) {
    window.location = "aeESRElementMapping.aspx?trustID=" + trustID;
}
