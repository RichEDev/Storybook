ALTER TABLE [dbo].[contract_productdetails_recharge]
    ADD CONSTRAINT [DF_Contract_productdetails_Recharge_rechargeAmount] DEFAULT ((0)) FOR [rechargeAmount];

