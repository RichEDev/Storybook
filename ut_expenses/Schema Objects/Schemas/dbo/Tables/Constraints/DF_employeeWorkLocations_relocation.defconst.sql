ALTER TABLE [dbo].[employeeWorkLocations]
    ADD CONSTRAINT [DF_employeeWorkLocations_relocation] DEFAULT ((0)) FOR [temporary];

