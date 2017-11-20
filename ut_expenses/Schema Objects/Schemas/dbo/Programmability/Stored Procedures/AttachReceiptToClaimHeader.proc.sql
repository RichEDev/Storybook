CREATE PROCEDURE [dbo].[AttachReceiptToClaimHeader]
 @receiptId int,
 @claimId int,
 @userId int
AS
BEGIN
 UPDATE Receipts
 SET ClaimId = @claimId, UserId = NULL, ModifiedBy = @userId, ModifiedOn = CURRENT_TIMESTAMP
 WHERE ReceiptId = @receiptId;

  DECLARE @textReceiptId nvarchar(100) = CONVERT(nvarchar, @receiptId);
  DECLARE @textClaimId nvarchar(100) = CONVERT(nvarchar, @claimId);
  DECLARE @textClaimName nvarchar(100);
  SELECT @textClaimName = [name] FROM claims WHERE claimid = @claimId;

 DECLARE @log nvarchar(4000) = 'Receipt (' + @textReceiptId + ') attached to claim header (' + @textClaimName + ')';
 EXEC addUpdateEntryToAuditLog @userId, @userId, 186, @receiptId, NULL, NULL, @textClaimId, @log, NULL;

 RETURN @receiptId;
END
GO