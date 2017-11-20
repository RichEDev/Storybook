ALTER TABLE [dbo].[workflowSteps]
    ADD CONSTRAINT [DF_workflow_steps_parentStepID] DEFAULT ((0)) FOR [parentStepID];

