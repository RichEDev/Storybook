using System;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Configuration;
using Spend_Management;
using SpendManagementLibrary;

namespace expenses.admin
{
    public class cFaqs
    {
        public int nAccountID;
        public Cache Cache = System.Web.HttpContext.Current.Cache;
        public System.Collections.Generic.SortedList<int, cFaq> listGlobalFaqs;
        public System.Collections.Generic.SortedList<int, cFaq> listCustomerFaqs;

        public cFaqs(int accountID)
        {
            nAccountID = accountID;
            InitialiseData();
        }

        private void InitialiseData()
        {

            listGlobalFaqs = (System.Collections.Generic.SortedList<int, cFaq>)Cache["global_faqs"];
            listCustomerFaqs = (System.Collections.Generic.SortedList<int, cFaq>)Cache["customer_faqs"];

            if (listGlobalFaqs == null)
            {
                CacheGlobalList();
            }

            if (listCustomerFaqs == null)
            {
                CacheCustomerList();
            }
        }

        public System.Collections.Generic.SortedList<int, cFaq> CacheGlobalList()
        {
            listGlobalFaqs = new System.Collections.Generic.SortedList<int, cFaq>();
            string strSQL = "SELECT faqid, question, answer, tip, datecreated, faqcategoryid FROM dbo.global_faqs";
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expdata.sqlexecute.CommandText = strSQL;
            SqlCacheDependency dependency = null;

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                dependency = new SqlCacheDependency(expdata.sqlexecute);
            }

            strSQL = "SELECT global_faqs.faqid, global_faqs.question, global_faqs.answer, global_faqs.tip, global_faqs.datecreated, global_faqs.faqcategoryid, global_faqcategories.category FROM global_faqs, global_faqcategories WHERE global_faqcategories.faqcategoryid=global_faqs.faqcategoryid";

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                cFaq tmpFaq;

                while (reader.Read())
                {
                    int faqID = reader.GetInt32(reader.GetOrdinal("faqid"));
                    string question = reader.GetString(reader.GetOrdinal("question"));
                    string answer = reader.GetString(reader.GetOrdinal("answer"));

                    string tip;
                    if (reader.IsDBNull(reader.GetOrdinal("tip")))
                    {
                        tip = "";
                    }
                    else
                    {
                        tip = reader.GetString(reader.GetOrdinal("tip"));
                    }

                    DateTime dateCreated = reader.GetDateTime(reader.GetOrdinal("datecreated"));
                    int faqCategoryID = reader.GetInt32(reader.GetOrdinal("faqcategoryid"));
                    string catName = reader.GetString(reader.GetOrdinal("category"));
                    tmpFaq = new cFaq(faqID, faqCategoryID, nAccountID, question, answer, tip, dateCreated, catName, true);
                    listGlobalFaqs.Add(faqID, tmpFaq);
                }

                reader.Close();
            }

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                Cache.Insert("global_faqs", listGlobalFaqs, dependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Permanent), CacheItemPriority.Default, null);
            }

            return listGlobalFaqs;
        }

        public System.Collections.Generic.SortedList<int, cFaq> CacheCustomerList()
        {
            listCustomerFaqs = new System.Collections.Generic.SortedList<int, cFaq>();
            string strSQL = "SELECT faqid, question, answer, tip, datecreated, faqcategoryid FROM dbo.faqs";
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            expdata.sqlexecute.CommandText = strSQL;
            SqlCacheDependency dep = null;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                dep = new SqlCacheDependency(expdata.sqlexecute);
            }

            strSQL = "SELECT faqs.faqid, faqs.question, faqs.answer, faqs.tip, faqs.datecreated, faqs.faqcategoryid, faqcategories.category FROM faqs, faqcategories WHERE faqcategories.faqcategoryid=faqs.faqcategoryid";
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                cFaq tmpFaq;

                while (reader.Read())
                {
                    int faqID = reader.GetInt32(reader.GetOrdinal("faqid"));
                    string question = reader.GetString(reader.GetOrdinal("question"));
                    string answer = reader.GetString(reader.GetOrdinal("answer"));
                    string tip = "";

                    if (reader.IsDBNull(reader.GetOrdinal("tip")))
                    {
                        tip = "";
                    }
                    else
                    {
                        tip = reader.GetString(reader.GetOrdinal("tip"));
                    }

                    DateTime dateCreated = reader.GetDateTime(reader.GetOrdinal("datecreated"));
                    int faqCategoryID = reader.GetInt32(reader.GetOrdinal("faqcategoryid"));
                    string catName = reader.GetString(reader.GetOrdinal("category"));
                    tmpFaq = new cFaq(faqID, faqCategoryID, nAccountID, question, answer, tip, dateCreated, catName, false);
                    listCustomerFaqs.Add(faqID, tmpFaq);
                }

                reader.Close();
            }

            if (dep != null)
            {
                Cache.Insert("customer_faqs_" + nAccountID, listCustomerFaqs, dep,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Medium),
                    CacheItemPriority.Default, null);
            }

            return listGlobalFaqs;
        }

        public cFaq getFaqById(int faqID)
        {
            cFaq reqFaq;

            if (listGlobalFaqs.ContainsKey(faqID))
            {
                reqFaq = (cFaq)listGlobalFaqs[faqID];
            }
            else
            {
                reqFaq = (cFaq)listCustomerFaqs[faqID];
            }

            return reqFaq;
        }

        public System.Collections.Generic.SortedList<string, cFaq> getList()
        {
            //System.Collections.Generic.SortedList<int, cFaq> lstGlobalFaqs = (System.Collections.Generic.SortedList<int, cFaq>)Cache["global_faqs"];
            //System.Collections.Generic.SortedList<int, cFaq> lstCustomerFaqs = (System.Collections.Generic.SortedList<int, cFaq>)Cache["customer_faqs_" + nAccountID];

            System.Collections.Generic.SortedList<string, cFaq> lstAllFaqs = new System.Collections.Generic.SortedList<string, cFaq>();

            cFaq tmpFaq;

            int i = 0;
            for (i = 0; i < listGlobalFaqs.Count; i++)
            {
                tmpFaq = (cFaq)listGlobalFaqs.Values[i];
                lstAllFaqs.Add(tmpFaq.CategoryName + "_" + tmpFaq.question, tmpFaq);
            }

            for (i = 0; i < listCustomerFaqs.Count; i++)
            {
                tmpFaq = (cFaq)listCustomerFaqs.Values[i];
                lstAllFaqs.Add(tmpFaq.CategoryName + "_" + tmpFaq.question, tmpFaq);
            }

            return lstAllFaqs;
        }

        private bool alreadyExists(int faqid, int categoryid, string question)
        {
            foreach (cFaq faq in listCustomerFaqs.Values)
            {
                if (faqid > 0)
                {
                    if (faq.faqcategoryid == categoryid && faq.question.ToLower() == question.ToLower() && faq.faqid != faqid)
                    {
                        return true;
                    }
                }
                else
                {
                    if (faq.faqcategoryid == categoryid && faq.question.ToLower() == question.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public int addFaq(string question, string answer, string tip, int faqCategoryID)
        {
            if (alreadyExists(0, faqCategoryID, question))
            {
                return -1;
            }
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cAuditLog clsaudit = new cAuditLog();

            expdata.sqlexecute.Parameters.AddWithValue("@faqcategoryid", faqCategoryID);
            string strsql = "insert into faqs (question, answer, tip, faqcategoryid) values (@question,@answer,@tip, @faqcategoryid);select @identity = scope_identity()";

            if (question.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@question", question.Substring(0, 3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@question", question);
            }
            if (answer.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@answer", answer.Substring(0, 3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@answer", answer);
            }
            if (tip.Length > 200)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tip", tip.Substring(0, 199));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tip", tip);
            }
            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            int faqid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();
            InitialiseData();
            if (question.Length > 2000)
            {
                clsaudit.addRecord(SpendManagementElement.FAQS, question.Substring(0, 1999), faqid);
            }
            else
            {
                clsaudit.addRecord(SpendManagementElement.FAQS, question, faqid);
            }
            InitialiseData();
            return 0;
        }

        public int updateFaq(int faqID, string question, string answer, string tip, int faqCategoryID)
        {
            if (alreadyExists(faqID, faqCategoryID, question))
            {
                return -1;
            }
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cAuditLog clsaudit = new cAuditLog();
            cFaq reqfaq = getFaqById(faqID);
            string strsql = "update faqs set question = @question, answer = @answer, tip = @tip, faqcategoryid = @faqcategoryid where faqid = @faqid";
            if (question.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@question", question.Substring(0, 3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@question", question);
            }
            if (answer.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@answer", answer.Substring(0, 3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@answer", answer);
            }
            if (tip.Length > 200)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tip", tip.Substring(0, 199));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tip", tip);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@faqcategoryid", faqCategoryID);
            expdata.sqlexecute.Parameters.AddWithValue("@faqid", faqID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            #region auditlog
            if (reqfaq.question != question)
            {
                if (question.Length > 2000)
                {
                    clsaudit.editRecord(faqID, question, SpendManagementElement.FAQS, new Guid("af7e2d8a-0fd4-4d51-88a2-74d12d40a12a"), reqfaq.question, question.Substring(0, 1999));
                }
                else
                {
                    clsaudit.editRecord(faqID, question, SpendManagementElement.FAQS, new Guid("af7e2d8a-0fd4-4d51-88a2-74d12d40a12a"), reqfaq.question, question);
                }
            }
            if (reqfaq.answer != answer)
            {
                if (answer.Length > 2000)
                {
                    clsaudit.editRecord(faqID, question, SpendManagementElement.FAQS, new Guid("9f9d1d43-d7a6-4006-862e-b9926f2606ed"), reqfaq.answer, answer.Substring(0, 1999));
                }
                else
                {
                    clsaudit.editRecord(faqID, question, SpendManagementElement.FAQS, new Guid("9f9d1d43-d7a6-4006-862e-b9926f2606ed"), reqfaq.answer, answer);
                }
            }
            if (reqfaq.tip != tip)
            {
                clsaudit.editRecord(faqID, question, SpendManagementElement.FAQS, new Guid("e723d943-4a57-42df-8726-75e364864a41"), reqfaq.tip, tip);
            }
            #endregion

            InitialiseData();
            return 0;
        }

        public void deleteFaq(int faqID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cAuditLog clsaudit = new cAuditLog();
            cFaq reqfaq = getFaqById(faqID);
            expdata.sqlexecute.Parameters.AddWithValue("@faqid", faqID);
            string strsql = "delete from faqs where faqid = @faqid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if (reqfaq.question.Length > 2000)
            {
                clsaudit.deleteRecord(SpendManagementElement.FAQS, faqID, reqfaq.question.Substring(0, 2000));
            }
            else
            {
                clsaudit.deleteRecord(SpendManagementElement.FAQS, faqID, reqfaq.question);
            }
            InitialiseData();
        }

        public System.Data.DataTable getGrid(bool global)
        {
            cFaq reqfaq;
            int i = 0;
            object[] values;
            System.Data.DataTable faqs = new System.Data.DataTable();
            faqs.Columns.Add("faqid", System.Type.GetType("System.Int32"));
            faqs.Columns.Add("question", System.Type.GetType("System.String"));
            faqs.Columns.Add("datecreated", System.Type.GetType("System.DateTime"));

            System.Collections.Generic.SortedList<string, cFaq> listAllFaqs = getList();

            for (i = 0; i < listAllFaqs.Count; i++)
            {
                reqfaq = (cFaq)listAllFaqs.Values[i];
                if ((global == false && reqfaq.Global == false) || global == true)
                {
                    values = new object[3];
                    values[0] = reqfaq.faqid;
                    values[1] = reqfaq.question;
                    values[2] = reqfaq.datecreated;
                    faqs.Rows.Add(values);
                }
            }

            return faqs;
        }

        public bool categoryExists(int faqCategoryID)
        {
            int i = 0;
            cFaq tmpFaq;

            for (i = 0; i < listCustomerFaqs.Count; i++)
            {
                tmpFaq = (cFaq)listCustomerFaqs.Values[i];
                if (tmpFaq.faqcategoryid == faqCategoryID)
                {
                    return true;
                }
            }

            return false;

        }

    }

    public class cFaq
    {
        private int nFaqid;
        private int nFaqcategoryid;
        private int nAccountid;
        private string sQuestion;
        private string sAnswer;
        private string sTip;
        private DateTime dtDateCreated;
        private string sCategoryName;
        private bool bGlobalFaq;

        public cFaq(int faqid, int faqcategoryid, int accountid, string question, string answer, string tip, DateTime datecreated, bool global)
        {
            nFaqid = faqid;
            nFaqcategoryid = faqcategoryid;
            nAccountid = accountid;
            sQuestion = question;
            sAnswer = answer;
            sTip = tip;
            dtDateCreated = datecreated;
            bGlobalFaq = global;
        }

        public cFaq(int faqid, int faqcategoryid, int accountid, string question, string answer, string tip, DateTime datecreated, string categoryName, bool global)
        {
            nFaqid = faqid;
            nFaqcategoryid = faqcategoryid;
            nAccountid = accountid;
            sQuestion = question;
            sAnswer = answer;
            sTip = tip;
            dtDateCreated = datecreated;
            sCategoryName = categoryName;
            bGlobalFaq = global;
        }

        #region Properties

        public int faqid
        {
            get { return nFaqid; }
        }

        public string CategoryName
        {
            get { return sCategoryName; }
            set { sCategoryName = value; }
        }

        public int faqcategoryid
        {
            get { return nFaqcategoryid; }
        }
        public int accountid
        {
            get { return nAccountid; }
        }
        public string question
        {
            get { return sQuestion; }
        }
        public string answer
        {
            get { return sAnswer; }
        }
        public string tip
        {
            get { return sTip; }
        }
        public DateTime datecreated
        {
            get { return dtDateCreated; }
        }

        public bool Global
        {
            get { return bGlobalFaq; }
        }

        #endregion
    }














































    public class cFaqCategory
    {
        private int nFaqcategoryid;
        private int nAccountid;
        private string sCategory;
        private bool bGlobal;

        public cFaqCategory(int faqcategoryid, int accountid, string category, bool global)
        {
            nFaqcategoryid = faqcategoryid;
            nAccountid = accountid;
            sCategory = category;
            bGlobal = global;
        }

        public int faqcategoryid
        {
            get { return nFaqcategoryid; }
        }

        public int accountid
        {
            get { return nAccountid; }
        }

        public string category
        {
            get { return sCategory; }
        }

        public bool Global
        {
            get { return bGlobal; }
        }
    }



    public class cFaqCategories
    {
        private System.Web.Caching.Cache Cache = System.Web.HttpContext.Current.Cache;
        private int nAccountID;
        private System.Collections.Generic.SortedList<int, cFaqCategory> listGlobalFaqCategories;
        private System.Collections.Generic.SortedList<int, cFaqCategory> listCustomerFaqCategories;

        public cFaqCategories(int accountID)
        {
            nAccountID = accountID;
            InitializeData();
        }

        public void InitializeData()
        {
            listGlobalFaqCategories = (System.Collections.Generic.SortedList<int, cFaqCategory>)Cache["globalfaqcategories"];
            listCustomerFaqCategories = (System.Collections.Generic.SortedList<int, cFaqCategory>)Cache["customerfaqcategories_" + nAccountID];

            if (listGlobalFaqCategories == null)
            {
                CacheGlobalCategories();
            }

            if (listCustomerFaqCategories == null)
            {
                CacheCustomerCategories();
            }
        }

        public void CacheGlobalCategories()
        {
            listGlobalFaqCategories = new System.Collections.Generic.SortedList<int, cFaqCategory>();

            string strSQL = "SELECT faqcategoryid, category FROM dbo.global_faqcategories";

            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expdata.sqlexecute.CommandText = strSQL;
            
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("globalfaqcategories", listGlobalFaqCategories, dep,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Permanent),
                    CacheItemPriority.Default, null);
            }

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                int FaqCategoryID;
                string Category;

                cFaqCategory tmpCat;

                while (reader.Read())
                {
                    FaqCategoryID = reader.GetInt32(reader.GetOrdinal("faqcategoryid"));
                    Category = reader.GetString(reader.GetOrdinal("category"));

                    tmpCat = new cFaqCategory(FaqCategoryID, nAccountID, Category, true);
                    listGlobalFaqCategories.Add(FaqCategoryID, tmpCat);
                }

                reader.Close();
            }
        }

        public void CacheCustomerCategories()
        {
            listCustomerFaqCategories = new System.Collections.Generic.SortedList<int, cFaqCategory>();

            string strSQL = "SELECT faqcategoryid, category FROM dbo.faqcategories";

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            expdata.sqlexecute.CommandText = strSQL;
            
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("customerfaqcategories_" + nAccountID, listCustomerFaqCategories, dep,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Permanent),
                    CacheItemPriority.Default, null);
            }

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                int FaqCategoryID;
                string Category;

                cFaqCategory tmpCat;

                while (reader.Read())
                {
                    FaqCategoryID = reader.GetInt32(reader.GetOrdinal("faqcategoryid"));
                    Category = reader.GetString(reader.GetOrdinal("category"));

                    tmpCat = new cFaqCategory(FaqCategoryID, nAccountID, Category, false);
                    listCustomerFaqCategories.Add(FaqCategoryID, tmpCat);
                }

                reader.Close();
            }

        }

        public System.Collections.Generic.SortedList<string, cFaqCategory> getList()
        {
            System.Collections.Generic.SortedList<string, cFaqCategory> listCategories = new System.Collections.Generic.SortedList<string, cFaqCategory>();

            int i = 0;
            cFaqCategory tmpCat;
            for (i = 0; i < listGlobalFaqCategories.Count; i++)
            {
                tmpCat = (cFaqCategory)listGlobalFaqCategories.Values[i];
                listCategories.Add(tmpCat.category, tmpCat);
            }

            for (i = 0; i < listCustomerFaqCategories.Count; i++)
            {
                tmpCat = (cFaqCategory)listCustomerFaqCategories.Values[i];

                if (!listCategories.ContainsKey(tmpCat.category))
                {
                    listCategories.Add(tmpCat.category, tmpCat);
                }
            }

            return listCategories;
        }


        public int itemCount
        {
            get { return listCustomerFaqCategories.Count + listGlobalFaqCategories.Count; }
        }

        public void CreateDropDown(ref System.Web.UI.WebControls.DropDownList lst, bool global)
        {
            System.Collections.Generic.SortedList<string, cFaqCategory> listAllCategories = getList();// new System.Collections.Generic.SortedList<int, cFaqCategory>();
            int i;
            cFaqCategory reqfaqcat;

            System.Web.UI.WebControls.ListItem item;
            for (i = 0; i < listAllCategories.Count; i++)
            {
                reqfaqcat = (cFaqCategory)listAllCategories.Values[i];
                if ((global == false && reqfaqcat.Global == false) || global == true)
                {
                    item = new System.Web.UI.WebControls.ListItem(reqfaqcat.category, reqfaqcat.faqcategoryid.ToString());
                    lst.Items.Add(item);
                }
            }
        }

        private bool alreadyExists(string category, int categoryID, int action)
        {
            cFaqCategory reqCat;
            int i;

            for (i = 0; i < listCustomerFaqCategories.Count; i++)
            {
                reqCat = (cFaqCategory)listCustomerFaqCategories.Values[i];
                if (action == 2)
                {
                    if (reqCat.category.ToLower() == category.ToLower() && reqCat.faqcategoryid != categoryID)
                    {
                        return true;
                    }
                }
                else
                {
                    if (reqCat.category.ToLower() == category.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public byte addCategory(string category)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cAuditLog clsaudit = new cAuditLog();
            if (alreadyExists(category, 0, 0) == true)
            {
                return 1;
            }

            string strsql = "insert into faqcategories (category) values (@category);select @identity = scope_identity()";
            expdata.sqlexecute.Parameters.AddWithValue("@category", category);
            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            int categoryid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            clsaudit.addRecord(SpendManagementElement.FAQS, category, categoryid);

            InitializeData();
            return 0;
        }

        public byte updateCategory(int faqcategoryid, string category)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));

            cAuditLog clsaudit = new cAuditLog();
            cFaqCategory reqcat = getFaqCategoryById(faqcategoryid, false);
            if (alreadyExists(category, faqcategoryid, 2) == true)
            {
                return 1;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@faqcategoryid", faqcategoryid);
            string strsql = "update faqcategories set category = @category where faqcategoryid = @faqcategoryid";
            expdata.sqlexecute.Parameters.AddWithValue("@category", category);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (reqcat.category != category)
            {
                clsaudit.editRecord(faqcategoryid, category, SpendManagementElement.FAQS, new Guid("07144ecd-174e-4bda-8258-f1be3ac36f0e"), reqcat.category, category);
            }

            InitializeData();
            return 0;
        }

        public bool deleteCategory(int faqcategoryid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));

            cAuditLog clsaudit = new cAuditLog();
            cFaqCategory reqcat = getFaqCategoryById(faqcategoryid, false);
            cFaqs clsfaqs = new cFaqs(nAccountID);
            if (clsfaqs.categoryExists(faqcategoryid) == true)
            {
                return false;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@faqcategoryid", faqcategoryid);
            string strsql = "delete from faqcategories where faqcategoryid = @faqcategoryid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            clsaudit.deleteRecord(SpendManagementElement.FAQS, faqcategoryid, reqcat.category);
            InitializeData();
            return true;
        }

        public cFaqCategory getFaqCategoryById(int faqcategoryid, bool global)
        {
            if (global == true)
            {
                return (cFaqCategory)listGlobalFaqCategories[faqcategoryid];
            }
            else
            {
                return (cFaqCategory)listCustomerFaqCategories[faqcategoryid];
            }
        }

        public System.Data.DataTable getGrid(bool global)
        {
            System.Collections.Generic.SortedList<string, cFaqCategory> listAllCats = getList();
            System.Data.DataTable table = new System.Data.DataTable();
            int i;
            object[] values;
            cFaqCategory reqfaqcat;

            table.Columns.Add("faqcategoryid", System.Type.GetType("System.Int32"));
            table.Columns.Add("category", System.Type.GetType("System.String"));

            for (i = 0; i < listAllCats.Count; i++)
            {
                reqfaqcat = (cFaqCategory)listAllCats.Values[i];
                if ((global == false && reqfaqcat.Global == false) || global == true)
                {
                    values = new object[2];
                    values[0] = reqfaqcat.faqcategoryid;
                    values[1] = reqfaqcat.category;
                    table.Rows.Add(values);
                }
            }

            return table;
        }
    }
}
