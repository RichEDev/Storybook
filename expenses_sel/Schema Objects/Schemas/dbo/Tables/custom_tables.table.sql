CREATE TABLE [dbo].[custom_tables] (
    [tableid_old]             INT              IDENTITY (1, 1) NOT NULL,
    [tablename]               NVARCHAR (50)    NOT NULL,
    [jointype]                TINYINT          NULL,
    [allowreporton]           BIT              NOT NULL,
    [description]             NVARCHAR (50)    NULL,
    [primarykey]              UNIQUEIDENTIFIER NULL,
    [keyfield]                UNIQUEIDENTIFIER NULL,
    [allowimport]             BIT              NOT NULL,
    [amendedon]               DATETIME         NULL,
    [allowworkflow]           BIT              NOT NULL,
    [allowentityrelationship] BIT              NOT NULL,
    [tableid]                 UNIQUEIDENTIFIER NOT NULL,
    [hasUserDefinedFields]    BIT              NOT NULL,
    [userdefined_table]       UNIQUEIDENTIFIER NULL,
    [parentTableID]           UNIQUEIDENTIFIER NULL
);

