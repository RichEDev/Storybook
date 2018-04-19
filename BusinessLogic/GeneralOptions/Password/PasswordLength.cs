namespace BusinessLogic.GeneralOptions.Password
{
    /// <summary>
    /// An enum to store the password length option
    /// </summary>
    public enum PasswordLength
    {
        /// <summary>
        /// Any length
        /// </summary>
        AnyLength = 1,

        /// <summary>
        /// Equal To
        /// </summary>
        EqualTo = 2,

        /// <summary>
        /// Greater Than
        /// </summary>
        GreaterThan = 3,

        /// <summary>
        /// Less Than
        /// </summary>
        LessThan = 4,

        /// <summary>
        /// Between
        /// </summary>
        Between = 5
    }
}
