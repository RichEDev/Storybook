CREATE PROCEDURE [dbo].[GetBankAccount] 
@employeeId INT
AS
BEGIN

IF @employeeId>0 
 SELECT BankAccountId,EmployeeId,dbo.getDecryptedValue(AccountName) AS AccountName,dbo.getDecryptedValue(AccountNumber) AS AccountNumber,AccountType,dbo.getDecryptedValue(SortCode) AS SortCode,dbo.getDecryptedValue(Reference) AS Reference,CurrencyId,CountryId,archived AS archived, dbo.getDecryptedValue(Iban) as Iban, dbo.getDecryptedValue(SwiftCode) as SwiftCode FROM BankAccounts WHERE EmployeeId = @employeeId
ELSE
 RETURN 0

END
GO


