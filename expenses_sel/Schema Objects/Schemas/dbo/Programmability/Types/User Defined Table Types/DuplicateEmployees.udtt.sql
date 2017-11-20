CREATE TYPE [dbo].[DuplicateEmployees] AS  TABLE (
    [usernameToKeep] NVARCHAR (50) NOT NULL,
    [usernameToDrop] NVARCHAR (50) NOT NULL,
    [updateDetails]  BIT           DEFAULT ((0)) NOT NULL);

