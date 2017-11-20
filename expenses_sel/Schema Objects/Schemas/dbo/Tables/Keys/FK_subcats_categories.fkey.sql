ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [FK_subcats_categories] FOREIGN KEY ([categoryid]) REFERENCES [dbo].[categories] ([categoryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

