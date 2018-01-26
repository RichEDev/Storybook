namespace SpendManagementHelpers.TreeControl
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// Shared methods
    /// </summary>
    internal class SharedTreeMethods
    {
        public static string GenerateCSS()
        {
            return new StringBuilder(@"
                    <style type=").Append("\"text/css\">").Append(@"
                        /* overwrite default jsTree styles to work with modalpopup */
                        #vakata-dragged { z-index: 10007 !important; }
                        #vakata-dragged.jstree-default ins { background:transparent !important; }
                        #vakata-dragged.jstree-default .jstree-ok { background:url('') !important; }
                        #vakata-dragged.jstree-default .jstree-invalid { background:url('') !important; }
                        #jstree-marker.jstree-default { z-index: 10006 !important; }
                        #jstree-marker-line.jstree-default { z-index: 10005 !important; }

                        li.tree-node-disabled { display: none; }

                        div.treemenu, div.treemenuleft { background-color: #19A2E6; border: 1px solid #19A2E6 ; border-bottom-width: 0; padding: 1px 5px; font-size: 2px; }
                        div.treemenuLeft { text-align: right; }
                        div.treediv { overflow: auto; border: 1px solid #999999; text-align: center; }
                        div.treediv ul { text-align: left; }

                        div.treediv-filters li, div.treediv-filters li a { height: 30px; }
                        div.treediv-filters li a span.nodeText,
                        div.treediv-filters li a span.filterInfo,
                        div.treediv-filters li a span.criteria,
                        div.treediv-filters li a span.editImage
                        {
                            display: inline-block;
                            width: 120px;
                            border: 1px solid transparent;
                            border-right-color: Grey;   
                            border-bottom-color: Grey;
                            border-left-width: 0px;  
                            text-align: center;
                            line-height: 28px;
                            height: 28px;
                            vertical-align: middle;
                            overflow: hidden;
                            font-size: 8pt;  
                        }

                        div.treediv-filters li a span.nodeText { width: 145px; }

                        div.treediv-filters li a span.filterInfo { width: 155px; }

                        div.treediv-filters li a span.criteria { width: 250px; }

                        div.treediv-filters li a span.editImage { width: 25px; }

                        div.treediv-filters li a span.editImage img { margin-top: 6px; }

                        div.treediv-filters ins { display: none !important; }

                        div.treediv-filters li, div.treediv-filters li a
                        {
                            height: 30px;
                            vertical-align: middle;
                            overflow: hidden;
                            padding: 0;
                            margin: 0;
                        }

                        div.treediv-filters.jstree-default .jstree-clicked, div.treediv-filters li a
                        {                            
                            border: 0px solid transparent;
                            padding: 0;
                            margin: 0;
                        }
                        div.treediv-filters li a.jstree-clicked span.nodeText,
                        div.treediv-filters li a.jstree-clicked span.filterInfo,
                        div.treediv-filters li a.jstree-clicked span.criteria,
                        div.treediv-filters li a.jstree-clicked span.editImage
                        {       
                            border-top-color: #000000;
                            border-bottom-color: #000000;
                            font-weight: bold;
                        }

                        div.treediv-custom li, div.treediv-custom li a { height: 30px; }
                        div.treediv-custom li a span.nodeText,
                        div.treediv-custom li a span.filterInfo,
                        div.treediv-custom li a span.criteria,
                        div.treediv-custom li a span.editImage
                        {
                            display: inline-block;
                            width: 120px;
                            border: 1px solid transparent;
                            border-right-color: Grey;   
                            border-bottom-color: Grey;
                            border-left-width: 0px;  
                            text-align: center;
                            line-height: 28px;
                            height: 28px;
                            vertical-align: middle;
                            overflow: hidden;
                            font-size: 8pt;  
                        }
                        div.treediv-custom li a span.nodeText { width: 145px; }

                        div.treediv-custom li a span.filterInfo { width: 155px; }

                        div.treediv-custom li a span.criteria { width: 250px; }

                        div.treediv-custom li a span.editImage { width: 25px; }

                        div.treediv-custom li a span.editImage img { margin-top: 6px; }

                        div.treediv-custom ins { display: none !important; }

                        div.treediv-custom li, div.treediv-custom li a
                        {
                            height: 30px;
                            vertical-align: middle;
                            
                            padding: 0;
                            margin: 0;
                            width: 1479px;
                            display: inline-block;
                            
                        }

                        div.treediv-custom.jstree-default .jstree-clicked, div.treediv-custom li a
                        {                            
                            border: 0px solid transparent;
                            padding: 0;
                            margin: 0;
                        }
                        div.treediv-custom li a.jstree-clicked span.nodeText,
                        div.treediv-custom li a.jstree-clicked span.filterInfo,
                        div.treediv-custom li a.jstree-clicked span.criteria,
                        div.treediv-custom li a.jstree-clicked span.editImage
                        {       
                            border-top-color: #000000;
                            border-bottom-color: #000000;
                            font-weight: bold;
                        }

                        .jstree-default a .jstree-icon { background-position:0px 0px; }                      
                    </style>").ToString();
        }

        public static string GenerateJavaScript(string themePath)
        {
            return new StringBuilder(@"$.jstree._themes = '").Append(themePath).Append(@"';").ToString();
        }

        public static void TryAddWebResources(ref ScriptManager parentSM)
        {
            if (parentSM == null)
            {
                // it may make sense to decouple this from the sel.X.js structure and have one seperately for the Helpers
                // if the library grows to the point where we'd want to use it elsewhere
                throw new InvalidOperationException("SpendManagementHelpers.Tree requires a ScriptManager on the page and for sel.main.js to have been registered");

                //string treeJS = Page.ClientScript.GetWebResourceUrl(typeof(Tree), "SpendManagementHelpers.Tree.sel.trees.js");
                //Page.ClientScript.RegisterClientScriptInclude("sel.trees.js", treeJS);
            }
            else
            {
                // when finished, would be good to combine and minify these, unless we can get the ScriptManager set up to do so
                // .Net 3.5/4 may have introduced a <CombinedScripts>
                //parentSM.Scripts.Add(new ScriptReference("TreeDrop/jquery.jstree.js", "SpendManagementHelpers")); // this is a very slightly modified version of jsTree for the "unique" functionality being done by ID rather than Title
                //parentSM.Scripts.Add(new ScriptReference("TreeDrop/sel.trees.js", "SpendManagementHelpers"));

                var selTrees = new ScriptReference(Path + "/shared/javascript/minify/sel.trees.js");
                var filterDialogPlugin = new ScriptReference(Path + "/shared/javascript/minify/sel.filterDialog.js?date=20180126");
                var ajaxService = new ScriptReference(Path + "/shared/javascript/sel.ajax.js");
                var jQueryTree = new ScriptReference(Path + "/shared/javascript/minify/jquery.jstree.js");
                parentSM.Scripts.Add(jQueryTree);
                parentSM.Scripts.Add(selTrees);
                parentSM.Scripts.Add(filterDialogPlugin);
                parentSM.Scripts.Add(ajaxService);
            }
        }

        public static string Path
        {
            get
            {
                return HttpRuntime.AppDomainAppVirtualPath == "/" ? string.Empty : HttpRuntime.AppDomainAppVirtualPath;
            }
        }
    }
}