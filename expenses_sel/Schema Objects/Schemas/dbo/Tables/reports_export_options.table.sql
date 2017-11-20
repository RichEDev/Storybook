CREATE TABLE [dbo].[reports_export_options] (
    [employeeid]            INT              NOT NULL,
    [excelheader]           BIT              NOT NULL,
    [csvheader]             BIT              NOT NULL,
    [flatfileheader]        BIT              NOT NULL,
    [reportID]              UNIQUEIDENTIFIER NOT NULL,
    [footerid]              UNIQUEIDENTIFIER NULL,
    [drilldownreport]       UNIQUEIDENTIFIER NULL,
    [oldreportid]           INT              NULL,
    [oldfooterid]           INT              NULL,
    [olddrilldownreport]    INT              NULL,
    [delimiter]             NVARCHAR (10)    NULL,
    [removeCarriageReturns] BIT              NOT NULL,
    [encloseInSpeechMarks]  BIT              NOT NULL
);

