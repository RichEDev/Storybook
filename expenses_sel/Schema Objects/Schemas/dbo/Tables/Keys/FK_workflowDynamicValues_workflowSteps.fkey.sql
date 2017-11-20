ALTER TABLE [dbo].[workflowDynamicValues]
    ADD CONSTRAINT [FK_workflowDynamicValues_workflowSteps] FOREIGN KEY ([workflowStepID]) REFERENCES [dbo].[workflowSteps] ([workflowStepID]) ON DELETE CASCADE ON UPDATE CASCADE;

