ALTER TABLE [dbo].[tasks]
    ADD CONSTRAINT [FK_codes_tasktypes_tasks] FOREIGN KEY ([taskTypeId]) REFERENCES [dbo].[codes_tasktypes] ([typeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

