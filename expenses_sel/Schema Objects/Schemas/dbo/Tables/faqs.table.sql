CREATE TABLE [dbo].[faqs] (
    [faqid]         INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [question]      NVARCHAR (4000) NOT NULL,
    [answer]        NVARCHAR (4000) NOT NULL,
    [tip]           NVARCHAR (200)  NULL,
    [datecreated]   DATETIME        NOT NULL,
    [faqcategoryid] INT             NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL
);

