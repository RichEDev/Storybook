namespace UnitTest2012Ultimate.expenses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SpendManagementLibrary;

    /// <summary>
    /// Mileage Category Object for  testing.
    /// </summary>
    class cMileageCatObject
    {
        /// <summary>
        /// The template.
        /// </summary>
        /// <param name="mileageid">
        /// The mileage id.
        /// </param>
        /// <param name="carsize">
        /// The car size.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="thresholdType">
        /// The threshold type.
        /// </param>
        /// <param name="calcmilestotal">
        /// The calculate miles total.
        /// </param>
        /// <param name="dateRanges">
        /// The date ranges.
        /// </param>
        /// <param name="mileUom">
        /// The mile Unit of Measure.
        /// </param>
        /// <param name="catvalid">
        /// The cat valid.
        /// </param>
        /// <param name="catvalidcomment">
        /// The cat valid comment.
        /// </param>
        /// <param name="currencyid">
        /// The currency id.
        /// </param>
        /// <param name="createdon">
        /// The created on.
        /// </param>
        /// <param name="createdby">
        /// The created by.
        /// </param>
        /// <returns>
        /// The <see cref="cMileageCat"/>.
        /// </returns>
        public static cMileageCat Template(int mileageid, string carsize, string comment, ThresholdType thresholdType, bool calcmilestotal, List<cMileageDaterange> dateRanges, MileageUOM mileUom, bool catvalid, string catvalidcomment, int currencyid, DateTime createdon, int createdby)
        {
            return new cMileageCat(mileageid, carsize, comment, thresholdType, calcmilestotal, dateRanges, mileUom, catvalid, catvalidcomment, currencyid, createdon, createdby, null, null, string.Empty, 0, 0, null);
        }
    }
}
