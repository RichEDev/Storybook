namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Models.Types;
    using Spend_Management;

    internal class HotelRepository : BaseRepository<Hotel>, ISupportsActionContext
    {

        private readonly IActionContext _actionContext;

        public HotelRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.Id, x => x.HotelName)
        {
            this._actionContext = actionContext;
        }

        public override IList<Hotel> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a Hotel by its Id
        /// </summary>
        /// <param name="id">THe Id of the Hotel</param>
        /// <returns>The <see cref="Hotel">Hotel</see></returns>
        public override Hotel Get(int id)
        {
            SpendManagementLibrary.Hotels.Hotel hotel = SpendManagementLibrary.Hotels.Hotel.Get(id);
            return new Hotel().From(hotel, ActionContext);
        }

        /// <summary>
        /// Gets a list of hotels based on the hotel name search criteria
        /// </summary>
        /// <param name="hotelName"></param>
        /// <returns></returns>
        public List<Hotel> GetHotelsByName(string hotelName)
        {

            IEnumerable<SpendManagementLibrary.Hotels.Hotel> hotels = this.ActionContext.Hotels.GetHotelsByName(hotelName);

            var hotelSearchResults = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                hotelSearchResults.Add(new Hotel().From(hotel, ActionContext));
            }

            return hotelSearchResults;
        }

        /// <summary>
        /// Saves a new Hotel
        /// </summary>
        /// <param name="hotel">The <see cref="Hotel">Hotel</see> data to be saved</param>
        /// <returns>The details of the newly saved <see cref="Hotel">Hotel</see></returns>
        public override Hotel Add(Hotel hotel)
        {
            var hotelId = ActionContext.Hotel.Add(hotel.HotelName, hotel.Address1, hotel.Address2, hotel.City,
                hotel.County, hotel.PostCode, hotel.Country, 0, string.Empty, string.Empty, User.EmployeeID);

            return this.Get(hotelId);
        }
    }
}