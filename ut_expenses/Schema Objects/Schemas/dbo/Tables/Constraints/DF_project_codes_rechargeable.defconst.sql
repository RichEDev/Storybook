ALTER TABLE [dbo].[project_codes]
    ADD CONSTRAINT [DF_project_codes_rechargeable] DEFAULT (0) FOR [rechargeable];

