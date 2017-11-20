using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
   public class cCustomMenuElement
    {

        private int nCustomMenuid;
        private string sCustomMenuName;
        private int? nCustomParentid;
        private string sDescription;
        private bool? bIsCustom;
       private string sMenuIcon;
        private cCustomMenuElement miParent;
        private string sImageUrl = "/static/icons/48/plain/";

        private List<cCustomMenuElement> lstChildren = new List<cCustomMenuElement>();

       /// <summary>
       /// Initialises a new instance of the <see cref="cCustomMenuElement"/> class.
       /// </summary>
       /// <param name="menuid">
       /// The menuid.
       /// </param>
       /// <param name="menuname">
       /// The menuname.
       /// </param>
       /// <param name="parentid">
       /// The parentid.
       /// </param>
       /// <param name="description">
       /// The description.
       /// </param>
       /// <param name="menuicon">
       /// The menuicon.
       /// </param>
       /// <param name="iscustom">
       /// The iscustom.
       /// </param>
       public cCustomMenuElement(int menuid, string menuname, int? parentid,string description, string menuicon, bool? iscustom)
        {
            nCustomMenuid = menuid;
            sCustomMenuName = menuname;
            nCustomParentid = parentid;
            sDescription = description;
            bIsCustom = iscustom;
            sMenuIcon = menuicon;
            sImageUrl = sImageUrl + menuicon;

        }

       /// <summary>
       /// The set parent.
       /// </summary>
       /// <param name="parent">
       /// The parent.
       /// </param>
       public void setParent(cCustomMenuElement parent)
        {
            miParent = parent;
        }

       /// <summary>
       /// The add child.
       /// </summary>
       /// <param name="child">
       /// The child.
       /// </param>
       public void addChild(cCustomMenuElement child)
        {
            lstChildren.Add(child);
        }

        #region properties
        public int menuid
        {
            get { return nCustomMenuid; }
        }
        public string menuname
        {
            get { return sCustomMenuName; }
        }
        public cCustomMenuElement parent
        {
            get { return miParent; }
        }
        public int? parentid
        {
            get { return nCustomParentid; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public string menuicon
        {
            get { return sMenuIcon; }
        }
        public string imageurl
        {
            get { return sImageUrl; }
        }
        public bool? iscustom
        {
            get { return bIsCustom; }
        }
        public List<cCustomMenuElement> children
        {
            get { return lstChildren; }
        }
        #endregion
    }
}
