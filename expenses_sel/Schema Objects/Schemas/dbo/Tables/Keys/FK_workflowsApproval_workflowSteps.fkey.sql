ALTER TABLE [dbo].[workflowsApproval]
    ADD CONSTRAINT [FK_workflowsApproval_workflowSteps] FOREIGN KEY ([workflowStepID]) REFERENCES [dbo].[workflowSteps] ([workflowStepID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

