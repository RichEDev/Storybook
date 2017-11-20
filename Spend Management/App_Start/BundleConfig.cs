namespace Spend_Management.App_Start
{
    using System.Web.Optimization;
    using System.Web.UI;

    using SpendManagementLibrary;

    /// <summary>
    /// The set up and configuration of the bundles available to the app
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Defines the bundles and their content
        /// </summary>
        /// <param name="bundles">A bundle collection to populate</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // this is the definition for a named bundle
            // "new ScriptBundle()" can be used if you want minification of js files (more aggressive minify than web essentials 2012)
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~" + GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-1.9.1.min.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui-1.9.2.custom.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/json2.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.watermark.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/jQuery/easytree/jquery.easytree.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/expense/bootstrap.min.js",
                "~" + GlobalVariables.StaticContentLibrary + "/js/expense/jquery.qtip.min.js",
                "~/shared/javascript/minify/jquery-selui-address.js",
                "~/shared/javaScript/common.js"
                ));

            // this allows the bundle to be referenced by a simple name in ScriptManager/ToolkitScriptManager/ScriptManagerProxy
            ScriptManager.ScriptResourceMapping.AddDefinition("jQuery", new ScriptResourceDefinition
            {
                Path = "~/bundles/jquery"
            });

            bundles.Add(new Bundle("~/bundles/syncfusion").Include(
                // these will probably be replaced by a single file, once we get access to synfusions script generation tool
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/external/jquery.easing.1.3.min.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/external/jquery.globalize.min.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/external/jsrender.min.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/common/ej.core.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/common/ej.data.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/common/ej.draggable.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/common/ej.scroller.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.pager.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.waitingpopup.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.radiobutton.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.dropdownlist.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.dialog.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.button.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.autocomplete.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.checkbox.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.datepicker.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.datetimepicker.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.editor.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.toolbar.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.grid.js",
                "~" + GlobalVariables.StaticContentLibrary + "/syncfusion/scripts/web/ej.web.all.js"
            ));

            ScriptManager.ScriptResourceMapping.AddDefinition("SyncFusion", new ScriptResourceDefinition
            {
                Path = "~/bundles/syncfusion"
            });

            bundles.Add(new Bundle("~/bundles/common").Include(
                "~/shared/javaScript/minify/sel.common.js",
                "~/shared/javaScript/minify/sel.errors.js",         // formerly in sel.common.js
                "~/shared/javaScript/minify/sel.controls.js",       // formerly in sel.common.js
                "~/shared/javaScript/minify/sel.forms.js",          // formerly in sel.common.js
                "~/shared/javaScript/minify/sel.masterPopup.js",    // formerly in sel.common.js
                "~/shared/javaScript/minify/sel.input.js"           // formerly in sel.common.js
                ));

            ScriptManager.ScriptResourceMapping.AddDefinition("common", new ScriptResourceDefinition
            {
                Path = "~/bundles/common"
            });

            bundles.Add(new Bundle("~/bundles/master").Include(
                "~/shared/javaScript/minify/sel.main.js",
                "~/bundles/common",
                "~/shared/javaScript/minify/sel.jsonParse.js",
                "~/shared/javaScript/minify/sel.grid.js",
                "~/shared/javaScript/shared.js",
                "~/shared/javaScript/tooltips.js"
                ));

            ScriptManager.ScriptResourceMapping.AddDefinition("master", new ScriptResourceDefinition
            {
                Path = "~/bundles/master"
            });

            /*
             * WEB ESSENTIALS
             * single file bundles - to take advantage of the pre-generated minified/non-minified version of the file in release/debug
             */

            bundles.Add(new Bundle("~/bundles/tooltips").Include("~/shared/javaScript/minify/sel.tooltips.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("tooltips", new ScriptResourceDefinition
            {
                Path = "~/bundles/tooltips"
            });

            bundles.Add(new Bundle("~/bundles/logon").Include("~/shared/javaScript/minify/sel.logon.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("logon", new ScriptResourceDefinition
            {
                Path = "~/bundles/logon"
            });

            bundles.Add(new Bundle("~/bundles/documentmerge").Include("~/shared/javaScript/minify/sel.docMerge.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("documentMerge", new ScriptResourceDefinition
            {
                Path = "~/bundles/documentmerge"
            });

            /*
             * ScriptBundle
             * single file bundles - to generate a fully minified version
             */

            bundles.Add(new ScriptBundle("~/bundles/addresses").Include("~/shared/javaScript/minify/sel.addresses.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("addresses", new ScriptResourceDefinition
            {
                Path = "~/bundles/addresses"
            });

            bundles.Add(new ScriptBundle("~/bundles/organisations").Include("~/shared/javaScript/minify/sel.organisations.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("organisations", new ScriptResourceDefinition
            {
                Path = "~/bundles/organisations"
            });

            bundles.Add(new ScriptBundle("~/bundles/expenses").Include("~/shared/javaScript/minify/sel.expenses.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("expenses", new ScriptResourceDefinition
            {
                Path = "~/bundles/expenses"
            });

            bundles.Add(new ScriptBundle("~/bundles/idletimeout").Include("~/shared/javaScript/minify/sel.idletimeout.js"));
            ScriptManager.ScriptResourceMapping.AddDefinition("idletimeout", new ScriptResourceDefinition
            {
                Path = "~/bundles/idletimeout"
            });

            bundles.Add(new ScriptBundle("~/bundles/knowledge").Include(
                "~/shared/javaScript/minify/sel.knowledge.js",
                "~/shared/javaScript/minify/sel.helpAndSupportTickets.js"
            ));
            ScriptManager.ScriptResourceMapping.AddDefinition("knowledge", new ScriptResourceDefinition
            {
                Path = "~/bundles/knowledge"
            });

        }
    }
}