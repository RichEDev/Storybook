CREATE TABLE [dbo].[contract_productdetails_calloff] (
    [calloffId]         INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractProductId] INT      NULL,
    [subAccountId]      INT      NULL,
    [supplierId]        INT      NULL,
    [calloffQuantity]   SMALLINT NULL,
    [sublocationId]     INT      NULL
);

