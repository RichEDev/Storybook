namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A step in a route
    /// </summary>
    public class RouteStep
    {
        /// <summary>
        /// The description retrieved from PostcodeAnywhere.
        /// </summary>
        private string _description;

        /// <summary>
        /// The _action image for this RouteStep.
        /// </summary>
        private string _actionImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteStep"/> class.
        /// </summary>
        /// <param name="segementNumber">
        /// The segement number.
        /// </param>
        /// <param name="stepNumber">
        /// The step number.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="road">
        /// The road.
        /// </param>
        /// <param name="stepTime">
        /// The step time.
        /// </param>
        /// <param name="stepDistance">
        /// The step distance.
        /// </param>
        /// <param name="totalTime">
        /// The total time.
        /// </param>
        /// <param name="totalDistance">
        /// The total distance.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        public RouteStep(int segementNumber, int stepNumber, string action, string description, string road, int stepTime, int stepDistance, int totalTime, int totalDistance, object line)
        {
            this.SegementNumber = segementNumber;
            this.StepNumber = stepNumber;
            this.Action = action;
            this._description = description;
            this.Road = road;
            this.StepTime = stepTime;
            this.StepDistanceInMeters = stepDistance;
            this.TotalTime = totalTime;
            this.TotalDistance = totalDistance;
            this.Line = line;
        }

        /// <summary>
        /// Gets or sets the sequential step number of a route
        /// </summary>
        public int StepNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the action of this rout step (eg left turn)
        /// </summary>
        public string Action { get; protected set; }

        /// <summary>
        /// Gets the description of this route step (eg left turn on to [Doddington Road]), this is a modified version of the description retrieved from PostcodeAnywhere
        /// </summary>
        public string Description 
        {
            get
            {
                string newDescription;
                if (string.IsNullOrWhiteSpace(this.Road) == false && this._description.Contains(this.Road))
                {
                    Regex regex = new Regex(@"\[.*\]");
                    Match match = regex.Match(this._description);

                    string newRoad = match.Success ? this.Road.Replace(match.Value, match.Value.TrimStart(new[] { '[' }).TrimEnd(new[] { ']' })) : this.Road;

                    newDescription = this._description.Replace(this.Road, "<b>" + newRoad + "</b>");
                }
                else
                {
                    newDescription = this._description;
                }

                return newDescription;
            }

            set
            {
                this._description = value;
            }
        }

        /// <summary>
        /// Gets the formatted total time up to this step in the route in hours and minutes
        /// </summary>
        public string TotalTimeFormatted
        {
            get
            {
                TimeSpan totalTime = TimeSpan.FromSeconds(this.TotalTime);

                string totalTimeFormatted = string.Empty;

                if (totalTime.Hours > 0)
                {
                    totalTimeFormatted = (int)totalTime.TotalHours + "h ";
                }

                if (totalTime.Minutes > 0)
                {
                    totalTimeFormatted += totalTime.Minutes + "m";
                }

                return totalTimeFormatted.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the estimated total distance of this route uptil this route step
        /// </summary>
        public int TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets the line details to be drawn on a map
        /// </summary>
        public object Line { get; protected set; }

        public double StepDistanceInFeet
        {
            get
            {
                return Math.Round(this.StepDistanceInMeters * 3.2808399, 0, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Gets a formatted version of the distance for this RouteStep eg 2.4 mi or 400 ft
        /// </summary>
        public string StepDistanceFormatted
        {
            get
            {
                double distance = this.StepDistanceInMiles;
                if (distance == 0)
                {
                    return "&nbsp;";
                }

                if (distance < 0.1)
                {
                    return this.StepDistanceInFeet + " ft";
                }

                return distance + " mi";
            }
        }

        /// <summary>
        /// Gets the formatted step time in hours and minutes
        /// </summary>
        public string StepTimeFormatted
        {
            get
            {
                TimeSpan stepTime = TimeSpan.FromSeconds(this.StepTime);

                string totalStepTimeFormatted = string.Empty;

                if (stepTime.Hours > 0)
                {
                    totalStepTimeFormatted = (int)stepTime.TotalHours + "h ";
                }

                if (stepTime.Minutes > 0)
                {
                    totalStepTimeFormatted += stepTime.Minutes + "m";
                }

                return totalStepTimeFormatted.Trim();
            }
        }

        /// <summary>
        /// Gets or sets an image related to the Action of thie RouteStep
        /// </summary>
        public string ActionImage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._actionImage))
                {
                    this._actionImage = this.GetActionImageSrc();
                }

                return this._actionImage;
            }

            set
            {
                this._actionImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the estimated total time of this route uptil this route step
        /// </summary>
        private int TotalTime { get; set; }

        /// <summary>
        /// Gets or sets the estimated duration it will take to complete this route step
        /// </summary>
        private int StepTime { get; set; }
        
        /// <summary>
        /// Gets or sets the estimated distance in meters this route step is
        /// </summary>
        private int StepDistanceInMeters { get; set; }

        /// <summary>
        /// Gets the estimated distance in miles this route step is (rounded at two decimal places)
        /// </summary>
        private double StepDistanceInMiles
        {
            get
            {
                return Math.Round(this.StepDistanceInMeters / 1609.344, 2, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Gets or sets the segement of your trip (grouped by waypoints)
        /// </summary>
        private int SegementNumber { get; set; }

        /// <summary>
        /// Gets or sets the road number and or name this route step is on
        /// </summary>
        private string Road { get; set; }

        /// <summary>
        /// Gets an image src for a turn dependant on severity;
        /// </summary>
        /// <param name="severity">The severity of the turn, with 1 being a slight turn, 3 being a right angle, and 6 being a hairpin.</param>
        /// <param name="leftTurn">True if its a left turn, false if its a right turn</param>
        /// <returns>Part of the image src used to represent this turn severity</returns>
        private static string GetTurnSeveritySrc(int severity, bool leftTurn)
        {
            StringBuilder src = new StringBuilder(leftTurn ? "left" : "right");

            switch (severity)
            {
                case 6:
                case 5:
                case 4:
                    src.Append("Hairpin");
                    break;
                case 3:
                    src.Append("RightAngle");
                    break;
                case 2:
                case 1:
                    src.Append("Bear");
                    break;
            }

            src.Append(".png");

            return src.ToString();
        }

        /// <summary>
        /// Gets a path to a relevant icon for the Action property of this RouteStep
        /// </summary>
        /// <returns>A relative path to an image for this RouteStep</returns>
        private string GetActionImageSrc()
        {
            int turnSeverity;
            string src = string.Empty;

            if (this.Action == "D")
            {
                // Depart from start or waypoint.
                src = "start.png";
            }
            else if (this.Action == "A")
            {
                // Arrive at the end or waypoint.
                src = "end.png";
            }
            else if (this.Action == "S")
            {
                // Continue straight on.
                src = "continue.png";
            }
            else if (this.Action == "M")
            {
                // Merge. This is generated when we merge onto another road but we have no choice of ways.
                src = "merge.png";
            }
            else if (this.Action == "F")
            {
                // Ferry crossing.
                src = "ferry.png";
            }
            else if (this.Action == "W")
            {
                // Logistics warning message, when the restriction is acceptable to the given vehicle dimensions. (FreightDirections/FreightDirectionsAndLines only.)
                src = "warning.png";
            }
            else if (this.Action == "CW")
            {
                // Logistics critical warning message, when the restriction is not acceptable to the given vehicle dimensions. (FreightDirections/FreightDirectionsAndLines only.)
                src = "warning.png";
            }
            else if (this.Action.Length == 2 && (this.Action.StartsWith("R") || this.Action.StartsWith("L")) && int.TryParse(this.Action.Substring(1, 1), out turnSeverity))
            {
                src = GetTurnSeveritySrc(turnSeverity, this.Action.StartsWith("L"));
            }
            else if (this.Action.Length == 3 && this.Action.EndsWith("S") && (this.Action.StartsWith("R") || this.Action.StartsWith("L")) && int.TryParse(this.Action.Substring(1, 1), out turnSeverity))
            {
                src = GetTurnSeveritySrc(turnSeverity, this.Action.StartsWith("L"));
            }
            else if (this.Action.Length == 4 && (this.Action.StartsWith("R") || this.Action.StartsWith("L")) && (this.Action.Substring(2, 1) == "L" || this.Action.Substring(2, 1) == "R"))
            {
                src = (this.Action.StartsWith("L") ? "left" : "right") + "Then" + (this.Action.Substring(2, 1) == "L" ? "Left" : "Right") + ".png";
            }
            else if (this.Action.StartsWith("E") && (this.Action.Length == 4 || this.Action.Length == 3))
            {
                src = "roundabout.png";
            }

            if (src == string.Empty)
            {
                src = "unknown.png";
            }

            return src;
        }
    }
}