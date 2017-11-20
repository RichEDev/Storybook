namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Javascript JoinVia
    /// </summary>
    public class JSJoinVia
    {
        public int JoinViaID = 0;
        public string JoinViaDescription = string.Empty;
        public Guid JoinViaAS = Guid.Empty;
        public List<Part> Parts = new List<Part>();

        public class Part
        {
            public Guid ViaID = Guid.Empty;
            public JoinViaPart.IDType ViaType = JoinViaPart.IDType.Field;
            public JoinViaPart.JoinType JoinType = JoinViaPart.JoinType.LEFT;
        }
    }
}