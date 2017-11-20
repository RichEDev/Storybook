CREATE TABLE [dbo].[contract_productdetails_recharge] (
    [rechargeItemId]    INT      IDENTITY (1, 1) NOT NULL,
    [rechargeId]        INT      NULL,
    [rechargePeriod]    DATETIME NULL,
    [rechargeAmount]    FLOAT    NOT NULL,
    [oneOffCharge]      INT      NOT NULL,
    [contractProductId] INT      NULL,
    [rechargeEntityId]  INT      NULL
);

