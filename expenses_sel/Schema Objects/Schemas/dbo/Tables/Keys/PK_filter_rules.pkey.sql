﻿ALTER TABLE [dbo].[filter_rule_values]
    ADD CONSTRAINT [PK_filter_rules] PRIMARY KEY CLUSTERED ([filterruleid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

