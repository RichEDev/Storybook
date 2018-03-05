CREATE PROCEDURE [dbo].[SaveWalletReceipt](
	@fileExtension NVARCHAR(6)
	,@CreatedBy INT
	,@Status INT
	,@CUemployeeID INT
	,@CUdelegateID INT
)
AS
DECLARE @returnValue INT;
BEGIN

	INSERT INTO WalletReceipts( FileExtension, [Status], CreatedBy)
	VALUES(@fileExtension, @Status, @CreatedBy)

	DECLARE @walletReceiptId INT = SCOPE_IDENTITY();

	EXEC addInsertEntryToAuditLog @CUemployeeID
			,@CUdelegateID
			,199
			,@walletReceiptId
			,NULL
			,NULL;

	RETURN SCOPE_IDENTITY();
END