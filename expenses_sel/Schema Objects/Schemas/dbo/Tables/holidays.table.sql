CREATE TABLE [dbo].[holidays] (
    [holidayid]  INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [employeeid] INT      NOT NULL,
    [startdate]  DATETIME NOT NULL,
    [enddate]    DATETIME NOT NULL,
    [CreatedOn]  DATETIME NULL,
    [CreatedBy]  INT      NULL,
    [ModifiedOn] DATETIME NULL,
    [ModifiedBy] INT      NULL
);

