using System;

/// <summary>
/// A class to hold and format start and end dates for sub cats.
/// </summary>
internal class SubCatDates
{
    private DateTime _startDate;

    private DateTime _endDate;

    /// <summary>
    /// Get the formatted start date
    /// </summary>
    public string StartDate => this._startDate.ToString("yyyy-MM-dd");

    /// <summary>
    /// Get the formatted end date
    /// </summary>
    public string EndDate => this._endDate.ToString("yyyy-MM-dd");

    /// <summary>
    /// Create a new instance of <see cref="SubCatDates"/>
    /// </summary>
    /// <param name="startDate">the <see cref="DateTime"/>value for start date</param>
    /// <param name="endDate">the <see cref="DateTime"/>value for end date</param>
    public SubCatDates(DateTime startDate, DateTime endDate)
    {
        this._startDate = startDate;
        this._endDate = endDate;
    }

}