CREATE TABLE [dbo].[scheduled_reports] (
    [scheduleid]        INT              IDENTITY (1, 1) NOT NULL,
    [employeeid]        INT              NOT NULL,
    [scheduletype]      TINYINT          NOT NULL,
    [date]              DATETIME         NULL,
    [repeat_frequency]  TINYINT          NULL,
    [week]              TINYINT          NULL,
    [calendar_days]     NVARCHAR (50)    NULL,
    [startdate]         DATETIME         NOT NULL,
    [enddate]           DATETIME         NULL,
    [starttime]         DATETIME         NOT NULL,
    [outputtype]        TINYINT          NOT NULL,
    [deliverymethod]    TINYINT          NOT NULL,
    [emailaddresses]    NVARCHAR (4000)  NULL,
    [ftpserver]         NVARCHAR (1000)  NULL,
    [ftpusername]       NVARCHAR (100)   NULL,
    [ftppassword]       NVARCHAR (100)   NULL,
    [financialexportid] INT              NULL,
    [reportID]          UNIQUEIDENTIFIER NOT NULL,
    [oldreportid]       INT              NULL,
    [emailbody]         NVARCHAR (MAX)   NULL,
    [ftpusessl]         BIT              NOT NULL
);

