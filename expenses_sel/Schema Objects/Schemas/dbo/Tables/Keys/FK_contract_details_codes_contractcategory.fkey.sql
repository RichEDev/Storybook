ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_codes_contractcategory] FOREIGN KEY ([categoryId]) REFERENCES [dbo].[codes_contractcategory] ([categoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

