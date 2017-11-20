namespace EsrFramework.Models
{
    using System;

    public class EsrElementField
    {
        #region Public Properties

        public byte? Aggregate { get; set; }
        public int ElementFieldId { get; set; }
        public int ElementId { get; set; }
        public int GlobalElementFieldId { get; set; }
        public byte Order { get; set; }
        public Guid ReportColumnId { get; set; }

        #endregion
    }
}