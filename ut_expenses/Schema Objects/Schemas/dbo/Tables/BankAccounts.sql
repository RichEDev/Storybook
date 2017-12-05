CREATE TABLE [dbo].[BankAccounts] (
    [BankAccountId] INT             IDENTITY (1, 1) NOT NULL,
    [EmployeeId]    INT             NOT NULL,
    [AccountName]   VARBINARY (MAX) NOT NULL,
    [AccountNumber] VARBINARY (MAX) NOT NULL,
    [AccountType]   INT             NOT NULL,
    [SortCode]      VARBINARY (MAX) NULL,
    [Reference]     VARBINARY (MAX) NULL,
    [CurrencyId]    INT             NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    [CountryId] INT NULL, 
    [archived] BIT NOT NULL DEFAULT 0, 
	[SwiftCode] VARBINARY(MAX) NULL,
	[Iban] VARBINARY(MAX) NULL,
    CONSTRAINT [PK_BankAccounts] PRIMARY KEY CLUSTERED ([BankAccountId] ASC),
    CONSTRAINT [FK_BankAccounts_currencies] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE CASCADE,
    CONSTRAINT [FK_BankAccounts_employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE
);





