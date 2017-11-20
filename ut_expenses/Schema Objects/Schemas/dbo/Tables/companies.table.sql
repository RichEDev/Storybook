CREATE TABLE [dbo].[companies] (
    [companyid]             INT             IDENTITY (1, 1) NOT NULL,
    [company]               NVARCHAR (250)  NOT NULL,
    [archived]              BIT             CONSTRAINT [DF_companies_archived] DEFAULT (0) NOT NULL,
    [comment]               NVARCHAR (4000) NULL,
    [companycode]           NVARCHAR (60)   NULL,
    [showfrom]              BIT             CONSTRAINT [DF_companies_showfrom] DEFAULT (0) NOT NULL,
    [showto]                BIT             CONSTRAINT [DF_companies_showto] DEFAULT (0) NOT NULL,
    [CreatedOn]             DATETIME        NULL,
    [CreatedBy]             INT             NULL,
    [ModifiedOn]            DATETIME        NULL,
    [ModifiedBy]            INT             NULL,
    [address1]              NVARCHAR (250)  NULL,
    [address2]              NVARCHAR (250)  NULL,
    [city]                  NVARCHAR (250)  NULL,
    [county]                NVARCHAR (250)  NULL,
    [postcode]              NVARCHAR (250)  NULL,
    [country]               INT             NULL,
    [parentcompanyid]       INT             NULL,
    [iscompany]             BIT             CONSTRAINT [DF_companies_iscompany] DEFAULT ((0)) NOT NULL,
    [CacheExpiry]           DATETIME        NULL,
    [addressCreationMethod] TINYINT         CONSTRAINT [DF_companies_addressCreationMethod] DEFAULT ((1)) NOT NULL,
    [isPrivateAddress]      BIT             CONSTRAINT [DF_companies_isPrivateAddress] DEFAULT ((0)) NOT NULL,
    [addressLookupDate]     DATETIME        NULL,
    [subAccountID]          INT             NULL,
    [address3]              NVARCHAR (250)  NULL,
    [CapturePlusId] NVARCHAR(40) NULL
);



