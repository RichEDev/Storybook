﻿ALTER TABLE [dbo].[customJoinBreakdown]
    ADD CONSTRAINT [PK_customJoinBreakdown] PRIMARY KEY NONCLUSTERED ([joinbreakdownid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
