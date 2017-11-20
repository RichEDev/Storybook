ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_mileageprev] DEFAULT (0) FOR [mileageprev];

