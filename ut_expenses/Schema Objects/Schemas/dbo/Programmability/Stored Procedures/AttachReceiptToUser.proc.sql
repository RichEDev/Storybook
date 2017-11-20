CREATE PROCEDURE [dbo].[AttachReceiptToUser]
 @receiptId int,
 @employeeId int,
 @userId int
AS
BEGIN
 DECLARE @oldClaimId int = (SELECT ClaimId from Receipts WHERE ReceiptId = @receiptId);
 DECLARE @textReceiptId nvarchar(100) = CONVERT(nvarchar, @receiptId);
 DECLARE @textEmployeeId nvarchar(100) = CONVERT(nvarchar, @employeeId);
 DECLARE @textOldClaimId nvarchar(100) = CONVERT(nvarchar, @oldClaimId);
 DECLARE @textOldClaimName nvarchar(100);
 SELECT @textOldClaimName = CONVERT(nvarchar, [name]) FROM claims WHERE claimid = @oldClaimId;

 DECLARE @log nvarchar(4000) = 'Receipt (' + @textReceiptId + ') detached from a claim header (' + @textOldClaimName + ') and linked to an employee (' + @textEmployeeId + ')';

 UPDATE Receipts
 SET UserId = @employeeId, ClaimId = NULL, ModifiedBy = @userId, ModifiedOn = CURRENT_TIMESTAMP
 WHERE ReceiptId = @receiptId; 

    EXEC addUpdateEntryToAuditLog @userId, @userId, 186, @receiptId, NULL, @textOldClaimId, @textEmployeeId, @log, NULL;
return @receiptId
END
GO