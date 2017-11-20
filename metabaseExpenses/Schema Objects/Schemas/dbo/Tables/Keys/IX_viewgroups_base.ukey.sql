﻿ALTER TABLE [dbo].[viewgroups_base]
    ADD CONSTRAINT [IX_viewgroups_base] UNIQUE NONCLUSTERED ([parentid] ASC, [groupname] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

