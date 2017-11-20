ALTER TABLE [dbo].[userdefinedLocations]
    ADD CONSTRAINT [FK_userdefinedLocations_companies] FOREIGN KEY ([locationid]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE CASCADE ON UPDATE NO ACTION;

