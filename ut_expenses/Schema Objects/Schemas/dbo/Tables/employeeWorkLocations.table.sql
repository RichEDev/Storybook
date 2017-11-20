CREATE TABLE [dbo].[employeeWorkLocations] (
    [employeeLocationID] INT      IDENTITY (1, 1)  NOT NULL,
    [employeeID]         INT      NOT NULL,
    [locationID]         INT      NOT NULL,
    [startDate]          DATETIME NULL,
    [endDate]            DATETIME NULL,
    [active]             BIT      NOT NULL,
    [temporary]          BIT      NOT NULL,
    [createdOn]          DATETIME NOT NULL,
    [createdBy]          INT      NOT NULL,
    [modifiedOn]         DATETIME NULL,
    [modifiedBy]         INT      NULL
);

