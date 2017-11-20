ALTER TABLE [dbo].[contract_productdetails_oneoffcharge]
    ADD CONSTRAINT [DF_contract_productdetails_oneoffcharge_contractId] DEFAULT ((0)) FOR [contractId];

