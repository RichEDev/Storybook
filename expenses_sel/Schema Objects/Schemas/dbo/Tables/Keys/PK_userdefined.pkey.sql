﻿ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [PK_userdefined] PRIMARY KEY NONCLUSTERED ([userdefineid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

