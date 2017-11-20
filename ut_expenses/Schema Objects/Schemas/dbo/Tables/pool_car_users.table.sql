CREATE TABLE [dbo].[pool_car_users] (
    [carid]       INT      NOT NULL,
    [employeeid]  INT      NOT NULL,
    [createdon]   DATETIME NULL,
    [createdby]   INT      NULL,
    [modifiedon]  DATETIME NULL,
    [modifiedby]  INT      NULL,
    [CacheExpiry] DATETIME NULL
);

