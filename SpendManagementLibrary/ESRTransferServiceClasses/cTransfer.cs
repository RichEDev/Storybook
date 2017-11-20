using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.ESRTransferServiceClasses
{
    /// <summary>
    /// Contains the data for the outbound file import
    /// </summary>
    [Serializable()]
    public class cESRFileInfo
    {
        /// <summary>
        /// 
        /// </summary>
        private int nDataID;
        /// <summary>
        /// 
        /// </summary>
        private int nTrustID;

        /// <summary>
        /// 
        /// </summary>
        private string sFileName;

        /// <summary>
        /// 
        /// </summary>
        private byte[] arrFileData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrustID"></param>
        /// <param name="FileData"></param>
        public cESRFileInfo(int DataID, int TrustID, string fileName, byte[] FileData)
        {
            nDataID = DataID;
            nTrustID = TrustID;
            sFileName = fileName;
            arrFileData = FileData;
        }

        public cESRFileInfo()
        {

        }

        /// <summary>
        /// ID of the Outbound File data
        /// </summary>
        public int DataID
        {
            get { return nDataID; }
        }

        /// <summary>
        /// ID of the trust
        /// </summary>
        public int TrustID
        {
            get { return nTrustID; }
        }

        public string fileName
        {
            get { return sFileName; }
        }

        /// <summary>
        /// byte[] of the File Data
        /// </summary>
        public byte[] FileData
        {
            get { return arrFileData; }
        }
    }

    /// <summary>
    /// Outbound File Status Type
    /// </summary>
    [Serializable()]
    public enum OutboundStatus
    {
        None = 0,
        AwaitingImport,
        Complete,
        ValidationFailed,
        Failed
    }


    /// <summary>
    /// Class to store Outbound File Properties.
    /// </summary>
    [Serializable()]
    public class cOutboundFileDetails
    {
        private string sFileName;
        private DateTime dtFileCreatedOn;
        private DateTime dtFielModifiedOn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FileCreatedOn"></param>
        /// <param name="FileModifiedOn"></param>
        public cOutboundFileDetails(string FileName, DateTime FileCreatedOn, DateTime FileModifiedOn)
        {
            sFileName = FileName;
            dtFileCreatedOn = FileCreatedOn;
            dtFielModifiedOn = FileModifiedOn;
        }

        #region Properties

        /// <summary>
        /// File name of the Outbound File
        /// </summary>
        public string FileName
        {
            get { return sFileName; }
        }

        /// <summary>
        /// Date the Outbound File was Created On
        /// </summary>
        public DateTime FileCreatedOn
        {
            get { return dtFileCreatedOn; }
        }

        // <summary>
        /// Date the Outbound File was Modified On
        /// </summary>
        public DateTime FileModifiedOn
        {
            get { return dtFielModifiedOn; }
        }
        #endregion

    }

    [Serializable()]
    public struct TrustUpdateInfo
    {
        public int AccountID;
        public List<cESRTrust> lstTrusts;
    }
}
