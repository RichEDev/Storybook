var docID = 0;

function DeleteTemplate(documentId) {
    if (confirm('Click OK to confirm deletion of the template')) {
        docID = documentId;
        Spend_Management.svcDocumentTemplates.deleteTemplate(documentId, deleteSuccess, deleteFail);
    }
}

function deleteSuccess(data) {
    if (data == 0) {
        var table_row = document.getElementById('ctl00_contentmain_row' + docID);
        if (table_row != null) {
            table_row.style.display = 'none';
        }
    }
    else {
        SEL.MasterPopup.ShowMasterPopup('Could not delete template as it may be assigned to a document configuration.', 'Delete Template Error');
    }
    docID = 0;

    return;
}

function deleteFail(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to delete the template. ' + error, 'Delete Template Error');
    alert('An error occurred attempting to delete the template\n' + error);
    docID = 0;
    return;
}

function CopyTemplate(documentId) {
    docID = documentId;
    $find(copynewmodalcntl).show();
}

function SaveTemplateCopy() {
    var newTitleCntl = document.getElementById(copynewtitletxt);
    if (newTitleCntl != null) {
        var newDescriptionCntl = document.getElementById(copynewdesctxt);
        var desc = '';
        if (newDescriptionCntl != null) {
            desc = newDescriptionCntl.value;
        }
        Spend_Management.svcDocumentTemplates.CopyTemplate(docID, newTitleCntl.value, desc, onCopyComplete, onCopyError);
    }
}

function onCopyComplete(data) {
    window.location.href = 'admindoctemplates.aspx?ret=' + data[1];
}

function onCopyError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to save the new template. Please contact your administrator.', 'Template Save Error');
}

function deleteDocMergeAssociation(docmergeassociationid) {
    if (confirm('Are you sure you wish to delete the selected Document Template Association?\nPlease note that any related Torch History Attachments will also be deleted.')) {
        Spend_Management.svcDocumentTemplates.deleteDocMergeAssociation(docmergeassociationid, onDeleteMergeAssociationComplete, onDeleteMergeAssociationError);
    }
}

function onDeleteMergeAssociationError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to delete the merge association for the document template. Please contact your administrator.', 'Merge Association Delete Error');
}

function onDeleteMergeAssociationComplete(data) {
    if (data !== 0) {
        SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to delete the merge association for the document template. Please contact your administrator.', 'Merge Association Delete Error');
    }
    else {
        getMergeAssociationGrid(docID);
        getAvailableMergeAssociationsGrid(docID);
    }
}

function getMergeAssociationGrid(docID) {
    Spend_Management.svcDocumentTemplates.getMergeAssociationGrid(docID, function (data) { document.getElementById("divGrid").innerHTML = data[2]; SEL.Grid.updateGrid(data[1]); });
}

function getAvailableMergeAssociationsGrid(docID) {
    Spend_Management.svcDocumentTemplates.getAvailableMergeAssociationsGrid(docID, function (data) { document.getElementById("divGridAvailable").innerHTML = data[2]; SEL.Grid.updateGrid(data[1]); });
}

function launchAddAssocsModal() {
    $find(mdlAddAssocs).show();

    return;
}

function ensureSelectionMade(gridid) {
    if (SEL.Grid.getSelectedItemsFromGrid(gridid).length == 0) {
        SEL.MasterPopup.ShowMasterPopup('An association is mandatory.', 'Page Validation Failed');
        return false;
    }
    return true;
}
