ALTER TABLE [dbo].[floathistory]
    ADD CONSTRAINT [FK_floathistory_floats] FOREIGN KEY ([floatid]) REFERENCES [dbo].[floats] ([floatid]) ON DELETE CASCADE ON UPDATE CASCADE;

