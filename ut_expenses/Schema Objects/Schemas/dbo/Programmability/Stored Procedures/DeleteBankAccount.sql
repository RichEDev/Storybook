CREATE PROCEDURE [dbo].[DeleteBankAccount]
@bankAccountid INT, 
@CUemployeeID INT,
@CUdelegateID INT
AS

BEGIN

	SET NOCOUNT ON;
	DECLARE @returnCode INT;
	SET @returnCode = 0;

	-- Check to see if bank account is in use
	DECLARE @count INT;
	SET @count = (SELECT COUNT (BankAccountId) FROM savedexpenses WHERE BankAccountId = @bankAccountid);
	IF @count > 0
		RETURN -2; 

	 DECLARE @tableId UNIQUEIDENTIFIER = (SELECT tableid FROM tables WHERE tablename = 'BankAccounts');
     EXEC @returnCode = dbo.checkReferencedBy @tableId, @bankAccountid;

	DECLARE @accountName INT
	IF @returnCode = 0
	BEGIN
	    SELECT @accountName=dbo.getEncryptedValue(AccountName) FROM BankAccounts WHERE BankAccountId = @bankAccountid;
		DELETE FROM BankAccounts WHERE BankAccountId = @bankAccountid;
		EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 165, @bankAccountid, @accountName, null;

		END
		

	RETURN @returnCode;	

END


