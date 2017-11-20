namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.SessionState;

    /// <summary>
    /// Summary description for SessionModule
    /// </summary>
    public class SessionHttpHandler : IHttpModule
    {
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += this.context_BeginRequest;
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext currentContext = (sender as HttpApplication)?.Context;
            
            if (currentContext != null)
            {
                // If its in the list allow read/write otherwise just read.
                // NOTE this is case sensitive at present, the less processing this does the better
                SessionStateBehavior behavior = SessionConfig.Paths.Contains(currentContext.Request.CurrentExecutionFilePath.ToLower()) ? SessionStateBehavior.Required : SessionStateBehavior.ReadOnly;
                currentContext.SetSessionStateBehavior(behavior);
            }
        }
    }
}