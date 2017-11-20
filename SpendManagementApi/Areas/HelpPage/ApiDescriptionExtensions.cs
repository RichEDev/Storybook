namespace SpendManagementApi.Areas.HelpPage
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Http.Description;
    using System.Linq;
    using System.Reflection;

    using SpendManagementApi.Attributes;

    /// <summary>
    /// Extension objects for the ApiDescription object.
    /// </summary>
    public static class ApiDescriptionExtensions
    {
        /// <summary>
        /// Generates an URI-friendly ID for the <see cref="ApiDescription"/>. E.g. "Get-Values-id_name" instead of "GetValues/{id}?name={name}"
        /// </summary>
        /// <param name="description">The <see cref="ApiDescription"/>.</param>
        /// <returns>The ID as a string.</returns>
        public static string GetFriendlyId(this ApiDescription description)
        {
            string path = description.RelativePath;
            string[] urlParts = path.Split('?');
            string localPath = urlParts[0];
            string queryKeyString = null;
            if (urlParts.Length > 1)
            {
                string query = urlParts[1];
                string[] queryKeys = HttpUtility.ParseQueryString(query).AllKeys;
                queryKeyString = String.Join("_", queryKeys);
            }

            var friendlyPath = new StringBuilder();
            friendlyPath.AppendFormat("{0}-{1}",
                description.HttpMethod.Method,
                localPath.Replace("/", "-").Replace("{", String.Empty).Replace("}", String.Empty));
            if (queryKeyString != null)
            {
                friendlyPath.AppendFormat("_{0}", queryKeyString);
            }
            return friendlyPath.ToString();
        }

        /// <summary>
        /// Gets the version number of the controller from the ApiDescription object.
        /// </summary>
        /// <param name="description">The ApiDescription object.</param>
        /// <returns>The version number as a string.</returns>
        public static int GetVersionNumber(this ApiDescription description)
        {
            int versionNumber = 0;
            var attributes = (description.ActionDescriptor).ControllerDescriptor.ControllerType.CustomAttributes;

            CustomAttributeData versionAttribute = attributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(VersionAttribute));

            if (versionAttribute != null)
            {
                versionNumber = Convert.ToInt32(versionAttribute.ConstructorArguments[0].Value);
            }

            return versionNumber;
        }
    }
}