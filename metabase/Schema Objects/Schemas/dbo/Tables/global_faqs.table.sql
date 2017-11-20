CREATE TABLE [dbo].[global_faqs] (
    [faqid]         INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [question]      NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [answer]        NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [tip]           NVARCHAR (200)  COLLATE Latin1_General_CI_AS NULL,
    [datecreated]   DATETIME        NOT NULL,
    [faqcategoryid] INT             NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL
);

