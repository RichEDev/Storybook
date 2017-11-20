CREATE TABLE [dbo].[CustomEntityDisabledModuleMenuView] (
    [ViewId]   INT NOT NULL,
    [ModuleId] INT NOT NULL,
    [Menuid]   INT NOT NULL,
    CONSTRAINT [PK_CustomEntityDisabledModuleMenuView] PRIMARY KEY CLUSTERED ([ViewId] ASC, [ModuleId] ASC, [Menuid] ASC),
    CONSTRAINT [FK_CustomEntityDisabledModuleMenuView_customEntityViews] FOREIGN KEY ([ViewId]) REFERENCES [dbo].[customEntityViews] ([viewid]) ON DELETE CASCADE
);

