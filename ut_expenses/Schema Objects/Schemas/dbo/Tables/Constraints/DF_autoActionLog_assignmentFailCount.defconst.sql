ALTER TABLE [dbo].[autoActionLog]
    ADD CONSTRAINT [DF_autoActionLog_assignmentFailCount] DEFAULT ((0)) FOR [assignmentFailCount];

