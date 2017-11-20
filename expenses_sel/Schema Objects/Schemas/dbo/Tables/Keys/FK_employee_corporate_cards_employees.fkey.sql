ALTER TABLE [dbo].[employee_corporate_cards]
    ADD CONSTRAINT [FK_employee_corporate_cards_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

