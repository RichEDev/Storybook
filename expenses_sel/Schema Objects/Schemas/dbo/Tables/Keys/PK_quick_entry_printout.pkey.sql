﻿ALTER TABLE [dbo].[quick_entry_printout]
    ADD CONSTRAINT [PK_quick_entry_printout] PRIMARY KEY CLUSTERED ([quickentryid] ASC, [order] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

