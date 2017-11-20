ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [FK_registeredusers_hostnames] FOREIGN KEY ([hostnameID]) REFERENCES [dbo].[hostnames] ([hostnameID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

