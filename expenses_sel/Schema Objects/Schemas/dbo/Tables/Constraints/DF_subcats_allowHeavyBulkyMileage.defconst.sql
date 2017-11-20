ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_allowHeavyBulkyMileage] DEFAULT ((0)) FOR [allowHeavyBulkyMileage];

