ALTER TABLE [dbo].[exporthistory]
    ADD CONSTRAINT [DF_exporthistory_exportStatus] DEFAULT ((0)) FOR [exportStatus];

