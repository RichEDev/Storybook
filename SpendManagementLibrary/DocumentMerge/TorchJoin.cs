using System;

namespace SpendManagementLibrary.DocumentMerge
{
    /// <summary>
    /// Allows specification of join information between the torch data tables and the depth level (table heirarchy)
    /// </summary>
    class TorchJoin : IComparable<TorchJoin>
    {
        public string TableName { get; set; }

        public string Join { get; set; }

        public int DepthLevel { get; set; }

        public TorchJoin(string tableName, string join, int depthLevel)
        {
            TableName = tableName;
            Join = join;
            DepthLevel = depthLevel;
        }

        public int CompareTo(TorchJoin other)
        {
            return this.DepthLevel > other.DepthLevel ? this.DepthLevel : other.DepthLevel;
        }
    }
}
