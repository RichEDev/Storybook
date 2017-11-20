ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_Portion] DEFAULT ((0)) FOR [portion];

