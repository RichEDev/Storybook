namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Postcode Anywhere related functionality
    /// </summary>
    public class PostcodeAnywhere
    {
        #region Fields

        /// <summary>
        /// Our PostcodeAnywhere account code
        /// </summary>
        private const string AccountCode = "SOFTW11120";

        /// <summary>
        /// The accountID for the currently logged in user
        /// </summary>
        private readonly int? _accountIdentifier;

        /// <summary>
        /// Our default PostcodeAnywhere license key - used if none is specified for this account in cAccount
        /// </summary>
        private readonly string _postCodeAnywhereLicenseKey = "BD99-GB22-RR15-XC56";

        #endregion Fields

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="PostcodeAnywhere"/> class.
        /// </summary>
        /// <param name="accountIdentifier">The accountid.</param>
        public PostcodeAnywhere(int? accountIdentifier)
        {
            this._accountIdentifier = accountIdentifier;
            var accounts = new cAccounts();
            string postcodeAnywhereKey = this._accountIdentifier.HasValue ? 
                accounts.GetAccountByID(this._accountIdentifier.Value).PostcodeAnywhereKey :
                string.Empty;
            if (postcodeAnywhereKey != string.Empty)
            {
                this._postCodeAnywhereLicenseKey = postcodeAnywhereKey;
            }
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        /// <summary>
        /// Looks up an address based on a search term for a country.
        /// </summary>
        /// <param name="searchterm">The criteria to search for</param>
        /// <param name="country">The country</param>
        /// <returns>A DataSet containing Id, Match, Suggestion, IsRetrievable, AutoSelect</returns>
        public DataTable CapturePlusInteractiveAutoComplete(string searchterm, string country)
        {
            // Build the url
            var url = "http://services.postcodeanywhere.co.uk/CapturePlus/Interactive/AutoComplete/v2.00/dataset.ws?";
            url += "&Key=" + HttpUtility.UrlEncode(this._postCodeAnywhereLicenseKey);
            url += "&SearchTerm=" + HttpUtility.UrlEncode(searchterm);
            url += "&Country=" + HttpUtility.UrlEncode(country);

            // Create the dataset
            var dataSet = new DataSet();

            using (dataSet)
            {
                dataSet.ReadXml(url);

                if (dataSet.Tables.Count == 0)
                {
                    return new DataTable();
                }

                var dataTable = dataSet.Tables[0];

                // Check for an error
                if (dataSet.Tables.Count == 1 && dataTable.Columns.Count == 4 && dataTable.Columns[0].ColumnName == "Error")
                {
                    return new DataTable();
                }

                // Return the dataset
                return dataTable;
            }
        }

        /// <summary>
        /// Looks up an address based on a Postcode Anywhere Id
        /// Uses PostcodeAnywhere's "InteractiveRetrieveById" API Endpoint
        /// </summary>
        /// <param name="id">The PostcodeAnywhere retrievable address Id</param>
        /// <returns>A DataSet containing the resultant address data</returns>
        public DataTable CapturePlusInteractiveRetrieveById(string id)
        {
            // Build the url
            var url = "http://services.postcodeanywhere.co.uk/CapturePlus/Interactive/RetrieveById/v2.00/dataset.ws?";
            url += "&Key=" + HttpUtility.UrlEncode(this._postCodeAnywhereLicenseKey);
            url += "&Id=" + HttpUtility.UrlEncode(id);
            url += "&UserName=";
            url += "&Application=";
            url += "&CustomerId=";

            // Create the datatable
            var dataSet = new DataSet();
            using (dataSet)
            {
                dataSet.ReadXml(url);
                var dataTable = dataSet.Tables[0];

                // Check for an error
                if (dataTable.Columns.Count == 4 && dataTable.Columns[0].ColumnName == "Error")
                {
                    // An error has occurred
                    return new DataTable();
                }

                // Return the dataset
                return dataTable;
            }
        }

        /// <summary>
        /// Looks up an address by its postcode anywhere identifier.
        /// </summary>
        /// <param name="postcodeAnywhereAutoCompleteIdentifier">The ID to search for.</param>
        /// <returns>A DataTable of results from the web service call</returns>
        public DataTable CapturePlusInteractiveFind(string postcodeAnywhereAutoCompleteIdentifier) 
        {
            // Build the url
            var url = "http://services.postcodeanywhere.co.uk/CapturePlus/Interactive/AutoCompleteFind/v2.00/dataset.ws?";
            url += "&Key=" + HttpUtility.UrlEncode(this._postCodeAnywhereLicenseKey);
            url += "&Id=" + postcodeAnywhereAutoCompleteIdentifier;

            // Create the datatable
            var dataSet = new DataSet();
            using (dataSet)
            {
                dataSet.ReadXml(url);
                var dataTable = dataSet.Tables[0];

                // Check for an error
                if (dataTable.Columns.Count == 4 && dataTable.Columns[0].ColumnName == "Error")
                {
                    // An error has occurred
                    return new DataTable();
                }

                // Return the dataset
                return dataTable;
            }
        }

        /// <summary>
        /// Get the distance between locations, returns the distances closest to the centre of the postcode on a route
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="origin">The coordinates (longitude/latitude comma separated pair) of the start of the route. A postcode is also valid</param>
        /// <param name="destination">The coordinates (longitude/latitude comma separated pair) of the finish of the route. A postcode is also valid.</param>
        /// <param name="wayPoints">The coordinates (latitude, longitude or easting, northing) of any waypoints. Postcodes are also valid. (comma seperated)</param>
        /// <param name="distanceType">Specifies how the distances between the stores are calculated.</param>
        /// <returns>The distance of the route or 0 if a distance cannot be obtained</returns>
        public decimal? GetDistanceByClosestRoadOnRouteToPostcodeCentre(ICurrentUserBase user, string origin, string destination, string wayPoints, DistanceType distanceType)
        {
            string distanceFieldName = (distanceType == DistanceType.Fastest) ? "FastestDistanceWithRoads" : "ShortestDistanceWithRoads";
            decimal? distance = GetDistanceFromGlobalAddressDatabase(origin, destination, distanceFieldName);

            // if there was no cached distance look it up 
            if (!distance.HasValue)
            {
                string url = string.Format("http://services.postcodeanywhere.co.uk/DistancesAndDirections/Interactive/Distance/v1.00/dataset.ws?Key={0}&Start={1}&Finish={2}&WayPoints={3}&DistanceType={4}", this._postCodeAnywhereLicenseKey, origin, destination, wayPoints, distanceType);
                var dataset = new DataSet();
                using (dataset)
                {
                    dataset.ReadXml(url);

                    if (dataset.Tables.Count == 1 && dataset.Tables[0].Columns.Count == 4 && dataset.Tables[0].Columns[0].ColumnName == "Error")
                    {
                        cEventlog.LogEntry(string.Format("Unable to retrieve GetDistanceByClosestRoadOnRouteToPostcodeCentre from PostcodeAnywhere\nStart: {0}\nEnd: {1}\nWaypoints: {2}\nException: {3}", origin, destination, wayPoints, dataset.Tables[0].Rows[0].ItemArray[1]));
                        return null;
                    }

                    distance = Convert.ToDecimal(Math.Round(Convert.ToInt32(dataset.Tables[0].Rows[0]["TotalDistance"]) / 1609.344, 2, MidpointRounding.AwayFromZero));
                }

                LogAddressDistanceLookup(user, origin, destination);
                SaveDistanceInGlobalAddressDatabase(origin, destination, distance.Value, distanceFieldName);
            }

            return distance.Value;
        }

        /// <summary>
        /// Looks up a distance between the centre of two UK postcodes
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="origin">The coordinates (longitude/latitude comma separated pair) of the start of the route. A postcode is also valid</param>
        /// <param name="destination">The coordinates (longitude/latitude comma separated pair) of the finish of the route. A postcode is also valid.</param>
        /// <param name="distanceType">The distance type</param>
        /// <param name="cost">The cost</param>
        /// <returns>The distance between two postcodes or 0 if a distance cannot be found.</returns>
        public decimal? LookupDistanceByPostcodeCentres(ICurrentUserBase user, string origin, string destination, DistanceType distanceType, PostcodeAnywhereCost cost)
        {
            string distanceFieldName = (cost == PostcodeAnywhereCost.Time) ? "FastestDistanceWithPostcodeCentres" : "ShortestDistanceWithPostcodeCentres";
            decimal? distance = GetDistanceFromGlobalAddressDatabase(origin, destination, distanceFieldName);

            // if there was no cached distance look it up 
            if (!distance.HasValue)
            {
                string url = string.Format(
                    "http://services.postcodeanywhere.co.uk/uk/lookup.asmx/Distance2?Origin={0}&Destination={1}&AccountCode={2}&LicenseKey={3}&DistanceType={4}&Cost={5}&MachineId={6}",
                    origin,
                    destination,
                    AccountCode,
                    this._postCodeAnywhereLicenseKey,
                    distanceType,
                    cost,
                    string.Empty);

                var dataset = new DataSet();
                using (dataset)
                {
                    dataset.ReadXml(url);

                    if (dataset.Tables.Count == 1 && dataset.Tables[0].Columns.Count == 3 && dataset.Tables[0].Columns[0].ColumnName == "IsError")
                    {
                        cEventlog.LogEntry(string.Format(
                            "Unable to retrieve LookupDistanceByPostcodeCentres from PostcodeAnywhere\nOrigin: {0}\nDestination: {1}\ndistanceType: {2}\ncost: {3}\nException: {4}",
                            origin,
                            destination,
                            distanceType,
                            cost,
                            dataset.Tables[0].Rows[0].ItemArray[1]));
                        return null;
                    }

                    distance = decimal.Parse(dataset.Tables[2].Rows[0]["Distance"].ToString());
                }

                LogAddressDistanceLookup(user, origin, destination);
                SaveDistanceInGlobalAddressDatabase(origin, destination, distance.Value, distanceFieldName);
            }

            return distance; 
        }

        /// <summary>
        /// Gets a set of route steps from a start point to an end point
        /// </summary>
        /// <param name="wayPoints">
        /// The way Points.
        /// </param>
        /// <param name="distanceType">
        /// The distance Type.
        /// </param>
        /// <param name="dateTime">
        /// The DateTime the route is to start on. Provide this parameter only if you wish to utilise superior 'Speed Profiles' historic traffic speed data over time for avoiding rush hours and such.
        /// </param>
        /// <param name="actionImagePrefix">
        /// The prefix address to the images used to show each Route Step action.
        /// </param>
        /// <param name="polyLineResolution">
        /// Specifies the maximum deviation of the returned polyline from the true polyline in metres. Set to 0 to return the full-resolution polyline or higher for a simplified polyline with fewer points, defaults to 15.
        /// </param>
        /// <param name="lineStyle">
        /// Specifies the output format for the line, defaults to "JSLatLong"
        /// </param>
        /// <returns>
        /// A list of route steps from the start location to the finish location or null if an error occurs.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown if a start or finish waypoint can not be extracted
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any arguments are null
        /// </exception>
        public Route GetRoute(List<string> wayPoints, DistanceType distanceType, DateTime dateTime, string actionImagePrefix, int polyLineResolution = 15, string lineStyle = "JSLatLong")
        {
            if (wayPoints == null)
            {
                return new Route(Route.NoWayPoints);
            }

            /* Check to see if there are any missing/blank lines in the middle of a journey and that each even number (destinations) is equal to the next odd number (origins) bar the first and last.
             * 
             * Journey grid example:
             *  FROM (1)    TO (2)
             *  FROM (3)    TO (4)
             *  FROM (5)    TO (6)
             *  
             * Checks work in reverse order 6 to 1 as opposed to 1 to 6
             *   
             * As soon as one box has a value then every preceeding box must have one, if not they have removed a to/from which would result in an invalid route             * 
             * 2 should be the same as 3, 4 should be the same as 5 - if they are not they are not correctly sequenced as a single journey
             */
            bool wayPointFound = false;
            string lastFromWayPoint = string.Empty;
            for (int i = wayPoints.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(wayPoints[i]) == false)
                {
                    wayPointFound = true;

                    if (i % 2 == 0)
                    {
                        lastFromWayPoint = wayPoints[i];
                    }
                    else
                    {
                        if (lastFromWayPoint != wayPoints[i] && lastFromWayPoint != string.Empty)
                        {
                            return new Route(Route.InvalidWayPointSequence);
                        }
                    }
                }
                else
                {
                    if (wayPointFound)
                    {
                        return new Route(Route.InvalidWayPointSequence);
                    }
                }
            }

            // remove any waypoints going to the same waypoint
            var finalWaypoints = new List<string>();
            foreach (string wayPoint in wayPoints)
            {
                if (finalWaypoints.Count == 0 || finalWaypoints[finalWaypoints.Count - 1] != wayPoint)
                {
                    if (string.IsNullOrWhiteSpace(wayPoint) == false)
                    {
                        finalWaypoints.Add(wayPoint);
                    }
                    else
                    {
                        return new Route(Route.InvalidWayPoints);
                    }
                }
            }

            if (finalWaypoints.Count < 2)
            {
                return new Route(Route.InvalidWayPoints);
            }

            string start = finalWaypoints[0];
            string finish = finalWaypoints[finalWaypoints.Count - 1];

            var sb = new StringBuilder();
            for (int i = 1; i < finalWaypoints.Count; i++)
            {
                if (i < finalWaypoints.Count - 1)
                {
                    sb.Append(finalWaypoints[i] + ",");
                }
            }

            if (string.IsNullOrWhiteSpace(start))
            {
                return new Route(Route.NoStartWayPoint);
            }

            if (string.IsNullOrWhiteSpace(finish))
            {
                return new Route(Route.NoDestinationWayPoint);
            }

            if (polyLineResolution < 1)
            {
                polyLineResolution = 15;
            }

            string dayOfWeek = dateTime.ToString("dddd");
            string minutesSinceMidWeek = Convert.ToString((int)Math.Round(DateTime.Now.TimeOfDay.TotalSeconds / 60));

            string url = string.Format("http://services.postcodeanywhere.co.uk/DistancesAndDirections/Interactive/DirectionsAndLines/v3.00/dataset.ws?Key={0}&Start={1}&Finish={2}&WayPoints={3}&DistanceType={4}&LineStyle={5}&StartDay={6}&StartTime={7}&PolyLineResolution={8}", this._postCodeAnywhereLicenseKey, HttpUtility.UrlEncode(start), HttpUtility.UrlEncode(finish), HttpUtility.UrlEncode(sb.ToString()), distanceType, lineStyle, dayOfWeek, minutesSinceMidWeek, polyLineResolution);

            var dataSet = new DataSet();

            using (dataSet)
            {
                dataSet.ReadXml(url);

                // Check for an error
                if (dataSet.Tables.Count == 1 && dataSet.Tables[0].Columns.Count == 4 && dataSet.Tables[0].Columns[0].ColumnName == "Error")
                {
                    cEventlog.LogEntry(string.Format("Unable to retrieve DirectionAndLines from PostcodeAnywhere.\nStart: {0}\nEnd: {1}\nWaypoints: {2}\nException: {3}", start, finish, sb, dataSet.Tables[0].Rows[0].ItemArray[1]));
                    return new Route(Route.SupplierError);
                }
            }

            var route = new Route(finalWaypoints, (from DataRow row in dataSet.Tables[0].Rows select new RouteStep(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToString(row[2]), Convert.ToString(row[3]), Convert.ToString(row[4]), Convert.ToInt32(row[5]), Convert.ToInt32(row[6]), Convert.ToInt32(row[7]), Convert.ToInt32(row[8]), row[9])).ToList(), distanceType);

            foreach (RouteStep routeStep in route.Steps)
            {
                routeStep.ActionImage = actionImagePrefix + routeStep.ActionImage;
            }

            return route;
        }

        #endregion Public Methods and Operators

        /// <summary>
        /// Attempts to retrieve the distance between two locations in the global AddressPostcodeDistances table
        /// </summary>
        /// <param name="origin">The "from" postcode</param>
        /// <param name="destination">The "to" postcode></param>
        /// <param name="distanceFieldName">The name of the distance field from which to retrieve the distance</param>
        /// <returns>The distance, or null if none was found</returns>
        private static decimal? GetDistanceFromGlobalAddressDatabase(string origin, string destination, string distanceFieldName)
        {
            var db = new DatabaseConnection(GlobalVariables.GlobalAddressDatabaseConnectionString);

            // attempt to get the cached distance from the global address database
            string sql = string.Format("SELECT TOP 1 {0} FROM addressLocatorDistances WHERE OriginLocator = @Origin AND DestinationLocator = @Destination", distanceFieldName);
            db.sqlexecute.Parameters.AddWithValue("@Origin", origin);
            db.sqlexecute.Parameters.AddWithValue("@Destination", destination);

            var distance = db.ExecuteScalar<object>(sql);
            return (distance == null || distance == DBNull.Value) ? null : (decimal?)distance;
        }

        /// <summary>
        /// Saves a distance between two locations in the global AddressPostcodeDistances table
        /// </summary>
        /// <param name="origin">The "from" postcode</param>
        /// <param name="destination">The "to" postcode></param>
        /// <param name="distance">The distance between the two locations</param>
        /// <param name="distanceFieldName">The name of the distance field in which to store the distance</param>
        private static void SaveDistanceInGlobalAddressDatabase(string origin, string destination, decimal distance, string distanceFieldName)
        {
            distanceFieldName = "@" + distanceFieldName;
            using (IDBConnection databaseConnection = new DatabaseConnection(GlobalVariables.GlobalAddressDatabaseConnectionString))
            {
                databaseConnection.AddWithValue("@Origin", origin);
                databaseConnection.AddWithValue("@Destination", destination);
                databaseConnection.AddWithValue(distanceFieldName, distance, 18, 2);
                databaseConnection.ExecuteProc("SaveAddressDistance");

                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Saves a log entry in the AddressDistanceLookupLog table and updates the AddressLookupsRemaining account property
        /// </summary>
        /// <param name="user">the current user</param>
        /// <param name="origin">the origin (postcode / coordinate) for the distance lookup</param>
        /// <param name="destination">the destination (postcode / coordinate) for the distance lookup</param>
        private static void LogAddressDistanceLookup(ICurrentUserBase user, string origin, string destination)
        {
            var accounts = new cAccounts();
            var account = accounts.GetAccountByID(user.AccountID);

            if (account.AddressLookupChargeable)
            {
                accounts.DecrementAddressDistanceLookupsRemaining(account.accountid);

                var db = new DBConnection(account.ConnectionString);
                string sql = string.Format("INSERT INTO addressDistanceLookupLog (Origin, Destination) VALUES (@Origin, @Destination);");
                db.sqlexecute.Parameters.AddWithValue("@Origin", origin);
                db.sqlexecute.Parameters.AddWithValue("@Destination", destination);

                db.ExecuteSQL(sql);
            }
        }

        /// <summary>
        /// Saves a log entry in the AddressLookupLog table and updates the AddressLookupsRemaining account property
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="address">The address that was looked up</param>
        public static void LogAddressLookup(ICurrentUserBase user, Address address)
        {
            var accounts = new cAccounts();
            var account = accounts.GetAccountByID(user.AccountID);

            if (account.AddressLookupChargeable)
            {
                accounts.DecrementAddressLookupsRemaining(account.accountid);

                var db = new DBConnection(account.ConnectionString);
                string sql = string.Format("INSERT INTO addressLookupLog (AddressText) VALUES (@AddressText);");
                db.sqlexecute.Parameters.AddWithValue("@AddressText", address.FriendlyName);

                db.ExecuteSQL(sql);
            }
        }
    }
}
