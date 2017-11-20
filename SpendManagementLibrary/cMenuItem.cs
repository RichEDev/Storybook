using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Individual menu options on a master page, inherits System.Web.UI.Control to give access to ResolveUrl()
    /// </summary>
    public class cMenuItem : System.Web.UI.Control
    {
        private string sLogo;
        private string sLabel;
        private string sDescription;
        private string sURL;
        private int nLogoSize;
        private string sTarget;
        private bool bNewWindow;
        private string sLogoExt;

    	public cMenuItem(string logo, int logosize, string label, string description, string url)
        {
            sLogo = logo;
            nLogoSize = logosize;
            sLabel = label;
            sDescription = description;
            sURL = url;
            sLogoExt = "png";
        }

        public cMenuItem(string logo, int logosize, string label, string description, string url, string logoExt)
        {
            sLogo = logo;
            nLogoSize = logosize;
            sLabel = label;
            sDescription = description;
            sURL = url;
            sLogoExt = logoExt;
        }

		public cMenuItem(string logo, int logosize, string label, string description, string url, bool iscustom)
		{
			sLogo = logo;
			nLogoSize = logosize;
			sLabel = label;
			sDescription = description;
			sURL = url;
			this.IsCustom = iscustom;
		}

        public cMenuItem(string logo, int logosize, string label, string description, string url, string target, bool newWindow)
        {
            sLogo = logo;
            nLogoSize = logosize;
            sLabel = label;
            sDescription = description;
            sURL = url;
            sTarget = target;
            bNewWindow = newWindow;
            sLogoExt = "png";
        }

        public cMenuItem(string logo, int logosize, string label, string description, string url, string target, bool newWindow, string logoExt)
        {
            sLogo = logo;
            nLogoSize = logosize;
            sLabel = label;
            sDescription = description;
            sURL = url;
            sTarget = target;
            bNewWindow = newWindow;
            sLogoExt = logoExt;
        }

        #region properties
        /// <summary>
        /// Logo Extension
        /// </summary>
        public string LogoExt
        {
            get { return sLogoExt; }
        }
        /// <summary>
        /// Logo name
        /// </summary>
        public string logo
        {
            get { return sLogo; }
        }
        /// <summary>
        /// Label/Title
        /// </summary>
        public string label
        {
            get { return sLabel; }
        }
        /// <summary>
        /// Description text
        /// </summary>
        public string description
        {
            get { return sDescription; }
        }
        /// <summary>
        /// Size of the logo image in pixels to use
        /// </summary>
        public int logosize
        {
            get { return nLogoSize; }
        }
        /// <summary>
        /// Raw URL
        /// </summary>
        public string url
        {
            get { return sURL; }
        }
        /// <summary>
        /// URL after ResolveUrl()
        /// </summary>
        public string resolvedUrl
        {
            get { return this.ResolveUrl(sURL); }
        }
        /// <summary>
        /// Target window name
        /// </summary>
        public string target
        {
            get { return sTarget; }
        }

        /// <summary>
        /// Is this to open in a new window
        /// </summary>
        public bool NewWindow
        {
            get { return bNewWindow; }
        }

		/// <summary>
		/// Gets a value indicating whether the menu item is custom
		/// </summary>
    	public bool IsCustom { get; private set; }

    	#endregion
    }
}
