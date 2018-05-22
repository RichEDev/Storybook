namespace BusinessLogic.Announcements
{
    using System;

    /// <summary>
    /// Flag enumerator to calculate the audience of an announcement
    /// </summary>
    [Flags]
    public enum AudienceFlags
    {
        /// <summary>
        /// Value stating whether the audience is Administrator 
        /// </summary>
        Admins = 1,

        /// <summary>
        /// Value stating whether the audience is Claimants
        /// </summary>
        Claimants = 2,

        /// <summary>
        /// Value stating whether the audience is Nhs
        /// </summary>
        Nhs = 4,

        /// <summary>
        /// Value stating whether the audience is NonNhs
        /// </summary>
        NonNhs = 8
    }
}