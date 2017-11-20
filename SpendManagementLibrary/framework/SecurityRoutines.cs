using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SpendManagementLibrary
{
    public class SecurityRoutines
    {
        public static UserInfo GetCountInfo(UserInfo usrInfo, cFWSettings fws)
        {
            try
            {
                cFWDBConnection db = new cFWDBConnection();
                db.DBOpen(fws, false);

                db.AddDBParam("locId", usrInfo.ActiveLocation, true);
                db.RunSQL("SELECT COUNT(*) AS [ReturnCount] FROM [codes_sites] WHERE [Location Id] = @locId", db.glDBWorkL, false, "", false);
                if (!db.glError)
                {
                    usrInfo.glSiteCount = (int)db.GetFieldValue(db.glDBWorkL, "ReturnCount", 0, 0);
                }
                else
                {
                    usrInfo.glSiteCount = 0;
                }

                db.AddDBParam("locId", usrInfo.ActiveLocation, true);
                db.RunSQL("SELECT COUNT(*) AS [ReturnCount] FROM [staff_details] WHERE [Location Id] = @locId", db.glDBWorkL, false, "", false);
                if (!db.glError)
                {
                    usrInfo.glStaffCount = (int)db.GetFieldValue(db.glDBWorkL, "ReturnCount", 0, 0);
                }
                else
                {
                    usrInfo.glStaffCount = 0;
                }

                if (fws.glUseRechargeFunction)
                {
                    db.AddDBParam("locId", usrInfo.ActiveLocation, true);
                    db.RunSQL("SELECT COUNT(*) AS [ReturnCount] FROM [codes_accountcodes] WHERE [Location Id] = @locId", db.glDBWorkL, false, "", false);
                    if (!db.glError)
                    {
                        usrInfo.glRechargeAccCount = (int)db.GetFieldValue(db.glDBWorkL, "ReturnCount", 0, 0);
                    }
                    else
                    {
                        usrInfo.glRechargeAccCount = 0;
                    }

                    db.AddDBParam("locId", usrInfo.ActiveLocation, true);
                    db.RunSQL("SELECT COUNT(*) AS [ReturnCount] FROM [codes_rechargeentity] WHERE [Location Id] = @locId", db.glDBWorkL, false, "", false);
                    if (!db.glError)
                    {
                        usrInfo.glRechargeClientCount = (int)db.GetFieldValue(db.glDBWorkL, "ReturnCount", 0, 0);
                    }
                    else
                    {
                        usrInfo.glRechargeClientCount = 0;
                    }
                }
                db.DBClose();

                return usrInfo;
            }
            catch
            {
                usrInfo.glRechargeClientCount = 0;
                usrInfo.glRechargeAccCount = 0;
                usrInfo.glSiteCount = 0;
                usrInfo.glStaffCount = 0;

                return usrInfo;
            }

        }
    }
}
