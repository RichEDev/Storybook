namespace SpendManagementApi.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.ServiceModel.Channels;
    using System.Web;
    using SpendManagementLibrary;
    using Models.Common;

    /// <summary>Provides some IP Address utilities.</summary>
    public static class IPValidator
    {
        private const string HttpContext = "MS_HttpContext";
        private const string ConfigKeySelenityIpAddress = "SelenityIPAddressWhiteList";
        private const string ConfigKeySystemGreenLightIpAddresses = "SystemGreenLightIPAddressWhiteList";
        private static List<string> _selenityIpList;
        private static List<string> _systemGreenLightIpList;

        /// <summary>Returns the IP address of the client from the HttpRequestMessage.</summary>
        /// <param name="request">The HttpRequestMessage.</param>
        /// <returns>The IP Address.</returns>
        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            IPAddress address = null;
            var ipString = "";

            if (request.Properties.ContainsKey(HttpContext))
            {
                ipString = ((HttpContextBase)request.Properties[HttpContext]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                ipString = ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;
            }

            address = IPAddress.Parse(ipString);

            if (address == null)
            {
                throw new Exception("Client IP Address Not Found in HttpRequest");
            }

            ipString = address.ToString();
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return ipString;
            }

            try
            {
                address = Dns.GetHostAddresses(ipString).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (Exception error)
            {
                throw new ApiException("Resolving the host failed for the following IP address: " + ipString, error.Message);
            }

            if (address == null)
            {
                throw new Exception("Version 4 Client IP Address Not Found in HttpRequest");
            }

            return address.ToString();
        }

        /// <summary>Validates the IP Address with the Spend Management database.</summary>
        /// <param name="request">The request.</param>
        /// <param name="accountId">The account Id in which to look for IPAddresses</param>
        /// <returns>A value indicating whether or not the IP address is allowed</returns>
        public static bool Validate(HttpRequestMessage request, int accountId)
        {
            if (request.IsLocal())
            {
                return true;
            }

            var ip = GetClientIpAddress(request);
            var clsIpFilters = new cIPFilters(accountId);
            var canAccess = clsIpFilters.CheckValidIPForAccesss(ip);

            if (canAccess)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the IP address comes from one of the predefined address in the config.
        /// These should be set for known Expedite processing locations and pipe-separated.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A bool indicating if the user comes from expedite.</returns>
        public static bool IsSelenity(HttpRequestMessage request)
        {
            if (request.IsLocal())
            {
                return true;
            }

            var ip = GetClientIpAddress(request);

            // populate the static IP list if it isn't already.
            if (_selenityIpList == null)
            {
                _selenityIpList = PopulateStaticIpAddress(ConfigKeySelenityIpAddress);
            }

            return ip.Contains("192.168.") || _selenityIpList.Contains(ip);
        }

        /// <summary>
        /// Determines whether the IP address comes from one of the predefined address in the config.
        /// These should be set for known authorised user processing locations and pipe-separated.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A bool indicating if the user comes from greenlight.</returns>
        public static bool IsSystemGreenlightOnly(HttpRequestMessage request)
        {
            if (request.IsLocal())
            {
                return true;
            }

            var ip = GetClientIpAddress(request);

            // populate the static IP list if it isn't already.
           _systemGreenLightIpList = PopulateStaticIpAddress(ConfigKeySystemGreenLightIpAddresses);
            return ip.Contains("192.168.") ||_systemGreenLightIpList.Contains(ip);
        }

        /// <summary>
        /// Populate authorised static Ip address.
        /// </summary>
        /// <param name="configKey"> Configuration key for the ip address</param>
        /// <returns>List of Ip address</returns>
        private static List<string> PopulateStaticIpAddress(string configKey)
        {
            var ipString = ConfigurationManager.AppSettings[configKey] ?? "127.0.0.1";
            return ipString.Split(char.Parse("|")).ToList();
        }


    }
}