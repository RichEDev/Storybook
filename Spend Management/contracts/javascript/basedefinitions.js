function baseDefinitions() {
    this.recId = -1;
    this.fieldValues = new Array(); 
    this.bdColumns = new Array();
    this.bdColumnType = new Array();
    this.bdTableId = '';
    this.element = 0;
    
    this.addColumn = function(fieldid, type) {
        this.bdColumns.push(fieldid);
        this.bdColumnType.push(type);
    };

    this.gridID = function() {
        return 'bdGrid_' + this.bdTableId.replace(/-/g,"_");
    }

    this.editDefinition = function (recordid)
    {
        this.recId = recordid;

        Spend_Management.svcBaseDefinitions.getBaseDefinitionRecord(this.element, this.recId, this.bdColumns,
            function (data)
            {
                var modalPanel = $find(modalDefEdit);

                if (modalPanel !== null)
                {
                    this.fieldValues = data.slice();
                    var cntl;
                    var lbl;

                    for (var x = 0; x < data.length; x++)
                    {
                        switch (data[x].fieldType)
                        {
                            case 'X':
                                cntl = document.getElementById(getFieldKey('chk', data[x]));
                                if (cntl !== null)
                                {
                                    if (data[x].fieldValue === '1' || data[x].fieldValue === 'true' || data[x].fieldValue === 'True')
                                    {
                                        cntl.checked = true;
                                    }
                                    else
                                    {
                                        cntl.checked = false;
                                    }
                                }
                                break;
                            case 'N':
                                if (data[x].genlist === true || data[x].isValueList === true)
                                {
                                    cntl = document.getElementById(getFieldKey('lst', data[x]));
                                    if (cntl !== null)
                                    {
                                        if (data[x].fieldName == 'associatedSubAccountID')
                                        {
                                            lbl = document.getElementById(getFieldKey('lbl', data[x]));

                                            if (recordid >= -1)
                                            {
                                                cntl.style.display = 'none';
                                                lbl.style.display = 'none';
                                            }
                                            else
                                            {
                                                cntl.style.display = '';
                                                lbl.style.display = '';
                                            }
                                        }
                                        else
                                        {
                                            for (var y = 0; y < cntl.options.length; y++)
                                            {
                                                if (cntl.options[y].value == data[x].fieldValue)
                                                {
                                                    cntl.options[y].selected = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    cntl = document.getElementById(getFieldKey('txt', data[x]));
                                    if (cntl !== null)
                                    {
                                        cntl.value = data[x].fieldValue;
                                    }
                                }
                                break;
                            default:
                                cntl = document.getElementById(getFieldKey('txt', data[x]));
                                if (cntl !== null)
                                {
                                    cntl.value = data[x].fieldValue;
                                }
                                break;
                        }
                    }

                    modalPanel.show();
                }
            },
            function (error)
            {
                SEL.MasterPopup.ShowMasterPopup('An error occurred trying to retrieve the edit field values\n' + error, 'Edit Definition');
            });
    };

    this.deleteDefinition = function (recordid)
    {
        if (confirm('Click OK to confirm definition deletion'))
        {
            Spend_Management.svcBaseDefinitions.deleteDefinition(this.element, recordid,
                function (result)
                {
                    if (result === -1)
                    {
                        SEL.MasterPopup.ShowMasterPopup('The definition you attempted to delete is currently in use, it must be removed from all records before it can be deleted.', 'Delete Definition');
                    }
                    else
                    {
                        baseDefObject.getBaseDefinitionTable();
                    }
                },
                function (error)
                {
                    SEL.MasterPopup.ShowMasterPopup('A problem occurred while attempting to delete the definition.\n' + error, 'Delete Definition');
                });
        }
    };

    this.archiveDefinition = function (recordid)
    {
        if (confirm('Click OK to confirm definition archive/unarchive'))
        {
            Spend_Management.svcBaseDefinitions.ArchiveDefinition(this.element, recordid,
                function (result)
                {
                    baseDefObject.getBaseDefinitionTable();
                },
                function (error)
                {
                    SEL.MasterPopup.ShowMasterPopup('A problem occurred while attempting to archive the definition.\n' + error, 'Delete Definition');
                });
        }
    };

    this.getBaseDefinitionTable = function () {
        PageMethods.getGrid(this.gridID(),
        function (data) {
            var cntl = $g('bdefDivPanel');
            if (cntl !== null) {
                cntl.innerHTML = data[2];
                SEL.Grid.updateGrid(data[1]);
            }
        },
        function (error) {
            SEL.MasterPopup.ShowMasterPopup('error occurred while attempting to generate the base definition grid\n' + error, 'Retrieve Definitions');
        });
    };

    this.saveDefinition = function () {
        if (validateform(validationGroup) == false)
            return;

        for (var x = 0; x < fieldValues.length; x++) {
            switch (fieldValues[x].fieldType) {
                case 'X':
                    cntl = document.getElementById(getFieldKey('chk', fieldValues[x]));
                    if (cntl !== null) {
                        fieldValues[x].fieldValue = cntl.checked ? 1 : 0;
                    }
                    break;
                case 'N':
                    if (fieldValues[x].genlist == true || fieldValues[x].isValueList == true) {
                        cntl = document.getElementById(getFieldKey('lst', fieldValues[x]));
                        if (cntl !== null) {
                            fieldValues[x].fieldValue = cntl.options[cntl.selectedIndex].value;
                        }
                    }
                    else {
                        cntl = document.getElementById(getFieldKey('txt', fieldValues[x]));
                        if (cntl !== null) {
                            fieldValues[x].fieldValue = cntl.value;
                        }
                    }
                    break;
                default:
                    cntl = document.getElementById(getFieldKey('txt', fieldValues[x]));
                    if (cntl !== null) {
                        fieldValues[x].fieldValue = cntl.value;
                    }
                    break;
            }
        }
        Spend_Management.svcBaseDefinitions.saveDefinition(this.element, this.recId, fieldValues, function (data) {
            var modalPanel = $find(modalDefEdit);

            if (data === 0 || data === -1) {
                SEL.MasterPopup.ShowMasterPopup('This definiton already exists and cannot be added', 'Duplicate Definition');
                return;
            }

            if (modalPanel !== null) {
                //values = null;
                //recId = 0;
                modalPanel.hide();
            }

            baseDefObject.getBaseDefinitionTable();

        }, function (error) {
            SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to save the base definition.\n' + error, 'Save Definition');
        });
    };
}

function getFieldKey(prefix, fieldDef) {
    var tmpKey = 'ctl00_contentmain_' + prefix + fieldDef.tableName + '_' + fieldDef.fieldName;
    return tmpKey;
};
