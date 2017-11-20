﻿ALTER TABLE [dbo].[car_mileagecats]
    ADD CONSTRAINT [PK_car_mileagecats] PRIMARY KEY CLUSTERED ([carid] ASC, [mileageid] ASC) WITH (FILLFACTOR = 90, ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

