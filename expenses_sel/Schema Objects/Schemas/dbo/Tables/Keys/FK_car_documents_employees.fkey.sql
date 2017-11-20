ALTER TABLE [dbo].[car_documents]
    ADD CONSTRAINT [FK_car_documents_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

