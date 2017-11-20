ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_workflowUpdate] DEFAULT ((0)) FOR [workflowUpdate];

