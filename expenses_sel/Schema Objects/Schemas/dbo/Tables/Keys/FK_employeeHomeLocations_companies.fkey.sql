ALTER TABLE [dbo].[employeeHomeLocations]
    ADD CONSTRAINT [FK_employeeHomeLocations_companies] FOREIGN KEY ([locationID]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE CASCADE ON UPDATE NO ACTION;

