using System;

/// <summary>
/// A Function that is available for Calculated columns.
/// </summary>
/// 
namespace Spend_Management
{
    [Serializable()]
    public class Function
    {
        public Function()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="remarks">
        /// The remarks.
        /// </param>
        /// <param name="example">
        /// The example.
        /// </param>
        /// <param name="syntax">
        /// The syntax.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public Function(int id, string functionName, string description, string remarks, string example, string syntax, string parent)
        {
            this.FunctionName = functionName;
            this.Description = description;
            this.Remarks = remarks;
            this.Example = example;
            this.Syntax = syntax;
            this.ID = id;
            this.Parent = parent;
        }

        public int ID { get; set; }

        public string FunctionName { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public string Example { get; set; }

        public string Syntax { get; set; }

        public string Parent { get; set; }
    } 
}
