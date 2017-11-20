namespace SpendManagementApi.Models.Types.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using global::Utilities.StringManipulation;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Requests.Address;

    /// <summary>
    /// A step in a route
    /// </summary>
    public class RouteStep : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Addresses.RouteStep, RouteStep>
    {
        /// <summary>
        /// Gets or sets the sequential step number of a route
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// Gets or sets the action of this route step (eg left turn)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the description of this route step (eg left turn on to [Doddington Road]), this is a modified version of the description retrieved from PostcodeAnywhere
        /// </summary>
        public string Description { get; set; }
      
        /// <summary>
        /// Gets or sets the formatted total time up to this step in the route in hours and minutes
        /// </summary>
        public string TotalTimeFormatted { get; set; }
      
        /// <summary>
        /// Gets or sets the estimated total distance of this route uptil this route step
        /// </summary>
        public int TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="MileageJourneyCoordinate">MileageJourneyCoordinate</see>
        /// </summary>
        public List<MileageJourneyCoordinate> Line { get; set; }

        /// <summary>
        /// Gets or sets the step distance in feet
        /// </summary>
        public double StepDistanceInFeet { get; set; }

        /// <summary>
        /// Gets or sets a formatted version of the distance for this RouteStep eg 2.4 mi or 400 ft
        /// </summary>
        public string StepDistanceFormatted { get; set; }

        /// <summary>
        /// Gets or sets the formatted step time in hours and minutes
        /// </summary>
        public string StepTimeFormatted { get; set; }

        /// <summary>
        /// Gets or sets an image related to the Action of the RouteStep
        /// </summary>
        public string ActionImage { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public RouteStep From(SpendManagementLibrary.Addresses.RouteStep dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            HtmlUtility util = HtmlUtility.Instance;

            this.StepNumber = dbType.StepNumber;
            this.Action = dbType.Action;
            this.Description = util.RemoveHTMLTagsFromString(dbType.Description);
            this.TotalTimeFormatted = dbType.TotalTimeFormatted;
            this.TotalDistance = dbType.TotalDistance;
            this.Line = this.ProcessCoordinates(dbType.Line);
            this.StepDistanceInFeet = dbType.StepDistanceInFeet;
            this.StepTimeFormatted = dbType.StepTimeFormatted;
            this.ActionImage = dbType.ActionImage;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Addresses.RouteStep To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Process routes coordinates. For examples,  "[[53.21993,-0.45764],[53.21998,-0.45712],[53.21989,-0.45681]]" becomes a list of <see cref="MileageJourneyCoordinate">MileageJourneyCoordinate</see>
        /// </summary>
        /// <param name="coordinates">
        /// The coordinates object.
        /// </param>
        /// <returns>
        /// A <see cref="MileageJourneyCoordinate">MileageJourneyCoordinate</see>
        /// </returns>
        private List<MileageJourneyCoordinate> ProcessCoordinates(object coordinates)
        {       
            var matchResults = new List<string>();
    
            // remove leading and trailing [ ]
            string format = coordinates.ToString();
            format = format.Remove(0, 1);
            format = format.Remove(format.Length - 1);

            var pattern = @"\[(.*?)\]";
            var matches = Regex.Matches(format, pattern);

            foreach (Match m in matches)
            {
                Group result = m.Groups[1];
                matchResults.Add(result.ToString());
            }

            char[] splitchar = { ',' };
            var mileageJourneyCoordinateList = new List<MileageJourneyCoordinate>();

            foreach (string coordinate in matchResults)
            {
                // split string into latitude and longitude 
                string[] split = coordinate.Split(splitchar);

                mileageJourneyCoordinateList.Add(new MileageJourneyCoordinate { Latitude = split[0], Longitude = split[1] });
            }

            return mileageJourneyCoordinateList;
        }
    }
}