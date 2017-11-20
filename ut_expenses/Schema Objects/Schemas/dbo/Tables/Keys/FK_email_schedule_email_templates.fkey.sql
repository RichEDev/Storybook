ALTER TABLE [dbo].[email_schedule]
    ADD CONSTRAINT [FK_email_schedule_email_templates] FOREIGN KEY ([templateId]) REFERENCES [dbo].[email_templates] ([templateId]) ON DELETE CASCADE ON UPDATE NO ACTION;

