ALTER TABLE [dbo].[scheduled_months]
    ADD CONSTRAINT [FK_scheduled_months_scheduled_reports] FOREIGN KEY ([scheduleid]) REFERENCES [dbo].[scheduled_reports] ([scheduleid]) ON DELETE CASCADE ON UPDATE NO ACTION;

