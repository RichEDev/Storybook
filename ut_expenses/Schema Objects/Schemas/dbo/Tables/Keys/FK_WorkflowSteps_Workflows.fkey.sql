ALTER TABLE [dbo].[workflowSteps]
    ADD CONSTRAINT [FK_WorkflowSteps_Workflows] FOREIGN KEY ([workflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE CASCADE ON UPDATE CASCADE;

