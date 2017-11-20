CREATE TABLE [dbo].[tables_base] (
    [tableid_old]             INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [tablename]               NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [jointype]                TINYINT          NULL,
    [allowreporton]           BIT              NOT NULL,
    [description]             NVARCHAR (50)    COLLATE Latin1_General_CI_AS NULL,
    [primarykey]              UNIQUEIDENTIFIER NULL,
    [keyfield]                UNIQUEIDENTIFIER NULL,
    [allowimport]             BIT              NOT NULL,
    [amendedon]               DATETIME         NULL,
    [allowworkflow]           BIT              NOT NULL,
    [allowentityrelationship] BIT              NOT NULL,
    [tableid]                 UNIQUEIDENTIFIER NOT NULL,
    [hasUserDefinedFields]    BIT              NOT NULL,
    [userdefined_table]       UNIQUEIDENTIFIER NULL,
    [elementID]               INT              NULL,
    [subAccountIDField]       INT              NULL,
    [tableFrom]               INT              DEFAULT ((0)) NOT NULL
);

