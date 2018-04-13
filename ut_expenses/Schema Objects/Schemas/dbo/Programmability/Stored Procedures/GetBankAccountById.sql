CREATE PROCEDURE [dbo].[GetBankAccountById] @Id INT

AS

SELECT [BankAccountId]
        ,[EmployeeId]
       ,dbo.getDecryptedValue([AccountName]) As AccountName
       ,dbo.getDecryptedValue([AccountNumber]) As AccountNumber
       ,[AccountType]
       ,dbo.getDecryptedValue([SortCode]) As SortCode
       ,dbo.getDecryptedValue([Reference]) As Reference
       ,[CurrencyId]
       ,[createdOn]
       ,[createdBy]
       ,[modifiedOn]
       ,[modifiedBy]
       ,[CountryId]
       ,[archived]
       ,dbo.getDecryptedValue([SwiftCode]) As SwiftCode
       ,dbo.getDecryptedValue([Iban]) As Iban
  FROM [dbo].[BankAccounts]
  WHERE BankAccountId = @Id