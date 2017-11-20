CREATE TABLE [dbo].[reportcriteria] (
    [condition]     TINYINT          NOT NULL,
    [value1]        NVARCHAR (1000)  NULL,
    [value2]        NVARCHAR (1000)  NULL,
    [andor]         TINYINT          NOT NULL,
    [order]         INT              NOT NULL,
    [runtime]       BIT              NOT NULL,
    [groupnumber]   TINYINT          NULL,
    [reportID]      UNIQUEIDENTIFIER NOT NULL,
    [criteriaid]    UNIQUEIDENTIFIER NOT NULL,
    [fieldID]       UNIQUEIDENTIFIER NOT NULL,
    [oldcriteriaid] INT              IDENTITY (1, 1) NOT NULL,
    [oldreportid]   INT              NULL
);

