namespace EsrFramework.Models
{
    using System;

    public class EsrTrust
    {
        #region Public Properties

        public byte EsrVersionNumber { get; set; }
        public bool Archived { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CurrentOutboundSequence { get; set; }
        public string DelimiterCharacter { get; set; }
        public string FtpAddress { get; set; }
        public string FtpPassword { get; set; }
        public string FtpUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string PeriodRun { get; set; }
        public string PeriodType { get; set; }
        public int RunSequenceNumber { get; set; }
        public int TrustId { get; set; }
        public string TrustName { get; set; }
        public string TrustVpd { get; set; }

        #endregion
    }
}