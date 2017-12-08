namespace SpendManagementApi.Controllers.V2
{
	using System.Web.Http;
	using System.Web.Http.Description;

	using SpendManagementApi.Attributes;
	using SpendManagementApi.Models.Requests.MobileMetricData;
	using SpendManagementApi.Models.Responses.MobileMetrics;
	using SpendManagementApi.Repositories;

	/// <summary>
	/// The mobile metric data v2 controller.
	/// </summary>
	[Version(2)]
	[RoutePrefix("MobileMetricData")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class MobileMetricDataV2Controller : BaseApiController
	{
		/// <summary>
		/// An instance of <see cref="MobileMetricDataRepository"/>
		/// </summary>
		private MobileMetricDataRepository _repository;

		/// <summary>
		/// Updates the metric device data in the database
		/// </summary>
		/// <param name="request">
		/// The <see cref="MobileMetricDataRequest">MobileMetricDataRequest</see>.
		/// </param>
		/// <returns>
		/// The <see cref="MobileMetricDataResponse">MobileMetricDataResponse</see> containing the all details of device metrics.
		/// </returns>
		[HttpPut, Route("UpdateMobileMetricData")]
		[AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
		public MobileMetricDataResponse UpdateMobileMetricData([FromBody] MobileMetricDataRequest request)
		{
			var response = this.InitialiseResponse<MobileMetricDataResponse>();
			response.Item = this._repository.UpdateMetricData(request);
			return response;
		}

		/// <summary>
		/// Initializes repository.
		/// </summary>
		protected override void Init()
		{
			this._repository = new MobileMetricDataRepository(this.CurrentUser);
		}
	}
}