//###############################Custom Fields###########################################

var RelatedFieldID = '';
var editCustField = false;

function showCustomFieldModal(isAdd)
{
    if (isAdd)
    {
        editCustField = false;
        fieldID = '';
        clearCustomFieldModal();
        showCustFieldOptions();
    }
    
    var modal = $find(modCustomFields);
    modal.show();
    return;
}

function showCustFieldOptions()
{
    var ddlFieldType = document.getElementById(ddlCustFieldType);
    var fieldType = ddlFieldType.options[ddlFieldType.selectedIndex].value;

    document.getElementById('divAlias').style.display = 'none';
    document.getElementById('divFunction').style.display = 'none';

    switch (fieldType)
    {
        case '1':
            document.getElementById('divAlias').style.display = '';
            break;
        case '2':
            document.getElementById('divFunction').style.display = '';
            break;
    }
    return;
}

function LoadFields()
{
    var ddlTables = document.getElementById(ddlTablesID);
    var tableGUID = ddlTables.options[ddlTables.selectedIndex].value;
    Spend_Management.svcCustomFields.GetTableFields(tableGUID, LoadFieldsComplete, WebServiceError);
    return;
}

function LoadFieldsComplete(result)
{
    var ddlFields = document.getElementById(ddlFieldsID);

    var option;

    ddlFields.options.length = 0;

    option = document.createElement("option");
    option.text = "[None]";
    option.value = "0";
    ddlFields.options.add(option);

    for (var i = 0; i < result.length; i++)
    {
        option = document.createElement("option");
        option.text = result[i].Description;
        option.value = result[i].FieldID;
        ddlFields.options.add(option);
    }

    if (editCustField)
    {
        for (var i = 0; i < ddlFields.options.length; i++)
        {
            if (ddlFields.options[i].value == RelatedFieldID)
            {
                ddlFields.selectedIndex = i;
                break;
            }
        }
    }
    return;
}

function editCustomField(id)
{
    fieldID = id;
    editCustField = true;

    Spend_Management.svcCustomFields.getCustomField(fieldID, editCustomFieldComplete, WebServiceError);
    return;
}

function editCustomFieldComplete(data)
{
    var ddlFieldCat = document.getElementById(ddlCustFieldType);
    ddlFieldCat.disabled = true;

    for (var i = 0; i < ddlFieldCat.options.length; i++)
    {
        if (ddlFieldCat.options[i].value == data.FieldCat)
        {
            ddlFieldCat.selectedIndex = i;
            break;
        }
    }

    var ddlTables = document.getElementById(ddlTablesID);

    for (var i = 0; i < ddlTables.options.length; i++)
    {
        if (ddlTables.options[i].value == data.TableID)
        {
            ddlTables.selectedIndex = i;
            break;
        }
    }

    showCustFieldOptions();

    document.getElementById(txtCustFieldDesc).value = data.Description;

    switch (data.FieldCat)
    {
        case 1:
            RelatedFieldID = data.RelatedFieldID;
            LoadFields();
            break;

        case 2:
            document.getElementById(txtCustFieldName).value = data.FieldName;

            var ddlDataType = document.getElementById(ddlDataTypeID);

            for (var i = 0; i < ddlDataType.options.length; i++)
            {
                if (ddlDataType.options[i].value == data.DataType)
                {
                    ddlDataType.selectedIndex = i;
                    break;
                }
            }

            break;
    }

    showCustomFieldModal(false);

    return;
}

function clearCustomFieldModal()
{
    var ddlFieldCat = document.getElementById(ddlCustFieldType);
    ddlFieldCat.disabled = false;
    document.getElementById(ddlCustFieldType).selectedIndex = 0;
    document.getElementById(txtCustFieldDesc).value = '';
    document.getElementById(ddlFieldsID).selectedIndex = 0;
    document.getElementById(ddlTablesID).selectedIndex = 0;
    document.getElementById(txtCustFieldName).value = '';
    document.getElementById(ddlDataTypeID).selectedIndex = 0;
}

function SaveCustomField()
{
    var ddlFieldType = document.getElementById(ddlCustFieldType);
    var fieldType = ddlFieldType.options[ddlFieldType.selectedIndex].value;

    customFieldObj = new CustomFieldObject();

    customFieldObj.FieldCat = fieldType;
    customFieldObj.Description = document.getElementById(txtCustFieldDesc).value;
    customFieldObj.FieldID = fieldID;
    
    var ddlTables = document.getElementById(ddlTablesID);
    customFieldObj.TableID = ddlTables.options[ddlTables.selectedIndex].value;

    switch (fieldType)
    {
        case "1":
            var ddlFields = document.getElementById(ddlFieldsID);
            customFieldObj.RelatedFieldID = ddlFields.options[ddlFields.selectedIndex].value;
            break;

        case "2":

            customFieldObj.FieldName = document.getElementById(txtCustFieldName).value;

            var ddlDataType = document.getElementById(ddlDataTypeID);
            customFieldObj.DataType = ddlDataType.options[ddlDataType.selectedIndex].value;
            customFieldObj.RelatedFieldID = '';
            break;
    }

    Spend_Management.svcCustomFields.SaveCustomField(customFieldObj, SaveCustomFieldComplete, WebServiceError);
    return;
}

function SaveCustomFieldComplete(data)
{
    fieldID = data;

    Spend_Management.svcCustomFields.getFieldGrid(RefreshFieldGridComplete, WebServiceError);
    
    var modal = $find(modCustomFields);
    modal.hide();
    return;
}

function RefreshFieldGridComplete(data)
{
    document.getElementById(pnlFieldID).innerHTML = data;
    return;
}

function DeleteCustomField(ID)
{
    currentRowID = ID;
    if (confirm('Are you sure you wish to delete the selected custom field?'))
    {
        Spend_Management.svcCustomFields.DeleteCustomField(ID, DeleteCustomFieldComplete, WebServiceError);
    }
    return;
}

function DeleteCustomFieldComplete(returnCode)
{
    if (returnCode === 0)
    {
        deleteGridRow('gridFields', currentRowID);
    }
    else
    {
        DisplayReturnMessage(returnCode);
    }
    return;
}

//####Custom Field Object####
function CustomFieldObject()
{
    this.FieldID = null;
    this.FieldName = null;
    this.Description = null;
    this.DataType = null;
    this.TableID = null;
    this.FieldCat = null;
    this.RelatedFieldID = null;
}