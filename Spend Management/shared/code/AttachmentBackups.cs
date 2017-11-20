using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using Syncfusion.XlsIO;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Spend_Management.AttachmentBackups
{
    /// <summary>
    /// A class to pass through to the CompressFiles method to help generate the information spreadsheet
    /// </summary>
    public class AttachmentBackupDetails
    {
        public string Supplier { get; set; }
        public string ContractNumber { get; set; }
        public string Contract { get; set; }
        public string Product { get; set; }
        public string Contact { get; set; }
        public string InvoiceNumber { get; set; }
        public string Invoice { get; set; }
        public string TaskType { get; set; }
        public string TaskNote { get; set; }
        public string Note { get; set; }
        public string AttachmentType { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }

        /// <summary>
        /// Constructor for the class
        /// </summary>
        public AttachmentBackupDetails()
        {
            Supplier = string.Empty;
            ContractNumber = string.Empty;
            Contract = string.Empty;
            Product = string.Empty;
            Contact = string.Empty;
            InvoiceNumber = string.Empty;
            Invoice = string.Empty;
            TaskType = string.Empty;
            TaskNote = string.Empty;
            Note = string.Empty; ;
            AttachmentType = String.Empty;
            Description = String.Empty;
            FileName = string.Empty;
        }
    }

    /// <summary>
    /// A class passed back from the CompressFiles method for each zip volume created.
    /// </summary>
    public class AttachmentBackupZipDetails
    {
        public string ZipFilePath { get; set; }
        public int ZipFileSize { get; set; }

        public AttachmentBackupZipDetails()
        {
            ZipFilePath = string.Empty;
            ZipFileSize = 0;
        }
    }

    /// <summary>
    /// Static class to generate zip volumes from attachments
    /// </summary>
    public static class AttachmentBackupManager
    {
        /// <summary>
        /// Compress a set of files given as one of the parameters into one or more zip volumes. Designed to work with Framework at present.
        /// </summary>
        /// <param name="zip_label">A label used to identify the package</param>
        /// <param name="destination_path">Destibation folder to store the created zip volume(s)</param>
        /// <param name="file_info">Full file path information for all the files needing to be compressed</param>
        /// <param name="split_volume_size">The size in Mb to use as the split size for each created volume</param>
        /// <param name="CreateInfoSpreadsheet">Specifies whether to create a spreadsheet attached inside the zip file about the files compressed</param>
        /// <returns>string html representing the grid</returns>
        public static List<AttachmentBackupZipDetails> CompressFiles(string zip_label, string destination_path, List<AttachmentBackupDetails> attachment_info, int split_volume_size = 700, bool CreateInfoSpreadsheet = false)
        {

            destination_path += destination_path.EndsWith(@"\") ? "" : @"\";
            List<AttachmentBackupZipDetails> created_zips = new List<AttachmentBackupZipDetails>();
            string zipfile = destination_path + zip_label + ".zip";
            string sTempDir = destination_path + zip_label;
            if (Directory.Exists(sTempDir)) Directory.Delete(sTempDir, true);
            Directory.CreateDirectory(sTempDir);

            if (CreateInfoSpreadsheet)
            {
                ExcelEngine xl = new ExcelEngine();
                using (xl)
                {
                    IApplication xlApp = xl.Excel;
                    IWorkbook wkbk = xl.Excel.Workbooks.Create(1);
                    IWorksheet sht1 = wkbk.Worksheets[0];

                    sht1.Range["A1"].Text = "Supplier";
                    sht1.Range["B1"].Text = "Contract Number";
                    sht1.Range["C1"].Text = "Contract";
                    sht1.Range["D1"].Text = "Product";
                    sht1.Range["E1"].Text = "Contact";
                    sht1.Range["F1"].Text = "Invoice Number";
                    sht1.Range["G1"].Text = "Invoice";
                    sht1.Range["H1"].Text = "Task Type";
                    sht1.Range["I1"].Text = "Task Note";
                    sht1.Range["J1"].Text = "Note";
                    sht1.Range["K1"].Text = "Attachment Type";
                    sht1.Range["L1"].Text = "Description";

                    for (int i = 1; i <= attachment_info.Count; i++)
                    {
                        sht1.Range["A" + (i + 1).ToString()].Text = attachment_info[i - 1].Supplier;
                        sht1.Range["B" + (i + 1).ToString()].Text = attachment_info[i - 1].ContractNumber;
                        sht1.Range["C" + (i + 1).ToString()].Text = attachment_info[i - 1].Contract;
                        sht1.Range["D" + (i + 1).ToString()].Text = attachment_info[i - 1].Product;
                        sht1.Range["E" + (i + 1).ToString()].Text = attachment_info[i - 1].Contact;
                        sht1.Range["F" + (i + 1).ToString()].Text = attachment_info[i - 1].InvoiceNumber;
                        sht1.Range["G" + (i + 1).ToString()].Text = attachment_info[i - 1].Invoice;
                        sht1.Range["H" + (i + 1).ToString()].Text = attachment_info[i - 1].TaskType;
                        sht1.Range["I" + (i + 1).ToString()].Text = attachment_info[i - 1].TaskNote;
                        sht1.Range["J" + (i + 1).ToString()].Text = attachment_info[i - 1].Note;
                        sht1.Range["K" + (i + 1).ToString()].Text = attachment_info[i - 1].AttachmentType;
                        sht1.Range["L" + (i + 1).ToString()].Text = attachment_info[i - 1].Description;
                    }
                    wkbk.SaveAs(sTempDir + @"\info.xls", ExcelSaveType.SaveAsXLS);
                    wkbk.Close();
                }
            }
            using (ZipFile zip = new ZipFile(zipfile))
            {
                zip.ZipErrorAction = ZipErrorAction.Skip;
                zip.MaxOutputSegmentSize = split_volume_size * 1024 * 1024;
                zip.TempFileFolder = sTempDir;
                List<string> file_info = (from x in attachment_info
                                          select x.FileName).ToList();
                try
                {
                    if (CreateInfoSpreadsheet)
                    {
                        zip.AddFile(sTempDir + @"\info.xls", Path.GetDirectoryName(file_info.FirstOrDefault()));
                    }
                    zip.AddFiles(file_info, true, Path.GetDirectoryName(file_info.FirstOrDefault()));
                }
                catch (Exception e)
                {
                    if (e.Message == "An item with the same key has already been added.")
                    {
                        zip.Dispose();
                        Directory.Delete(sTempDir, true);
                        throw new InvalidOperationException("An item with the same key has already been added to this zip file.");
                    }
                    else
                    {
                        zip.Dispose();
                        Directory.Delete(sTempDir, true);
                        throw;
                    }
                }
                try
                {
                    zip.Save();
                    Thread.Sleep(3000);
                    FileInfo f = new FileInfo(zipfile);
                    AttachmentBackupZipDetails zip_detail = new AttachmentBackupZipDetails();
                    zip_detail.ZipFilePath = zipfile;
                    zip_detail.ZipFileSize = (int)f.Length / 1024;
                    created_zips.Add(zip_detail);
                    for (int i = 1; i < zip.NumberOfSegmentsForMostRecentSave; i++)
                    {
                        string sVol = "";
                        if (i < 10)
                            sVol = "0";
                        string zipVolumefile = destination_path + zip_label + ".z" + sVol + i;
                        f = new FileInfo(zipVolumefile);
                        zip_detail = new AttachmentBackupZipDetails();
                        zip_detail.ZipFilePath = zipVolumefile;
                        zip_detail.ZipFileSize = (int)f.Length / 1024;
                        created_zips.Add(zip_detail);
                    }
                }
                catch (Exception e)
                {
                    zip.Dispose();
                    Directory.Delete(sTempDir, true);
                    throw e;
                }
                zip.Dispose();
            }
            Directory.Delete(sTempDir, true);
            return created_zips;
        }

        /// <summary>
        /// Mark all attachments ready to process for backup.
        /// </summary>
        /// <param name="zip_label">Label string for the zip volumes</param>        
        /// <param name="account_id">Account id of the user</param>
        /// <param name="subaccount_id">The sub account used for the backup</param>
        /// <param name="employee_id">Employee id of the user</param>
        /// <param name="root_directory_path">The filepath base e.g. c:\</param>
        /// <param name="destination_path">Destination path for the created zip volumes</param>
        /// <param name="split_size">The size in Mb to split the zip volumes</param>
        /// <param name="create_info_spreadsheet">Boolean to specify whether to create an information spreadsheet inside the volumes with attachment information</param>
        /// <returns>True or False</returns>
        public static bool MarkAllAttachmentsForBackup(string zip_label, int account_id, int subaccount_id, int employee_id, string root_directory_path, string destination_path, int split_size = 700, bool create_info_spreadsheet = false)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(account_id));
            db.sqlexecute.Parameters.AddWithValue("@subaccountid", subaccount_id);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            cSuppliers suppliers = new cSuppliers(account_id, subaccount_id);
            List<int> lstsupplierids = suppliers.getSupplierIdsForSubAccount();

            using (SqlDataReader reader = db.GetStoredProcReader("attachmentBackupPackageGetAllAttachmentInfo"))
            {
                db.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    AttachmentBackupDetails abd = new AttachmentBackupDetails();
                    
                    abd.Supplier = reader.GetString(reader.GetOrdinal("supplier"));
				    abd.ContractNumber = reader.GetString(reader.GetOrdinal("contractnumber"));
				    abd.Contract = reader.GetString(reader.GetOrdinal("contractdescription"));
				    abd.Product = reader.GetString(reader.GetOrdinal("product"));
				    abd.Contact = reader.GetString(reader.GetOrdinal("contact"));
				    abd.InvoiceNumber = reader.GetString(reader.GetOrdinal("invoicenumber"));
				    abd.Invoice = reader.GetString(reader.GetOrdinal("invoicedescription"));
				    abd.TaskType = reader.GetString(reader.GetOrdinal("tasktype"));
				    abd.TaskNote = reader.GetString(reader.GetOrdinal("taskdescription"));
				    abd.Note = reader.GetString(reader.GetOrdinal("note"));
				    abd.AttachmentType = reader.GetString(reader.GetOrdinal("attachmenttype"));
				    abd.Description = reader.GetString(reader.GetOrdinal("attachmentdescription"));
				    abd.FileName = reader.GetString(reader.GetOrdinal("attachmentfilename"));
                    
                    attachments.Add(abd);
                }
                reader.Close();
            }
            AttachmentBackupManager.CompressFiles(zip_label, destination_path, attachments, split_size, create_info_spreadsheet);
            return true;
        }

        /// <summary>
        /// Returns the grid representing all backup packages that have been created and waiting for download
        /// </summary>
        /// <param name="account_id">Account id of the user</param>
        /// <param name="employee_id">Employee id of the user</param>
        /// <param name="packages_ready_for_download">Whether to retrieve packages processed and ready for download, or currently in the queue</param>
        /// <returns>string html representing the grid</returns>
        internal static string[] GetPackagesGrid(int account_id, int employee_id, bool packages_ready_for_download)
        {
            int processed = packages_ready_for_download ? 1 : 0;
            string sEmptyText = packages_ready_for_download ? "There are no backup packages ready for download." : "There are no backup packages queued for processing.";
            string sGridID = packages_ready_for_download ? "gAttachmentPackagesProcessed" : "gAttachmentPackagesUnprocessed";
            string sSql = "SELECT ID, PackageLabel, ExpiryDate FROM attachment_backup_package";

            cGridNew grid = new cGridNew(account_id, employee_id, sGridID, sSql);
            cFields fields = new cFields(account_id);
            //cFieldColumn column = new cFieldColumn(fields.GetFieldByTableAndFieldName("attachment_backup_package", "ExpiryDate"));
            var column = new cFieldColumn(new cField());

            grid.SortedColumn = column;
            grid.KeyField = "ID";
            grid.getColumnByName("ID").hidden = true;
            if (packages_ready_for_download)
            {
                grid.enabledeleting = true;
                grid.deletelink = "javascript:SEL.AttachmentBackup.DeleteAttachmentBackupPackage('{ID}');";
                grid.enableupdating = true;
                grid.editlink = "javascript:SEL.AttachmentBackup.LaunchPackageVolumes('{ID}');";
                grid.getColumnByName("ExpiryDate").hidden = true;
                //grid.addFilter(fields.GetFieldByTableAndFieldName("attachment_backup_package", "Processed"), ConditionType.Equals, new object[] { processed }, null, ConditionJoiner.None);                
                grid.addFilter(new cField(), ConditionType.Equals, new object[] { processed }, null, ConditionJoiner.None);
            }
            grid.CssClass = "datatbl";
            grid.EmptyText = sEmptyText;

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Returns a grid representing the volumes within an attachment backup package. Links for downloading the zip volumes are included.
        /// </summary>
        /// <param name="package_id">Guid id field of the attachment backup package</param>
        /// <param name="account_id">Account id of the user</param>
        /// <param name="employee_id">Employee id of the user</param>
        /// <returns>string html representing the grid</returns>
        internal static string[] GetVolumesForPackageGrid(Guid package_id, int account_id, int employee_id)
        {
            string sSql = "SELECT ID, AttachmentBackupPackageID, FilePath, Order, FileLength FROM attachment_backup_package_volume";

            cGridNew grid = new cGridNew(account_id, employee_id, "gAttachmentPackageVolumes", sSql);
            cFields fields = new cFields(account_id);
            //cFieldColumn column = new cFieldColumn(fields.GetFieldByTableAndFieldName("attachment_backup_package_volume", "Order"));
            cFieldColumn column = new cFieldColumn(new cField(), "Order");

            grid.SortedColumn = column;
            grid.KeyField = "ID";
            grid.getColumnByName("ID").hidden = true;
            grid.getColumnByName("AttachmentBackupPackageID").hidden = true;
            grid.getColumnByName("FilePath").hidden = true;
            grid.enableupdating = true;
            grid.editlink = "javascript:SEL.AttachmentBackup.DownloadPackageVolumes({ID});";
            //grid.addFilter(fields.GetFieldByTableAndFieldName("attachment_backup_package_volume", "AttachmentBackupPackageID"), ConditionType.Equals, new object[] { package_id }, null, ConditionJoiner.None);
            grid.addFilter(new cField(), ConditionType.Equals, new object[] { package_id }, null, ConditionJoiner.None);
            grid.CssClass = "datatbl";
            grid.EmptyText = "This package contains has no available volumes for download";

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

    }
}
