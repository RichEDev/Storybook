CREATE TABLE [dbo].[mileage_thresholds] (
    [mileagethresholdid]  INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [mileagedateid]       INT      NOT NULL,
    [rangetype]           TINYINT  NOT NULL,
    [rangevalue1]         INT      NULL,
    [rangevalue2]         INT      NULL,
    [ppmpetrol]           MONEY    NOT NULL,
    [ppmdiesel]           MONEY    NOT NULL,
    [ppmlpg]              MONEY    NOT NULL,
    [amountforvatp]       MONEY    NOT NULL,
    [amountforvatd]       MONEY    NOT NULL,
    [amountforvatlpg]     MONEY    NOT NULL,
    [passenger1]          MONEY    NOT NULL,
    [passengerx]          MONEY    NOT NULL,
    [CreatedOn]           DATETIME NULL,
    [CreatedBy]           INT      NULL,
    [ModifiedOn]          DATETIME NULL,
    [ModifiedBy]          INT      NULL,
    [heavyBulkyEquipment] MONEY    NOT NULL
);

