namespace SpendManagementLibrary.Expedite.DTO
{
    using System.Collections.Generic;
    using System.Linq;

    public class TreeDTO
    {
        public List<EnvelopeManagementDTONode> Children { get; set; }
    }

    public class IdDTONode
    {
        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<IdDTONode> children { get; set; }

        /// <summary>
        /// The Id of the parent, or 0
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// Whether the node is a folder / branch
        /// </summary>
        public bool isFolder { get; set; }
    }

    public static class IdDTONodeExtensions
    {
        /// <summary>
        /// Gets all descendants
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<IdDTONode> Descendants(this IdDTONode root)
        {
            var nodes = new Stack<IdDTONode>(new[] { root });
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in node.children) nodes.Push(n);
            }
        }
    }

    public class EnvelopeManagementDTONode
    {
        public EnvelopeManagementDTONode() { }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<EnvelopeManagementDTONode> children { get; set; }

        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Whether or not a particualr node is active. Only one node should be active and it will be highlighted in blue
        /// </summary>
        public bool isActive { get; set; }

        /// <summary>
        /// Allows a node to link to a different web page
        /// </summary>
        public string href { get; set; }

        /// <summary>
        /// Determines how a node should open a link, possible values are '_blank', '_self', '_parent' and '_top'. The default is '_self'.
        /// </summary>
        public string hrefTarget { get; set; }

        /// <summary>
        /// If a node is lazy it will call the url defined here in order to populate its children. The url should return an array of nodes and can be populated either when the tree is first initialised or in the 'openLazyNode' event
        /// </summary>
        public string lazyUrl { get; set; }

        /// <summary>
        /// When lazyUrl is called anything defined in lazyUrlJson will be passed to the server in the same request
        /// </summary>
        public string lazyUrlJson { get; set; }

        /// <summary>
        /// Any classes passed in here will span the underlying li element when the tree is rendered
        /// </summary>
        public string liClass { get; set; }

        /// <summary>
        /// Any classes passed in here will span the text element when the tree is rendered
        /// </summary>
        public string textCss { get; set; }

        /// <summary>
        /// Any text entered here will appear as a tooltip when the node is hovered over
        /// </summary>
        public string tooltip { get; set; }

        /// <summary>
        /// Any jqueryUI icon class for example 'ui-icon-scissors'. For this to work please make sure jqueryUI has been imported, more jqueryUI icons can be found here
        /// </summary>
        public string uiIcon { get; set; }

        /// <summary>
        /// Whether or not a node is open
        /// </summary>
        public bool isExpanded { get; set; }

        /// <summary>
        /// If a node is lazy it will fire the 'openLazyNode' event when opened and then call the url defined in lazyUrl and populate its children with the response
        /// </summary>
        public virtual bool isLazy { get; set; }

        /// <summary>
        /// Whether or not a node is a folder. By default folders have a different icon and they are the only type of node that can be dropped to
        /// </summary>
        public virtual bool isFolder { get; set; }

        /// <summary>
        /// Passing in a url to an image here will override the nodes default icon
        /// </summary>
        public virtual string iconUrl { get; set; }
    }

    public class BranchDTO : EnvelopeManagementDTONode
    {
        public override bool isLazy { get { return true; } }

        public override bool isFolder { get { return true; } }
    }

    public class LeafDTO : EnvelopeManagementDTONode
    {
        public override bool isLazy { get { return false; } }

        public override bool isFolder { get { return false; } }
    }

    public class EnvelopeManagementEnvelope : BranchDTO
    {
        public EnvelopeManagementEnvelope()
        {
        }

        public EnvelopeManagementEnvelope(Envelope envelope)
        {
            id = envelope.EnvelopeId.ToString();
            text = envelope.EnvelopeNumber;
            tooltip = "Envelope " + text;
        }

        public int? accountId { get; set; }

        public int? userId { get; set; }

        public int? claimId { get; set; }

        public override string iconUrl { get { return GlobalVariables.StaticContentLibrary + "/icons/16/plain/airmail.png"; } }
    }

    public class EnvelopeManagementReceipt : LeafDTO
    {
        public EnvelopeManagementReceipt()
        {
        }

        public EnvelopeManagementReceipt(Receipt receipt)
        {
            id = receipt.ReceiptId.ToString();
            text = receipt.ReceiptId + "." + receipt.Extension;
            tooltip = "Receipt " + text;
        }

        public string tempUrl { get; set; }

        public override string iconUrl { get { return GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/scroll.png"; } }
    }

    
    public class EnvelopeManagementClaim : BranchDTO
    {
        public EnvelopeManagementClaim()
        {
        }

        public EnvelopeManagementClaim(cClaim claim)
        {
            id = "claim_" + claim.claimid;
            text = string.Format("{0}, {1} item{2}, total: {3}, date: {4}",
                //claim.ReferenceNumber == null ? claim.name : claim.name + " (" + claim.ReferenceNumber + ")",
                claim.name,
                claim.NumberOfItems, claim.NumberOfItems == 1 ? string.Empty : "s", claim.Total.ToString("C"),
                claim.datesubmitted.ToShortDateString());
            children = new List<EnvelopeManagementDTONode>();
        }

        public override string iconUrl { get { return GlobalVariables.StaticContentLibrary + "/icons/16/plain/folder2.png"; } }
    }
}
