ALTER TABLE [dbo].[employeeWorkLocations]
    ADD CONSTRAINT [FK_employeeWorkLocations_companies] FOREIGN KEY ([locationID]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE CASCADE ON UPDATE NO ACTION;

