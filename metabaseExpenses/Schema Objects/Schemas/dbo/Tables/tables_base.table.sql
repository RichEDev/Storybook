CREATE TABLE [dbo].[tables_base] (
    [tableid_old]             INT              IDENTITY (1241, 1) NOT FOR REPLICATION NOT NULL,
    [tablename]               NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [jointype]                TINYINT          NULL,
    [allowreporton]           BIT              CONSTRAINT [DF_tables_allowreporton] DEFAULT ((0)) NOT NULL,
    [description]             NVARCHAR (50)    COLLATE Latin1_General_CI_AS NULL,
    [primarykey]              UNIQUEIDENTIFIER NULL,
    [keyfield]                UNIQUEIDENTIFIER NULL,
    [allowimport]             BIT              CONSTRAINT [DF_tables_allowimport] DEFAULT ((0)) NOT NULL,
    [amendedon]               DATETIME         CONSTRAINT [DF_tables_amendedon] DEFAULT (getdate()) NULL,
    [allowworkflow]           BIT              CONSTRAINT [DF_tables_allowworkflow] DEFAULT ((0)) NOT NULL,
    [allowentityrelationship] BIT              CONSTRAINT [DF_tables_base_allowentityrelationsip] DEFAULT ((0)) NOT NULL,
    [tableid]                 UNIQUEIDENTIFIER CONSTRAINT [DF_tables_base_tableid_new] DEFAULT (newid()) NOT NULL,
    [hasUserDefinedFields]    BIT              CONSTRAINT [DF_tables_base_hasUserDefinedFields] DEFAULT ((0)) NOT NULL,
    [userdefined_table]       UNIQUEIDENTIFIER NULL,
    [elementID]               INT              NULL,
    [subAccountIDField]       INT              NULL,
    [tableFrom]               INT              CONSTRAINT [DF__tables_ba__table__105805DF] DEFAULT ((0)) NOT NULL,
    [relabel_param]           NVARCHAR (150)   NULL,
    [linkingTable]			  BIT			   CONSTRAINT [DF_tables_base_linkingTable] DEFAULT ((0))NOT NULL , 
    CONSTRAINT [PK_tables] PRIMARY KEY NONCLUSTERED ([tableid] ASC),
    CONSTRAINT [FK_tables_base_fields_base] FOREIGN KEY ([primarykey]) REFERENCES [dbo].[fields_base] ([fieldid]),
    CONSTRAINT [FK_tables_base_fields_base1] FOREIGN KEY ([keyfield]) REFERENCES [dbo].[fields_base] ([fieldid]),
    CONSTRAINT [FK_tables_base_tables_base] FOREIGN KEY ([userdefined_table]) REFERENCES [dbo].[tables_base] ([tableid]),
    CONSTRAINT [IX_tables_base] UNIQUE NONCLUSTERED ([tablename] ASC)
);



