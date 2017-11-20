ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [FK_signoffs_groups] FOREIGN KEY ([groupid]) REFERENCES [dbo].[groups] ([groupid]) ON DELETE CASCADE ON UPDATE CASCADE;

