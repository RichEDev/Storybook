CREATE TABLE [dbo].[reports] (
    [oldreportid]    INT              IDENTITY (1, 1) NOT NULL,
    [employeeid]     INT              NOT NULL,
    [reportname]     NVARCHAR (150)   NOT NULL,
    [description]    NVARCHAR (2000)  NULL,
    [personalreport] BIT              NOT NULL,
    [curexportnum]   INT              NOT NULL,
    [lastexportdate] DATETIME         NULL,
    [footerreport]   BIT              NOT NULL,
    [readonly]       BIT              NOT NULL,
    [forclaimants]   BIT              NOT NULL,
    [allowexport]    BIT              NOT NULL,
    [exporttype]     TINYINT          NOT NULL,
    [CreatedOn]      DATETIME         NULL,
    [CreatedBy]      INT              NULL,
    [ModifiedOn]     DATETIME         NULL,
    [ModifiedBy]     INT              NULL,
    [reportID]       UNIQUEIDENTIFIER NOT NULL,
    [folderID]       UNIQUEIDENTIFIER NULL,
    [basetable]      UNIQUEIDENTIFIER NOT NULL,
    [limit]          SMALLINT         NOT NULL,
    [subAccountId]   INT              NULL
);

