ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_heavyGoods] DEFAULT ((0)) FOR [heavyBulkyEquipment];

