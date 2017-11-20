ALTER TABLE [dbo].[contract_productdetails_recharge]
    ADD CONSTRAINT [DF_Contract_productdetails_Recharge_OneOffCharge] DEFAULT ((0)) FOR [oneOffCharge];

