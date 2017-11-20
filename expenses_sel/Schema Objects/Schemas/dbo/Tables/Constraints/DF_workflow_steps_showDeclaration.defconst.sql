ALTER TABLE [dbo].[workflowSteps]
    ADD CONSTRAINT [DF_workflow_steps_showDeclaration] DEFAULT ((0)) FOR [showQuestionDialog];

