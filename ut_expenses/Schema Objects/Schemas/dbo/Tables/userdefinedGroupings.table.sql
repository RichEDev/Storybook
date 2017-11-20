CREATE TABLE [dbo].[userdefinedGroupings] (
    [userdefinedGroupID] INT              IDENTITY (1, 1)  NOT NULL,
    [groupName]          NVARCHAR (50)    NOT NULL,
    [order]              INT              NOT NULL,
    [associatedTable]    UNIQUEIDENTIFIER NOT NULL,
    [createdOn]          DATETIME         NOT NULL,
    [createdBy]          INT              NOT NULL,
    [modifiedOn]         DATETIME         NULL,
    [modifiedBy]         INT              NULL
);

