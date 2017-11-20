ALTER TABLE [dbo].[accountsLicencedElements]
    ADD CONSTRAINT [FK_registeredusers_licenced_elements_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE ON UPDATE NO ACTION;

