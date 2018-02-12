CREATE PROCEDURE [dbo].[GetAccounts]
AS
SELECT
accountid, companyname, companyid, contact, nousers, accounttype, expiry, dbserver, dbname, dbusername, dbpassword, archived,
quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled,
reportDatabaseID, isNHSCustomer, contactHelpDeskAllowed, postcodeAnywhereKey, licencedUsers, mapsEnabled,
ReceiptServiceEnabled, addressLookupProvider, addressLookupsChargeable, addressLookupPsmaAgreement, addressInternationalLookupsAndCoordinates,
addressLookupsRemaining, addressDistanceLookupsRemaining, licenceType, annualContract, renewalDate, contactEmail, ValidationServiceEnabled, DaysToWaitBeforeSentEnvelopeIsMissing
,PaymentServiceEnabled,[DVLALookUpKey], postCodeAnywherePaymentServiceKey FROM
dbo.registeredusers;

RETURN 0;

GO