﻿ALTER TABLE [dbo].[subcats_userdefined]
    ADD CONSTRAINT [PK_subcats-userdefined] PRIMARY KEY CLUSTERED ([subuserdefineid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

