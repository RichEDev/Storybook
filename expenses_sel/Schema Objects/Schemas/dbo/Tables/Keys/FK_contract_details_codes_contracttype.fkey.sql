ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_codes_contracttype] FOREIGN KEY ([contractTypeId]) REFERENCES [dbo].[codes_contracttype] ([contractTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

