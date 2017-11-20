ALTER TABLE [dbo].[customEntityFormFields]
    ADD CONSTRAINT [FK_customEntityFormFields_customEntityAttributes] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

