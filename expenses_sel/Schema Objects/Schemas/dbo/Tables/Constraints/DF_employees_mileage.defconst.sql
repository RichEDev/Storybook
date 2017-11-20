ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_mileage] DEFAULT (0) FOR [mileage];

