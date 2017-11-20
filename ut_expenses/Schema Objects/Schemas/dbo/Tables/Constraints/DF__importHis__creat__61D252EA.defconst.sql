ALTER TABLE [dbo].[importHistory]
    ADD CONSTRAINT [DF__importHis__creat__61D252EA] DEFAULT (getdate()) FOR [createdOn];

