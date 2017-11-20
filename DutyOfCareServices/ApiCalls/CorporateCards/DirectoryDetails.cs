using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using ApiClientHelper.Models;
using DataColumn = System.Data.DataColumn;

namespace DutyOfCareServices.ApiCalls.CorporateCards
{
    /// <summary>
    /// A class to handle the data for a specified directory (previous and current files and sizes).
    /// </summary>
    public class DirectoryDetails
    {
        /// <summary>
        /// A private <see cref="List"/> of <seealso cref="FileInformation"/>
        /// </summary>
        private List<FileInformation> _fileInformation;

        /// <summary>
        /// Gets the directory being processed.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// A <see cref="List"/> of <seealso cref="FileInformation"/> giving the current versus previous file sizes.
        /// </summary>
        public List<FileInformation> FileInformation => this._fileInformation;

        /// <summary>
        /// Create a new instance of <see cref="DirectoryDetails"/>
        /// </summary>
        /// <param name="directory">The directory to scan</param>
        public DirectoryDetails(string directory)
        {
            this.Directory = directory;
            if (!this.Directory.EndsWith("/"))
            {
                this.Directory += "/";
            }
            this._fileInformation = this.CreateFileInfo();
        }

        /// <summary>
        /// Move the given file to a sub folder named Complete
        /// </summary>
        /// <param name="fileInformation"></param>
        public void MoveToSucess(FileInformation fileInformation)
        {
            this.MoveFile(fileInformation.FileName, @"Complete");
            
        }

        /// <summary>
        /// Move the given file to a sub folder named IdNotFound
        /// </summary>
        /// <param name="fileInformation"></param>
        public void MoveToNotFound(FileInformation fileInformation)
        {
            this.MoveFile(fileInformation.FileName, @"IdNotFound");
        }

        /// <summary>
        /// Move the given file to a sub folder named Invalid
        /// </summary>
        /// <param name="fileInformation"></param>
        public void MoveToInvalid(FileInformation fileInformation)
        {
            this.MoveFile(fileInformation.FileName, @"Invalid");
        }

        /// <summary>
        /// Move the given file to a sub folder named ApiFail
        /// </summary>
        /// <param name="fileInformation"></param>
        public void MoveToApiFail(FileInformation fileInformation)
        {
            this.MoveFile(fileInformation.FileName, @"ApiFail");
        }

        /// <summary>
        /// Create an instance of <see cref="list"/> <seealso cref="FileInformation"/>
        /// </summary>
        /// <returns>an instance of <see cref="list"/> <seealso cref="FileInformation"/></returns>
        private List<FileInformation> CreateFileInfo()
        {
            var previousFileInfo = this.GetPreviousDetail();
            return this.GetCurrentFileInfo(previousFileInfo);
        }

        /// <summary>
        /// Get an instance of <see cref="list"/> <seealso cref="FileInformation"/> containing current and previoud file information
        /// </summary>
        /// <param name="previousFileInfo">A <see cref="DataSet"/>Containing the previous FileInformation </param>
        /// <returns>an instance of <see cref="list"/> <seealso cref="FileInformation"/></returns>
        private List<FileInformation> GetCurrentFileInfo(DataSet previousFileInfo)
        {
            var result = new List<FileInformation>();
            var files = System.IO.Directory.GetFiles(this.Directory);
            this.StoreCurrentDetail(files);
            if (previousFileInfo.Tables.Count > 0)
            {
                foreach (var file in files.Where(f => !f.Contains("FileInfo.xml")))
                {
                
                    var fileContent = File.ReadAllBytes(file);
                    var view = new DataView(previousFileInfo.Tables[0]) {RowFilter = $"FileName = '{file}'"};
                    var filteredTable = view.ToTable();
                    var previousSize = 0;
                    if (filteredTable.Rows.Count > 0)
                    {
                        previousSize = int.Parse(filteredTable.Rows[0]["Size"].ToString());
                    }
                
                    var fileInfo = new FileInformation(file, previousSize, fileContent.Length);
                    result.Add(fileInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a <see cref="DataSet"/> Containing the file information from the last run.
        /// </summary>
        /// <returns>A <see cref="DataSet"/>Containg the file information from the last run of this function. </returns>
        private DataSet GetPreviousDetail()
        {
            var filename = this.Directory + "FileInfo.xml";
            if (!File.Exists(filename))
            {
                return new DataSet("Empty");
            }

            var data = new DataSet("Populated");
            data.ReadXml(filename);
            return data;
        }

        /// <summary>
        /// Move a file from the current <see cref="Directory"/> to a specified sub directory
        /// </summary>
        /// <param name="fileName">The name of the file to move.</param>
        /// <param name="folder">The folder to move the file too.</param>
        private void MoveFile(string fileName, string folder)
        {
            if (!System.IO.Directory.Exists(this.Directory + folder))
            {
                System.IO.Directory.CreateDirectory(this.Directory + folder);
            }

            var file = fileName.Replace(this.Directory, string.Empty);

            if (File.Exists(this.Directory+folder + @"\" + file))
            {
                file = this.MakeFilenameUnique(file, folder);
            }

            File.Move(fileName, this.Directory+ folder + @"\" + file);
        }

        /// <summary>
        /// Make a given file name unique in it's target directory.
        /// </summary>
        /// <param name="file">The file to check</param>
        /// <param name="folder">The folder that the file will be unique in.</param>
        /// <returns>A filename made unique for the target folder.</returns>
        private string MakeFilenameUnique(string file, string folder)
        {
            var fileNumber = 0;
            var exists = true;
            var newFile = file;
            var extension = Path.GetExtension(file);
            var baseFile = Path.GetFileNameWithoutExtension(file);


            while (exists)
            {
                fileNumber++;
                newFile = $"{baseFile}({fileNumber}){extension}";
                exists = File.Exists(this.Directory + folder+ @"\" + newFile);
            }

            return newFile;
        }

        /// <summary>
        /// Given an array of file names, store the names and sizes in a file stored in the current <see cref="Directory"/>
        /// </summary>
        /// <param name="files">An <see>
        ///         <cref>string[]</cref>
        ///     </see>
        ///     of file names</param>
        private void StoreCurrentDetail(string[] files)
        {
            var data = new DataSet();
            var table = new DataTable("files");
            var column = new DataColumn("FileName");
            table.Columns.Add(column);

            column = new DataColumn("Size", typeof(int));
            table.Columns.Add(column);

            data.Tables.Add(table);
            foreach (string file in files)
            {
                var row = table.NewRow();
                row["FileName"] = file;
                var fileContent = File.ReadAllBytes(file);
                row["Size"] = fileContent.Length;
                table.Rows.Add(row);
            }

            data.WriteXml(this.Directory + "FileInfo.xml");
        }
    }
}
