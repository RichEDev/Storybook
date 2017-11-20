ALTER TABLE [dbo].[email_schedule]
    ADD CONSTRAINT [DF_email_schedule_runSubAccountId] DEFAULT ((0)) FOR [runSubAccountId];

