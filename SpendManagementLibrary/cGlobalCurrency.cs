using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cGlobalCurrency
    {
        private int nGlobalCurrencyID;
        private string sLabel;
        private string sAlphaCode;
        private string sNumericCode;
        public string sUnicodeSymbol;
        private string sSymbol;
        private DateTime dtCreatedon;
        private DateTime dtModifiedon;

        public cGlobalCurrency(int globalcurrencyid, string label, string alphacode, string numericode, string unicodesymbol, DateTime createdon, DateTime modifiedon)
        {
            nGlobalCurrencyID = globalcurrencyid;
            sLabel = label;
            sAlphaCode = alphacode;
            sNumericCode = numericode;
            sUnicodeSymbol = unicodesymbol;
            dtCreatedon = createdon;
            dtModifiedon = modifiedon;
            sSymbol = "";
        }

        #region properties
        public int globalcurrencyid
        {
            get { return nGlobalCurrencyID; }
        }
        public string label
        {
            get { return sLabel; }
        }
        public string alphacode
        {
            get { return sAlphaCode; }
        }
        public string numericcode
        {
            get { return sNumericCode; }
        }
        public string symbol
        {
            get 
            {
                if (sSymbol == "")
                {
                    string[] temp = sUnicodeSymbol.Split(',');
                    for (int i = 0; i < temp.GetLength(0); i++)
                    {
                        string s = temp[0];
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            byte[] b = BitConverter.GetBytes(int.Parse(s));
                            sSymbol += Encoding.UTF32.GetString(b);
                        }
                    }
                }
                return sSymbol.Trim(); 
            }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public DateTime modifiedon
        {
            get { return dtModifiedon; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineGlobalCurrencyInfo
    {
        public Dictionary<int, cGlobalCurrency> lstonlineglobcurrencies;
        public List<int> lstglobcurrencyids;
    }
}
