ALTER TABLE [dbo].[workflow_field_monitors]
    ADD CONSTRAINT [DF_workflow_field_monitors_fieldID] DEFAULT ((0)) FOR [fieldID];

