ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [DF_savedexpenses_journey_steps_numCalculatedMiles] DEFAULT ((0)) FOR [numActualMiles];

