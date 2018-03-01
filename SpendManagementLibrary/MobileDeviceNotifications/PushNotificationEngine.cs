namespace SpendManagementLibrary.MobileDeviceNotifications
{
	using BusinessLogic;
	using Common.Logging;
	using SpendManagementLibrary.MobileDeviceNotifications.NotificationManager;
	using SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications;
	using SpendManagementLibrary.MobileMetrics;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Threading.Tasks;

	public class PushNotificationEngine
	{
		/// <summary>
		/// An instance of <see cref="ILog"/> for logging diagnostics and information.
		/// </summary>
		private static readonly ILog Log = new LogFactory<MobileDeviceInstallationService>().GetLogger();

		/// <summary>
		/// Sets the Notification Template <see cref="NotificationTemplate"/> instance.
		/// </summary>
		private readonly NotificationTemplate notificationTemplate;

		/// <summary>
		/// Sets the checkers for claim.
		/// </summary>
		private readonly List<int> checkerIds;

		/// <summary>
		/// Sets the account id.
		/// </summary>
		private readonly int accountId;

		/// <summary>
		/// Sets the claim id.
		/// </summary>
		private readonly int claimId;

		/// <summary>
		/// Sets mobile metric data of each checkers.
		/// </summary>
		private List<MobileMetricData> mobileMetricData;

		/// <summary>
		/// Azure Hub Connection String with full access.
		/// </summary>
		private readonly string azureHubConnectionString;

		/// <summary>
		/// Azure hub Name.
		/// </summary>
		private readonly string azureHubName;

		/// /// <summary>
		/// Initializes a new instance of the <see cref="PushNotificationEngine"/> class.
		/// </summary>
		/// <param name="template">Instance of NotificationTemplate <see cref="NotificationTemplate"/></param>
		/// <param name="checkerIds">List of checkers</param>
		/// <param name="claimId">Claim Id</param>
		/// <param name="accountId">Account Id</param>
		public PushNotificationEngine(NotificationTemplate template, List<int> checkerIds, int claimId, int accountId)
		{
			this.notificationTemplate = template;
			this.checkerIds = checkerIds;
			this.accountId = accountId;
			this.claimId = claimId;

			this.azureHubConnectionString = ConfigurationManager.AppSettings["AzureHubConnectionString"];
			this.azureHubName = ConfigurationManager.AppSettings["AzureHubName"];
		}

		/// <summary>
		/// Validate all inputs before sending push messages.
		/// </summary>
		/// <returns>true if validation passed otherwise false.</returns>
		private bool Validate()
		{
			Guard.ThrowIfNull(this.checkerIds, nameof(this.checkerIds));
			Guard.ThrowIfNullOrWhiteSpace(this.notificationTemplate.MobileNotificationMessage, nameof(this.notificationTemplate.MobileNotificationMessage));
			if (this.checkerIds.Count == 0)
			{
				Log.Error("Employees list for sending push messages can not be empty.");
				return false;
			}

			if (this.notificationTemplate.CanSendMobileNotification.GetValueOrDefault() == false)
			{
				Log.Error("Template " + this.notificationTemplate.TemplateName + " has not been configured for sending push messages.");
				return false;
			}

			// IF all basic validation verified, get push notification details from DB
			this.mobileMetricData = UpdateMobileMetricData.GetPushNotificationDetailsOfEmployee(this.checkerIds, this.accountId);
			Guard.ThrowIfNull(this.checkerIds, nameof(this.mobileMetricData));
			if (this.mobileMetricData.Count == 0)
			{
				Log.Error("Push notification details for sending push messages can not be empty.");
				return false;
			}
			else
			{
				return true; // Indicates that Push engine can send messages to employees.
			}

		}

		/// <summary>
		/// The method is used for sending push messages for each employee.
		/// </summary>
		/// <retunn>A Task</retunn>
		public async Task SendPushMessagesAsync()
		{
			if (this.Validate())
			{
				string notificationParameter = checkerIds.Count == 1 ? "claim/" + claimId : "claim";
				PushNotificationTemplateFactory pushNotificationTemplateFactory = new PushNotificationTemplateFactory(this.notificationTemplate.MobileNotificationMessage, notificationParameter);

				foreach (var mobiledata in mobileMetricData)
				{
					try
					{
						var platform = this.GetProviderPlatform(mobiledata.MobilePlatform.Trim().ToLower());
						string notificationMessage = pushNotificationTemplateFactory.GetNotificationMessage(platform);
						PushNotificationManagerFactory.GetPushNotification(platform, notificationMessage, mobiledata.RegisteredTag).SendPushMessageAsync(this.azureHubConnectionString, this.azureHubName);
					}
					catch
					{
						// Do Nothing
						continue;
					}
				}
			}
		}

		/// <summary>
		/// Gets the Mobile Platform value based on provided string.
		/// </summary>
		/// <param name="platform">platform value</param>
		/// <returns><see cref="MobileNotificationPlatform"/></returns>
		/// <exception cref="ApplicationException">Throws exception when provided string does not have defined enum.</exception>
		private MobileNotificationPlatform GetProviderPlatform(string platform)
		{
			switch (platform)
			{
				case "android":
					return MobileNotificationPlatform.Gcm;
				case "ios":
					return MobileNotificationPlatform.Apns;
				case "uwp":
					return MobileNotificationPlatform.Wns;
				case "winphone":
					return MobileNotificationPlatform.Mpns;
				default:
					throw new ApplicationException("Mobile Notification platform does not match with defined enums.");
			}
		}
	}
}
