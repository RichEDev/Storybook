ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_PostWarrantyApportionId] DEFAULT ((0)) FOR [postWarrantyApportionId];

