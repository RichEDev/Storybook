ALTER TABLE [dbo].[workflowSubWorkflowTracker]
    ADD CONSTRAINT [FK_workflowSubWorkflowTracker_workflows1] FOREIGN KEY ([secondaryWorkflowID]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

