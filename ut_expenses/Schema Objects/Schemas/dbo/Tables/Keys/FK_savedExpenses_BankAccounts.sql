ALTER TABLE dbo.savedExpenses 
ADD CONSTRAINT FK_savedExpenses_BankAccounts FOREIGN KEY
(
	BankAccountId
)
REFERENCES dbo.BankAccounts
(
	BankAccountId
) 
ON DELETE NO ACTION