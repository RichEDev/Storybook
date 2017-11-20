ALTER TABLE [dbo].[codes_rechargeentity]
    ADD CONSTRAINT [FK_codes_rechargeentity_employees2] FOREIGN KEY ([deputyRep]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

