namespace Spend_Management.shared.webServices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Exceptions;

    /// <summary>
    ///     Vehicle Journey Rate web service.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcVehicleJourneyRate : WebService
    {
        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string[] Controls { get; set; }
        }

        /// <summary>
        ///     Creates the date ranges grid.
        /// </summary>
        /// <param name="contextKey">The mileageid.</param>
        /// <returns>An array containing the grid control ID, javascript, and html.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] DateRangeGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            int mileageId = 0;
            if (!String.IsNullOrEmpty(contextKey))
            {
                int.TryParse(contextKey, out mileageId);
            }
            
            var fields = new cFields(user.AccountID);
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridDateRanges", "select mileagedateid, daterangetype, datevalue1, datevalue2 from mileage_dateranges")
            {
                EmptyText = "There are no date ranges defined for this vehicle journey rate",
                KeyField = "mileagedateid",
                enableupdating = true,
                editlink = "javascript:SEL.VehicleJourneyRate.DateRange.Edit({mileagedateid});",
                enabledeleting = true,
                deletelink = "javascript:SEL.VehicleJourneyRate.DateRange.Delete({mileagedateid});"
            };
            grid.getColumnByName("mileagedateid").hidden = true;
            grid.addFilter(fields.GetFieldByID(new Guid("417dad53-9162-48cd-b905-37139c5933c6")), ConditionType.Equals, new object[] { mileageId }, null, ConditionJoiner.None);
            ((cFieldColumn)grid.getColumnByName("daterangetype")).addValueListItem((int)DateRangeType.Before, "Before");
            ((cFieldColumn)grid.getColumnByName("daterangetype")).addValueListItem((int)DateRangeType.AfterOrEqualTo, "After or equal to");
            ((cFieldColumn)grid.getColumnByName("daterangetype")).addValueListItem((int)DateRangeType.Between, "Between");
            ((cFieldColumn)grid.getColumnByName("daterangetype")).addValueListItem((int)DateRangeType.Any, "Any");

            var retVals = new List<string> { grid.GridID };
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        ///     Deletes a Mileage Date Range.
        /// </summary>
        /// <param name="mileageId">The parent mileageId.</param>
        /// <param name="mileageDateId">mileageDateId to be deleted.</param>
        /// <returns>3 if the Mileage Category has an "Any" date range, otherwise 0.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public byte DateRangeDelete(int mileageId, int mileageDateId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Delete))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            mileageCats.deleteMileageDateRange(mileageId, mileageDateId);

            cMileageCat mileageCat = mileageCats.GetMileageCatById(mileageId);

            if (mileageCat != null && mileageCat.dateRanges.Any(range => range.daterangetype == DateRangeType.Any && range.mileagedateid != mileageDateId))
            {
                return (byte)DateRangeType.Any;
            }

            return 0;
        }

        /// <summary>
        ///     The save vehicle journey rate.
        /// </summary>
        /// <param name="mileageId">
        ///     The mileage id.
        /// </param>
        /// <param name="carSize">
        ///     The carsize.
        /// </param>
        /// <param name="comment">
        ///     The comment.
        /// </param>
        /// <param name="thresholdType">
        ///     The thresholdtype.
        /// </param>
        /// <param name="calcMilesTotal">
        ///     The calcmilestotal.
        /// </param>
        /// <param name="mileUom">
        ///     The mile uom.
        /// </param>
        /// <param name="currencyId">
        ///     The currencyid.
        /// </param>
        /// <param name="userRateTable">
        ///     The user rate table.
        /// </param>
        /// <param name="userRateEngineFromSize">
        ///     The user rate engine from size.
        /// </param>
        /// <param name="userRateEngineToSize">
        ///     The user rate engine to size.
        /// </param>
        /// <param name="financialYearId">
        ///     The financial Year Id.
        /// </param>
        /// <returns>
        ///     The mileageid.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int VehicleJourneyRateSave(int mileageId, string carSize, string comment, ThresholdType thresholdType, bool calcMilesTotal, MileageUOM mileUom, int currencyId, string userRateTable, int userRateEngineFromSize, int userRateEngineToSize, int financialYearId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var mileageCats = new cMileagecats(user.AccountID);

            cMileageCat oldCat = null;
            if (mileageId > 0)
            {
                oldCat = mileageCats.GetMileageCatById(mileageId);
            }

            if ((oldCat == null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add)) ||
                (oldCat != null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit)))
            {
                throw new Exception("You do not have permission to this method.");
            }

            return mileageCats.saveVehicleJourneyRate(
                new cMileageCat(mileageId, carSize, comment, thresholdType, calcMilesTotal,
                    new List<cMileageDaterange>(), mileUom, false, String.Empty, currencyId,
                    (oldCat != null ? oldCat.createdon : DateTime.UtcNow),
                    (oldCat != null ? oldCat.createdby : user.EmployeeID),
                    (oldCat != null ? (DateTime?)DateTime.UtcNow : null),
                    (oldCat != null ? (int?)user.EmployeeID : null),
                    userRateTable, userRateEngineFromSize, userRateEngineToSize, financialYearId));
        }

        /// <summary>
        ///     Deletes a Vehicle Journey Rate from the database.
        /// </summary>
        /// <param name="mileageid">The mileageid.</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void VehicleJourneyRateDelete(int mileageId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Delete))
            {
                throw new Exception("You do not have permission to this method.");
            }

            new cMileagecats(user.AccountID).deleteMileageCat(mileageId);
        }

        /// <summary>
        ///     Creates a threshold grid.
        /// </summary>
        /// <param name="contextKey">Mileage Date ID.</param>
        /// <returns>An array containing the grid control ID, javascript, and html.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] ThresholdGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            int dateRangeId = 0;
            if (!String.IsNullOrEmpty(contextKey))
            {
                Int32.TryParse(contextKey, out dateRangeId);
            }

            var fields = new cFields(user.AccountID);
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridThresholds", "select mileagethresholdid, rangetype, rangevalue1, rangevalue2 from mileage_thresholds")
            {
                EmptyText = "There are no thresholds defined for this date range",
                enableupdating = true,
                editlink = "javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Edit({mileagethresholdid});",
                enabledeleting = true,
                deletelink = "javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Delete({mileagethresholdid}, " + dateRangeId + ");",
                KeyField = "mileagethresholdid"
            };
            grid.getColumnByName("mileagethresholdid").hidden = true;
            grid.addFilter(fields.GetFieldByID(new Guid("e93d7fc8-3f74-4c9e-ad32-be5fe9d7ace3")), ConditionType.Equals, new object[] { dateRangeId }, null, ConditionJoiner.None);
            ((cFieldColumn)grid.getColumnByName("rangetype")).addValueListItem((int)RangeType.GreaterThanOrEqualTo, "Greater than or equal");
            ((cFieldColumn)grid.getColumnByName("rangetype")).addValueListItem((int)RangeType.LessThan, "Less than");
            ((cFieldColumn)grid.getColumnByName("rangetype")).addValueListItem((int)RangeType.Between, "Between");
            ((cFieldColumn)grid.getColumnByName("rangetype")).addValueListItem((int)RangeType.Any, "Any");

            var gridArray = new List<string> { grid.GridID };
            gridArray.AddRange(grid.generateGrid());
            return gridArray.ToArray();
        }

        /// <summary>
        ///     Saves a Mileage date range.
        /// </summary>
        /// <returns>An array containing the dateRangeId, any error message, and the DateRangeType or null if there was an error.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] DateRangeSave(int dateRangeId, int mileageId, DateRangeType dateRangeType, DateTime? dateValue1, DateTime? dateValue2)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add) && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            var errorMessage = mileageCats.mileageDateRangeExists(mileageId, dateRangeId, ref dateValue1, ref dateValue2, ref dateRangeType, user.EmployeeID);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                return new object[] { dateRangeId, errorMessage, null };
            }

            cMileageDaterange oldDateRange = null;
            if (dateRangeId > 0)
            {
                oldDateRange = mileageCats.getMileageDateRangeById(mileageCats.GetMileageCatById(mileageId), dateRangeId);
            }

            if ((oldDateRange == null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add)) ||
                (oldDateRange != null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit)))
            {
                throw new Exception("You do not have permission to this method.");
            }

            dateRangeId = mileageCats.saveDateRange(
                new cMileageDaterange(dateRangeId, mileageId, dateValue1, dateValue2, new List<cMileageThreshold>(), dateRangeType,
                    (oldDateRange != null ? oldDateRange.createdon : DateTime.UtcNow),
                    (oldDateRange != null ? oldDateRange.createdby : user.EmployeeID),
                    (oldDateRange != null ? (DateTime?)DateTime.UtcNow : null),
                    (oldDateRange != null ? (int?)user.EmployeeID : null)),
                    mileageId);

            return new object[] { dateRangeId, null, dateRangeType };
        }

        /// <summary>
        ///     Saves a Mileage threshold to the database.
        /// </summary>
        /// <returns>An array containing the thresholdId and any error message.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] ThresholdSave(int thresholdId, int mileageId, int dateRangeId, RangeType rangeType, decimal? threshold1, decimal? threshold2, decimal passenger, decimal passengerx, decimal heavyBulky)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add) && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            var errorMessage = mileageCats.mileageThresholdExists(mileageId, dateRangeId, thresholdId, ref rangeType, ref threshold1, ref threshold2, user.EmployeeID);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                return new object[] { thresholdId, errorMessage };
            }

            cMileageThreshold oldThreshold = null;
            if (thresholdId > 0)
            {
                oldThreshold = mileageCats.getMileageThresholdById(mileageCats.getMileageDateRangeById(mileageCats.GetMileageCatById(mileageId), dateRangeId), thresholdId);
            }

            if ((oldThreshold == null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add)) ||
                (oldThreshold != null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit)))
            {
                throw new Exception("You do not have permission to this method.");
            }

            thresholdId = mileageCats.saveThreshold(mileageId,
                new cMileageThreshold(
                    thresholdId, dateRangeId, threshold1, threshold2, rangeType, passenger, passengerx,
                    (oldThreshold != null ? oldThreshold.CreatedOn : DateTime.UtcNow),
                    (oldThreshold != null ? oldThreshold.CreatedBy : user.EmployeeID),
                    (oldThreshold != null ? (DateTime?)DateTime.UtcNow : null),
                    (oldThreshold != null ? (int?)user.EmployeeID : null),
                    heavyBulky));

            return new object[] { thresholdId, null };
        }

        /// <summary>
        ///     Get a MileageThresholdDateRange from the database.
        /// </summary>
        /// <param name="dateRangeId">dateRangeId.</param>
        /// <param name="mileageId">The parent mileageId.</param>
        /// <returns>
        ///     An array containing the MileageDateRange object and "3" if any of the MileageThreshold.RangeTypes is "Any",
        ///     otherwise null.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] DateRangeGet(int dateRangeId, int mileageId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            cMileageDaterange dateRange = mileageCats.getMileageDateRangeById(mileageCats.GetMileageCatById(mileageId), dateRangeId);

            return new object[] { dateRange, dateRange.thresholds.Any(t => t.RangeType == RangeType.Any) ? (byte?)RangeType.Any : null };
        }

        /// <summary>
        ///     Get a Mileage Threshold from the database.
        /// </summary>
        /// <param name="thresholdId">thresholdId.</param>
        /// <param name="dateRangeId">The parent dateRangeId.</param>
        /// <param name="mileageId">The grandparent mileageId.</param>
        /// <returns>MileageThreshold object.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cMileageThreshold ThresholdGet(int thresholdId, int dateRangeId, int mileageId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            return mileageCats.getMileageThresholdById(mileageCats.getMileageDateRangeById(mileageCats.GetMileageCatById(mileageId), dateRangeId), thresholdId);
        }

        /// <summary>
        ///     Deletes a Mileage Threshold from the database.
        /// </summary>
        /// <param name="thresholdId">thresholdId.</param>
        /// <param name="dateRangeId">The parent dateRangeId.</param>
        /// <param name="mileageId">The grandparent mileageId.</param>
        /// <returns>3 if the parent Date Range has an "Any" Threshold, otherwise 0.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public byte ThresholdDelete(int mileageId, int dateRangeId, int thresholdId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Delete))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            mileageCats.deleteMileageThreshold(thresholdId, mileageId);

            cMileageCat grandparentMileageCat = mileageCats.GetMileageCatById(mileageId);
            if (grandparentMileageCat != null)
            {
                cMileageDaterange parentDateRange = mileageCats.getMileageDateRangeById(grandparentMileageCat, dateRangeId);
                if (parentDateRange != null && parentDateRange.thresholds.Any(threshold => threshold.RangeType == RangeType.Any && threshold.MileageThresholdId != thresholdId))
                {
                    return (byte)RangeType.Any;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Get a list of DateRangeTypes for the given DateRange.
        /// </summary>
        /// <param name="mileageId">Parent mileageId.</param>
        /// <param name="dateRangeType">dateRangeType.</param>
        /// <returns>A comma separated list of DateRangeTypes.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string DateRangeAvailableTypes(int mileageId, object dateRangeType)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            cMileageCat mileageCat = new cMileagecats(user.AccountID).GetMileageCatById(mileageId);
            if (mileageCat == null)
            {
                return String.Empty;
            }

            if (mileageCat.dateRanges.Count == 0)
            {
                return "0,1,2,3";
            }

            bool containsAny = false;
            bool containsBefore = false;
            bool containsAfter = false;

            foreach (cMileageDaterange curRange in mileageCat.dateRanges)
            {
                switch (curRange.daterangetype)
                {
                    case DateRangeType.Any:
                        containsAny = true;
                        break;

                    case DateRangeType.Before:
                        containsBefore = true;
                        break;

                    case DateRangeType.AfterOrEqualTo:
                        containsAfter = true;
                        break;
                }
            }

            if (containsAny)
            {
                return "0,1,2,3";
            }
                    
            if (containsBefore && !containsAfter)
            {
                //If is null then a date range is being added
                return dateRangeType != null ? "0,1,2" : "1,2";
            }
                    
            if (!containsBefore && containsAfter)
            {
                //If is null then a date range is being added
                return dateRangeType != null ? "0,1,2" : "0,2";
            }

            if (!containsBefore)
            {
                return "0,1,2";
            }

            //If is null then a date range is being added
            if (dateRangeType == null)
            {
                return "2";
            }

            switch ((DateRangeType)Byte.Parse(dateRangeType.ToString()))
            {
                case DateRangeType.Before:
                    return "0,2";

                case DateRangeType.AfterOrEqualTo:
                    return "1,2";

                default:
                    return "2";
            }
        }

        /// <summary>
        ///     Get the Threshold rates grid.
        /// </summary>
        /// <param name="gridId">Control ID.</param>
        /// <param name="thresholdId">thresholdId.</param>
        /// <returns>An array containing the grid control ID, javascript, and html.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] ThresholdRateGrid(string gridId, int thresholdId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var gridArray = new List<string> { gridId };
            gridArray.AddRange(aemileage.GenerateRatesGrid(user, gridId, thresholdId));
            return gridArray.ToArray();
        }

        /// <summary>
        ///     Get a comma separated list of valid new RangeTypes.
        /// </summary>
        /// <param name="mileageId">The parent mileageId.</param>
        /// <param name="dateRangeId">dateRangeId.</param>
        /// <param name="thresholdType">thresholdType.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string ThresholdAvailableRangeTypes(int mileageId, int dateRangeId, object thresholdType)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to this method.");
            }

            var mileageCats = new cMileagecats(user.AccountID);
            cMileageCat mileageCat = mileageCats.GetMileageCatById(mileageId);
            cMileageDaterange dateRange = mileageCats.getMileageDateRangeById(mileageCat, dateRangeId);

            if (mileageCat == null || dateRange == null)
            {
                return String.Empty;
            }

            if (dateRange.thresholds.Count == 0)
            {
                return "0,1,2,3";
            }
                
            bool containsAny = false;
            bool containsLessThan = false;
            bool containsGreater = false;

            foreach (cMileageThreshold threshold in dateRange.thresholds)
            {
                switch (threshold.RangeType)
                {
                    case RangeType.Any:
                        containsAny = true;
                        break;

                    case RangeType.LessThan:
                        containsLessThan = true;
                        break;

                    case RangeType.GreaterThanOrEqualTo:
                        containsGreater = true;
                        break;
                }
            }
                
            if (containsAny)
            {
                return "0,1,2,3";
            }
                    
            if (containsLessThan && !containsGreater)
            {
                //If is null then a threshold is being added
                return thresholdType != null ? "0,1,2" : "0,1";
            }
                    
            if (!containsLessThan && containsGreater)
            {
                //If is null then a threshold is being added
                return thresholdType != null ? "0,1,2" : "1,2";
            }

            if (!containsLessThan)
            {
                return "0,1,2";
            }

            //If is null then a threshold is being added
            if (thresholdType == null)
            {
                return "1";
            }

            if (((RangeType)Byte.Parse(thresholdType.ToString())) == RangeType.LessThan)
            {
                return "1,2";
            }
                            
            if (((RangeType)Byte.Parse(thresholdType.ToString())) == RangeType.GreaterThanOrEqualTo)
            {
                return "0,1";
            }

            return "1";
        }

        /// <summary>
        ///     Gets a Mileage Threshold Rate.
        /// </summary>
        /// <param name="thresholdRateId">thresholdRateId.</param>
        /// <returns>The VehicleJourneyRateThresholdRate.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public VehicleJourneyRateThresholdRate ThresholdRateGet(int thresholdRateId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.View))
            {
                throw new Exception("You do not have permission to view VJR threshold rates.");
            }

            return VehicleJourneyRateThresholdRate.Get(user, thresholdRateId);
        }

        /// <summary>
        ///     Save a VJR threshold rate to the database.
        /// </summary>
        /// <param name="thresholdRate">The VJR threshold rate data to save.</param>
        /// <returns>An object which contains true on success or a message on error.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Response ThresholdRateSave(VehicleJourneyRateThresholdRate thresholdRate)
        {
            try
            {
                CurrentUser user = cMisc.GetCurrentUser();
                if ((thresholdRate.MileageThresholdRateId == null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Add)) ||
                    (thresholdRate.MileageThresholdRateId != null && !svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Edit)))
                {
                    return new Response { Message = "You do not have permission to save a VJR threshold rate." };
                }

                thresholdRate.Save(user);

                return new Response { Success = true };
            }
            catch (ValidationException ex)
            {
                return new Response { Message = ex.Message, Controls = new[] { ex.Field } };
            }
            catch (Exception ex)
            {
                return new Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }
        }

        /// <summary>
        ///     Delete a VJR threshold rate from the database.
        /// </summary>
        /// <param name="mileageThresholdRateId">The VJR threshold rate ID to delete.</param>
        /// <returns>An object which contains true on success or a message on error.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Response ThresholdRateDelete(int mileageThresholdRateId)
        {
            try
            {
                CurrentUser user = cMisc.GetCurrentUser();
                if (!svcVehicleJourneyRate.CurrentUserHasPermission(user, AccessRoleType.Delete))
                {
                    return new Response { Message = "You do not have permission to this method." };
                }

                VehicleJourneyRateThresholdRate.Delete(user, mileageThresholdRateId);

                return new Response { Success = true };
            }
            catch (Exception ex)
            {
                return new Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }
        }

        private static bool CurrentUserHasPermission(ICurrentUser user, AccessRoleType requiredAccessRoleType)
        {
            return (user != null && user.Account != null &&
                    user.CheckAccessRole(requiredAccessRoleType, SpendManagementElement.VehicleJourneyRateCategories, true));
        }
    }
}
