ALTER TABLE [dbo].[customEntityViews]
    ADD CONSTRAINT [DF_customEntityViews_allowdelete] DEFAULT ((0)) FOR [allowdelete];

