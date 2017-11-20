/// <summary>
/// Attachment type Methods
/// </summary>    
(function () {
    var scriptName = "AttachmentTypes";
    function execute() {
        SEL.registerNamespace("SEL.AttachmentTypes");
        SEL.AttachmentTypes =
        {

            /// <summary>
            /// Attachment Type Object
            /// </summary>
            AttachmentType: function (nMimeID, sFileExtension, sMimeHeader, sDescription) {
                this.MimeID = nMimeID;
                this.FileExtension = sFileExtension;
                this.MimeHeader = sMimeHeader;
                this.Description = sDescription;
            },

            /// <summary>
            /// Get the Attachment Type dropdown details
            /// </summary>
            GetAttachmentTypeData: function () {
                Spend_Management.svcAttachmentTypes.GetAttachmentData(this.GetAttachmentTypeDataComplete, this.OnError);
            },

            /// <summary>
            /// Get the Attachment Type dropdown details complete so that the dropdown can be populated
            /// </summary>
            GetAttachmentTypeDataComplete: function (lstItems) {
                var newOpt1;
                var cmbAttachmentType = document.getElementById(cmbAttachmentTypeID);

                cmbAttachmentType.options.length = 0;

                for (var i = 0; i < lstItems.length; i++) {
                    newOpt1 = document.createElement('option');
                    newOpt1.value = lstItems[i].Value;
                    newOpt1.text = lstItems[i].Text;

                    cmbAttachmentType.options.add(newOpt1);
                }
            },

            /// <summary>
            /// Catch any error and show an error message to users
            /// </summary>
            OnError: function (error) {
                if (error["_message"] != null) {
                    SEL.MasterPopup.ShowMasterPopup(error["_message"], "Attachment Type Error");
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup(error, "Attachment Type Error");
                }
                return;
            },

            /// <summary>
            /// Save the attachment type to the database
            /// </summary>
            SaveAttachmentType: function () {
                var cmbAttachmentType = document.getElementById(cmbAttachmentTypeID);

                Spend_Management.svcAttachmentTypes.SaveAttachmentType(cmbAttachmentType[cmbAttachmentType.selectedIndex].value, this.SaveAttachmentTypeComplete, this.OnError);
            },

            /// <summary>
            /// Save attachment type complete to refresh the grid with the newly added attachment type
            /// </summary>
            SaveAttachmentTypeComplete: function (data) {
                SEL.Common.HideModal(modAttachmentTypesID);
                Spend_Management.svcAttachmentTypes.CreateAttachmentTypeGrid(SEL.AttachmentTypes.RefreshAttachmentTypeGrid, this.OnError);
            },

            /// <summary>
            /// Refresh the attachment type grid
            /// </summary>
            RefreshAttachmentTypeGrid: function (data) {
                document.getElementById('divGrid').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);
            },

            /// <summary>
            /// Delete the attachment type
            /// </summary>
            DeleteAttachmentType: function (mimeID) {
                if (confirm('Are you sure you want to delete the Attachment Type?')) {
                    Spend_Management.svcAttachmentTypes.DeleteAttachmentType(mimeID, this.DeleteAttachmentTypeComplete, this.OnError);
                }
            },

            /// <summary>
            /// Display the return error if one has occurred
            /// </summary>
            DeleteAttachmentTypeComplete: function (retVal) {
                switch (retVal) {
                    case 0:
                        Spend_Management.svcAttachmentTypes.CreateAttachmentTypeGrid(SEL.AttachmentTypes.RefreshAttachmentTypeGrid, this.OnError);
                        break;
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is currently in use on a car document attachment, it must be removed from all car records before it can be deleted.');
                        break;
                    case -2:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is currently in use on an employee licence attachment, it must be removed from all employee records before it can be deleted.');
                        break;
                    case -3:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is currently in use on an Email Template attachment, it must be removed from all email template records before it can be deleted.');
                        break;
                    case -4:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is currently in use on a GreenLight attachment, it must be removed from all GreenLight records before it can be deleted.', 'Delete Attachment Type');
                        break;
                    case -5:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is currently a mandatory requirement for use with mobile devices. It cannot be deleted while mobile device receipt attachments are in use.');
                    break;
                    default:
                        break;
                }
            },

            /// <summary>
            /// Archive or unarchive the attachment type
            /// </summary>
            ChangeArchiveStatus: function (mimeID) {
                if (confirm('Click OK to confirm attachment type archive/unarchive')) {
                    Spend_Management.svcAttachmentTypes.ArchiveAttachmentType(mimeID, this.ChangeArchiveStatusComplete, this.OnError);
                }
            },

            /// <summary>
            /// Refresh the attachment type grid when an archive or unarchive has completed
            /// </summary>
            ChangeArchiveStatusComplete: function (data) {
                if (data == -5)
                    SEL.MasterPopup.ShowMasterPopup('This attachment type is currently a mandatory requirement for use with mobile devices. It cannot be archived while mobile device receipt attachments are in use.');
                else
                    Spend_Management.svcAttachmentTypes.CreateAttachmentTypeGrid(SEL.AttachmentTypes.RefreshAttachmentTypeGrid, this.OnError);
            },

            /// <summary>
            /// Save the custom attachment type to the database
            /// </summary>
            SaveCustomAttachmentType: function () {
                if (validateform('vgCustomAttach')) {
                    var txtExtension = document.getElementById(txtExtensionID);
                    var txtMimeHeader = document.getElementById(txtMimeHeaderID);
                    var txtDescription = document.getElementById(txtDescriptionID);

                    var attachObject = new this.AttachmentType(GlobalMimeID, txtExtension.value, txtMimeHeader.value, txtDescription.value);

                    Spend_Management.svcAttachmentTypes.SaveCustomAttachmentType(attachObject, GlobalMimeID, this.SaveCustomAttachmentTypeComplete, this.OnError);
                }
                return;
            },

            /// <summary>
            /// Save custom attachment type complete to refresh the grid with the newly added custom attachment type
            /// </summary>
            SaveCustomAttachmentTypeComplete: function (data) {
                switch (data) {
                    case 0:
                        SEL.Common.HideModal(modCustAttachmentTypesID);
                        Spend_Management.svcAttachmentTypes.CreateCustomAttachmentTypeGrid(SEL.AttachmentTypes.RefreshCustomAttachmentTypeGrid, this.OnError);
                        break;
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type already exists as a global attachment type and cannot be saved.', 'Save Custom Attachment Type');
                        break;
                    case -2:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type already exists as a custom attachment type and cannot be saved.', 'Save Custom Attachment Type');
                        break;
                    case -3:
                        SEL.MasterPopup.ShowMasterPopup('This attachment type is disallowed as it is potentially harmful and cannot be saved.', 'Save Custom Attachment Type');
                        break;
                    default:
                        break;
                }
            },

            /// <summary>
            /// Refresh the custom attachment type grid
            /// </summary>
            RefreshCustomAttachmentTypeGrid: function (data) {
                var gridDiv = $g('divCustGrid');
                if (gridDiv !== null) {
                    gridDiv.innerHTML = data[2];
                    SEL.Grid.updateGrid(data[1]);
                }
            },

            /// <summary>
            /// Get the custom Attachment Type dropdown details
            /// </summary>
            GetCustomAttachmentTypeData: function (mimeID) {
                GlobalMimeID = mimeID;
                Spend_Management.svcAttachmentTypes.GetCustomAttachmentData(mimeID, this.PopulateCustomAttachmentTypeData, this.OnError);
            },

            /// <summary>
            /// Get the custom Attachment Type object so that the form can be populated
            /// </summary>
            PopulateCustomAttachmentTypeData: function (attObject) {
                if (attObject !== null) {
                    document.getElementById(txtExtensionID).value = attObject.FileExtension;
                    document.getElementById(txtMimeHeaderID).value = attObject.MimeHeader;
                    document.getElementById(txtDescriptionID).value = attObject.Description;
                }
                else {
                    document.getElementById(txtExtensionID).value = '';
                    document.getElementById(txtMimeHeaderID).value = '';
                    document.getElementById(txtDescriptionID).value = '';
                }

                SEL.Common.ShowModal(modCustAttachmentTypesID);
            },

            /// <summary>
            /// Delete the custom attachment type
            /// </summary>
            DeleteCustomAttachmentType: function (mimeID) {
                if (confirm('Are you sure you want to delete the Custom Attachment Type?')) {
                    Spend_Management.svcAttachmentTypes.DeleteCustomAttachmentType(mimeID, this.DeleteCustomAttachmentTypeComplete, this.OnError);
                }
            },

            /// <summary>
            /// Display the return error if one has occurred
            /// </summary>
            DeleteCustomAttachmentTypeComplete: function (retVal) {
                if (retVal === -1) {
                    SEL.MasterPopup.ShowMasterPopup('This custom attachment type is currently in use for the assigned attachment types, it must be removed before it can be deleted.', 'Delete Custom Attachment Type');
                    return;
                }

                Spend_Management.svcAttachmentTypes.CreateCustomAttachmentTypeGrid(SEL.AttachmentTypes.RefreshCustomAttachmentTypeGrid, this.OnError);
                return;
            }

        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
})();
