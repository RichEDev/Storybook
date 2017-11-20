ALTER TABLE [dbo].[workflowConditions]
    ADD CONSTRAINT [DF_workflow_conditions_updateCriteria] DEFAULT ((0)) FOR [updateCriteria];

