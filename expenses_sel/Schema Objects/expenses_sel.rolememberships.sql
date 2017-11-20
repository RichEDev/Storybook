EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'expenseuser';
GO
EXECUTE sp_addrolemember @rolename = N'dbo', @membername = N'spenduser';
GO


