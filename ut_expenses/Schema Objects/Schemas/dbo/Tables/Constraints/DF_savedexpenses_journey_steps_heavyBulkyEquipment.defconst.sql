ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [DF_savedexpenses_journey_steps_heavyBulkyEquipment] DEFAULT ((0)) FOR [heavyBulkyEquipment];

