ALTER TABLE [dbo].[workflowConditions]
    ADD CONSTRAINT [FK_workflowConditions_workflows] FOREIGN KEY ([workflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

