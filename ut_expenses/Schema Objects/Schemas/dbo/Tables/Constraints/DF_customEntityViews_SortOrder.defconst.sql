ALTER TABLE [dbo].[customEntityViews]
    ADD CONSTRAINT [DF_customEntityViews_SortOrder] DEFAULT ((0)) FOR [SortOrder];

