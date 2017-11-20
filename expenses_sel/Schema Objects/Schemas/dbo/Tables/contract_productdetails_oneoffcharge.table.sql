CREATE TABLE [dbo].[contract_productdetails_oneoffcharge] (
    [chargeId]         INT      IDENTITY (1, 1) NOT NULL,
    [contractId]       INT      NOT NULL,
    [chargeDate]       DATETIME NOT NULL,
    [rechargeEntityId] INT      NOT NULL,
    [chargeAmount]     FLOAT    NOT NULL
);

