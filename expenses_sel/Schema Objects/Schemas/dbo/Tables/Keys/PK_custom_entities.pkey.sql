﻿ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [PK_custom_entities] PRIMARY KEY CLUSTERED ([entityid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

