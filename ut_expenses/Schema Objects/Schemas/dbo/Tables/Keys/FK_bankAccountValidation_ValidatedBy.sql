﻿ALTER TABLE [dbo].[bankAccountValidation]
    ADD CONSTRAINT [FK_bankAccountValidation_ValidatedBy] FOREIGN KEY ([ValidatedBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;