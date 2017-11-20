ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [DF_savedexpenses_journey_steps_exceeded_recommended_mileage] DEFAULT ((0)) FOR [exceeded_recommended_mileage];

