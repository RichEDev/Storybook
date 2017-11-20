ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [PK_savedexpenses_journey_details_1] PRIMARY KEY CLUSTERED ([expenseid] ASC, [step_number] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

