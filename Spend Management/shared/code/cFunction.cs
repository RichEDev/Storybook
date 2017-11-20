using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for cFunction
/// </summary>
/// 
namespace Spend_Management
{
    [Serializable()]
    public class cFunction
    {
        string functionName;
        string description;
        string remarks;
        string example;
        string syntax;
        int id;
        string parent;
        
        public cFunction(int iId, string sFunctionName, string sDescription, string sRemarks, string sExample, string sSyntax, string sParent)
        {
            functionName = sFunctionName;
            description = sDescription;
            remarks = sRemarks;
            example = sExample;
            syntax = sSyntax;
            id = iId;
            parent = sParent;
        }

        public int ID
        {
            get
            {
                return id;
            }
        }

        public string FunctionName
        {
            get
            {
                return functionName;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public string Remarks
        {
            get
            {
                return remarks;
            }
        }

        public string Example
        {
            get
            {
                return example;
            }
        }

        public string Syntax
        {
            get
            {
                return syntax;
            }
        }

        public string Parent
        {
            get
            {
                return parent;
            }
        }
    } 
}
