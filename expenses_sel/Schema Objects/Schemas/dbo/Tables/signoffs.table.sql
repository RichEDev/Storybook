CREATE TABLE [dbo].[signoffs] (
    [signoffid]          INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [groupid]            INT      NOT NULL,
    [signofftype]        TINYINT  NOT NULL,
    [relid]              INT      NOT NULL,
    [include]            INT      NOT NULL,
    [amount]             MONEY    NOT NULL,
    [notify]             INT      NOT NULL,
    [stage]              TINYINT  NOT NULL,
    [holidaytype]        INT      NOT NULL,
    [holidayid]          INT      NOT NULL,
    [onholiday]          TINYINT  NOT NULL,
    [includeid]          INT      NULL,
    [claimantmail]       BIT      NOT NULL,
    [singlesignoff]      BIT      NOT NULL,
    [sendmail]           BIT      NOT NULL,
    [displaydeclaration] BIT      NOT NULL,
    [CreatedOn]          DATETIME NULL,
    [CreatedBy]          INT      NULL,
    [ModifiedOn]         DATETIME NULL,
    [ModifiedBy]         INT      NULL
);

