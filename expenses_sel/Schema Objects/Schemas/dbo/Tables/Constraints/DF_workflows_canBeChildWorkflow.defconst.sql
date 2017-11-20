ALTER TABLE [dbo].[workflows]
    ADD CONSTRAINT [DF_workflows_canBeChildWorkflow] DEFAULT ((0)) FOR [canBeChildWorkflow];

