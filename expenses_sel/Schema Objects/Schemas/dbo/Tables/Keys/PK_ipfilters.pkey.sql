﻿ALTER TABLE [dbo].[ipfilters]
    ADD CONSTRAINT [PK_ipfilters] PRIMARY KEY CLUSTERED ([ipFilterID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

