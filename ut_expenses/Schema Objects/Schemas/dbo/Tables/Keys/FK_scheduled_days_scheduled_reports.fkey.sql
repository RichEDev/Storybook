ALTER TABLE [dbo].[scheduled_days]
    ADD CONSTRAINT [FK_scheduled_days_scheduled_reports] FOREIGN KEY ([scheduleid]) REFERENCES [dbo].[scheduled_reports] ([scheduleid]) ON DELETE CASCADE ON UPDATE NO ACTION;

