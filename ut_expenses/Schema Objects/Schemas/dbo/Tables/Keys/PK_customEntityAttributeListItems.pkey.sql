﻿ALTER TABLE [dbo].[customEntityAttributeListItems]
    ADD CONSTRAINT [PK_customEntityAttributeListItems] PRIMARY KEY CLUSTERED ([attributeid] ASC, [item] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

