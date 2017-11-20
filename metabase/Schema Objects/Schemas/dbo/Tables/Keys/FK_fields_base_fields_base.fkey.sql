ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [FK_fields_base_fields_base] FOREIGN KEY ([lookupfield]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

