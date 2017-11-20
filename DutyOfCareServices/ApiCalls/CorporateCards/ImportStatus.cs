namespace DutyOfCareServices.ApiCalls.CorporateCards
{
    /// <summary>
    /// An enum to indicate the result of a file import.
    /// </summary>
    public enum ImportStatus
    {
        Sucess = 0,
        IdNotFound = 1,
        InvalidFile = 2,
        ApiFail = 3
    }
}