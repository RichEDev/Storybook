﻿ALTER TABLE [dbo].[defineditems]
    ADD CONSTRAINT [PK_defineditems] PRIMARY KEY NONCLUSTERED ([itemid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

