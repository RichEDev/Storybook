CREATE TABLE [dbo].[customEntityViewFields] (
    [viewid]      INT              NOT NULL,
    [fieldid]     UNIQUEIDENTIFIER NOT NULL,
    [order]       TINYINT          NOT NULL,
    [viewFieldId] INT              IDENTITY (1, 1) NOT NULL,
    [joinViaID]   INT              NULL,
    CONSTRAINT [PK_customEntityViewFields] PRIMARY KEY CLUSTERED ([viewFieldId] ASC),
    CONSTRAINT [FK_customEntityViewFields_customEntityViews] FOREIGN KEY ([viewid]) REFERENCES [dbo].[customEntityViews] ([viewid]) ON DELETE CASCADE,
    CONSTRAINT [FK_customEntityViewFields_joinVia] FOREIGN KEY ([joinViaID]) REFERENCES [dbo].[joinVia] ([joinViaID]) ON DELETE CASCADE
);



