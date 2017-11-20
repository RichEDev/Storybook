ALTER TABLE [dbo].[customEntityViews]
    ADD CONSTRAINT [DF_customEntityViews_BuiltIn] DEFAULT ((0)) FOR [BuiltIn];