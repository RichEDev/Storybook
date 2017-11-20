CREATE TABLE [dbo].[customEntityAttachmentFields] (
    [tableid]     UNIQUEIDENTIFIER NOT NULL,
    [fieldid]     UNIQUEIDENTIFIER NOT NULL,
    [field]       NVARCHAR (500)   NOT NULL,
    [fieldtype]   NVARCHAR (2)     NOT NULL,
    [idfield]     BIT              NOT NULL,
    [description] NVARCHAR (1000)  NOT NULL
);

