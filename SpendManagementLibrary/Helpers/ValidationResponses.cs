namespace SpendManagementLibrary.Helpers
{
    class ValidationResponses
    {
        public const string InvalidId = "The Id must be greater than -1";
        public const string InvalidDateRange = "Date cannot be greater than today, or less than 01/01/1753";
        public const string InvalidCreatedById = "The CreatedById is must be greater than 0";
        public const string InvalidEnumValue = "The enum value is incorrect";
        public const string InvalidUdfId = "The UDF Id must be greater than 0";
        public const string DictionaryNull = "The Dictionary cannot be null";
    }
}
