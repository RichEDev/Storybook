﻿ALTER TABLE [dbo].[userdefinedDepartments]
    ADD CONSTRAINT [PK_userdefinedDepartments] PRIMARY KEY CLUSTERED ([departmentid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

