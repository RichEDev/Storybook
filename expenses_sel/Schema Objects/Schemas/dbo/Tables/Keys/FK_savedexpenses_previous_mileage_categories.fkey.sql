ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_mileage_categories] FOREIGN KEY ([mileageid]) REFERENCES [dbo].[mileage_categories] ([mileageid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

