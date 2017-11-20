using System;
using System.Web;
using System.Web.UI;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SpendManagementLibrary;

namespace FWClasses
{
    public class cParams : cParamsBase
    {

        public Cache Cache = HttpRuntime.Cache;

        public cParams(UserInfo uinfo, cFWSettings fws, int location)
            : base(fws.MetabaseCustomerId, uinfo.ActiveLocation)
        {
            cFWS = fws;
            cUInfo = uinfo;

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                CreateDependency();
            }

            if (Cache["FWParams_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + location.ToString()] == null)
            {
                ParamList = CacheList();
            }
            else
            {
                ParamList = (SortedList<string, cParam>)Cache["FWParams_" + fws.MetabaseAccountKey.Replace(" ", "_") + fws.glDatabase.Replace(" ", "_") + location.ToString()];
            }
        }

        private void CreateDependency()
        {
            if (Cache["paramdependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + this.curLocation] == null)
            {
                Cache.Insert("paramdependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + this.curLocation, 1);
            }
        }

        private CacheDependency GetDependency()
        {
            var dependency = new String[1];
            dependency[0] = "paramdependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + this.curLocation;

            var dep = new CacheDependency(null, dependency);

            return dep;
        }

        public void InvalidateCache()
        {
            Cache.Remove("paramdependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + curLocation.ToString());
            CreateDependency();
        }


        private SortedList<string, cParam> CacheList()
        {
            cFWDBConnection db = new cFWDBConnection();
            SortedList<string, cParam> newparams = new SortedList<string, cParam>();

            string sql = "SELECT * FROM fwparams WHERE [Location Id] = @locId";

            db.DBOpen(cFWS, false);
            db.AddDBParam("locId", curLocation, true);
            db.RunSQL(sql, db.glDBWorkA, false, "", false);

            foreach (DataRow drow in db.glDBWorkA.Tables[0].Rows)
            {
                string name = (string)drow["Param"];
                string value = (string)drow["Value"];
                bool canEdit;
                if ((Int16)drow["Editable"] == 1)
                {
                    canEdit = true;
                }
                else
                {
                    canEdit = false;
                }
                cParam newParam = new cParam(name, value, canEdit);
                newparams.Add(name, newParam);
            }
            db.DBClose();

            if (newparams.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                Cache.Insert("FWParams_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + curLocation.ToString(), newparams, GetDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
            }
            return newparams;
        }

    }


}
