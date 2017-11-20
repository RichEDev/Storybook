ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [FK_reportcriteria_fields_base] FOREIGN KEY ([fieldID]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE CASCADE ON UPDATE NO ACTION;

