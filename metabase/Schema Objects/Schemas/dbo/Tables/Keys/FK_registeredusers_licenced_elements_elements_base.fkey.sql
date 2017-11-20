ALTER TABLE [dbo].[accountsLicencedElements]
    ADD CONSTRAINT [FK_registeredusers_licenced_elements_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

