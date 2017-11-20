ALTER TABLE [dbo].[workflowConditions]
    ADD CONSTRAINT [FK_workflow_conditions_workflow_conditions] FOREIGN KEY ([workflowStepID]) REFERENCES [dbo].[workflowSteps] ([workflowStepID]) ON DELETE CASCADE ON UPDATE CASCADE;

