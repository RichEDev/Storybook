ALTER TABLE [dbo].[audienceEmployees]
    ADD CONSTRAINT [FK_audienceEmployees_audiences] FOREIGN KEY ([audienceID]) REFERENCES [dbo].[audiences] ([audienceID]) ON DELETE CASCADE ON UPDATE CASCADE;

