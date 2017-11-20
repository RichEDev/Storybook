var divID;
var attachDocType;
var ElementID;

function deleteAttachment(attachmentid, tableid, id, docType, gridDomId) 
{
    attachDocType = docType;
    ElementID = id;

    if (!gridDomId) {
        gridDomId = 'gridAttachments';
    }
    
    currentRowID = attachmentid;
    if (confirm('Are you sure you wish to delete the selected attachment?')) 
    {
        Spend_Management.svcAttachments.deleteAttachment(tableid, attachmentid, id, docType, function () {
            deleteAttachmentSuccess(gridDomId);
        }, deleteAttachmentError);
    }
}

function deleteAttachmentSuccess(gridDomId) {

    isAttach = false; // reset the isAttach variable for cars modal otherwise pops modal again on save

    var grid = document.getElementById('tbl_' + gridDomId);
    if (grid != null) {
        SEL.Grid.deleteGridRow(gridDomId, currentRowID);
    }
    else 
    {
        if (divID != "") 
        {
            refreshAttachDiv(divID, attachDocType, ElementID);
        }
    }
	return;
}

function deleteAttachmentError(error) {
	alert('An error occurred attempting to delete the selected attachment');
	return;
}

function addAttachmentTableEntry(gridid, data) 
{
	var grid = document.getElementById('tbl_' + gridid);
	if (grid != null) {
		var emptyTextRow = document.getElementById('tbl_' + gridid + '_emptytext');
		if (emptyTextRow != null) {
			emptyTextRow.style.display = 'none';
		}

		var newRow = grid.insertRow(-1);
		newRow.id = 'tbl_' + gridid + '_' + data[0];
		var newCell;

		newCell = newRow.insertCell(-1);
		newCell.setAttribute("class", "row1");
		newCell.setAttribute("className", "row1");
		newCell.innerHTML = "<a href=\"javascript:deleteAttachment(" + data[0] + ",'" + data[1] + "',0,0);\"><img alt=\"Delete\" src=\"" + appPath + "/shared/images/icons/delete2.png\"></a>";

		newCell = newRow.insertCell(-1);
		newCell.setAttribute("class", "row1");
		newCell.setAttribute("className", "row1");
		newCell.innerHTML = "<a href=\"javascript:viewAttachment(" + data[0] + ",'" + data[1] + "');\"><img alt=\"View\" src=\"" + appPath + "/shared/images/icons/16/plain/zoom_in.png\"></a>";

        // if there's a publish column then add a cell for it
		if ($(grid).find("th img[alt=Publish]").length) {
		    newCell = newRow.insertCell(-1);
		    newCell.setAttribute("class", "row1");
		    newCell.setAttribute("className", "row1");
		    newCell.innerHTML = '<a href="javascript:toggleTorchAttachmentPublished(' + data[0] + ',\'' + data[1] + '\', \'' + gridid + '\');"><img alt="View" src="/static/icons/16/new-icons/document_up.png"></a>';
		}

		newCell = newRow.insertCell(-1);
		newCell.setAttribute("class", "row1");
		newCell.setAttribute("className", "row1");
		newCell.innerHTML = data[2]; // title

		newCell = newRow.insertCell(-1);
		newCell.setAttribute("class", "row1");
		newCell.setAttribute("className", "row1");
		newCell.innerHTML = data[4]; // description

		newCell = newRow.insertCell(-1);
		newCell.setAttribute("class", "row1");
		newCell.setAttribute("className", "row1");
		newCell.innerHTML = data[3]; // filename
		SEL.Grid.refreshGrid(gridid, 1);
}
	return;
}

function viewAttachment(attachmentid, tableid) {
    window.open('/shared/getDocument.axd?attid=' + attachmentid + '&tableid=' + tableid);
}

function viewFieldLevelAttachment(fileId, entityId, viewId, recordId, fileInputId, isLookUpDisplayField) {
    entityId = entityId || 0;
    viewId = viewId || 0;
    recordId = recordId || 0;
    fileInputId = fileInputId || null;
    isLookUpDisplayField = isLookUpDisplayField || null;

    var referenceValue;
    if (isLookUpDisplayField === true) {
        referenceValue = $("#" + fileInputId ).attr("referenceValue");
    } else {
        referenceValue = $("#" + fileInputId + "_attachmentLink").attr("referenceValue");
    }
   
    if (fileId !== referenceValue) {
        isLookUpDisplayField = false;
    }

    if (viewId === 0 || entityId === 0 || recordId === 0 || fileInputId === null || fileId !== referenceValue) {
        window.open('/shared/getDocument.axd?fileID=' + fileId + "&isLookUpDisplayField=" + isLookUpDisplayField);
    } else {
        window.open("/shared/getDocument.axd?fileID=" + fileId + "&viewid=" + viewId + "&entityid=" + entityId + "&recordid=" + recordId + "&isLookUpDisplayField=" + isLookUpDisplayField);
    }
}

// UploadAttachment.ascx functions
var divFileUpload;
var divProgress;
var ifrFile;

function initFileUpload(iFrameName, height) {
  
    $('#iFrEmailAttach').contents().find('#btnUpload').css("display", "none");
    $('#iFrEmailAttach').contents().find('#cmdCancelAttach').css("display", "none");
 
	divFileUpload = document.getElementById(divfileuploadID);
	divProgress = document.getElementById(divprogressID);
	ifrFile = document.getElementById(iFrameName);

	if (ifrFile != null) {
	    var btnUpload = ifrFile.contentWindow.document.getElementById('btnUpload');
	    if (height > 0) {
            //fixing buttons on chrome (GL attachment's pop up)
	        if (navigator.userAgent.indexOf("Chrome") != -1) {
	            height = height + 40 ;
	        }
	        divFileUpload.style.height = height + 'px';
	        divProgress.style.height = height + 'px';
	    }

	    if (btnUpload != undefined) {
	        btnUpload.onclick = function(event)
	        {
	            ifrFile = document.getElementById(iFrameName);
	            if (ifrFile.contentWindow.Page_ClientValidate() === false)
	            {
	                var ControlValidators = ifrFile.contentWindow.Page_Validators;
	                validationErrorMessage = "<ul style=\"margin:0; padding: 0;list-style-type: none;\">";

	                for (i = 0; i < ControlValidators.length; i++) {
	                    if (ControlValidators[i].isvalid == false && typeof (ControlValidators[i].errormessage) == "string" ) {
	                        validationErrorMessage += "<li>" + ControlValidators[i].errormessage + "</li>";
	                    }
	                }

	                validationErrorMessage += "</ul>";
	                if (SEL.MasterPopup != null) {
	                    if (typeof SEL.MasterPopup.ShowMasterPopup == 'function') {
	                        typeof SEL.MasterPopup.ShowMasterPopup(validationErrorMessage, "Page Validation Failed");
	                    }
	                }
	                else {
	                    validationErrorMessage = "";
	                    for (i = 0; i < ControlValidators.length; i++) {
	                        if (Page_Validators[i].isvalid == false && typeof (ControlValidators[i].errormessage) == "string" && (validationGroup == null || ControlValidators[i].validationGroup == validationGroup)) {
	                            validationErrorMessage += "-" + ControlValidators[i].errormessage + "\n";
	                        }
	                    }
	                    if (validationErrorMessage.length > 0) {
	                        alert(validationErrorMessage);
	                    }
	                } return;
	            }

	            divFileUpload.style.display = 'none';
                divProgress.style.display = 'block';
            };
        }
    }
}

function UploadComplete(message, isError, attachmentid, tableid, attachment_title, attFilename, description) 
{
	divFileUpload.style.display = 'block';
	divProgress.style.display = 'none';
    isAttach = false; // reset the isAttach variable for cars modal otherwise pops modal again on save

	if (attFilename.length > 0)
	{
	    var data = new Array();
	    data.push(attachmentid);
	    data.push(tableid);
	    data.push(attachment_title);
	    data.push(attFilename);
	    data.push(description);
	    addAttachmentTableEntry('gridAttachments', data); // this is in Attachment.js
	    if (tableid == '65046502-92fa-4027-bd7c-c6bceac26352') // update cache entry for cars or poolcars page
	    {
	        UpdateCarCache();
	    }
	    ifrFile.contentWindow.doClose(); // this is in the FileUploadIFrame    	
	}
	else
	{
	    if (message.length > 0)
	    {
	        ifrFile.contentWindow.document.getElementById('lblStatus').innerHTML = message;

	        if (isError)
	        {
	            if (ifrFile.contentWindow.document.getElementById('fileUploadBox') != undefined) {
	                ifrFile.contentWindow.document.getElementById('fileUploadBox').value = '';
	                return;
	            }
	            // you cant focus on a fileuploadbox for security reasons
	            //ifrFile.contentWindow.document.getElementById('fileUploadBox').focus();
	        }
	    }
	}
}

function refreshAttachDiv(IDOfDiv, docType, ID) 
{
    isAttach = false; // reset the isAttach variable for cars modal otherwise pops modal again on save

    divID = IDOfDiv;
    Spend_Management.svcAttachments.getAttachmentData(docType, ID, refreshAttachDivComplete);
}

function refreshAttachDivComplete(data)
{
    var div;

    if (data[0] != 0)
    {
        div = document.getElementById(divID);

        if (data[1] == 0)
        {
            switch (data[0])
            {
                case 1:
                    div.innerHTML = "<a href=\"javascript:attachDocType = 1;isAttach = true;currentAction = 'LicenceAttachment';appendLicenceAttachment();\"><img src=\"" + appPath + "/shared/images/icons/16/plain/add2.png\" alt=\"\" id=\"butAttachLicence\" /></a>";
                    break;
                case 2:
                    div.innerHTML = "<a href=\"javascript:attachDocType = 2;isAttach = true;divID = 'taxDiv';appendAttachment();\"><img src=\"" + appPath + "/shared/images/icons/16/plain/add2.png\" alt=\"\" id=\"butAttachTax\" /></a>";
                    break;
                case 3:
                    div.innerHTML = "<a href=\"javascript:attachDocType = 3;isAttach = true;divID = 'MOTDiv';appendAttachment();\"><img src=\"" + appPath + "/shared/images/icons/16/plain/add2.png\" alt=\"\" id=\"butAttachMOT\" /></a>";
                    break;
                case 4:
                    div.innerHTML = "<a href=\"javascript:attachDocType = 4;isAttach = true;divID = 'insuranceDiv';appendAttachment();\"><img src=\"" + appPath + "/shared/images/icons/16/plain/add2.png\" alt=\"\" id=\"butAttachInsurance\" /></a>";
                    break;
                case 5:
                    div.innerHTML = "<a href=\"javascript:attachDocType = 5;isAttach = true;divID = 'serviceDiv';appendAttachment();\"><img src=\"" + appPath + "/shared/images/icons/16/plain/add2.png\" alt=\"\" id=\"butAttachservice\" /></a>";
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (data[0])
            {
                case 1:
                    div.innerHTML = "<img border=\"0\" src=\"" + appPath + "/shared/images/icons/24/plain/view.png\" style=\"cursor:pointer;cursor:hand\" onclick=\"javascript:window.open(appPath + '/shared/getDocument.axd?attid=" + data[1] + "&tableid=" + data[3] + "\');\"><a href=\"javascript:divID = 'licenceDiv';deleteAttachment(" + data[1] + ",'" + data[3] + "'," + data[2] + "," + data[0] + ");UpdateCarCache();\"><img src=\"" + appPath + "/shared/images/icons/24/plain/delete2.gif\" alt=\"\" id=\"butDeleteLicence\" /></a>";
                    break;
                case 2:
                    div.innerHTML = "<img border=\"0\" src=\"" + appPath + "/shared/images/icons/24/plain/view.png\" style=\"cursor:pointer;cursor:hand\" onclick=\"javascript:window.open(appPath + '/shared/getDocument.axd?attid=" + data[1] + "&tableid=" + data[3] + "\');\"><a href=\"javascript:divID = 'taxDiv';deleteAttachment(" + data[1] + ",'" + data[3] + "'," + data[2] + "," + data[0] + ");UpdateCarCache();\"><img src=\"" + appPath + "/shared/images/icons/24/plain/delete2.gif\" alt=\"\" id=\"butDeleteTax\" /></a>";
                    break;
                case 3:
                    div.innerHTML = "<img border=\"0\" src=\"" + appPath + "/shared/images/icons/24/plain/view.png\" style=\"cursor:pointer;cursor:hand\" onclick=\"javascript:window.open(appPath + '/shared/getDocument.axd?attid=" + data[1] + "&tableid=" + data[3] + "\');\"><a href=\"javascript:divID = 'MOTDiv';deleteAttachment(" + data[1] + ",'" + data[3] + "'," + data[2] + "," + data[0] + ");UpdateCarCache();\"><img src=\"" + appPath + "/shared/images/icons/24/plain/delete2.gif\" alt=\"\" id=\"butDeleteMOT\" /></a>";
                    break;
                case 4:
                    div.innerHTML = "<img border=\"0\" src=\"" + appPath + "/shared/images/icons/24/plain/view.png\" style=\"cursor:pointer;cursor:hand\" onclick=\"javascript:window.open(appPath + '/shared/getDocument.axd?attid=" + data[1] + "&tableid=" + data[3] + "\');\"><a href=\"javascript:divID = 'insuranceDiv';deleteAttachment(" + data[1] + ",'" + data[3] + "'," + data[2] + "," + data[0] + ");UpdateCarCache();\"><img src=\"" + appPath + "/shared/images/icons/24/plain/delete2.gif\" alt=\"\" id=\"butDeleteInsurance\" /></a>";
                    break;
                case 5:
                    div.innerHTML = "<img border=\"0\" src=\"" + appPath + "/shared/images/icons/24/plain/view.png\" style=\"cursor:pointer;cursor:hand\" onclick=\"javascript:window.open(appPath + '/shared/getDocument.axd?attid=" + data[1] + "&tableid=" + data[3] + "\');\"><a href=\"javascript:divID = 'serviceDiv';deleteAttachment(" + data[1] + ",'" + data[3] + "'," + data[2] + "," + data[0] + ");UpdateCarCache();\"><img src=\"" + appPath + "/shared/images/icons/24/plain/delete2.gif\" alt=\"\" id=\"butDeleteService\" /></a>";
                    break;
                default:
                    break;
            }
        }
    }
}

function SetFileUploadReplacementFileName(val, hyperlink, replacementLiteral, typeControl, fileUploadControl, filePathControl, fileNameControl, includeImageLibrary, validatorControl, changedControl, deleteIconControl, isMandatory) {
    var theValidator = $g(validatorControl);
    theValidator.isvalid = true;
    ValidatorUpdateDisplay(theValidator);
    $('#' + changedControl).val('changed');
    var i = val.lastIndexOf("\\");
    document.getElementById(hyperlink).hidden = true;
    document.getElementById(hyperlink).style.display =  'none';  
    var selected = val.substring(i + 1);
    var selectedText = selected;
    if (selected.length > 6) {
        selected = selected.substring(0, 6) + '...';
    }
    $('#' + replacementLiteral).html(selected);
    $('#' + replacementLiteral)[0].title = selectedText;
    $('#' + typeControl).val('FileBrowser');
    ResetImageLibraryControls(filePathControl, fileNameControl);
    if (includeImageLibrary === true) {
        $f('mdlImageLibrary').hide();
    }
    if (isMandatory == "False") {
        $('img[id*=' + deleteIconControl + ']').css('display', '');
    }
    return true;
        }

function ResetImageLibraryControls (filePathControl, fileNameControl) {
    $('#' + filePathControl).val('');
    $('#' + fileNameControl).val('');
    }

function ShowImageBrowserPopup(pathControl, nameControl, uploadControl, typeControl, hyperLinkControl, replacementControl, changedControl, validatorControl, iconName, imageLibraryModalId, deleteIconControl, isMandatory) {
    var selected = iconName;
    var imageLabelTextControl = $('#' + replacementControl);
    if (imageLabelTextControl.length > 0 && imageLabelTextControl[0].title !== '')
    {
        selected = imageLabelTextControl[0].title;
    }
    jQuery.ImageBrowser.options['PathControl'] = pathControl;
    jQuery.ImageBrowser.options['NameControl'] = nameControl;
    jQuery.ImageBrowser.options['FileUploadControl'] = uploadControl;
    jQuery.ImageBrowser.options['UploadTypeControl'] = typeControl;
    jQuery.ImageBrowser.options['HyperlinkControl'] = hyperLinkControl;
    jQuery.ImageBrowser.options['ReplacementControl'] = replacementControl;
    jQuery.ImageBrowser.options['ChangeFlagControl'] = changedControl;
    jQuery.ImageBrowser.options['ValidatorControl'] = validatorControl;
    jQuery.ImageBrowser.options['SelectedIcon'] = selected;
    jQuery.ImageBrowser.options['DeleteIconControl'] = $('img[id*=' + deleteIconControl + ']')[0].id;
    jQuery.ImageBrowser.options['IsMandatory'] = isMandatory;
    jQuery.ImageBrowser();
    SEL.Common.ShowModal(imageLibraryModalId);
}

function ClearAttachment(changedControlId, hyperlinkControlId, replacementControlId, deleteIconControl, uploadTypeControl)
{
    $('#' + changedControlId).val('changed');
    $('#' + uploadTypeControl).val('');
    $('#' + hyperlinkControlId).html('');
    $('#' + replacementControlId).html('');
    $('#' + replacementControlId)[0].title = '';
    deleteIconControl.css('display', 'none');
}

// torchGeneratedAttachmentList.ascx functions
function toggleTorchAttachmentPublished(attachmentId, tableId, gridDomId) {
    Spend_Management.svcAttachments.toggleTorchAttachmentPublished(tableId, attachmentId, function () {
        var grid = document.getElementById('tbl_' + gridDomId);
        if (grid != null) {
            SEL.Grid.refreshGrid(gridDomId, SEL.Grid.getCurrentPageNum(gridDomId));
        }
    }, function () {
        SEL.MasterPopup.ShowMasterPopup('An error occurred updating the attachment', 'Save Attachment');
    });

}
function ShowAttachmentUploadModal() {
   
    $("#uploadModal").dialog({
        modal: true,
        autoOpen: true,
        resizable: false,
        title: "Upload Attachment",
        width: 920,
        buttons: [
                    {
                         text: 'upload',
                         id: 'btnmodalUpload',
                         click: function () {
                             var iFrameEmailtemplateIdField = $('#iFrEmailAttach').contents().find('#GenIDVal');
                             if (typeof iFrameEmailtemplateIdField.val() === 'undefined' || !iFrameEmailtemplateIdField || iFrameEmailtemplateIdField.val() === '' || iFrameEmailtemplateIdField.val() === 0) {
                                 iFrameEmailtemplateIdField.val($('#hdnEmailtemplateId').val());
                             }
                             $('#iFrEmailAttach').contents().find('#btnUpload').click();
                         }
                          
                    },
                    {
                        text: 'cancel',
                        id: 'btnModalCancel',
                        click: function ()
                        {
                            $(this).dialog('close');
                        }
                    }
        ]
    });
   
  
}
function CloseAttachmentUploadModalModal() {
    $("#uploadModal").dialog("close");
}