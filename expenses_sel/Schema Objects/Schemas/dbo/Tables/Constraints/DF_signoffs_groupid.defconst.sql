ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_groupid] DEFAULT (0) FOR [groupid];

