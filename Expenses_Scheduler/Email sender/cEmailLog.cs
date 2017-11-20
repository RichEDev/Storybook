using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using System.Diagnostics;
using System.IO;

namespace Expenses_Scheduler
{
    public class cEmailLog
    {
        readonly StringBuilder _logInfo = new StringBuilder();
        public void AddToLog(string strIn, cAccount account)
        {
            Debug.AutoFlush = true;
            _logInfo.Append(DateTime.Now + " " + strIn + "\r\n");
            Debug.WriteLine(strIn);
        }

        public void WriteLog(cAccount account)
        {
            if (!System.IO.Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
            {
                System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\logs");
            }

            StreamWriter writer = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\logs\\FWEmail_" + account.companyid + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log", true);


            writer.Write(_logInfo.ToString());
            writer.Close();
            _logInfo.Remove(0, _logInfo.Length);
        }
    }
}
