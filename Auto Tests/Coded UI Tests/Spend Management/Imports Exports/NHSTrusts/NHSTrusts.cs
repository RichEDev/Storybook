namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Imports_Exports.NHSTrusts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The nhs trusts.
    /// </summary>
    public class NHSTrusts
    {
        public int TrustID { get; set; }
        public string TrustVPD { get; set; }
        public string PeriodType { get; set; }
        public string PeriodRun { get; set; }
        public int RunSequenceNumber { get; set; }
        public string FtpAddress { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public bool Archived { get; set; }
        public string TrustName { get; set; }
        public string DelimiterCharacter { get; set; }
        public byte ESRInterfaceVersion { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }

            NHSTrusts dto = obj as NHSTrusts;
            if (dto == null) { return false; }

            if (this.TrustVPD != dto.TrustVPD)
            {
                return false;
            }
            if (this.TrustName != dto.TrustName)
            {
                return false;
            }
            if (this.RunSequenceNumber != dto.RunSequenceNumber)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NHSTrusts"/> class.
        /// </summary>
        public NHSTrusts()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NHSTrusts"/> class.
        /// </summary>
        /// <param name="trustID">
        /// The trust id.
        /// </param>
        /// <param name="trustVPD">
        /// The trust vpd.
        /// </param>
        /// <param name="periodType">
        /// The period type.
        /// </param>
        /// <param name="periodRun">
        /// The period run.
        /// </param>
        /// <param name="runSequenceNumber">
        /// The run sequence number.
        /// </param>
        /// <param name="ftpAddress">
        /// The ftp address.
        /// </param>
        /// <param name="ftpUsername">
        /// The ftp username.
        /// </param>
        /// <param name="ftpPassword">
        /// The ftp password.
        /// </param>
        /// <param name="archived">
        /// The archived.
        /// </param>
        /// <param name="trustName">
        /// The trust name.
        /// </param>
        /// <param name="delimiterCharacter">
        /// The delimiter character.
        /// </param>
        /// <param name="esrInterfaceVersion">
        /// The esr interface version.
        /// </param>
        public NHSTrusts(int trustID, string trustVPD, string periodType, string periodRun, int runSequenceNumber, string ftpAddress, string ftpUsername, string ftpPassword, bool archived, string trustName, string delimiterCharacter, byte esrInterfaceVersion)
        {
            this.TrustID = trustID;
            this.TrustVPD = trustVPD;
            this.PeriodType = periodType;
            this.PeriodRun = periodRun;
            this.RunSequenceNumber = runSequenceNumber;
            this.FtpAddress = ftpAddress;
            this.FtpUsername = ftpUsername;
            this.FtpPassword = ftpPassword;
            this.Archived = archived;
            this.TrustName = trustName;
            this.DelimiterCharacter = delimiterCharacter;
            this.ESRInterfaceVersion = esrInterfaceVersion;
        }

    }

    /// <summary>
    /// The friendly name.
    /// </summary>
    public static class FriendlyName
    {
        /// <summary>
        /// The get period run friendly name.
        /// </summary>
        /// <param name="periodRunDbValue">
        /// The period run db value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPeriodRunFriendlyName(string periodRunDbValue)
        {
            return periodRunDbValue == "N" ? "Normal" : "Supplementary";
        }

        /// <summary>
        /// The get period type friendly name.
        /// </summary>
        /// <param name="periodTypeDbValue">
        /// The period type db value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPeriodTypeFriendlyName(string periodTypeDbValue)
        {
            string periodTypeFriendlyName = string.Empty;

            switch (periodTypeDbValue)
            {
                case "W":
                    periodTypeFriendlyName = "Weekly";
                    break;
                case "M":
                    periodTypeFriendlyName = "Monthly";
                    break;
                case "L":
                    periodTypeFriendlyName = "Lunar Month";
                    break;
                case "F":
                    periodTypeFriendlyName = "Bi-Week";
                    break;
            }
            return periodTypeFriendlyName;
        }
    }

    /// <summary>
    /// The esr interface version.
    /// </summary>
    public enum ESRInterfaceVersion
    {
        [Description("v1.0")]
        Version1 = 1,
        [Description("v2.0")]
        Version2 = 2
    }
}
