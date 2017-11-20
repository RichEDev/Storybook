function deleteUserdefinedGrouping(userdefinedGroupID) {

    currentRowID = userdefinedGroupID;
    if (confirm('Are you sure you wish to delete the selected grouping?')) {
        PageMethods.deleteUserdefinedGrouping(accountid, employeeid, userdefinedGroupID);
        SEL.Grid.deleteGridRow('gridGroupings', currentRowID);
    }
}

function displayAssociationTable() {
    var ddl = document.getElementById(ddlArea);
    if (ddl != null) {
        var areaID = ddl.options[ddl.selectedIndex].value;

        Spend_Management.svcUserFieldGroupings.getAssociationGrid(areaID, groupingID, onGetGridSuccess, onGetGridFail);

    }
}

function onGetGridFail(error) {
    alert('displayAssociationTable() method failed\n' + error);
    return;
}

function onGetGridSuccess(data) {
    var div = document.getElementById('divAssociations');
    if (div != null) {
        if (data.length > 0) {
            div.innerHTML = parseScript(data[2]);
        } else {
            div.innerHTML = "";
        }
    }
}

function saveGrouping() {
    var groupname = document.getElementById(txtgroupname).value;
    var order = 0;
//    if (order == '') {
//        order = 0;
//    }
    var areaID;
    var ddl = document.getElementById(ddlArea);
    if (ddl != null) {
        areaID = ddl.options[ddl.selectedIndex].value;
    }
    var selectedItems = SEL.Grid.getSelectedItemsFromGrid('assocGrid_' + areaID.replace(/-/g, '_'));

    PageMethods.saveGrouping(groupingID, groupname, areaID, order, selectedItems, onSaveSucess, onSaveFail);
}

function onSaveSucess(retStr) {
    if (retStr != '') {
        alert(retStr);
    }
    else {
        window.location.href = 'userdefinedFieldGroupings.aspx';
    }
}

function onSaveFail(error) {
    alert('Saving of user defined field grouping failed.\n' + error);
}