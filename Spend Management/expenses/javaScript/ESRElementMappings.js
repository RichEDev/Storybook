/*
** Functions
*/

// Take the data from the relevant fields
function SaveESRElementMapping()
{
    // validate
    if (validateform() == false) { return; }
    
    TrustID = parseInt(TrustID);
    var elementDropDown = document.getElementById(ElementDDLID);
    var globalESRElementID = parseInt(elementDropDown.value);
    
    var fieldsPanel = document.getElementById(FieldsPanelID);
    var ESRFields = Array();
    for (i = 0; i < fieldsPanel.childNodes.length; i++)
    {
        if (fieldsPanel.childNodes[i].firstChild !== null)
        {
            var temp = Array();
            var fieldId = parseInt(fieldsPanel.childNodes[i].getElementsByTagName('input')[0].value);
            var reportColumn = fieldsPanel.childNodes[i].getElementsByTagName('select')[0].value;
            if (reportColumn != "0") // check its not default blank value
            {
                temp.push(fieldId);
                temp.push(reportColumn);
        
                ESRFields.push(temp);
            }
        }
    }
    
    var subCatsPanel = document.getElementById(SubCatsPanelID);
    var subCatsArray = Array();
    for (i = 0; i < subCatsPanel.childNodes.length; i++)
    {
        if (subCatsPanel.childNodes[i].firstChild !== null)
        {
            var subCatInputs = subCatsPanel.childNodes[i].getElementsByTagName('input');
            var temp = Array();
            for (j = 0; j < subCatInputs.length; j = j + 2)
            {
                if (subCatInputs[j].checked == true && subCatInputs[j + 1] != undefined && subCatInputs[j + 1] != null)
                {
                    subCatsArray.push(parseInt(subCatInputs[j + 1].value));
                }
        }
        }

    }
    if (subCatsArray.length == 0)
    {
        SEL.MasterPopup.ShowMasterPopup("You must select at least one Expense Item Type", "Page Validation Failed");
        return;
    }

    Spend_Management.ESR.SaveESRElementMapping(TrustID, ElementID, globalESRElementID, ESRFields, subCatsArray, SaveESRElementMappingComplete, errorMessage);
    return;
}
function SaveESRElementMappingComplete(data)
{
    //window.location = '/expenses/NHS/ESRElementMappings.aspx?trustid=' + TrustID;
    window.location = 'ESRElementMappings.aspx?trustid=' + TrustID;
    return;
}


// Used by the close button and cancel
function CancelESRElementMapping()
{
    //window.location = '/expenses/NHS/ESRElementMappings.aspx?trustid=' + TrustID;
    window.location = 'ESRElementMappings.aspx?trustid=' + TrustID;
}

// Delete the row in cGridNew and remove item mapping from database
function DeleteElementMapping(elementID, trustID)
{
    if (confirm("Are you sure you wish to delete this Element Mapping?"))
    {
        Spend_Management.ESR.DeleteElementMapping(elementID, trustID, DeleteElementMappingComplete, errorMessage);
    }
    return;
}
function DeleteElementMappingComplete(data)
{
    if (data > 0)
    {
        var tbl = document.getElementById('tbl_gridESRElementSummary');
        for (var i = 1; i < tbl.rows.length; i++)
        {
            if (tbl.rows[i].id == "tbl_gridESRElementSummary_" + data)
            {
                tbl.deleteRow(i);
                break;
            }
        }
    }
    return;
}



function errorMessage(data)
{
    if (data["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"], "Web Service Error");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, "Web Service Error");
    }
    return;
}
