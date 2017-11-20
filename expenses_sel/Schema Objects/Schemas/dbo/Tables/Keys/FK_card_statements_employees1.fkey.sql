ALTER TABLE [dbo].[card_statements_base]
    ADD CONSTRAINT [FK_card_statements_employees1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE SET NULL ON UPDATE NO ACTION;

