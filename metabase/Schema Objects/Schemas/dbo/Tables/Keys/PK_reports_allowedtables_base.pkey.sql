﻿ALTER TABLE [dbo].[reports_allowedtables_base]
    ADD CONSTRAINT [PK_reports_allowedtables_base] PRIMARY KEY CLUSTERED ([basetableid] ASC, [tableid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

