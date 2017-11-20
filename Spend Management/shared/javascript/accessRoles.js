function checkIfRowShouldBeViewable(chk, action)
{
    var checked = 0;
    var checks = chk.parentNode.parentNode.getElementsByTagName('input');
    
    for (var i = 1; i < checks.length; i++)
    {
        if (checks[i].checked === true)
        {
            checked++;
        }
    }

    if (checked > 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}

function deleteAccessRole(accessRoleID)
{
    if (confirm("Are you sure you wish to delete this access role?"))
    {
        Spend_Management.svcAccessRoles.DeleteAccessRole(accessRoleID, deleteAccessRoleComplete, deleteAccessRoleCallBack);
    }
}

function deleteAccessRoleComplete(data) {
    if (data[0] == true) {
        SEL.Grid.deleteGridRow('accessRolesGrid', data[1]);
    }

    SEL.MasterPopup.ShowMasterPopup(data[2], 'Access Role Delete');
}

function updateModuleElementCheckboxes(chk, action) 
{
    var tableBase = chk.parentNode.parentNode.parentNode;
    var checkboxes = tableBase.getElementsByTagName("input");
    
    updateElementCheckboxes(chk, action);
    
    for(var i = 0; i < checkboxes.length; i++) 
    {
        if (checkboxes[i].id.indexOf('chk_cat_') > -1)
        {
            var catStrArr = Array();
            catStrArr = checkboxes[i].id.split('_');
            
            var catID = 0;
            catID = catStrArr[catStrArr.length - 1];
            
            if (chk.checked === true)
            {
                if (checkboxes[i].id.indexOf(action.toLowerCase()) > -1)
                {
                    checkboxes[i].checked = true;
                    updateCategoryElementCheckboxes(checkboxes[i], action, catID);
                }
            }
            else
            {
                if (checkboxes[i].id.indexOf(action.toLowerCase()) > -1)
                {
                    if (action != "View")
                    {
                        checkboxes[i].checked = false;
                    }
                    else
                    {
                        if (checkIfRowShouldBeViewable(checkboxes[i], action) === false)
                        {
                            checkboxes[i].checked = false;
                        }
                    }
                    updateCategoryElementCheckboxes(checkboxes[i], action, catID);
                }
            }
        }
    }
    
    return;   
}


function updateCategoryElementCheckboxes(chk, action, catID)
{
    var tableBase = chk.parentNode.parentNode.parentNode;
    var checkboxes = tableBase.getElementsByTagName("input");

    updateElementCheckboxes(chk, action);

    for (var i = 0; i < checkboxes.length; i++)
    {
        if (checkboxes[i].parentNode.parentNode.id.indexOf('cat_child_' + catID + '_') > -1 && checkboxes[i].id.indexOf('chk_cat_') == -1)
        {
            if (chk.checked === true)
            {
                if (checkboxes[i].id.indexOf(action.toLowerCase()) > -1)
                {
                    checkboxes[i].checked = true;

                    if (action != "View")
                    {
                        updateElementCheckboxes(checkboxes[i], action);
                    }
                }
            }
            else
            {
                if (checkboxes[i].id.indexOf(action.toLowerCase()) > -1)
                {
                    if (action != "View")
                    {
                        checkboxes[i].checked = false;
                        updateElementCheckboxes(checkboxes[i], action);
                    }
                    else
                    {
                        if (checkIfRowShouldBeViewable(checkboxes[i], action) === false)
                        {
                            checkboxes[i].checked = false;
                        }
                    }
                }
            }
        }
    }

    return;
}

function updateElementCheckboxes(chk, action) 
{
    if (action != "View")
    {
        if (chk.checked === true)
        {
            chk.parentNode.parentNode.getElementsByTagName('input')[0].checked = true;
            chk.parentNode.parentNode.getElementsByTagName('input')[0].disabled = true;
        }
        else
        {
            if (checkIfRowShouldBeViewable(chk, action) === false)
            {
                chk.parentNode.parentNode.getElementsByTagName('input')[0].disabled = false;
            }
        }
    }

   return;

}

function checkViewChecks()
{
    var chks = document.getElementById('tableHolder').getElementsByTagName('input');
    for (var i = 0;i< chks.length; i++)
    {
        if (chks[i].id !== null && chks[i].id.indexOf('chk_element_view') > -1)
        {
            if (checkIfRowShouldBeViewable(chks[i], '') === true)
            {
                chks[i].parentNode.parentNode.getElementsByTagName('input')[0].disabled = true;
            }
        }
    }
}

function saveAccessRoleElementAccess(accessRoleID, accessRoleName, description)
{
    if (validateform(null) === false) { return; }
    
    var checkBoxes = document.getElementById('tableHolder').getElementsByTagName("input");

    var elementAccessRolesArray = new Array();
    var entityAccessRolesArray = new Array();

    for (var i = 0; i < checkBoxes.length; i++) {
        if (checkBoxes[i].type !== undefined && checkBoxes[i].type == "checkbox" && checkBoxes[i].id.indexOf('chk_element_') > -1 && checkBoxes[i].id.indexOf('tbCustomEntities') == -1) {
            var elementID = checkBoxes[i].id.substr(checkBoxes[i].id.lastIndexOf('_') + 1);

            if (elementAccessRolesArray[elementID] === undefined) {
                elementAccessRolesArray[elementID] = new Array();
            }

            if (checkBoxes[i].id.indexOf('chk_element_view_') > -1) {
                elementAccessRolesArray[elementID][0] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_add_') > -1) {
                elementAccessRolesArray[elementID][1] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_edit_') > -1) {
                elementAccessRolesArray[elementID][2] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_delete_') > -1) {
                elementAccessRolesArray[elementID][3] = checkBoxes[i].checked;
            }
            else {
                elementAccessRolesArray[elementID].push(false);
            }
        }
        else if (checkBoxes[i].type !== undefined && checkBoxes[i].type == "checkbox" && checkBoxes[i].id.indexOf('cev_chk_element_') > -1 && checkBoxes[i].id.indexOf('tbCustomEntities') > -1) //&& checkBoxes[i].parentNode.parentNode.innerText.indexOf('View - ') === 0)
        {
            var viewID = checkBoxes[i].id.substr(checkBoxes[i].id.lastIndexOf('_') + 1);
            var entityIDStrArr = checkBoxes[i].parentNode.parentNode.id.split('_');
            var entityID = entityIDStrArr[entityIDStrArr.length - 2];

            if (entityAccessRolesArray[entityID] === undefined) {
                entityAccessRolesArray[entityID] = new Array();
            }
            if (entityAccessRolesArray[entityID][2] === undefined) {
                entityAccessRolesArray[entityID][2] = new Array();
            }
            if (entityAccessRolesArray[entityID][2][viewID] === undefined) {
                entityAccessRolesArray[entityID][2][viewID] = new Array();
            }

            if (checkBoxes[i].id.indexOf('chk_element_view_') > -1) {
                entityAccessRolesArray[entityID][2][viewID][0] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_add_') > -1) {
                entityAccessRolesArray[entityID][2][viewID][1] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_edit_') > -1) {
                entityAccessRolesArray[entityID][2][viewID][2] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_element_delete_') > -1) {
                entityAccessRolesArray[entityID][2][viewID][3] = checkBoxes[i].checked;
            }
            else {
                entityAccessRolesArray[entityID][2][viewID].push(false);
            }
        }
        else if (checkBoxes[i].type !== undefined && checkBoxes[i].type == "checkbox" && checkBoxes[i].id.indexOf('chk_cat_') > -1 && checkBoxes[i].id.indexOf('tbCustomEntities') > -1) {
            var entityID = checkBoxes[i].id.substr(checkBoxes[i].id.lastIndexOf('_') + 1);

            if (entityAccessRolesArray[entityID] === undefined) {
                entityAccessRolesArray[entityID] = new Array();
            }
            if (entityAccessRolesArray[entityID][1] === undefined) {
                entityAccessRolesArray[entityID][1] = new Array();
                entityAccessRolesArray[entityID][1][0] = new Array();
            }

            if (checkBoxes[i].id.indexOf('chk_cat_view_') > -1) {
                entityAccessRolesArray[entityID][1][0][0] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_cat_add_') > -1) {
                entityAccessRolesArray[entityID][1][0][1] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_cat_edit_') > -1) {
                entityAccessRolesArray[entityID][1][0][2] = checkBoxes[i].checked;
            }
            else if (checkBoxes[i].id.indexOf('chk_cat_delete_') > -1) {
                entityAccessRolesArray[entityID][1][0][3] = checkBoxes[i].checked;
            }
            else {
                entityAccessRolesArray[entityID][1][0].push(false);
            }
        }
    }
    
    accessRoleName = document.getElementById(accessRoleName).value;
    description = document.getElementById(description).value;
    var canAdjustProjectCodes = false;
    var chkCntl = $('input[name$=' + chkCanEditProjectCodesObj + ']')[0];
    if (chkCntl != null) {
        canAdjustProjectCodes = chkCntl.checked;
    }
    
    var canAdjustDepartment = false;
    chkCntl = $('input[name$=' + chkCanEditDepartmentsObj + ']')[0];
    if (chkCntl != null) {
        canAdjustDepartment = chkCntl.checked;
    }

    var canAdjustCostCodes = false;
    chkCntl = $('input[name$=' + chkCanEditCostCodesObj + ']')[0];
    if (chkCntl != null) {
        canAdjustCostCodes = chkCntl.checked;
    }

    var mustHaveBankAccount = false;
    chkCntl = $('input[name$=' + chkMustHaveBankAccountObj + ']')[0];
    if (chkCntl != null) {
        mustHaveBankAccount = chkCntl.checked;
    }
    
    var roleAccessLevelDiv = document.getElementById("reportAccessLevelDiv").getElementsByTagName("input");
    var roleAccessLevel;
    
    for(var i = 0; i < roleAccessLevelDiv.length; i++)
    {
        if(roleAccessLevelDiv[i].checked === true) 
        {
            roleAccessLevel = roleAccessLevelDiv[i].id.substr(roleAccessLevelDiv[0].id.indexOf("radReportAccessLevel_") + 21);
        }
    }

    var maximumClaimAmount;
    if(claimMaximumAmount !== null) 
    {
        maximumClaimAmount = claimMaximumAmount.value;
    } 
    else 
    {
        maximumClaimAmount = 0;
    }
    
    var minimumClaimAmount;
    if(claimMinimumAmount !== null) 
    {
        minimumClaimAmount = claimMinimumAmount.value;
    }
    else
    {
        minimumClaimAmount = 0;
    }
    
    var reportableAccessRoles = new Array();
    
    var reportableAccessRoleCheckboxes = document.getElementById(linkedAccessRolesDivID).getElementsByTagName("input");
    
    for(var i = 0; i < reportableAccessRoleCheckboxes.length; i++)
    {
        if(reportableAccessRoleCheckboxes[i].checked === true) 
        {
            reportableAccessRoles.push(reportableAccessRoleCheckboxes[i].id.substr(16));
        }
   }

    var allowWebsiteAccess = $('input[name$=chkWebsite]').prop('checked');
    var allowMobileAccess = $('input[name$=chkMobile]').prop('checked');
    var allowApiAccess = $('input[name$=chkAPI]').prop('checked');
    var selectedFields = SEL.AccessRole.SelectedFields;
    var removedFields = SEL.AccessRole.RemovedFields;
    var exclusionType = SEL.AccessRole.ExclusionType;
    Spend_Management.svcAccessRoles.SaveAccessRoleElementAccess(accessRoleID, accessRoleName, description, roleAccessLevel, elementAccessRolesArray, entityAccessRolesArray, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveBankAccount, reportableAccessRoles, allowWebsiteAccess, allowMobileAccess, allowApiAccess, selectedFields, removedFields, exclusionType, returnMethod);
    return;
}

function deleteAccessRoleCallBack(data)
{
    if (data === false)
    {
        SEL.MasterPopup.ShowMasterPopup("This access role is currently in use.", "Error");
    }
}

function returnMethod(data)
{
    if (isNaN(data) === false) 
    {
        switch (data) 
        {
            case "-1":
                SEL.MasterPopup.ShowMasterPopup("This access role name is already in use.", "Warning");
                break;
            case "-2":
                SEL.MasterPopup.ShowMasterPopup("An error occurred saving your access role.", "Error");
                break;
            default:
                window.location = "accessRoles.aspx";
                break;
        }
        
        
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup("Insufficient AccessRoles to perform this action.", "Warning");
    }
}

function ChangeCategoryDisplay(catID) 
{
    var trs = document.getElementById('tableHolder').getElementsByTagName("tr");
    var image;

    for (var i = 0; i < trs.length; i++)
    {
        if (trs[i].id.indexOf("_cat_child_" + catID + "_") > -1)
        {
            if (trs[i].style.display == "none")
            {
                trs[i].style.display = "";
                image = document.getElementById("cat_parent_" + catID + "_expand_image").setAttribute("src", "/shared/images/buttons/close.gif");
            } else
            {
                trs[i].style.display = "none";
                image = document.getElementById("cat_parent_" + catID + "_expand_image").setAttribute("src", "/shared/images/buttons/open.gif");
            }
        }
    }
    
    return;
}

function ValidateProductAccess(source, args) {

    var valid = $('span[groupname=productAccess]').children(':checked').length > 0;

    if (valid) {
        // Disable validators - set mandatory off on all labels.
        $('.productAccess').each(function (key, value)
        {
            if (value.innerHTML.indexOf('*') !== -1) {
                value.innerHTML = value.innerHTML.replace('*', '');
                $(value).removeClass('mandatory');
            }
        });
        $('.cvProductAccess').each(function (key, value) {
            SwitchValidator(value, true);
        });
    } else {
        // Enable validators = set mandatory in all labels.
        $('.productAccess').each(function (key, value) {
            if (value.innerHTML.indexOf('*') === -1) {
                value.innerHTML += '*';
                $(value).addClass('mandatory');
            }
        });
        $('.cvProductAccess').each(function (key, value) {
            SwitchValidator(value, false);
        });
    }

    if (args !== undefined && args !== null) {
        args.IsValid = valid;
    }
}

function SwitchValidator(validator, isvalid) {

    validator.isvalid = isvalid;
    ValidatorUpdateDisplay(validator);
}
