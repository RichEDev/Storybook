ALTER TABLE [dbo].[claimhistory]
    ADD CONSTRAINT [FK_claimhistory_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE SET NULL;

