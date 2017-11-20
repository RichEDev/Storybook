using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cMenuElement
    {
        private int nMenuid;
        private string sMenuName;
        private int? nParentid;
        private bool bIsCustom;
        private cMenuElement miParent;
        private List<cMenuElement> lstChildren = new List<cMenuElement>();

        public cMenuElement(int menuid, string menuname, int? parentid, bool iscustom)
        {
            nMenuid = menuid;
            sMenuName = menuname;
            nParentid = parentid;
            bIsCustom = iscustom;
        }

        public void setParent(cMenuElement parent)
        {
            miParent = parent;
        }
        public void addChild(cMenuElement child)
        {
            lstChildren.Add(child);
        }
        #region properties
        public int menuid
        {
            get { return nMenuid; }
        }
        public string menuname
        {
            get { return sMenuName; }
        }
        public cMenuElement parent
        {
            get { return miParent; }
        }
        public int? parentid
        {
            get { return nParentid; }
        }
        public bool iscustom
        {
            get { return bIsCustom; }
        }
        public List<cMenuElement> children
        {
            get { return lstChildren; }
        }
        #endregion
    }
}
