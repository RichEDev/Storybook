ALTER TABLE [dbo].[email_schedule]
    ADD CONSTRAINT [DF_Email_Schedule_EmailType] DEFAULT ((0)) FOR [emailType];

