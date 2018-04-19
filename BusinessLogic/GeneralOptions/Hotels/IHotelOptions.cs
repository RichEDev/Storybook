namespace BusinessLogic.GeneralOptions.Hotels
{
    /// <summary>
    /// Defines a <see cref="IHotelOptions"/> and it's members
    /// </summary>
    public interface IHotelOptions
    {
        /// <summary>
        /// Gets or sets the show reviews
        /// </summary>
        bool ShowReviews { get; set; }

        /// <summary>
        /// Gets or sets the send review request
        /// </summary>
        bool SendReviewRequests { get; set; }
    }
}
