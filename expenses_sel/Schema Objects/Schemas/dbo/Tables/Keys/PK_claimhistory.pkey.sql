﻿ALTER TABLE [dbo].[claimhistory]
    ADD CONSTRAINT [PK_claimhistory] PRIMARY KEY CLUSTERED ([claimhistoryid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

