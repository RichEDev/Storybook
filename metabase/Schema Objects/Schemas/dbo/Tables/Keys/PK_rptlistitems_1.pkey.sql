﻿ALTER TABLE [dbo].[rptlistitems]
    ADD CONSTRAINT [PK_rptlistitems_1] PRIMARY KEY CLUSTERED ([listitem] ASC, [fieldid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

