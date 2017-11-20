ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [FK_mileage_thresholds_mileage_dateranges] FOREIGN KEY ([mileagedateid]) REFERENCES [dbo].[mileage_dateranges] ([mileagedateid]) ON DELETE CASCADE ON UPDATE NO ACTION;

