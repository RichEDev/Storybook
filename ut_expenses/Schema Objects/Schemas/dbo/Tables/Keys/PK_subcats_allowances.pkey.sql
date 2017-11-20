﻿ALTER TABLE [dbo].[subcats_allowances]
    ADD CONSTRAINT [PK_subcats_allowances] PRIMARY KEY CLUSTERED ([subcatid] ASC, [allowanceid] ASC) WITH (FILLFACTOR = 90, ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

