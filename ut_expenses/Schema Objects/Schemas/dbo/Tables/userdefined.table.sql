﻿CREATE TABLE [dbo].[userdefined] (
    [userdefineid]            INT              IDENTITY (1, 1) NOT NULL,
    [attribute_name]          NVARCHAR (100)   NULL,
    [fieldtype]               TINYINT          CONSTRAINT [DF_userdefined_fieldtype] DEFAULT (0) NOT NULL,
    [specific]                BIT              CONSTRAINT [DF_userdefined_specific] DEFAULT (0) NOT NULL,
    [mandatory]               BIT              CONSTRAINT [DF_userdefined_mandatory] DEFAULT (0) NOT NULL,
    [description]             NVARCHAR (4000)  NULL,
    [order]                   INT              CONSTRAINT [DF_userdefined_order] DEFAULT (0) NOT NULL,
    [CreatedOn]               DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [ModifiedOn]              DATETIME         NULL,
    [ModifiedBy]              INT              NULL,
    [tooltip]                 NVARCHAR (4000)  NULL,
    [display_name]            NVARCHAR (100)   NOT NULL,
    [maxlength]               INT              NULL,
    [format]                  TINYINT          NULL,
    [defaultvalue]            NVARCHAR (50)    NULL,
    [fieldid]                 UNIQUEIDENTIFIER CONSTRAINT [DF_userdefined_fieldid] DEFAULT (newid()) NOT NULL,
    [tableid]                 UNIQUEIDENTIFIER NULL,
    [groupID]                 INT              NULL,
    [archived]                BIT              CONSTRAINT [DF_userdefined_archived] DEFAULT ((0)) NOT NULL,
    [precision]               TINYINT          NULL,
    [allowSearch]             BIT              CONSTRAINT [DF__userdefin__allow__046B5AC3] DEFAULT ((0)) NOT NULL,
    [hyperlinkText]           NVARCHAR (MAX)   NULL,
    [hyperlinkPath]           NVARCHAR (MAX)   NULL,
    [relatedTable]            UNIQUEIDENTIFIER CONSTRAINT [DF_userdefined_relatedTable] DEFAULT (NULL) NULL,
    [displayField]            UNIQUEIDENTIFIER NULL,
    [maxRows]                 INT              NULL,
    [allowEmployeeToPopulate] BIT              CONSTRAINT [DF_userdefined_allowEmployeeToPopulate] DEFAULT ((0)) NOT NULL,
	[Encrypted]				  BIT			   NOT NULL DEFAULT ((0))
    CONSTRAINT [PK_userdefined] PRIMARY KEY NONCLUSTERED ([userdefineid] ASC),
    CONSTRAINT [FK_userdefined_userdefinedGroupings] FOREIGN KEY ([groupID]) REFERENCES [dbo].[userdefinedGroupings] ([userdefinedGroupID]) ON DELETE SET NULL
);



