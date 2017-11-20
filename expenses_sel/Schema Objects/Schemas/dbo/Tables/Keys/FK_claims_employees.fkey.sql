ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [FK_claims_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

