ALTER TABLE [dbo].[task_history]
    ADD CONSTRAINT [FK_tasks_task_history] FOREIGN KEY ([taskId]) REFERENCES [dbo].[tasks] ([taskId]) ON DELETE CASCADE ON UPDATE NO ACTION;

