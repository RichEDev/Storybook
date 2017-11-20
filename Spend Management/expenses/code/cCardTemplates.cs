using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Xml;
using System.Collections.Generic;
using SpendManagementLibrary;

namespace Spend_Management
{
    public class cCardTemplates
    {
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        private int nAccountid;

        public cCardTemplates(int accountid)
        {
            nAccountid = accountid;
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        public cCardTemplate getTemplate(string provider)
        {
            cAccounts acc = new cAccounts();

            cCardTemplate template = (cCardTemplate)Cache["cardtemplate" + provider];
            if (template == null)
            {
                string templatePath = acc.GetFilePaths(accountid, FilePathType.CardTemplate) + provider + ".xml";
                try
                {
                    template = readTemplate(templatePath);
                    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                    {
                        Cache.Insert("cardtemplate" + provider, template, null,
                            Cache.NoAbsoluteExpiration,
                            TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.UltraShort),
                            CacheItemPriority.Default, null);
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Failed to load template\n\n" + ex.Message);
                }
            }
            return template;
        }

        private cCardTemplate readTemplate(string file)
        {
            #region Card Template variables

            cCardTemplate cardTemplate = null;
            string name = string.Empty;
            string sFileType = string.Empty;
            ImportType fileType = ImportType.FixedWidth;
            string delimiter = string.Empty;
            int sheet = 0;
            string quotedCharacters = string.Empty;
            SortedList<CardRecordType, cCardRecordType> recTypes = new SortedList<CardRecordType, cCardRecordType>();

            #endregion

            #region Card record variables

            CardRecordType RecordType = CardRecordType.None;
            string UniqueValue;
            List<ExcludeValueObject> lstExcludeValues = null;
            
            List<cCardTemplateField> flds = null;
            List<cValidationField> headerFlds = null;
            List<cValidationField> footerFlds = null;

            #endregion

            #region Field variables

            string fieldName, fieldType, mappedTable, mappedField, label, format, defaultValue;
            
            CurrencyLookup currencyType = CurrencyLookup.None;
            CountryLookup countryType = CountryLookup.None;
            int maxLength, startPosition, endPosition, fieldIndex, sign;
            bool displayInUnallocatedCardGrid, displayInMoreDetailsTable;
            int numHeaderRecords = 0;
            int numFooterRecords = 0;
            int? numDecimalPlaces = 0;

            #endregion

            XDocument templateDoc = XDocument.Load(file);
            XElement element = null;

            #region CardTemplate Element

            element = templateDoc.Element("CardTemplate");

            if(element != null)
            {
                name = element.Attribute(XName.Get("Name")).Value;
                sFileType = element.Attribute(XName.Get("FileType")).Value;

                if (element.Attribute(XName.Get("NumberHeaderRecords")) != null)
                {
                    int.TryParse(element.Attribute(XName.Get("NumberHeaderRecords")).Value, out numHeaderRecords);
                }

                if (element.Attribute(XName.Get("NumberFooterRecords")) != null)
                {
                    int.TryParse(element.Attribute(XName.Get("NumberFooterRecords")).Value, out numFooterRecords);
                }

                //Set the file type of the template
                switch (sFileType)
                {
                    case "FlatFile":
                        fileType = ImportType.FixedWidth;
                        break;
                    case "Text":
                        fileType = ImportType.FlatFile;
                        break;
                    case "XLS":
                        fileType = ImportType.Excel;
                        break;
                }

                if (element.Attribute(XName.Get("Delimiter")) != null)
                {
                    delimiter = element.Attribute(XName.Get("Delimiter")).Value;
                }

                if (element.Attribute(XName.Get("Sheet")) != null)
                {
                    int.TryParse(element.Attribute(XName.Get("Sheet")).Value, out sheet);
                }
            }

            #endregion

            #region Iterate sub elements

            foreach (XElement subElement in element.Elements())
            {
                #region Reset Card Record variables

                RecordType = CardRecordType.None;
                UniqueValue = string.Empty;

                #endregion

                if (subElement.Name == XName.Get("Fields"))
                {
                    flds = new List<cCardTemplateField>();
                    headerFlds = new List<cValidationField>();
                    footerFlds = new List<cValidationField>();
                    lstExcludeValues = new List<ExcludeValueObject>();

                    if (subElement.Attribute(XName.Get("Type")) != null)
                    {
                        Enum.TryParse(subElement.Attribute(XName.Get("Type")).Value, out RecordType);
                    }

                    if (subElement.Attribute(XName.Get("UniqueValue")) != null)
                    {
                        UniqueValue = subElement.Attribute(XName.Get("UniqueValue")).Value;
                    }

                    //Iterate the field elements for the header, footer and normal fields 

                    foreach (XElement fieldElement in subElement.Elements())
                    {
                        switch (fieldElement.Name.ToString())
                        {
                            case "HeaderRecord":
                                getValidationFieldValues(fieldElement, ref headerFlds);
                                break;

                            case "Field":

                                #region Reset field variables

                                fieldName = string.Empty;
                                fieldType = string.Empty;
                                mappedTable = string.Empty;
                                mappedField = string.Empty;
                                label = string.Empty;
                                format = string.Empty;
                                defaultValue = string.Empty;
                                maxLength = 0;
                                currencyType = CurrencyLookup.None;
                                countryType = CountryLookup.None;
                                startPosition = 0;
                                endPosition = 0;
                                displayInUnallocatedCardGrid = false;
                                displayInMoreDetailsTable = false;
                                fieldIndex = 0;
                                numDecimalPlaces = null;
                                sign = -1;

                                #endregion

                                if (fieldElement.Attribute(XName.Get("Name")) != null)
                                {
                                    fieldName = fieldElement.Attribute(XName.Get("Name")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("Label")) != null)
                                {
                                    label = fieldElement.Attribute(XName.Get("Label")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("MappedTable")) != null)
                                {
                                    mappedTable = fieldElement.Attribute(XName.Get("MappedTable")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("MappedField")) != null)
                                {
                                    mappedField = fieldElement.Attribute(XName.Get("MappedField")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("FieldType")) != null)
                                {
                                    fieldType = fieldElement.Attribute(XName.Get("FieldType")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("Format")) != null)
                                {
                                    format = fieldElement.Attribute(XName.Get("Format")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("MaxLength")) != null)
                                {
                                    int.TryParse(fieldElement.Attribute(XName.Get("MaxLength")).Value, out maxLength);
                                }

                                if (fieldElement.Attribute(XName.Get("StartPosition")) != null)
                                {
                                    int.TryParse(fieldElement.Attribute(XName.Get("StartPosition")).Value, out startPosition);
                                }

                                if (fieldElement.Attribute(XName.Get("EndPosition")) != null)
                                {
                                    int.TryParse(fieldElement.Attribute(XName.Get("EndPosition")).Value, out endPosition);
                                }

                                if (fieldElement.Attribute(XName.Get("Sign")) != null)
                                {
                                    int.TryParse(fieldElement.Attribute(XName.Get("Sign")).Value, out sign);
                                }

                                if (fieldElement.Attribute(XName.Get("DefaultValue")) != null)
                                {
                                    defaultValue = fieldElement.Attribute(XName.Get("DefaultValue")).Value;
                                }

                                if (fieldElement.Attribute(XName.Get("DisplayInUnallocatedCardGrid")) != null)
                                {
                                    if (fieldElement.Attribute(XName.Get("DisplayInUnallocatedCardGrid")).Value.ToLower() == "yes")
                                    {
                                        displayInUnallocatedCardGrid = true;
                                    }
                                }

                                if (fieldElement.Attribute(XName.Get("DisplayInMoreDetailsTable")) != null)
                                {
                                    if (fieldElement.Attribute(XName.Get("DisplayInMoreDetailsTable")).Value.ToLower() == "yes")
                                    {
                                        displayInMoreDetailsTable = true;
                                    }
                                }

                                if (fieldElement.Attribute(XName.Get("CurrencyType")) != null)
                                {
                                    switch (fieldElement.Attribute(XName.Get("CurrencyType")).Value)
                                    {
                                        case "numeric":
                                            currencyType = CurrencyLookup.Numeric;
                                            break;
                                        case "alpha":
                                            currencyType = CurrencyLookup.Alpha;
                                            break;
                                        case "label":
                                            currencyType = CurrencyLookup.Label;
                                            break;
                                    }
                                }

                                if (fieldElement.Attribute(XName.Get("CountryType")) != null)
                                {
                                    switch (fieldElement.Attribute(XName.Get("CountryType")).Value)
                                    {
                                        case "alpha":
                                        countryType = CountryLookup.Alpha;
                                        break;
                                    case "label":
                                        countryType = CountryLookup.Label;
                                        break;
                                    case "alpha3":
                                        countryType = CountryLookup.Alpha3;
                                        break;
                                    case "numeric3":
                                        countryType = CountryLookup.Numeric3;
                                        break;
                                    }
                                }

                                if (fieldElement.Attribute(XName.Get("FieldIndex")) != null)
                                {
                                    int.TryParse(fieldElement.Attribute(XName.Get("FieldIndex")).Value, out fieldIndex);
                                }

                                if (fieldElement.Attribute(XName.Get("NumDecimalPlaces")) != null)
                                {
                                    int tempInt = 0;
                                    int.TryParse(fieldElement.Attribute(XName.Get("NumDecimalPlaces")).Value, out tempInt);

                                    if (tempInt > 0)
                                    {
                                        numDecimalPlaces = tempInt;
                                    }
                                }

                                if (fileType == ImportType.FixedWidth)
                                {
                                    flds.Add(new cFlatField(fieldName, mappedTable, mappedField, fieldType, maxLength, startPosition, endPosition, displayInUnallocatedCardGrid, label, displayInMoreDetailsTable, format, defaultValue, currencyType, countryType, numDecimalPlaces, sign));
                                }
                                else
                                {
                                    flds.Add(new cCardTemplateField(fieldName, mappedTable, mappedField, fieldType, maxLength, displayInUnallocatedCardGrid, label, displayInMoreDetailsTable, format, defaultValue, fieldIndex, currencyType, countryType, numDecimalPlaces));
                                }
                                break;

                            case "FooterRecord":
                                getValidationFieldValues(fieldElement, ref footerFlds);
                                break;
                            case "ExcludeValues":
                                foreach (XElement excludeElement in fieldElement.Elements())
                                {
                                    ExcludeValueObject exObj = new ExcludeValueObject();

                                    if (excludeElement.Attribute(XName.Get("ExcludeValueColumnIndex")) != null)
                                    {
                                        int.TryParse(excludeElement.Attribute(XName.Get("ExcludeValueColumnIndex")).Value, out exObj.excludeValueIndex);
                                    }

                                    exObj.excludeValue = excludeElement.Value;

                                    lstExcludeValues.Add(exObj);
                                }
                                break;
                        }

                    }

                    recTypes.Add(RecordType, new cCardRecordType(RecordType, UniqueValue, lstExcludeValues, flds, headerFlds, footerFlds));
                }  
                
                #region Quoted Characters

                if (subElement.Name == XName.Get("QuotedCharacters"))
                {
                    if (subElement != null)
                    {
                        quotedCharacters = subElement.Value;
                    }
                }

                #endregion
            }

            #endregion

            cardTemplate = new cCardTemplate(name, numHeaderRecords, numFooterRecords, fileType, delimiter, sheet, recTypes, quotedCharacters);

            return cardTemplate;              
        }

        /// <summary>
        /// Populate the validation field values for the header and footer fields
        /// </summary>
        /// <param name="element"></param>
        /// <param name="lstValFields"></param>
        private void getValidationFieldValues(XElement element, ref List<cValidationField> lstValFields)
        {
            string valFieldType, expectedText, transactionCode;
            int recordTypeColumnIndex, lookupColumnIndex;

            foreach (XElement subElement in element.Elements())
            {
                #region Reset field variables

                valFieldType = string.Empty;
                expectedText = string.Empty;
                transactionCode = string.Empty;
                recordTypeColumnIndex = 0;
                lookupColumnIndex = 0;

                #endregion

                if (subElement.Attribute(XName.Get("FieldType")) != null)
                {
                    valFieldType = subElement.Attribute(XName.Get("FieldType")).Value;
                }

                if (subElement.Value != null)
                {
                    expectedText = subElement.Value;
                }

                if (subElement.Attribute(XName.Get("TransactionCode")) != null)
                {
                    transactionCode = subElement.Attribute(XName.Get("TransactionCode")).Value;
                }

                if (subElement.Attribute(XName.Get("RecordTypeColumnIndex")) != null)
                {
                    int.TryParse(subElement.Attribute(XName.Get("RecordTypeColumnIndex")).Value, out recordTypeColumnIndex);
                }

                if (subElement.Attribute(XName.Get("LookupColumnIndex")) != null)
                {
                    int.TryParse(subElement.Attribute(XName.Get("LookupColumnIndex")).Value, out lookupColumnIndex);
                }

                lstValFields.Add(new cValidationField(valFieldType, expectedText, transactionCode, recordTypeColumnIndex, lookupColumnIndex));
            }
        }

    }

    
}
