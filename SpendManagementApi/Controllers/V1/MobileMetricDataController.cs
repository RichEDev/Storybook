namespace SpendManagementApi.Controllers.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Requests.MobileMetricData;
    using SpendManagementApi.Models.Responses;

    using SpendManagementApi.Repositories;

	/// <summary>
    /// The mobile metric data v1 controller.
    /// </summary>
    [Version(1)]
    [RoutePrefix("MobileMetricData")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileMetricDataV1Controller : BaseApiController
	{
		/// <summary>
		/// An instance of <see cref="MobileMetricDataRepository">111</see>
		/// </summary>
		private MobileMetricDataRepository _repository;

		/// <summary>
		/// Updates the metric device data in the database
		/// </summary>
		/// <param name="request">
		/// The <see cref="MobileMetricDataRequest">MobileMetricDataRequest</see>.
		/// </param>
		/// <returns>
		/// The <see cref="NumericResponse">NumericResponse</see> containing the internal identifer for the device.
		/// </returns>
		[HttpPut, Route("UpdateMobileMetricData")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public NumericResponse UpdateMobileMetricData([FromBody] MobileMetricDataRequest request)
		{
			var response = this.InitialiseResponse<NumericResponse>();
			response.Item = this._repository.UpdateMetricData(request.MetricData);
			return response;
		}

		/// <summary>
		/// Sets the status of the mobile device.
		/// </summary>
		/// <param name="request">
		/// The <see cref="SetDeviceStatusRequest">SetDeviceNotificationStatusRequest</see>.
		/// </param>
		/// <returns>
		/// The <see cref="NumericResponse">NumericResponse</see> 1, if the notification status is true, else 0.
		/// </returns>
		[HttpPut, Route("SetMobileDeviceStatus")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public NumericResponse SetMobileDeviceStatus([FromBody] SetDeviceStatusRequest request)
		{
			var response = this.InitialiseResponse<NumericResponse>();
			response.Item = this._repository.SetMobileDeviceStatus(request.DeviceId, request.IsActive, request.AllowNotifications);
			return response;
		}

		/// <summary>
		/// The init.
		/// </summary>
		protected override void Init()
		{
			this._repository = new MobileMetricDataRepository(this.CurrentUser);
		}
    }
}