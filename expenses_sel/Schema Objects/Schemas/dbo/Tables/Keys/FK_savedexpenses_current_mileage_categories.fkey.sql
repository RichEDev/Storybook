ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_mileage_categories] FOREIGN KEY ([mileageid]) REFERENCES [dbo].[mileage_categories] ([mileageid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

