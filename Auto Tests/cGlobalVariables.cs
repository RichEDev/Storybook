using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace Auto_Tests
{
    /// <summary>
    /// This is a collection of pre-defined global variables
    /// </summary>
    public static class cGlobalVariables
    {
        /// <summary>
        /// This is used to determine if tests are currently running on a local machine
        /// </summary>
        public static bool UsingLocal
        {
            get
            {
                bool bUsingLocal = Convert.ToBoolean(ConfigurationManager.AppSettings["UsingLocalMachine"]);
                return bUsingLocal;
            }
        }


        /// <summary>
        /// Used to set the product that the codedui tests will run against
        /// </summary>
        /// <returns></returns>
        public static ProductType GetProductFromAppConfig()
        {
            string product = Convert.ToString(ConfigurationManager.AppSettings["executingProduct"].ToString());

            switch (product.ToLower())
            {
                case "expenses":
                    return ProductType.expenses;
                case "framework":
                    return ProductType.framework;
                case "corporate diligence":
                    return ProductType.corporateD;
                case "smart diligence":
                    return ProductType.smartD;
                default:
                    return ProductType.expenses;
            }
        }

        /// <summary>
        /// Use this when referencing the Company ID currently in use
        /// </summary>
        public static string CompanyID(ProductType product)
        {
            if (UsingLocal == true)
            {
                switch (product)
                {
                    case ProductType.expenses:
                        return ConfigurationManager.AppSettings["expensesCompanyID"];
                    case ProductType.framework:
                        return ConfigurationManager.AppSettings["frameworkCompanyID"];
                    default:
                        return ConfigurationManager.AppSettings["expensesCompanyID"];
                }
            }
            else
            {
                return "CodedUITests";
            }
        }


        /// <summary>
        /// Use this when referencing the claimant Username currently in use
        /// </summary>
        public static string ClaimantUserName(ProductType product)
        {
            if (UsingLocal == true)
            {
                switch (product)
                {
                    case ProductType.expenses:
                        return ConfigurationManager.AppSettings["expensesClaimantUsername"];
                    case ProductType.framework:
                        return ConfigurationManager.AppSettings["frameworkClaimantUsername"];
                    default:
                        return ConfigurationManager.AppSettings["expensesClaimantUsername"];
                }
            }
            else
            {
                return "CodedUIClaimant";
            }
        }


        /// <summary>
        /// Use this when referencing the administrator Username currently in use
        /// </summary>
        public static string AdministratorUserName(ProductType product)
        {
            if (UsingLocal == true)
            {
                switch (product)
                {
                    case ProductType.expenses:
                        return ConfigurationManager.AppSettings["expensesAdminUsername"];
                    case ProductType.framework:
                        return ConfigurationManager.AppSettings["frameworkAdminUsername"];
                    default:
                        return ConfigurationManager.AppSettings["expensesAdminUsername"];
                }
            }
            else
            {
                return "CodedUIAdmin";
            }
        }


        /// <summary>
        /// Use this when referencing the administrator Password currently in use
        /// </summary>
        public static string AdministratorPassword(ProductType product)
        {
            if (UsingLocal == true)
            {
                switch (product)
                {
                    case ProductType.expenses:
                        return ConfigurationManager.AppSettings["expensesAdminPassword"];
                    case ProductType.framework:
                        return ConfigurationManager.AppSettings["frameworkAdminPassword"];
                    default:
                        return ConfigurationManager.AppSettings["expensesAdminPassword"];
                }
            }
            else
            {
                return "Password1";
            }
        }


        /// <summary>
        /// Use this when referencing the claimant Password currently in use
        /// </summary>
        public static string ClaimantPassword(ProductType product)
        {
            if (UsingLocal == true)
            {
                switch (product)
                {
                    case ProductType.expenses:
                        return ConfigurationManager.AppSettings["expensesClaimantPassword"];
                    case ProductType.framework:
                        return ConfigurationManager.AppSettings["frameworkClaimantPassword"];
                    default:
                        return ConfigurationManager.AppSettings["expensesClaimantPassword"];
                }
            }
            else
            {
                return "Password1";
            }
        }

        /// <summary>
        /// Use this when referencing the default address for expenses
        /// </summary>
        public static string ExpensesAddress
        {
            get
            {
                string expensesWebAddress = "https://" + ConfigurationManager.AppSettings["expensesWebAddress"];
                return expensesWebAddress;
            }
        }


        /// <summary>
        /// Use this when referencing the default address for framework
        /// </summary>
        public static string FrameworkAddress
        {
            get
            {
                string frameworkWebAddress = "https://" + ConfigurationManager.AppSettings["frameworkWebAddress"];
                return frameworkWebAddress;
            }
        }

        public static string DataSourceDatabaseConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString(); }
        }


        /// <summary>
        /// Use this to extract the firstname and surname name of a user
        /// </summary>
        /// <param name="logonType">Pass in the logontype to use</param>
        public static string EntireUsername(ProductType product, LogonType logonType)
        {
            string sUserName = string.Empty;

            if (logonType == LogonType.administrator)
            {
                sUserName = cGlobalVariables.AdministratorUserName(product);
            }
            else if (logonType == LogonType.claimant)
            {
                sUserName = cGlobalVariables.ClaimantUserName(product);
            }

            cDatabaseConnection database = new cDatabaseConnection(cGlobalVariables.dbConnectionString(product));

            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT firstname, surname FROM employees WHERE username = '" + sUserName + "'");

            reader.Read();

            sUserName = reader.GetValue(0).ToString();
            sUserName = sUserName + " ";
            sUserName = sUserName + reader.GetValue(1).ToString();

            reader.Close();

            return sUserName;
        }


        /// <summary>
        /// Use this to when referencing a database connection string
        /// </summary>
        /// <param name="logonType">Pass in the product being used</param>
        public static string dbConnectionString(ProductType product)
        {
            switch (product)
            {
                case ProductType.expenses:
                    return ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString();
                case ProductType.framework:
                    return ConfigurationManager.ConnectionStrings["frameworkDatabase"].ToString();
                default:
                    return ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString();
            }
        }

        /// <summary>
        /// Use this to when referencing a metabase connection string
        /// </summary>
        /// <param name="logonType">Pass in the product being used</param>
        public static string MetabaseConnectionString(ProductType product)
        {
            switch (product)
            {
                case ProductType.expenses:
                    return ConfigurationManager.ConnectionStrings["expensesMetabase"].ToString();
                case ProductType.framework:
                    return ConfigurationManager.ConnectionStrings["frameworkMetabase"].ToString();
                default:
                    return ConfigurationManager.ConnectionStrings["expensesMetabase"].ToString();
            }
        }


    }



    /// <summary>
    /// Use this to pass a usertype of claimant or administrator
    /// </summary>
    public enum LogonType
    {
        /// <summary>
        /// claimant
        /// </summary>
        claimant,
        /// <summary>
        /// administrator
        /// </summary>
        administrator
    }

    /// <summary>
    /// Use this to set which Company ID to use for logon
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// Use this for expenses
        /// </summary>
        [Description("Expenses")]
        expenses,
        /// <summary>
        /// Use this for framework
        /// </summary>
        [Description("Framework")]
        framework,
        /// <summary>
        /// Use this for Corporate Diligence
        /// </summary>
        [Description("Corporate Diligence")]
        corporateD,
        /// <summary>
        /// Use this for Smart Diligence
        /// </summary>
        [Description("Smart Diligence")]
        smartD
    }
}
