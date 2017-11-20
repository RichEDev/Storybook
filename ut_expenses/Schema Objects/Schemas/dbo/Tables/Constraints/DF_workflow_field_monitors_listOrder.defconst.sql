ALTER TABLE [dbo].[workflow_field_monitors]
    ADD CONSTRAINT [DF_workflow_field_monitors_listOrder] DEFAULT ((0)) FOR [listOrder];

