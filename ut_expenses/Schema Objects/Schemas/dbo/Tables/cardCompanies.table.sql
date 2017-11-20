CREATE TABLE [dbo].[cardCompanies] (
    [cardCompanyID] INT           IDENTITY (1, 1) NOT NULL,
    [companyName]   NVARCHAR (80) NOT NULL,
    [companyNumber] NVARCHAR (80) NOT NULL,
    [usedForImport] BIT           NOT NULL,
    [createdOn]     DATE          NULL,
    [createdBy]     INT           NULL,
    [modifiedOn]    DATE          NULL,
    [modifiedBy]    INT           NULL
);

