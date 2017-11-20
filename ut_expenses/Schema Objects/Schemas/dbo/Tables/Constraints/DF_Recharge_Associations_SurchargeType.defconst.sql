ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_SurchargeType] DEFAULT ((0)) FOR [surchargeType];

