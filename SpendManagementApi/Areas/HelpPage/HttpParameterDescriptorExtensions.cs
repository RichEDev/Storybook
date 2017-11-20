using System;
using System.Text;
using System.Web;
using System.Web.Http.Description;

namespace SpendManagementApi.Areas.HelpPage
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Controllers;

    public static class HttpParameterDescriptorExtensions
    {
        /// <summary>
        /// Generates type information for the HttpParameterDescriptor
        /// </summary>
        /// <param name="parameter">The <see cref="HttpParameterDescriptor"/>.</param>
        /// <returns>The ID as a string.</returns>
        public static string GetTypeInformation(this HttpParameterDescriptor parameter)
        {
            StringBuilder sb = new StringBuilder();
            GetType(parameter.ParameterType, sb);
            return sb.ToString();
        }

        private static void GetType(Type type, StringBuilder sb)
        {
            List<PropertyInfo> properties = type.GetProperties().ToList();//.Where(p => p.IsDefined(typeof(RequiredAttribute), true)).ToList();
            properties.ToList().ForEach(p =>
            {
                Type t = p.PropertyType;
                if (!t.IsPrimitive && !t.IsEnum & !(t.Namespace??string.Empty).StartsWith("System"))
                {
                    GetType(t, sb);
                }
                else
                {
                    sb.AppendLine(string.Format("{0} - {1}.{2}\r\n", p.Name, t.Namespace, t.Name));
                }
            });
        }
    }
}