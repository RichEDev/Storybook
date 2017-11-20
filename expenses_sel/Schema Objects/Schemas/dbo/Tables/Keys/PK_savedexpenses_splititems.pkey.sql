ALTER TABLE [dbo].[savedexpenses_splititems]
    ADD CONSTRAINT [PK_savedexpenses_splititems] PRIMARY KEY CLUSTERED ([primaryitem] ASC, [splititem] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

