ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_codes_contractstatus] FOREIGN KEY ([contractStatusId]) REFERENCES [dbo].[codes_contractstatus] ([statusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

