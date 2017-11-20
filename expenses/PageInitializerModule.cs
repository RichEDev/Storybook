using System.Linq;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace expenses
{
    public sealed class PageInitializerModule : IHttpModule
    {
        private readonly string[] _avoidedPages = new[]
        {
            "/shared/logon.aspx"
        };
        
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(PageInitializerModule));
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (sender, e) => {
                var handler = context.Context.CurrentHandler;
                if (handler != null && this._avoidedPages.Contains(HttpContext.Current.Request.FilePath) == false)
                {
                    string name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") &&
                        !name.StartsWith("Microsoft"))
                    {
                        Global.InitializeHandler(handler);
                    }
                }
            };
        }

        void IHttpModule.Dispose() { }
    }
}