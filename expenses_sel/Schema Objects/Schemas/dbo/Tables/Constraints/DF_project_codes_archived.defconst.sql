ALTER TABLE [dbo].[project_codes]
    ADD CONSTRAINT [DF_project_codes_archived] DEFAULT (0) FOR [archived];

