CREATE TABLE [dbo].[reports] (
    [oldreportid]       INT              IDENTITY (2436, 1) NOT FOR REPLICATION NOT NULL,
    [reportname]        NVARCHAR (150)   COLLATE Latin1_General_CI_AS NOT NULL,
    [description]       NVARCHAR (2000)  COLLATE Latin1_General_CI_AS NULL,
    [curexportnum]      INT              CONSTRAINT [DF_reports_curexportnum] DEFAULT ((1)) NOT NULL,
    [lastexportdate]    DATETIME         NULL,
    [footerreport]      BIT              CONSTRAINT [DF_reports_footerreport] DEFAULT ((0)) NOT NULL,
    [oldfooterreportid] INT              NULL,
    [oldfolderid]       INT              NULL,
    [readonly]          BIT              CONSTRAINT [DF_reports_readonly] DEFAULT ((0)) NOT NULL,
    [forclaimants]      BIT              CONSTRAINT [DF_reports_forclaimants] DEFAULT ((0)) NOT NULL,
    [allowexport]       BIT              CONSTRAINT [DF_reports_allowexport] DEFAULT ((0)) NOT NULL,
    [exporttype]        TINYINT          CONSTRAINT [DF_reports_exporttype] DEFAULT ((3)) NOT NULL,
    [CreatedOn]         DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [ModifiedOn]        DATETIME         NULL,
    [ModifiedBy]        INT              NULL,
    [staticReportSQL]   NVARCHAR (MAX)   COLLATE Latin1_General_CI_AS NULL,
    [limit]             SMALLINT         CONSTRAINT [DF_reports_limit] DEFAULT ((0)) NOT NULL,
    [basetable]         UNIQUEIDENTIFIER NOT NULL,
    [reportid]          UNIQUEIDENTIFIER CONSTRAINT [DF_reports_reportid] DEFAULT (newid()) NOT NULL,
    [folderid]          UNIQUEIDENTIFIER NULL,
    [footerreportid]    UNIQUEIDENTIFIER NULL,
    [module]			TINYINT			 NULL, 
    CONSTRAINT [PK_reports] PRIMARY KEY NONCLUSTERED ([reportid] ASC),
    CONSTRAINT [FK_reports_report_folders] FOREIGN KEY ([folderid]) REFERENCES [dbo].[report_folders] ([folderid]),
    CONSTRAINT [FK_reports_tables_base] FOREIGN KEY ([basetable]) REFERENCES [dbo].[tables_base] ([tableid])
);



