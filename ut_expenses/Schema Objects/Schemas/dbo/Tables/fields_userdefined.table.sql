CREATE TABLE [dbo].[fields_userdefined] (
    [fieldid]     INT             IDENTITY (1000, 1) NOT NULL,
    [tableid]     INT             NOT NULL,
    [field]       NVARCHAR (50)   NOT NULL,
    [fieldtype]   CHAR (1)        NULL,
    [description] NVARCHAR (1000) NULL,
    [comment]     NVARCHAR (4000) NULL,
    [normalview]  BIT             NOT NULL,
    [viewgroupid] INT             NOT NULL,
    [genlist]     BIT             NOT NULL,
    [width]       INT             NOT NULL,
    [cantotal]    BIT             NOT NULL,
    [printout]    BIT             NOT NULL,
    [modifiedon]  DATETIME        NULL
);

