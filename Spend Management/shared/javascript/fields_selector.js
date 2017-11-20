function populateForEdit(conditionDetails, action, tableID)
{
    if(action == "Select")
    {
        action = "false";
    } else {
        action = "true";
    }
    
    Spend_Management.svcWorkflows.RebuildFieldSelectorForEdit(conditionDetails, action, tableID, populateForEditComplete, populateDDError);

    return;
}

function populateForEditComplete(data) 
{
    document.getElementById("fieldselectordiv").innerHTML = data[0];
    globalRowIndex = data[1];
    addNewRow();
    return;
}




function populateConditions(dropdown)
{
    focusedConditionDD = dropdown + "_ddlCondition";
    focusedValueSpan = dropdown + "_6";
    var fieldID = document.getElementById(dropdown + "_ddlField")[document.getElementById(dropdown + "_ddlField").selectedIndex].value;
    
    if(fieldID != "0") 
    {
        if(document.getElementById(dropdown + "_ddlField").options[0].value == "0")
        {
            document.getElementById(dropdown + "_ddlField").options[0] = null;
        }

        Spend_Management.svcWorkflows.SelectableConditions(fieldID, populateConditionsComplete, populateDDError);
    }

    return;
}



function getCriteriaArray() {

    var criteria = document.getElementById('tbl').tBodies[0].rows;
    var valueControl;
    var criteriaArray = new Array();
    var rowID;

    for (var i = 0; i < criteria.length; i++) 
    {
    
        valueControl = document.getElementById(criteria[i].id + "_value");

        if (valueControl != null) { // If the value control does nto exist dont add it to the array, this will ignore the final row and any incorrectly filled out criteria
        
            criteriaArray[i] = new Array();
            criteriaArray[i][0] = document.getElementById(criteria[i].id + "_ddlField")[document.getElementById(criteria[i].id + "_ddlField").selectedIndex].value;
            criteriaArray[i][1] = document.getElementById(criteria[i].id + "_ddlCondition")[document.getElementById(criteria[i].id + "_ddlCondition").selectedIndex].value;
            
            if(criteriaArray[i][2] == "Select") {
            criteriaArray[i][2] = 1;
            } else {
            criteriaArray[i][2] = 2;
            }

            if (criteriaMode == "Select") {
                criteriaArray[i][3] = false;
            } else {
                //criteriaArray[i][3] = document.getElementById(criteria[i].id + "_ddlField").parentNode.parentNode.parentNode.childNodes[5].childNodes[0].childNodes[0].value;
                criteriaArray[i][3] = document.getElementById(criteria[i].id + "_runtime").checked;
            }

            if (valueControl.tagName == "SELECT") 
            {
                criteriaArray[i][4] = valueControl[valueControl.selectedIndex].value.replace(/"/g, '&quot;'); ;
            }
            else 
            {
                if (valueControl.type == "text") 
                {
                    criteriaArray[i][4] = valueControl.value;
                }
                else if (valueControl.type == "checkbox") 
                 {
                    criteriaArray[i][4] = valueControl.value;
                }
            }
            
            if(criteriaArray[i][1] == 8)
            {
                criteriaArray[i][5] = document.getElementById(criteria[i].id + "_value2").value;
            } 
            else 
            {
                criteriaArray[i][5] = null;    
            }

            rowID = valueControl.parentNode.parentNode.parentNode.id
        }
    }

    return criteriaArray;
}

function getCriteriaAsString()
{

    var criteria = document.getElementById('tbl').tBodies[0].rows;
    var valueControl;
    var criteriaArray = [];
    var rowID;

    var criteriaString = "";

    for (var i = 0; i < criteria.length; i++)
    {

        valueControl = document.getElementById(criteria[i].id + "_value");

        if (valueControl != null)
        { // If the value control does nto exist dont add it to the array, this will ignore the final row and any incorrectly filled out criteria

            criteriaString += " " + document.getElementById(criteria[i].id + "_ddlField")[document.getElementById(criteria[i].id + "_ddlField").selectedIndex].text;
            criteriaString += " " + document.getElementById(criteria[i].id + "_ddlCondition")[document.getElementById(criteria[i].id + "_ddlCondition").selectedIndex].text;

            if (criteriaMode != "Select")
            {
                if (document.getElementById(criteria[i].id + "_ddlField").parentNode.parentNode.parentNode.childNodes[5].childNodes[0].childNodes[0].value == true)
                {
                    criteriaString += " Runtime";
                }
            }

            if (valueControl.tagName == "SELECT")
            {
                criteriaString += " " + valueControl[valueControl.selectedIndex].value;
            }
            else
            {
                if (valueControl.type == "text")
                {
                    criteriaString += " " + valueControl.value;
                }
                else if (valueControl.type == "checkbox")
                {
                    criteriaString += " " + valueControl.value;
                }
            }

            if (parseInt(document.getElementById(criteria[i].id + "_ddlCondition")[document.getElementById(criteria[i].id + "_ddlCondition").selectedIndex]) == 8)
            {
                criteriaString += " " + document.getElementById(criteria[i].id + "_value2").value;
            }

            rowID = valueControl.parentNode.parentNode.parentNode.id;
        }
    }

    return criteriaString;
}

function addNewRow(editCirteria) {
    var tbl = document.getElementById("tbl").tBodies[0];
    var row = tbl.insertRow(tbl.rows.length);
    var span;
    var cell;
    var ddl;
    var rowIndex = globalRowIndex;

    row.id = globalRowIndex;

    cell = row.insertCell(0);
    cell.setAttribute("className", "fieldstbl");

    span = document.createElement("span");
    span.id = rowIndex + "_1";
    span.setAttribute("class", "hiddenclass");
    span.setAttribute("className", "hiddenclass");

    if (tbl.rows.length > 1) 
    {
        span.innerHTML = "<a href=\"javascript:deleteRow(" + rowIndex + ");\"><img src=\"/shared/images/icons/delete2.gif\" border=\"0\" align=\"absmiddle\" alt=\"Delete criteria\" title=\"Delete criteria\" /></a>";
    } 
    else 
    {
        span.innerHTML = "<img src=\"/shared/images/icons/delete_disabled.png\" border=\"0\" align=\"absmiddle\" alt=\"Delete criteria\" title=\"Delete criteria\" />";
    }
    
    cell.appendChild(span);

    cell = row.insertCell(1);
    cell.id = rowIndex + "_0";
    cell.onclick = function() { displayRow(rowIndex); };

    span = document.createElement("span");
    span.id = rowIndex + "_2";
    span.setAttribute("class", "visableclass");
    span.setAttribute("className", "visableclass");

    span.innerHTML = "<a href=\"javascript:void(0);\">Element</a>";

    cell.appendChild(span);
    span = document.createElement("span");
    span.id = rowIndex + "_3";
    span.setAttribute("class", "hiddenclass");
    span.setAttribute("className", "hiddenclass");

    ddl = document.createElement("select");
    ddl.id = rowIndex + "_ddlTable";
    ddl.onchange = function() { selectTable(rowIndex); };

    span.appendChild(ddl);
    cell.appendChild(span);

    var tablesDDID = ddl.id;

    cell = row.insertCell(2);
    span = document.createElement("span");
    span.id = rowIndex + "_4";
    span.setAttribute("class", "hiddenclass");
    span.setAttribute("className", "hiddenclass");

    ddl = document.createElement("select");
    ddl.id = rowIndex + "_ddlField";
    ddl.onchange = function() { selectField(rowIndex); };
    ddl.options[0] = new Option("Field", "-1");

    span.appendChild(ddl);
    cell.appendChild(span);

    cell = row.insertCell(3);
    span = document.createElement("span");
    span.id = rowIndex + "_5";
    span.setAttribute("class", "hiddenclass");
    span.setAttribute("className", "hiddenclass");

    ddl = document.createElement("select");
    ddl.onchange = function() { selectedOperatorChanged(rowIndex); };
    ddl.id = rowIndex + "_ddlCondition";
    ddl.options[0] = new Option("Operator", "0");

    span.appendChild(ddl);

    cell.appendChild(span);

    cell = row.insertCell(4);
    span = document.createElement("span");
    span.id = rowIndex + "_6";
    span.setAttribute("class", "hiddenclass");
    span.setAttribute("className", "hiddenclass");
    textNode = document.createTextNode(" "); 
    span.appendChild(textNode);
    cell.appendChild(span);

    cell = row.insertCell(5);
    span = document.createElement("span");
    span.id = rowIndex + "_7";
    textNode = document.createTextNode(" "); 
    span.appendChild(textNode);
    span.style.display = "none";
    cell.appendChild(span);
    
    if(criteriaMode == "Update")
    {
        cell.style.display = "none";
    }
    
    cell = row.insertCell(6);
    cell.align = "center";
    chk = document.createElement("input");
    chk.id = rowIndex + "_runtime";
    chk.type = "checkbox";
    chk.value = 'true';
    chk.style.display = "none";
    span = document.createElement("span");
    span.appendChild(chk);
    cell.appendChild(span);
    
    if(criteriaMode == "Select") 
    {
        cell.style.display = "none";
    }

    var baseTableID = document.getElementById(workflowTypeClientID)[document.getElementById(workflowTypeClientID).selectedIndex].value;

    if (baseTableID < 0) {
        if (baseTableID == -1) {
            baseTableID = 'D70D9E5F-37E2-4025-9492-3BCF6AA746A8';
        }
    }

    populateTables(tablesDDID, baseTableID);
    globalRowIndex++;
    return;
}

function selectedOperatorChanged(rowIndex)
{
    var selectedOperator = document.getElementById(rowIndex + "_ddlCondition")[document.getElementById(rowIndex + "_ddlCondition").selectedIndex].value;
    
    if(selectedOperator == 8)
    {
        document.getElementById(rowIndex + "_7").style.display = "";
        document.getElementById(rowIndex + "_7").innerHTML = "";
        var textbox = document.createElement("input");
        textbox.type = "text";
        textbox.size = 10;
        textbox.id = rowIndex + "_value2";
        document.getElementById(rowIndex + "_7").appendChild(textbox);
    } else {
        document.getElementById(rowIndex + "_7").style.display = "none";
        if(document.getElementById(rowIndex + "_7").getElementsByTagName("input")[0] != undefined) 
        {
            document.getElementById(rowIndex + "_7").getElementsByTagName("input")[0].value = "";
        }
    }
}

function selectTable(row) {
    var ddl = document.getElementById(row + "_ddlTable");
    var tbl = document.getElementById("tbl").tBodies[0];
    var currentRow = document.getElementById(row);
    var lastTableSelector = document.getElementById(tbl.rows[tbl.rows.length - 1].id + "_ddlTable")[document.getElementById(tbl.rows[tbl.rows.length - 1].id + "_ddlTable").selectedIndex].value;
    
    if ((tbl.rows[tbl.rows.length - 1].id != row.id) && lastTableSelector != -1) {
        addNewRow();
    }

    var action = ddl[document.getElementById(row + "_ddlTable").selectedIndex].value;

    if (ddl.options[0].value == -1) {
        ddl.remove(0);
    }
    
    populateFields(action, row);
    return;
}

function selectField(row) {
    var ddl = document.getElementById(row + "_ddlField");
    if (ddl.options[0].value == -1) {
        ddl.remove(0);
    }
    populateConditions(row)
    selectedOperatorChanged(row);
}

function deleteRow(row) {
    var tbl = document.getElementById("tbl").tBodies[0];
    if (tbl.rows.length > 1) {
        for (var i = 0; i < tbl.rows.length; i++) {
            if (tbl.rows[i].id == row) {
                tbl.deleteRow(i);
                if (i == tbl.rows.length) {
                    addNewRow();
                }
                break;
            }
        }
    }
    else {
        SEL.MasterPopup.ShowMasterPopup("You cannot delete the only row.", "Notice");
    }
    return;
}


function clearCriteria() {
    var criteria = document.getElementById('tbl').tBodies[0];
    for (i = criteria.rows.length - 1; i > -1; i--) {
        criteria.deleteRow(i);
    }
    addNewRow();
    return;
}

function CriteriaArrayToString(delimiter) {
    var criteria = getCriteriaArray();
    var criteriaString = "";
    for (var i = 0; i < criteria.length; i++) {
        criteriaString += criteria[i]["fieldID"] + delimiter + criteria[i]["condition"] + delimiter + criteria[i]["criteriaMode"] + delimiter + criteria[i]["runtime"] + delimiter + criteria[i]["value"];
        if (i < criteria.length) {
            criteriaString + delimiter;
        }
    }
    return criteriaString;
}


function populateConditionsComplete(result)
{
    var dropdown = document.getElementById(focusedConditionDD);
    var option;
    
    dropdown.options.length = 0;
    
    for(var i = 0; i < result[0].length; i++)
    {
        option = document.createElement("option");
        option.text = result[0][i].Text;
        option.value = result[0][i].Value;
        dropdown.options.add(option);    
    }
    
    var valueSpan = document.getElementById(focusedValueSpan);
    valueSpan.innerHTML = "";
    var newControl;
    var textNode;
    var rowID = valueSpan.parentNode.parentNode.id;

    var valueID = rowID + "_value";
    
    if(result[1].length == 0)
    {
        newControl = document.createElement("input");
        
        if(result[2] == "X") 
        {
            newControl = document.createElement("select");
            option = document.createElement("option");
            option.value=1;
            option.text = "Yes";
            
            newControl.options[0] = option;
            
            option = document.createElement("option");
            option.value=0;
            option.text = "No";
            
            newControl.options[1] = option;
            
            valueSpan.appendChild(newControl);
        }
        else 
        {
            if(result[2] == "D")
            {
                // date cal?
            }
            else
            {

                newControl.onkeyup = function() { autoComplete(valueID, null, result[3], 7, null, null) };
                // add autocomplete for text            
            }
            newControl.type = "text";
            newControl.size = 10;

        }
    }
    else 
    {
        newControl = document.createElement("select");
    
        for(i = 0; i < result[1].length; i++)
        {
            option = document.createElement("option");
            option.text = result[1][i].Text;
            option.value = result[1][i].Value;
            newControl.options.add(option);    
        }
    }

    newControl.id = valueID;

    valueSpan.appendChild(newControl);

    if (criteriaMode == "Update") {
            // UPDATE RUNTIME FOR THIS ROW
            document.getElementById(rowID + "_runtime").style.display = "";
    }
    
    if(textNode != undefined)
    {
        valueSpan.appendChild(textNode);
    }
    
    return;
}


function populateTablesComplete(result) 
{
    var dropdown = document.getElementById(focusedTableDD);
    var option;


    dropdown.options.length = 0;
    
    option = document.createElement("option");
    option.text = "Element";
    option.value = "-1";
    dropdown.options.add(option);    
    
    for(var i = 0; i < result.length; i++)
    {
        option = document.createElement("option");
        option.text = result[i].Description;
        option.value = result[i].TableID;
        dropdown.options.add(option);    
    }
    return;
}

function populateTables(dropdown, baseTableID)
{
  focusedTableDD = dropdown;


  Spend_Management.svcWorkflows.GetAllowedTables(baseTableID, populateTablesComplete, populateDDError);
  
  return;
} 

function displayRow(row) {
//    var spanIDs = new Array(row + '_1', row + '_2', row + '_3', row + '_4', row + '_5', row + '_6', row + '_7');
var spanIDs = new Array(row + '_1', row + '_2', row + '_3', row + '_4', row + '_5', row + '_6');

    for (var i = 0; i < spanIDs.length; i++) {
        var span = document.getElementById(spanIDs[i]);
        if (span.className == "hiddenclass") {
            span.setAttribute("class", "visableclass");
            span.setAttribute("className", "visableclass");
        }
        else {
            span.setAttribute("class", "hiddenclass");
            span.setAttribute("className", "hiddenclass");
        }
    }

    var td = document.getElementById(row + "_0");

    td.onclick = "";

    return;
}


function SetCriteriaMode(action) {
    if (action == 'Update') {
        criteriaMode = "Update";
        showRuntimeColumn("show");
        showValueTwoColumn("hide");
    } else {
        criteriaMode = "Select";
        showRuntimeColumn("hide");
        showValueTwoColumn("show");
    }
}

function populateFields(tableID, dropdown)
{
    focusedFieldDD = dropdown + "_ddlField";

    var conditions = document.getElementById(dropdown + "_ddlCondition");

    conditions.options.length = 0;

    conditions.options[0] = new Option("Operator", "0");

    var values = document.getElementById(dropdown + "_6");

    values.innerHTML = "&nbsp;";
    var isUpdate = false;
    if (criteriaMode == "Update") {
        isUpdate = true;
    }

    Spend_Management.svcWorkflows.GetTableFields(tableID, isUpdate, populateFieldsComplete, populateDDError);
    return;
}


function populateFieldsComplete(result)
{
    var dropdown = document.getElementById(focusedFieldDD);

    var option;

    dropdown.options.length = 0;

    option = document.createElement("option");
    option.text = "Field";
    option.value = "-1";
    dropdown.options.add(option);

    for (var i = 0; i < result.length; i++)
    {
        option = document.createElement("option");
        option.text = result[i].Description;
        option.value = result[i].FieldID;
        dropdown.options.add(option);
    }

    return;
}

function clearTable() {
    var tbl = document.getElementById("tbl").tBodies[0];

    for (var i = 0; i < tbl.rows.length; i++) {
        tbl.deleteRow(i);
    }

    addNewRow();
}

function populateDDError(result)
{
    SEL.MasterPopup.ShowMasterPopup(result._message, "Error");
    return;
}

function populateValues(cell)
{

}

function showRuntimeColumn(showOrHide) 
{
    var tbl = document.getElementById('tbl');

    for (var i = 0 ; i < tbl.rows.length; i++) {
        if (tbl.rows[i].childNodes[6] != undefined && tbl.rows[i].childNodes[6] != null) 
        {
            if (showOrHide == "hide") 
            {
                if(tbl.rows[i].childNodes[6].style != undefined) 
                {
                    tbl.rows[i].childNodes[6].style.display = "none";
                }
            } 
            else 
            {
                if(tbl.rows[i].childNodes[6].style != undefined) 
                {
                    tbl.rows[i].childNodes[6].style.display = "";
                }
            }
        }
    }
return;
}

function showValueTwoColumn(showOrHide) 
{
    var tbl = document.getElementById('tbl');

    for (var i = 0 ; i < tbl.rows.length; i++) {
        if (tbl.rows[i].childNodes[5] != undefined && tbl.rows[i].childNodes[5] != null) 
        {
            if (showOrHide == "hide") 
            {
                tbl.rows[i].childNodes[5].style.display = "none";
            } 
            else 
            {
                tbl.rows[i].childNodes[5].style.display = "";
            }
        }
    }
return;
}
