ALTER TABLE [dbo].[cardrecords]
    ADD CONSTRAINT [FK_cardrecords_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

