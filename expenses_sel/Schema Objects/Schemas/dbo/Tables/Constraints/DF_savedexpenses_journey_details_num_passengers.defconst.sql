ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [DF_savedexpenses_journey_details_num_passengers] DEFAULT ((0)) FOR [num_passengers];

