using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
	public static class cDef
	{
		public static string EncryptionKey = "qJdK3e4Ms4dU4sBfJ5sK43udKdT543TsB65ANdjTsmG23pLdYsNs23Hys23kQnc42Ld3ju";
		public static string DATE_FORMAT = "dd/MM/yyyy";
		public static string CURRENCY_FORMAT = "£#,###,##0.00";
		public static Color TITLE_COLOUR = Color.FromArgb(255, 255, 255, 153);
		public static string TITLE_COLOUR_HEX = "#FFFF99";
		public static Color DARK_ROW_COLOUR = Color.FromArgb(255, 204, 204, 255);
		public static string DARK_ROW_COLOUR_HEX = "#CCCCFF";
		public static Color BORDER_COLOUR = Color.FromArgb(255, 102, 102, 153);
		public static string BORDER_COLOUR_HEX = "#666699";
		public static Color LIGHT_ROW_COLOUR = Color.FromArgb(255, 255, 255, 255);
		public static string LIGHT_ROW_COLOUR_HEX = "#FFFFFF";
		public static string HTML_fontStyle = " style=\"font: xx-small Arial,sans-serif\" ";
		public static int PAGE_ROWS = 30;
		public static int DBVERSION = 24;
		public static int HEADERBAR_HEIGHT = 120;
		public static int NAVBAR_WIDTH = 160;
		public static int UF_MAXCHARLENGTH = 50;
		public static int UF_MAXCOUNT = 25; // maximum number of entities before listbox is replace by text and search icon
		public static string UBQ_LINK_KEY = "SELFW_UBQLINK";
		public static int MAX_CP_DISPLAY = 25;
		public static int SQL_MIN_YEAR = 1753;
		public static string CRM_FW_GUID = "61C54E87-298F-DA11-A13D-001143EC7885";
		public static string CRM_SUPPORTSERVICE_URL = "https://selenity.com/helpandsupport.asmx";
		public static string CRM_SUPPORT_URL = "https://support.selenity.com";

		// now from modules base
        //public static string FW_BRAND_TITLE_HTML = "<span class=\"framework\" style=\"font-size: 20px;\">framework</span><span class=\"framework2009\" style=\"font-size: 20px;\">2009</span>";
		//public static string FW_BRAND_TITLE_TEXT = "framework2009";

	}
}
