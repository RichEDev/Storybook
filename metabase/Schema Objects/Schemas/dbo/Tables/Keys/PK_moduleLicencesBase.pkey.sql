﻿ALTER TABLE [dbo].[moduleLicencesBase]
    ADD CONSTRAINT [PK_moduleLicencesBase] PRIMARY KEY CLUSTERED ([moduleID] ASC, [accountID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

