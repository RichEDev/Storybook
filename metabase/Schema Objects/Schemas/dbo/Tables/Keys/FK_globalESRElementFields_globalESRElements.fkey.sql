ALTER TABLE [dbo].[globalESRElementFields]
    ADD CONSTRAINT [FK_globalESRElementFields_globalESRElements] FOREIGN KEY ([globalESRElementID]) REFERENCES [dbo].[globalESRElements] ([globalESRElementID]) ON DELETE CASCADE ON UPDATE CASCADE;

