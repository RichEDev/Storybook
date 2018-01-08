using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
using System.ServiceModel;


namespace Spend_Management.expenses.code
{
    public class SendMessageEnum
    {
        [Obsolete] //use the one in the SpendManagementLibrary EnumHelpers.cs
        public static Guid GetEnumDescription(Enum EnumConstant)
        {
            if (EnumConstant != null)
            {
                FieldInfo fi = EnumConstant.GetType().GetField(EnumConstant.ToString());
                DescriptionAttribute[] attr = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attr.Length > 0)
                {
                    return new Guid(attr[0].Description);
                }
                //return EnumConstant.ToString();
            }
            return Guid.Empty;
        }

    }

    /// <summary>
    /// enum is useing for message description
    /// </summary>
    public enum SendMessageDescription
    {
        /// <summary>
        /// Sent to a new employee after they have been added
        /// </summary>
        [Description("BBE82BE2-473E-4403-8F21-4DCF2AD98D4C")]
        SentToANewEmployeeAfterTheyHaveBeenAdded = 1,

        /// <summary>
        /// Sent to an administrator after a set of expenses have been submitted
        /// </summary>
        [Description("F929969F-B2F3-4B98-9252-7AE6B17A418B")]
        SentToAnAdministratorAfterASetOfExpensesHaveBeenSubmitted = 2,

        /// <summary>
        /// Sent to a user if an expense item gets returned
        /// </summary>
        [Description("43088AC9-222F-4794-9F8D-2CA8C7DE4951")]
        SentToAUserIfAnExpenseItemGetsReturned = 3,

        /// <summary>
        /// Sent to notify administrator of any returned expenses being corrected"
        /// </summary>
        [Description("0ff16be2-9bf2-4413-b539-bcdaaf428331")]
        SentToNotifyAdministratorOfAnyReturnedExpensesBeingCorrected = 4,

        /// <summary>
        /// Sent when an employee gets locked out"
        /// </summary>
        [Description("03803b7d-e690-46fe-ad04-5cb5babd9a6d")]
        SentWhenAnEmployeeGetsLockedOut = 5,

        /// <summary>
        /// Sent to notify a user their expenses have been paid
        /// </summary>
        [Description("29258532-0fc1-4630-97df-1699ed4c5510")]
        SentToNotifyAUserTheirExpensesHaveBeenPaid = 6,

        /// <summary>
        /// This e-mail is sent to notify users of a claim being made
        /// </summary>
        [Description("92a8bf8b-b936-43af-b62a-c1a80e8f6425")]
        ThisEmailIsSentToNotifyUsersOfAClaimBeingMade = 8,

        /// <summary>
        /// Sent to an administrator to request an advance
        /// </summary>
        [Description("a5bad4f0-3a11-4898-8490-ed1b81f538da")]
        SentToAnAdministratorToRequestAnAdvance =9,

        /// <summary>
        /// Sent to a claimant when an item has been deleted
        /// </summary>
        [Description("4adf5146-a0bd-4ba4-81ce-0d7027b95358")]
        SentToAClaimantWhenAnItemHasBeenDeleted= 10,
        /// <summary>
        /// Sent to a claimant when a claim reaches the next stage in the approval process
        /// </summary>
        [Description("2d9985d7-72b2-4f82-879c-b3ca5ef22312")]
        SentToAClaimantWhenAClaimReachesTheNextStageInTheApprovalProcess = 11,
        /// <summary>
        /// Sent to an administrator to notify them of a advance being requested
        /// </summary>
        [Description("4dcda127-60c4-4478-a79f-f1677ae8257b")]
        SentToAnAdministratorToNotifyThemOfAAdvanceBeingRequested = 12,
        /// <summary>
        /// Send to a claimant to notify them that their advance has been approved
        /// </summary>
        [Description("e6fa3456-1670-42ee-94ca-7b7d796a3ffb")]
        SendToAClaimantToNotifyThemThatTheirAdvanceHasBeenApproved = 13,
        /// <summary>
        /// Send to a claimant to notify them that their advance has been rejected
        /// </summary>
        [Description("1e581255-e673-46a2-8615-fb13a2c1e8c9")]
        SendToAClaimantToNotifyThemThatTheirAdvanceHasBeenRejected = 14,
        /// <summary>
        /// Sent to an administrator to notify them an advance has been disputed
        /// </summary>
        [Description("9c98115f-da75-48d6-8124-42ef20c5d2a7")]
        SentToAnAdministratorToNotifyThemAnAdvanceHasBeenDispute = 15,
        /// <summary>
        /// Sent to an claimant to notify them if their claim has been unsubmitted by an administrator
        /// </summary>
        [Description("3b5042c0-69da-4d7b-85cd-d740569c633e")]
        SentToAnClaimantToNotifyThemIfTheirClaimHasBeenUnsubmittedByAnAdministrator = 17,
        /// <summary>
        /// This e-mail is sent to notify users their purchase card statement is ready
        /// </summary>
        [Description("867bb8a3-c616-4284-bbc2-592188953ced")]
        ThisEmailIsSentToNotifyUsersTheirPurchaseCardStatementIsReady = 18,
        /// <summary>
        /// Sent to a claimant when their envelope is received for Scan & Attach
        /// </summary>
        [Description("3fc4c3e9-b122-44cf-8761-0a0327ee6c6b")]
        SentToAClaimantWhenTheirEnvelopeIsReceivedForScanAndAttach = 19,
        /// <summary>
        /// Sent to a claimant when their envelope is marked as sent, but has not been received after a specified number of days
        /// </summary>
        [Description("000e02ad-baf6-4983-9472-23f9345feadc")]
        SentToAClaimantWhenTheirEnvelopeIsMarkedAsSentButHasNotBeenReceivedAfterAspecifiedNumberOfDays = 20,
        /// <summary>
        /// Sent to a claimant when an expense fails receipt validation
        /// </summary>
        [Description("60086E03-E781-450F-B67B-F7DD60ABEAAE")]
        SentToAClaimantWhenAnExpenseFailsReceiptValidation = 21,
        /// <summary>
        /// Sent to an account administrator when a previously unidentified envelope has been attached to their account
        /// </summary>
        [Description("906D9D33-186F-428D-AD8A-B4E442182D7A")]
        SentToAnAccountAdministratorWhenAPreviouslyUnidentifiedEnvelopeHasBeenAttachedToTheirAccount = 22,
        /// <summary>
        /// Sent to a claimant when an receipt matching for all expenses cannot be fully completed
        /// </summary>
        [Description("DE4EB218-C432-4804-ABAD-5276B1523E9E")]
        SentToAClaimantWhenAnReceiptMatchingForAllExpenseCannotBeFullyCompleted = 23,
        /// <summary>
        /// Sent To A Claimant When Payment Service Mark Claim As Reimbursed
        /// </summary>
        [Description("618DB425-F430-4660-9525-EBAB444ED754")]
        SentToAClaimantWhenPaymentServiceMarkClaimAsReimbursed = 24,
        /// <summary>
        /// Sent to a line manager when a duty of care document is due to expire.
        /// </summary>
        [Description("7F9FC6FB-E6AA-4D97-8342-A405803DB253")]
        SentToAsApproverWhenClaimantsDocIsAboutToExpire = 25,
        /// <summary>
        /// Sent to a claimant when a duty of care document is due to expire.
        /// </summary>
        [Description("74cc59f4-61b5-4e66-931a-8710bc86022d")]
        SentToAClaimantWhenADutyOfCareDocumentIsDueToExpire = 26,

        /// Sent to an approver who has pending claims
        /// </summary>
        [Description("56FD1DE3-AE19-4857-9541-76056A2FF42F")]
        SentToAnApproverWhoHasPendingClaims = 27,

        /// <summary>
        /// This email is sent when an employee requests a password reset from a method other than a mobile device.
        /// </summary>
        [Description("F3722386-8F29-4AF9-A8F0-E0BD7EA6D81A")]
        SentWhenAnEmployeeRequestsAPasswordResetFromAMethodOtherThanAMobileDevice=1120,

        /// <summary>
        /// This email is sent when an employee requests a password reset from a mobile device.
        /// </summary>
        [Description("BC7B0397-60B1-43DE-B4C3-636EAF246ACF")]
        SentWhenAnEmployeeRequestsAPasswordResetFromAMobileDevice=1121,

        /// <summary>
        /// Sent when an employee requests a password reset
        /// </summary>
        [Description("70A54C39-9EAE-481F-9D69-6BA8706C7A36")]
        SentWhenAnEmployeeRequestsAPasswordReset = 1122,

        /// <summary>
        /// This email is sent when an employee locks account from a method other than a mobile device.
        /// </summary>
        [Description("85DD50AA-BF2F-48F6-A1FB-5B49C4EC0A9D")]
        SentWhenAnEmployeeLocksAccountFromAMethodOtherThanAMobileDevice = 1123,

        /// <summary>
        /// This email is sent when an employee locks account from a mobile device.
        /// </summary>
        [Description("B2B3C3CA-5116-48BA-A7D9-2C59AE6C80FE")]
        SentWhenAnEmployeeLocksAccountFromAMobileDevice = 1124,

        /// <summary>
        /// This email is sent when an employee requests a password reset from the Expenses Mobile app.
        /// </summary>
        [Description("D7AB44ED-97F7-4A5F-B714-570114579EC4")]
        SentWhenAnEmployeeRequestsAPasswordResetFromExpensesMobile = 1125,

        /// <summary>
        /// This email is sent when an employee locks their account from expenses mobile.
        /// </summary>
        [Description("6dca72f8-4fcb-44fe-9e90-4e8b080aa0e7")]
        SentWhenAnEmployeeLocksAccountFromExpensesMobile = 1126,

        /// <summary>
        /// Sent when a vehicle has been added
        /// </summary>
        [Description("cf180b21-4d19-4cd1-b5e5-5266c3d268c8")]
        SentWhenaVehicleHasBeenAdded = 2119,
        /// <summary>
        /// Sent when a vehicle has been activated for use
        /// </summary>
        [Description("193619b5-3a41-42f9-9d8e-93e89ae95040")]
        SentWhenaVehicleHasBeenActivatedForUse = 2120,
        /// <summary>
        /// This e-mail is sent to notify users their credit card statement is ready
        /// </summary>
        [Description("43766A2A-F76B-486A-9BC1-BE69E3EA9C84")]
        ThisEmailIsSentToNotifyUsersTheirCreditCardStatementIsReady = 105,
        /// <summary>
        /// Sent to an claimant to notify them their advance has been paid
        /// </summary>
        [Description("a79c6af4-3c58-46c9-93be-4056e777ea03")]
        SentToAClaimantToNotifyThemTheirAdvanceHasBeenPaid = 114,
        /// <summary>
        ///Sent when audit log cleared 
        /// </summary>
        [Description("456DA9A4-747A-4D9B-A275-46BFDC5CB6AA")]
        SentWhenAnEmployeeClearAuditLog =28,       

        /// <summary>
        /// Sent to claimant to remind them about unsubmitted claims based on frequency configured in general optiion
        /// </summary>
        [Description("386F8F07-1FA8-41D1-A800-9EF0AADACC60")]
        SentToAClaimantToRemindThemOfUnsubmittedClaims=29,

        /// <summary>
        ///Sent when claimant unsubmit the claim
        /// </summary>
        [Description("5AB44BD8-ACFE-481A-8BC3-66A4A15DFADF")]
        SentToArroverWhenClaimIsUnsubmittedByClaimant = 30,

        /// <summary>
        /// Sent when claimant submit request for consent portal access with the information like url for the portal and security code to access the portal to submit the DVLA consent
        /// </summary>
        [Description("61C92AE2-2FAA-4E81-9B7A-71F25AE32936")]
        SentToClaimantWhenConsentPortalAccessRequestIsSubmitted = 31,

        /// <summary>
        /// Sent to a claimant when consent is due to expire.
        /// </summary>
        [Description("4457F8DF-BDB1-4C5A-BFA1-1B1FA3C52DF6")]
        SentToClaimantWhenConsentIsDueToExpire = 32,

        /// <summary>
        /// Sent to an employee when their account is activated 
        /// </summary>
        [Description("81EBC966-2D61-4C1A-BB39-B3D9BA9F5544")]
        SentToAnEmployeeAfterTheirAccountHaveBeenActivated = 33,

        /// <summary>
        /// Sent to claimant for invalid driving licence.
        /// </summary>
        [Description("2599CFCA-9A3A-4530-906A-3EC5C697F6C6")]
        SentToClaimantForInvalidDrivingLicence = 35,

        /// <summary>
        /// Sent to approver when claimant driving licence is invalid.
        /// </summary>
        [Description("EC677111-ABE6-45F3-A47E-3BF706186ACE")]
        SentToApproverWhenClaimantDrivingLicenceIsInvalid = 36,

        /// <summary>
        /// Sent to approver when claimant driving licence is due to expire.
        /// </summary>
        [Description("6CDA99DA-3F42-4084-BF7A-E016598F12E9")]
        SentToClaimantWhenDrivingLicenceReviewIsDueToExpire = 37,

        /// <summary>
        /// Sent to an administrator when an automatic card import fails for some reason.
        /// </summary>
        [Description("56EC0718-A570-4E06-9D41-791D68FFDA11")]
        SentToAnAdministratorWhenACardImportFails = 38,

        /// <summary>
        /// Sent to approver when claimant opts out of consent check.
        /// </summary>
        [Description("63F2598D-B6BF-4CC3-9772-B378907E392C")]
        SentToApproverWhenClaimantOptsOutOfConsentCheck = 39,

        /// <summary>
        /// Sent to an approver when claimant denies consent check.
        /// </summary>
        [Description("5483FCA4-5E65-45EE-BB93-15104E61267C")]
        SentToApproverWhenClaimantDeniesConsentCheck = 40
    }
}