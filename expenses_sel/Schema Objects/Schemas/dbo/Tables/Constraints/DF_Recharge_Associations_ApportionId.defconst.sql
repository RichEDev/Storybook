ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_ApportionId] DEFAULT ((0)) FOR [apportionId];

