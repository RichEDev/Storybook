/*
**  JS for Cost Centre Panel
*/

function ccbShowCostCentreBreakdown(itemId)
{
    if (itemId != null && itemId != 0 && ccbItemArray[itemId] != null)
    {
        ccbItemId = itemId;
    }
    else
    {
        ccbItemId = itemId;
    }
    
    DeleteCostCentreBreakdownRow();
    PopulateWithDefaultBreakdown();
    
    return;
}

function ccbAddCostCentreBreakdownRow(dep, cc, pc, perc)
{
    var tbl = document.getElementById(ccbTableId).tBodies[0],
        existingLastRow = $("tr:last-of-type>td:first", tbl).data("rownumber"),
        newRow = tbl.insertRow(-1),
        newCell,
        rowNumber = existingLastRow === null ? 0 : existingLastRow + 1;

    newCell = newRow.insertCell(-1);
    newCell.className = ccbRowClass;
    $(newCell).data("rownumber", rowNumber);
    newCell.innerHTML = ccbReadOnly
        ? "<img src=\"/shared/images/icons/delete2.png\" style=\"visibility: hidden;\" />"
        : "<img src=\"/shared/images/icons/delete2.png\" onclick=\"DeleteCostCentreBreakdownRow(this);\" style=\"cursor: pointer; cursor: hand;\" />";


    if (ccbUseCostCentres === true)
    {
        if (ccbUseDepartments === true)
        {
            newCell = newRow.insertCell(-1);
            newCell.className = ccbRowClass;
            newCell.appendChild(ccbCopyCellControls(ccbddlDepartmentsId, rowNumber, dep, (ccbAutoCompleteEntries.departments.hasOwnProperty("" + dep) ? ccbAutoCompleteEntries.departments["" + dep] : "")));
        }
        
        if (ccbUseCostCodes === true)
        {
            newCell = newRow.insertCell(-1);
            newCell.className = ccbRowClass;
            newCell.appendChild(ccbCopyCellControls(ccbddlCostCodesId, rowNumber, cc, (ccbAutoCompleteEntries.costcodes.hasOwnProperty("" + cc) ? ccbAutoCompleteEntries.costcodes["" + cc] : "")));
        }
        
        if (ccbUseProjectCodes === true)
        {
            newCell = newRow.insertCell(-1);
            newCell.className = ccbRowClass;
            newCell.appendChild(ccbCopyCellControls(ccbddlProjectCodesId, rowNumber, pc, (ccbAutoCompleteEntries.projectcodes.hasOwnProperty("" + pc) ? ccbAutoCompleteEntries.projectcodes["" + pc] : "")));
        }
        
        newCell = newRow.insertCell(-1);
        newCell.className = ccbRowClass;
        newCell.innerHTML = "<input class=\"ccbpercent\" type=\"text\" maxlength=\"3\" style=\"width: 30px;\" onblur=\"CalculateCostCentreBreakdownPercentage(this);\" value=\"" + perc + "\"" + (ccbReadOnly ? " disabled=\"disabled\"" : "") + " />";
    }

    // alternate the row class
    ccbRowClass = (ccbRowClass === "row1") ? "row2" : "row1";

    return;
}

function ccbCopyCellControls(controlSetSourceId, rowNumber, valueId, valueText)
{
    var controlsContainer = document.getElementById(controlSetSourceId).cloneNode(true),
        comboSelect = $(">.autocompletecombo-select", controlsContainer),
        comboTextBox = $(">.autocompletecombo-text", controlsContainer),
        comboTextBoxId = $(">.autocompletecombo-id", controlsContainer),
        comboSearchIcon = $(">.autocompletecombo-icon", controlsContainer);
    
    // update control ids
    controlsContainer.id = controlsContainer.id + "_" + rowNumber;
    comboSelect.attr("id", comboSelect.attr("id") + "_" + rowNumber);
    comboTextBox.attr("id", comboTextBox.attr("id") + "_" + rowNumber);
    comboTextBoxId.attr("id", comboTextBox.attr("id") + "_ID");
    comboSearchIcon.attr("id", comboSearchIcon.attr("id") + "_" + rowNumber);

    if (comboSelect[0].options.length === 0)
    {
        var scriptblock = document.createElement("script");
        try
        {
            scriptblock.innerHTML = "$(document).ready(function(){" + comboTextBox.data("jsbind").replace("'##CONTROLID##'", "'" + comboTextBox.attr("id") + "'") + "});";
        }
        catch(e)
        {
            scriptblock.text = "$(document).ready(function(){" + comboTextBox.data("jsbind").replace("'##CONTROLID##'", "'" + comboTextBox.attr("id") + "'") + "});";
        }
        controlsContainer.appendChild(scriptblock);
    }

    if (isFinite(valueId))
    {
        comboTextBoxId.val(valueId);
        
        if (comboSelect[0].options.length > 0)
        {
            comboSelect.val(valueId);
        }
        else if (typeof valueText === "string" && valueText !== "")
        {
            comboTextBox.val(valueText);
        }
    }

    return controlsContainer;
}

function ccbGetBreakdownRows(purchaseOrderProductId)
{
    // give them the array entry or return default single line
    var row = [];
    if (purchaseOrderProductId != null && purchaseOrderProductId > 0 && ccbItemArray[purchaseOrderProductId] != null)
    {
        row = ccbItemArray[purchaseOrderProductId];
    }
    else
    {
        row.push(Array(0, 0, 0, 100));
    }
    return row;
}

function DeleteCostCentreBreakdownRow(oTdChild)
{
    // oTdChild should be an element within a table cell in the row
    // "null" deletes all rows
    var tbl = document.getElementById(ccbTableId).tBodies[0];
    if (oTdChild != null)
    {
        tbl.deleteRow(oTdChild.parentNode.parentNode.rowIndex);
        if (tbl.rows.length < 2)
        {
            ccbAddCostCentreBreakdownRow(0, 0, 0, 100);
        }
        ccbRowClass = ccbRowClassUpdate(); // recolours the rows and then resets the global rowclass variable
    }
    else if (oTdChild == null)
    {
        //for (var i = 1; i < tbl.rows.length; i++)
        for (var i = (tbl.rows.length - 1); i > 0; i--)
        {
            tbl.deleteRow(i);
        }
        ccbRowClass = "row1";
    }
    return;
}

function ccbRowClassUpdate()
{
    var tbl = document.getElementById(ccbTableId).tBodies[0],
        rc = "row1";
    // start at 1 to skip header
    for (var i = 1; i < tbl.rows.length; i++)
    {
        for (var j = 0; j < tbl.rows[i].cells.length; j++)
        {
            tbl.rows[i].cells[j].className = rc;
        }
        // alternate the row class name
        rc = (rc == "row1") ? "row2" : "row1";
    }
    return rc; // passes back the class name for next row
}

function ccbAddItemArray(prodId, costCentreBreakdown)
{
    var i;
    // skip if something at this array point
    if (ccbItemArray[prodId] === null)
    {
        ccbItemArray[prodId] = [];
        if (costCentreBreakdown != null && costCentreBreakdown.length > 0)
        {
            for (i = 0; i < costCentreBreakdown.length; i++)
            {
                ccbItemArray[prodId].push([
                    (costCentreBreakdown[i].Department == null ? 0 : costCentreBreakdown[i].Department.departmentid),
                    (costCentreBreakdown[i].CostCode == null ? 0 : costCentreBreakdown[i].CostCode.costcodeid),
                    (costCentreBreakdown[i].ProjectCode == null ? 0 : costCentreBreakdown[i].ProjectCode.projectcodeid),
                    (costCentreBreakdown[i].PercentSplit == null ? 0 : costCentreBreakdown[i].PercentSplit)
                ]);
            }
        }
        else if (ccbEmployeeDefaults.length > 0)
        {
            for (i = 0; i < ccbEmployeeDefaults.length; i++)
            {
                ccbItemArray[prodId].push([
                    (ccbEmployeeDefaults[i][0] == null ? 0 : ccbEmployeeDefaults[i][0]),
                    (ccbEmployeeDefaults[i][1] == null ? 0 : ccbEmployeeDefaults[i][1]),
                    (ccbEmployeeDefaults[i][2] == null ? 0 : ccbEmployeeDefaults[i][2]),
                    (ccbEmployeeDefaults[i][3] == null ? 0 : ccbEmployeeDefaults[i][3])
                ]);
            }
        }
        else
        {
            // get the first item in the controls if present on the page
            if (ccbUseCostCentres === true)
            {
                ccbItemArray[prodId].push([
                    (ccbUseDepartments === true ?  document.getElementById(ccbddlDepartmentsId).options[0].value : 0),
                    (ccbUseCostCodes === true ? document.getElementById(ccbddlCostCodesId).options[0].value : 0),
                    (ccbUseProjectCodes === true ? document.getElementById(ccbddlProjectCodesId).options[0].value : 0),
                    100
                ]);
            }
            else
            {
                ccbItemArray[prodId].push([0, 0, 0, 100]);
            }
        }
    }
    return;
}

function PopulateWithDefaultBreakdown()
{
    var i;
    // pick up the items from the appropriate itemid if present
    if (ccbItemArray.length > 0 && ccbItemId > 0 && ccbItemArray[ccbItemId] != null)
    {
        var itemBreakdown = ccbItemArray[ccbItemId];
        for (i = 0; i < itemBreakdown.length; i++)
        {
            ccbAddCostCentreBreakdownRow(itemBreakdown[i][0], itemBreakdown[i][1], itemBreakdown[i][2], itemBreakdown[i][3]);
        }
    }
    else if (ccbEmployeeDefaults.length > 0)
    {
        for (i = 0; i < ccbEmployeeDefaults.length; i++)
        {
            ccbAddCostCentreBreakdownRow(ccbEmployeeDefaults[i][0], ccbEmployeeDefaults[i][1], ccbEmployeeDefaults[i][2], ccbEmployeeDefaults[i][3]);
        }
    }
    else
    {
        ccbAddCostCentreBreakdownRow(0, 0, 0, 100);
    }
    return;
}

function CalculateCostCentreBreakdownPercentage(oPercentInput)
{
    var tbl = document.getElementById(ccbTableId).tBodies[0],
        i;
    if (oPercentInput != null)
    {
        // are we editing the last row? rowIndex is 0 indexed, length is 1 indexed
        var lastrow = (oPercentInput.parentNode.parentNode.rowIndex == (tbl.rows[tbl.rows.length - 1].rowIndex)) ? true : false;

        // delete rows without numbers in or those that are 0
        if (isNaN(Math.round(oPercentInput.value)) || Math.round(oPercentInput.value) == 0)
        {
            DeleteCostCentreBreakdownRow(oPercentInput);
        }
        else
        {
            // disallow negatives
            if (Math.round(oPercentInput.value) < 0)
            {
                oPercentInput.value = Math.abs(Math.round(oPercentInput.value));
            }

            var totalPercent = 0,
                pgeCells = $(".ccbpercent", tbl); // array of the percentage inputs
            // total up the percentage fields of all rows
            for (i = 0; i < pgeCells.length; i++)
            {
                totalPercent = totalPercent + Math.round(pgeCells[i].value);
            }
            
            // round down if too big, do nothing if too small unless its the last row in which case create a new row
            if (totalPercent == 100)
            {
                oPercentInput.value = Math.round(oPercentInput.value);
            }
            else if (totalPercent > 100)
            {
                oPercentInput.value = Math.round(oPercentInput.value) - (totalPercent - 100);
            }
            else
            {
                oPercentInput.value = Math.round(oPercentInput.value);
                if (lastrow == true)
                {
                    ccbAddCostCentreBreakdownRow(0, 0, 0, (100 - totalPercent));
                }
            }
            return;
        }
    }
    return;
}

function ccbTotalPercentage()
{
    var tbl = document.getElementById(ccbTableId).tBodies[0],
        totalPercent = 0,
        pgeCells = $(".ccbpercent", tbl); // array of the percentage inputs
    // total up the percentage fields of all rows
    for (var i = 0; i < pgeCells.length; i++)
    {
        totalPercent = totalPercent + Math.round(pgeCells[i].value);
    }
    return totalPercent;
}

function ccbSaveBreakdown()
{
    if (ccbUseCostCentres === true)
    {
        var i,
            lastCellId,
            rowValues,
            dValue,
            ccValue,
            pcValue,
            tbl = document.getElementById(ccbTableId).tBodies[0];
        
        if (ccbTotalPercentage() !== 100)
        {
            SEL.MasterPopup.ShowMasterPopup("The cost centre breakdown percentages must total 100", "Validation Failed");
            return false;
        }

        //clear current items for this id
        ccbItemArray[ccbItemId] = [];
        
        for (i = 1; i < tbl.rows.length; i++)
        {
            lastCellId = tbl.rows[i].cells.length - 1;
            rowValues = $(".autocompletecombo-id", tbl.rows[i]).get();
            pcValue = (ccbUseProjectCodes ? parseInt(rowValues.pop().value) : 0);
            ccValue = (ccbUseCostCodes ? parseInt(rowValues.pop().value) : 0);
            dValue = (ccbUseDepartments ? parseInt(rowValues.pop().value) : 0);
            
            ccbItemArray[ccbItemId].push([
                isFinite(dValue) && dValue > 0 ? dValue : 0,
                isFinite(ccValue) && ccValue > 0 ? ccValue : 0,
                isFinite(pcValue) && pcValue > 0 ? pcValue : 0,
                parseInt(tbl.rows[i].cells[lastCellId].getElementsByTagName('input')[0].value)
            ]);
        }
    }

    return true;
}

function ccbOpen(ccbItemArrayIndex)
{
    ccbShowCostCentreBreakdown(ccbItemArrayIndex);
    $find(ccbModalId).show();
    return;
}

function ccbSave()
{
    var succeeded = ccbSaveBreakdown();
    if (succeeded === true) { $find(ccbModalId).hide(); }
    return;
}

function ccbClose()
{
    $find(ccbModalId).hide();
    return;
}





// copied from RelationshipTextBoxOnPageLoad()
function CostCentreBreakdownOnPageLoad()
{
    if (CostCentreBreakdownOnLoadArray == undefined)
    {
        return;
    }

    for (var i = 0; i < CostCentreBreakdownOnLoadArray.length; i++)
    {
        var tmpArray = CostCentreBreakdownOnLoadArray[i];
        CheckCostCentreBreakdown(null, tmpArray[0], tmpArray[1], tmpArray[2], 'onkeyup', tmpArray[3], tmpArray[4]);
    }
}
// copied from checkRelationshipTextbox(evt, txtBox, pnl, hdn, action, fieldID, additionalOnMatchFunc)
function CheckCostCentreBreakdown(evt, txtBox, pnl, hdn, action, fieldID, additionalOnMatchFunc)
{
    return true;
}
