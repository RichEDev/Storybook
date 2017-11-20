/// <summary>
/// Document Merge Methods
/// </summary>    
(function () {
    var scriptName = "AttachmentBackup";
    function execute() {
        SEL.registerNamespace("SEL.AttachmentBackup");
        SEL.AttachmentBackup =
        {
            DeleteAttachmentBackupPackage: function (id) {
                alert('in progress - package: ' + id);
            },
            CreateFullBackup: function () {
                alert('in progress');
            },
            LaunchFilteredBackup: function () {
                alert('in progress');
            },
            LaunchPackageVolumes: function (id) {
                Spend_Management.svcAttachmentBackupManager.GetVolumesForPackageGrid(id, SEL.AttachmentBackup.OnLaunchPackageVolumesComplete, SEL.AttachmentBackup.OnLaunchPackageVolumesError);
            },
            OnLaunchPackageVolumesComplete: function (data) {
                $g('divGridAttachmentVolumes').innerHTML = data[2];
                SEL.Grid.updateGrid(data[1]);
                $f('mdlAttachmentVolumes').show();
            },
            OnLaunchPackageVolumesError: function (data) {
                SEL.MasterPopup.ShowMasterPopup('Error encountered retrieving download volumes for this package', 'Attachment Backup Manager - Package Volumes');
            }
        }
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
})();