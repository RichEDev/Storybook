namespace SpendManagementApi.Areas.HelpPage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;
    using System.Web.Http.Routing;

    /// <summary>
    /// 
    /// </summary>
    public class SelApiExplorer : ApiExplorer
    {
        private readonly string _version;

        /// <summary>
        /// An implementation of the API Explorer that allows for our versioning.
        /// </summary>
        /// <param name="version"></param>
        public SelApiExplorer(string version)
            : base(GlobalConfiguration.Configuration)
        {
            if (this.VersionNumbers == null)
            {
                this.VersionNumbers = new List<int>();
            }

            this._version = version != null ? "V" + version.ToUpperInvariant() : "V1";
        }


        /// <summary>
        /// A list of version numbers that have controllers coded against them.
        /// </summary>
        public List<int> VersionNumbers { get; private set; } 

        /// <summary>
        /// Overrides the base function by allowing the different versions to be checked
        /// </summary>
        /// <returns>If the controller descriptor should be parsed,</returns>
        public override bool ShouldExploreController(
            string controllerVariableValue,
            HttpControllerDescriptor controllerDescriptor,
            IHttpRoute route)
        {
            int versionNumber = 0;
            var regEx = new Regex(@"^.+\.V(\d+)\..+$");

            if (regEx.IsMatch(controllerDescriptor.ControllerType.FullName))
            {
                versionNumber = int.Parse(regEx.Match(controllerDescriptor.ControllerType.FullName).Groups[1].Value);
            }

            if (versionNumber != 0 && !this.VersionNumbers.Contains(versionNumber))
            {
                this.VersionNumbers.Add(versionNumber);
            }

            return controllerDescriptor.ControllerType.FullName.Contains(string.Format(".{0}.", this._version)) && base.ShouldExploreController(controllerVariableValue, controllerDescriptor, route);
        }
    }
}