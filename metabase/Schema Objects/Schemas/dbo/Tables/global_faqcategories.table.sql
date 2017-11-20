﻿CREATE TABLE [dbo].[global_faqcategories] (
    [faqcategoryid] INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [category]      NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [CreatedOn]     DATETIME      NULL,
    [CreatedBy]     INT           NULL,
    [ModifiedOn]    DATETIME      NULL,
    [ModifiedBy]    INT           NULL
);

