namespace SpendManagementApi.Areas.HelpPage.Controllers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Mvc;
    using Models;

    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        /// <summary>
        /// Creates a Help Controller using GlobablConfiguration.
        /// </summary>
        public HelpController() : this(GlobalConfiguration.Configuration) { }

        /// <summary>
        /// Creates a Help Controller using HttpConfiguration.
        /// </summary>
        public HelpController(HttpConfiguration config) { Configuration = config; }

        /// <summary>
        /// The HttpConfiguration.
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// The Home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Overview of the API.
        /// </summary>
        /// <returns></returns>
        public ActionResult Overview()
        {
            ViewBag.TokenExpiry = ConfigurationManager.AppSettings["AuthTokenExpiryMinutes"];
            ViewBag.ExpensesSite = ConfigurationManager.AppSettings["ExpensesSite"];
            ViewBag.ApiSite = Request.Url.DnsSafeHost + "/";
            return View();
        }

        /// <summary>
        /// The Getting Started Section
        /// </summary>
        /// <returns></returns>
        public ActionResult GettingStarted()
        {
            ViewBag.ExpensesSite = ConfigurationManager.AppSettings["ExpensesSite"];
            ViewBag.ApiSite = Request.Url.DnsSafeHost + "/";
            return View();
        }

        /// <summary>
        /// The index of the API
        /// </summary>
        /// <returns></returns>
        public ActionResult Reference(string versionId)
        {
            int lowestSupportedApiVersionfirstSupportedVersion;
            int version = this.GetVersionNumber(versionId, out lowestSupportedApiVersionfirstSupportedVersion);
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            Configuration.Services.Replace(typeof(IApiExplorer), new SelApiExplorer(version.ToString(CultureInfo.InvariantCulture)));
            var apiExplorer = (SelApiExplorer)Configuration.Services.GetApiExplorer();
            var apiDescriptions = apiExplorer.ApiDescriptions;
            var versionNumbers = apiExplorer.VersionNumbers.Where(versionNumber => versionNumber != 0 && versionNumber >= lowestSupportedApiVersionfirstSupportedVersion).ToList();

            ViewBag.VersionNumbers = new List<int>(versionNumbers.OrderBy(versionNumber => versionNumber));
            ViewBag.SelectedVersion = version;

            return View(apiDescriptions);
        }

        /// <summary>
        /// Displays information on the selected action
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public ActionResult Api(string apiId, string versionId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                int lowestSupportedApiVersionfirstSupportedVersion;
                int version = this.GetVersionNumber(versionId, out lowestSupportedApiVersionfirstSupportedVersion);
                ViewBag.SelectedVersion = version;

                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View("Error");
        }

        /// <summary>
        /// Displays type information for all public types exposed by the API
        /// </summary>
        /// <returns></returns>
        public ActionResult TypeInformation()
        {
            var typeLibrary = HelpPageConfig.ApiTypeLibrary == null ? null : HelpPageConfig.ApiTypeLibrary.FirstOrDefault();
            if (typeLibrary == null)
            {
                var dp = Configuration.Services.GetDocumentationProvider() as XmlDocumentationProvider;
                var ad = Configuration.Services.GetApiExplorer();

                if (dp == null)
                {
                    throw new ConfigurationErrorsException("Have you changed the default documentation provider? This method expects the XmlDocumentationProvider");
                }

                typeLibrary = dp.GetAllTypeDocumentation(ad);
                HelpPageConfig.ApiTypeLibrary = new ConcurrentBag<TypeLibrary>(new[] { typeLibrary });
            }

            return View("ApiTypes", typeLibrary);
        }

        private int GetVersionNumber(string versionId, out int lowestSupportedApiVersionfirstSupportedVersion)
        {
            int defaultApiVersion = 1;
            int version = string.IsNullOrEmpty(versionId) ? 0 : Convert.ToInt32(versionId);

            if (ConfigurationManager.AppSettings["DefaultApiVersion"] != null)
            {
                defaultApiVersion = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultApiVersion"]);
            }

            lowestSupportedApiVersionfirstSupportedVersion = 1;

            if (ConfigurationManager.AppSettings["LowestSupportedApiVersion"] != null)
            {
                lowestSupportedApiVersionfirstSupportedVersion = Convert.ToInt32(ConfigurationManager.AppSettings["LowestSupportedApiVersion"]);
            }

            if (version == 0)
            {
                version = defaultApiVersion;
            }

            if (version < lowestSupportedApiVersionfirstSupportedVersion)
            {
                version = lowestSupportedApiVersionfirstSupportedVersion;
            }

            return version;
        }
    }
}