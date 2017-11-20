namespace SpendManagementLibrary
{
	using System;

	/// <summary>
	/// An Employee's Menu Favourite
	/// </summary>
	[Serializable]
    public class MenuFavourite
    {
    	/// <summary>
    	/// Initialises a new instance of the <see cref="MenuFavourite"/> class.
    	/// </summary>
    	/// <param name="menufavouriteid">
    	/// The menufavouriteid.
    	/// </param>
    	/// <param name="title">
    	/// The title.
    	/// </param>
    	/// <param name="iconlocation">
    	/// The iconlocation.
    	/// </param>
		/// <param name="onclickurl">
		/// The onclick URL.
		/// </param>
    	/// <param name="order">
    	/// The order.
    	/// </param>
    	public MenuFavourite(int menufavouriteid, string title, string iconlocation, string onclickurl, byte order)
        {            
            this.MenuFavouriteID = menufavouriteid;
            this.Title = title;            
            this.IconLocation = iconlocation;
    		this.OnClickUrl = onclickurl;
        	this.Order = order;            
        }

        #region properties

    	/// <summary>
    	/// Gets the ID of the Menu Favourite
    	/// </summary>
    	public int MenuFavouriteID { get; private set; }

    	/// <summary>
    	/// Gets the Title of the Menu Favourite
    	/// </summary>
    	public string Title { get; private set; }

    	/// <summary>
    	/// Gets the Icon Location of the Menu Favourite
    	/// </summary>
    	public string IconLocation { get; private set; }

		/// <summary>
		/// Gets the onclick URL of the Menu Favourite
		/// </summary>
		public string OnClickUrl { get; private set; }

    	/// <summary>
    	/// Gets the Order of the Menu Favourite 
    	/// </summary>
    	public byte Order { get; private set; }
        
        #endregion
    }
}
