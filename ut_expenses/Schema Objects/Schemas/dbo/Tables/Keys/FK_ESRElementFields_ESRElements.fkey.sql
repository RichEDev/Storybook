ALTER TABLE [dbo].[ESRElementFields]
    ADD CONSTRAINT [FK_ESRElementFields_ESRElements] FOREIGN KEY ([elementID]) REFERENCES [dbo].[ESRElements] ([elementID]) ON DELETE CASCADE ON UPDATE NO ACTION;

