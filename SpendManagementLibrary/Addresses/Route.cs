namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A route taken when traveling.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Error message for when no waypoints are supplied.
        /// </summary>
        public const string NoWayPoints = "No waypoints supplied.";

        /// <summary>
        /// Error message for when invalid waypoints are supplied.
        /// </summary>
        public const string InvalidWayPoints = "Invalid waypoints supplied.";

        /// <summary>
        /// Error message when the start cannot be determined.
        /// </summary>
        public const string NoStartWayPoint = "Unable to determine start waypoint.";

        /// <summary>
        /// Error message for when the destination can not be determined.
        /// </summary>
        public const string NoDestinationWayPoint = "Unable to determine destination waypoint.";

        /// <summary>
        /// Error message for when PostCodeAnywhere cannot be reached or errors when trying to obtain route
        /// </summary>
        public const string SupplierError = "Unable to retrieve route information.";

        /// <summary>
        /// Error when a generic invalid request is made
        /// </summary>
        public const string InvalidRequest = "Invalid request.";

        /// <summary>
        /// Error when the user does not have permission to view the route, data protection of home addresses for example
        /// </summary>
        public const string NoPermission = "You do not have permission to view the route.";

        /// <summary>
        /// Error message for when an invalid sequence is requested e.g. missing waypoints in the middle of a route
        /// </summary>
        public const string InvalidWayPointSequence = "Invalid waypoint sequence.";

        /// <summary>Initializes a new instance of the <see cref="Route"/> class.</summary>
        /// <param name="wayPoints">The way points for this route</param>
        /// <param name="routeSteps">The route steps.</param>
        /// <param name="routingType">The routing type used.</param>
        public Route(List<string> wayPoints, List<RouteStep> routeSteps, DistanceType routingType)
        {
            this.RoutingType = routingType.ToString();
            this.WayPoints = wayPoints;
            this.Steps = routeSteps;
            this.DistanceInMeters = routeSteps[routeSteps.Count - 1].TotalDistance;
            this.TotalTimeFormatted = routeSteps[routeSteps.Count - 1].TotalTimeFormatted;
            this.Error = string.Empty;
        }

        public Route(string routingError)
        {
            this.Error = routingError;
        }

        /// <summary>
        /// Gets or sets any error that has occured for this route, string.Empty if no error has occured
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// Gets or sets the type of routing lookup that is performed e.g. fastest or shortest
        /// </summary>
        public string RoutingType { get; protected set; }

        /// <summary>
        /// Gets or sets the way points for this route.
        /// </summary>
        public List<string> WayPoints { get; protected set; }

        /// <summary>
        /// Gets or sets the steps for this route.
        /// </summary>
        public List<RouteStep> Steps { get; protected set; }

        /// <summary>
        /// Gets or sets the total time formatted for this route i.e. 1h 27m or 1h or 37m.
        /// </summary>
        public string TotalTimeFormatted { get; set; }

        /// <summary>
        /// Gets a formatted version of the distance for this RouteStep eg 2.4 mi or 400 ft
        /// </summary>
        public string DistanceFormatted
        {
            get
            {
                double distance = this.DistanceInMiles;
                if (distance == 0)
                {
                    return "&nbsp;";
                }

                if (distance < 0.5)
                {
                    return this.DistanceInMeters + " ft";
                }

                return distance + " mi";
            }
        }

        /// <summary>
        /// Gets or sets the estimated distance in meters this route step is
        /// </summary>
        private int DistanceInMeters { get; set; }

        /// <summary>
        /// Gets the estimated distance in miles this route step is (rounded at two decimal places)
        /// </summary>
        private double DistanceInMiles
        {
            get
            {
                return Math.Round(this.DistanceInMeters / 1609.344, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}