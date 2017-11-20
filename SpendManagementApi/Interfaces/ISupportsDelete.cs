using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Interfaces
{
    public interface ISupportsDelete
    {
        bool ForDelete { get; set; }
    }

    public class Deleteable : ISupportsDelete
    {
        /// <summary>
        /// Gets or sets a value indicating whether the element supports deletion
        /// </summary>
        public bool ForDelete { get; set; }
    }
}