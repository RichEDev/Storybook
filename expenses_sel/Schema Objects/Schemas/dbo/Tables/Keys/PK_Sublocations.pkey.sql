﻿ALTER TABLE [dbo].[sublocations]
    ADD CONSTRAINT [PK_Sublocations] PRIMARY KEY CLUSTERED ([Sublocation Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

