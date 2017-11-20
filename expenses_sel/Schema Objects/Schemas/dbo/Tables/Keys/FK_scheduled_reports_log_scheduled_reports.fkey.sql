ALTER TABLE [dbo].[scheduled_reports_log]
    ADD CONSTRAINT [FK_scheduled_reports_log_scheduled_reports] FOREIGN KEY ([scheduleid]) REFERENCES [dbo].[scheduled_reports] ([scheduleid]) ON DELETE CASCADE ON UPDATE NO ACTION;

