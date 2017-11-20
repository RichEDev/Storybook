// page colours
var evenStepColour = '#ecf0fc'; // "#f9fbff"; //  "#C4D5E7";
var oddStepColour = '#ffffff'; // "#9BBFE2";
var selectedStepColour = '#ffffcc';
var errorColour = '#cc9999'; // '#b6677b';
// step tracking variables
var selectedStep = -1;
var insertBeforeStep = -1;
// page condition variables
var saving = false; // true once a save has started asynchronously
//var fromSaveButton = false; // true when user clicks on the page's save button, false at all other times
var checkGenDetails = true; // true when the general details need to be validated
var workflowHasChanged = false; // true when anything gets edited or removed on the page
var hasSelectedStep = false; // is one of the step divs selected / highlighted
var validatedSteps; // used to hold the steps from the page for validation before saving

var stepsCounter = 0;
var reqFields;
var reqFieldCount = 0;
var testing = true;
var CurrentStep = undefined; // tracks the currently edited step

var invalidFields;
var dragPanels = [];
var currentWorkflowGridRow;
var inEditMode = false;

if (currentSteps === undefined) {
    var currentSteps = [];
}

var stepActionTypes = [];
stepActionTypes[1] = "Approval";
stepActionTypes[2] = "Change Value";
stepActionTypes[3] = "Check Condition";
stepActionTypes[4] = "Decision";
stepActionTypes[5] = "Move To Step";
stepActionTypes[6] = "Run Sub Workflow";
stepActionTypes[7] = "SendEmail";
stepActionTypes[8] = "Else Condition";
stepActionTypes[9] = "Finish Workflow";
stepActionTypes[10] = "Else Otherwise";
stepActionTypes[11] = "Create Dynamic Value";
stepActionTypes[12] = "Decision False";
stepActionTypes[13] = "Show Message";
stepActionTypes[14] = "Change Form";

function addStepsToStepsInEdit() {
    if (currentSteps.length > 0) {
        deleteStepsHelp();
    }
    for (var x = 0; x < currentSteps.length; x++) {
        var step;
        if (currentSteps[x] != undefined) {
            if (currentSteps[x]["parentStepID"] > -1) {
                step = "step_" + (currentSteps[x]["parentStepID"]);
            } else {
                step = -1;
            }
            addStepToSteps(step, "<b>" + stepActionTypes[currentSteps[x]["action"]] + "</b><br />" + currentSteps[x]["description"]); // GetStepDescription(currentSteps[x]));// 
        }
    }
}

function SetWorkflowChanged() 
{
    workflowHasChanged = true;
    checkGenDetails = true;
    return; 
}

function toggleUserControl(action)
{
    var div = document.getElementById('fieldSelDiv');
    
    if(action == "hide")
    {
        div.setAttribute("class", "hiddendiv");
        div.setAttribute("className", "hiddendiv");    
    } 
    else if(action == "show")
    {
        div.setAttribute("class", "visablediv");
        div.setAttribute("className", "visablediv");    
    } 
    else if (div.className == "visabletd")
    {
        div.setAttribute("class", "hiddendiv");
        div.setAttribute("className", "hiddendiv");
    }
    else 
    {
        div.setAttribute("class", "visablediv");
        div.setAttribute("className", "visablediv");
    }
    
    return;
}


function ColourSteps()
{

        var allDivs = document.getElementsByTagName('div');
        
        var j = 0;
    	for(var i = 0; i < allDivs.length; i++) 
    	{
    	    if(allDivs[i].id.substr(0, 5) == "step_")
    	    {
    	        if(j%2) 
    	        {
    	            allDivs[i].style.backgroundColor = evenStepColour;
    	            //allDivs[i].setAttribute("class", "row1");
    	            //allDivs[i].setAttribute("className", "row1");
    	        }
    	        else 
    	        {
    	            allDivs[i].style.backgroundColor = oddStepColour;
    	            //allDivs[i].setAttribute("class", "row2");
    	            //allDivs[i].setAttribute("className", "row2");
    	        }
    	        j++;
    	    }
    	}
}

function populateApproverSelectComplete(results)
{
    var ddl = document.getElementById('approverSelect');
    var option;
    ddl.options.length = 0;
    
    if(results.length == 0)
    {
        ddl.style.display = "none";
    } 
    else 
    {
        ddl.style.display = "";
        for(var i = 0; i < results.length; i++) 
        {
            option = document.createElement("option");
            option.value = results[i].Value;
            option.text = results[i].Text;
            option.selected = results[i].Selected;
            ddl.options[i] = option;
        }    
    }

    return;    
}


function errorGettingString()
{
    saving = false;
    SEL.MasterPopup.ShowMasterPopup("An error occured whilst saving the workflow, no updates have been made to the existing workflow due to this error.", "Error");
}

function populateSpecificDetails(data)
{
    if(data !== undefined) {
            document.getElementById('specificDetails').innerHTML = data[3];
        
            if(parseInt(data[0]) == 5) 
            {
                var ddl = document.getElementById('ddlSelectedMoveToStep');
                ddl.options.length = 0;
                var option;
                for(var i = 0; i < currentSteps.length; i++) 
                {
                    if(currentSteps[i] != null) 
                    {
                        if(currentSteps[i]["divID"] != selectedStep)
                        {
                            option = document.createElement("option");
                            //option.value = currentSteps[i]["divID"];
                            option.value = i;
                            option.text = currentSteps[i]["description"];
                            
                            if(option.text != "" && option.value != "") 
                            {
                                ddl.options[ddl.options.length] = option;
                            }
                        }
                    }
                }

            }

            if (data[2] !== undefined && (data[2] === true || data[2] === "True")) {
                populateStepForEdit();
            }
        }
        return;
}

function populateStepForEdit() {
    var stepID = selectedStep.substr(5);
    var step = currentSteps[stepID];
    document.getElementById("txtStepDescription").value = step["description"];
    var baseTableID = document.getElementById(workflowTypeClientID)[document.getElementById(workflowTypeClientID).selectedIndex].value;

    var stepActions = document.getElementById(ddlStepAction);
    var i;
    for (i = 0; i < stepActions.options.length; i++) {
        if (stepActions.options[i].value == step["action"]) {
            stepActions.options[i].selected = true;
            break;
        }
    }

    var ddl;


    switch (parseInt(step["action"])) {
        case 1: // approval
            var approverType = document.getElementById("approvalType");

            for (i = 0; i < approverType.options.length; i++) {
                if (approverType.options[i].value == step["approverType"]) {
                    approverType.options[i].selected = true;
                    break;
                }
            }

            populateApproverSelect(step["actionID"]);

            if (step["approvalEmailTemplateID"] != null) {
                var emailTemplates = document.getElementById("emailTemplate");

                if (emailTemplates !== undefined)  // This will not exist if there are no valid email templates for the base table of this workflow
                {
                    for (i = 0; i < emailTemplates.options.length; i++) {
                        if (emailTemplates.options[i].value == parseInt(step["approvalEmailTemplateID"])) {
                            emailTemplates.options[i].selected = true;
                            break;
                        }
                    }
                }
            }

            document.getElementById("txtMessage").value = step["message"];

            if (baseTableID == "d70d9e5f-37e2-4025-9492-3bcf6aa746a8") {
                document.getElementById("oneclicksignoff").checked = step["oneclicksignoff"];
                document.getElementById("approvalShowDeclaration").checked = step["showdeclaration"];

                if (step["showdeclaration"] === true) {
                    showapprovaldeclaration();
                    document.getElementById("txtQuestion").value = step["question"];
                    document.getElementById("txtTrueButton").value = step["trueAnswer"];
                    document.getElementById("txtFalseButton").value = step["falseAnswer"];
                }
            }

            break;
        case 2: // change value 
            populateForEdit(step["conditions"], "Update", baseTableID);
            toggleUserControl("show");
            break;
        case 3: // check condition
        case 8: // else condition
            populateForEdit(step["conditions"], "Select", baseTableID);
            toggleUserControl("show");
            break;
        case 10: // else otherwise
            break;
        case 4: // decision
            document.getElementById("txtQuestion").value = step["question"];
            document.getElementById("txtTrueButton").value = step["trueAnswer"];
            document.getElementById("txtFalseButton").value = step["falseAnswer"];
            break;
        case 5: // move to step
            ddl = document.getElementById('ddlSelectedMoveToStep');
            for (i = 0; i < ddl.options.length; i++) {
                if (ddl.options[i].value == step["actionID"]) {
                    ddl.options[i].selected = true;
                    break
                }
            }
            break;
        case 6: // run sub workflow 
            ddl = document.getElementById('ddlSelectedRunSubWorkflows');
            for (i = 0; i < ddl.options.length; i++) {
                if (ddl.options[i].value == step["actionID"]) {
                    ddl.options[i].selected = true;
                    break
                }
            }
            break;
        case 7: // send email
            ddl = document.getElementById('emailTemplate');
            for (i = 0; i < ddl.options.length; i++) {
                if (ddl.options[i].value == step["actionID"]) {
                    ddl.options[i].selected = true;
                    break
                }
            }
            break;
        case 11: // dynamic value 
            populateForEdit(step["conditions"], "Update", baseTableID);
            toggleUserControl("show");
            break;
        case 12: // decision false
            break;
        case 13:
            var txtMessage = document.getElementById("txtMessage");
            if (txtMessage !== undefined) {
                txtMessage.value = step["message"];
            }
            break;
        case 14:
            ddl = document.getElementById('ddlFormID');
            for (i = 0; i < ddl.options.length; i++) {
                if (ddl.options[i].value == step["formID"]) {
                    ddl.options[i].selected = true;
                    break;
                }
            }
            break;
        case 9:
            break;
        default:
            alert(step["action"] + " missing from populateStepForEdit()");
            break;
    }
}

function ToggleActiveStep(div, evt) {
    // event handling
    var e = (evt) ? evt : window.event;
    if (evt !== null) {
        if (window.event) {
            e.cancelBubble = true;
        }
        else {
            e.stopPropagation();
        }
    }

    //reset the colours
    ColourSteps();

    if (selectedStep == div.id) // we're clicking on the currently selected div
    {
        SetActiveStep(-1);
    }
    else {
        SetActiveStep(div.id);
        div.style.backgroundColor = selectedStepColour;
    }
    return;
}


function SetActiveStep(divID) {
    if (divID === null || divID === undefined || divID === -1) {
        selectedStep = -1;
        hasSelectedStep = false;
    }
    else {
        selectedStep = divID;
        hasSelectedStep = true;
    }
    return;
}


function GetActiveStep() { return selectedStep; }

function showapprovaldeclaration()
{
    var checkbox = document.getElementById('approvalShowDeclaration');
    if(checkbox.checked === true)
    {
        document.getElementById('approvalDeclaration').style.display = "";
    } else {
        document.getElementById('approvalDeclaration').style.display = "none";
    }
}

function isValid()
{
    var field;
    var valid = true;
    var j = 0;
    invalidFields = [];
    
    for(var i = 0; i < reqFields.length; i++) 
    {
        field = document.getElementById(reqFields[i]);
       
        if((field.nodeName == "INPUT" || field.nodeName == "TEXTAREA") && (field.value == "" || field.value === null || field === undefined))
        {
            invalidFields[j] = field.title;
            j++;
            valid = false;
        }
        else if (field.nodeName == "SELECT" && field.options[field.selectedIndex].value == 0) {
            invalidFields[j] = field.title;
            j++;
            valid = false;
        }   
    }
    
    return valid;
}

function addRequiredField(field)
{
    reqFields[reqFieldCount] = field;
    reqFieldCount++;
    return;
}

function deleteStep()
{
    if(hasSelectedStep === true)
    {
        var stepToDelete = document.getElementById(selectedStep);
        var stepID = stepToDelete.id.substr(5);
        var i;
        var relatedSteps = [];
        
        
        for(i = 0; i < currentSteps.length; i++) 
        {
            if(currentSteps[i] !== null && currentSteps[i]["relatedStepID"] == stepID) 
            {
                relatedSteps.push(i);
            }
        }
        
        var deleteMessage = "";

        if(parseInt(currentSteps[stepID]["action"]) == 12)
        {
            SEL.MasterPopup.ShowMasterPopup("This step cannot be deleted as it is related to another step.");
            return;
        } 
        else if(relatedSteps.length > 0) 
        {
            deleteMessage = "Deleting this step will delete all related steps and sub steps, are you sure you wish to proceed?";        
        } 
        else if(stepToDelete.getElementsByTagName('DIV').length > 0) 
        {
            deleteMessage = "Deleting this step will delete all of it's sub steps, are you sure you wish to proceed?";
        }
        else
        {        
            deleteMessage = "Are you sure you want to delete this step?";
        }
        
        if(confirm(deleteMessage) === false) 
        {
            return;
        }

        for(i = 0; i < currentSteps.length; i++) 
        {
            if(currentSteps[i] !== null && currentSteps[i]["divID"] == stepToDelete.id)
            {
                currentSteps[i] = null;
            }
        }
        
        var subSteps = stepToDelete.getElementsByTagName("DIV")
        for(i = 0; i < subSteps.length; i++) 
        {
            if(subSteps[i].id.substr(0,5) == "step_") 
            {
                currentSteps[subSteps[i].id.substr(5)] = null;
            }
        }
        
        for(i = 0; i < relatedSteps.length; i++)
        {
            currentSteps[relatedSteps[i]] = null;
            if (stepToDelete.parentNode !== undefined) {
                stepToDelete.parentNode.removeChild(document.getElementById("step_" + relatedSteps[i]));
            }
        }
        
        stepToDelete.parentNode.removeChild(stepToDelete);
        
        hasSelectedStep = false;
        selectedStep = -1;

        workflowHasChanged = true;
        ColourSteps();

        var tmpDelNode;
        for (i = 0; i < currentSteps.length; i++) // ensure that any substeps that were deleted are also removed from currentSteps
        {
            if (currentSteps[i] !== null) {
                tmpDelNode = document.getElementById(currentSteps[i]['divID']);
                if (tmpDelNode === null) {
                    currentSteps[i] = null;
                }
            }
        }
        var moveToStepRefErr = false;
        for (i = 0; i < currentSteps.length; i++) // find all of the movetosteps that referenced this step and highlight them as errors
        {
            if (currentSteps[i] !== null && currentSteps[i]['action'] == 5 && currentSteps[currentSteps[i]['actionID']] === null) {
                document.getElementById(currentSteps[i]['divID']).style.backgroundColor = errorColour;
                moveToStepRefErr = true;
            }
        }
        if (moveToStepRefErr) {
            SEL.MasterPopup.ShowMasterPopup('At least one step was deleted that was the target of a "move to step" step. Please check any that were highlighted red.', 'Move to Step References');
        }
    }
    else 
    {
        SEL.MasterPopup.ShowMasterPopup('Please select the step you want to delete.', 'Delete Step');
    }
    return;
}

function showStepOptions(evt, optionsdivID)
{   
    var e=(evt)?evt:window.event;
    
    if (window.event) {
        e.cancelBubble=true;
    } else {
        e.stopPropagation();
    }
    if (hasSelectedStep === true)
    {
        var selectedStepAction = selectedStep.substr(5);
    }
    var i = 0;
    
    var optionsDiv = document.getElementById('divStepOptions');
    if (hasSelectedStep === true && stepsCounter > 0 && (currentSteps[selectedStepAction]["action"] == "1" || currentSteps[selectedStepAction]["action"] == "8" || currentSteps[selectedStepAction]["action"] == "3" || currentSteps[selectedStepAction]["action"] == "10" || currentSteps[selectedStepAction]["action"] == "4" || currentSteps[selectedStepAction]["action"] == "12"))
    {
        optionsDiv.innerHTML = "<a href=\"javascript:showNewStepModal(null, false);\" title=\"Add Step\"><img src=\"/shared/images/icons/16/Plain/add2.png\" border=\"0\" align=\"absmiddle\" alt=\"Add Step\" title=\"Add Step\" /> Add Sub Step</a>"; //     
    } 
    else 
    {
        optionsDiv.innerHTML = "<a href=\"javascript:showNewStepModal(null, false);\" title=\"Add Step\"><img src=\"/shared/images/icons/16/Plain/add2.png\" border=\"0\" align=\"absmiddle\" alt=\"Add Step\" title=\"Add Step\" /> Add Step</a>"; //     
    }

    
    if(hasSelectedStep === true && stepsCounter > 0) 
    {
       

        
        if(currentSteps[selectedStepAction] !== null && (currentSteps[selectedStepAction]["action"] == "3" || currentSteps[selectedStepAction]["action"] == "8")) {
            var tmpStr = (currentSteps[selectedStepAction]["action"] == "8") ? " before" : "";
            optionsDiv.innerHTML += "<br />&nbsp;-&nbsp;<a href=\"javascript:showNewStepModal('elsecondition', false);\" title=\"\"><img src=\"/shared/images/icons/16/Plain/add2.png\" border=\"0\" align=\"absmiddle\" alt=\"Add else condition" + tmpStr + "\" title=\"Add else condition" + tmpStr + "\" border=\"0\" /> Add else condition" + tmpStr + "</a>";
                
            var hasOtherwise = false;
            for( i = 0; i < currentSteps.length; i++) 
            {
                if(currentSteps[selectedStepAction] !== null && currentSteps[i] !== null && currentSteps[selectedStepAction]["relatedStepID"] === null && currentSteps[i]["action"] == "10" && currentSteps[i]["relatedStepID"] == currentSteps[selectedStepAction]["divID"].substr(5)) 
                {
                    hasOtherwise = true;
                    break;  
                } 
                else if(currentSteps[i] !== null && currentSteps[i]["action"] == "10" && currentSteps[i]["relatedStepID"] == currentSteps[selectedStepAction]["relatedStepID"])
                {
                    hasOtherwise = true;
                    break;  
                 }     
            }
            
            if(hasOtherwise === false) 
            {
                optionsDiv.innerHTML += "<br />&nbsp;-&nbsp;<a href=\"javascript:showNewStepModal('otherwisecondition', false);\" alt=\"Add otherwise condition\" title=\"Add otherwise condition\"><img src=\"/shared/images/icons/16/Plain/add2.png\" border=\"0\" align=\"absmiddle\" border=\"0\" /> Add otherwise condition</a>";
            }
        }

        if (hasSelectedStep === true && stepsCounter > 0 && currentSteps[selectedStepAction]["action"] != 8 && currentSteps[selectedStepAction]["action"] != 10 && currentSteps[selectedStepAction]["action"] != 12 && currentSteps[selectedStepAction]["action"] != 12) {
            optionsDiv.innerHTML += "<br /><a href=\"javascript:showNewStepModal('insertstepbefore', false);\" title=\"\"><img src=\"/shared/images/icons/16/Plain/add2.png\" border=\"0\" align=\"absmiddle\" alt=\"Add Step Before\" title=\"Add Step Before\" border=\"0\" /> Add Step Before</a>";
        }

        if(currentSteps[selectedStepAction]["action"] != 12)
        {
            optionsDiv.innerHTML += "<br /><a href=\"javascript:editStep();\" title=\"Edit Step\"><img src=\"/shared/images/icons/edit.gif\" border=\"0\" align=\"absmiddle\" /> Edit Step</a>";
        }
        
        if(parseInt(currentSteps[selectedStepAction]["action"]) != 12)
        {
            optionsDiv.innerHTML += "<br /><a href=\"javascript:deleteStep();\" title=\"Delete Step\" id=\"deleteStep\"><img src=\"/shared/images/icons/delete2.gif\" border=\"0\" align=\"absmiddle\" alt=\"Delete Step\" title=\"Delete Step\" /> Delete Step</a>";
        }
        

    } 
    
    $find(optionsdivID)._popupBehavior._parentElement = document.getElementById('stepOptions');
    $find(optionsdivID).showPopup();
}

function deleteWorkflow(workflowID)
{
    if(confirm("Are you sure you want to delete this workflow?") === true)
    {
        currentWorkflowGridRow = workflowID;
        Spend_Management.svcWorkflows.DeleteWorkflow(workflowID, DeleteWorkflowComplete, errorGettingString);
    } 
    return;
}

function DeleteWorkflowComplete(data) 
{
    if(data === false) 
    {
        SEL.MasterPopup.ShowMasterPopup("This workflow is currently in use", "Workflow in use");
        return;
    } 
    else 
    {
        SEL.Grid.deleteGridRow('workflows', currentWorkflowGridRow);
    }
}

function addStepDetailsToArray(stepID, key, value)
{
    currentSteps[stepID][key] = value;
    return;
}


function GetWorkflowSteps() {
    var workflowStepID = 0;
    var workflowSteps = [];
    var newIndex = [];
    var i;
    var id;
    var tmp;

    var onScreenSteps = document.getElementById("stepsLayout").getElementsByTagName("div");
    var stepsOrder = [];

    for (i = 0; i < onScreenSteps.length; i++) {
        if (onScreenSteps[i].id !== null && onScreenSteps[i].id.substr(5) !== null && isFinite(parseInt(onScreenSteps[i].id.substr(5))) && onScreenSteps[i].id.substr(0, 5) == "step_") // check the div is a proper stepdiv and we can parse a stepid out from it
        {
            stepsOrder.push(parseInt(onScreenSteps[i].id.substr(5), 10));
        }
        else {
            return [];
        }
    }

    for (i = 0; i < stepsOrder.length; i++) {
        id = stepsOrder[i];
        if (currentSteps[id] !== undefined && currentSteps[id] !== null) {
            workflowSteps[workflowStepID] = [];
            workflowSteps[workflowStepID][0] = currentSteps[id]["action"];
            workflowSteps[workflowStepID][1] = currentSteps[id]["description"];
            workflowSteps[workflowStepID][2] = currentSteps[id]["question"];
            workflowSteps[workflowStepID][3] = currentSteps[id]["trueAnswer"];
            workflowSteps[workflowStepID][4] = currentSteps[id]["falseAnswer"];
            workflowSteps[workflowStepID][5] = [];
            workflowSteps[workflowStepID][5] = currentSteps[id]["conditions"];
            workflowSteps[workflowStepID][6] = currentSteps[id]["approverType"];
            workflowSteps[workflowStepID][7] = currentSteps[id]["actionID"];
            workflowSteps[workflowStepID][8] = currentSteps[id]["oneclicksignoff"];
            workflowSteps[workflowStepID][9] = currentSteps[id]["showdeclaration"];
            workflowSteps[workflowStepID][10] = parseInt(currentSteps[id]["parentStepID"], 10);
            workflowSteps[workflowStepID][11] = parseInt(currentSteps[id]["relatedStepID"], 10);
            workflowSteps[workflowStepID][12] = currentSteps[id]["divID"];
            workflowSteps[workflowStepID][13] = currentSteps[id]["approvalEmailTemplateID"];
            workflowSteps[workflowStepID][14] = currentSteps[id]["message"];
            workflowSteps[workflowStepID][15] = currentSteps[id]["formID"];

            newIndex[id] = workflowStepID;

            workflowStepID++;
        }
    }

    for (i = 0; i < workflowSteps.length; i++) // replace the indexes that may have changed
    {
        if (workflowSteps[i][0] === 5) {
            tmp = parseInt(workflowSteps[i][7], 10);
            workflowSteps[i][7] = (isFinite(tmp)) ? newIndex[tmp] : null;
        }
        workflowSteps[i][10] = (isFinite(workflowSteps[i][10]) && workflowSteps[i][10] !== -1) ? newIndex[workflowSteps[i][10]] : null;
        workflowSteps[i][11] = (isFinite(workflowSteps[i][11]) && workflowSteps[i][11] !== -1) ? newIndex[workflowSteps[i][11]] : null;
    }

    var nextStep = 0;
    for (i = 0; i < workflowSteps.length; i++) {
        if (workflowSteps[i][0] === 1 && workflowSteps[i][11] === null) // look for Approval steps that don't have a related step (meaning they're not rejection steps)
        {
            for (nextStep = i + 1; nextStep <= workflowSteps.length; nextStep++) {
                if (nextStep == workflowSteps.length) { return -1; }

                if (isSubStepOfStep(workflowSteps, nextStep, i)) {
                    continue;
                }
                else {
                    if (workflowSteps[nextStep][0] !== 1 || workflowSteps[nextStep][10] !== workflowSteps[i][10]) {
                        return -1;
                    }
                    else {
                        workflowSteps[nextStep][11] = i;
                        break;
                    }
                }
            }
        }
    }
    return workflowSteps;
}

function isSubStepOfStep(steps, stepToTest, parentStep) {
    if (steps !== null && steps[stepToTest] !== null && steps[stepToTest][10] !== undefined) {
        if (steps[stepToTest][10] === null) // we've reached the top of the tree
        {
            return false;
        }
        else if (steps[stepToTest][10] === parentStep) // it's a direct substep of this parent
        {
            return true;
        }
        else // could still be a sub-sub step, recurse up a level and check till we find it or run out of tree to climb
        {
            isSubStepOfStep(steps, steps[stepToTest][10], parentStep);
        }
    }
    else {
        return -1;
    }
}

function updateStepText(stepID, actionType, description, actionText) {
    var spans = document.getElementById(stepID);
    if (spans !== null) {
        spans = document.getElementById(stepID).getElementsByTagName('span')[0].getElementsByTagName('span');
        if (spans.length > 1) {
            if (spans[0] !== null && spans[0].parentNode.parentNode.id == stepID && actionType !== undefined && actionType !== null) // step type
            {
                spans[0].innerText = actionType;
            }

            if (spans[1] !== undefined && spans[1] !== null && spans[1].parentNode.parentNode.id == stepID) // step description
            {
                spans[1].innerText = description;
            }

            if (spans[2] !== undefined && spans[2] !== null && spans[2].parentNode.parentNode.id == stepID && actionText !== undefined && actionText !== null) // step action
            {
                spans[2].innerText = actionText;
            }
        }
    }
    return;
}

function addStepToSteps(parentStepID, actionType, description, actionText) {
    var steps = document.getElementById("stepsLayout");
    var beforeStepNode = document.getElementById(insertBeforeStep);

    var newStepDiv = document.createElement("div");
    newStepDiv.id = "step_" + stepsCounter;
    newStepDiv.onmousedown = function () { ToggleActiveStep(this, event) };
    newStepDiv.setAttribute("class", "workflowStep");
    newStepDiv.setAttribute("className", "workflowStep");
    //newStepDiv.innerHTML = "<img src=\"/shared/images/icons/edit.png\" onclick=\"SetActiveStep(this.parentNode.id); editStep();\">";

    if (parseInt(currentSteps[stepsCounter]["action"]) != 12) {
        newStepDiv.innerHTML = "<img src=\"/shared/images/icons/edit.png\" onclick=\"SetActiveStep(this.parentNode.id); editStep();\"><img src=\"/shared/images/icons/delete2.png\" onclick=\"SetActiveStep(this.parentNode.id); deleteStep();\">";
    }
    else {
        newStepDiv.innerHTML = "";
    }

    newStepDiv.innerHTML += "<span style=\"vertical-align: top;\"> - <strong><span>" + actionType + "</span></strong>&nbsp;&nbsp;&nbsp;&nbsp;<span>(" + description + ")</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>" + actionText + "</span></span>";

    var i;
    if (parentStepID == -1) {
        if (insertBeforeStep !== -1) // inserting not adding
        {
            beforeStepNode.parentNode.insertBefore(newStepDiv, beforeStepNode);
        }
        else {
            steps.appendChild(newStepDiv);
        }
    }
    else {
        var parentDiv;

        if (parseInt(currentSteps[stepsCounter]["action"], 10) == 8) // else step
        {
            var curStep = document.getElementById('step_' + currentSteps[stepsCounter]["relatedStepID"]);
            var curParent = curStep.parentNode;
            var nextStep;

            if (selectedStep !== -1 && selectedStep !== null && parseInt(currentSteps[selectedStep.substr(5)]["action"], 10) == 8) {
                nextStep = document.getElementById(selectedStep);
            }
            else if (selectedStep !== -1 && selectedStep !== null && parseInt(currentSteps[selectedStep.substr(5)]["action"], 10) == 3) {
                // go past any else steps (same relatedID) till we reach end or a non-related field
                for (nextStep = curStep.nextSibling; nextStep !== undefined && nextStep !== null && currentSteps[nextStep.id.substr(5)]['action'] != 10 && currentSteps[nextStep.id.substr(5)]['relatedStepID'] == currentSteps[stepsCounter]['relatedStepID']; nextStep = curStep.nextSibling) {
                    curStep = nextStep;
                }
            }
            else {
                nextStep = curStep.nextSibling;
            }

            if (curStep.style.marginLeft == "54px") {
                newStepDiv.style.marginLeft = "54px";
            }

            if (nextStep === undefined || nextStep === null) {
                curParent.appendChild(newStepDiv);
            }
            else {
                curParent.insertBefore(newStepDiv, nextStep);
            }
        }
        else if (parseInt(currentSteps[stepsCounter]["action"]) == 10) // otherwise step
        {
            var curStep = document.getElementById('step_' + currentSteps[stepsCounter]["relatedStepID"]);
            var curParent = curStep.parentNode;
            var nextStep;

            // go past any else steps (same relatedID) till we reach end or a non-related field
            for (nextStep = curStep.nextSibling; nextStep !== undefined && nextStep !== null && currentSteps[nextStep.id.substr(5)]['relatedStepID'] == currentSteps[stepsCounter]['relatedStepID']; nextStep = curStep.nextSibling) {
                curStep = nextStep;
            }

            if (curStep.style.marginLeft == "54px") {
                newStepDiv.style.marginLeft = "54px";
            }

            if (nextStep === undefined || nextStep === null) {
                curParent.appendChild(newStepDiv);
            }
            else {
                curParent.insertBefore(newStepDiv, nextStep);
            }
        }
        else {
            parentDiv = document.getElementById(parentStepID);

            if (currentSteps[parentStepID.substr(5)]["action"] == "1" || currentSteps[parentStepID.substr(5)]["action"] == "3" || currentSteps[parentStepID.substr(5)]["action"] == "8" || currentSteps[parentStepID.substr(5)]["action"] == "10" || currentSteps[parentStepID.substr(5)]["action"] == "4" || currentSteps[parentStepID.substr(5)]["action"] == "12") {

                if (insertBeforeStep !== -1) // inserting not adding
                {
                    if (parentDiv !== null && parentDiv.style.marginLeft == "54px") { newStepDiv.style.marginLeft = "54px"; }
                    beforeStepNode.parentNode.insertBefore(newStepDiv, beforeStepNode);
                }
                else {
                    newStepDiv.style.marginLeft = "54px";
                    parentDiv.appendChild(newStepDiv);
                }
            }
            else {
                if (currentSteps[parentStepID.substr(5)]["parentStepID"] === -1 || currentSteps[parentStepID.substr(5)]["parentStepID"] === null) {

                    if (insertBeforeStep !== -1) // inserting not adding
                    {
                        beforeStepNode.parentNode.insertBefore(newStepDiv, beforeStepNode);
                    }
                    else {
                        steps.appendChild(newStepDiv);
                    }
                }
                else {
                    if (parentDiv !== null && parentDiv.style.marginLeft == "54px") { newStepDiv.style.marginLeft = "54px"; }

                    if (insertBeforeStep !== -1) // inserting not adding
                    {
                        beforeStepNode.parentNode.insertBefore(newStepDiv, beforeStepNode);
                    }
                    else {
                        if (currentSteps[parentStepID.substr(5)]["parentStepID"] !== null && currentSteps[parentStepID.substr(5)]["parentStepID"] !== -1) {
                            document.getElementById("step_" + currentSteps[parentStepID.substr(5)]["parentStepID"]).appendChild(newStepDiv);
                        }
                        else {
                            parentDiv = document.getElementById("stepsLayout");
                            parentDiv.appendChild(newStepDiv);
                        }
                    }
                }
            }
        }
    }

    stepsCounter++;
    ColourSteps();
    selectedStep = -1;
    insertBeforeStep = -1;
    hasSelectedStep = false;
    return;
}

function populateApproverSelect(approverID)
{
    if(approverID === undefined || approverID === null)
    {
        approverID = null;
    }
    var approverType = document.getElementById('approvalType')[document.getElementById('approvalType').selectedIndex].value;
    Spend_Management.svcWorkflows.GetApproverList(approverType, 0, approverID, populateApproverSelectComplete);
    return;
}

function AEConditionSteps(include)
{
    var ddl = document.getElementById(ddlStepAction);
    var i;
    var existAlready = false;
    
    if(include === true) 
    {
        
        for(i = 0; i < ddl.options.length; i++)
        {
            if(ddl.options[i].value == "10" || ddl.options[i].value == "8")
            {
                existAlready = true;
                break;
            }
        }
        
        var option;
        if(existAlready === false) 
        {
            option = document.createElement("option");
            option.value = 8;
            option.text = "Else Condition";
            ddl.options[ddl.options.length] = option;

            option = document.createElement("option");
            option.value = 10;
            option.text = "Otherwise Condition";
            ddl.options[ddl.options.length] = option;
        }
    }
    else
    {
        var optionsToDelete = [];
        for(i = 0; i < ddl.options.length; i++)
        {
            if(ddl.options[i].value == "10" || ddl.options[i].value == "8")
            {
                ddl.remove(i);
                AEConditionSteps(false);
            }
        }   
    }
}

function showNewStepModal(action) {
    CurrentStep = undefined;
    $find(popupOptions).hidePopup();
    insertBeforeStep = -1;

    document.getElementById('specificDetails').innerHTML = "&nbsp;";
    var ddl;
    var i;
    if (action === undefined || action === null) {
        AEConditionSteps(false);
        document.getElementById(ddlStepAction).options[0].selected = true;
        document.getElementById(ddlStepAction).disabled = false;
        toggleUserControl("hide");
    }
    else if (action == "insertstepbefore") {
        AEConditionSteps(false);
        document.getElementById(ddlStepAction).options[0].selected = true;
        document.getElementById(ddlStepAction).disabled = false;
        toggleUserControl("hide");
        insertBeforeStep = (isNaN(parseInt(selectedStep.substr(5), 10))) ? -1 : selectedStep;
    }
    else if (action == "elsecondition") {
        AEConditionSteps(true);
        ddl = document.getElementById(ddlStepAction);
        for (i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == "8") {
                ddl.options[i].selected = true;
                break;
            }
        }

        if (editing === false) {
            ddl.onchange();
            changeSpecificDetails(false);
        }
        ddl.disabled = true;
        SetCriteriaMode('Select');
        clearCriteria();
    }
    else if (action == "otherwisecondition") {
        AEConditionSteps(true);
        ddl = document.getElementById(ddlStepAction);
        for (i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == "10") {
                ddl.options[i].selected = true;
                break;
            }
        }

        ddl.disabled = true;
        if (editing === false) {
            ddl.onchange();
            changeSpecificDetails(false);
        }
        SetCriteriaMode('Select');
        clearCriteria();
    }
    else if (action == "decision") {
        //AEConditionSteps(true);
        ddl = document.getElementById(ddlStepAction);
        for (i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == "4") {
                ddl.options[i].selected = true;
                break;
            }
        }

        ddl.disabled = true;

        SetCriteriaMode('Select');
        clearCriteria();
    }
    else if (action == "ifcondition") {
        //AEConditionSteps(true);
        ddl = document.getElementById(ddlStepAction);
        for (i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == "3") {
                ddl.options[i].selected = true;
                break;
            }
        }

        ddl.disabled = true;

        SetCriteriaMode('Select');
        clearCriteria();
    }

    $find(popupModal).show();

    return;
}


function GetStepDescription(step)
{
    var stepDivDescription;
    switch (parseInt(step["action"]))
    {
        case 1: // approval
            stepDivDescription = "Approval by: " + document.getElementById("approvalType")[document.getElementById("approvalType").selectedIndex].text;

            if (document.getElementById("approvalType")[document.getElementById("approvalType").selectedIndex].value != "5" && document.getElementById("approvalType")[document.getElementById("approvalType").selectedIndex].value != "4")
            {
                var approverIDObj = document.getElementById("approverSelect");
                if (document.getElementById("approverSelect") !== undefined && document.getElementById("approverSelect").options.length > 0)
                {
                    stepDivDescription += ", " + document.getElementById("approverSelect")[document.getElementById("approverSelect").selectedIndex].text;
                }
            }
            break;
        case 2: // change val
//            stepDivDescription = "Change value";
//            stepDivDescription += "<br />" + getCriteriaAsString();
            stepDivDescription = getCriteriaAsString();
            break;
        case 3: // check con
//            stepDivDescription = "Check condition:";
//            stepDivDescription += "<br />" + getCriteriaAsString();
            stepDivDescription = getCriteriaAsString();
            break;
        case 4: // decision
            stepDivDescription = "Decision: " + document.getElementById("txtQuestion").value;
            break;
        case 5: // move to step
            stepDivDescription = "Move to step: " + document.getElementById("ddlSelectedMoveToStep")[document.getElementById("ddlSelectedMoveToStep").selectedIndex].text;
            break;
        case 6: // run sub workflow
            stepDivDescription = "Run sub-workflow: " + document.getElementById("ddlSelectedRunSubWorkflows")[document.getElementById("ddlSelectedRunSubWorkflows").selectedIndex].text;
            break;
        case 7: // send email
            stepDivDescription = "Send email: " + document.getElementById("emailTemplate")[document.getElementById("emailTemplate").selectedIndex].text;
            break;
        case 8: // else condition
            stepDivDescription = "Else (related to " + step["relatedStepID"] + ")";
            stepDivDescription += getCriteriaAsString();
            break;
        case 9: // finishworkflow
            stepDivDescription = "Finish workflow";
            break;
        case 10: //  else otherwise
            stepDivDescription = "Else otherwise (related to " + step["relatedStepID"] + ")";
            break;
        case 11: // create dynamic value
            stepDivDescription = getCriteriaAsString();
            break;
        case 12: // decision false
            break;
        case 13:
            stepDivDescription = "Show Message " + step["message"];
            break;
        case 14:
            stepDivDescription = "Change Form" + step["formID"];
            break;
        default:
            alert(step["action"] + " missing from GetStepDescription()");
            break;
    }
    return stepDivDescription;
}

function addStep() 
{
    workflowHasChanged = true;
    reqFields = [];
    reqFieldCount = 0;
    
    var thisStepID = stepsCounter;
    var actionValue = document.getElementById(ddlStepAction)[document.getElementById(ddlStepAction).selectedIndex].value;
    if (parseInt(actionValue, 10) == 0) {
        return;
    }
    var actionText = document.getElementById(ddlStepAction)[document.getElementById(ddlStepAction).selectedIndex].text;
    var workflowType = document.getElementById(workflowTypeClientID)[document.getElementById(workflowTypeClientID).selectedIndex].value;
    var txtStepDescription = document.getElementById('txtStepDescription').value;
    var currentActiveStep_ = GetActiveStep();
    var showdeclaration;
    var oneclick;
    var question;
    var truebutton;
    var falsebutton;
    var emailTemplateID;
    var i;
    var stepDivDescription;
    var newStepKey;
    var selectedStepID = (selectedStep !== -1 && selectedStep !== null) ? selectedStep.substr(5) : -1;

    CurrentStep = undefined;
    
    if(inEditMode === true) 
    {
        newStepKey = parseInt(selectedStep.substr(5), 10);
        CurrentStep = currentSteps[newStepKey];
    } 
    else
    {
        newStepKey = stepsCounter;
    }

    var tmpParentID;
    var tmpRelatedID = null;
    if (inEditMode === true) {
        tmpParentID = currentSteps[selectedStepID]["parentStepID"];
        tmpRelatedID = currentSteps[selectedStepID]["relatedStepID"];
    }
    else if (selectedStep === -1 || selectedStep === null) {
        tmpParentID = -1;
    }
    else if (currentSteps[selectedStepID]["action"] == "1" || currentSteps[selectedStepID]["action"] == "3" || currentSteps[selectedStepID]["action"] == "8" || currentSteps[selectedStepID]["action"] == "10" || currentSteps[selectedStepID]["action"] == "4" || currentSteps[selectedStepID]["action"] == "12") {
        if (insertBeforeStep !== -1) {
            tmpParentID = currentSteps[insertBeforeStep.substr(5)]["parentStepID"];
        }
        else {
            tmpParentID = selectedStepID;
        }
    }
    else {
        tmpParentID = currentSteps[selectedStepID]["parentStepID"];
    }

    currentSteps[newStepKey] = [];
    currentSteps[newStepKey]["action"] = parseInt(actionValue, 10);
    currentSteps[newStepKey]["description"] = txtStepDescription;
    currentSteps[newStepKey]["question"] = "";
    currentSteps[newStepKey]["trueAnswer"] = "";
    currentSteps[newStepKey]["falseAnswer"] = "";
    currentSteps[newStepKey]["conditions"] = getCriteriaArray();
    currentSteps[newStepKey]["approverType"] = 0;
    currentSteps[newStepKey]["actionID"] = 0;
    currentSteps[newStepKey]["oneclicksignoff"] = false;
    currentSteps[newStepKey]["showdeclaration"] = false;
    currentSteps[newStepKey]["parentStepID"] = tmpParentID;
    currentSteps[newStepKey]["relatedStepID"] = tmpRelatedID;
    currentSteps[newStepKey]["divID"] = "step_" + newStepKey;
    currentSteps[newStepKey]["approvalEmailTemplateID"] = null;
    currentSteps[newStepKey]["message"] = "";

    addRequiredField("txtStepDescription");

    if (actionValue == "0") // Select the step
    {
        SEL.MasterPopup.ShowMasterPopup("Please select what action this step should perform.", "Required Fields");
        return;
    }
    else if (actionValue == "1") // approval
    {
        if (workflowType == "d70d9e5f-37e2-4025-9492-3bcf6aa746a8")
        {
            showdeclaration = document.getElementById("approvalShowDeclaration").checked;
            oneclick = document.getElementById("oneclicksignoff").checked;
            question = document.getElementById("txtQuestion").value;
            truebutton = document.getElementById("txtTrueButton").value;
            falsebutton = document.getElementById("txtFalseButton").value;
        }
        else
        {
            showdeclaration = false;
            oneclick = false;
            question = "";
            truebutton = "";
            falsebutton = "";
        }

        var approverType = document.getElementById("approvalType")[document.getElementById("approvalType").selectedIndex].value;

        var approverIDObj = document.getElementById("approverSelect");
        var approverID = null;
        if (approverIDObj !== undefined && approverIDObj.options.length > 0 && parseInt(approverType) != 4 && parseInt(approverType) != 5)
        {
            approverID = approverIDObj[approverIDObj.selectedIndex].value;
        }

        var emailTemplateObj = document.getElementById("emailTemplate");
        if (emailTemplateObj !== undefined)
        {
            emailTemplateID = emailTemplateObj[emailTemplateObj.selectedIndex].value;
        }
        else
        {
            emailTemplateID = null;
        }


        var messageObj = document.getElementById('txtMessage');
        if (messageObj !== null) {
            addStepDetailsToArray(newStepKey, "message", messageObj.value);
        }

        addStepDetailsToArray(newStepKey, "approverType", approverType);
        addStepDetailsToArray(newStepKey, "actionID", approverID);
        addStepDetailsToArray(newStepKey, "question", question);
        addStepDetailsToArray(newStepKey, "trueAnswer", truebutton);
        addStepDetailsToArray(newStepKey, "falseAnswer", falsebutton);
        addStepDetailsToArray(newStepKey, "oneclicksignoff", oneclick);
        addStepDetailsToArray(newStepKey, "showdeclaration", showdeclaration);

        if (parseInt(emailTemplateID) == 0)
        {
            addStepDetailsToArray(newStepKey, "approvalEmailTemplateID", null);
        }
        else
        {
            addStepDetailsToArray(newStepKey, "approvalEmailTemplateID", emailTemplateID);
        }

    }
    else if (actionValue == "2") // change value
    {

    }
    else if (actionValue == "3") // check condition
    {

    }
    else if (actionValue == "11") // dynamic value
    {

    }
    else if (actionValue == "10" || actionValue == "8")
    {
        var step = currentSteps[newStepKey];
        var DuplicateConditionCount = 0;

        if (actionValue == "8") {
            for (var i = 0; i < currentSteps.length; i++) {
                if (i != newStepKey) // canwenot
                {
                    if (currentSteps[i] !== null) // combinethese
                    {
                        if (currentSteps[i].action == actionValue) // this may need changing to == 8 or 3 and related=related or related = i
                        {
                            for (var j = 0; j < currentSteps[i].conditions.length; j++) {
                                if (currentSteps[i].conditions[j][0] === step.conditions[j][0] && currentSteps[i].conditions[j][1] === step.conditions[j][1] && currentSteps[i].conditions[j][3] === step.conditions[j][3] && currentSteps[i].conditions[j][4] === step.conditions[j][4] && ((currentSteps[i].conditions[j][5] === "" && step.conditions[j][5] === null) || (currentSteps[i].conditions[j][5] === step.conditions[j][5]))) {
                                    DuplicateConditionCount++;
                                }
                            }

                            if (DuplicateConditionCount === currentSteps[i].conditions.length) {
                                SEL.MasterPopup.ShowMasterPopup("You cannot have two else conditions the same ", "Condition validation");
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (currentSteps[selectedStep.substr(5)]["relatedStepID"] === null) {
            currentSteps[newStepKey]["relatedStepID"] = selectedStep.substr(5);
        }
        else {
            currentSteps[newStepKey]["relatedStepID"] = currentSteps[selectedStep.substr(5)]["relatedStepID"];
        }

        if (currentSteps[selectedStep.substr(5)]["parentStepID"] === null) {
            //currentSteps[newStepKey]["parentStepID"] = selectedStep.substr(5); // check check
        }
        else {
            currentSteps[newStepKey]["parentStepID"] = currentSteps[selectedStep.substr(5)]["parentStepID"];
            //selectedStep = currentSteps[selectedStep.substr(5)]["divID"]; // check check
        }
    }
    else if (actionValue == "4") // descision
    {
        question = document.getElementById("txtQuestion").value;
        truebutton = document.getElementById("txtTrueButton").value;
        falsebutton = document.getElementById("txtFalseButton").value;

        addRequiredField("txtQuestion");
        addRequiredField("txtTrueButton");
        addRequiredField("txtFalseButton");

        addStepDetailsToArray(newStepKey, "question", question);
        addStepDetailsToArray(newStepKey, "trueAnswer", truebutton);
        addStepDetailsToArray(newStepKey, "falseAnswer", falsebutton);
    }
    else if (actionValue == "5") // move to step
    {
        var moveToStepObj = document.getElementById("ddlSelectedMoveToStep");
        if (moveToStepObj !== null)
        {
            var moveToStepID = moveToStepObj[moveToStepObj.selectedIndex].value;
            //addRequiredField("ddlSelectedMoveToStep");
            addStepDetailsToArray(newStepKey, "actionID", moveToStepID);
        } else
        {
            SEL.MasterPopup.ShowMasterPopup("You must have a valid step selected to add a move to step step", "Required Fields");
            return;
        }
    }
    else if (actionValue == "6") // Run sub-workflow
    {
        var subWorkflowObj = document.getElementById("ddlSelectedRunSubWorkflows");
        if (subWorkflowObj !== null) {
            subWorkflowID = subWorkflowObj[subWorkflowObj.selectedIndex].value;
            //addRequiredField("ddlSelectedRunSubWorkflows");
            addStepDetailsToArray(newStepKey, "actionID", subWorkflowID);
        }
        else {
            SEL.MasterPopup.ShowMasterPopup("You must have a valid sub-workflow selected to add a run sub-workflow step", "Required Fields");
            return;
        }
    }
    else if (actionValue == "7") // Send Email
    {
        var sendEmailObj = document.getElementById("emailTemplate");
        if (sendEmailObj !== null)
        {
            emailTemplateID = sendEmailObj[sendEmailObj.selectedIndex].value;
            addStepDetailsToArray(newStepKey, "actionID", emailTemplateID);
        }
        else
        {
            SEL.MasterPopup.ShowMasterPopup("You must have a valid email selected to add a send email step", "Required Fields");
            return;
        }
    }
    else if (actionValue == "13") // ShowMessage
    {
        var showMessageMessage = document.getElementById("txtMessage").value;
        if (showMessageMessage != "")
        {
            addStepDetailsToArray(newStepKey, "message", showMessageMessage);
        }
        else
        {
            SEL.MasterPopup.ShowMasterPopup("You must enter a valid message", "Required Fields");
        }
    }
    else if (actionValue == "14")
    {
        var formObj = document.getElementById("ddlFormID");
        var formID = formObj[formObj.selectedIndex].value;

        addStepDetailsToArray(newStepKey, "formID", formID);
    }
    else if (actionValue == "9")
    {
        
    }
    else
    {
        alert("Invalid action in addStep()");
    }
    stepDivDescription = GetStepDescription(currentSteps[newStepKey]);
    
    if(isValid() === true)
    {
        if (((parseInt(actionValue) == 2 || parseInt(actionValue) == 3 || parseInt(actionValue) == 8 || parseInt(actionValue) == 11) && currentSteps[newStepKey]["conditions"].length == 0))
        {
            SEL.MasterPopup.ShowMasterPopup("You currently have no criteria selected", "Required Fields");
            return;
        }
        if(inEditMode === false)
        {
            if (actionValue == "4" ) {
                currentSteps[newStepKey + 1] = [];
                currentSteps[newStepKey + 1]["action"] = 12;
                currentSteps[newStepKey + 1]["description"] = txtStepDescription;
                currentSteps[newStepKey + 1]["question"] = "";
                currentSteps[newStepKey + 1]["trueAnswer"] = "";
                currentSteps[newStepKey + 1]["falseAnswer"] = "";
                currentSteps[newStepKey + 1]["conditions"] = getCriteriaArray();
                currentSteps[newStepKey + 1]["approverType"] = 0;
                currentSteps[newStepKey + 1]["actionID"] = 0;
                currentSteps[newStepKey + 1]["oneclicksignoff"] = false;
                currentSteps[newStepKey + 1]["showdeclaration"] = false;
                currentSteps[newStepKey + 1]["parentStepID"] = tmpParentID;
                currentSteps[newStepKey + 1]["relatedStepID"] = newStepKey;
                currentSteps[newStepKey + 1]["divID"] = "step_" + (newStepKey + 1);
                currentSteps[newStepKey + 1]["approvalEmailTemplateID"] = null;
                currentSteps[newStepKey + 1]["message"] = "";

                var ins = insertBeforeStep; // this gets reset in addStepToSteps but will be needed for the decision false step
                addStepToSteps(currentActiveStep_, actionText, stepDivDescription, actionText + " (" + truebutton + ")</b><br />" + stepDivDescription);  //txtStepDescription);
                insertBeforeStep = ins;
                addStepToSteps(currentActiveStep_, actionText, stepDivDescription, actionText + " (" + falsebutton + ")");
                   
            } else {
                addStepToSteps(GetActiveStep(), actionText, stepDivDescription, actionText); // txtStepDescription);
            }
        }
        else
        {
            currentSteps[newStepKey]["description"] = txtStepDescription;

            if (parseInt(actionValue) == 4) {
                updateStepText(currentSteps[newStepKey]["divID"], actionText, txtStepDescription, actionText + " (" + truebutton + ")");
                //                for (i = 0; i < currentSteps.length; i++)
                //                {
                //                    if (currentSteps[i]["relatedStepID"] == newStepKey)
                //                    {
                //                        currentSteps[i]["description"] = txtStepDescription;
                updateStepText(currentSteps[newStepKey + 1]["divID"], actionText, txtStepDescription, actionText + " (" + falsebutton + ")");
                //                    }
                //                }
            }
            else {
                updateStepText(currentSteps[newStepKey]["divID"], actionText, stepDivDescription, actionText); //txtStepDescription);
            }
        }
        
        cancelStepModal(false);
        clearCriteria();
        toggleUserControl('hide');
        deleteStepsHelp();
    }
    else
    {
        var errorMessage = "You have not completed all of the required fields.";
        
        for(i = 0; i < invalidFields.length; i++)
        {
            errorMessage += "<br />- " + invalidFields[i];
        }
        SEL.MasterPopup.ShowMasterPopup(errorMessage, "Required Fields");
    }
    
    inEditMode = false;
    
    return;
}

function deleteStepsHelp() {
    var stepsHelp = document.getElementById('stepsHelp');
    if (stepsHelp !== null) {
        stepsHelp.parentNode.removeChild(stepsHelp);
    }
}

function changeSpecificDetails(edit) {
    var stepAction = document.getElementById(ddlStepAction)[document.getElementById(ddlStepAction).selectedIndex].value;
    var workflowType = document.getElementById(workflowTypeClientID)[document.getElementById(workflowTypeClientID).selectedIndex].value;

    if (stepAction != "0") {
        if (stepAction == "2")  // Change Value
        {
            toggleUserControl("show");
            SetCriteriaMode('Update');
            clearCriteria();
        }
        else if (stepAction == "3" || stepAction == "8") // check condition / else
        {
            toggleUserControl("show");
            SetCriteriaMode('Select');
            clearCriteria();
        }
        else if (stepAction == "11") // dynamic value
        {
            toggleUserControl("show");
            //SetCriteriaMode("DynamicValue");
            SetCriteriaMode('Update'); // temporary to allow direct formula input
            clearCriteria();
        }
        else {
            toggleUserControl("hide");
        }

        if (edit === undefined) {
            edit = false;
        }

        Spend_Management.svcWorkflows.GetStepActionFields(stepAction, workflowType, workflowID, edit, populateSpecificDetails, errorGettingString);
    } else {
        populateSpecificDetails('hide');
    }
}

function cancelStepModal(isCancelButton) {
    if (isCancelButton) {
        if (CurrentStep !== undefined) {
            currentSteps[selectedStep.substr(5)] = CurrentStep;
        }
    }
    inEditMode = false;
    $find(popupModal).hide();

    populateSpecificDetails("");
    document.getElementById(ddlStepAction)[0].selected = true;
}

function editStep() {
    if (hasSelectedStep === true) {
        var stepID = parseInt(selectedStep.substr(5), 10);

        if (isNaN(stepID) === false) {
            inEditMode = true;
            var ddlAction = null;
            if (parseInt(currentSteps[stepID]["action"]) == 8) {
                AEConditionSteps(true);
                ddlAction = "elsecondition";
            }
            else if (parseInt(currentSteps[stepID]["action"]) == 10) {
                AEConditionSteps(true);
                ddlAction = "elseotherwise";
            }
            else if (parseInt(currentSteps[stepID]["action"]) == 3) {
                ddlAction = "ifcondition";
            }
            else if (parseInt(currentSteps[stepID]["action"]) == 4) {
                ddlAction = "decision";
            }

            var stepDetails = currentSteps[stepID];
            var stepAction = document.getElementById(ddlStepAction);
            for (var x = 0; x < stepAction.options.length; x++) {
                if (stepAction.options[x].value == stepDetails["action"]) {
                    stepAction.options[x].selected = true;
                    break;
                }
            }

            changeSpecificDetails(true);
            showNewStepModal(ddlAction, true);

        }
    }
    return;
}

function SaveWorkflow(fromSaveButton) 
{
    if (saving === true) {
        SEL.MasterPopup.ShowMasterPopup("This workflow is currently in the process of being saved", "Saving");
        return;
    }

    if (ValidateGeneralDetails() === true && ValidateSteps() === true)
    {
        if (workflowHasChanged === true)
        {
            var wfName = $get(workflowNameClientID);
            var wfDescription = $get(workflowDescriptionClientID);
            var wfWorkflowType = $get(workflowTypeClientID);
            var wfAvailableAsAChildWorkflow = $get(workflowAvailableAsAChildWorkflowClientID);
            var wfRunOnCreation = $get(workflowRunOnCreationClientID);
            var wfRunOnChange = $get(workflowRunOnChangeClientID);
            var wfRunOnDelete = $get(workflowRunOnDeleteClientID);

            // validatedSteps should have been populated by the ValidateSteps

            Spend_Management.svcWorkflows.SaveWorkflow(workflowID, wfName.value, wfDescription.value, wfWorkflowType.value, wfAvailableAsAChildWorkflow.checked, wfRunOnCreation.checked, wfRunOnChange.checked, wfRunOnDelete.checked, validatedSteps, SaveWorkflowComplete, errorGettingString);
            saving = true;
        }
        else
        {
            workflowHasChanged = false;
            if (fromSaveButton === true)
            {
                window.location = 'workflows.aspx';
            }
        }
    }

    return;
}


function SaveWorkflowComplete(results) {
    saving = false;
    if (results > 0 && !isNaN(results)) {
        workflowID = results;
        workflowHasChanged = false;
        window.location = 'workflows.aspx';
    }
    else if (results == -1 && !isNaN(results)) {
        SEL.MasterPopup.ShowMasterPopup("A workflow with that name already exists.", "Error");
    }
    else if (results == -2 && !isNaN(results)) {
        if (testing === true) {
            errorGettingString(results)
        } else {
            SEL.MasterPopup.ShowMasterPopup("An error occured whilst saving the workflow, no updates have been made to the existing workflow due to this error.", "Error");
        }
    }
    else {
        SEL.MasterPopup.ShowMasterPopup("Invalid Access.", "Error");
    }
    return;
}

function ValidateGeneralDetails() {
    reqFields = [];
    reqFieldCount = 0;
    addRequiredField(workflowNameClientID);
    addRequiredField(workflowTypeClientID);

    if (isValid() === false) {
        $find(tabContainerClientID).set_activeTabIndex(0);

        var errorMessage = 'The following fields need to be completed before you can add any steps or save the workflow:<ul>';
        for (var i = 0; i < invalidFields.length; i++) {
            errorMessage += "<li>" + invalidFields[i] + "</li>";
        }
        errorMessage += "</ul>";
        SEL.MasterPopup.ShowMasterPopup(errorMessage, "Required Fields");
        return false;
    }

    return true;
}

function ValidateSteps() {
    validatedSteps = [];
    validatedSteps = GetWorkflowSteps();

    if (validatedSteps === null || validatedSteps.length === 0) {
        $find(tabContainerClientID).set_activeTabIndex(1);
        SEL.MasterPopup.ShowMasterPopup("You must have at least one active step to save the workflow", "Required Fields");
        return false;
    }
    else if (validatedSteps === -1) {
        $find(tabContainerClientID).set_activeTabIndex(1);
        SEL.MasterPopup.ShowMasterPopup("You must have an Approval (false) step immediately after your first Approval step", "Required Fields");
        return false;
    }

    return true;
}

function addStepDetailsConditionsToArray(stepID, conditions)
{
    
    return;
}

function UpdateDecisionStep(entityID, workflowID, response)
{
    Spend_Management.svcWorkflows.CompleteDecisionStep(entityID, workflowID, response, UpdateDecisionStepComplete, errorGettingString);
        return false;
}

function UpdateDecisionStepComplete(data)
{
//    window.location = '/home.aspx';

    if (data === null)
    {

    }
    else {
        if (data.Status != 1) {
            switch (parseInt(data.NextStep.Action)) {
                case 13:
                    SEL.MasterPopup.ShowMasterPopup(data.NextStep.Message, "Response");
                    break;
                case 14:
                    __doPostBack('__Page', '');
                    break;
                default:
                    alert("Unhandled workflow step.");
                    break;
            }

            /// 7 = FireAndForget
            if (parseInt(data.NextStep.Status) === 7) {

            }
        }
    }
    return false;
}

function UpdateApprovalStep(entityID, workflowID, response)
{
    Spend_Management.svcWorkflows.CompleteApprovalStep(entityID, workflowID, response, UpdateApprovalStepComplete, errorGettingString);
}

function UpdateApprovalStepComplete(data)
{
//    window.location = '/expenses/home.aspx';
    if (data === null)
    {
        window.location = window.location;
    }
    else
    {
        switch (parseInt(data.NextStep.Action))
        {
            case 13:
                SEL.MasterPopup.ShowMasterPopup(data.NextStep.Message, "Response");
                break;
            default:
                alert("Unhandled workflow step.");
                break;
        }
    }
}
