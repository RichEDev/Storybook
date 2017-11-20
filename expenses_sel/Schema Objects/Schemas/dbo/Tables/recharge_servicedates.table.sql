CREATE TABLE [dbo].[recharge_servicedates] (
    [serviceDateId] INT      IDENTITY (1, 1) NOT NULL,
    [rechargeId]    INT      NOT NULL,
    [offlineFrom]   DATETIME NULL,
    [onlineFrom]    DATETIME NULL
);

