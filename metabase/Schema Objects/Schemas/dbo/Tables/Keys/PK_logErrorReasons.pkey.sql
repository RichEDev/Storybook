﻿ALTER TABLE [dbo].[logErrorReasons]
    ADD CONSTRAINT [PK_logErrorReasons] PRIMARY KEY CLUSTERED ([logReasonID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

