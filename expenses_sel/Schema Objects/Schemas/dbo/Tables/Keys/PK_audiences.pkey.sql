﻿ALTER TABLE [dbo].[audiences]
    ADD CONSTRAINT [PK_audiences] PRIMARY KEY CLUSTERED ([audienceID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

