﻿ALTER TABLE [dbo].[scheduled_days]
    ADD CONSTRAINT [PK_scheduled_days] PRIMARY KEY CLUSTERED ([scheduleid] ASC, [day] ASC) WITH (FILLFACTOR = 90, ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

