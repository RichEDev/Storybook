using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SpendManagementLibrary.Addresses;
using Spend_Management.shared.webServices;

namespace Spend_Management
{
    /// <summary>
    /// Static method(s) for setting up Automapper
    /// </summary>
    public static class AutomapperConfig
    {
        /// <summary>
        /// This tells Automapper about the types it is going to have to map
        /// and asserts that it can map them ok.
        /// </summary>
        public static void Configure()
        {
            Mapper.CreateMap<Address, svcAddresses.ManualAddress>();
            Mapper.AssertConfigurationIsValid();
        }
    }
}