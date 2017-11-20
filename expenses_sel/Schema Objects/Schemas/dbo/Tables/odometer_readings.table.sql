CREATE TABLE [dbo].[odometer_readings] (
    [odometerid]      INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [carid]           INT      NOT NULL,
    [datestamp]       DATETIME NULL,
    [oldreading]      INT      NOT NULL,
    [newreading]      INT      NULL,
    [businessmileage] BIT      NULL,
    [createdon]       DATETIME NULL,
    [createdby]       INT      NULL,
    [modifiedon]      DATETIME NULL,
    [modifiedby]      INT      NULL
);

