ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_PostWarrantyPortion] DEFAULT ((0)) FOR [postWarrantyPortion];

