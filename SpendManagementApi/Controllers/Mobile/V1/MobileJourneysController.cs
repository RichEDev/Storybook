namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    /// <summary>
    /// The controller handling journeys from mobile devices.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileJourneysV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Saves journeys for a mobile user from their device.
        /// </summary>
        /// <param name="journeys">A list of <see cref="MobileJourney">journeys</see> to save.</param>
        /// <returns>A <see cref="SaveJourneyResult"/> detailing success or failure.</returns>
        [HttpPost]
        [MobileAuth]
        [Route("mobile/journeys/save")]
        public SaveJourneyResult SaveJourneys([FromBody]List<MobileJourney> journeys)
        {
            SaveJourneyResult result = new SaveJourneyResult { FunctionName = "SaveJourneys", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                if (journeys == null)
                {
                    result.ReturnCode = MobileReturnCode.InvalidJourneys;
                }
                else
                {
                    cMobileDevices clsmobile = new cMobileDevices(this.PairingKeySerialKey.PairingKey.AccountID);
                    MobileDevice curDevice = clsmobile.GetDeviceByPairingKey(this.PairingKeySerialKey.PairingKey.Pairingkey);

                    var ids = new SortedList<string, int>();
                    foreach (MobileJourney journey in journeys)
                    {
                        int id = clsmobile.SaveMobileJourney(
                            this.PairingKeySerialKey.PairingKey.EmployeeID,
                            journey.SubcatId,
                            journey.JourneyJson,
                            curDevice.DeviceType.DeviceTypeId,
                            journey.JourneyDateTime,
                            journey.JourneyStartTime,
                            journey.JourneyEndTime,
                            journey.Active);

                        ids.Add("MobileID", journey.JourneyId);
                        ids.Add("ServerID", id);
                    }

                    result.List = ids;
                }
            }

            return result;
        }
    }
}
