ALTER TABLE [dbo].[joinViaParts]
    ADD CONSTRAINT [FK_joinViaParts_joinVia] FOREIGN KEY ([joinViaID]) REFERENCES [dbo].[joinVia] ([joinViaID]) ON DELETE CASCADE ON UPDATE NO ACTION;

