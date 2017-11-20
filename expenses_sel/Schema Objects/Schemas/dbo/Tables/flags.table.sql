CREATE TABLE [dbo].[flags] (
    [flagID]             INT             IDENTITY (1, 1) NOT NULL,
    [flagType]           TINYINT         NOT NULL,
    [action]             TINYINT         NOT NULL,
    [flagText]           NVARCHAR (MAX)  NOT NULL,
    [amberTolerance]     TINYINT         NULL,
    [redTolerance]       TINYINT         NULL,
    [frequency]          TINYINT         NULL,
    [period]             TINYINT         NULL,
    [periodType]         TINYINT         NULL,
    [limit]              DECIMAL (18, 2) NULL,
    [dateComparisonType] TINYINT         NULL,
    [dateToCompare]      DATETIME        NULL,
    [numberOfMonths]     TINYINT         NULL,
    [createdOn]          DATETIME        NOT NULL,
    [createdBy]          INT             NULL,
    [modifiedOn]         DATETIME        NULL,
    [modifiedBy]         INT             NULL,
    [description]        NVARCHAR (MAX)  NULL,
    [active]             BIT             NOT NULL
);

