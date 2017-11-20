using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HtmlAgilityPack;

namespace Utilities.StringManipulation
{
    using System.Text.RegularExpressions;

    public sealed class HtmlUtility
    {
        private static volatile HtmlUtility _instance;
        private static readonly object Root = new object();

        private HtmlUtility() { }

        public static HtmlUtility Instance
        {
            get
            {
                if (_instance == null)
                    lock (Root)
                        if (_instance == null)
                            _instance = new HtmlUtility();

                return _instance;
            }
        }

        /// <summary>
        /// Attempt to fix HTML with missing or non-matching <P></P> tags
        /// This will;
        /// Remove all ending p tags
        /// replace all p tags with end p and start p
        /// remove the first end p
        /// add an end p to the end of the string.
        /// </summary>
        /// <param name="htmlData"></param>
        /// <returns></returns>
        public string FixPTags(string htmlData)
        {
            var sb = new StringBuilder(htmlData);
            sb.Replace("</p>", string.Empty);
            sb.Replace("</P", string.Empty);
            sb.Replace("<p>", "</p><p>");
            sb.Replace("<P", "</p><p>");
            sb.Append("</p>");
            var result = sb.ToString();
            var idx = result.IndexOf("</p>");
            if (idx > -1)
            {
                result = result.Remove(idx, 4);
            }

            return result;
        }

   
         /// <summary>
         /// Takes raw HTML input and cleans against a whitelist
         /// </summary>
         /// <param name="source">Html source</param>
         /// <returns>Clean output</returns>
        public string SanitizeHtml(string source, string htmlWhiteListTemplatePath)
        {
            HtmlDocument html = GetHtml(source);
            if (html == null) return String.Empty;
            //Forces empty nodes to become closed. For example, <br> becomes <br/>
            html.OptionWriteEmptyNodes = true;
            // All the nodes
            HtmlNode allNodes = html.DocumentNode;
            Dictionary<string, string[]> htmlWhiteList = GenerateHtmlWhiteList(htmlWhiteListTemplatePath);

            // Select whitelist tag names
            string[] whitelist = (from kv in htmlWhiteList
                                  select kv.Key).ToArray();

            // Scrub tags not in whitelist
            CleanNodes(allNodes, whitelist);

            // Filter the attributes of the remaining
            foreach (KeyValuePair<string, string[]> tag in htmlWhiteList)
            {
                IEnumerable<HtmlNode> nodes = (from n in allNodes.DescendantsAndSelf()
                                               where n.Name == tag.Key
                                               select n);

                // No nodes? Skip.
                if (nodes == null) continue;

                foreach (var n in nodes)
                {
                    // No attributes? Skip.
                    if (!n.HasAttributes) continue;

                    // Get all the allowed attributes for this tag
                    HtmlAttribute[] attr = n.Attributes.ToArray();
                    foreach (HtmlAttribute a in attr)
                    {
                        if (!tag.Value.Contains(a.Name))
                        {
                            a.Remove(); // Attribute wasn't in the whitelist
                        }
                        else
                        {
                            if (a.Name == "href" || a.Name == "src")
                            {
                                a.Value = (!string.IsNullOrEmpty(a.Value)) ? a.Value.Replace("\r", "").Replace("\n", "") : "";
                                a.Value =
                                    (!string.IsNullOrEmpty(a.Value) &&
                                    (a.Value.IndexOf("javascript") < 10 || a.Value.IndexOf("eval") < 10)) ?
                                    a.Value.Replace("javascript", "").Replace("eval", "") : a.Value;
                            }
                            else
                            {
                                a.Value =
                                    Microsoft.Security.Application.Encoder.HtmlAttributeEncode(a.Value);
                            }
                        }
                    }
                }
            }
           
            return allNodes.InnerHtml;
        }

        /// <summary>
        /// Removes html tags from a string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The cleaned up <see cref="string"/>
        /// </returns>
        public string RemoveHTMLTagsFromString(string source)
        {
            Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
            return htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Reads the whitelist of HTML tags/attributes from the XML template. 
        /// Generates a dictionary of acceptable HTML characters and their attributes
        /// </summary>
        /// <param name="htmlWhiteListTemplatePath">The path to the XML template document</param>
        /// <returns>A Dictionary of HTML tags and their attbutes</returns>
        private static Dictionary<string, string[]> GenerateHtmlWhiteList(string htmlWhiteListTemplatePath)
        {
            var document = new XmlDocument();
            document.Load(htmlWhiteListTemplatePath);

            var htmlWhiteList = new Dictionary<string, string[]>();
            var xmlNodeList = document.SelectNodes("/template/HTMLTag");

            if (xmlNodeList != null)

                foreach (XmlNode childNode in xmlNodeList)
                {
                    var selectSingleNode = childNode.SelectSingleNode("KeyTag");

                    if (selectSingleNode != null)
                    {
                        string instanceName = selectSingleNode.InnerText;
                        var values = new List<string>();
                        var selectNodes = childNode.SelectNodes("Attributes/Attribute");

                        if (selectNodes != null)

                            foreach (XmlNode value in selectNodes)
                            {
                                values.Add(value.InnerText);
                            }

                        htmlWhiteList.Add(instanceName, values.ToArray());
                    }
                }

            return htmlWhiteList;
        }

        /// <summary>
        /// Recursively delete nodes not in the whitelist
        /// </summary>
        private static void CleanNodes(HtmlNode node, string[] whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whitelist.Contains(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                    return; // We're done
                }
            }

            if (node.HasChildNodes)
                CleanChildren(node, whitelist);
        }

        /// <summary>
        /// Apply CleanNodes to each of the child nodes
        /// </summary>
        private static void CleanChildren(HtmlNode parent, string[] whitelist)
        {
            for (int i = parent.ChildNodes.Count - 1; i >= 0; i--)
                CleanNodes(parent.ChildNodes[i], whitelist);
        }

        /// <summary>
        /// Helper function that returns an HTML document from text
        /// </summary>
        private static HtmlDocument GetHtml(string source)
        {
            var html = new HtmlDocument();
            html.OptionFixNestedTags = true;
            html.OptionAutoCloseOnEnd = true;
            html.OptionDefaultStreamEncoding = Encoding.UTF8;
            html.LoadHtml(source);

            // Encode any code blocks independently so they won't
            // be stripped out completely when we do a final cleanup
            foreach (var n in html.DocumentNode.DescendantNodesAndSelf())
            {
                if (n.Name == "code")
                {
                    HtmlAttribute[] attr = n.Attributes.ToArray();

                    foreach (HtmlAttribute a in attr)
                    {
                        if (a.Name != "style" && a.Name != "class") { a.Remove(); }
                    }

                    n.InnerHtml =
                        Microsoft.Security.Application.Encoder.HtmlEncode(n.InnerHtml);
                }
            }

            return html;
        }
    }
}


