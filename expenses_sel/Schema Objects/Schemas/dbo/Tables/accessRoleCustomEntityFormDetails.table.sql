CREATE TABLE [dbo].[accessRoleCustomEntityFormDetails] (
    [accessRoleCustomEntityFormDetailsID] INT IDENTITY (1, 1) NOT NULL,
    [roleID]                              INT NOT NULL,
    [customEntityID]                      INT NOT NULL,
    [customEntityFormID]                  INT NOT NULL,
    [viewAccess]                          BIT NOT NULL,
    [insertAccess]                        BIT NOT NULL,
    [updateAccess]                        BIT NOT NULL,
    [deleteAccess]                        BIT NOT NULL
);

