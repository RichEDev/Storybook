ALTER TABLE [dbo].[workflowConditions]
    ADD CONSTRAINT [DF_workflow_conditions_replaceElements] DEFAULT ((0)) FOR [replaceElements];

