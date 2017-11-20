ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_RechargeAmount] DEFAULT ((0)) FOR [rechargeAmount];

