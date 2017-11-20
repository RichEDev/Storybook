CREATE TABLE [dbo].[costcodes] (
    [costcodeid]  INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [costcode]    NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [archived]    BIT             NOT NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL
);

