ALTER TABLE [dbo].[contract_productdetails_oneoffcharge]
    ADD CONSTRAINT [DF_contract_productdetails_oneoffcharge_rechargeEntityId] DEFAULT ((0)) FOR [rechargeEntityId];

