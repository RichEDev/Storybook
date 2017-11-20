CREATE TABLE [dbo].[reportcriteria] (
    [condition]   TINYINT          NOT NULL,
    [value1]      NVARCHAR (1000)  COLLATE Latin1_General_CI_AS NULL,
    [value2]      NVARCHAR (1000)  COLLATE Latin1_General_CI_AS NULL,
    [andor]       TINYINT          NOT NULL,
    [order]       INT              NOT NULL,
    [runtime]     BIT              NOT NULL,
    [groupnumber] TINYINT          NULL,
    [criteriaid]  UNIQUEIDENTIFIER NOT NULL,
    [reportid]    UNIQUEIDENTIFIER NOT NULL,
    [fieldID]     UNIQUEIDENTIFIER NULL
);

