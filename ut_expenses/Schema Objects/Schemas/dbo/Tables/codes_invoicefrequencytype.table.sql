CREATE TABLE [dbo].[codes_invoicefrequencytype] (
    [invoiceFrequencyTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId]             INT           NULL,
    [invoiceFrequencyTypeDesc] NVARCHAR (50) NULL,
    [frequencyInMonths]        SMALLINT      NULL,
    [archived]                 BIT           NOT NULL,
    [createdon]                DATETIME      NULL,
    [createdby]                INT           NULL,
    [modifiedon]               DATETIME      NULL,
    [modifiedby]               INT           NULL
);

