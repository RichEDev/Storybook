﻿ALTER TABLE [dbo].[codes_accountcodes]
    ADD CONSTRAINT [PK_codes_accountcodes] PRIMARY KEY CLUSTERED ([CodeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

