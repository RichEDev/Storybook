CREATE TABLE [dbo].[employeeHomeLocations] (
    [employeeLocationID] INT      IDENTITY (1, 1)  NOT NULL,
    [employeeID]         INT      NOT NULL,
    [locationID]         INT      NULL,
    [startDate]          DATETIME NOT NULL,
    [endDate]            DATETIME NULL,
    [CreatedOn]          DATETIME NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [ModifiedOn]         DATETIME NULL,
    [ModifiedBy]         INT      NULL
);

