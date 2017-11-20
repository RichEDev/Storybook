ALTER TABLE [dbo].[customEntityViews]
    ADD CONSTRAINT [DF_customEntityViews_allowapproval] DEFAULT ((0)) FOR [allowapproval];

