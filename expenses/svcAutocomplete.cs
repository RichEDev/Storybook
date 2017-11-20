using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections.Specialized;
using expenses;
using AjaxControlToolkit;
using expenses.Old_App_Code;
using ExpensesLibrary;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Text;
using System.Data;
using SpendManagementLibrary;
using Spend_Management;

/// <summary>
/// Summary description for svcAutocomplete
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService()]
public class svcAutocomplete : System.Web.Services.WebService
{
    DBConnection expdata;
    string strsql;

    public svcAutocomplete()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public List<string> getCompanyList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;

        cCompanies clscomps = new cCompanies(user.accountid);
        cEmployees clsemp = new cEmployees(user.accountid);
        cEmployee reqemp = clsemp.GetEmployeeById(user.employeeid);

        if (prefixText.ToLower().StartsWith("h"))
        {
            cCompany home = clscomps.GetCompanyById(reqemp.homelocationid);

            if (home != null)
            {
                items.Add("Home");
            }
        }

        if (prefixText.ToLower().StartsWith("o"))
        {
            cCompany office = clscomps.GetCompanyById(reqemp.officelocationid);

            if (office != null)
            {
                items.Add("Office");
            }
        }

        strsql = "select top 10 company from companies where archived = 0 and showto = 1 and company like @company order by company";
        
        expdata.sqlexecute.Parameters.AddWithValue("@company", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            if ((reader.GetString(0) == "Home" && items.Contains("Home")) || (reader.GetString(0) == "Office" && items.Contains("Office")))
            {

            }
            else
            {
                items.Add(reader.GetString(0));
            }
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getCompanies(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);


        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 company from companies where archived = 0 and company like @company order by company";

        expdata.sqlexecute.Parameters.AddWithValue("@company", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getEmployeeUsername(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        string strsql;
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        SqlDataReader reader;
        strsql = "select top 10 username from employees where username like @username order by username";
        expdata.sqlexecute.Parameters.AddWithValue("@username", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;

    }

    [WebMethod]
    public List<string> getEmployee(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        string strsql;
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        SqlDataReader reader;
        strsql = "select top 10 surname, firstname from employees where surname like @surname order by surname";
        expdata.sqlexecute.Parameters.AddWithValue("@surname", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0) + ", " + reader.GetString(1));
        }
        reader.Close();
        return items;

    }

    [WebMethod]
    public List<string> getEmployeeNameAndUsername(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        string strsql;
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        SqlDataReader reader;
        strsql = "select top 10 surname, firstname, username from employees where surname like @surname order by surname";
        expdata.sqlexecute.Parameters.AddWithValue("@surname", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0) + ", " + reader.GetString(1) + " [" + reader.GetString(reader.GetOrdinal("username")) + "]");
        }
        reader.Close();
        return items;

    }

    [WebMethod]
    public List<string> getParentCompanyList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;

        cCompanies clscomps = new cCompanies(user.accountid);
        cEmployees clsemp = new cEmployees(user.accountid);
        cEmployee reqemp = clsemp.GetEmployeeById(user.employeeid);

        if (prefixText.ToLower().StartsWith("h"))
        {
            cCompany home = clscomps.GetCompanyById(reqemp.homelocationid);

            if (home != null)
            {
                items.Add("Home");
            }
        }

        if (prefixText.ToLower().StartsWith("o"))
        {
            cCompany office = clscomps.GetCompanyById(reqemp.officelocationid);

            if (office != null)
            {
                items.Add("Office");
            }
        }

        strsql = "select top 10 company from companies where archived = 0 and isCompany = 1 and company like @company order by company";

        expdata.sqlexecute.Parameters.AddWithValue("@company", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            if ((reader.GetString(0) == "Home" && items.Contains("Home")) || (reader.GetString(0) == "Office" && items.Contains("Office")))
            {

            }
            else
            {
                items.Add(reader.GetString(0));
            }
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getFromList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;

        cCompanies clscomps = new cCompanies(user.accountid);
        cEmployees clsemp = new cEmployees(user.accountid);
        cEmployee reqemp = clsemp.GetEmployeeById(user.employeeid);

        if (prefixText.ToLower().StartsWith("h"))
        {
            cCompany home = clscomps.GetCompanyById(reqemp.homelocationid);

            if (home != null)
            {
                items.Add("Home");
            }
        }

        if (prefixText.ToLower().StartsWith("o"))
        {
            cCompany office = clscomps.GetCompanyById(reqemp.officelocationid);

            if (office != null)
            {
                items.Add("Office");
            }
        }
        
        strsql = "select top 10 company from companies where archived = 0 and showfrom = 1 and company like @company order by company";
        
        expdata.sqlexecute.Parameters.AddWithValue("@company", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            if ((reader.GetString(0) == "Home" && items.Contains("Home")) || (reader.GetString(0) == "Office" && items.Contains("Office")))
            {
                
            }
            else
            {
                items.Add(reader.GetString(0));
            }
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getDepartmentList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        
        
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 department from departments where department like @department AND archived = 0 order by department";
        
        expdata.sqlexecute.Parameters.AddWithValue("@department", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getDepartmentListByDescription(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 description from departments where description like @department order by description";
        
        expdata.sqlexecute.Parameters.AddWithValue("@department", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getCostcodeList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
    
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 costcode from costcodes where costcode like @costcode AND archived = 0 order by costcode";
        
        expdata.sqlexecute.Parameters.AddWithValue("@costcode", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getCostcodeListByDescription(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
       
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 description from costcodes where description like @costcode order by description";
        
        expdata.sqlexecute.Parameters.AddWithValue("@costcode", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getProjectcodeList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
       
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 projectcode from projectcodes where projectcode like @projectcode AND archived = 0 order by projectcode";
        
        expdata.sqlexecute.Parameters.AddWithValue("@projectcode", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getReasonList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 reason from reasons where reason like @reason order by reason";

        expdata.sqlexecute.Parameters.AddWithValue("@reason", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getProjectcodeListByDescription(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 description from projectcodes where description like @projectcode order by description";
        
        expdata.sqlexecute.Parameters.AddWithValue("@projectcode", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public List<string> getHotelList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 hotelname + ', ' + city from hotels where hotelname like @hotelname order by hotelname";
        
        expdata.sqlexecute.Parameters.AddWithValue("@hotelname", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }
    [WebMethod]
    public CascadingDropDownNameValue[] getCategoryDropdown(string knownCategoryValues, string category)
    {

        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cEmployees clsemployees = new cEmployees(user.accountid);
        cEmployee reqemp = clsemployees.GetEmployeeById(user.employeeid);
        expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        switch (category)
        {
            case "category":
                return getCategoryList(clsemployees.getResultantRoleSet(reqemp.employeeid)).ToArray();
            case "subcat":
                int categoryid;
                StringDictionary kv =CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
                categoryid = int.Parse(kv["category"]);
                return getSubcatList(categoryid, clsemployees.getResultantRoleSet(reqemp.employeeid)).ToArray();
                
        }
        return null;
    }

    private List<CascadingDropDownNameValue> getCategoryList(Dictionary<int, cRoleSubcat> roleitems)
    {
        List<CascadingDropDownNameValue> cats = new List<CascadingDropDownNameValue>();
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cCategories clscategories = new cCategories(user.accountid);
        cCategory category;
        foreach (cRoleSubcat rolesub in roleitems.Values)
        {
            category = clscategories.FindById(rolesub.subcat.categoryid);
            if (cats.Contains(new CascadingDropDownNameValue(category.category, category.categoryid.ToString())) == false)
            {
                cats.Add(new CascadingDropDownNameValue(category.category,category.categoryid.ToString()));
            }
        }

        return cats;
    }

    private List<CascadingDropDownNameValue> getSubcatList(int categoryid, Dictionary<int, cRoleSubcat> roleitems)
    {
        List<CascadingDropDownNameValue> subcats = new List<CascadingDropDownNameValue>();

        SortedList<string, cRoleSubcat> sorted = new SortedList<string, cRoleSubcat>();
        foreach (cRoleSubcat rolesub in roleitems.Values)
        {
            if (rolesub.subcat.categoryid == categoryid)
            {
                sorted.Add(rolesub.subcat.subcat, rolesub);
            }
        }

        foreach (cRoleSubcat rolesub in sorted.Values)
        {

            subcats.Add(new CascadingDropDownNameValue(rolesub.subcat.subcat, rolesub.subcat.subcatid.ToString()));

        }

        return subcats;
    }

    [WebMethod]
    public CascadingDropDownNameValue[] getMileageCategoriesByCar(string knownCategoryValues, string category, string contextKey)
    {
        CurrentUser user = cMisc.getCurrentUser(contextKey);
        StringDictionary kv = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
        int carid = int.Parse(kv["undefined"]);
        cEmployees clsemployees = new cEmployees(user.accountid);
        cEmployee reqemp = clsemployees.GetEmployeeById(user.employeeid);
        cMileagecats clsmileage = new cMileagecats(user.accountid);
        cMileageCat mileagecat;
        cCar car = reqemp.getCarById(carid);

        List<CascadingDropDownNameValue> mileagecats = new List<CascadingDropDownNameValue>();
        foreach (int i in car.mileagecats)
        {
            mileagecat = clsmileage.GetMileageCatById(i);
            mileagecats.Add(new CascadingDropDownNameValue(mileagecat.carsize, mileagecat.mileageid.ToString()));
        }
        return mileagecats.ToArray();
    }

    [WebMethod]
    public List<string> getCountryList(string prefixText, int count)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);


        expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
        List<string> items = new List<string>();
        System.Data.SqlClient.SqlDataReader reader;
        strsql = "select top 10 country from global_countries where country like @country order by country";

        expdata.sqlexecute.Parameters.AddWithValue("@country", prefixText + "%");
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }
        reader.Close();
        return items;
    }

    [WebMethod]
    public object[] checkLocation(string location, int nCompanyType, string nameboxid, string valboxid, string popupid, string tabsid, string target)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        object[] results = new object[9];
        CompanyType companytype = (CompanyType)nCompanyType;
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.accountid));
        cCompanies clscomps = new cCompanies(user.accountid);
        cEmployees clsemp = new cEmployees(user.accountid);
        cEmployee reqemp = clsemp.GetEmployeeById(user.employeeid);
        cCompany home = null;
        cCompany office = null;

        if (location.ToLower() == "home")
        {
            home = clscomps.GetCompanyById(reqemp.homelocationid);

            if (home != null)
            {
                results[0] = 1;
                results[1] = home.companyid;
                results[2] = "Home";
            }
        }

        if (location.ToLower() == "office")
        {
            office = clscomps.GetCompanyById(reqemp.officelocationid);

            if (office != null)
            {
                results[0] = 1;
                results[1] = office.companyid;
                results[2] = "Office";
            }
        }

        if (home == null && office == null)
        {
            int count;
            strsql = "select count(company) from companies where (company like @company or postcode like @company)";
            switch (companytype)
            {
                case CompanyType.From:
                    strsql += " and showfrom = 1";
                    break;
                case CompanyType.To:
                    strsql += " and showto = 1";
                    break;
                case CompanyType.Company:
                    strsql += " and iscompany = 1";
                    break;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@company", location + "%");
            count = expdata.getcount(strsql);


            switch (count)
            {
                case 0: //company doesn't exist, need to add
                    results[0] = 0;
                    //is it a postcode?
                    string[] address = getAddress("united kingdom", location);
                    if (address.GetLength(0) > 0)
                    {
                        results[1] = 1;
                    }
                    else
                    {
                        results[1] = 0;
                    }
                    break;
                case 1: //1 company matches so return that
                    SqlDataReader reader;
                    results[0] = 1;
                    strsql = "select companyid, company from companies where (company like @company or postcode like @company)";
                    switch (companytype)
                    {
                        case CompanyType.From:
                            strsql += " and showfrom = 1";
                            break;
                        case CompanyType.To:
                            strsql += " and showto = 1";
                            break;
                        case CompanyType.Company:
                            strsql += " and iscompany = 1";
                            break;
                    }
                    reader = expdata.GetReader(strsql);
                    while (reader.Read())
                    {
                        results[1] = reader.GetInt32(0);
                        results[2] = reader.GetString(1);
                    }
                    reader.Close();
                    break;
                default: //more than 1 company, show search box
                    results[0] = 2;
                    break;
            }
        }
        
        results[3] = nameboxid;
        results[4] = valboxid;
        results[5] = popupid;
        results[6] = companytype;
        results[7] = tabsid;
        results[8] = target;
        expdata.sqlexecute.Parameters.Clear();
        return results;
    }


    [WebMethod]
    [ScriptMethod]
    public string getLast10Locations(string contextKey)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        cCompanies clscompanies = new cCompanies(user.accountid);
        StringBuilder output = new StringBuilder();
        DataTable tbl = null;

        CompanyType companytype = (CompanyType)Convert.ToInt32(contextKey);
        switch (companytype)
        {
            case CompanyType.Company:
                tbl = clscompanies.getLast10CompanyLocationsByEmployeeid(user.employeeid);

                break;
            case CompanyType.To:

                tbl = clscompanies.getLast10ToLocationsByEmployeeid(user.employeeid);
                
                break;
            case CompanyType.From:
                tbl = clscompanies.getLast10FromLocationsByEmployeeid(user.employeeid);
                
                break;
        }

        output.Append("<table>");
        if (tbl != null)
        {
            foreach (DataRow row in tbl.Rows)
            {
                output.Append("<tr><td><a href=\"javascript:selectLocation(" + (int)row["companyid"] + ",'" + (string)row["company"].ToString().Replace("'", "\\'") + "');\">" + (string)row["company"] + "</a></td></tr>");
            }
            output.Append("</table>");
        }

        return output.ToString();
    }

    [WebMethod]
    [ScriptMethod]
    public string getTop10Locations(string contextKey)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        cCompanies clscompanies = new cCompanies(user.accountid);
        StringBuilder output = new StringBuilder();
        DataTable tbl = null;

        CompanyType companytype = (CompanyType)Convert.ToInt32(contextKey);
        switch (companytype)
        {
            case CompanyType.Company:
                tbl = clscompanies.getTop10CompanyLocationsByEmployeeid(user.employeeid);

                break;
            case CompanyType.To:

                tbl = clscompanies.getTop10ToLocationsByEmployeeid(user.employeeid);

                break;
            case CompanyType.From:
                tbl = clscompanies.getTop10FromLocationsByEmployeeid(user.employeeid);

                break;
        }

        output.Append("<table>");
        if (tbl != null)
        {
            foreach (DataRow row in tbl.Rows)
            {
                output.Append("<tr><td><a href=\"javascript:selectLocation(" + (int)row["companyid"] + ",'" + (string)row["company"].ToString().Replace("'", "\\'") + "');\">" + (string)row["company"] + "</a></td></tr>");
            }
            output.Append("</table>");
        }

        return output.ToString();
    }

    [WebMethod]
    [ScriptMethod]
    public string getLocations(string contextKey)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        string[] data = contextKey.Split(',');
        string company = data[0];
        CompanyType companytype = (CompanyType)Convert.ToInt32(data[1]);
        cCompanies clscompanies = new cCompanies(user.accountid);
        StringBuilder output = new StringBuilder();
        DataTable tbl = null;


        tbl = clscompanies.searchForLocations(company, companytype);

        output.Append("<table>");
        if (tbl != null)
        {
            foreach (DataRow row in tbl.Rows)
            {
                output.Append("<tr><td><a href=\"javascript:selectLocation(" + (int)row["companyid"] + ",'" + (string)row["company"].ToString().Replace("'", "\\'") + "');\">" + (string)row["company"] + "</a></td></tr>");
            }
            output.Append("</table>");
        }

        return output.ToString();
    }

    public object[] searchForLocationAutoLog(string name, string address1, string address2, string postcode)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cCompanies clsCompanies = new cCompanies(user.accountid);
        DataTable tbl = null;
        object[] data = new object[8];

        tbl = clsCompanies.searchForAutoLogLocation(name, address1, address1, postcode);
        if (tbl.Rows.Count == 1)
        {
            data[0] = tbl.Rows[0]["companyid"];
            data[1] = tbl.Rows[0]["company"];
            data[2] = tbl.Rows[0]["address1"];
            data[3] = tbl.Rows[0]["address2"];
            data[4] = tbl.Rows[0]["city"];
            data[5] = tbl.Rows[0]["county"];
            data[6] = tbl.Rows[0]["postcode"];
            data[7] = tbl.Rows[0]["country"];

            return data;
        }
        else
        {
            return null;
        }
    }

    [WebMethod]
    [ScriptMethod]
    public string searchForLocationsAutoLog(string name, string address1, string address2, string postcode) 
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cCompanies clsCompanies = new cCompanies(user.accountid);
        DataTable tbl = null;

        tbl = clsCompanies.searchForAutoLogLocation(name, address1, address1, postcode);

        StringBuilder sbOutput = new StringBuilder();

        if (tbl != null)
        {
            sbOutput.Append("<table>");

            foreach (DataRow row in tbl.Rows)
            {

                sbOutput.Append("<tr><td><a href=\"javascript:selectLocation(" + (int)row["companyid"] + ",'" + (string)row["company"].ToString().Replace("'", "\\'") + "');\">" + (string)row["company"] + "</a></td></tr>");

            }

            sbOutput.Append("</table>");
        }

        return sbOutput.ToString();
    }

    [WebMethod]
    [ScriptMethod]
    public string searchForLocations(int ncompanytype, string name, string address1, string address2, string city, string county, string postcode, string country)
    {
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);

        
        int ncountry = 0;
        CompanyType companytype = (CompanyType)ncompanytype;
        cCompanies clscompanies = new cCompanies(user.accountid);
        StringBuilder output = new StringBuilder();
        DataTable tbl = null;
        bool homeExists = false;
        bool officeExists = false;
        cCompany home = null;
        cCompany office = null;

        cGlobalCountries clscountries = new cGlobalCountries();
        cGlobalCountry clscountry = clscountries.getCountryByName(country);
        if (clscountry != null)
        {
            ncountry = clscountry.globalcountryid;
        }

        tbl = clscompanies.searchForLocations(name, address1, address2, city, county, postcode, ncountry, companytype);

        output.Append("<table>");
        if (tbl != null)
        {
            foreach (DataRow row in tbl.Rows)
            {
                if ((string)row["company"] == "Home")
                {
                    homeExists = true;
                }

                if ((string)row["company"] == "Office")
                {
                    officeExists = true;
                }
            }
            
            cEmployees clsemp = new cEmployees(user.accountid);
            cEmployee reqemp = clsemp.GetEmployeeById(user.employeeid);

            if (!homeExists)
            {
                home = clscompanies.GetCompanyById(reqemp.homelocationid);

                if (home != null)
                {
                    tbl.Rows.Add(home.companyid, "Home");
                }
            }

            if (!officeExists)
            {
                office = clscompanies.GetCompanyById(reqemp.officelocationid);

                if (office != null)
                {
                    tbl.Rows.Add(office.companyid, "Office");
                }
            }

            foreach (DataRow row in tbl.Rows)
            {
                output.Append("<tr><td><a href=\"javascript:selectLocation(" + (int)row["companyid"] + ",'" + (string)row["company"].ToString().Replace("'", "\\'") + "');\">" + (string)row["company"] + "</a></td></tr>");
            }
            output.Append("</table>");
        }

        return output.ToString();
    }

    [WebMethod]
    public object[] addLocation(string name, string address1, string address2, string city, string county, string postcode, string country, int ncompanytype)
    {
        object[] data = new object[3];
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cCompanies clscompanies = new cCompanies(user.accountid);
        cCompany company = clscompanies.getCompanyFromName(name.Trim().ToLower());
        bool showfrom = false;
        bool showto = false;
        bool iscompany = false;
        int companyid;
        int ncountry = 0;
        cGlobalCountries clscountries = new cGlobalCountries();
        cGlobalCountry clscountry = clscountries.getCountryByName(country);
        if (clscountry != null)
        {
            ncountry = clscountry.globalcountryid;
        }
        
        CompanyType companytype = (CompanyType)ncompanytype;

        if (companytype == CompanyType.Company)
        {
            iscompany = true;
        }
        else
        {
            showfrom = true;
            showto = true;
        }


        if (company != null)
        {
            if (company.archived)
            {
                data[0] = 1;
            }
            else
            {
                data[0] = 0;
            }
            cCompany newcompany = new cCompany(company.companyid, company.company, company.companycode, company.archived, company.comment, showto, showfrom, company.createdon, company.createdby, DateTime.Now, user.employeeid, company.address1, company.address2, company.city, company.county, company.postcode, company.country, company.parentcompanyid, iscompany, company.userdefined);
            clscompanies.saveCompany(newcompany);
            data[1] = company.companyid;
            data[2] = company.company;
        }
        else
        {
            company = new cCompany(0, name, "", false, "", showfrom, showto, DateTime.Now, user.employeeid, new DateTime(1900, 01, 01), 0, address1, address2, city, county, postcode, ncountry, 0, iscompany, new Dictionary<int, object>());
            companyid = clscompanies.saveCompany(company);

            data[0] = 0;
            data[1] = companyid;
            data[2] = name;
        }

        return data;
    }

    [WebMethod]
    public string[] getAddress(string country, string postcode)
    {
        expenses.postcodeanywhere.LookupInternational pca = new expenses.postcodeanywhere.LookupInternational();
        if (ConfigurationManager.AppSettings["ProxyServer"] != null)
        {

            WebProxy proxy = new WebProxy();
            proxy.Address = new Uri(ConfigurationManager.AppSettings["ProxyServer"]);
            pca.Proxy = proxy;
        }
        expenses.postcodeanywhere.StreetResults results = pca.FetchStreets("", postcode, country, "", "SOFTW11120", "BD99-GB22-RR15-XC56", "");

        string[] address = new string[5];
        if (results.ErrorNumber == 0 && results.Results.GetLength(0) > 0)
        {
            address[0] = results.Results[0].Street;
            address[1] = results.Results[0].District;
            address[2] = results.Results[0].City;
            address[3] = results.Results[0].State;
            address[4] = results.Results[0].Postcode;
            return address;
        }

        return new string[0];
    }

    [WebMethod]
    public object[] getUsualMileage(string from, string to, string fromnameid, string sCarid)
    {
        int carid = int.Parse(sCarid);
        object[] results = new object[5];
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cCompanies clscompanies = new cCompanies(user.accountid);
        cEmployees clsemps = new cEmployees(user.accountid);
        cEmployee reqemp = clsemps.GetEmployeeById(user.employeeid);
        
        if (carid == 0)
        {
            carid = reqemp.getDefaultCarId();
        }
        cCar car = reqemp.getCarById(carid);
        cMileagecats clsmileagecats = new cMileagecats(user.accountid);
        
        cCompany fromcompany = clscompanies.getCompanyFromName(from);
        cCompany tocompany = clscompanies.getCompanyFromName(to);
        decimal mileageVal = 0;

        if (fromcompany != null && tocompany != null)
        {
            results[0] = 0;
            mileageVal = clscompanies.getDistance(fromcompany.companyid, tocompany.companyid, user.employeeid);

            if (car != null)
            {
                if (car.defaultuom == MileageUOM.KM)
                {
                    mileageVal = clsmileagecats.convertMilesToKM(mileageVal);
                }
            }

            results[1] = mileageVal;
            results[2] = fromnameid.Replace("txtfrom", "txtmileage");
            results[3] = fromnameid.Replace("txtfrom", "labCalcMiles");
            results[4] = fromnameid.Replace("txtfrom", "txtCalcMiles");
        }
        else
        {
            results[0] = 1;
        }
        return results;
    }

    [WebMethod]
    public string getJourneyDetails(string contextKey)
    {
        string[] temp = contextKey.Split(',');
        int claimid = Convert.ToInt32(temp[0].Trim());
        int expenseid = Convert.ToInt32(temp[1].Trim());
        string rowclass = "row1";
        CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
        cClaims clsclaims = new cClaims(user.accountid);
        cClaim reqclaim = clsclaims.getClaimById(claimid);
        cExpenseItem item = reqclaim.getExpenseItemById(expenseid);
        
        StringBuilder output = new StringBuilder();
        output.Append("<table class=\"datatbl\">");
        output.Append("<tr><th>Start Location</th><th>End Location</th><th>Number Miles<br />(Reimbursable)</th><th>Number Miles<br />(Actual)</th><th>Number Passengers</th></tr>");
        if (item.journeysteps != null)
        {
            foreach (cJourneyStep step in item.journeysteps.Values)
            {
                output.Append("<tr>");
                output.Append("<td class=\"" + rowclass + "\">");
                
                if (step.startlocation != null)
                {
                    output.Append(step.startlocation.company);
                }
                output.Append("</td>");
                output.Append("<td class=\"" + rowclass + "\">");
                
                if (step.endlocation != null)
                {
                    output.Append(step.endlocation.company);
                }
                output.Append("</td>");
                output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + step.nummiles + "</td>");
                output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + step.numpassengers + "</td>");
                output.Append("</tr>");
                if (rowclass == "row1")
                {
                    rowclass = "row2";
                }
                else
                {
                    rowclass = "row1";
                }
            }
        }
        output.Append("</table>");
        return output.ToString();
    }


    
}

