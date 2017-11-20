ALTER TABLE [dbo].[autoActionLog]
    ADD CONSTRAINT [DF_autoActionLog_archiveFailCount] DEFAULT ((0)) FOR [archiveFailCount];

