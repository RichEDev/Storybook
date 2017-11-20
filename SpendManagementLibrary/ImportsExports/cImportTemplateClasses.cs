using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spend_Management
{
    [Serializable()]
    public class cWorksheetColumnMappingTemplate
    {
        private string sName;
        private int nColumnRef;
        private string sFieldType;
        private int nMaxLength;
        private bool bIsUnique;
        private bool bIsMandatory;
        private string sLinkRef; //The reference to the actual data field for the import e.g. ID of object
        private string sLinkMatch; //The data field used to match the link ref to e.g. name of the object

        public cWorksheetColumnMappingTemplate(string name, int columnRef, string fieldType)
        {
            sName = name;
            nColumnRef = columnRef;
            sFieldType = fieldType;
        }

        public cWorksheetColumnMappingTemplate(string name, int columnRef, string fieldType, int maxLength, bool isUnique, bool isMandatory, string linkRef, string linkMatch)
        {
            sName = name;
            nColumnRef = columnRef;
            sFieldType = fieldType;
            nMaxLength = maxLength;
            bIsUnique = isUnique;
            bIsMandatory = isMandatory;
            sLinkRef = linkRef;
            sLinkMatch = linkMatch;
        }

        #region Properties

        public string name
        {
            get { return sName; }
        }

        public int columnRef
        {
            get { return nColumnRef; }
        }

        public string fieldType
        {
            get { return sFieldType; }
        }

        public int maxLength
        {
            get { return nMaxLength; }
        }

        public bool isUnique
        {
            get { return bIsUnique; }
        }

        public bool isMandatory
        {
            get { return bIsMandatory; }
        }

        public string linkRef
        {
            get { return sLinkRef; }
        }

        public string linkMatch
        {
            get { return sLinkMatch; }
        }
        #endregion

    }

    [Serializable()]
    public class cWorksheetMappingTemplate
    {
        private int nWorksheetIndex;
        private string sWorksheetName;
        private int nStartRow;
        private List<cWorksheetColumnMappingTemplate> lstMappings;

        public cWorksheetMappingTemplate(int worksheetIndex, string worksheetName, int startRow, List<cWorksheetColumnMappingTemplate> mappings)
        {
            nWorksheetIndex = worksheetIndex;
            sWorksheetName = worksheetName;
            nStartRow = startRow;
            lstMappings = mappings;
        }

        #region Properties

        public int worksheetIndex
        {
            get { return nWorksheetIndex; }
        }

        public string worksheetName
        {
            get { return sWorksheetName; }
        }

        public int startRow
        {
            get { return nStartRow; }
        }

        public List<cWorksheetColumnMappingTemplate> mappings
        {
            get { return lstMappings; }
        }
        #endregion
    }

    [Serializable()]
    public enum WorksheetStatus
    {
        Validating = 0,
        Valid,
        Invalid,
        Importing,
        Imported,
        Failed
    }

    public enum ImportStatus
    {
        Validating = 0,
        Valid,
        Invalid,
        Importing,
        Complete,
        Failed
    }

    [Serializable()]
    public struct ImportStatusValues
    {
        public int numOfRows;
        public int processedRows;
        public string sheetName;
        public WorksheetStatus status;
    }

    [Serializable()]
    public class cImportInfo
    {
        private int nCurrentSheet;
        private string sCurrentSheetName;
        private ImportStatus status;
        private SortedList<int, ImportStatusValues> lstProcessingWorksheets;

        public cImportInfo(int currentSheet, string currentSheetName, ImportStatus importStatus, SortedList<int, ImportStatusValues> worksheets)
        {
            nCurrentSheet = currentSheet;
            sCurrentSheetName = currentSheetName;
            status = importStatus;
            lstProcessingWorksheets = worksheets;
        }

        #region Properties

        public int currentSheet
        {
            get { return nCurrentSheet; }
            set { nCurrentSheet = value; }
        }
        public string currentSheetName
        {
            get { return sCurrentSheetName; }
            set { sCurrentSheetName = value; }
        }

        public ImportStatus importStatus
        {
            get { return status; }
            set { status = value; }
        }
        public SortedList<int, ImportStatusValues> worksheets
        {
            get { return lstProcessingWorksheets; }
            set { lstProcessingWorksheets = value; }
        }
        #endregion

    }

    [Serializable()]
    public struct ImportProcessData
    {
        public Dictionary<string, List<Dictionary<string, object>>> lstWorksheets;
        public Dictionary<string, string> lstRequireWorksheets;
        public object workbook;
    }
}
