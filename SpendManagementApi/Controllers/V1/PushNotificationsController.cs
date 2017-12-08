using SpendManagementApi.Models.Responses;

namespace SpendManagementApi.Controllers.V1
{
	using System.Collections.Generic;
	using System.Web.Http;
	using System.Web.Http.Description;

	using Attributes;
	using Models.Common;
	using Models.Requests.PushNotifications;
	using Repositories;
	using Models.Responses.MobileMetrics;
	using System.Threading.Tasks;

	/// <summary>
	/// The Push Notifications V1 controller.
	/// </summary>
	[RoutePrefix("PushNotifications")]
	[Version(1)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class PushNotificationsV1Controller : BaseApiController
	{
		/// <summary>
		/// An instance of <see cref="PushNotificationsRepository"/>
		/// </summary>
		private PushNotificationsRepository _repository;

		/// <summary>
		/// Returns the endpoints available.
		/// </summary>
		/// <returns>
		/// The <see cref="List{T}"/>.
		/// </returns>
		[HttpOptions, Route("")]
		public List<Link> Options()
		{
			return this.Links("Options");
		}

		/// <summary>
		/// Register device with a notification hub using an installation approach.
		/// </summary>
		/// <param name="request">
		/// The <see cref="MobileDeviceInstallationRequest">MobileDeviceInstallationRequest</see>
		/// </param>
		/// <returns>
		/// The <see cref="MobileMetricDataResponse">MobileMetricDataResponse</see> containing the all details of device metrics.
		/// </returns>
		[HttpPost, Route("RegisterWithNotificationHub")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public async Task<MobileMetricDataResponse> RegisterWithNotificationHub([FromBody] MobileDeviceInstallationRequest request)
		{
			var response = this.InitialiseResponse<MobileMetricDataResponse>();
			response.Item = await this._repository.RegisterForPushNotifications(request);
			return response;
		}

		/// <summary>
		/// Unregister device from a notification hub.
		/// </summary>
		/// <param name="request">
		/// The <see cref="MobileDeviceInstallationRequest">MobileDeviceInstallationRequest</see>
		/// </param>
		/// <returns>
		/// The <see cref="MobileMetricDataResponse">MobileMetricDataResponse</see> containing the all details of device metrics.
		/// </returns>
		[HttpDelete, Route("UnregisterFromNotificationHub")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public async Task<MobileMetricDataResponse> UnregisterFromNotificationHub([FromBody] MobileDeviceInstallationRequest request)
		{
			var response = this.InitialiseResponse<MobileMetricDataResponse>();
			response.Item = await this._repository.UnregisterForPushNotifications(request);
			return response;
		}

		/// <summary>
		/// Updates the registartion time to live for devices on hub. Do not call it once time to live has been set.
		/// </summary>
		/// <returns>
		/// True for successfull operation otherwise False.
		/// </returns>
		[HttpPut, Route("UpdateRegistrationTimeOnHub")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public async Task<BooleanResponse> UpdateRegistrationTimeOnHub()
		{
			var response = this.InitialiseResponse<BooleanResponse>();
			response.Item = await this._repository.UpdateRegistrationTimeOnHub();
			return response;
		}

		/// <summary>
		/// Initializes repository.
		/// </summary>
		protected override void Init()
		{
			this._repository = new PushNotificationsRepository(this.CurrentUser);
		}
	}
}