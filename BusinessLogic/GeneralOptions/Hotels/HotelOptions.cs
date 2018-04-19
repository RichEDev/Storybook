namespace BusinessLogic.GeneralOptions.Hotels
{
    /// <summary>
    /// Defines a <see cref="HotelOptions"/> and it's members
    /// </summary>
    public class HotelOptions : IHotelOptions
    {
        /// <summary>
        /// Gets or sets the show reviews
        /// </summary>
        public bool ShowReviews { get; set; }

        /// <summary>
        /// Gets or sets the send review request
        /// </summary>
        public bool SendReviewRequests { get; set; }
    }
}
