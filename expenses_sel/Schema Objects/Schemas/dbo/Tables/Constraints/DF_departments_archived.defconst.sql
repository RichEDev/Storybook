ALTER TABLE [dbo].[departments]
    ADD CONSTRAINT [DF_departments_archived] DEFAULT (0) FOR [archived];

