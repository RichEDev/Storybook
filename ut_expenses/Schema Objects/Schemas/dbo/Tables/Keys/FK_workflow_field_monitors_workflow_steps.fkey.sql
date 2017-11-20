ALTER TABLE [dbo].[workflow_field_monitors]
    ADD CONSTRAINT [FK_workflow_field_monitors_workflow_steps] FOREIGN KEY ([workflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

