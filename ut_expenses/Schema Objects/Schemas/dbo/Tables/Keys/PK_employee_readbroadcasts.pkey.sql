ALTER TABLE [dbo].[employee_readbroadcasts]
    ADD CONSTRAINT [PK_employee_readbroadcasts] PRIMARY KEY CLUSTERED ([employeeid] ASC, [broadcastid] ASC) WITH (FILLFACTOR = 90, ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

