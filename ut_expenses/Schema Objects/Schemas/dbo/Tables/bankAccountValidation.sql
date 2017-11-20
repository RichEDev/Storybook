CREATE TABLE [dbo].[bankAccountValidation]
(
	[BankAccountValidationId] INT NOT NULL IDENTITY , 
    [BankAccountId] INT NULL, 
    [EmployeeId] INT NOT NULL, 
    [ValidatedOn] DATETIME NOT NULL, 
    [ValidatedBy] INT NOT NULL, 
    [Valid] BIT NOT NULL, 
    [StatusInformation] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_bankAccountValidation] PRIMARY KEY ([BankAccountValidationId]) 
)
