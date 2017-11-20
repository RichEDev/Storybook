using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Text;
using SpendManagementLibrary;


/// <summary>
/// Summary description for cUserView
/// </summary>
/// 
[Serializable()]
public class cUserView
{
    private int nAccountid;
    private int nEmployeeid;
    UserView uvView;
    SortedList<int, cField> lstfields;
    private string sSql = "";
    private bool bPrintView;
    
    public cUserView(int accountid, int employeeid, UserView view, bool printview, SortedList<int, cField> fields, string sql)
    {
        nAccountid = accountid;
        nEmployeeid = employeeid;
        uvView = view;
        bPrintView = printview;
        lstfields = fields;
        sSql = sql;
    }

    
    #region properties
    public int employeeid
    {
        get { return nEmployeeid; }
    }
    public int accountid
    {
        get { return nAccountid; }
    }
    public UserView viewtype
    {
        get { return uvView; }
    }
    public bool printView
    {
        get { return bPrintView; }
    }
    public string sql
    {
        get
        {
            return sSql;
        }
    }
    public SortedList<int, cField> fields
    {
        get { return lstfields; }
    }

    #endregion
}


[Serializable()]
public enum UserView
{
    Current = 1,
    Submitted,
    Previous,
    CheckAndPay,
    CurrentPrint,
    SubmittedPrint,
    PreviousPrint,
    CheckAndPayPrint
}

[Serializable()]
public struct sViewInfo
{
    public SortedList<int, List<int>> lstviews;
    public List<int> lstdefviewfields;
    public List<int> lstprintviewfields;
}
