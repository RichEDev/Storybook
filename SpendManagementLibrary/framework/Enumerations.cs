using System;
public enum AttachmentHandling
{
    Upload = 0,
    EmailRequest = 1
}

public enum ViewType
{
    Basic = 0,
    Normal = 1,
    Enhanced = 2
}

public enum PasswordLengthSetting
{
    AnyLength = 1,
    MustEqual = 2,
    MustBeGreaterThan = 3,
    MustBeLessThan = 4,
    MustBeBetween = 5
}

public enum fwIconSize
{
    Small = 16,
    Medium = 24,
    Large = 48
}

public enum DeleteMethod
{
    Delete = 0,
    Archive = 1
}

public enum LogonStatus
{
    Active = 0,
    Frozen = 1,
    Suspended = 2,
    Archived = 3
}

public enum AttachmentArea
{
    CONTRACT = 0,
    VENDOR = 1,
    CONTRACT_NOTES = 2,
    VENDOR_NOTES = 3,
    PRODUCT_NOTES = 4,
    INVOICE_NOTES = 5,
    CONTACT_NOTES = 6,
    TASKS = 7
}

public enum AppAreas
{
    CONTRACT_DETAILS = 1,
    CONTRACT_PRODUCTS = 2,
    PRODUCT_DETAILS = 3,
    VENDOR_DETAILS = 4,
    CONTRACT_GROUPING = 5,
    RECHARGE_GROUPING = 6,
    CONPROD_GROUPING = 7,
    STAFF_DETAILS = 8,
    INVOICE_DETAILS = 9,
    INVOICE_FORECASTS = 10,
    VENDOR_CONTACTS = 11,
    VENDOR_GROUPING = 12
}

public enum SummaryTabs
{
    ContractDetail = 0,
    ContractAdditional = 1,
    ContractProduct = 2,
    InvoiceDetail = 3,
    InvoiceForecast = 4,
    NotesSummary = 5,
    LinkedContracts = 6,
    RechargeTemplate = 7,
    RechargePayments = 8,
    RechargeFunction = 98,
    RechargeFields = 99
}

public enum PwdValidateReturnCodes
{
    Pwd_OK = 0,
    Pwd_BadLength = 1,
    Pwd_NoCaps = 2,
    Pwd_NoNum = 3,
    Pwd_Previous = 4,
    Pwd_NoSymbol = 5
}

public enum AttachmentType
{
    Open = 0,
    Secure = 1,
    Audience = 2,
    Hyperlink = 3
}

public enum emailType
{
    ContractReview = 1,
    OverdueInvoice = 2,
    AuditCleardown = 3,
    LicenceExpiry = 4
}

public enum emailFreq
{
    Once = 0,
    Daily = 1,
    Weekly = 2,
    MonthlyOnFirstXDay = 3,
    MonthlyOnDay = 4,
    Every_n_Days = 5
}

public enum RechargeApportionType
{
    Percentage = 0,
    Fixed = 1,
    n_Units = 2
}

public enum SurchargeType
{
    Percentage = 0,
    Fixed = 1
}

public enum StandardReportTabs
{
    Favourites = 0,
    Contract = 1,
    Financial = 2,
    Recharge = 3
}

public enum AudienceType
{
    Individual = 0,
    Team = 1
}

public enum TeamType
{
    Employee = 0,
    User = 1
}

public enum UserFieldType
{
    Character = 1,
    Number = 2,
    DateField = 3,
    DDList = 4,
    Checkbox = 5,
    Text = 6,
    Float = 7,
    Hyperlink = 8,
    StaffName_Ref = 100,
    Site_Ref = 101,
    RechargeClient_Ref = 102,
    RechargeAcc_Code = 103
}

public enum MaintType
{
    SingleInflator = 0,
    GreaterXY = 1,
    LesserXY = 2
}

[Serializable]
public enum ForecastType
{
    Prod_v_Inflator = 0,
    InflatorOnly = 1,
    Staged = 2
}

public enum AudienceViewType
{
    NoAudience = 0,
    AllowAllIfNoneExist = 1,
    DenyAllIfNoneExist = 2
}