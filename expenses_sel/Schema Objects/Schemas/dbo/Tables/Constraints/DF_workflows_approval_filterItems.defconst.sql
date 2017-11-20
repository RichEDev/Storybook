ALTER TABLE [dbo].[workflowsApproval]
    ADD CONSTRAINT [DF_workflows_approval_filterItems] DEFAULT ((0)) FOR [filterItems];

