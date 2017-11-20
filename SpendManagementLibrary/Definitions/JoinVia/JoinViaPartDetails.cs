namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;

    /// <summary>
    /// The details necessary for creating an SQL Join string portion
    /// </summary>
    [Serializable]
    public class JoinViaPartDetails
    {
        /// <summary>
        /// Gets or sets the type of join
        /// </summary>
        public JoinViaPart.JoinType JoinType { get; set; }

        /// <summary>
        /// Gets or sets the source of the join, the "left" side
        /// </summary>
        public JoinSide Left { get; set; }

        /// <summary>
        /// Gets or sets the destination of the join, the "right" side
        /// </summary>
        public JoinSide Right { get; set; }

        /// <summary>
        /// The source or destination of a join, the "left" side or "right" side
        /// </summary>
        public struct JoinSide
        {
            /// <summary>
            /// Gets or sets the table name/alias AS
            /// </summary>
            public cTable Table
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the column name
            /// </summary>
            public cField Column
            {
                get;
                set;
            }
        }
    }
}