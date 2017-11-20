ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [DF_Recharge_Associations_Surcharge] DEFAULT ((0)) FOR [surcharge];

