ALTER TABLE [dbo].[task_history]
    ADD CONSTRAINT [DF__task_hist__dates__779776D7] DEFAULT (getdate()) FOR [datestamp];

