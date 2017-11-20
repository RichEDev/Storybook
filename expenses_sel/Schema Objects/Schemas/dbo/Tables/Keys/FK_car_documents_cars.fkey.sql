ALTER TABLE [dbo].[car_documents]
    ADD CONSTRAINT [FK_car_documents_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE NO ACTION ON UPDATE CASCADE;

