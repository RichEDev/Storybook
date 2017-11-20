using System;
using System.Web.Script.Services;
using System.Web.Services;
using SpendManagementLibrary;
using System.Data;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for svcMenu
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class svcMenu : System.Web.Services.WebService
	{
		/// <summary>
		/// Put something here
		/// </summary>
		/// <param name="menuTitle">sdfdsf sdf d</param>
		/// <param name="menuImageLocation">sdfsdf sdfsdf</param>
		/// <param name="menuOnClickUrl">sdfsdf sdfsdf sdf sdf</param>
		/// <param name="order">sdfsdf sdfsdf sdf dsdf</param>
		/// <returns>asdfdf sdfsdf sdf</returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod]
		public int SaveMenuFavourite(string menuTitle, string menuImageLocation, string menuOnClickUrl, byte order)
		{
			int returnVal;

			if (menuTitle == string.Empty)
			{
				returnVal = -2;
			}
			else if (menuImageLocation == string.Empty || menuOnClickUrl == string.Empty)
			{
				returnVal = -3;
			}
			else
			{
				var currentUser = cMisc.GetCurrentUser();

				var menuFavourites = new MenuFavourites(currentUser);

				returnVal = menuFavourites.SaveFavouriteMenuItem(menuTitle, menuImageLocation, menuOnClickUrl, order, currentUser.EmployeeID);	
			}
			
			return returnVal;
		}


		[WebMethod(EnableSession = true)]
		[ScriptMethod]
		public int DeleteMenuFavourite(int menuFavouriteID)
		{
			int returnVal;

			if (menuFavouriteID < 1)
			{
				returnVal = -3;
			} 
			else
			{
				var currentUser = cMisc.GetCurrentUser();

				var menuFavourites = new MenuFavourites(currentUser);

				returnVal = menuFavourites.DeleteFavouriteMenuItem(menuFavouriteID, currentUser.EmployeeID);
			}

			return returnVal;
		}
	}
}
