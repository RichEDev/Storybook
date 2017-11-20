namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary.Holidays;

    using Spend_Management;

    using Holiday = SpendManagementApi.Models.Types.Holidays.Holiday;

    /// <summary>
    /// The holiday repository.
    /// </summary>
    internal class HolidayRepository : BaseRepository<Holiday>, ISupportsActionContext
    {
        /// <summary>
        /// The _action context.
        /// </summary>
        private readonly IActionContext _actionContext;

        /// <summary>
        /// The _holiday data.
        /// </summary>
        private readonly Holidays _holidayData;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayRepository"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public HolidayRepository(ICurrentUser user, IActionContext actionContext = null) : base(user, actionContext, Holiday => Holiday.HolidayId, null)
        {
            this._holidayData = ActionContext.Holidays;
        }

        /// <summary>
        /// Get all of the current users holidays
        /// </summary>
        /// <returns>A list of <see cref="Holiday">Holiday</see></returns>
        public override IList<Holiday> GetAll()
        {
            this.DoesUserHaveAccessToEmployees();
            var holidays = this._holidayData.GetAllEmployeeHolidays(User.EmployeeID);
            var employeeHolidays = new List<Holiday>();

            foreach (var holiday in holidays)
            {
                employeeHolidays.Add(new Holiday().From(holiday, this._actionContext));
            }

            return employeeHolidays;
        }

        /// <summary>
        /// Gets a <see cref="Holiday"/> by its Id
        /// </summary>
        /// <param name="id">The Holiday Id</param>
        /// <returns></returns>
        public override Holiday Get(int id)
        {
            this.DoesUserHaveAccessToEmployees();
            var holiday = this._holidayData.GetHolidayById(id);
            return new Holiday().From(holiday, ActionContext);
        }

        /// <summary>
        /// Saves a  <see cref="Holiday"/> for the current user.
        /// </summary>
        /// <param name="holiday">The <see cref="Holiday">Holiday</see> data to be saved</param>
        /// <returns>The details of the newly saved <see cref="Holiday">Holiday</see></returns>
        public Holiday Add(HolidayRequest holiday)
        {
            this.DoesUserHaveAccessToEmployees();
            this.ValidateHolidayDates(holiday);

            var holidayId = this._holidayData.AddHoliday(User.EmployeeID, holiday.StartDate, holiday.EndDate);
            return this.Get(holidayId);
        }

        /// <summary>
        /// Updates a <see cref="Holiday"/> by its Id
        /// </summary>
        /// <param name="holiday">The <see cref="Holiday">Holiday</see> data to be updated</param>
        /// <returns>The details of the updated <see cref="Holiday">Holiday</see></returns>
        public Holiday Update(HolidayRequest holiday)
        {
            this.CheckHolidayOwnership(holiday.Id);
            this.DoesUserHaveAccessToEmployees();
            this.ValidateHolidayDates(holiday);

            this._holidayData.UpdateHoliday(holiday.Id, holiday.StartDate, holiday.EndDate);
            return this.Get(holiday.Id);
        }

        /// <summary>
        /// Deletes a <see cref="Holiday"/>, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the CorporateCard to delete.</param>
        /// <returns>Null, if the card was deleted successfully.</returns>
        public override Holiday Delete(int id)
        {
            this.DoesUserHaveAccessToEmployees();
            this.CheckHolidayOwnership(id);
            this._holidayData.DeleteHoliday(id);
            return this.Get(id);
        }

        private void DoesUserHaveAccessToEmployees()
        {
            if (!ActionContext.Holidays.DoesEmployeeHaveAccessToHolidays(User.EmployeeID))
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorInvalidHolidayPermission);
            }
        }

        private void CheckHolidayOwnership(int Id)
        {
            var holiday = this.Get(Id);

            if (holiday.EmployeeId != this.User.EmployeeID)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorNotHolidayOwner);
            }
        }

        private void ValidateHolidayDates(HolidayRequest holiday)
        {
            if (holiday.StartDate > holiday.EndDate)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorHolidayStartDateAfterEndDate);
            }

            if (holiday.EndDate < holiday.StartDate)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorHolidayEndDateBeforeStartDate);
            }
        }
    }
}