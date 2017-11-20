ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [FK_customEntityAttributes_workflows] FOREIGN KEY ([workflowid]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE SET NULL ON UPDATE NO ACTION;

