﻿namespace SpendManagementApi
{
    using System;
    using System.Security.Principal;
    using BusinessLogic.Identity;

    /// <summary>
    /// Defines a <see cref="WebPrincipal"/> and all it's members
    /// </summary>
    public class WebPrincipal : IPrincipal
    {
        /// <summary>
        /// Constructor for this <see cref="WebPrincipal"/> that allows us to set the <see cref="IIdentity"/>
        /// </summary>
        /// <param name="userIdentity">The <see cref="UserIdentity"/></param>
        public WebPrincipal(UserIdentity userIdentity)
        {
            this.Identity = userIdentity;
        }

        /// <inheritdoc />
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the <see cref="IIdentity"/>
        /// </summary>
        public IIdentity Identity { get; }
    }
}