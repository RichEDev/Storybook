ALTER TABLE [dbo].[tasks]
    ADD CONSTRAINT [DF__tasks__taskCreat__10A2506C] DEFAULT (getdate()) FOR [taskCreationDate];

