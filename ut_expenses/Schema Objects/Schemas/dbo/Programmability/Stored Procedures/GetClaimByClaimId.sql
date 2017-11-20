CREATE PROCEDURE [dbo].[GetClaimByClaimId] @claimId INT
AS
BEGIN
 SELECT claimid
  ,claimno
  ,employeeid
  ,approved
  ,paid
  ,datesubmitted
  ,datepaid
  ,[description]
  ,[status]
  ,splitApprovalStage
  ,teamid
  ,checkerid
  ,stage
  ,submitted
  ,NAME
  ,currencyid
  ,ReferenceNumber
  ,CreatedOn
  ,CreatedBy
  ,ModifiedOn
  ,ModifiedBy
  ,hasHistory
  ,currentApprover
  ,totalStageCount
  ,hasReturnedItems
  ,hasCashItems
  ,hasCreditCardItems
  ,hasPurchaseCardItems
  ,hasFlaggedItems
  ,numberOfItems
  ,startDate
  ,endDate
  ,total
  ,amountPayable
  ,numberOfReceipts
  ,numberOfUnapprovedItems
  ,creditcardtotal
  ,purchasecardtotal
  ,PayBeforeValidate
 FROM dbo.claims
 WHERE claimid = @claimId

 RETURN 0;
END
