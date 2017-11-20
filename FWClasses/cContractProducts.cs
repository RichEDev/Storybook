using System;
using System.Collections.Generic;
using System.Web.Caching;

using SpendManagementLibrary;
using Spend_Management;

namespace FWClasses
{
    public class cContractProduct
    {
        private int nConProdId;
        public int ContractProductId
        {
            get { return nConProdId; }
        }
        private int nContractId;
        public int ContractId
        {
            get { return nContractId; }
        }
        private cProduct cpContractProduct;
        public cProduct ContractProduct
        {
            get { return cpContractProduct; }
        }

        public cContractProduct(int CPId, int contractId, cProduct contractproduct)
        {
            nConProdId = CPId;
            nContractId = contractId;
            cpContractProduct = contractproduct;
        }
    }

    public class cContractProductColl
    {
        private readonly Cache cache = System.Web.HttpRuntime.Cache;
        private SortedList<int, cContractProduct> slConProds;
        private cFWSettings fws;
        private UserInfo uinfo;
        private int nContractId;

        public cContractProductColl(cFWSettings cFWS, UserInfo user_info, int ContractId)
        {
            fws = cFWS;
            uinfo = user_info;
            nContractId = ContractId;

            if (this.cache["contractproducts_" + fws.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + nContractId.ToString()] == null)
            {
                slConProds = CacheItems();
            }
            else
            {
                slConProds = (SortedList<int, cContractProduct>)this.cache["contractproducts_" + fws.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + nContractId.ToString()];
            }

        }

        private SortedList<int, cContractProduct> CacheItems()
        {
            SortedList<int, cContractProduct> items = new SortedList<int, cContractProduct>();
            cProducts products = new cProducts(fws.MetabaseCustomerId, uinfo.ActiveLocation);

            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);
            string sql = "SELECT * FROM contract_productdetails WHERE [Contract Id] = @conId";
            db.AddDBParam("conId", nContractId, true);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int cpid = reader.GetInt32(reader.GetOrdinal("Contract-Product Id"));
                    int conid = reader.GetInt32(reader.GetOrdinal("Contract Id"));
                    int prodid = reader.GetInt32(reader.GetOrdinal("Product Id"));
                    cProduct prod = products.GetProductById(prodid);

                    cContractProduct newprod = new cContractProduct(cpid, conid, prod);
                    items.Add(newprod.ContractProductId, newprod);
                }
                reader.Close();
            }

            if (items.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                this.cache.Insert("contractproducts_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + "_" + uinfo.ActiveLocation, items, GetDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
            }

            db.DBClose();

            return items;
        }

        public int Count
        {
            get { return slConProds.Count; }
        }

        public int GetContractId(int CPid)
        {
            int contractId = 0;
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);
            db.FWDb("R", "contract_productdetails", "Contract-Product Id", CPid, "", "", "", "", "", "", "", "", "", "");
            if (db.FWDbFlag)
            {
                if (db.FWDbFindVal("Contract Id", 1).Trim() != "")
                {
                    contractId = int.Parse(db.FWDbFindVal("Contract Id", 1));
                }
            }
            db.DBClose();

            return contractId;
        }

        private void CreateDependency()
        {
            if (this.cache["contractproductsdependency_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + "_" + this.nContractId] == null)
            {
                this.cache.Insert("contractproductsdependency_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + "_" + this.nContractId, 1);
            }
        }

        CacheDependency GetDependency()
        {
            CacheDependency dep;
            String[] dependency;
            dependency = new string[1];
            dependency[0] = "contractproductsdependency_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + "_" + this.nContractId;
            dep = new CacheDependency(null, dependency);
            return dep;
        }

        public void InvalidateCache()
        {
            this.cache.Remove("contractproductsdependency_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + "_" + this.nContractId);

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                CreateDependency();
            }
        }
    }
}
