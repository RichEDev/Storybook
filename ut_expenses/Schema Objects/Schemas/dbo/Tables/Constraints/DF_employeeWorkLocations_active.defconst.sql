ALTER TABLE [dbo].[employeeWorkLocations]
    ADD CONSTRAINT [DF_employeeWorkLocations_active] DEFAULT ((0)) FOR [active];

