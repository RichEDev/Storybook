namespace SpendManagementApi.Areas.HelpPage
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;
    using System.Xml.XPath;
    using Microsoft.CSharp;
    using Attributes.Validation;
    using SpendManagementApi.Models.Types;
    using SpendManagementLibrary;

    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from an XML documentation file.
    /// </summary>
    public class XmlDocumentationProvider : IDocumentationProvider
    {
        private readonly XPathNavigator  _documentNavigator;
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertExpression = "/doc/members/member[@name='P:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentationProvider"/> class.
        /// </summary>
        /// <param name="documentPath">The physical path to XML document.</param>
        public XmlDocumentationProvider(string documentPath)
        {
            if (documentPath == null)
            {
                throw new ArgumentNullException("documentPath");
            }
            var xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            XPathNavigator typeNode = GetTypeNode(controllerDescriptor);
            return GetTagValue(typeNode, "summary");
        }

        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "summary");
        }

        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            var reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                XPathNavigator methodNode = GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                if (methodNode != null)
                {
                    string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    XPathNavigator parameterNode = methodNode.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                    if (parameterNode != null)
                    {
                        return parameterNode.Value.Trim();
                    }
                }
            }

            return null;
        }

        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "returns");
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            var reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                return _documentNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = null;

            if (method.DeclaringType != null)
            {
                name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", method.DeclaringType.FullName, method.Name);
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length != 0)
                {
                    string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                    name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
                }
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.SelectSingleNode(tagName);
                if (node != null)
                {
                    return ConvertSeeToAnchor(node.OuterXml);
                }
            }

            return null;
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string typeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(GetTypeName).ToArray();
                return String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", typeName, String.Join(",", argumentTypeNames));
            }

            return type.FullName;
        }

        private XPathNavigator GetTypeNode(HttpControllerDescriptor controllerDescriptor)
        {
            Type controllerType = controllerDescriptor.ControllerType;
            string controllerTypeName = controllerType.FullName;
            if (controllerType.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                controllerTypeName = controllerTypeName.Replace("+", ".");
            }
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
            return _documentNavigator.SelectSingleNode(selectExpression);
        }


        /// <summary>Returns all of the type information for this library.</summary>
        /// <returns>A Type Library object containing.</returns>
        public TypeLibrary GetAllTypeDocumentation(IApiExplorer apiExplorer)
        {
            const string BaseType = "BaseExternalType";
            const string Types = "SpendManagementApi.Models.Types";
            const string Employees = "SpendManagementApi.Models.Types.Employees";
            const string Requests = "SpendManagementApi.Models.Requests";

            var apiTypes = Assembly.GetExecutingAssembly().GetTypes();
            var everything = new TypeLibrary();

            // for each type that is an enum
            var typeList = apiTypes.Where(t => t.IsEnum && t.IsPublic).ToList();
            typeList.Add(typeof(SpendManagementElement));
            typeList.Add(typeof(AccessRoleLevel));
            typeList.Add(typeof(CorporateCardType));
            typeList.Add(typeof(EmailNotificationType));
            typeList.Add(typeof(CustomerType));
            typeList.Add(typeof(FieldType));
            typeList = typeList.OrderBy(t => t.Name).ToList();
            everything.EnumTypes = CreateTypeDocumentation(typeList);

            // for each type that is public, of the namespaces above, not a BaseExternalType, and is not an enum:
            typeList = apiTypes.Where(t => (t.Namespace == Types || t.Namespace == Employees) && t.IsPublic && !t.Name.EndsWith(BaseType) && !t.IsEnum)
                                    .OrderBy(t => t.Name)
                                    .ToList();

            // get reference types
            everything.ReferenceTypes = CreateTypeDocumentation(typeList, everything.EnumTypes.Select(t => t.Value.Type).ToList());


            // for each type that is a request
            typeList = apiTypes.Where(t => (t.Namespace == Requests) && !t.IsEnum).OrderBy(t => t.Name).ToList();
            everything.RequestTypes = CreateTypeDocumentation(typeList, everything.EnumTypes.Select(t => t.Value.Type).ToList());

            return everything;
        }

        /// <summary>
        /// This method creates Type documentaion which inlcudes Atual Type,Documentaion and Documentaion for each member under the type , for the List of Type
        /// </summary>
        /// <param name="typeList">List of different Types to create Type Document it can be Request/Reference/Enum Types</param>
        /// <param name="enumTypes">List of Enum Types</param>
        /// <returns><see cref="TypeDocumentation"/>List of Type Documents for the List of Type</returns>
        private List<KeyValuePair<string, TypeDocumentation>> CreateTypeDocumentation(List<Type> typeList, List<Type> enumTypes = null)
        {
            var typeDocs = new List<KeyValuePair< string, TypeDocumentation>>();
            
            using (var codeProvider = new CSharpCodeProvider())
            {
                typeList.ForEach(t =>
                {
                   
                    var doc = GetTypeNode(t);
                    var mem = GetTypeProperties(t, codeProvider, enumTypes);

                    var typdoc = new TypeDocumentation
                    {
                        Type = t,
                        Documentation = doc,
                        MemberDocumentation = mem
                    };
                    var kvp = new KeyValuePair<string, TypeDocumentation>(t.Name, typdoc);
                    typeDocs.Add(kvp);
                });
            }
            return typeDocs;
        }


        private string GetTypeNode(Type type, string properTypeName = null)
        {
            var name = properTypeName ?? GetProperTypeName(type);
            var selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, name);
            var navigator = _documentNavigator.SelectSingleNode(selectExpression);
            return navigator != null ? RemoveNamespaces(ConvertSeeToAnchor(navigator.InnerXml), false) : null;
        }

        private Dictionary<string, MemberDocumentation> GetTypeProperties(Type type, CSharpCodeProvider codeProvider, List<Type> existingEnumsTypes = null)
        {
            var output = new Dictionary<string, MemberDocumentation>();
            var properties = type.GetProperties().Where(x => x.PropertyType.IsPublic && x.DeclaringType != typeof(BaseExternalType)).ToList();

            properties.ForEach(prop =>
            {
                if (prop.DeclaringType != null)
                {
                    var tName = prop.DeclaringType.FullName;
                    var name = string.Format(CultureInfo.InvariantCulture, PropertExpression, tName + "." + prop.Name);
                    var node = this._documentNavigator.SelectSingleNode(name);
                    var docs = new MemberDocumentation { Rules = new List<string>() };
                    var typeRef = new CodeTypeReference(prop.PropertyType);
                    var typeFriendlyName = codeProvider.GetTypeOutput(typeRef);

                    var simplified = RemoveNamespaces(typeFriendlyName, false);
                    if (simplified.IndexOf(".", StringComparison.Ordinal) > -1)
                    {
                        simplified = simplified.Substring(simplified.IndexOf(".", StringComparison.Ordinal) + 1);
                    }

                    if (prop.PropertyType.FullName.Contains("SpendManagementApi") || (existingEnumsTypes!= null && existingEnumsTypes.Contains(prop.PropertyType)))
                    {
                        var nonWrappedType = simplified.Contains("<") ? RemoveNamespaces(typeFriendlyName) : simplified;
                        simplified = simplified.Replace("<", "&lt;").Replace(">", "&gt;");
                        docs.Type = "<a class=\"help-page-api-element-link\" href=\"/Help/TypeInformation#" + nonWrappedType + "\" title=\"Go to " + nonWrappedType + "\">" + simplified + "</a>";
                    }
                    else
                    {
                        docs.Type = simplified.Replace("<", "&lt;").Replace(">", "&gt;");
                    }

                    // pull out the comments from the file
                    if (node != null && node.HasChildren)
                    {
                        if (node.MoveToChild("summary", ""))
                        {
                            docs.Summary = RemoveNamespaces(ConvertSeeToAnchor(node.InnerXml), false);

                            if (node.MoveToNext("returns", ""))
                            {
                                docs.Return = RemoveNamespaces(ConvertSeeToAnchor(node.InnerXml, "return"), false);
                            }
                        }

                        if (output.ContainsKey(prop.Name))
                        {
                            output[prop.Name] = docs;
                        }
                        else
                        {
                            output.Add(prop.Name, docs);
                        }

                    }

                    // create the rules
                    var customAttributes = prop.GetCustomAttributes(true);
                    foreach (var attribute in customAttributes)
                    {
                        if (attribute is RequiredAttribute)
                        {
                            var requiredIf = attribute as RequiredIfAttribute;
                            if (requiredIf != null)
                            {
                                docs.Rules.Add("Required if " + requiredIf.PropertyName + " is populated.");
                            }
                            else docs.Rules.Add("Required");
                        }

                        var lengthAttribute = attribute as MaxLengthAttribute;
                        if (lengthAttribute != null)
                        {
                            docs.Rules.Add("Max Length: " + lengthAttribute.Length);
                        }

                        var valueAttribute = attribute as IsSpendManagementValueAttribute;
                        if (valueAttribute != null)
                        {
                            docs.Rules.Add("Must be a SpendManagementElement");
                        }

                        if (attribute is ValidEnumValueAttribute)
                        {
                            docs.Rules.Add("Must be a valid enum member");
                        }
                    }

                    if (prop.GetSetMethod(true) != null && !prop.GetSetMethod(true).IsPublic)
                    {
                        docs.Rules.Add("ReadOnly");
                    }
                }
            });

            return output;
        }

        /// <summary>
        /// Removes the namespaces from a type name. If the type is wrapped with IEnumerable, IList, List or Nullable then these can be removed too.
        /// </summary>
        /// <param name="input">The full type name.</param>
        /// <param name="includeWrappers">Whether to strip out wrappers.</param>
        /// <returns>The modified string.</returns>
        public static string RemoveNamespaces(string input, bool includeWrappers = true)
        {
            var result = input.Replace("System.Collections.Generic.", string.Empty)
                                .Replace("System.", string.Empty)
                                .Replace("`1", string.Empty)
                                .Replace("SpendManagementLibrary.", string.Empty)
                                .Replace("SpendManagementApi.Models.Types.Employees.", string.Empty)
                                .Replace("SpendManagementApi.Models.Types.", string.Empty)
                                .Replace("SpendManagementApi.Common.Enums.", string.Empty);

            if (includeWrappers)
            {
                result = result.Replace("IEnumerable<", string.Empty)
                                .Replace("IList<", string.Empty)
                                .Replace("List<", string.Empty)
                                .Replace(">", string.Empty)
                                .Replace("System.Nullable<", string.Empty);
            }

            return result;
        }

        private string GetProperTypeName(Type type)
        {
            return type.IsNested ? type.FullName.Replace("+", ".") : type.FullName;
        }

        /// <summary>
        /// Converts the &lt;see cref=  to an &lt;a href= for the given section of some documentation (summary / return nodes).
        /// </summary>
        /// <param name="input">The string to put in.</param>
        /// <param name="outerTag">The outer name, which will be removed</param>
        /// <returns></returns>
        public static string ConvertSeeToAnchor(string input, string outerTag = "summary")
        {
            return input.Replace("<" + outerTag + ">", "")
                .Replace("</" + outerTag + ">", "")
                .Replace("<see cref=\"P:", "<a class=\"help-page-api-element-link\" href=\"/Help/TypeInformation#")
                .Replace("<see cref=\"T:", "<a class=\"help-page-api-element-link\" href=\"/Help/TypeInformation#")
                .Replace("</see>", "</a>")
                .Trim();
        }
    }
}
