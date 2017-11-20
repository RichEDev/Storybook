CREATE TABLE [dbo].[codes_inflatormetrics] (
    [metricId]             INT             IDENTITY (1, 1) NOT NULL,
    [name]                 VARCHAR (20)    NULL,
    [old_Percentage]       FLOAT           NULL,
    [subAccountId]         INT             NOT NULL,
    [old_requiresExtraPct] INT             NULL,
    [percentage]           DECIMAL (18, 2) NOT NULL,
    [requiresExtraPct]     BIT             NOT NULL,
    [createdOn]            DATETIME        NULL,
    [createdBy]            INT             NULL,
    [modifiedOn]           DATETIME        NULL,
    [modifiedBy]           INT             NULL,
    [archived]             BIT             NOT NULL
);

