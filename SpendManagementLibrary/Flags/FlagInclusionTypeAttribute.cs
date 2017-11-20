namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The flag inclusion type attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FlagInclusionTypeAttribute : Attribute
    {
        /// <summary>
        /// The inclusion type.
        /// </summary>
        private readonly FlagInclusionType inclusionType;

        /// <summary>
        /// Initialises a new instance of the <see cref="FlagInclusionTypeAttribute"/> class.
        /// </summary>
        /// <param name="inclusiontype">
        /// The flag inclusion type.
        /// </param>
        public FlagInclusionTypeAttribute(FlagInclusionType inclusiontype)
        {
            this.inclusionType = inclusiontype;
        }

        /// <summary>
        /// Gets the inclusion type of the given flag type.
        /// </summary>
        /// <param name="flagtype">
        /// The flag type to get the inclusion type for.
        /// </param>
        /// <returns>
        /// The <see cref="FlagInclusionType"/>.
        /// </returns>
        public static FlagInclusionType Get(FlagType flagtype)
        {
            if (flagtype != null)
            {
                MemberInfo[] mi = flagtype.GetType().GetMember(flagtype.ToString());
                if (mi.Length > 0)
                {
                    FlagInclusionTypeAttribute attr =
                        GetCustomAttribute(mi[0], typeof(FlagInclusionTypeAttribute)) as
                        FlagInclusionTypeAttribute;
                    if (attr != null)
                    {
                        return attr.inclusionType;
                    }
                }
            }

            return FlagInclusionType.All;
        }
    }
}
