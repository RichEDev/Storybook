CREATE TABLE [dbo].[global_faqs] (
    [faqid]         INT             IDENTITY (59, 1) NOT FOR REPLICATION NOT NULL,
    [question]      NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [answer]        NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [tip]           NVARCHAR (200)  COLLATE Latin1_General_CI_AS NULL,
    [datecreated]   DATETIME        CONSTRAINT [DF_faqs_datecreated] DEFAULT (getdate()) NOT NULL,
    [faqcategoryid] INT             NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    CONSTRAINT [PK_faqs] PRIMARY KEY CLUSTERED ([faqid] ASC),
    CONSTRAINT [FK_faqs_global_faqcategories] FOREIGN KEY ([faqcategoryid]) REFERENCES [dbo].[global_faqcategories] ([faqcategoryid]) NOT FOR REPLICATION
);



