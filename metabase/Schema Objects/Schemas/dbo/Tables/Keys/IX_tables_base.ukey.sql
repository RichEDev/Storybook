﻿ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [IX_tables_base] UNIQUE NONCLUSTERED ([tablename] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

