ALTER TABLE [dbo].[exporteditems]
    ADD CONSTRAINT [FK_exporteditems_exporthistory] FOREIGN KEY ([exporthistoryid]) REFERENCES [dbo].[exporthistory] ([exporthistoryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

