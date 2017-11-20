namespace EsrGo2FromNhsWcfLibrary
{
    using System.Collections.Generic;

    public class FileHeadersAndRows
    {
        public int FileId
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public List<string> FileRows
        {
            get;
            set;
        }
    }
}
