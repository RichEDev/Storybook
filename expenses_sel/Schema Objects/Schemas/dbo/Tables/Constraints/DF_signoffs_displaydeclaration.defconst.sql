ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_displaydeclaration] DEFAULT ((0)) FOR [displaydeclaration];

