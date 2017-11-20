ALTER TABLE [dbo].[workflowsApproval]
    ADD CONSTRAINT [FK_workflows_approval_workflows_approval] FOREIGN KEY ([emailTemplateID]) REFERENCES [dbo].[emailTemplates] ([emailtemplateid]) ON DELETE CASCADE ON UPDATE CASCADE;

