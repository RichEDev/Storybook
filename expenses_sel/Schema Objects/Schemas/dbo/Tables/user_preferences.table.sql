CREATE TABLE [dbo].[user_preferences] (
    [preferenceId] INT IDENTITY (1, 1) NOT NULL,
    [subAccountId] INT NOT NULL,
    [employeeid]   INT NOT NULL,
    [categoryId]   INT NOT NULL,
    [UF1_Id]       INT NULL,
    [UF2_Id]       INT NULL
);

