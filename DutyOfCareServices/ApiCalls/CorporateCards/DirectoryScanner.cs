namespace DutyOfCareServices.ApiCalls.CorporateCards
{
    using System;
    using System.Collections.Generic;
    using ApiClientHelper.Models;
    using Common.Logging;

    /// <summary>
    /// A class to handle watching the files and sizes in a specified directory
    /// </summary>
    public class DirectoryScanner
    {
        /// <summary>
        /// A private instance of <see cref="DirectoryDetails"/>
        /// </summary>
        private DirectoryDetails _directoryDetails;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<DirectoryScanner>().GetLogger();

        /// <summary>
        /// Create a new instance of <see cref="DirectoryScanner"/>
        /// </summary>
        /// <param name="directory">The directory to scan</param>
        /// <param name="directoryDetails">an instance of <see cref="DirectoryDetails"/></param>
        public DirectoryScanner(string directory, DirectoryDetails directoryDetails)
        {
            this.Directory = directory;
            this._directoryDetails = directoryDetails;
        }

        /// <summary>
        /// Gets the directory that is being monitored.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Get a <see cref="List{T}"/> of <seealso cref="FileInformation"/> for the current <seealso cref="Directory"/>
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <seealso cref="FileInformation"/> for the current <seealso cref="Directory"/></returns>
        public List<FileInformation> GetInformation()
        {
            Log.Debug("GetInformation");
            return this._directoryDetails.FileInformation;
        }

        /// <summary>
        /// Move a file to a directory based on <seealso cref="ImportStatus"/>
        /// </summary>
        /// <param name="fileInformation">An instance of <see cref="FileInformation"/> pointing to a file to move</param>
        /// <param name="sucess">An instance of <see cref="ImportStatus"/>indicating the location of the file.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given <see cref="ImportStatus"/>was out of range.</exception>
        public void MoveFile(FileInformation fileInformation, ImportStatus sucess)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Move file {fileInformation.FileName} to {sucess} folder");
            }

            switch (sucess)
            {
                case ImportStatus.Sucess:
                    this._directoryDetails.MoveToSucess(fileInformation);
                    break;
                case ImportStatus.IdNotFound:
                    this._directoryDetails.MoveToNotFound(fileInformation);
                    break;
                case ImportStatus.InvalidFile:
                    this._directoryDetails.MoveToInvalid(fileInformation);
                    break;
                case ImportStatus.ApiFail:
                    this._directoryDetails.MoveToApiFail(fileInformation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sucess), sucess, null);
            }
        }
    }
}
