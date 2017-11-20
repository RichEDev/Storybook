ALTER TABLE [dbo].[categories]
    ADD CONSTRAINT [FK_categories_categories] FOREIGN KEY ([categoryid]) REFERENCES [dbo].[categories] ([categoryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

