namespace BusinessLogic.Constants.Tables
{
    using System;

    /// <summary>
    /// The definition of reasons.
    /// </summary>
    public static class Reasons
    {
        /// <summary>
        /// The ID of the reasons table.
        /// </summary>
        public static Guid TableId => new Guid("83077E08-FE7D-4C1A-A306-BE4327C349C1");

        /// <summary>
        /// The fields relating to reasons.
        /// </summary>
        public static class Fields
        {
            /// <summary>
            /// Gets the Reason ID.
            /// </summary>
            public static Guid ReasonId => new Guid("02AEB708-52E3-4A6D-90D8-E335220D4249");

            /// <summary>
            /// Gets the Reason.
            /// </summary>
            public static Guid Reason => new Guid("AF839FE7-8A52-4BD1-962C-8A87F22D4A10");

            /// <summary>
            /// Gets the description.
            /// </summary>
            public static Guid Description => new Guid("44AB09E8-3294-4FDD-BA50-97F5D69C0C64");

            /// <summary>
            /// Gets a value indicating whether the reason is archived.
            /// </summary>
            public static Guid Archived => new Guid("CDC425FE-9860-4598-95FD-07D22E840255");

            /// <summary>
            /// Gets the employee ID who created the reason.
            /// </summary>
            public static Guid CreatedBy => new Guid("7C0200E7-CEF1-40AE-9F96-B00A5F59DCFF");

            /// <summary>
            /// Gets the Date the reason was created.
            /// </summary>
            public static Guid CreatedOn => new Guid("60FE9333-B5DE-4767-B947-00C66EEFC868");

            /// <summary>
            /// Gets the employee ID who modified the reason.
            /// </summary>
            public static Guid ModifiedBy => new Guid("A4DD9A5A-AC03-407C-9FD1-A71366CBE135");

            /// <summary>
            /// Gets the Date the reason was modified.
            /// </summary>
            public static Guid ModifiedOn => new Guid("06EB1CD3-09BC-4E15-8DA9-0E96B383497C");

            /// <summary>
            /// Gets the account code vat for the reason.
            /// </summary>
            public static Guid AccountCodeVat => new Guid("71864488-74F0-4990-B6A5-736BBFB5A1BB");

            /// <summary>
            /// Gets the account code no vat for the reason.
            /// </summary>
            public static Guid AccountCodeNoVat => new Guid("1C15034E-AE99-474F-A8DB-2B593FCAEA2F");
        }
    }
}
