namespace SpendManagementApi.Models.Types.Addresses
{
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// A route taken when traveling.
    /// </summary>
    public class Route : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Addresses.Route, Route>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class. 
        /// Creates an instance of <see cref="Route"/>
        /// </summary>
        /// <param name="routingError">
        /// The routing error message
        /// </param>
        public Route(string routingError)
        {
            this.Error = routingError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class. 
        /// Creates an instance of <see cref="Route"/>
        /// </summary>
        public Route()
        {

        }

        /// <summary>
        /// Gets or sets any error that has occured for this route, string.Empty if no error has occured
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the type of routing lookup that is performed e.g. fastest or shortest
        /// </summary>
        public string RoutingType { get; set; }

        /// <summary>
        /// Gets or sets the way points for this route.
        /// </summary>
        public List<string> WayPoints { get; set; }

        /// <summary>
        /// Gets or sets the steps for this route.
        /// </summary>
        public List<RouteStep> Steps { get; set; }

        /// <summary>
        /// Gets or sets the total time formatted for this route i.e. 1h 27m or 1h or 37m.
        /// </summary>
        public string TotalTimeFormatted { get; set; }

        /// <summary>
        /// Gets a formatted version of the distance for this RouteStep eg 2.4 mi or 400 ft
        /// </summary>
        public string DistanceFormatted { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public Route From(SpendManagementLibrary.Addresses.Route dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            var routeSteps = dbType.Steps.Select(routeStep => new RouteStep().From(routeStep, actionContext)).ToList();

            this.Error = dbType.Error;
            this.RoutingType = dbType.RoutingType;
            this.WayPoints = dbType.WayPoints;
            this.Steps = routeSteps;
            this.TotalTimeFormatted = dbType.TotalTimeFormatted;
            this.DistanceFormatted = dbType.DistanceFormatted;

            return this;

        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Addresses.Route To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}