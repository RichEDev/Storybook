﻿ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_allowanceamount] DEFAULT ((0)) FOR [allowanceamount];

