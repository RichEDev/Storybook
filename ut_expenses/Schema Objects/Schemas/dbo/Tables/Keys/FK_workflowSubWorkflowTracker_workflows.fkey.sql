ALTER TABLE [dbo].[workflowSubWorkflowTracker]
    ADD CONSTRAINT [FK_workflowSubWorkflowTracker_workflows] FOREIGN KEY ([primaryWorkflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE CASCADE ON UPDATE NO ACTION;

