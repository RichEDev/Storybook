ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_workflowSearch] DEFAULT ((0)) FOR [workflowSearch];

