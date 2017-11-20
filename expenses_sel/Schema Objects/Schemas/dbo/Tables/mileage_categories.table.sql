CREATE TABLE [dbo].[mileage_categories] (
    [mileageid]       INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [carsize]         NVARCHAR (50)   NOT NULL,
    [comment]         NVARCHAR (4000) NULL,
    [calcmilestotal]  BIT             NOT NULL,
    [CreatedOn]       DATETIME        NULL,
    [CreatedBy]       INT             NULL,
    [ModifiedOn]      DATETIME        NULL,
    [ModifiedBy]      INT             NULL,
    [thresholdtype]   TINYINT         NOT NULL,
    [catvalid]        BIT             NOT NULL,
    [unit]            TINYINT         NOT NULL,
    [catvalidcomment] NVARCHAR (500)  NULL,
    [currencyid]      INT             NULL
);

