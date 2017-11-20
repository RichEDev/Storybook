CREATE TABLE [dbo].[messages] (
    [messageid]   TINYINT         NOT NULL,
    [message]     NTEXT           COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [send]        BIT             NOT NULL,
    [subject]     NVARCHAR (4000) NOT NULL,
    [description] NVARCHAR (2000) NULL,
    [direction]   TINYINT         NOT NULL,
    [sendnote]    BIT             NOT NULL,
    [note]        NTEXT           COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL
);

