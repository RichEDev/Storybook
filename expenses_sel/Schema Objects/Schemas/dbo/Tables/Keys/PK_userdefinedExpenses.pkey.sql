﻿ALTER TABLE [dbo].[userdefinedExpenses]
    ADD CONSTRAINT [PK_userdefinedExpenses] PRIMARY KEY CLUSTERED ([expenseid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

