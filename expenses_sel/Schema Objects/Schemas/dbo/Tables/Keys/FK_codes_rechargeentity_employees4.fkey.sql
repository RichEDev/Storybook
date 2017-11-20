ALTER TABLE [dbo].[codes_rechargeentity]
    ADD CONSTRAINT [FK_codes_rechargeentity_employees4] FOREIGN KEY ([serviceMgr]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

