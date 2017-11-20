CREATE TABLE [dbo].[accessRoleElementDetails] (
    [roleID]       INT NOT NULL,
    [elementID]    INT NOT NULL,
    [updateAccess] BIT NOT NULL,
    [insertAccess] BIT NOT NULL,
    [deleteAccess] BIT NOT NULL,
    [viewAccess]   BIT NOT NULL
);

