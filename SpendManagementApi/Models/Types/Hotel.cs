namespace SpendManagementApi.Models.Types
{   
    using System.ComponentModel.DataAnnotations; 
    using Interfaces;

    /// <summary>
    /// A Type to hold information relating to a hotel
    /// </summary>
    public class Hotel : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Hotels.Hotel, Hotel>
    {
        /// <summary>
        /// The Id of the Hotel
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the hotel
        /// </summary>
        [Required]
        public string HotelName { get; set; } 

        /// <summary>
        /// The first line of the address
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// The second line of the address
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// The city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The county
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// The post code
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// The country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Converts a SpendManagement Hotel to an Hotel API Type
        /// </summary>
        /// <param name="dbType">The SpendManagement hotel</param>
        /// <param name="actionContext">The Action Context</param>
        /// <returns><see cref="Hotel">Hotel</see></returns>
        public Hotel From(SpendManagementLibrary.Hotels.Hotel dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Id = dbType.hotelid;
            this.HotelName = dbType.hotelname;
            this.Address1 = dbType.address1;
            this.Address2 = dbType.address2;
            this.City = dbType.city;
            this.County = dbType.county;
            this.PostCode = dbType.postcode;
            this.Country = dbType.country;

            return this;

        }
        public SpendManagementLibrary.Hotels.Hotel To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }

}