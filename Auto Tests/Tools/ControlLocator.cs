using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;


namespace Auto_Tests.Tools
{
    /// <summary>
    /// Locates HtmlControl with provided Id and rturns
    /// instance to caller.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ControlLocator<T> where T : HtmlControl 
    {
        public T findControl(string id, T t)
        {
            t.SearchProperties.Clear();
            t.SearchProperties["Id"] = id;
            t.Find();
            return t;
        }
    }

    public class UIControlLocator<T> where T : UITestControl
    {
        public T findControl(string id, T t, string innerText = null)
        {
            t.SearchProperties.Clear();
            if(!string.IsNullOrEmpty(id))
            {
                t.SearchProperties["Id"] = id;
            }
            if (!string.IsNullOrEmpty(innerText))
            {
                t.SearchProperties["InnerText"] = innerText;
            }
            t.Find();

            return t;
        }
    }

    public class TreeControlLocator<T> where T : HtmlControl
    {
        public T findControl(string id, T t)
        {
            t.SearchProperties.Clear();
            t.SearchProperties["fieldid"] = id;
            t.Find();
            return t;
        }
    }

    public class SectionControlLocator<T> where T : HtmlControl
    {
        public T findControl(string classname, string innertext, T t)
        {
            t.SearchProperties.Clear();
            t.SearchProperties["InnerText"] = innertext;
            t.SearchProperties["Class"] = classname;
            t.Find();
            return t;
        }
    }

    public class TabControlLocator<T> where T : HtmlControl
    {
        public T findControl(string classname, string innertext, T t)
        {
            t.SearchProperties.Clear();
            t.SearchProperties["InnerText"] = innertext;
            t.SearchProperties["Class"] = classname;
            t.Find();
            return t;
        }
    }

    public class TreeNodeLocator
    {
        /// <summary>
        /// Returns a new TreeNode based on the inner text provided
        /// </summary>
        /// <param name="innerText"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public UITestControl FindControl(UITestControl searchSpaceLimit, string innerText)
        {
            UITestControl TreeNode = new UITestControl(searchSpaceLimit);
            //k5dc9fdda-36a5-4965-bb0f-48588daf09c2_ndd45bd97-8cbb-43d2-92cc-4a1361d35885
            TreeNode.FilterProperties["Id"] = "k92013d68-a5fc-424a-939a-0af22c49fdaf_ndd45bd97-8cbb-43d2-92cc-4a1361d35885";
            //TreeNode.FilterProperties["InnerText"] = innerText;
            //TreeNode.FilterProperties["Class"] = "jstree-leaf";
            TreeNode.Find();
            return TreeNode;
        }
    }
}
     