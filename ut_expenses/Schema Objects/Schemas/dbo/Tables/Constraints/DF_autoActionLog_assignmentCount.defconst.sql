ALTER TABLE [dbo].[autoActionLog]
    ADD CONSTRAINT [DF_autoActionLog_assignmentCount] DEFAULT ((0)) FOR [assignmentCount];

