namespace SpendManagementLibrary
{
    /// <summary>
    /// AttributeFormat enumeration
    /// </summary>
    public enum AttributeFormat
    {
        NotSet = 0,
        SingleLine = 1,
        MultiLine,
        DateTime,
        DateOnly,
        TimeOnly,
        FormattedText,
        SingleLineWide,
        ListStandard,
        ListWide,
        ContactEmail = 11,
        ContactPhone = 12,
        ContactSMS = 13
    }
}