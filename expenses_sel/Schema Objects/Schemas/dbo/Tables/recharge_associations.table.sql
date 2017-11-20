CREATE TABLE [dbo].[recharge_associations] (
    [rechargeId]              INT      IDENTITY (1, 1) NOT NULL,
    [rechargeEntityId]        INT      NULL,
    [contractProductId]       INT      NULL,
    [apportionId]             INT      NOT NULL,
    [portion]                 INT      NOT NULL,
    [surcharge]               FLOAT    NOT NULL,
    [surchargeType]           INT      NOT NULL,
    [rechargePeriod]          DATETIME NULL,
    [rechargeAmount]          FLOAT    NOT NULL,
    [supportStartDate]        DATETIME NULL,
    [warrantyEndDate]         DATETIME NULL,
    [postWarrantyApportionId] INT      NOT NULL,
    [postWarrantyPortion]     FLOAT    NOT NULL,
    [supportEndDate]          DATETIME NULL
);

