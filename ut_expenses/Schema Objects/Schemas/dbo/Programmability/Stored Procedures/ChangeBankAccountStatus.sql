CREATE PROCEDURE [dbo].[ChangeBankAccountStatus]
@bankAccountId INT,
@archive INT,
@CUemployeeId INT,
@CUdelegateID INT
AS

DECLARE @count INT;
DECLARE @oldarchive BIT;
DECLARE @recordtitle NVARCHAR(2000);
DECLARE @BankAccountElement int

SELECT @BankAccountElement=elementID from Elements where elementFriendlyName='Bank Accounts'
SELECT @oldarchive = archived, @recordtitle = dbo.getDecryptedValue(AccountName) FROM BankAccounts WHERE BankAccountId = @bankAccountId;
UPDATE BankAccounts SET archived = @archive WHERE BankAccountId = @bankAccountId;

IF @oldarchive <> @archive
    EXEC addUpdateEntryToAuditLog @CUemployeeId, null, @BankAccountElement, @BankAccountId, '4FD4EAAD-0B04-4890-93D1-56D53EBFB0BA', @oldarchive, @archive, @recordtitle, null;


GO


