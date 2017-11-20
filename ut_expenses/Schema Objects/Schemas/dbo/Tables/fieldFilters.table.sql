CREATE TABLE [dbo].[fieldFilters] (
    [viewid]       INT              NULL,
    [fieldid]      UNIQUEIDENTIFIER NOT NULL,
    [condition]    TINYINT          NOT NULL,
    [value]        NVARCHAR (150)   NOT NULL,
    [order]        TINYINT          NOT NULL,
    [joinViaID]    INT              NULL,
    [valueTwo]     NVARCHAR (150)   CONSTRAINT [DF_fieldFilters_valueTwo] DEFAULT ('') NOT NULL,
    [viewFilterID] INT              IDENTITY (1, 1) NOT NULL,
    [attributeid]  INT              NULL,
    [userdefineid] INT              NULL,
    [formid] INT NULL, 
    [isParentFilter] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_fieldFilters_customEntityAttributes] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]),
    CONSTRAINT [FK_fieldFilters_customEntityViews] FOREIGN KEY ([viewid]) REFERENCES [dbo].[customEntityViews] ([viewid]) ON DELETE CASCADE,
    CONSTRAINT [FK_fieldFilters_joinVia] FOREIGN KEY ([joinViaID]) REFERENCES [dbo].[joinVia] ([joinViaID]) ON DELETE CASCADE,
    CONSTRAINT [FK_fieldFilters_userdefined] FOREIGN KEY ([userdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE CASCADE
);



