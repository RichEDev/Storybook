ALTER TABLE [dbo].[workflowsApproval]
    ADD CONSTRAINT [FK_workflowsApproval_workflows] FOREIGN KEY ([workflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE CASCADE ON UPDATE NO ACTION;

