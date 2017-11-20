ALTER TABLE [dbo].[location_distances]
    ADD CONSTRAINT [FK_location_distances_companies] FOREIGN KEY ([locationa]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

