namespace InternalApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using InternalApi.Models;

    using Spend_Management;

    [RoutePrefix("ClaimReminders")]
    public class ClaimRemindersController : ApiController
    {
        /// <summary>
        /// To notify approvers of pending claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyApproversOfPendingClaims")]
        public IHttpActionResult NotifyApproversOfPendingClaims()
        {
            var response = new ClaimReminderResponse();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimApproverOfPendingClaims();
            }
            catch (Exception ex)
            {
                response.ErrorMessage =
                    $"Error details:{ex.Message}{Environment.NewLine} Stack Trace:{ex.StackTrace}";
            }

            return this.Json(response);
        }

        /// <summary>
        /// To notify approvers of pending claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyApproversOfPendingClaims/{accountId:int}")]
        public IHttpActionResult NotifyApproversOfPendingClaims([FromUri] int accountId)
        {
            var response = new ClaimReminderResponse();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimApproverOfPendingClaims(accountId);
            }
            catch (Exception ex)
            {
                response.ErrorMessage =
                    $"Error details:{ex.Message}{Environment.NewLine} Stack Trace:{ex.StackTrace}";
            }

            return this.Json(response);
        }

        /// <summary>
        /// To notify claimants for current claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyClaimantsOfCurrentClaims")]
        public IHttpActionResult NotifyClaimantsOfCurrentClaims()
        {
            var response = new ClaimReminderResponse();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimantsOfCurrentClaims();
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message + "Error details" + ex.InnerException;
            }

            return this.Json(response);
        }

        /// <summary>
        /// To notify claimants for current claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyClaimantsOfCurrentClaims/{accountId:int}")]
        public IHttpActionResult NotifyClaimantsOfCurrentClaims([FromUri] int accountId)
        {
            var response = new ClaimReminderResponse();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimantsOfCurrentClaims(accountId);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message + "Error details" + ex.InnerException;
            }

            return this.Json(response);
        }
    }
}