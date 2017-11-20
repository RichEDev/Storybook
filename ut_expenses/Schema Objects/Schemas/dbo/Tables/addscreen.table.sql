CREATE TABLE [dbo].[addscreen] (
    [display]     BIT              NOT NULL,
    [mandatory]   BIT              NOT NULL,
    [code]        NVARCHAR (50)    NOT NULL,
    [description] NVARCHAR (50)    NULL,
    [individual]  BIT              NOT NULL,
    [displaycc]   BIT              NOT NULL,
    [mandatorycc] BIT              NOT NULL,
    [displaypc]   BIT              NOT NULL,
    [mandatorypc] BIT              NOT NULL,
    [createdon]   DATETIME         NULL,
    [createdby]   INT              NULL,
    [modifiedon]  DATETIME         NULL,
    [modifiedby]  INT              NULL,
    [fieldID]     UNIQUEIDENTIFIER NOT NULL
);

