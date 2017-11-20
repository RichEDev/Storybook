CREATE TABLE [dbo].[claims_base] (
    [claimid]       INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [claimno]       INT             NOT NULL,
    [employeeid]    INT             NOT NULL,
    [approved]      BIT             NOT NULL,
    [paid]          BIT             NOT NULL,
    [datesubmitted] DATETIME        NULL,
    [datepaid]      DATETIME        NULL,
    [description]   NVARCHAR (2000) NULL,
    [status]        TINYINT         NOT NULL,
    [teamid]        INT             NULL,
    [checkerid]     INT             NULL,
    [stage]         INT             NOT NULL,
    [submitted]     BIT             NULL,
    [name]          NVARCHAR (50)   NOT NULL,
    [currencyid]    INT             NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    [CacheExpiry]   DATETIME        NULL
);

