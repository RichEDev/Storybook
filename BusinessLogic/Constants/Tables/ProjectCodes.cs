namespace BusinessLogic.Constants.Tables
{
    using System;

    /// <summary>
    /// The definition of project codes.
    /// </summary>
    public static class ProjectCodes
    {
        /// <summary>
        /// The ID of the Project Code table.
        /// </summary>
        public static Guid TableId => new Guid("e1ef483c-7870-42ce-be54-ecc5c1d5fb34");

        /// <summary>
        /// The fields relating to project codes..
        /// </summary>
        public static class Fields
        {
            /// <summary>
            /// Gets the Project Code ID.
            /// </summary>
            public static Guid ProjectcodeId => new Guid("311857E6-33C2-47A4-AD6F-F38708B2A45B");

            /// <summary>
            /// Gets the projectcode.
            /// </summary>
            public static Guid Projectcode => new Guid("6D06B15E-A157-4F56-9FF2-E488D7647219");

            /// <summary>
            /// Gets the description.
            /// </summary>
            public static Guid Description => new Guid("0AD6004F-7DFD-4655-95FE-5C86FF5E4BE4");

            /// <summary>
            /// Gets a value indicating whether the project code is archived.
            /// </summary>
            public static Guid Archived => new Guid("7b406750-adbd-461f-9d36-97dbdbd8f451");

            /// <summary>
            /// Gets the employee ID who created the project code.
            /// </summary>
            public static Guid CreatedBy => new Guid("AD3BFF2E-9ABF-4F39-8A39-1BE9172097F1");

            /// <summary>
            /// Gets the Date the project code was created.
            /// </summary>
            public static Guid CreatedOn => new Guid("39EE41D3-C7B2-4BD4-9FC6-87919A485E70");

            /// <summary>
            /// Gets the employee ID who modified the project code.
            /// </summary>
            public static Guid ModifiedBy => new Guid("A2A25B42-22EA-4787-B7B2-D95016F66987");

            /// <summary>
            /// Gets the Date the project code was modified.
            /// </summary>
            public static Guid ModifiedOn => new Guid("2B34FE24-6B7F-4B58-92EF-2A450EE863C3");

            /// <summary>
            /// Gets a value indicating whether the project code is rechargable.
            /// </summary>
            public static Guid Rechargeable => new Guid("2396564B-76A4-4C0C-971B-1E7915E9F3A0");

            /// <summary>
            /// Does this table contain a field with the given <see cref="Guid"/>.
            /// </summary>
            /// <param name="id">
            /// The <see cref="Guid"/>id to search for.
            /// </param>
            /// <returns>
            /// <see cref="bool"/> True if this table contains a field with the <param name="id"></param>.
            /// </returns>
            public static bool Contains(Guid id)
            {
                return id == Projectcode || id == ProjectcodeId || id == Description || id == Archived || id == CreatedBy || id == CreatedOn || id == ModifiedBy || id == ModifiedOn || id == Rechargeable;
            }
        }
    }
}
