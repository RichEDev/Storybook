using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using expenses;
using System.Web.Caching;
using ExpensesLibrary;
using System.Collections.Generic;
using SpendManagementLibrary;
/// <summary>
/// Summary description for cCardProviders
/// </summary>
public class cCardProviders
{
    DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
    string strsql;

    System.Collections.SortedList list;

    System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

    public cCardProviders()
    {
        expdata.changeDB(Database.MetaBase);
        InitialiseData();
    }

    
    private void InitialiseData()
    {
        list = (System.Collections.SortedList)Cache["cardproviders"];
        if (list == null)
        {
            list = CacheList();
        }

    }

    private System.Collections.SortedList CacheList()
    {
        cCardProvider newprov;
        CorporateCardType cardtype;
        string cardprovider;
        int cardproviderid;
        DateTime createdon;
        int createdby;
        DateTime? modifiedon;
        int? modifiedby;

        System.Collections.SortedList list = new System.Collections.SortedList();
        System.Data.SqlClient.SqlDataReader reader;

        strsql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
        expdata.sqlexecute.CommandText = strsql;
        SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            cardproviderid = reader.GetInt32(0);
            cardprovider = reader.GetString(1);
            cardtype = (CorporateCardType)reader.GetByte(2);
            if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
            {
                createdon = new DateTime(1900, 01, 01);
            }
            else
            {
                createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
            }
            if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
            {
                createdby = 0;
            }
            else
            {
                createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
            }
            if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
            {
                modifiedon = new DateTime(1900, 01, 01);
            }
            else
            {
                modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
            }
            if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
            {
                modifiedby = 0;
            }
            else
            {
                modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
            }
            newprov = new cCardProvider(cardproviderid, cardprovider, cardtype, createdon, createdby, modifiedon, modifiedby);
            list.Add(cardproviderid, newprov);
        }
        reader.Close();

        Cache.Insert("cardproviders", list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
        return list;
    }

    public System.Web.UI.WebControls.ListItem[] CreateList()
    {
        System.Web.UI.WebControls.ListItem[] items = new ListItem[list.Count];
        int count = 0;
        cCardProvider prov;

        for (int i = 0; i < list.Count; i++)
        {
            prov = (cCardProvider)list.GetByIndex(i);
            items[count] = new ListItem(prov.cardprovider, prov.cardproviderid.ToString());
            count++;


        }

        return items;
    }
    public System.Web.UI.WebControls.ListItem[] CreateList(bool purchasecard)
    {
        System.Web.UI.WebControls.ListItem[] items;
        int count = 0;
        cCardProvider prov;

        for (int i = 0; i < list.Count; i++)
        {
            prov = (cCardProvider)list.GetByIndex(i);
            if (purchasecard)
            {
                if (prov.cardtype == CorporateCardType.PurchaseCard)
                {
                    count++;
                }
            }
            else
            {
                if (prov.cardtype == CorporateCardType.CreditCard)
                {
                    count++;
                }
            }
        }

        items = new ListItem[count];

        count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            prov = (cCardProvider)list.GetByIndex(i);

            if (purchasecard)
            {
                if (prov.cardtype == CorporateCardType.PurchaseCard)
                {
                    items[count] = new ListItem(prov.cardprovider, prov.cardproviderid.ToString());
                    count++;
                }
            }
            else
            {
                if (prov.cardtype == CorporateCardType.CreditCard)
                {
                    items[count] = new ListItem(prov.cardprovider, prov.cardproviderid.ToString());
                    count++;
                }
            }
            
        }
        return items;
    }

    public cCardProvider getProviderByID(int id)
    {
        return (cCardProvider)list[id];
    }
    public cCardProvider getProviderByName(string name)
    {
        foreach (cCardProvider provider in list.Values)
        {
            if (provider.cardprovider == name)
            {
                return provider;
            }
        }
        return null;
    }

    public Dictionary<int, cCardProvider> getCardProviders()
    {
        Dictionary<int, cCardProvider> lstproviders = new Dictionary<int, cCardProvider>();

        foreach (cCardProvider prov in list.Values)
        {
            lstproviders.Add(prov.cardproviderid, prov);
        }

        return lstproviders;
    }

}




