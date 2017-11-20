using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Holds information about an individual NHS Trust for use with ESR
    /// </summary>
    [Serializable()]
    public class cESRTrust
    {
        private int nTrustID;

        private int nExpTrustID;

        private int nAccountID;

        private string sTrustName;

        private string sTrustVPD;

        private string sPeriodType;

        private string sPeriodRun;

        private int nRunSequenceNumber;

        private string sFtpAddress;

        private string sFtpUsername;

        private string sFtpPassword;

        private bool bArchived;

        private DateTime? dtCreatedOn;

        private DateTime? dtModifiedOn;

        private string sDelimiterCharacter;

        private byte nEsrTrustVersionNumber;

        private int? nCurrentOutboundSequenceNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trustID"></param>
        /// <param name="trustName"></param>
        /// <param name="trustVPD"></param>
        /// <param name="periodType"></param>
        /// <param name="periodRun"></param>
        /// <param name="runSequenceNumber"></param>
        /// <param name="ftpAddress"></param>
        /// <param name="ftpUsername"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="archived"></param>
        /// <param name="createdOn"></param>
        /// <param name="modifiedOn"></param>
        /// <param name="delimiterCharacter"></param>
        /// <param name="esrVersionNumber"></param>
        /// <param name="currentOutboundSequence"></param>
        public cESRTrust(
            int trustID,
            string trustName,
            string trustVPD,
            string periodType,
            string periodRun,
            int runSequenceNumber,
            string ftpAddress,
            string ftpUsername,
            string ftpPassword,
            bool archived,
            DateTime? createdOn,
            DateTime? modifiedOn,
            string delimiterCharacter,
            byte esrVersionNumber,
            int? currentOutboundSequence)
        {
            nTrustID = trustID;
            nRunSequenceNumber = runSequenceNumber;
            sTrustName = trustName;
            sTrustVPD = trustVPD;
            sPeriodType = periodType;
            sPeriodRun = periodRun;
            sFtpAddress = ftpAddress;
            sFtpPassword = ftpPassword;
            sFtpUsername = ftpUsername;
            bArchived = archived;
            dtCreatedOn = createdOn;
            dtModifiedOn = modifiedOn;
            sDelimiterCharacter = delimiterCharacter;
            nEsrTrustVersionNumber = esrVersionNumber;
            nCurrentOutboundSequenceNumber = currentOutboundSequence;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trustID"></param>
        /// <param name="AccountID"></param>
        /// <param name="trustName"></param>
        /// <param name="trustVPD"></param>
        /// <param name="ftpAddress"></param>
        /// <param name="ftpUsername"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="archived"></param>
        /// <param name="createdOn"></param>
        /// <param name="modifiedOn"></param>
        public cESRTrust(
            int trustID, int expTrustID, int AccountID, string trustName, string trustVPD, string ftpAddress, string ftpUsername, string ftpPassword, bool archived, DateTime? createdOn, DateTime? modifiedOn)
        {
            nTrustID = trustID;
            nExpTrustID = expTrustID;
            nAccountID = AccountID;
            sTrustName = trustName;
            sTrustVPD = trustVPD;
            sFtpAddress = ftpAddress;
            sFtpPassword = ftpPassword;
            sFtpUsername = ftpUsername;
            bArchived = archived;
            dtCreatedOn = createdOn;
            dtModifiedOn = modifiedOn;
            nEsrTrustVersionNumber = 1;
            sDelimiterCharacter = ",";
            nCurrentOutboundSequenceNumber = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public cESRTrust()
        {

        }

        /// <summary>
        /// Gets or sets the TrustID for this trust in the ESR Transfer Service
        /// </summary>
        public int TrustID
        {
            get
            {
                return nTrustID;
            }
            set
            {
                nTrustID = value;
            }
        }

        /// <summary>
        /// Trust ID stored in expenses
        /// </summary>
        public int expTrustID
        {
            get
            {
                return nExpTrustID;
            }
            set
            {
                nExpTrustID = value;
            }
        }

        /// <summary>
        /// Gets or sets the AccountID for this trust
        /// </summary>
        public int AccountID
        {
            get
            {
                return nAccountID;
            }
            set
            {
                nAccountID = value;
            }
        }

        /// <summary>
        /// Gets or sets the used with this trust
        /// </summary>
        public string TrustName
        {
            get
            {
                return sTrustName;
            }
            set
            {
                sTrustName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Trust VPD used with this trust
        /// </summary>
        public string TrustVPD
        {
            get
            {
                return sTrustVPD;
            }
            set
            {
                sTrustVPD = value;
            }
        }

        /// <summary>
        /// Gets or sets the Period Type used with this trust
        /// </summary>
        public string PeriodType
        {
            get
            {
                return sPeriodType;
            }
            set
            {
                sPeriodType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Period Run used with this trust
        /// </summary>
        public string PeriodRun
        {
            get
            {
                return sPeriodRun;
            }
            set
            {
                sPeriodRun = value;
            }
        }

        /// <summary>
        /// Gets or sets the RunSequenceNumber
        /// </summary>
        public int RunSequenceNumber
        {
            get
            {
                return nRunSequenceNumber;
            }
            set
            {
                nRunSequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the FTP Address used for this trust
        /// </summary>
        public string FTPAddress
        {
            get
            {
                return sFtpAddress;
            }
            set
            {
                sFtpAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the FTP Username used for this trust
        /// </summary>
        public string FTPUsername
        {
            get
            {
                return sFtpUsername;
            }
            set
            {
                sFtpUsername = value;
            }
        }

        /// <summary>
        /// Gets or sets the FTP password used for this trust, this must be encrypted.
        /// </summary>
        public string FTPPassword
        {
            get
            {
                return sFtpPassword;
            }
            set
            {
                sFtpPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets if this trust is archived
        /// </summary>
        public bool Archived
        {
            get
            {
                return bArchived;
            }
            set
            {
                bArchived = value;
            }
        }

        /// <summary>
        /// Date trust record created
        /// </summary>
        public DateTime? CreatedOn
        {
            get
            {
                return dtCreatedOn;
            }
        }

        /// <summary>
        /// Gets the date the trust was last modified
        /// </summary>
        public DateTime? ModifiedOn
        {
            get
            {
                return dtModifiedOn;
            }
        }

        /// <summary>
        /// The character used to delimit the values in ESR Outbound file
        /// </summary>
        public string DelimiterCharacter
        {
            get
            {
                return sDelimiterCharacter;
            }
            set
            {
                sDelimiterCharacter = value;
            }
        }

        /// <summary>
        /// The version of the ESR Outbound interface used by the trust
        /// </summary>
        public byte EsrInterfaceVersionNumber
        {
            get
            {
                return nEsrTrustVersionNumber;
            }
            set
            {
                nEsrTrustVersionNumber = value;
            }
        }

        /// <summary>
        /// Current outbound file sequence number processed
        /// </summary>
        public int? CurrentOutboundSequenceNumber
        {
            get
            {
                return nCurrentOutboundSequenceNumber;
            }
            set
            {
                nCurrentOutboundSequenceNumber = value;
            }
        }
    }

    /// <summary>
    /// Used for the deletion of an ESR trust
    /// </summary>
    public enum ESRTrustReturnVal
    {
        /// <summary>
        /// Successfully deleted
        /// </summary>
        Success = 0,

        /// <summary>
        /// Trust associated to a financial export
        /// </summary>
        FinancialExportAssociated
    }
}
