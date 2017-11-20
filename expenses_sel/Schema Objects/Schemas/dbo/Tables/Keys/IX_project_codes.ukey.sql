﻿ALTER TABLE [dbo].[project_codes]
    ADD CONSTRAINT [IX_project_codes] UNIQUE NONCLUSTERED ([projectcode] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

